using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    /// <summary>
    /// RuleForAttribute which will be decorated to computed property represent a rule
    /// </summary>
    public class RuleForAttribute : ValidationAttribute, IClientValidatable
    {
        public RuleForAttribute(string property)
        {
            Property = property;
        }
        
        /// <summary>
        /// Gets or sets the actual property that this rule is applied for.
        /// </summary>
        /// <value>
        /// The actual property.
        /// </value>
        public string Property { get; protected set; }

        #region IClientValidatable Members

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                                       ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                    ValidationType = "rulefor"
                };
            rule.ValidationParameters.Add("property", Property);
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