using System.Reflection;
using Ko.Mvc.KnockoutAttributes;
using Ko.Utils.Extensions;

namespace KoLib.T4Helpers
{
    /// <summary>
    /// Contains helpers method of knockout view model member (properties and fields)
    /// </summary>
    public static class KnockoutMemberExtensions
    {
        /// <summary>
        /// Check whether a specific member is knockout computed property
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsKnockoutComputed(this MemberInfo member)
        {
            return member.Has<KnockoutComputedAttribute>();
        }

        /// <summary>
        /// Check whether a specific member is knockout event handler property
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsKnockoutEventHandler(this MemberInfo member)
        {
            return member.Has<KnockoutEventHandlerAttribute>();
        }
    }
}