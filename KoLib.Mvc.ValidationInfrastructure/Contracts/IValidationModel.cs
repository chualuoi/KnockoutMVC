using System.Collections.Generic;
using KoLib.Mvc.ValidationInfrastructure.Helpers;

namespace KoLib.Mvc.ValidationInfrastructure.Contracts
{
    public interface IValidationModel
    {
        /// <summary>
        /// Gets a list of inapplicable properties for the current model data
        /// </summary>
        /// <returns></returns>
        List<PropertyExpression> GetInapplicableProperties();

        /// <summary>
        /// Gets a list of locked readonly properties for the current model data
        /// </summary>
        /// <returns></returns>
        List<PropertyExpression> GetLockedReadOnlyProperties();

        /// <summary>
        /// Gets a list of computed readonly properties for the current model data
        /// </summary>
        /// <returns></returns>
        Dictionary<PropertyExpression, PropertyExpression> GetComputedReadOnlyProperties();
    }
}