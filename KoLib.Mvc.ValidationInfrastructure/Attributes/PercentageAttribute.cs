using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class PercentageAttribute : ValidationAttribute, IClientValidatable
    {
        #region Overrides of ValidationAttribute

        public override bool IsValid(object value)
        {
            return true;
        }

        #endregion

        #region Implementation of IClientValidatable

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                    ValidationType = "percentage"
                };

            yield return rule;
        }

        #endregion
    }
}