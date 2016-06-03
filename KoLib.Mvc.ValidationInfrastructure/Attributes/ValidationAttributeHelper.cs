using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public static class ValidationAttributeHelper
    {
        public static bool CheckConditioner(string conditioner, ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(conditioner))
            {
                var conditionerProperty = validationContext.ObjectType.GetProperty(conditioner);
                if (conditionerProperty != null)
                {
                    var conditionerValue =
                        conditionerProperty.GetValue(validationContext.ObjectInstance, new object[] {}) as bool?;
                    if (!conditionerValue.HasValue || !conditionerValue.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string GetValidationName(this ModelMetadata metadata)
        {
            var validationName = Constants.DefaultValidationName;

            if (!string.IsNullOrWhiteSpace(metadata.ShortDisplayName) && !string.Equals(metadata.ShortDisplayName, metadata.DisplayName, StringComparison.Ordinal))
            {
                validationName = metadata.ShortDisplayName;
            }

            if (!string.IsNullOrWhiteSpace(metadata.ShortDisplayName) && !string.Equals(metadata.ShortDisplayName, metadata.DisplayName, StringComparison.Ordinal))
            {
                validationName = metadata.ShortDisplayName;
            }
            return validationName;
        }
    }
}
