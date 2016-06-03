using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class CustomRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public CustomRegularExpressionAttribute(string pattern) 
            : base(pattern)
        {
        }

        #region Overrides of RegularExpressionAttribute

        public override string FormatErrorMessage(string name)
        {
            var messageTemplate = ErrorMessageString;
            if (string.IsNullOrWhiteSpace(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessageResourceName))
            {
                messageTemplate = Constants.DefaultRequiredErrorTemplateEnter;
            }

            return string.Format(CultureInfo.CurrentCulture, messageTemplate, name);
        }

        public override bool IsValid(object value)
        {
            if(value == null)
            {
                return true;
            }
            return base.IsValid(value);
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
                ValidationType = "fregex"
            };

            rule.ValidationParameters.Add("pattern", Pattern);

            return rule;
        }

        #endregion

    }
}
