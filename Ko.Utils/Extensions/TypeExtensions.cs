using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Ko.Utils.Extensions
{
    /// <summary>
    /// This class contains extension method to manipulate type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type has a specific attribute.
        /// </summary>
        /// <param name="t">The type to process.</param>
        /// <param name="attrType">Type of the attr.</param>
        /// <param name="inherits">
        /// Specifies whether to search the member's inheritance chain to find the attributes
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified type has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this Type t, Type attrType, bool inherits = true)
        {
            var customAttributes = t.GetCustomAttributes(attrType, inherits);
            return customAttributes.Any();
        }

        /// <summary>
        /// Checks if the type has TAttribute
        /// </summary>
        /// <typeparam name="TAttribute">
        /// Attribute type
        /// </typeparam>
        /// <param name="type">
        /// Given type
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes
        /// </param>
        /// <returns>
        /// </returns>
        public static bool Has<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
        {
            var result = type.Get<TAttribute>(inherit);

            return result != null;
        }
        
        /// <summary>
        /// Determines whether a specific type is an atom type.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>
        ///   <c>true</c> type is atom type otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAtomType(this Type t)
        {
            return (t.IsPrimitive || t == typeof(string) || t.IsNumeric());
        }

        /// <summary>
        /// Determines whether a specific type is a nullable type.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>
        ///   <c>true</c> type is nullable type otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this Type t)
        {
            return (t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Determines whether [is I enumerable] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is I enumerable] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnumerable(this Type type)
        {
            //Check if the type itself is IEnumerable<>
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    var enumerableTemplate = typeof(IEnumerable<>);
                    var assignableType = enumerableTemplate.MakeGenericType(genericArgs);
                    if (assignableType == type)
                    {
                        return true;
                    }
                }
            }

            //Check if this type implements IEnumerable<>
            var result = from Type t in type.GetInterfaces()
                         where (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                         select t;
            return result.Count() > 0;
        }

        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        public static bool IsNumeric(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumeric(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Get string value of integral (atom) type
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string StringValueAtomType(this object value)
        {
            if (value == null)
            {
                return null;
            }
            var valueType = value.GetType();
            if (!IsAtomType(valueType))
            {
                throw new ArgumentException("Can only process atom type value", "value");
            }
            var stringValue = valueType == typeof(char) || valueType == typeof(string) ? String.Format("'{0}'", value) :
                                                                                                                           (valueType == typeof(bool) ? value.ToString().ToLower() : value.ToString());
            return stringValue;
        }

        /// <summary>
        /// Get the generic full name of a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GenericFullName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName;
            }
            return type.FullName != null ? type.FullName.Substring(0, type.FullName.IndexOf("`")) : null;
        }

        /// <summary>
        /// Get the generics name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GenericName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }
            return type.Name.Substring(0, type.Name.IndexOf("`"));
        }
                
        /// <summary>
        /// Get Generic Method matching type arguments
        /// </summary>
        /// <param name="type">Type to get method</param>
        /// <param name="name">Method name</param>
        /// <param name="genericTypeArgs">An array of types to be substituted for the type parameters 
        /// of the current generic method definition.
        /// </param>
        /// <param name="paramTypes">An array of types to be substituted for the type parameters 
        /// of the current generic type definition.
        /// </param>
        /// <returns>MethodInfo of found method</returns>
        public static MethodInfo GetGenericMethod(this Type type, string name, Type[] genericTypeArgs, Type[] paramTypes)
        {
            foreach (MethodInfo m in type.GetMethods())
                if (m.Name == name)
                {
                    ParameterInfo[] pa = m.GetParameters();
                    if (pa.Length == paramTypes.Length)
                    {
                        MethodInfo c = m.MakeGenericMethod(genericTypeArgs);
                        if (c.GetParameters().Select(p => p.ParameterType).SequenceEqual(paramTypes))
                            return c;
                    }
                }
            throw new Exception("Could not find a method matching the signature " + type + "." + name +
                                "<" + String.Join(", ", genericTypeArgs.AsEnumerable()) + ">" +
                                "(" + String.Join(", ", paramTypes.AsEnumerable()) + ").");
        }
        
        /// <summary>
        /// Tries to get the attribute from the given Type
        /// </summary>
        /// <typeparam name="TAttribute">
        /// Attribute type
        /// </typeparam>
        /// <param name="type">
        /// Given type
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes
        /// </param>
        /// <returns>
        /// </returns>
        public static TAttribute Get<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
        {

            var result = default(TAttribute);
            var found = type.GetCustomAttributes(typeof(TAttribute), inherit);
            if (found.Length > 0)
            {
                result = (TAttribute)found[0];
            }

            return result;
        }

        /// <summary>
        /// Get all public fields and properties at instance level for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<MemberInfo> GetPublicFieldsAndProperties(this Type type)
        {
            var fieldsProperties = new List<MemberInfo>();
            fieldsProperties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy));
            fieldsProperties.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy));
            return fieldsProperties;
        }

    }
}
