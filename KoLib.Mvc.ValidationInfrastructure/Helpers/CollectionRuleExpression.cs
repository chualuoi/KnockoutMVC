using System;
using System.Linq.Expressions;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public class CollectionRuleExpression<TModel>
    {
        private readonly Expression<Func<TModel, int, bool>> expression;
        private readonly Func<TModel, int, bool> rule;

        public CollectionRuleExpression(Expression<Func<TModel, int, bool>> expression)
        {
            this.expression = expression;
            rule = expression.Compile();
        }

        public Expression<Func<TModel, int, bool>> Expression
        {
            get { return expression; }
        }

        public static implicit operator Expression<Func<TModel, int, bool>>(CollectionRuleExpression<TModel> rule)
        {
            return rule.expression;
        }

        public static implicit operator CollectionRuleExpression<TModel>(Expression<Func<TModel, int, bool>> expression)
        {
            return new CollectionRuleExpression<TModel>(expression);
        }

        public bool IsTrue(TModel model, int index)
        {
            return rule(model, index);
        }       
        public bool IsTrue(object model, int index)
        {
            //TODO: should check type of object before call rule(...)

            return rule((TModel)model, index);
        }
    }
}