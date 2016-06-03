using System.Collections.Generic;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    /// <summary>
    /// <see cref="DummyRuleForAttribute"/> will not actually validate anything on server.
    /// Validation on server will be done another place. Usage of this property is primary for collection validation. 
    /// </summary>
    public class DummyRuleForAttribute : RuleForAttribute
    {
        public DummyRuleForAttribute(string property)
            : base(property)
        {
        }

        public override bool IsValid(object value)
        {
            return true;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                                        ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                    ValidationType = "dummyrulefor"
                };
            rule.ValidationParameters.Add("property", Property);
            yield return rule;
        }
    }
}