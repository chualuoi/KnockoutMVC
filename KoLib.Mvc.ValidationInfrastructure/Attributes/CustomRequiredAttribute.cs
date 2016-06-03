using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public enum EntryType
    {
        /// <summary>
        /// Indicates the input type is one of textarea
        /// </summary>
        TextArea,

        /// <summary>
        /// Indicates the input type is one of text
        /// </summary>
        Text,

        /// <summary>
        /// Indicates the input type is one of select
        /// </summary>
        Select,

        /// <summary>
        /// Indicates the input type is one of checkbox
        /// </summary>
        Checkbox,

        /// <summary>
        /// Indicates the input type is one of radio
        /// </summary>
        RadioGroup
    }

    public class CustomRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        #region Properties & Fields

        /// <summary>
        /// Parameters for custom error message on client
        /// </summary>
        public virtual string[] Paramerters { get; set; }

        /// <summary>
        /// The name of the boolean field/property indicates whether to apply this validation or not
        /// </summary>
        public string Conditioner { get; set; }

        public EntryType Input { get; set; }

        #endregion

        #region Overrides of RequiredAttribute

        /// <summary>
        /// Checks if the marked property has value when the indicating property has the desired value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var shouldValidate = ValidationAttributeHelper.CheckConditioner(Conditioner, validationContext);
            if (!shouldValidate)
            {
                return ValidationResult.Success;
            }

            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            var messageTemplate = ErrorMessageString;
            if (string.IsNullOrEmpty(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessageResourceName))
            {
                switch (Input)
                {
                    case EntryType.TextArea:
                    case EntryType.Text:
                        messageTemplate = Constants.DefaultRequiredErrorTemplateEnter;
                        break;
                    case EntryType.Select:
                    case EntryType.Checkbox:
                    case EntryType.RadioGroup:
                        messageTemplate = Constants.DefaultRequiredErrorTemplateSelect;
                        break;
                }
            }

            return string.Format(CultureInfo.CurrentCulture, messageTemplate, name);
        }

        #endregion

        #region Implementations of IClientValidatable

        /// <summary>
        /// Supports fo client validation
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
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
                    ValidationType = "frequired"
                };

            rule.ValidationParameters.Add("conditioner", Conditioner ?? string.Empty);
            rule.ValidationParameters.Add("parameters", string.Join(",", Paramerters ?? new string[0]));
            rule.ValidationParameters.Add("etemplate", Paramerters != null && Paramerters.Length != 0 ? ErrorMessageString : string.Empty);

            return rule;
        }

        #endregion

    }
}