using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class CustomStringLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        #region Ctors

        public CustomStringLengthAttribute(int maximumLength)
            : base(maximumLength)
        {
        }

        #endregion

        #region Overrides of StringLengthAttribute

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var svalue = value.ToString();

            return base.IsValid(svalue);
        }

        public override string FormatErrorMessage(string name)
        {
            string messageTemplate = ErrorMessageString;

            if (string.IsNullOrWhiteSpace(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessageResourceName))
            {
                messageTemplate = Constants.DefaultLengthOutOfRangeErrorTemplate;
            }

            return string.Format(CultureInfo.CurrentCulture, messageTemplate, name, MinimumLength, MaximumLength);
        }

        #endregion

        #region Implementation of IClientValidatable

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = GetRule(metadata, context);

            yield return rule;
        }

        #endregion

        #region Supports

        protected virtual ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var validationName = metadata.GetValidationName();

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(validationName),
                ValidationType = "length"
            };

            rule.ValidationParameters.Add("min", MinimumLength);
            rule.ValidationParameters.Add("max", MaximumLength);

            return rule;
        }

        #endregion
    }
}