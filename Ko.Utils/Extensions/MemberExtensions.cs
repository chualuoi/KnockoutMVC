using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Ko.Utils.Extensions
{
    /// <summary>
    /// Contains all extension method for MemberInfo (include PropertyInfo and FieldInfo)
    /// </summary>
    public static class MemberExtensions
    {
        /// <summary>
        /// Determines whether the specified property has attribute.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <returns>
        ///   <c>true</c> if the specified property has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this MemberInfo property, Type attrType)
        {
            //Check if there is custom data attribute associate with current property
            var q = from CustomAttributeData attr in CustomAttributeData.GetCustomAttributes(property) where (attrType.IsAssignableFrom(attr.Constructor.DeclaringType)) select attr;
            if (q.Any())
            {
                return true;
            }
            //Check if there is metadata associate with current property
            var metaDataAttribute = (MetadataTypeAttribute)property.DeclaringType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
            return metaDataAttribute != null && metaDataAttribute.MetadataClassType.GetMembers().Any(x => x.HasAttribute(attrType) && x.Name == property.Name);
        }

        /// <summary>
        /// Checks if given field has TAttribute
        /// </summary>
        /// <typeparam name="TAttribute">
        /// Attribute type
        /// </typeparam>
        /// <param name="field">
        /// Given field
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes
        /// </param>
        /// <returns>
        /// </returns>
        public static bool Has<TAttribute>(this MemberInfo field, bool inherit = false) where TAttribute : Attribute
        {
            var result = field.Get<TAttribute>(inherit);

            return result != null;
        }

        /// <summary>
        /// Checks if the PropertyDescriptor has TAttribute
        /// </summary>
        /// <typeparam name="TAttribute">
        /// Attribute type
        /// </typeparam>
        /// <param name="propertyDescriptor">
        /// Given PropertyDescriptor
        /// </param>
        /// <returns>
        /// </returns>
        public static bool Has<TAttribute>(this MemberDescriptor propertyDescriptor) where TAttribute : Attribute
        {
            var result = propertyDescriptor.Get<TAttribute>();

            return result != null;
        }

        /// <summary>
        /// Tries to get the attribute from the given MemberInfo
        /// </summary>
        /// <typeparam name="TAttribute">
        /// Attribute type
        /// </typeparam>
        /// <param name="member">
        /// Given member
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes
        /// </param>
        /// <returns>
        /// </returns>
        public static TAttribute Get<TAttribute>(this MemberInfo member, bool inherit = false) where TAttribute : Attribute
        {
            var result = default(TAttribute);
            var found = member.GetCustomAttributes(typeof(TAttribute), inherit);
            if (found.Length > 0)
            {
                result = (TAttribute)found[0];
            }

            return result;
        }

        /// <summary>
        /// Tries to get the attribute from the given PropertyDescriptor
        /// </summary>
        /// <typeparam name="TAttribute">
        /// Attribute type
        /// </typeparam>
        /// <param name="type">
        /// Given PropertyDescriptor
        /// </param>
        /// <returns>
        /// </returns>
        public static TAttribute Get<TAttribute>(this MemberDescriptor type) where TAttribute : Attribute
        {
            var result = type.Attributes[typeof(TAttribute)];

            return (TAttribute)result;
        }
        
        /// <summary>
        /// Gets the type of the member which is property and field.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException("memberInfo");
            }
            if (!(memberInfo is FieldInfo) && !(memberInfo is PropertyInfo))
            {
                throw new ArgumentException("Member must be property or field " + String.Format("{0}.{1}", memberInfo.DeclaringType.FullName, memberInfo.Name), "memberInfo");
            }
            return memberInfo is FieldInfo
                       ? ((FieldInfo)memberInfo).FieldType
                       : ((PropertyInfo)memberInfo).PropertyType;
        }
    }
}