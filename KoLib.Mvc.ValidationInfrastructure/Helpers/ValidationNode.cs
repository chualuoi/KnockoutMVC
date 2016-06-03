using System;
using System.Collections.Generic;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public abstract class ValidationNode<TModel> where TModel : class
    {
        private RuleExpression<TModel> applicableRule;
        private RuleExpression<TModel> readOnlyRule;

        protected ValidationNode(ValidationTree<TModel> parent)
        {
            Parent = parent;
        }

        public ValidationTree<TModel> Parent { get; set; }

        public RuleExpression<TModel> ApplicableRule
        {
            get
            {
                if (applicableRule == null)
                {
                    return new RuleExpression<TModel>(x => true);
                }
                return applicableRule;
            }
            set
            {
                if (applicableRule != null)
                {
                    throw new InvalidOperationException("applicable rule has been set");
                }
                applicableRule = value;
            }
        }

        public RuleExpression<TModel> ReadOnlyRule
        {
            get
            {
                if (readOnlyRule == null)
                {
                    return new RuleExpression<TModel>(x => false);
                }
                return readOnlyRule;
            }
            set
            {
                if (readOnlyRule != null)
                {
                    throw new InvalidOperationException("read only rule rule has been set");
                }
                readOnlyRule = value;
            }
        }

        public abstract bool IsLeaf { get; }

        public virtual IEnumerator<ValidationNode<TModel>> GetDepthFirstEnumerator()
        {
            yield return this;
        }
    }
}