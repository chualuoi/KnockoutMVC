using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Collections.Concurrent;
using Ko.Core;
using Ko.Core.BindingAttributes;
using Ko.Utils.Extensions;
using KoLib.Mvc.ValidationInfrastructure.Attributes;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public static class ValidationHelpers
    {
        /// <summary>
        /// Out put all validation messages for a specific property include all rules pointed to it
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns><see cref="MvcHtmlString"/> represents all validation messages</returns>
        public static MvcHtmlString RuleValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            var exText = ExpressionHelper.GetExpressionText(expression);
            var modelName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(exText);
            var rootModel = html.ViewData["rootModel"];
            var containerType = rootModel.GetType();
            var htmlViewContext = html.ViewData["htmlViewContext"];
            var htmlViewDataContainer = html.ViewData["htmlViewDataContainer"];
            var result = new StringBuilder();

            //Emit default validation rule of current property
            result.Append(EmbedInDiv(html.ValidationMessageFor(expression)));

            var matchedRules = new List<string>();

            //Check whether this is property we are emitting validation message for is in a collection
            var regex = new Regex(@"\[\d\]");
            if (regex.IsMatch(modelName))
            {
                //Finding the closest type
                var lastIndex = modelName.LastIndexOf(']');
                var prefix = modelName.Substring(0, lastIndex + 1);
                var propNames = prefix.Split('.');
                var nonCollectionPart = modelName.Substring(lastIndex + 2, modelName.Length - lastIndex - 2);
                for (var i = 0; i < propNames.Length; i++)
                {
                    if (regex.IsMatch(propNames[i]))
                    {
                        var collectionName = propNames[i].Substring(0, propNames[i].IndexOf('['));
                        var collectionType = containerType.GetProperty(collectionName).PropertyType;
                        if (collectionType.IsArray)
                        {
                            containerType = collectionType.GetElementType();
                        }
                        else
                        {
                            containerType = collectionType.GetGenericArguments()[0];
                        }
                    }
                    else
                    {
                        containerType = containerType.GetProperty(propNames[i]).PropertyType;
                    }
                }
                matchedRules.AddRange(containerType.GetMatchedRules(modelName, prefix, nonCollectionPart));
            }
            else
            {
                string nonCollectionPart = null;
                var lastDotIndex = modelName.LastIndexOf('.');
                if (lastDotIndex >= 0)
                {
                    nonCollectionPart = modelName.Substring(0, lastDotIndex);
                }
                matchedRules.AddRange(containerType.GetMatchedRules(modelName, null, nonCollectionPart));
            }

            //Emit rule validation
            foreach (var rule in matchedRules)
            {
                var typeTemplate = typeof(HtmlHelper<>);
                var typeParams = new[] { rootModel.GetType() };
                var htmlType = typeTemplate.MakeGenericType(typeParams);
                var tmpHtml = Activator.CreateInstance(htmlType, htmlViewContext, htmlViewDataContainer);
                result.Append(EmbedInDiv(((HtmlHelper)tmpHtml).ValidationMessage(rule)));
            }
            return new MvcHtmlString(result.ToString());
        }

        private static readonly ConcurrentDictionary<Tuple<Type, string>, MvcHtmlString> _hiddenRulesCache = new ConcurrentDictionary<Tuple<Type, string>, MvcHtmlString>();
        
        public static MvcHtmlString OutputHiddenRules<TModel>(this HtmlHelper<TModel> html)
        {
            return _hiddenRulesCache.GetOrAdd( new Tuple<Type, string>(typeof(TModel), html.ViewData.TemplateInfo.HtmlFieldPrefix), m => DoOutputHiddenRules(html));
        }

        private static MvcHtmlString DoOutputHiddenRules<TModel>(HtmlHelper<TModel> html)
        {
            //Find all properties with RuleFor attribute
            var properties = (typeof(TModel)).GetProperties().Where(
                x => x.HasAttribute(typeof(RuleForAttribute))
                    || x.HasAttribute(typeof(RuleForMultipleAttribute)));
            var outputString = new StringBuilder();
            foreach (var propertyInfo in properties)
            {
                var innerHtml = html.CreatHtmlHelper(propertyInfo.Name);

                var bindingContext = new KoBindings<TModel>().AddChild(new HiddenValueBinding{Value = propertyInfo.Name});
                var tagBuilder = innerHtml.CreateInput(InputType.Hidden, propertyInfo.Name);

                var validationAttributes = innerHtml.GetUnobtrusiveValidationAttributes("");

                tagBuilder.MergeAttributes(validationAttributes);
                tagBuilder.MergeAttribute(bindingContext.Name, bindingContext.Value);

                var result = tagBuilder.ToHtmlString();

                outputString.Append(result);
            }
            return new MvcHtmlString(outputString.ToString());
        }

        public static List<string> GetMatchedRules(this Type type, string modelName, string prefix = null, string nonCollectionPart = null)
        {
            var matchedRules = new List<string>();

            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var ruleForAttributes = propertyInfo.GetCustomAttributes(typeof(RuleForAttribute), false);
                if (ruleForAttributes.Cast<RuleForAttribute>().Any(attr => string.Equals(attr.Property.AppendPrefix(prefix), modelName)))
                {
                    matchedRules.Add(propertyInfo.Name.AppendPrefix(prefix));
                }
                var ruleForMultipleAttributes = propertyInfo.GetCustomAttributes(typeof(RuleForMultipleAttribute), false);

                if (ruleForMultipleAttributes.Cast<RuleForMultipleAttribute>().Any(attr => attr.Properties.Any(x => string.Equals(x.AppendPrefix(prefix), modelName))))
                {
                    matchedRules.Add(propertyInfo.Name.AppendPrefix(prefix));
                }
            }
            if (!string.IsNullOrWhiteSpace(nonCollectionPart))
            {
                var innerParts = nonCollectionPart.Split('.');
                var innerPart = innerParts[0];
                var propertyInfo = type.GetProperty(innerPart);
                if (propertyInfo == null)
                {
                    throw new InvalidOperationException("nonCollectionPart is not correct");
                }
                var innerType = propertyInfo.PropertyType;
                var innerPrefix = innerPart.AppendPrefix(prefix);
                var innerNonCollectionPart = string.Join(".", innerParts, 1, innerParts.Length - 1);
                matchedRules.AddRange(innerType.GetMatchedRules(modelName, innerPrefix, innerNonCollectionPart));
            }
            return matchedRules;
        }

        public static string AppendPrefix(this string modelName, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return modelName;
            }
            return prefix + "." + modelName;
        }

        private static string EmbedInDiv(IHtmlString htmlString)
        {
            return string.Format("<div>{0}</div>", htmlString.ToHtmlString());
        }

        public static TagBuilder CreateInput(this HtmlHelper html,InputType inputType, string suffix="")
        {
            var fullName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(suffix);

            var tagBuilder = new TagBuilder("input");

            tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
            tagBuilder.MergeAttribute("name", fullName, true);

            tagBuilder.GenerateId(fullName);

            ModelState modelState;
            if (html.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            return tagBuilder;
        }
        
        /// <summary>
        /// Returns html string for a TagBuilder instance
        /// </summary>
        /// <param name="tagBuilder">
        /// A TagBuilder instance
        /// </param>
        /// <param name="mode">
        /// Specifies what to generate
        /// </param>
        /// <returns></returns>
        private static IHtmlString ToHtmlString(this TagBuilder tagBuilder, TagRenderMode mode = TagRenderMode.Normal)
        {
            return new MvcHtmlString(tagBuilder.ToString(mode));
        }

        private static HtmlHelper CreatHtmlHelper<TModel>(
            this HtmlHelper<TModel> html,
            string propertyName)
        {
            var metadata = ModelMetadata.FromStringExpression(propertyName, html.ViewData);
            
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

            var viewData = new ViewDataDictionary(html.ViewDataContainer.ViewData)
            {
                Model = metadata.Model,
                ModelMetadata = metadata,
                TemplateInfo = new TemplateInfo
                {
                    FormattedModelValue = formattedModelValue,
                    HtmlFieldPrefix = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyName)
                }
            };

            return new HtmlHelper(html.ViewContext, new ViewDataContainer(viewData));
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }

            public ViewDataDictionary ViewData { get; set; }
        }
    }
}