using System;

namespace Ko.Mvc.KnockoutAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class KnockoutIgnoreAttribute : Attribute
    {
    }
}