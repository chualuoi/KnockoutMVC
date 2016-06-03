using System.Collections.Generic;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class DateRequiredAttribute : CustomRequiredAttribute
    {
        #region Overrides of ConditionalRequiredAttribute

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = GetRule(metadata, context);

            rule.ValidationType = "daterequired";

            yield return rule;
        }

        #endregion
    }
}