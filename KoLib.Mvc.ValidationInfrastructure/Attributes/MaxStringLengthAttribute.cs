using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class MaxStringLengthAttribute : CustomStringLengthAttribute
    {
        #region Ctors

        public MaxStringLengthAttribute(int maximumLength)
            : base(maximumLength)
        {
        }

        #endregion

        #region Overrides of CustomStringLengthAttribute

        public override string FormatErrorMessage(string name)
        {
            string messageTemplate = ErrorMessageString;

            if (string.IsNullOrWhiteSpace(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessageResourceName))
            {
                messageTemplate = Constants.DefaultTooLongErrorTemplate;
            }

            return string.Format(CultureInfo.CurrentCulture, messageTemplate, name, MaximumLength);
        }

        public override bool IsValid(object value)
        {
            MinimumLength = int.MinValue;

            return base.IsValid(value);
        }

        protected override ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule rule = base.GetRule(metadata, context);

            rule.ValidationParameters.Remove("min");

            return rule;
        }

        #endregion
    }
}