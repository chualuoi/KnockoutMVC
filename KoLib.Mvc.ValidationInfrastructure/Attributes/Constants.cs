namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public static class Constants
    {
        /// <summary>
        /// Regex of Postcode
        /// </summary>
        public const string PostcodeRegex = "^(GIR 0AA)$|^([A-PR-UWYZ]((\\d(\\d|[A-HJKSTUW])?)|([A-HK-Y]\\d(\\d|[ABEHMNPRV-Y])?)) \\d[ABD-HJLNP-UW-Z]{2})$";

        /// <summary>
        /// Regex of CardNumber
        /// </summary>
        public const string CardNumberRegex = "^\\d{12,19}$";

        /// <summary>
        /// Regex of CardNumber is Master
        /// </summary>
        public const string CardNumberMasterRegex = "^5[1-5][0-9]{14}$";

        /// <summary>
        /// Regex of CardNumber is Visa
        /// </summary>
        public const string CardNumberVisaRegex = "^4[0-9]{12}(?:[0-9]{3})?$";

        /// <summary>
        /// Regex of CardNumber is Solo
        /// </summary>
        public const string CardNumberSoloRegex = "(^(6334)[5-9](\\d{11}$|\\d{13,14}$)) |(^(6767)(\\d{12}$|\\d{14,15}$))";

        /// <summary>
        /// Regex of BankSortCode
        /// </summary>
        public const string BankSortCodeRegex = "^\\d{2}[-\\s]?\\d{2}[-\\s]?\\d{2}$";

        /// <summary>
        /// Regex of Duration
        /// </summary>
        public const string DurationRegex = "^((0?[0-9]|[1-4][0-9])\\s(0?[0-9]|1[0-1])|(50\\s0))$";

        /// <summary>
        /// Regex of CustomDateTime
        /// </summary>
        public const string DateTimeRegex = @"(((0[1-9]|[12][0-9]|3[01])([-./])(0[13578]|10|12)([-./])(\d{4}))|(([0][1-9]|[12][0-9]|30)([-./])(0[469]|11)([-./])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([-./])(02)([-./])(\d{4}))|((29)(\.|-|\/)(02)([-./])([02468][048]00))|((29)([-./])(02)([-./])([13579][26]00))|((29)([-./])(02)([-./])([0-9][0-9][0][48]))|((29)([-./])(02)([-./])([0-9][0-9][2468][048]))|((29)([-./])(02)([-./])([0-9][0-9][13579][26])))";
        
        //public const string DurationRegex = "^\\d{1,2}\\s(0?[0-9]|1[0-1])$";
        /// <summary>
        /// Regex of CardDate
        /// </summary>
        public const string CardDateRegex = "^(0[1-9]|1[0-2])\\d{2}$";

        /// <summary>
        /// Regex of DefaultDateTime
        /// </summary>
        public const string DefaultDateTimeFormat = "dd/MM/yyyy";
        /// <summary>
        /// Regex of PostCode when Search
        /// </summary>
        public const string PostcodeRegexNotRequired =
            "^(GIR 0AA)$|^ $|^([A-PR-UWYZ]((\\d(\\d|[A-HJKSTUW])?)|([A-HK-Y]\\d(\\d|[ABEHMNPRV-Y])?)) \\d[ABD-HJLNP-UW-Z]{2})$";

        /// <summary>
        /// Digits only
        /// </summary>
        //public const string NumericRegex = "^£?[0-9]+$";
        public const string CurrencyRegex = @"^-?£?(\d+,?)+.?\d*$";
        //{0}?(\d+{1}?)+{2}?\d*

        /// <summary>
        /// Regex of Percentage 
        /// </summary>
        public const string PercentageyRegex = @"^-?(\d+)+.?\d*%?$";

        /// <summary>
        /// Default Validation Name
        /// </summary>
        public const string DefaultValidationName = @"value";
        
        /// <summary>
        /// Default Required Error Template Enter
        /// </summary>
        public const string DefaultRequiredErrorTemplateEnter = "Please enter a valid {0}";

        /// <summary>
        /// Default Required Error Template Select
        /// </summary>
        public const string DefaultRequiredErrorTemplateSelect = "Please make a selection";

        /// <summary>
        /// Default Validation Name
        /// </summary>
        public const string DefaultTooShortErrorTemplate = "Please enter a valid {0} no less than {1} characters long";

        /// <summary>
        /// Default Too Long Error Template
        /// </summary>
        public const string DefaultTooLongErrorTemplate = "Please enter a valid {0} no more than {1} characters long";

        /// <summary>
        /// Default Length Out Of Range ErrorT emplate
        /// </summary>
        public const string DefaultLengthOutOfRangeErrorTemplate = "Please enter a valid {0} between {1} and {2} characters long";

        /// <summary>
        /// Default Length Single Range Error Template
        /// </summary>
        public const string DefaultLengthSingleRangeErrorTemplate = "Please enter a valid {0} {1} characters long";

        /// <summary>
        /// Default Too Small Error Template
        /// </summary>
        public const string DefaultTooSmallErrorTemplate = "Please enter a valid {0} greater than or equal to {1}";

        /// <summary>
        /// Default Too Large Error Template
        /// </summary>
        public const string DefaultTooLargeErrorTemplate = "Please enter a valid {0} smaller than or equal to {1}";

        /// <summary>
        /// Default Size Out Of Range Error Template
        /// </summary>
        public const string DefaultSizeOutOfRangeErrorTemplate = "Please enter a valid {0} between {1} and {2}";

        /// <summary>
        /// Default Date Too Small Error Template
        /// </summary>
        public const string DefaultDateTooSmallErrorTemplate = "Please enter a valid {0} no earlier than {1}";

        /// <summary>
        /// Default Date Too Large Error Template
        /// </summary>
        public const string DefaultDateTooLargeErrorTemplate = "Please enter a valid {0} no later than {1}";

        /// <summary>
        /// Default error message when use enter invalid friendly case Id
        /// </summary>
        public const string DefaultInvalidFriendlyCaseIdErrorTemplate = "The case id is invalid.";
    }
}
