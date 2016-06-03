using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class DateTimeRangeAttribute : RangeAttribute, IClientValidatable
    {
        public string Format { get; set; }

        public DateTimeRangeAttribute(int minimum, int maximum) : base(minimum, maximum)
        {
        }

        #region Overrides of RangeAttribute

        public override bool IsValid(object value)
        {
            var xvalue = value as DateTime?;
            if (!xvalue.HasValue)
            {
                return true;
            }

            var deviation = xvalue.Value - DateTime.Now;
            var ideviation = deviation.Days;

            return ideviation >= (int)Minimum && ideviation <= (int)Maximum;
        }

        public override string FormatErrorMessage(string name)
        {
            var imin = (int) Minimum;
            var imax = (int) Maximum;
            
            var format = !string.IsNullOrWhiteSpace(Format) ? Format : Constants.DefaultDateTimeFormat;

            var min = DateTime.Now.AddDays(imin).ToString(format);
            var max = DateTime.Now.AddDays(imax).ToString(format);

            return string.Format(ErrorMessageString ?? "The {0} must be between {1} and {2}", name, min, max);
        }

        #endregion

        #region Implementation of IClientValidatable

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                    ValidationType = "drange"
                };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }

        #endregion
    }
}