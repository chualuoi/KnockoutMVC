using System.Globalization;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public enum MinLengthType
    {
        TooShort,
        Single
    }

    public class MinStringLengthAttribute : CustomStringLengthAttribute
    {
        #region Properties & Fields

        public MinLengthType Type { get; set; }

        private readonly int minimumLength;

        #endregion

        #region Ctors

        public MinStringLengthAttribute(int minimumLength)
            : base(int.MaxValue)
        {
            this.minimumLength = minimumLength;
            MinimumLength = minimumLength;
            Type = MinLengthType.TooShort;
        }

        #endregion

        #region Overrides of CustomStringLengthAttribute

        public override string FormatErrorMessage(string name)
        {
            var messageTemplate = ErrorMessageString;

            if (string.IsNullOrWhiteSpace(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessageResourceName))
            {
                switch (Type)
                {
                    case MinLengthType.TooShort:
                        messageTemplate = Constants.DefaultTooShortErrorTemplate;
                        break;
                    case MinLengthType.Single:
                        messageTemplate = Constants.DefaultLengthSingleRangeErrorTemplate;
                        break;
                }
            }

            return string.Format(CultureInfo.CurrentCulture, messageTemplate, name, MinimumLength);
        }

        public override bool IsValid(object value)
        {
            MinimumLength = minimumLength;

            return base.IsValid(value);
        }

        protected override ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var rule = base.GetRule(metadata, context);

            rule.ValidationParameters.Remove("max");

            return rule;
        }

        #endregion
    }
}