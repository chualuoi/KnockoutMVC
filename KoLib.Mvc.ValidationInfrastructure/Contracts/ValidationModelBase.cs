using System.Collections.Generic;
using Avelo.Web.Mvc.ValidationInfrastructure.Helpers;

namespace Avelo.Web.Mvc.ValidationInfrastructure.Contracts
{ 
    public abstract class ValidationModelBase<TModel> : IValidationModel where TModel : class
    {
        public abstract ValidationTree<TModel> InstanceModelTree { get; }

        public List<PropertyExpression> GetInapplicableProperties()
        {
            return InstanceModelTree.GetInapplicableProperties(this as TModel);
        }

        public List<PropertyExpression> GetLockedReadOnlyProperties()
        {
            return InstanceModelTree.GetLockedReadOnlyProperties(this as TModel);
        }

        public Dictionary<PropertyExpression, PropertyExpression> GetComputedReadOnlyProperties()
        {
            return InstanceModelTree.GetComputedReadOnlyProperties(this as TModel);
        }

        public bool IsModelReadOnly()
        {
            return InstanceModelTree.ReadOnlyRule.IsTrue(this as TModel);
        }
    }
}