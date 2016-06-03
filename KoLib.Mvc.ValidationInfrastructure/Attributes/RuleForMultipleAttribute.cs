using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    /// <summary>
    /// RuleForAttribute which will be decorated to computed properties represent a rule
    /// </summary>
    public class RuleForMultipleAttribute : ValidationAttribute, IClientValidatable
    {
        public RuleForMultipleAttribute(string[] properties)
        {
            Properties = properties ?? new string[0];
        }

        public RuleForMultipleAttribute(string[] properties, string message)
        {
            Properties = properties;
            Message = message;
        }

        /// <summary>
        /// Gets or sets the actual properties that this rule is applied for.
        /// </summary>
        /// <value>
        /// The actual properties.
        /// </value>
        public string[] Properties { get; set; }

        /// <summary>
        /// The message text used to warning of the Property
        /// </summary>
        public string Message { get; set; }

        #region IClientValidatable Members

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                                       ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                    ValidationType = "xrulefor"
                };
            rule.ValidationParameters.Add("properties", string.Join(",", Properties));
            yield return rule;
        }

        #endregion

        public override bool IsValid(object value)
        {
            var bvalue = value as bool?;
            return !(bvalue.HasValue && bvalue.Value);
        }
    }
}