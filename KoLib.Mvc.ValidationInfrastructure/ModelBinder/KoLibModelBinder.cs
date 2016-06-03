using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Ko.Utils.Extensions;
using KoLib.Mvc.ValidationInfrastructure.Contracts;
using KoLib.Mvc.ValidationInfrastructure.Helpers;
using KoLib.Mvc.ValidationInfrastructure.ModelBinder.Resources;

namespace KoLib.Mvc.ValidationInfrastructure.ModelBinder
{
    public class KoLibModelBinder : DefaultModelBinder
    {
        #region Validation Tree support

        /// <summary>
        /// Converts given binding context to custom binding context
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        private static CustomModelBindingContext GetCustomModelBindingContext(ModelBindingContext bindingContext)
        {
            var customContext =
                bindingContext as CustomModelBindingContext
                ?? new CustomModelBindingContext
                       {
                           ModelState = bindingContext.ModelState,
                           FallbackToEmptyPrefix = bindingContext.FallbackToEmptyPrefix,
                           ModelMetadata = bindingContext.ModelMetadata,
                           ModelName = bindingContext.ModelName,
                           PropertyFilter = bindingContext.PropertyFilter,
                           ValueProvider = bindingContext.ValueProvider
                       };

            return customContext;
        }

        /// <summary>
        /// Creates a new custom binding context
        /// </summary>
        /// <param name="bindingContext">
        /// Current binding context
        /// </param>
        /// <param name="model">
        /// Current value of model to be bound
        /// </param>
        /// <param name="propertyFilter">
        /// New property filter
        /// </param>
        /// <param name="modelType">
        /// Type of current model
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// The main task of this method is to clone current binding context with new ModelMetadata and PropertyFilter
        /// </remarks>
        private static CustomModelBindingContext CreateCustomModelBindingContext(ModelBindingContext bindingContext, object model, Predicate<string> propertyFilter = null, Type modelType = null)
        {
            var customContext = GetCustomModelBindingContext(bindingContext);

            var newBindingContext = new CustomModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType ?? customContext.ModelType),
                ModelName = customContext.ModelName,
                ModelState = customContext.ModelState,
                PropertyFilter = propertyFilter ?? customContext.PropertyFilter,
                ValueProvider = customContext.ValueProvider,
                RebindingIndicator = customContext.RebindingIndicator,
                InapplicableProperties = customContext.InapplicableProperties,
                LockedReadOnlyProperties = customContext.LockedReadOnlyProperties,
                ComputedReadOnlyProperties = customContext.ComputedReadOnlyProperties,
                IncludedProperties = customContext.IncludedProperties,
                PropertyOriginalValues = customContext.PropertyOriginalValues,
                OriginalModel = customContext.OriginalModel,
                OriginalModelCompleted = customContext.OriginalModelCompleted
            };

