using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public static class TreeHelpers
    {
        public static Expression<Func<TModel, bool>> GetActualExpression<TModel>(this Expression<Func<TModel, bool>> expression) where  TModel : class 
        {
            if (expression.Body.NodeType == ExpressionType.Constant)
            {
                return expression;
            }
            if (expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException("expression must be member access expression", "expression");
            }
            //Check whether this class has inner static Rules class
            var modelType = typeof(TModel);

            var ruleName = ExpressionHelper.GetExpressionText(expression);
            var ruleTypeName = modelType.FullName + "+R";
            var ruleType = modelType.Assembly.GetType(ruleTypeName);
            //If this class does not contain Rules class and expression is member access --> it is the actual expression.
            if (ruleType == null)
            {
                return expression;
            }
            //Get rule value
            var rulePropInfo = ruleType.GetField(ruleName);
            //If this class contains Rules class but does not have rule with the same name --> it is the actual expression.
            if (rulePropInfo == null)
            {
                return expression;
            }
            return (RuleExpression<TModel>)rulePropInfo.GetValue(null);
        }
    }
}