using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class MustFalseParameterizedAttribute : CustomRequiredAttribute
    {
        /// <summary>
        /// Template for custom error message on client
        /// </summary>
        protected virtual string Etemplate { get; set; }

        /// <summary>
        /// Parameters for custom error message on client
        /// </summary>
        public override string[] Paramerters { get; set; }

        public MustFalseParameterizedAttribute(string errorTemplate, string[] parameters)
        {
            if (string.IsNullOrWhiteSpace(errorTemplate)) {
                throw new ArgumentException("Invalid error template");
            }

            if (parameters == null || parameters.Length == 0)
            {
                throw new ArgumentException("Invalid error parameters");
            }

            Etemplate = errorTemplate;
            Paramerters = parameters;
        }

        protected override ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var validationName = metadata.GetValidationName();

            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(validationName),
                    ValidationType = "xfalsep"
                };

            rule.ValidationParameters.Add("etemplate", Etemplate);
            rule.ValidationParameters.Add("parameters", string.Join(",", Paramerters));

            return rule;
        }
    }
}