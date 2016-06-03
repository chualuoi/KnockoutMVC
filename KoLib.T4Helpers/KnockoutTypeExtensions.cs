using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ko.Mvc.KnockoutAttributes;
using Ko.Utils.Extensions;
using Newtonsoft.Json;

namespace KoLib.T4Helpers
{
    /// <summary>
    /// Contains extensions for KnockoutViewModel type
    /// </summary>
    public static class KnockoutTypeExtensions
    {
        /// <summary>
        /// Gets all matched the types in a specific assembly.
        /// If assembly config item contains no namesppace collection --> return all types
        /// </summary>
        /// <param name="config">The config object contains information about namespaces and types in this assembly to get.</param>
        /// <returns></returns>
        public static List<Type> GetMatchTypes(this AssemblyConfig config)
        {
            try
            {
                //Load the assembly
                var assembly = Assembly.LoadFrom(config.Name);
                //Get all types in the assembly
                var allTypes = assembly.GetTypes().ToList();

                if (config.NamespaceCollection.Count == 0)
                {
                    //return all types contained in this assembly if we don't have any specific configuration
                    return allTypes;
                }
                var matchTypes = new List<Type>();
                //Search for types that match configuration
                foreach (var ns in config.NamespaceCollection)
                {
                    var typeInNamespace =
                        allTypes.Where(x => !String.IsNullOrWhiteSpace(x.Namespace) && x.Namespace.StartsWith(ns.Name)).ToList();
                    //Add all types in namspaces to matching list if this namespace does not contain any
                    //specific configuraiton about type matching
                    matchTypes.AddRange(ns.TypeCollection.Count == 0
                                            ? typeInNamespace
                                            : typeInNamespace.Where(x => ns.TypeCollection.Contains(x.Name)));
                }
                return matchTypes;
            }
            catch (Exception)
            {
                //If any exception occurs, return an empty list
                return new List<Type>();
            }
        }

        /// <summary>
        /// Gets the name of the knockout file.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetKnockoutFileName(this Type type)
        {
            if (type.IsKnockoutViewModel())
            {
                var knockViewModelAttributes = type.GetCustomAttributes(typeof (KnockoutViewModelAttribute), true);
                if (knockViewModelAttributes.Length > 0)
                {
                    var knockViewModelAttribute = (KnockoutViewModelAttribute) knockViewModelAttributes[0];
                    return String.IsNullOrWhiteSpace(knockViewModelAttribute.FileName)
                               ? type.GenericName()
                               : knockViewModelAttribute.FileName;
                }
            }
            return type.GenericName();
        }
        
        /// <summary>
        /// Create default json string for a specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string DefaultJsonString(this Type type)
        {
            if (!type.IsGenericType && !type.IsAbstract)
            {
                return JsonConvert.SerializeObject(Activator.CreateInstance(type));
            }
            var fieldsProperties = new List<MemberInfo>();
            fieldsProperties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy));
            fieldsProperties.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy));
            var jsonProperties = fieldsProperties.Select(fieldsProperty => String.Format("\"{0}\":null", fieldsProperty.Name)).ToList();
            return "{" + String.Join(", ", jsonProperties) + "}";
        }        

        /// <summary>
        /// Check whether specific type is Knockout View model
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsKnockoutViewModel(this Type type)
        {
            const string knockoutViewModelSuffix = "ViewModel";
            if(type == null)
            {
                throw new ArgumentNullException("type");
            }
            var matchingConvention = type.Name.EndsWith(knockoutViewModelSuffix,
                                                        StringComparison.InvariantCultureIgnoreCase);
            var attr = type.Get<KnockoutViewModelAttribute>();
            return attr !=  null ? attr.IsKnockoutViewModel : matchingConvention;
        }

        /// <summary>
        /// Check whether a specific type is a root knockout view model
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsRootKnockoutViewModel(this Type type)
        {
            var attr = type.Get<KnockoutViewModelAttribute>();
            return type.IsKnockoutViewModel() && attr != null && attr.RootIndicator;
        }

        /// <summary>
        /// Recursively gets the child knockout view model.
        /// </summary>
        /// <param name="t">The type to get child model.</param>
        /// <returns></returns>
        public static List<Type> GetChildKnockoutViewModel(this Type t)
        {
            var types = new List<Type>();
            var properties = t.GetPublicFieldsAndProperties();
            foreach (var property in properties)
            {
                if (property.HasAttribute(typeof(KnockoutComputedAttribute)) || property.HasAttribute(typeof(KnockoutEventHandlerAttribute))
                    || property.HasAttribute(typeof(KnockoutIgnoreAttribute)))
                {
                    continue;
                }

                var propertyType = property.GetMemberType();

                //If this property is complex type and knockout view model, add to knockout view model list
                //and recursively search in child type
                if (!propertyType.IsAtomType())
                {
                    types.Add(propertyType);
                    types.AddRange(propertyType.GetChildKnockoutViewModel());
                }
                //If current property is a collection
                if (propertyType != typeof(string) && propertyType.IsEnumerable())
                {
                    var elementType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments()[0];
                    if (!elementType.IsAtomType())
                    {
                        types.Add(elementType);
                        types.AddRange(elementType.GetChildKnockoutViewModel());
                    }
                }
            }
            return types;
        }
    }
}