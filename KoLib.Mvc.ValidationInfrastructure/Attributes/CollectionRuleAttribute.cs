using System;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class CollectionRuleAttribute : Attribute
    {
        public CollectionRuleAttribute(string ruleName, string holdValueProperty)
        {
            RuleName = ruleName;
            HoldValueProperty = holdValueProperty;
        }

        public string RuleName { get; set; }
        public string HoldValueProperty { get; set; }
    }
}