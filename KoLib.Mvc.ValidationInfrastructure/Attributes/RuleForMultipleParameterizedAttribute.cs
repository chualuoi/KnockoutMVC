using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    /// <summary>
    /// RuleForAttribute which will be decorated to computed property represent a rule
    /// </summary>
    public class RuleForMultipleParameterizedAttribute : RuleForMultipleAttribute
    {
        #region Ctors

        public RuleForMultipleParameterizedAttribute(string[] properties, string errorTemplate, string[] parameters)
            : base(properties)
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

        #region 

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
                               ValidationType = "xruleforp"
                           };
            rule.ValidationParameters.Add("properties", Properties);
            rule.ValidationParameters.Add("etemplate", Etemplate);
            rule.ValidationParameters.Add("parameters", string.Join(",", Paramerters));
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