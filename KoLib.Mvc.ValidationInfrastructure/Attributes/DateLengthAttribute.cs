using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class DateLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        public string Conditioner { get; set; }

        public DateLengthAttribute(int maximumLength) : base(maximumLength)
        {
        }

        #region Overrides of StringLengthAttribute

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var shouldValidate = ValidationAttributeHelper.CheckConditioner(Conditioner, validationContext);
            if (!shouldValidate)
            {
                return ValidationResult.Success;
            }

            var date = value as DateTime?;
            var svalue = date.HasValue ? date.Value.ToString(CultureInfo.InvariantCulture) : null;
            return base.IsValid(svalue, validationContext);
        }

        #endregion

        #region Implementation of IClientValidatable

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = GetRule(metadata, context);

            yield return rule;
        }

        protected virtual ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var fieldName = metadata.GetValidationName();

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(fieldName),
                ValidationType = "xlength"
            };

            rule.ValidationParameters.Add("conditioner", Conditioner);
            rule.ValidationParameters.Add("min", MinimumLength);
            rule.ValidationParameters.Add("max", MaximumLength);

            return rule;
        }
        #endregion
    }
}