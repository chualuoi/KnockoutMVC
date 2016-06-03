using System;
using System.Linq.Expressions;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public class RuleExpression<TModel>  where TModel : class
    {
        private readonly Expression<Func<TModel, bool>> expression;
        private readonly Func<TModel, bool> rule;

        public RuleExpression(Expression<Func<TModel, bool>> expression)
        {
            if (expression==null)
                throw new ArgumentNullException("expression");
            this.expression = expression;
            rule = expression.Compile();

        }

        public Expression<Func<TModel, bool>> Expression
        {
            get { return expression; }
        }

        public static implicit operator Expression<Func<TModel, bool>>(RuleExpression<TModel> rule)
        {
            return rule.expression;
        }

        public static implicit operator RuleExpression<TModel>(Expression<Func<TModel, bool>> expression)
        {
            return new RuleExpression<TModel>(expression);
        }

        public bool IsTrue(TModel model)
        {
            if (model==null)
                throw new ArgumentNullException("model");
            return rule(model);
        }

    }
}