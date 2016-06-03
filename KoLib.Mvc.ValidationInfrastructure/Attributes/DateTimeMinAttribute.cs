using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class DateTimeMinAttribute : MinAttribute
    {
        public string Format { get; set; }

        public DateTimeMinAttribute(int minimum) : base(minimum)
        {
        }

        #region Overrides of MinAttribute

        public override bool IsValid(object value)
        {
            var xvalue = value as DateTime?;
            if (!xvalue.HasValue)
            {
                return true;
            }

            var deviation = xvalue.Value - DateTime.Now;
            var ideviation = deviation.Days;

            return ideviation >= (int)Minimum;
        }

        public override string FormatErrorMessage(string name)
        {
            var imin = (int)Minimum;

            var format = !string.IsNullOrWhiteSpace(Format) ? Format : Constants.DefaultDateTimeFormat;

            var min = DateTime.Now.AddDays(imin).ToString(format);

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString ?? Constants.DefaultDateTooSmallErrorTemplate, name, min);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                    ValidationType = "dmax"
                };
            rule.ValidationParameters.Add("min", Minimum);

            yield return rule;
        }

        #endregion
    }
}