using System;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class DateRegexAttribute : CustomRegularExpressionAttribute
    {
        #region Ctors

        public DateRegexAttribute(string pattern) : base(pattern)
        {
        }

        #endregion

        #region Overrides of ValidationAttribute

        public override bool IsValid(object value)
        {
            var date = value as DateTime?;

            return date.HasValue;
        }

        protected override ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var rule = base.GetRule(metadata, context);

            rule.ValidationType = "dateregex";

            return rule;
        }

        #endregion

    }
}