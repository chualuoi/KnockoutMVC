using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class DateTimeMaxAttribute : MaxAttribute
    {
        public string Format { get; set; }

        public DateTimeMaxAttribute(int maximum) : base(maximum)
        {
        }

        #region Overrides of MaxAttribute

        public override bool IsValid(object value)
        {
            var xvalue = value as DateTime?;
            if (!xvalue.HasValue)
            {
                return true;
            }

            var deviation = xvalue.Value - DateTime.Now;
            var ideviation = deviation.Days;

            return ideviation <= (int)Maximum;
        }

        public override string FormatErrorMessage(string name)
        {
            var imax = (int)Maximum;

            var format = !string.IsNullOrWhiteSpace(Format) ? Format : Constants.DefaultDateTimeFormat;

            var max = DateTime.Now.AddDays(imax).ToString(format);

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString ?? Constants.DefaultDateTooLargeErrorTemplate, name, max);
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName ?? metadata.PropertyName),
                    ValidationType = "dmax"
                };
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }

        #endregion
    }
}