using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public class CollectionRule<TModel>
    {
        public PropertyExpression HoldingValueExpression { get; private set; }
        public PropertyExpression CollectionExpression { get; private set; }
        public CollectionRuleExpression<TModel> RuleExpression { get; private set; }
        public CollectionRule(CollectionRuleExpression<TModel> ruleExpression)
        {
            RuleExpression = ruleExpression;
        }

        public CollectionRule<TModel> With<TItem>(Expression<Func<TModel, IEnumerable<TItem>>> collectionExpression, Expression<Func<TItem, bool>> holdingValueExpression)
        {
            CollectionExpression = new PropertyExpression(collectionExpression);
            HoldingValueExpression = new PropertyExpression(holdingValueExpression);
            return this;
        }
    }
}