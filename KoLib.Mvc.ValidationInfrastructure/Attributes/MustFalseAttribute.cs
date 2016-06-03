using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    /// <summary>
    /// Marks a Boolean property must be false
    /// </summary>
    public class MustFalseAttribute : CustomRequiredAttribute
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

            isValid = isValid && !(bool)value;

            return isValid;
        }

        protected override ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var validationName = metadata.GetValidationName();

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(validationName),
                ValidationType = "xfalse"
            };

            return rule;
        }
    }
}