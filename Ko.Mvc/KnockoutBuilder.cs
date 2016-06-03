using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ko.Core;
using Ko.Core.BindingAttributes;

namespace Ko.Mvc
{
    public partial class KnockoutBuilder<TRoot, TParent, TModel>
    {
        #region Ctors

        public KnockoutBuilder(HtmlHelper<TModel> htmlHelper)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            Html = htmlHelper;
        }

        #endregion

        #region Properties

        public virtual KoBindings<IKoContext<TRoot, TParent, TModel>> Bind { get { return new KoBindings<IKoContext<TRoot, TParent, TModel>>(); } }

        public HtmlHelper<TModel> Html { get; private set; }

        #endregion

        #region Special

        #region Supports

        private static KoBindings<IKoContext<TRoot, TParent, TProperty>> AddBinding<TBinding, TProperty>(
            string bindingValue,
            KoBindings<IKoContext<TRoot, TParent, TProperty>> additionalBindings)
            where TBinding : KnockoutBinding, new()
        {
            var binding = new TBinding
                {
                    Value = bindingValue
                };

            if (additionalBindings != null)
            {
                var xbinding = additionalBindings.Get(binding.Name);
                if (xbinding != null)
                {
                    xbinding.Value = binding.Value;
                }
                else
                {
                    additionalBindings.AddChild(binding);
                }
            }
            else
            {
                additionalBindings = new KoBindings<IKoContext<TRoot, TParent, TProperty>>();
                additionalBindings.AddChild(binding);
            }

            return additionalBindings;
        }

        private static KoBindings<IKoContext<TRoot, TParent, TProperty>> AddBinding<TBinding, TProperty>(
            TBinding binding,
            KoBindings<IKoContext<TRoot, TParent, TProperty>> additionalBindings)
            where TBinding : ComplexBinding
        {
            if (additionalBindings != null)
            {
                var xbinding = additionalBindings.Get(binding.Name) as ComplexBinding;
                if (xbinding != null)
                {
                    binding.AddChildren(xbinding.Children);
                    xbinding.Children.Clear();
                    xbinding.AddChildren(binding.Children);
                }
                else
                {
                    additionalBindings.AddChild(binding);
                }
            }
            else
            {
                additionalBindings = new KoBindings<IKoContext<TRoot, TParent, TProperty>>();
                additionalBindings.AddChild(binding);
            }

            return additionalBindings;
        }

        #endregion

