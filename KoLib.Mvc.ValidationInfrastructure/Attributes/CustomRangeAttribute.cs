using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class CustomRangeAttribute : RangeAttribute, IClientValidatable
    {
        #region Properties & Fields

        /// <summary>
        /// Template for custom error message on client
        /// </summary>
        protected string etemplate;

        #endregion

        #region Ctors

        public CustomRangeAttribute(int minimum, int maximum) : base(minimum, maximum)
        {
        }

        public CustomRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        public CustomRangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }

        #endregion

        #region Overrides of RangeAttribute

        public override string FormatErrorMessage(string name)
        {
            var messageTemplate = ErrorMessageString;

            if (string.IsNullOrWhiteSpace(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessageResourceName))
            {
                messageTemplate = Constants.DefaultSizeOutOfRangeErrorTemplate;
            }

            etemplate = string.Format(CultureInfo.CurrentCulture, messageTemplate, name, "{0}", "{1}");

            return string.Format(CultureInfo.CurrentCulture, messageTemplate, name, Minimum, Maximum);
        }

        #endregion

        #region Implementation of IClientValidatable

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
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
                ValidationType = "frange"
            };

            rule.ValidationParameters.Add("etemplate", etemplate);
            rule.ValidationParameters.Add("max", Maximum);
            rule.ValidationParameters.Add("min", Minimum);
            return rule;
        }

        #endregion

    }
}