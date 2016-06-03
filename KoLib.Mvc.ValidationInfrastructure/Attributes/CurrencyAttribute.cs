using System.Globalization;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class CurrencyAttribute : CustomRegularExpressionAttribute
    {
        #region Overrides of ValidationAttribute

        public CurrencyAttribute(string pattern=".*") : base(pattern)
        {
        }

        public override bool IsValid(object value)
        {
            var decimalValue = value as decimal?;
            if (!decimalValue.HasValue || string.IsNullOrWhiteSpace(Pattern))
            {
                return true;
            }
            var stringValue = decimalValue.Value.ToString(CultureInfo.InvariantCulture);
            return base.IsValid(stringValue);
        }

        #endregion
    }
}