using System;
using System.Linq.Expressions;
using System.Reflection;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo AsPropertyInfo(this LambdaExpression expression)
        {
            PropertyInfo info = null;
            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                info = ((MemberExpression) expression.Body).Member as PropertyInfo;
            }
            return info;
        }

        public static object Default(this Type type)
        {
            var value = type.IsValueType ? Activator.CreateInstance(type) : null;

            return value;
        }
    }

    public class PropertyExpression
    {
        private readonly LambdaExpression _expression;


        private readonly Delegate _getter;
        public PropertyInfo PropertyInfo { get; private set; }    
        private readonly Delegate _setter;

        public PropertyExpression(LambdaExpression expression)
        {
            _expression = expression;
            _getter = expression.Compile();
            _setter = GetParentExpression(expression).Compile();
            PropertyInfo = expression.AsPropertyInfo();
        }

        public LambdaExpression LambdaExpression
        {
            get { return _expression; }
        }

        public object GetValue(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            return _getter.DynamicInvoke(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            object realTarget = _setter.DynamicInvoke(obj);
            if (PropertyInfo.GetSetMethod(true) != null)
            {
                PropertyInfo.SetValue(realTarget, value, null);
            }
        }

        private LambdaExpression GetParentExpression(LambdaExpression targetExpression)
        {
            if (targetExpression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = targetExpression.Body as MemberExpression;
                if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                {
                    ParameterExpression parameter = GetParameterExpression(memberExpression.Expression);
                    if (parameter != null)
                    {
                        return Expression.Lambda(memberExpression.Expression, parameter);
                    }
                }
                else
                {
                    return Expression.Lambda(memberExpression.Expression, memberExpression.Expression as ParameterExpression);
                }
            }
            throw new ArgumentException("Not supported");
        }

        private ParameterExpression GetParameterExpression(Expression expression)
        {
            while (expression.NodeType == ExpressionType.MemberAccess)
            {
                expression = ((MemberExpression) expression).Expression;
            }
            if (expression.NodeType == ExpressionType.Parameter)
            {
                return (ParameterExpression) expression;
            }
            return null;
        }
    }
}