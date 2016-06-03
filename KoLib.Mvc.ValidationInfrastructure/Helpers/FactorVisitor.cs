using System.Collections.Generic;
using System.Linq.Expressions;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public class FactorVisitor : ExpressionVisitor
    {
        List<Expression> factors = new List<Expression>();

        private FactorVisitor(Expression expression)
        {
            Visit(expression);
        }

        public static List<Expression> GetFactors(Expression expression)
        {
            return new FactorVisitor(expression).factors;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.Name == "HasValue" || node.Member.Name == "Value")
            {
                factors.Add(node.Expression);
            }
            else
            {
                factors.Add(node);
            }

            return node;
        }
    }
}