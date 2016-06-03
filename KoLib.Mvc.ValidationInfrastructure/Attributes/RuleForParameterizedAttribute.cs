using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    /// <summary>
    /// RuleForAttribute which will be decorated to computed property represent a rule
    /// </summary>
    public class RuleForParameterizedAttribute : RuleForAttribute
    {
        #region Ctors

        public RuleForParameterizedAttribute(string property, string errorTemplate, string[] parameters)
            : base(property)
        {
            if (string.IsNullOrWhiteSpace(errorTemplate))
            {
                throw new ArgumentException("Invalid error template");
            }

            if (parameters == null || parameters.Length == 0)
            {
                throw new ArgumentException("Invalid error parameters");
            }

            Etemplate = errorTemplate;
            Paramerters = parameters;
        }

        #endregion

        #region Properties & Fields

        /// <summary>
        /// Template for custom error message on client
        /// </summary>
        protected virtual string Etemplate { get; set; }

        /// <summary>
        /// Parameters for custom error message on client
        /// </summary>
        protected virtual string[] Paramerters { get; set; }

        #endregion

        #region IClientValidatable Members

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                                        ControllerContext context)
        {
            var validationName = metadata.GetValidationName();

            var rule = new ModelClientValidationRule
                           {
                               ErrorMessage = FormatErrorMessage(validationName),
                               ValidationType = "ruleforp"
                           };
            rule.ValidationParameters.Add("property", Property);
            rule.ValidationParameters.Add("etemplate", Etemplate);
            rule.ValidationParameters.Add("parameters", string.Join(",", Paramerters));
            yield return rule;
        }

        #endregion
    }
}