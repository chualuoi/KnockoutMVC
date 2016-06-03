using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ko.Core
{
    /// <summary>
    /// Provides helpers for converting C# Expression tree to javascript expression
    /// </summary>
    public static class KnockoutHelper
    {
        /// <summary>
        /// Converts C# expression
        /// to Knockout binding expression
        /// </summary>
        /// <typeparam name="TModel">
        /// Model type
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Property type
        /// </typeparam>
        /// <param name="expression">
        /// Expression to get member name
        /// </param>
        /// <returns>
        /// Knockout binding expression
        /// </returns>
        public static string ExtractExpression<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var param = expression.Parameters[0].Name;
            string result;

            var visitor = expression.Body;
            //Case: when we try to access member but its type is not object
            if (visitor.NodeType == ExpressionType.Convert)
            {
                var unaryExpression = (UnaryExpression)visitor;
                visitor = unaryExpression.Operand;
            }

            switch (visitor.NodeType)
            {
                case ExpressionType.Parameter:
                    result = KnockoutContants.CurrentContext;
                    break;
                case ExpressionType.MemberAccess:
                    result = AccessMember((MemberExpression)visitor, param);
                    break;
                default:
                    result = VisitExpression(visitor, param);
                    break;
            }
            return result;
        }

        public static bool IsStaticMember(MemberExpression expression)
        {
            var isStatic = false;
            if (expression.Member.DeclaringType != null)
            {
                var field = expression.Member.DeclaringType.GetField(expression.Member.Name, BindingFlags.Static | BindingFlags.Public);
                if (field == null)
                {
                    var property = expression.Member.DeclaringType.GetProperty(expression.Member.Name, BindingFlags.Static | BindingFlags.Public);
                    isStatic = property != null;
                }
                else
                {
                    isStatic = true;
                }
            }
            return isStatic;
        }

        #region Supports

        /// <summary>
        /// Visits the expressiosn tree to generate the expression in javascript
        /// </summary>
        /// <param name="expression">
        /// Body of input expression
        /// </param>
        /// <param name="param">
        /// Param of input expression
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Throws when the expression is not able/very hard to convert to JavaScript.
        /// So keep the binding expression as simple as possible.
        /// For complex expression, use computed property instead. 
        /// </exception>
        /// <returns></returns>
        private static string VisitExpression(Expression expression, string param)
        {
            var stringBuilder = new StringBuilder(string.Empty);
            var operatorInUse = string.Empty;
            bool? isBinary = true;
            var priorityIndicator = false;
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    isBinary = null;
                    stringBuilder.Append(KnockoutContants.CurrentContext + KnockoutContants.FunctionEnclosing);
                    break;
                case ExpressionType.MemberAccess:
                    isBinary = null;
                    var memberCallingChain = AccessMember((MemberExpression)expression, param, true);
                    stringBuilder.Append(memberCallingChain);
                    break;
                case ExpressionType.Constant:
                    isBinary = null;
                    var constantExpression = (ConstantExpression)expression;
                    var value = constantExpression.Value;
                    if (constantExpression.Type == typeof(string))
                    {
                        //Escapses HTML special characters
                        var svalue = value.ToString();
                        svalue = svalue
                            .Replace(@"\", @"\\")
                            .Replace(@"'", @"\'")
                            .Replace(@"""", @"&quot;")
                            ;
                        stringBuilder.AppendFormat("'{0}'", svalue);
                    }
                    else if (constantExpression.Type == typeof(bool))
                    {
                        //To javascript bool value
                        var @bool = value.ToString().ToLower();
                        stringBuilder.Append(@bool);
                    }
                    else
                    {
                        stringBuilder.Append(value);
                    }
                    break;
                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return VisitExpression(unaryExpression.Operand, param);
                case ExpressionType.LessThan:
                    operatorInUse = JavaScriptOperator.LessThan;
                    break;
                case ExpressionType.LessThanOrEqual:
                    operatorInUse = JavaScriptOperator.LessThanOrEqual;
                    break;
                case ExpressionType.GreaterThan:
                    operatorInUse = JavaScriptOperator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    operatorInUse = JavaScriptOperator.GreaterThanOrEqual;
                    break;
                case ExpressionType.OrElse:
                    priorityIndicator = true;
                    operatorInUse = JavaScriptOperator.LogicOr;
                    break;
                case ExpressionType.AndAlso:
                    operatorInUse = JavaScriptOperator.LogicAnd;
                    break;
                case ExpressionType.Equal:
                    operatorInUse = JavaScriptOperator.Equal;
                    break;
                case ExpressionType.NotEqual:
                    operatorInUse = JavaScriptOperator.NotEqual;
                    break;
                case ExpressionType.Add:
                    priorityIndicator = true;
                    operatorInUse = JavaScriptOperator.Add;
                    break;
                case ExpressionType.Subtract:
                    priorityIndicator = true;
                    operatorInUse = JavaScriptOperator.Subtract;
                    break;
                case ExpressionType.Modulo:
                    operatorInUse = JavaScriptOperator.Modulo;
                    break;
                case ExpressionType.Multiply:
                    operatorInUse = JavaScriptOperator.Multiply;
                    break;
                case ExpressionType.Divide:
                    operatorInUse = JavaScriptOperator.Divide;
                    break;
                case ExpressionType.Coalesce:
                    operatorInUse = JavaScriptOperator.LogicOr;
                    priorityIndicator = true;
                    break;
                case ExpressionType.Not:
                    operatorInUse = JavaScriptOperator.Not;
                    isBinary = false;
                    break;
                default:
                    throw new NotImplementedException("Invalid Expression Type");
            }

            if (isBinary.HasValue)
            {
                if (isBinary.Value)
                {
                    var binaryExpression = (BinaryExpression)expression;
                    var left = VisitExpression(binaryExpression.Left, param);
                    var right = VisitExpression(binaryExpression.Right, param);
                    if (priorityIndicator)
                    {
                        stringBuilder.Append('(');
                    }
                    stringBuilder.Append(left);
                    stringBuilder.Append(operatorInUse);
                    stringBuilder.Append(right);
                    if (priorityIndicator)
                    {
                        stringBuilder.Append(')');
                    }

                }
                else
                {
                    var unaryExpression = (UnaryExpression)expression;
                    var operand = VisitExpression(unaryExpression.Operand, param);
                    stringBuilder.Append(operatorInUse);
                    stringBuilder.Append("(");
                    stringBuilder.Append(operand);
                    stringBuilder.Append(")");

                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Uses to check member information
        /// </summary>
        /// <param name="expression">
        /// Member expression
        /// </param>
        /// <param name="param">
        /// Param name
        /// </param>
        /// <param name="endWithParentheses">
        /// Ends the member calling cha
        /// </param>
        /// <returns></returns>
        private static string AccessMember(MemberExpression expression, string param, bool endWithParentheses = false)
        {
            var isStatic = IsStaticMember(expression);
            if (isStatic)
            {
                //Get value of the member
                switch (expression.Member.MemberType)
                {
                    case MemberTypes.Property: 
                        var prop = (PropertyInfo)expression.Member;
                        return string.Format("'{0}'", prop.GetValue(null, null));

                    case MemberTypes.Field: 
                        var field = (FieldInfo)expression.Member;
                        return string.Format("'{0}'", field.GetValue(null));
                }
            }
            var result = expression.ToString();
            result = ReformatMemberCallingChain(result, param);
            if (endWithParentheses)
            {
                result += KnockoutContants.FunctionEnclosing;
            }
            return result;
        }

        /// <summary>
        /// Convert the C# member calling chain to Knockout member binding chain
        /// E.g: OuterMember.InnerMember => OuterMember().InnerMember
        /// </summary>
        /// <param name="memberCallingChain">
        /// C# calling chain
        /// </param>
        /// <param name="param">
        /// Param of input expression
        /// </param>
        /// <returns>
        /// Knockout binding chain
        /// </returns>
        public static string ReformatMemberCallingChain(string memberCallingChain, string param="x")
        {
            //
            //These variables is to ignore {param}.Ko{knockOutVariableName} in the calling chain
            //Then the ignored things will be replaced with correct Knockout variable name
            //

            //Member of Contexts in use
            var rootOriginal = string.Format("{0}.KoRoot.", param);
            var parentOriginal = string.Format("{0}.KoParent.", param);
            var currentOriginal = string.Format("{0}.KoData.", param);
            var indexOriginal = string.Format("{0}.KoIndex", param);

            //Placerholder
            var rootPlaceholder = string.Format("{0}_", KnockoutContants.RootContext);
            var parentPlaceholder = string.Format("{0}_", KnockoutContants.ParentContext);
            var currentPlaceholder = string.Format("{0}_", KnockoutContants.CurrentContext);

            bool reserved;
            //Reserves place holder for $root context if any
            memberCallingChain = ReservePlaceHolder(memberCallingChain, rootOriginal, rootPlaceholder, out  reserved);

            //Reserves place holder for $parent context if any
            memberCallingChain = reserved ? memberCallingChain : ReservePlaceHolder(memberCallingChain, parentOriginal, parentPlaceholder, out  reserved);

            //Reserves place holder for $data context if any
            memberCallingChain = reserved ? memberCallingChain : ReservePlaceHolder(memberCallingChain, currentOriginal, currentPlaceholder, out  reserved);

            //Contexts in use without calling to lower level
            var rootContextCalled = string.Format("{0}.KoRoot", param);
            var parentContextCalled = string.Format("{0}.KoParent", param);
            var currentContextCalled = string.Format("{0}.KoData", param);

            //Reserves place holder for $root context if any
            memberCallingChain = reserved ? memberCallingChain : ReservePlaceHolder(memberCallingChain, rootContextCalled, KnockoutContants.RootContext, out  reserved);

            //Reserves place holder for $parent context if any
            memberCallingChain = reserved ? memberCallingChain : ReservePlaceHolder(memberCallingChain, parentContextCalled, KnockoutContants.ParentContext, out  reserved);

            //Reserves place holder for $data context if any
            memberCallingChain = reserved ? memberCallingChain : ReservePlaceHolder(memberCallingChain, currentContextCalled, KnockoutContants.CurrentContext, out  reserved);

            //Reserves place holder for $index variable if any
            memberCallingChain = reserved ? memberCallingChain : ReservePlaceHolder(memberCallingChain, indexOriginal, KnockoutContants.Index, out  reserved);

            if (!reserved && memberCallingChain.StartsWith(param))
            {
                memberCallingChain = memberCallingChain.Remove(0, param.Length);
                if (memberCallingChain.StartsWith("."))
                {
                    memberCallingChain = "_" + memberCallingChain.Remove(0, 1);

                }
                memberCallingChain = KnockoutContants.CurrentContext + memberCallingChain;
            }

            memberCallingChain = memberCallingChain
                .Replace(KnockoutContants.Dot, KnockoutContants.FunctionEnclosing + KnockoutContants.Dot)
                .Replace(rootPlaceholder, KnockoutContants.RootContext + KnockoutContants.Dot)
                .Replace(parentPlaceholder, KnockoutContants.ParentContext + KnockoutContants.Dot)
                .Replace(currentPlaceholder, string.Empty)
                ;
            return memberCallingChain;
        }

        /// <summary>
        /// Uses to reserve placeholder for Knockout special variables
        /// </summary>
        /// <param name="input">
        /// Given string
        /// </param>
        /// <param name="toReserve">
        /// String to reserve a placeholder
        /// </param>
        /// <param name="placeHolder">
        /// Placeholder
        /// </param>
        /// <param name="reserved">
        /// Indicates that a placeholder is reserved
        /// </param>
        /// <returns>
        /// String with placeholder
        /// </returns>
        private static string ReservePlaceHolder(string input, string toReserve, string placeHolder, out bool reserved)
        {
            var result = input;
            reserved = input.StartsWith(toReserve);
            if (reserved)
            {
                result = placeHolder + input.Remove(0, toReserve.Length);
            }
            return result;
        }

        #endregion

    }
}