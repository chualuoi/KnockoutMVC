using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Helpers;
using Ko.Mvc.KnockoutAttributes;
using Ko.Utils.Extensions;

namespace KoLib.T4Helpers
{
    /// <summary>
    /// Contains static helpers for knockout view model T4 generator
    /// </summary>
    public static class KnockoutT4Helper
    {
        /// <summary>
        /// Format string for create function of observable object
        /// </summary>
        public const string ObservableCreate = @"return ko.observable(new {0}(options.data, self));";
        /// <summary>
        /// Format string for create function of non-observable object
        /// </summary>
        public const string NonObservableCreate = @"return new {0}(options.data, self);";

        /// <summary>
        /// Format string for create function of numeric observable object
        /// </summary>
        public const string NumericObservableCreate = @"return ko.numericObservable(options.data);";
        
        /// <summary>
        /// Creates constructor for knockout view model properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isStatic">Indicates whether this property is static</param>
        /// <returns></returns>
        public static KeyValuePair<string, string>? CreateConstructor(MemberInfo property, bool isStatic)
        {
            //Get type of the property
            var propertyType = property is PropertyInfo
                                ? ((PropertyInfo)property).PropertyType
                                : ((FieldInfo)property).FieldType;
            //Get value of the property
            var value = isStatic ? (property is PropertyInfo
                                        ? ((PropertyInfo)property).GetValue(null, null)
                                        : ((FieldInfo)property).GetValue(null))
                                 : null;

            //If this property is complex type (not primitive nor string), override the constructor
            string functionBody = string.Empty;

            #region Process complex type
            if (!propertyType.IsAtomType())
            {   
                #region Process if this property is enumerable
                if (propertyType.IsEnumerable())
                {
                    //Assume that this property is only list of viewmodel
                    //otherwise, indicate error
                    if (propertyType.GetGenericArguments().Count() > 1 &&
                        propertyType.GetGenericArguments().Any(x => x.HasAttribute(typeof(KnockoutViewModelAttribute))))
                    {
                        //TODO: Need a solution with custom ienumerable with multiple generic arguments
                        throw new Exception("Could not process complex ienumerable type: " + propertyType);
                    }

                    //If this property is IEnumerable not IEnumerable<>
                    if (!propertyType.GetGenericArguments().Any())
                    {
                        var elementType = propertyType.GetElementType();
                        if (elementType == null)
                        {
                            throw new Exception("Element type of IEnumerable cannot be null");
                        }
                        if (isStatic)
                        {
                            if (elementType.HasAttribute(typeof(KnockoutViewModelAttribute)))
                            {
                                var members = (from object item in (IEnumerable)value
                                               select
                                                   string.Format("new {0}({1})", elementType.GenericFullName(), Json.Encode(item))).ToList();
                                var array = "[" + string.Join(", ", members) + "]";
                                
                                if (property.HasAttribute(typeof(OneWayMappingAttribute)))
                                {
                                    return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                                    property.DeclaringType.GenericFullName(),
                                                                    property.Name),
                                                                    string.Format("{0}", array));
                                }
                                return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                                    property.DeclaringType.GenericFullName(),
                                                                    property.Name),
                                                                    string.Format("ko.observableArray({0});", array));
                            }
                            
                            if (property.HasAttribute(typeof(OneWayMappingAttribute)))
                            {
                                return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                                    property.DeclaringType.GenericFullName(),
                                                                    property.Name),
                                                                    string.Format("ko.mapping.fromJs({0})", Json.Encode(value)));
                            }
                            return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                                    property.DeclaringType.GenericFullName(),
                                                                    property.Name),
                                                                    string.Format("ko.obserableArray(ko.mapping.fromJs({0}));", Json.Encode(value)));
                        }
                        if (elementType.HasAttribute(typeof(KnockoutViewModelAttribute)))
                        {
                            if (property.HasAttribute(typeof(OneWayMappingAttribute)))
                            {
                                return new KeyValuePair<string, string>(property.Name,
                                string.Format(NonObservableCreate, elementType.GenericFullName()));
                            }
                            return new KeyValuePair<string, string>(property.Name,
                                string.Format(ObservableCreate, elementType.GenericFullName()));
                        }
                        return null;
                    }
                    //If the generic argument is not view model
                    if (!propertyType.GetGenericArguments()[0].HasAttribute(typeof(KnockoutViewModelAttribute)))
                    {
                        if (isStatic)
                        {
                            var functionTmp = property.HasAttribute(typeof(OneWayMappingAttribute))
                                                  ? string.Format("ko.mapping.fromJs({0});", Json.Encode(value.ToString()))
                                                  : string.Format("ko.observableArray(ko.mapping.fromJs({0}));", Json.Encode(value));
                            return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                                                  property.DeclaringType.GenericFullName(),
                                                                                  property.Name), functionTmp);
                        }
                        return null;
                    }
                    var viewModelType = propertyType.GetGenericArguments()[0].GenericFullName();
                    string function;
                    if (property.HasAttribute(typeof(OneWayMappingAttribute)))
                    {
                        function = string.Format(NonObservableCreate, viewModelType);
                    }
                    else
                    {
                        function = string.Format(ObservableCreate, viewModelType);
                    }
                    return new KeyValuePair<string, string>(property.Name, function);
                }
                #endregion

                #region Non-IEnumerable property

                //If we go here, this property is not IEnumerable
                //Process if this property is not knockout view model
                if (!propertyType.HasAttribute(typeof(KnockoutViewModelAttribute)))
                {
                    //encode property value to json string and use knockout mapping to create observable object
                    //if this property is static
                    if (isStatic)
                    {
                        var jsonString = string.Format("ko.mapping.fromJS{0}", Json.Encode(value));
                        return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                                    property.DeclaringType.GenericFullName(),
                                                                    property.Name), jsonString);
                    }
                    //if this property is not static, just ignore it and return null
                    //knockout mapping plug in will do the rest for us
                    return null;
                }

                if (isStatic)
                {
                    if (property.HasAttribute(typeof(OneWayMappingAttribute)))
                    {
                        return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                            property.DeclaringType.GenericFullName(),
                                                            property.Name),
                                                            string.Format("new {0}({1});",
                                                                  propertyType.GenericFullName(), Json.Encode(value))
                                                            );
                    }
                    return new KeyValuePair<string, string>(string.Format("{0}.{1}",
                                                           property.DeclaringType.GenericFullName(),
                                                           property.Name),
                                                           string.Format("ko.observable(new {0}({1}));",
                                                                 propertyType.GenericFullName(), Json.Encode(value))
                                                           );
                }

                //construct create function
                if (property.HasAttribute(typeof(OneWayMappingAttribute)))
                {
                    functionBody = string.Format(NonObservableCreate, propertyType.GenericFullName());
                }
                else
                {
                    functionBody = string.Format(ObservableCreate,
                                                 propertyType.GenericFullName());
                }
                return new KeyValuePair<string, string>(property.Name, functionBody);
                #endregion
            }
            #endregion

            #region Integral (atom type)

            //If this property is numeric, use numeric observable
            if(propertyType.IsNumeric())
            {
                functionBody = property.HasAttribute(typeof(OneWayMappingAttribute))
                                          ? "return options.data;" : NumericObservableCreate;
                return new KeyValuePair<string, string>(property.Name, functionBody);
            }
            //If this property is static, process it. Otherwise, just ignore it
            if (isStatic)
            {
                var stringValue = value.StringValueAtomType();
                functionBody = property.HasAttribute(typeof(OneWayMappingAttribute))
                                          ? stringValue : string.Format("ko.observable({0});", stringValue);
                return new KeyValuePair<string, string>(property.Name, functionBody);
            }
            return null;
            #endregion
        }

        /// <summary>
        /// Generate computation logic of computed property based on its expression
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GenerateComputedLogic(MemberInfo property)
        {
            if(property.IsKnockoutComputed())
            {
                //TODO: Write the logic here            
            }
            return null;
        }
    }
}