        public string With<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            string editorTemplate,
            KoBindings<IKoContext<TRoot, TParent, TProperty>> additionalBindings = null)
        {
            var bindingValue = KnockoutHelper.ExtractExpression(expression);

            additionalBindings = AddBinding<WithBinding, TProperty>(bindingValue, additionalBindings);

            using (Html.BeginVirtualElement(additionalBindings))
            {
                Html.ViewContext.Writer.WriteLine(Html.EditorFor(expression, editorTemplate));
            }

            return string.Empty;
        }

        public string TemplateFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            string templateName,
            KoBindings<IKoContext<TRoot, TParent, TProperty>> additionalBindings = null)
        {
            var binding =
                new TemplateBinding()
                    .AddChild(new NameBinding { Value = templateName })
                    .AddChild(new DataBinding { Value = KnockoutHelper.ExtractExpression(expression) });

            additionalBindings = AddBinding(binding, additionalBindings);

            using (Html.BeginVirtualElement(additionalBindings))
            {
                //TODO
            }

            return string.Empty;
        }

        public string ForeachTemplateFor<TProperty>(
            Expression<Func<TModel, IEnumerable<TProperty>>> expression,
            string templateName,
            KoBindings<IKoContext<TRoot, TParent, TProperty>> additionalBindings = null)
        {
            var binding =
                new TemplateBinding()
                    .AddChild(new NameBinding { Value = templateName })
                    .AddChild(new ForeachBinding { Value = KnockoutHelper.ExtractExpression(expression) });

            additionalBindings = AddBinding(binding, additionalBindings);

            using (Html.BeginVirtualElement(additionalBindings))
            {
                //TODO
            }

            return string.Empty;
        }

        public string Foreach<TProperty>(
            Expression<Func<TModel, IEnumerable<TProperty>>> expression,
            string editorTemplate,
            KoBindings<IKoContext<TRoot, TParent, TProperty>> additionalBindings = null)
        {
            if (additionalBindings == null)
            {
                additionalBindings = new KoBindings<IKoContext<TRoot, TParent, TProperty>>();
            }
            var foreachBinding = new ForeachBinding { Value = KnockoutHelper.ExtractExpression(expression) };

            var temp = additionalBindings.Get(foreachBinding.Name) as ComplexBinding;
            if (temp != null && temp.Children.Count > 0)
            {
                foreachBinding.AddChildren(temp.Children);
                additionalBindings.RemoveChild(temp);
            }
            additionalBindings.AddChild(foreachBinding);
            using (Html.BeginVirtualElement(additionalBindings))
            {
                //TODO: Html.ViewContext.Writer.WriteLine(Html.KoForEach(expression, editorTemplate));
            }

            return string.Empty;
        }

        #endregion

        #region Special - Disposable

        public DisposableKnockoutBuilder<TRoot, TModel, TProperty> With<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            KoBindings<IKoContext<TRoot, TParent, TModel>> additionalBindings = null)
        {
            var bindings = CreateBinding<TProperty, WithBinding>(expression, additionalBindings);

            var newBuilder = CreateBuilder(expression, bindings);

            return newBuilder;
        }

        public DisposableKnockoutBuilder<TRoot, TModel, TProperty> Foreach<TProperty>(
            Expression<Func<TModel, IEnumerable<TProperty>>> expression,
            KoBindings<IKoContext<TRoot, TParent, TModel>> additionalBindings = null)
        {
            var bindings = CreateForeachBinding(expression, additionalBindings);

            var newBuilder = CreateBuilder(expression, bindings);

            return newBuilder;
        }

        #endregion

        #region Supports

        private DisposableKnockoutBuilder<TRoot, TModel, TProperty> CreateBuilder<TProperty>(
            Expression<Func<TModel, IEnumerable<TProperty>>> expression,
            KoBindings<TProperty> bindings)
        {
            var htmlHelper = CreatHtmlHelper(expression);

            var newBuilder = new DisposableKnockoutBuilder<TRoot, TModel, TProperty>(htmlHelper, bindings);

            return newBuilder;
        }

        private DisposableKnockoutBuilder<TRoot, TModel, TProperty> CreateBuilder<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            KoBindings<TProperty> bindings)
        {
            var htmlHelper = CreatHtmlHelper(expression);

            var newBuilder = new DisposableKnockoutBuilder<TRoot, TModel, TProperty>(htmlHelper, bindings);

            return newBuilder;
        }

        private KoBindings<TProperty> CreateForeachBinding<TProperty>(
            Expression<Func<TModel, IEnumerable<TProperty>>> expression,
            KoBindings<IKoContext<TRoot, TParent, TModel>> additionalBindings)
        {
            var binding = new ForeachBinding
                {
                    Value = KnockoutHelper.ExtractExpression(expression)
                };

            var bindings =
                new KoBindings<TProperty>()
                    .AddChild(binding);

            if (additionalBindings != null && additionalBindings.Children.Count > 0)
            {
                var temp = additionalBindings.Get(binding.Name) as ComplexBinding;
                if (temp != null && temp.Children.Count > 0)
                {
                    binding.AddChildren(temp.Children);
                    additionalBindings.RemoveChild(temp);
                }

                bindings.AddChildren(additionalBindings.Children);
            }

            return bindings;
        }

        private KoBindings<TProperty> CreateBinding<TProperty, TBinding>(
            Expression<Func<TModel, TProperty>> expression,
            KoBindings<IKoContext<TRoot, TParent, TModel>> additionalBindings) where TBinding : KnockoutBinding, new()
        {
            var binding = new TBinding
                {
                    Value = KnockoutHelper.ExtractExpression(expression)
                };

            var bindings =
                new KoBindings<TProperty>()
                    .AddChild(binding);

            if (additionalBindings != null && additionalBindings.Children.Count > 0)
            {
                var xbinding = binding as ComplexBinding;
                if (xbinding != null)
                {
                    var temp = additionalBindings.Get(binding.Name) as ComplexBinding;
                    if (temp != null && temp.Children.Count > 0)
                    {
                        xbinding.AddChildren(temp.Children);
                        additionalBindings.RemoveChild(temp);
                    }
                }
                bindings.AddChildren(additionalBindings.Children);
            }

            return bindings;
        }

        private HtmlHelper<TProperty> CreatHtmlHelper<TProperty>(
            Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, Html.ViewData);

            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            if (metadata.ConvertEmptyStringToNull && String.Empty.Equals(metadata.Model))
            {
                metadata.Model = null;
            }

            var formattedModelValue = metadata.Model;

            var formatString = metadata.EditFormatString;
            if (metadata.Model != null && !String.IsNullOrEmpty(formatString))
            {
                formattedModelValue = String.Format(CultureInfo.CurrentCulture, formatString, metadata.Model);
            }

            var viewData = new ViewDataDictionary(Html.ViewDataContainer.ViewData)
                {
                    Model = metadata.Model,
                    ModelMetadata = metadata,
                    TemplateInfo = new TemplateInfo
                        {
                            FormattedModelValue = formattedModelValue,
                            HtmlFieldPrefix = Html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)
                        }
                };

            return new HtmlHelper<TProperty>(Html.ViewContext, new ViewDataContainer(viewData));
        }

        private HtmlHelper<TProperty> CreatHtmlHelper<TProperty>(
            Expression<Func<TModel, IEnumerable<TProperty>>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, Html.ViewData);

            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            if (metadata.ConvertEmptyStringToNull && String.Empty.Equals(metadata.Model))
            {
                metadata.Model = null;
            }

            var formattedModelValue = metadata.Model;

            var formatString = metadata.EditFormatString;
            if (metadata.Model != null && !String.IsNullOrEmpty(formatString))
            {
                formattedModelValue = String.Format(CultureInfo.CurrentCulture, formatString, metadata.Model);
            }

            var viewData = new ViewDataDictionary(Html.ViewDataContainer.ViewData)
                {
                    Model = metadata.Model,
                    ModelMetadata = metadata,
                    TemplateInfo = new TemplateInfo
                        {
                            FormattedModelValue = formattedModelValue,
                            HtmlFieldPrefix = Html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)
                        }
                };

            viewData["{___KO_LIB_FOREACH_INDICATOR___}"] = true;

            return new HtmlHelper<TProperty>(Html.ViewContext, new ViewDataContainer(viewData));
        }

        #endregion

    }
}