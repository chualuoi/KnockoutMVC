using System;

namespace Ko.Mvc.KnockoutAttributes
{
    /// <summary>
    /// This attribute used to decorate a property which will be knockout navigation property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class KnockoutNavigationAttribute : Attribute
    {
         
    }
}