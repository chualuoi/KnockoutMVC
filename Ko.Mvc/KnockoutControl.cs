using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Ko.Core;
using Ko.Core.BindingAttributes;

namespace Ko.Mvc
{

    #region Delegates

    public delegate MvcHtmlString ControlGeneratorDelegate<TModel, TProperty>(
        HtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        IDictionary<string, object> addtionalHtmlAttributes
        );

    public delegate MvcHtmlString ControlGeneratorDelegate(
        HtmlHelper html,
        string expression,
        object value,
        IDictionary<string, object> addtionalHtmlAttributes
        );

    #endregion

    public class KnockoutControl<TContext> : KoBindings<TContext>
    {
        
    }

    public class KnockoutControl<TContext, TModel, TProperty, TBinding>
        : KnockoutControl<TContext>
        where TBinding  : KnockoutBinding, new()
    {
        #region Fields

        private readonly ControlGeneratorDelegate<TModel, TProperty> controlGenerator;
        private readonly IDictionary<string, object> addtionalHtmlAttributes;
        private readonly Expression<Func<TModel, TProperty>> expression;
        private readonly HtmlHelper<TModel> html;

        #endregion

        #region Ctors

        public KnockoutControl(
            ControlGeneratorDelegate<TModel, TProperty> controlGenerator, 
            HtmlHelper<TModel> html, 
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> addtionalHtmlAttributes)
        {
            this.controlGenerator = controlGenerator;

            this.addtionalHtmlAttributes = addtionalHtmlAttributes;

            this.expression = expression;

            this.html = html;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            var bindingValue = KnockoutHelper.ExtractExpression(expression);
            var xadditionalHtmlAttributes = AddDataBindAttribute(bindingValue, addtionalHtmlAttributes);

            return controlGenerator(html, expression, xadditionalHtmlAttributes).ToHtmlString();
        }

        #endregion

        #region Supports

        private IDictionary<string, object> AddDataBindAttribute(
            string bindingValue,
            IDictionary<string, object> additionalHtmlAttributes)
        {
            if (html.IsForeach())
            {
                var modelName = html.NameForModel();
                var modelId = html.IdForModel();
                var bindingValueWithoutParentheses = bindingValue.Replace("(", string.Empty).Replace(")", string.Empty);
                var bindingValueWithoutParenthesesWithoutDot = bindingValueWithoutParentheses.Replace(".", "_");
                var nameBindingValue = string.Format("'{0}[' + $index + '].{1}'", modelName,
                                                     bindingValueWithoutParentheses);
                var idBindingValue = string.Format("'{0}_' + $index + '__{1}'", modelId,
                                                   bindingValueWithoutParenthesesWithoutDot);
                var attrBinding = new AttrBinding()
                    .AddChild(new NameBinding { Value = nameBindingValue })
                    .AddChild(new LazyBinding("id") { Value = idBindingValue });
                var eattrBinding = Get(attrBinding.Name) as ComplexBinding;
                if (eattrBinding != null)
                {
                    attrBinding.AddChildren(eattrBinding.Children);
                    RemoveChild(eattrBinding);
                }
                AddChild(attrBinding);
            }

            //Prepares the binding details
            KnockoutBinding valueBinding = new TBinding();

            //Gets the binding from the given additional bindings
            //Defaults to above information if not found
            valueBinding = Get(valueBinding.Name) ?? valueBinding;

            //Sets the binding value
            //(Re)Adds the binding to the given additional bindings
            valueBinding.Value = bindingValue;
            AddChild(valueBinding);

            if (additionalHtmlAttributes == null)
            {
                additionalHtmlAttributes = new RouteValueDictionary();
            }

            additionalHtmlAttributes.Add(Name, Value);

            return additionalHtmlAttributes;
        }

        #endregion
    }


    public class KnockoutControl<TContext, TBinding>
        : KnockoutControl<TContext>
        where TBinding  : KnockoutBinding, new()
    {
        #region Fields

        private readonly ControlGeneratorDelegate controlGenerator;
        private readonly IDictionary<string, object> addtionalHtmlAttributes;
        private readonly string expression;
        private readonly HtmlHelper html;

        #endregion

        #region Ctors

        public KnockoutControl(
            ControlGeneratorDelegate controlGenerator, 
            HtmlHelper html, 
            string expression,
            IDictionary<string, object> addtionalHtmlAttributes)
        {
            this.controlGenerator = controlGenerator;

            this.addtionalHtmlAttributes = addtionalHtmlAttributes;

            this.expression = expression;

            this.html = html;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            var bindingValue = KnockoutHelper.ReformatMemberCallingChain(expression);
            var xadditionalHtmlAttributes = AddDataBindAttribute(bindingValue, addtionalHtmlAttributes);

            return controlGenerator(html, expression, null, xadditionalHtmlAttributes).ToHtmlString();
        }

        #endregion

        #region Supports

        private IDictionary<string, object> AddDataBindAttribute(
            string bindingValue,
            IDictionary<string, object> additionalHtmlAttributes)
        {
            if (html.IsForeach())
            {
                var modelName = html.NameForModel();
                var modelId = html.IdForModel();
                var bindingValueWithoutParentheses = bindingValue.Replace("(", string.Empty).Replace(")", string.Empty);
                var bindingValueWithoutParenthesesWithoutDot = bindingValueWithoutParentheses.Replace(".", "_");
                var nameBindingValue = string.Format("'{0}[' + $index + '].{1}'", modelName,
                                                     bindingValueWithoutParentheses);
                var idBindingValue = string.Format("'{0}_' + $index + '__{1}'", modelId,
                                                   bindingValueWithoutParenthesesWithoutDot);
                var attrBinding = new AttrBinding()
                    .AddChild(new NameBinding { Value = nameBindingValue })
                    .AddChild(new LazyBinding("id") { Value = idBindingValue });
                var eattrBinding = Get(attrBinding.Name) as ComplexBinding;
                if (eattrBinding != null)
                {
                    attrBinding.AddChildren(eattrBinding.Children);
                    RemoveChild(eattrBinding);
                }
                AddChild(attrBinding);
            }

            //Prepares the binding details
            KnockoutBinding valueBinding = new TBinding();

            //Gets the binding from the given additional bindings
            //Defaults to above information if not found
            valueBinding = Get(valueBinding.Name) ?? valueBinding;

            //Sets the binding value
            //(Re)Adds the binding to the given additional bindings
            valueBinding.Value = bindingValue;
            AddChild(valueBinding);

            if (additionalHtmlAttributes == null)
            {
                additionalHtmlAttributes = new RouteValueDictionary();
            }

            additionalHtmlAttributes.Add(Name, Value);

            return additionalHtmlAttributes;
        }

        #endregion
    }
}