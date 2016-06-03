using System.Collections.Generic;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    /// <summary>
    /// Marks a Boolean property must be true
    /// </summary>
    public class MustTrueAttribute : CustomRequiredAttribute
    {
        /// <summary>
        /// Checks if the marked property is true
        /// </summary>
        /// <param name="value">
        /// The current value of the marked property
        /// </param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            var isValid = base.IsValid(value);

            if (!(value is bool))
            {
                return isValid;
            }

            isValid = isValid && (bool)value;

            return isValid;
        }

        /// <summary>
        /// Supports for client validation
        /// </summary>
        /// <param name="metadata">
        /// The metadata of current property
        /// </param>
        /// <param name="context">
        /// The context
        /// </param>
        /// <returns></returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                    ValidationType = "xtrue"
                };

            yield return rule;
        }
    }
}