            return newBindingContext;
        }

        /// <summary>
        /// Wraps IValidationModel.GetLockedReadOnlyProperties
        /// </summary>
        /// <param name="model">
        /// Value of current model
        /// </param>
        /// <returns></returns>
        private static List<PropertyExpression> GetLockedReadOnlyProperties(object model)
        {
            var result = new List<PropertyExpression>();

            var validationModel = model as IValidationModel;

            if (validationModel != null)
            {
                result = validationModel.GetLockedReadOnlyProperties() ?? result;
            }

            return result;
        }

        /// <summary>
        /// Wraps IValidationModel.GetInapplicableProperties
        /// </summary>
        /// <param name="model">
        /// Value of current model
        /// </param>
        /// <returns></returns>
        private static List<PropertyExpression> GetInapplicableProperties(object model)
        {
            var result = new List<PropertyExpression>();

            var validationModel = model as IValidationModel;

            if (validationModel != null)
            {
                result = validationModel.GetInapplicableProperties() ?? result;
            }

            return result;
        }

        /// <summary>
        /// Wraps IValidationModel.GetComputedReadOnlyProperties
        /// </summary>
        /// <param name="model">
        /// Value of current model
        /// </param>
        /// <returns></returns>
        private static Dictionary<PropertyExpression, PropertyExpression> GetComputedReadOnlyProperties(object model)
        {
            var result = new Dictionary<PropertyExpression, PropertyExpression>();

            var validationModel = model as IValidationModel;

            if (validationModel != null)
            {
                result = validationModel.GetComputedReadOnlyProperties() ?? result;
            }

            return result;
        }

        /// <summary>
        /// Converts list of propety expressions into list of property names
        /// </summary>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        private static List<string> GetStringNameProperties(List<PropertyExpression> propertyExpressions)
        {
            var result = new List<string>();

            if (propertyExpressions != null)
            {
                result = propertyExpressions.Select(x => ExpressionHelper.GetExpressionText(x.LambdaExpression)).ToList();
            }

            return result;
        }

        /// <summary>
        /// Defines default value of given model
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        private static object GetTypeDefaultValue(Type objType)
        {
            if (objType==null)
                throw new ArgumentNullException("objType");
            if (objType.IsAtomType() || objType.IsNullableType())
                return objType.Default();
            return Activator.CreateInstance(objType);
        }

        /// <summary>
        /// Sets value of inapplicable properties inside current model to default value
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<string> ResetInapplicableProperties(object model)
        {
            var ignoredList = new List<string>();

            var inapplicableProperties = GetInapplicableProperties(model);
            if (inapplicableProperties.Count > 0)
            {
                foreach (var inapplicableProperty in inapplicableProperties)
                {
                    var propertyInfo = inapplicableProperty.LambdaExpression.AsPropertyInfo();

                    var defaultValueAttribute =
                        propertyInfo.GetCustomAttributes(typeof (DefaultValueAttribute), true)
                            .FirstOrDefault() as DefaultValueAttribute;
                    
                    var defaultValue = defaultValueAttribute != null
                                           ? defaultValueAttribute.Value
                                           : GetTypeDefaultValue(propertyInfo.PropertyType);
                    
                    inapplicableProperty.SetValue(model, defaultValue);
                }

                ignoredList = GetStringNameProperties(inapplicableProperties);
            }

            return ignoredList;
        }

        /// <summary>
        /// Sets value of computed read only properties inside current model to value at loading time/or value of a get-only property rather than post value.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="originalValues"></param>
        /// <returns></returns>
        private static List<string> ComputeReadOnlyProperties(object model, Dictionary<string, object> originalValues)
        {
            var ignoredList = new List<string>();
            var computedReadOnlyProperties = GetComputedReadOnlyProperties(model);

            if (computedReadOnlyProperties.Count > 0)
            {
                foreach (var item in computedReadOnlyProperties)
                {
                    var property = item.Key;
                    var propertyName = ExpressionHelper.GetExpressionText(property.LambdaExpression);
                    if (item.Value != null)
                    {
                        var computedValue = item.Value.GetValue(model);
                        property.SetValue(model, computedValue);
                    }
                    else if (originalValues.ContainsKey(propertyName))
                    {
                        property.SetValue(model, originalValues[propertyName]);
                    }
                }

                var temp = GetStringNameProperties(new List<PropertyExpression>(computedReadOnlyProperties.Keys));

                ignoredList.AddRange(temp);
            }

            return ignoredList;
        }

        #endregion 

        #region Overloads of DefaultModelBinder

        protected virtual object BindModel(
            ControllerContext controllerContext,
            CustomModelBindingContext customContext)
        {
            var performedFallback = false;

            if (!String.IsNullOrEmpty(customContext.ModelName) &&
                !customContext.ValueProvider.ContainsPrefix(customContext.ModelName))
            {
                // We couldn't find any entry that began with the prefix. If this is the top-level element, fall back
                // to the empty prefix.
                if (customContext.FallbackToEmptyPrefix)
                {
                    customContext = new CustomModelBindingContext
                    {
                        ModelMetadata = customContext.ModelMetadata,
                        ModelState = customContext.ModelState,
                        PropertyFilter = customContext.PropertyFilter,
                        ValueProvider = customContext.ValueProvider,
                        IncludedProperties = customContext.IncludedProperties
                    };

                    performedFallback = true;
                }
                else
                {
                    return null;
                }
            }

            // Simple model = int, string, etc.; determined by calling TypeConverter.CanConvertFrom(typeof(string))
            // or by seeing if a value in the request exactly matches the name of the model we're binding.
            // Complex type = everything else.
            if (!performedFallback)
            {
                var performRequestValidation = ShouldPerformRequestValidation(controllerContext, customContext);
                var uvalueProvider = (customContext.ValueProvider as IUnvalidatedValueProvider) ??
                                     new UnvalidatedValueProviderWrapper(
                                         customContext.ValueProvider);
                var vpResult = uvalueProvider.GetValue(customContext.ModelName,
                                                       !performRequestValidation);
                if (vpResult != null)
                {
                    if (!customContext.PropertyOriginalValues.ContainsKey(customContext.ModelName))
                    {
                        customContext.PropertyOriginalValues.Add(customContext.ModelName, customContext.Model);
                    }
                    return BindSimpleModel(controllerContext, customContext, vpResult);
                }
            }

            if (!customContext.ModelMetadata.IsComplexType)
            {
                return null;
            }

            //Gets all static/locked read-only properties' names if any
            var ignoredProperties = GetLockedReadOnlyProperties(customContext.Model)
                .Select(x => ExpressionHelper.GetExpressionText(x.LambdaExpression))
                .ToList();

            if (ignoredProperties.Count > 0)
            {
                customContext.LockedReadOnlyProperties.AddRange(ignoredProperties);
            }
            //Checks if orginal model is populated completedly
            var conttainsModel = customContext.PropertyOriginalValues.ContainsKey(customContext.ModelName);
            if (conttainsModel)
            {
                var orginalModel = customContext.PropertyOriginalValues[customContext.ModelName];
                customContext.OriginalModel = orginalModel;
                customContext.OriginalModelCompleted = true;
            }
            var xmodel = BindComplexModel(controllerContext, customContext);
            if (!conttainsModel && !string.IsNullOrWhiteSpace(customContext.ModelName))
            {
                //Stores orginal model to the list of orginal values
                customContext.PropertyOriginalValues[customContext.ModelName] = customContext.OriginalModel;
                customContext.OriginalModelCompleted = true;
            }
            return xmodel;
        }

        protected virtual void BindProperty(
            ControllerContext controllerContext,
            CustomModelBindingContext customContext,
            PropertyDescriptor propertyDescriptor)
        {
            // need to skip properties that aren't part of the request, else we might hit a StackOverflowException
            var fullPropertyKey = CreateSubPropertyName(customContext.ModelName, propertyDescriptor.Name);
            if (!customContext.ValueProvider.ContainsPrefix(fullPropertyKey))
            {
                return;
            }

            // call into the property's model binder
            var propertyBinder = Binders.GetBinder(propertyDescriptor.PropertyType);
            var originalPropertyValue = propertyDescriptor.GetValue(customContext.Model);
            
            var propertyMetadata = customContext.PropertyMetadata[propertyDescriptor.Name];
            propertyMetadata.Model = originalPropertyValue;

            var prefix = string.Format("{0}.", propertyDescriptor.Name);

            //Removes prefix for properties on list of inapplicable, read-only, or validatable
            Func<List<string>, List<string>> removePrefix = i => i.Where(x => x.StartsWith(prefix))
                                                                     .Select(x => x.Remove(0, prefix.Length))
                                                                     .Where(x => !string.IsNullOrWhiteSpace(x))
                                                                     .ToList();

            var inapplicableProperties = removePrefix(customContext.InapplicableProperties);

            var lockedReadOnlyProperties = removePrefix(customContext.LockedReadOnlyProperties);

            var computedReadOnlyProperties = removePrefix(customContext.ComputedReadOnlyProperties);

            var includedProperties = removePrefix(customContext.IncludedProperties);

            var innerBindingContext = new CustomModelBindingContext
                                          {
                                              ModelMetadata = propertyMetadata,
                                              ModelState = customContext.ModelState,
                                              ModelName = fullPropertyKey,
                                              ValueProvider = customContext.ValueProvider,
                                              InapplicableProperties = inapplicableProperties,
                                              LockedReadOnlyProperties = lockedReadOnlyProperties,
                                              ComputedReadOnlyProperties = computedReadOnlyProperties,
                                              IncludedProperties = includedProperties,
                                              RebindingIndicator = false,
                                              PropertyOriginalValues = customContext.PropertyOriginalValues
                                          };


            var newPropertyValue = GetPropertyValue(controllerContext, innerBindingContext, propertyDescriptor,
                                                    propertyBinder);
            propertyMetadata.Model = newPropertyValue;

            // Try to set a value into the property unless we know it will fail 
            // (read-only properties)
            if (!propertyDescriptor.IsReadOnly && !customContext.OriginalModelCompleted)
            {
                propertyDescriptor.SetValue(customContext.OriginalModel, innerBindingContext.OriginalModel);
            }

            // validation
            var modelState = customContext.ModelState[fullPropertyKey];
            if (modelState == null || modelState.Errors.Count == 0)
            {
                if (OnPropertyValidating(controllerContext, customContext, propertyDescriptor, newPropertyValue))
                {
                    SetProperty(controllerContext, customContext, propertyDescriptor, newPropertyValue);
                    OnPropertyValidated(controllerContext, customContext, propertyDescriptor, newPropertyValue);
                }
            }
            else
            {
                SetProperty(controllerContext, customContext, propertyDescriptor, newPropertyValue);

                // Convert FormatExceptions (type conversion failures) into InvalidValue messages
                var temp =
                    modelState.Errors
                        .Where(err => String.IsNullOrEmpty(err.ErrorMessage) && err.Exception != null)
                        .ToList();

                foreach (var error in temp)
                {
                    for (var exception = error.Exception; exception != null; exception = exception.InnerException)
                    {
                        if (exception is FormatException)
                        {
                            var displayName = propertyMetadata.GetDisplayName();
                            var errorMessageTemplate = GetValueInvalidResource(controllerContext);
                            var errorMessage = string.Format(CultureInfo.CurrentCulture, errorMessageTemplate,
                                                             modelState.Value.AttemptedValue, displayName);
                            modelState.Errors.Remove(error);
                            modelState.Errors.Add(errorMessage);
                            break;
                        }
                    }
                }
            }
        }

        protected virtual void OnModelUpdated(
            ControllerContext controllerContext,
            CustomModelBindingContext customContext)
        {
            if (customContext.Model is IValidationModel && !customContext.RebindingIndicator)
            {
                var ignoredList = ResetInapplicableProperties(customContext.Model);
                if (ignoredList.Count > 0)
                {
                    ignoredList.RemoveAll(x => customContext.InapplicableProperties.Contains(x));
                    customContext.InapplicableProperties.AddRange(ignoredList);
                }

                var temp = ComputeReadOnlyProperties(customContext.Model, customContext.PropertyOriginalValues);

                if (temp.Count > 0)
                {
                    ignoredList.AddRange(temp);
                }
                customContext.RebindingIndicator = true;

                ignoredList.RemoveAll(x => customContext.LockedReadOnlyProperties.Contains(x));
                if (ignoredList.Count > 0)
                {
                    customContext.LockedReadOnlyProperties.AddRange(ignoredList);
                }

                if (customContext.IsRoot())
                {
                    customContext.ModelState.Clear();
                }

                BindModel(controllerContext, customContext);
                return;
            }

            //Normalize view model (calculate computed properties)
            var model = customContext.Model as INormalizableModel;
            if (model != null)
            {
                model.Normalize();
            }

            BaseOnModelUpdated(controllerContext, customContext);
        }

        /// <summary>
        /// Validates the data of updated model
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        protected virtual void BaseOnModelUpdated(ControllerContext controllerContext, CustomModelBindingContext bindingContext)
        {
            var startedValid = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            foreach (var validationResult in ModelValidator.GetModelValidator(bindingContext.ModelMetadata, controllerContext).Validate(null))
            {
                var subPropertyName = CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName);

                //Checks if property is validate-able
                if (!bindingContext.ValidatablePropertyFilter(validationResult.MemberName))
                {
                    if (bindingContext.ModelState.ContainsKey(subPropertyName))
                    {
                        bindingContext.ModelState.Remove(subPropertyName);
                    }

                    continue;
                }

                if (!startedValid.ContainsKey(subPropertyName))
                {
                    startedValid[subPropertyName] = bindingContext.ModelState.IsValidField(subPropertyName);
                }

                if (startedValid[subPropertyName])
                {
                    bindingContext.ModelState.AddModelError(subPropertyName, validationResult.Message);
                }
            }
        }

        #endregion

        #region Overrides of DefaultModelBinder

        public override object BindModel(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }
            var customContext = GetCustomModelBindingContext(bindingContext);

            return BindModel(controllerContext, customContext);
        }

        protected override void BindProperty(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            var customContext = GetCustomModelBindingContext(bindingContext);

            BindProperty(controllerContext, customContext, propertyDescriptor);
        }

        protected override void OnModelUpdated(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            var customContext = GetCustomModelBindingContext(bindingContext);

            OnModelUpdated(controllerContext, customContext);
        }

        #endregion

        #region Support

        internal object BindSimpleModel(ControllerContext controllerContext, ModelBindingContext bindingContext, ValueProviderResult valueProviderResult)
        {
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            // if the value provider returns an instance of the requested data type, we can just short-circuit
            // the evaluation and return that instance
            if (bindingContext.ModelType.IsInstanceOfType(valueProviderResult.RawValue))
            {
                return valueProviderResult.RawValue;
            }

            // since a string is an IEnumerable<char>, we want it to skip the two checks immediately following
            if (bindingContext.ModelType != typeof(string))
            {

                // conversion results in 3 cases, as below
                if (bindingContext.ModelType.IsArray)
                {
                    // case 1: user asked for an array
                    // ValueProviderResult.ConvertTo() understands array types, so pass in the array type directly
                    object modelArray = ConvertProviderResult(bindingContext.ModelState, bindingContext.ModelName, valueProviderResult, bindingContext.ModelType);
                    return modelArray;
                }

                Type enumerableType = TypeHelpers.ExtractGenericInterface(bindingContext.ModelType, typeof(IEnumerable<>));
                if (enumerableType != null)
                {
                    // case 2: user asked for a collection rather than an array
                    // need to call ConvertTo() on the array type, then copy the array to the collection
                    object modelCollection = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
                    Type elementType = enumerableType.GetGenericArguments()[0];
                    Type arrayType = elementType.MakeArrayType();
                    object modelArray = ConvertProviderResult(bindingContext.ModelState, bindingContext.ModelName, valueProviderResult, arrayType);

                    Type collectionType = typeof(ICollection<>).MakeGenericType(elementType);
                    if (collectionType.IsInstanceOfType(modelCollection))
                    {
                        CollectionHelpers.ReplaceCollection(elementType, modelCollection, modelArray);
                    }
                    return modelCollection;
                }
            }

            // case 3: user asked for an individual element
            object model = ConvertProviderResult(bindingContext.ModelState, bindingContext.ModelName, valueProviderResult, bindingContext.ModelType);
            return model;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.Web.Mvc.ValueProviderResult.ConvertTo(System.Type)", Justification = "The target object should make the correct culture determination, not this method.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We're recording this exception so that we can act on it later.")]
        private static object ConvertProviderResult(ModelStateDictionary modelState, string modelStateKey, ValueProviderResult valueProviderResult, Type destinationType)
        {
            try
            {
                object convertedValue = valueProviderResult.ConvertTo(destinationType);
                return convertedValue;
            }
            catch (Exception ex)
            {
                modelState.AddModelError(modelStateKey, ex);
                return null;
            }
        }

        private static void AddValueRequiredMessageToModelState(ControllerContext controllerContext,
                                                                ModelStateDictionary modelState, string modelStateKey,
                                                                Type elementType, object value)
        {
            if (value == null && !TypeHelpers.TypeAllowsNullValue(elementType) && modelState.IsValidField(modelStateKey))
            {
                modelState.AddModelError(modelStateKey, GetValueRequiredResource(controllerContext));
            }
        }

        internal void BindComplexElementalModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                                object model)
        {
            // need to replace the property filter + model object and create an inner binding context
            var newBindingContext = CreateComplexElementalModelBindingContext(controllerContext,
                                                                                              bindingContext, model);

            // validation
            if (OnModelUpdating(controllerContext, newBindingContext))
            {
                BindProperties(controllerContext, newBindingContext);
                OnModelUpdated(controllerContext, newBindingContext);
            }
        }

        internal object BindComplexModel(ControllerContext controllerContext, CustomModelBindingContext bindingContext)
        {
            var model = bindingContext.Model;
            var modelType = bindingContext.ModelType;

            // if we're being asked to create an array, create a list instead, then coerce to an array after the list is created
            if (model == null && modelType.IsArray)
            {
                var elementType = modelType.GetElementType();
                var listType = typeof (List<>).MakeGenericType(elementType);
                var collection = CreateModel(controllerContext, bindingContext, listType);

                var arrayBindingContext = CreateCustomModelBindingContext(bindingContext, collection,
                                                                          modelType: listType);

                var list = (IList) UpdateCollection(controllerContext, arrayBindingContext, elementType);

                if (list == null)
                {
                    return null;
                }

                var array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);

                bindingContext.OriginalModel = Array.CreateInstance(elementType, 0);

                return array;
            }

            if (model == null)
            {
                model = CreateModel(controllerContext, bindingContext, modelType); 
            }
            
            // special-case IDictionary<,> and ICollection<>
            var dictionaryType = TypeHelpers.ExtractGenericInterface(modelType, typeof(IDictionary<,>));
            if (dictionaryType != null)
            {
                var genericArguments = dictionaryType.GetGenericArguments();
                var keyType = genericArguments[0];
                var valueType = genericArguments[1];

                var dictionaryBindingContext = CreateCustomModelBindingContext(bindingContext, model, modelType: modelType);

                var dictionary = UpdateDictionary(controllerContext, dictionaryBindingContext, keyType, valueType);
                return dictionary;
            }

            var enumerableType = TypeHelpers.ExtractGenericInterface(modelType, typeof(IEnumerable<>));
            if (enumerableType != null)
            {
                var elementType = enumerableType.GetGenericArguments()[0];

                var collectionType = typeof(ICollection<>).MakeGenericType(elementType);
                if (collectionType.IsInstanceOfType(model))
                {
                    var collectionBindingContext = CreateCustomModelBindingContext(bindingContext, model, modelType: modelType);

                    var collection = UpdateCollection(controllerContext, collectionBindingContext, elementType);

                    return collection;
                }
            }

            if (!bindingContext.OriginalModelPopulated)
            {
                bindingContext.OriginalModel = CreateModel(controllerContext, bindingContext, modelType);
            }

            // otherwise, just update the properties on the complex type
            BindComplexElementalModel(controllerContext, bindingContext, model);
            return model;
        }

        private void BindProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var properties = GetFilteredModelProperties(controllerContext, bindingContext);
            foreach (var property in properties)
            {
                BindProperty(controllerContext, bindingContext, property);
            }
        }

        protected new IEnumerable<PropertyDescriptor> GetFilteredModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var properties = GetModelProperties(controllerContext, bindingContext);
            var customContext = bindingContext as CustomModelBindingContext;
            var propertyFilter = customContext != null ? customContext.FrameworkPropertyFilter : bindingContext.PropertyFilter;

            return from PropertyDescriptor property in properties
                   where ShouldUpdateProperty(property, propertyFilter)
                   select property;
        }

        private static bool ShouldUpdateProperty(PropertyDescriptor property, Predicate<string> propertyFilter)
        {
            if (property.IsReadOnly && !CanUpdateReadonlyTypedReference(property.PropertyType))
            {
                return false;
            }

            // if this property is rejected by the filter, move on
            if (!propertyFilter(property.Name))
            {
                return false;
            }

            // otherwise, allow
            return true;
        }

        private static bool CanUpdateReadonlyTypedReference(Type type)
        {
            // value types aren't strictly immutable, but because they have copy-by-value semantics
            // we can't update a value type that is marked readonly
            if (type.IsValueType)
            {
                return false;
            }

            // arrays are mutable, but because we can't change their length we shouldn't try
            // to update an array that is referenced readonly
            if (type.IsArray)
            {
                return false;
            }

            // special-case known common immutable types
            if (type == typeof(string))
            {
                return false;
            }

            return true;
        }

        internal ModelBindingContext CreateComplexElementalModelBindingContext(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
        {
            var bindAttr = (BindAttribute)GetTypeDescriptor(controllerContext, bindingContext).GetAttributes()[typeof(BindAttribute)];

            var newPropertyFilter = (bindAttr != null)
                                        ? propertyName =>
                                          bindAttr.IsPropertyAllowed(propertyName) &&
                                          bindingContext.PropertyFilter(propertyName)
                                        : bindingContext.PropertyFilter;

            var newBindingContext = CreateCustomModelBindingContext(bindingContext, model, newPropertyFilter);

            return newBindingContext;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo",
            MessageId = "System.Web.Mvc.ValueProviderResult.ConvertTo(System.Type)",
            Justification = "ValueProviderResult already handles culture conversion appropriately.")]
        private static void GetIndexes(ModelBindingContext bindingContext, out bool stopOnIndexNotFound, out IEnumerable<string> indexes)
        {
            var indexKey = CreateSubPropertyName(bindingContext.ModelName, "index");
            var vpResult = bindingContext.ValueProvider.GetValue(indexKey);

            if (vpResult != null)
            {
                var indexesArray = vpResult.ConvertTo(typeof(string[])) as string[];
                if (indexesArray != null)
                {
                    stopOnIndexNotFound = false;
                    indexes = indexesArray;
                    return;
                }
            }

            // just use a simple zero-based system
            stopOnIndexNotFound = true;
            indexes = GetZeroBasedIndexes();
        }

        // If the user specified a ResourceClassKey try to load the resource they specified.
        // If the class key is invalid, an exception will be thrown.
        // If the class key is valid but the resource is not found, it returns null, in which
        // case it will fall back to the MVC default error message.
        private static string GetUserResourceString(ControllerContext controllerContext, string resourceName)
        {
            string result = null;

            if (!String.IsNullOrEmpty(ResourceClassKey) && (controllerContext != null) &&
                (controllerContext.HttpContext != null))
            {
                result =
                    controllerContext.HttpContext.GetGlobalResourceObject(ResourceClassKey, resourceName,
                                                                          CultureInfo.CurrentUICulture) as string;
            }

            return result;
        }

        private static string GetValueInvalidResource(ControllerContext controllerContext)
        {
            return GetUserResourceString(controllerContext, "PropertyValueInvalid") ??
                   MvcResources.DefaultModelBinder_ValueInvalid;
        }

        private static string GetValueRequiredResource(ControllerContext controllerContext)
        {
            return GetUserResourceString(controllerContext, "PropertyValueRequired") ??
                   MvcResources.DefaultModelBinder_ValueRequired;
        }

        private static IEnumerable<string> GetZeroBasedIndexes()
        {
            for (int i = 0; ; i++)
            {
                yield return i.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static bool ShouldPerformRequestValidation(ControllerContext controllerContext,
                                                           ModelBindingContext bindingContext)
        {
            if (controllerContext == null || controllerContext.Controller == null || bindingContext == null ||
                bindingContext.ModelMetadata == null)
            {
                // To make unit testing easier, if the caller hasn't specified enough contextual information we just default
                // to always pulling the data from a collection that goes through request validation.
                return true;
            }

            // We should perform request validation only if both the controller and the model ask for it. This is the
            // default behavior for both. If either the controller (via [ValidateInput(false)]) or the model (via [AllowHtml])
            // opts out, we don't validate.
            return (controllerContext.Controller.ValidateRequest &&
                    bindingContext.ModelMetadata.RequestValidationEnabled);
        }

        internal object UpdateCollection(ControllerContext controllerContext, CustomModelBindingContext bindingContext,
                                        Type elementType)
        {

            bool stopOnIndexNotFound;
            IEnumerable<string> indexes;
            GetIndexes(bindingContext, out stopOnIndexNotFound, out indexes);
            var elementBinder = Binders.GetBinder(elementType);

            // build up a list of items from the request
            var modelList = new List<object>();
            var omodelList = new List<object>();
            var originalModel = bindingContext.Model.ToListObject();
            foreach (var currentIndex in indexes)
            {
                var subIndexKey = CreateSubIndexName(bindingContext.ModelName, currentIndex);
                if (!bindingContext.ValueProvider.ContainsPrefix(subIndexKey))
                {
                    if (stopOnIndexNotFound)
                    {
                        // we ran out of elements to pull
                        break;
                    }
                    continue;
                }

                /* Model binder should not always create new instance of object in the list
                 * The expected operation is: item in the list should only be created when there is no
                 * equal object in the original list
                 */
                var originalItemIndex = int.Parse(currentIndex);
                ModelMetadata modelMetadata = null;
                if (originalItemIndex < originalModel.Count)
                {
                    modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => originalModel[originalItemIndex], elementType);
                }
                else
                {
                    modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, elementType);
                }
                var innerContext = new CustomModelBindingContext
                                       {
                                           ModelMetadata = modelMetadata,
                                           ModelName = subIndexKey,
                                           ModelState = bindingContext.ModelState,
                                           ValueProvider = bindingContext.ValueProvider
                                       };
                var thisElement = elementBinder.BindModel(controllerContext, innerContext);

                // we need to merge model errors up
                AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, subIndexKey,
                                                    elementType, thisElement);
                modelList.Add(thisElement);
                omodelList.Add(innerContext.OriginalModel);
            }

            // if there weren't any elements at all in the request, just return
            if (modelList.Count == 0)
            {
                return null;
            }

            // replace the original collection
            var collection = bindingContext.Model;
            CollectionHelpers.ReplaceCollection(elementType, collection, modelList);
            if (!bindingContext.OriginalModelPopulated)
            {
                var ocollection = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
                CollectionHelpers.ReplaceCollection(elementType, ocollection, omodelList);
                bindingContext.OriginalModel = ocollection;
            }
            return collection;
        }

        internal object UpdateDictionary(ControllerContext controllerContext, CustomModelBindingContext bindingContext,
                                         Type keyType, Type valueType)
        {
            bool stopOnIndexNotFound;
            IEnumerable<string> indexes;
            GetIndexes(bindingContext, out stopOnIndexNotFound, out indexes);

            var keyBinder = Binders.GetBinder(keyType);
            var valueBinder = Binders.GetBinder(valueType);
            var dictSize = CollectionHelpers.GetCount(bindingContext.Model);
            // build up a list of items from the request
            var modelList = new List<KeyValuePair<object, object>>();
            var omodelList = new List<KeyValuePair<object, object>>();
            foreach (var currentIndex in indexes)
            {
                var subIndexKey = CreateSubIndexName(bindingContext.ModelName, currentIndex);
                var keyFieldKey = CreateSubPropertyName(subIndexKey, "key");
                var valueFieldKey = CreateSubPropertyName(subIndexKey, "value");

                if (
                    !(bindingContext.ValueProvider.ContainsPrefix(keyFieldKey) &&
                      bindingContext.ValueProvider.ContainsPrefix(valueFieldKey)))
                {
                    if (stopOnIndexNotFound)
                    {
                        // we ran out of elements to pull
                        break;
                    }
                    continue;
                }
                var idx = int.Parse(currentIndex);

                // bind the key
                ModelMetadata keyModelMetadata = null;
                if (idx < dictSize)
                {
                    var originalKeyObject = CollectionHelpers.GetKeyByIndex(bindingContext.Model, idx);
                    keyModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => originalKeyObject, keyType);
                }
                else
                {
                    keyModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, keyType);
                }

                var keyBindingContext = new CustomModelBindingContext
                {
                    ModelMetadata = keyModelMetadata,
                    ModelName = keyFieldKey,
                    ModelState = bindingContext.ModelState,
                    ValueProvider = bindingContext.ValueProvider
                };
                var thisKey = keyBinder.BindModel(controllerContext, keyBindingContext);

                // we need to merge model errors up
                AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, keyFieldKey, keyType,
                                                    thisKey);
                if (!keyType.IsInstanceOfType(thisKey))
                {
                    // we can't add an invalid key, so just move on
                    continue;
                }

                // bind the value
                ModelMetadata valueModelMetadata = null;
                if (idx < dictSize)
                {
                    object originalObject = CollectionHelpers.GetValueByIndex(bindingContext.Model, idx);
                    valueModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => originalObject, valueType);
                }
                else
                {
                    valueModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, valueType);
                }

                var valueBindingContext = new ModelBindingContext
                {
                    ModelMetadata = valueModelMetadata,
                    ModelName = valueFieldKey,
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                var thisValue = valueBinder.BindModel(controllerContext, valueBindingContext);

                // we need to merge model errors up
                AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, valueFieldKey,
                                                    valueType, thisValue);
                var kvp = new KeyValuePair<object, object>(thisKey, thisValue);
                modelList.Add(kvp);

                var okvp = new KeyValuePair<object, object>(thisKey, keyBindingContext.OriginalModel);
                omodelList.Add(okvp);
            }

            // if there weren't any elements at all in the request, just return
            if (modelList.Count == 0)
            {
                return null;
            }

            // replace the original collection
            var dictionary = bindingContext.Model;
            if (!bindingContext.OriginalModelPopulated)
            {
                var odictionary = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
                CollectionHelpers.ReplaceDictionary(keyType, valueType, odictionary, omodelList);
                bindingContext.OriginalModel = odictionary;
            }
            CollectionHelpers.ReplaceDictionary(keyType, valueType, dictionary, modelList);
            return dictionary;
        }

        // This helper type is used because we're working with strongly-typed collections, but we don't know the Ts
        // ahead of time. By using the generic methods below, we can consolidate the collection-specific code in a
        // single helper type rather than having reflection-based calls spread throughout the DefaultModelBinder type.
        // There is a single point of entry to each of the methods below, so they're fairly simple to maintain.

        #region Nested type: CollectionHelpers

        private static class CollectionHelpers
        {

            private static readonly MethodInfo _replaceCollectionMethod = typeof(CollectionHelpers).GetMethod("ReplaceCollectionImpl", BindingFlags.Static | BindingFlags.NonPublic);
            private static readonly MethodInfo _replaceDictionaryMethod = typeof(CollectionHelpers).GetMethod("ReplaceDictionaryImpl", BindingFlags.Static | BindingFlags.NonPublic);

			public static int GetCount(object dict)
            {
                dynamic dict2 = dict;
                return ((IDictionary)dict2).Keys.Count;
            }

            public static object GetKeyByIndex(object dict, int idx)
            {
                Type type = dict.GetType();
                FieldInfo info = type.GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
                if (info != null)
                {
                    Object element = ((Array)info.GetValue(dict)).GetValue(idx);
                    return element.GetType().GetField("key", BindingFlags.Public | BindingFlags.Instance).GetValue(element);
                }
                return default(object);
            }

            public static object GetValueByIndex(object dict, int idx)
            {
                Type type = dict.GetType();
                FieldInfo info = type.GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
                if (info != null)
                {
                    Object element = ((Array)info.GetValue(dict)).GetValue(idx);
                    return element.GetType().GetField("value", BindingFlags.Public | BindingFlags.Instance).GetValue(element);
                }

                return default(object);
            }
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void ReplaceCollection(Type collectionType, object collection, object newContents)
            {
                MethodInfo targetMethod = _replaceCollectionMethod.MakeGenericMethod(collectionType);
                targetMethod.Invoke(null, new object[] { collection, newContents });
            }

            private static void ReplaceCollectionImpl<T>(ICollection<T> collection, IEnumerable newContents)
            {
                collection.Clear();
                if (newContents != null)
                {
                    foreach (object item in newContents)
                    {
                        // if the item was not a T, some conversion failed. the error message will be propagated,
                        // but in the meanwhile we need to make a placeholder element in the array.
                        T castItem = (item is T) ? (T)item : default(T);
                        collection.Add(castItem);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void ReplaceDictionary(Type keyType, Type valueType, object dictionary, object newContents)
            {
                MethodInfo targetMethod = _replaceDictionaryMethod.MakeGenericMethod(keyType, valueType);
                targetMethod.Invoke(null, new object[] { dictionary, newContents });
            }

            private static void ReplaceDictionaryImpl<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<object, object>> newContents)
            {
                dictionary.Clear();
                foreach (KeyValuePair<object, object> item in newContents)
                {
                    // if the item was not a T, some conversion failed. the error message will be propagated,
                    // but in the meanwhile we need to make a placeholder element in the dictionary.
                    TKey castKey = (TKey)item.Key; // this cast shouldn't fail
                    TValue castValue = (item.Value is TValue) ? (TValue)item.Value : default(TValue);
                    dictionary[castKey] = castValue;
                }
            }
        }
        #endregion

        // Used to wrap an IValueProvider in an IUnvalidatedValueProvider

        #region Nested type: UnvalidatedValueProviderWrapper

        private sealed class UnvalidatedValueProviderWrapper : IUnvalidatedValueProvider
        {
            private readonly IValueProvider _backingProvider;

            public UnvalidatedValueProviderWrapper(IValueProvider backingProvider)
            {
                _backingProvider = backingProvider;
            }

            #region IUnvalidatedValueProvider Members

            public ValueProviderResult GetValue(string key, bool skipValidation)
            {
                // 'skipValidation' isn't understood by the backing provider and can be ignored
                return GetValue(key);
            }

            #endregion

            #region IValueProvider Members

            public bool ContainsPrefix(string prefix)
            {
                return _backingProvider.ContainsPrefix(prefix);
            }

            public ValueProviderResult GetValue(string key)
            {
                return _backingProvider.GetValue(key);
            }

            #endregion
        }

        #endregion

        #endregion
    }
}
