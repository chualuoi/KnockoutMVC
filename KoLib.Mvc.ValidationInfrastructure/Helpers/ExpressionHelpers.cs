using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public static class ExpressionHelpers
    {
        public static string GetExpressionText<TModel>(this Expression<Func<TModel, object>> lamda)
        {
            //TODO: Need to prevent lamda expression is not a member access
            
            if (lamda.Body.NodeType == ExpressionType.Convert)
            {
                string text = lamda.Body.ToString();
                text = text.Substring(8, text.Length - 9);
                return text.Substring(text.IndexOf('.') + 1);
            }
            return ExpressionHelper.GetExpressionText(lamda);
        }

        public static string GetExpressionText(this MemberExpression expression)
        {
            string text = expression.ToString();
            return text.Substring(text.IndexOf('.') + 1);
        }
    }
}