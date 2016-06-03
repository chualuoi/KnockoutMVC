using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using Ko.Core;
using Ko.Core.BindingAttributes;

namespace Ko.Mvc
{
    /// <summary>
    /// Provides extensions for HtmlString and HtmlHelper
    /// </summary>
    public static class KnockoutExtensions
    {
        public static bool IsForeach(this HtmlHelper helper)
        {
            var indicator = helper.ViewData["{___KO_LIB_FOREACH_INDICATOR___}"] as bool?;

            return indicator.HasValue && indicator.Value;
        }

        #region DataBind

        /// <summary>
        /// Inserts data-bind attribute to the given HtmlString instance
        /// </summary>
        /// <param name="htmlString">The given HtmlString instance</param>
        /// <param name="koBindings">KoBindings string</param>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        public static IHtmlString DataBind(this HtmlString htmlString, string koBindings, DataBindOptions option = DataBindOptions.Overwrite)
        {
            if (htmlString == null)
            {
                throw new ArgumentNullException("htmlString");
            }
            if (string.IsNullOrWhiteSpace(koBindings))
            {
                throw new ArgumentNullException("koBindings");
            }
            return (htmlString as IHtmlString).DataBind(koBindings, option);
        }

        /// <summary>
        /// Inserts data-bind attribute to the given HtmlString instance
        /// </summary>
        /// <typeparam name="TContext">
        /// C# Knockout binding context type
        /// </typeparam>
        /// <param name="htmlString">
        /// The given HtmlString instance
        /// </param>
        /// <param name="koBindings">
        /// KoBindings instance
        /// </param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IHtmlString DataBind<TContext>(this HtmlString htmlString, KoBindings<TContext> koBindings, DataBindOptions option = DataBindOptions.Overwrite)
        {
            if (htmlString == null)
            {
                throw new ArgumentNullException("htmlString");
            }
            if (koBindings == null || koBindings.Value.Length == 0)
            {
                throw new ArgumentNullException("koBindings");
            }

            return htmlString.DataBind(koBindings.ToString(), option);
        }

        public static IHtmlString ToolTipText(this HtmlString htmlString, string tipText)
        {
            if (htmlString == null)
            {
                throw new ArgumentNullException("htmlString");
            }

            if (string.IsNullOrWhiteSpace(tipText)) return htmlString;

            var html = htmlString.ToString().Trim();
            var startTagRegex = new Regex(CommonRegex.HtmlStartTagRegex);
            if (startTagRegex.IsMatch(html))
            {
                if (html.IndexOf("<input ") > 0)
                {
                    html = html.Replace("<input ", "<input tip='" + tipText + "'");
                }

                return new HtmlString(html);
            }

            return htmlString;

        }

        /// <summary>
        /// Inserts data-bind attribute to the given HtmlString instance
        /// </summary>
        /// <param name="htmlString">The given HelperResult instance</param>
        /// <param name="koBindings">KoBindings string</param>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        public static IHtmlString DataBind(this HelperResult htmlString, string koBindings, DataBindOptions option = DataBindOptions.Overwrite)
        {
            if (htmlString == null)
            {
                throw new ArgumentNullException("htmlString");
            }
            if (string.IsNullOrWhiteSpace(koBindings))
            {
                throw new ArgumentNullException("koBindings");
            }
            return (htmlString as IHtmlString).DataBind(koBindings, option);
        }

        /// <summary>
        /// Inserts data-bind attribute to the given HtmlString instance
        /// </summary>
        /// <typeparam name="TContext">C# Knockout binding context type</typeparam>
        /// <param name="htmlString">The given HelperResult instance</param>
        /// <param name="koBindings">KoBindings instance</param>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        public static IHtmlString DataBind<TContext>(this HelperResult htmlString, KoBindings<TContext> koBindings, DataBindOptions option = DataBindOptions.Overwrite)
        {
            if (htmlString == null)
            {
                throw new ArgumentNullException("htmlString");
            }
            if (koBindings == null || koBindings.Value.Length == 0)
            {
                throw new ArgumentNullException("koBindings");
            }

            return htmlString.DataBind(koBindings.ToString(), option);
        }

        /// <summary>
        /// Inserts data-bind attribute to the given IHtmlString instance
        /// </summary>
        /// <param name="htmlString">The given IHtmlString instance</param>
        /// <param name="koBindings">KoBindings string</param>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        private static IHtmlString DataBind(this IHtmlString htmlString, string koBindings, DataBindOptions option = DataBindOptions.Overwrite)
        {
            var dataBindRegex = new Regex(CommonRegex.DataBindRegex);
            //Validates the given koBindings string
            if (!string.IsNullOrWhiteSpace(koBindings) && !dataBindRegex.IsMatch(koBindings))
            {
                throw new ArgumentException("Invalid Knockout binding expression!");
            }
            var html = htmlString.ToString().Trim();
            var startTagRegex = new Regex(CommonRegex.HtmlStartTagRegex);
            if (startTagRegex.IsMatch(html))
            {
                var startTag = startTagRegex.Match(html).Value;
                html = html.Substring(startTag.Length);
                if (dataBindRegex.IsMatch(startTag))
                {
                    switch (option)
                    {
                        case DataBindOptions.Overwrite:
                            startTag = dataBindRegex.Replace(startTag, String.Empty);
                            break;
                        case DataBindOptions.Merge:
                            var valueRegex = new Regex(@"""([^""]*)""");
                            var oldDataBind = valueRegex.Match(dataBindRegex.Match(startTag).Value.Trim()).Value;
                            oldDataBind = oldDataBind.Substring(1, oldDataBind.Length - 2);
                            koBindings = koBindings.Insert(koBindings.IndexOf('"') + 1, oldDataBind + ", ");
                            startTag = dataBindRegex.Replace(startTag, " ");
                            break;
                        case DataBindOptions.DoNothing:
                            return htmlString;
                    }
                }
                //Checks if the given element is sefl-closed
                var index = startTag.IndexOf("/>", StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    index = startTag.IndexOf(">", StringComparison.OrdinalIgnoreCase);
                }
                if (index > -1)
                {
                    //Inserts a whitespace before the attribute
                    var databind = String.Format(" {0} ", koBindings);

                    return new HtmlString(startTag.Insert(index, databind) + html);
                }

            }
            return htmlString;
        }

        /// <summary>
        /// Validates the given bindings is an instance of KoBindings&lt;TContext&gt;
        /// </summary>
        /// <param name="koBindings">
        /// Given bindings
        /// </param>
        /// <returns>
        /// Context type of given bindings
        /// </returns>
        /// <exception cref="Exception">
        /// Throws when the given binding is not an instance of KoBindings&lt;TContext&gt;
        /// </exception>
        public static Type ValidateContextType(object koBindings)
        {
            var contextType = typeof(object);
            var koBindingsValid = koBindings != null;
            if (koBindingsValid)
            {
                var koBindingsTypeTemplate = typeof(KoBindings<>);
                var koBindingsType = koBindings.GetType();
                koBindingsValid = koBindingsType.IsGenericType && koBindingsType.Name == koBindingsTypeTemplate.Name;
                contextType = koBindingsValid ? koBindingsType.GetGenericArguments()[0] : contextType;
            }
            if (!koBindingsValid)
            {
                throw new Exception("Binding context is invalid.");
            }
            return contextType;
        }

        /// <summary>
        /// Gets all children of an KoBinding instance
        /// </summary>
        /// <param name="koBindings">
        /// Given bindings
        /// </param>
        /// <exception cref="Exception">
        /// Throws when the given binding is not an instance of KoBindings&lt;TContext&gt;
        /// </exception>
        public static IEnumerable<KnockoutBinding> GetChildren(object koBindings)
        {
            ValidateContextType(koBindings);
            var ko = koBindings.GetType();

            var children = ko.GetProperty("Children");

            return (IEnumerable<KnockoutBinding>)children.GetValue(koBindings, null);
        }

        /// <summary>
        /// Applies bindings to the given html string using reflection
        /// </summary>
        /// <param name="htmlString">
        /// Given html string
        /// </param>
        /// <param name="contextType">
        /// Binding context type
        /// </param>
        /// <param name="koBindings">
        /// Given Bindings
        /// </param>
        /// <returns></returns>
        public static IHtmlString DataBind(IHtmlString htmlString, Type contextType, object koBindings)
        {
            var genericDatabind = typeof(KnockoutExtensions)
                .GetMethods()
                .FirstOrDefault(x => x.Name == "DataBind" && x.IsGenericMethod);

            if (genericDatabind != null)
            {
                var databind = genericDatabind.MakeGenericMethod(contextType);
                var result = databind.Invoke(null, new[] { htmlString, koBindings, DataBindOptions.Overwrite });
                return result as IHtmlString;
            }

            return htmlString;
        }

        #endregion

        #region VirtualElement

        public static VirtualElement BeginVirtualElement<TModel>(this HtmlHelper htmlHelper, KoBindings<TModel> dataBind)
        {
            if (dataBind == null)
            {
                throw new ArgumentNullException("dataBind");
            }
            if (!dataBind.VirtualElementAllowed)
            {
                throw new ArgumentException("The given binding is not allowed in virtual element");
            }
            return VirtualElementHelper(htmlHelper, dataBind);
        }

        public static void EndVirtualElement(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewContext.Writer.Write("<!-- /ko -->");
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification =
                "Because disposing the object would write to the response stream, you don't want to prematurely dispose of this object."
            )]
        private static VirtualElement VirtualElementHelper<TModel>(this HtmlHelper htmlHelper, KoBindings<TModel> dataBind)
        {
            htmlHelper.ViewContext.Writer.Write("<!-- ko {0} -->", dataBind.Value);
            var virtualElement = new VirtualElement(htmlHelper.ViewContext);
            return virtualElement;
        }

        #endregion

        #region DisposableTag

        /// <summary>
        /// Generates the start tag of the given tag.
        /// The end tag will be generated by using EndTag metho or combinging this method with using statement
        /// </summary>
        /// <typeparam name="TModel">
        /// Model type
        /// </typeparam>
        /// <param name="htmlHelper">
        /// HtmlHelper instance
        /// </param>
        /// <param name="tagName">
        /// Tag name
        /// </param>
        /// <param name="koBindings">
        /// Databind attrbibute
        /// </param>
        /// <param name="htmlAttributes">
        /// Additional html attributes
        /// </param>
        /// <returns>
        /// A disposable object for using with using statement
        /// </returns>
        public static DisposableTag BeginTag<TModel>(this HtmlHelper htmlHelper, string tagName, KoBindings<TModel> koBindings = null, IDictionary<string, object> htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder(tagName);

            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(htmlAttributes);
            }
            if (koBindings != null)
            {
                tagBuilder.MergeAttribute(koBindings.Name, koBindings.Value, true);
            }

            var disposableElement = new DisposableTag(htmlHelper.ViewContext.Writer, tagBuilder);
            return disposableElement;
        }

        public static DisposableTag BeginTag(this HtmlHelper htmlHelper, string tagName, string koBindingValue = null, IDictionary<string, object> htmlAttributes = null)
        {
            var koBinding = string.IsNullOrWhiteSpace(koBindingValue)
                                ? null
                                : new KoBindings<object> { Value = koBindingValue };
            return BeginTag(htmlHelper, tagName, koBinding, htmlAttributes);
        }

        /// <summary>
        /// Generates the start tag of the given tag.
        /// The end tag will be generated by using EndTag metho or combinging this method with using statement
        /// </summary>
        /// <typeparam name="TModel">
        /// Model type
        /// </typeparam>
        /// <param name="htmlHelper">
        /// HtmlHelper instance
        /// </param>
        /// <param name="tagName">
        /// Tag name
        /// </param>
        /// <param name="koBindings">
        /// Databind attrbibute
        /// </param>
        /// <param name="htmlAttributes">
        /// Additional html attributes
        /// </param>
        /// <returns>
        /// A disposable object for using with using statement
        /// </returns>
        public static DisposableTag BeginTag<TModel>(this HtmlHelper htmlHelper, string tagName, KoBindings<TModel> koBindings = null, object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder(tagName);

            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            }
            if (koBindings != null)
            {
                var attributeValue = String.Join(",", koBindings.GetChildren());
                tagBuilder.MergeAttribute(koBindings.Name, attributeValue, true);
            }

            var disposableElement = new DisposableTag(htmlHelper.ViewContext.Writer, tagBuilder);
            return disposableElement;
        }

        /// <summary>
        /// Generates the start tag of the given tag.
        /// The end tag will be generated by using EndTag metho or combinging this method with using statement
        /// </summary>
        /// <param name="htmlHelper">
        /// HtmlHelper instance
        /// </param>
        /// <param name="tagName">
        /// Tag name
        /// </param>
        /// <param name="htmlAttributes">
        /// Additional html attributes
        /// </param>
        /// <returns>
        /// A disposable object for using with using statement
        /// </returns>
        public static DisposableTag BeginTag(this HtmlHelper htmlHelper, string tagName, IDictionary<string, object> htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder(tagName);

            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(htmlAttributes);
            }

            var disposableElement = new DisposableTag(htmlHelper.ViewContext.Writer, tagBuilder);
            return disposableElement;
        }

        /// <summary>
        /// Generates the start tag of the given tag.
        /// The end tag will be generated by using EndTag metho or combinging this method with using statement
        /// </summary>
        /// <param name="htmlHelper">
        /// HtmlHelper instance
        /// </param>
        /// <param name="tagName">
        /// Tag name
        /// </param>
        /// <param name="htmlAttributes">
        /// Additional html attributes
        /// </param>
        /// <returns>
        /// A disposable object for using with using statement
        /// </returns>
        public static DisposableTag BeginTag(this HtmlHelper htmlHelper, string tagName, object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder(tagName);

            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            }

            var disposableElement = new DisposableTag(htmlHelper.ViewContext.Writer, tagBuilder);
            return disposableElement;
        }

        /// <summary>
        /// Writes down the end tag of the given tag name
        /// </summary>
        /// <param name="htmlHelper">
        /// HtmlHelper instance
        /// </param>
        /// <param name="tagName">
        /// Tag name
        /// </param>
        public static void EndTag(this HtmlHelper htmlHelper, string tagName)
        {
            htmlHelper.ViewContext.Writer.Write("</{0}>", tagName);
        }

        #endregion

        #region CreateTag

        /// <summary>
        /// Generates normal html element, excluding self-closing element
        /// </summary>
        /// <param name="htmlHelper">
        /// An HtmlHelper instance
        /// </param>
        /// <param name="tagName">
        /// Tag name
        /// </param>
        /// <param name="htmlAttributes">
        /// Additional html attributes
        /// </param>
        /// <param name="selfClosing">
        /// Indicates the tag is self-closing
        /// </param>
        /// <returns></returns>
        public static HtmlString CreateTag(this HtmlHelper htmlHelper, string tagName, IDictionary<string, object> htmlAttributes = null, bool selfClosing = false)
        {
            var tagBuilder = new TagBuilder(tagName);

            if (htmlAttributes != null)
            {
                foreach (var htmlAttribute in htmlAttributes)
                {
                    tagBuilder.MergeAttribute(htmlAttribute.Key, htmlAttribute.Value.ToString());
                }
            }
            var tagRenderMode = selfClosing ? TagRenderMode.SelfClosing : TagRenderMode.Normal;
            return new HtmlString(tagBuilder.ToString(tagRenderMode));
        }

        /// <summary>
        /// Generates normal html element, excluding self-closing element
        /// </summary>
        /// <param name="htmlHelper">
        /// An HtmlHelper instance
        /// </param>
        /// <param name="tagName">
        /// Tag name
        /// </param>
        /// <param name="htmlAttributes">
        /// Additional html attributes
        /// </param>
        /// <param name="selfClosing">
        /// Indicates the tag is self-closing
        /// </param>
        /// <returns></returns>
        public static HtmlString CreateTag(this HtmlHelper htmlHelper, string tagName, object htmlAttributes = null, bool selfClosing = false)
        {
            var tagBuilder = new TagBuilder(tagName);

            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            }
            var tagRenderMode = selfClosing ? TagRenderMode.SelfClosing : TagRenderMode.Normal;
            return new HtmlString(tagBuilder.ToString(tagRenderMode));
        }

        #endregion

        #region KnockoutForEach

        public static MvcHtmlString KoForEach<TModel, TItem>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, IList<TItem>>> expression,
            string templateName, object additionalViewData = null, string afterRender = "ReparseForUnobtrusiveValidation", string afterAdd = null,
            string beforeRemove = null, string beforeMove = null, string afterMove = null)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            //Check whether the templateName is null or empty.
            //If true, then throw exception because we want to reuse mvc built-in editor for
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentNullException(templateName, "templateName must be specified");
            }

            var list = (IList<TItem>) expression.Compile().DynamicInvoke(htmlHelper.ViewData.Model);
            var isEmpty = false;
            if (list.Count == 0)
            {
                isEmpty = true;
                var dummyItem = Activator.CreateInstance<TItem>();
                list.Add(dummyItem);
            }
            var getItemMethod = typeof(IList<TItem>).GetMethod("get_Item");
            var getItemBody = Expression.Call(expression.Body, getItemMethod, new Expression[] { Expression.Constant(0) });
            var firstItem = Expression.Lambda<Func<TModel, TItem>>(getItemBody, expression.Parameters);

            var editorText = htmlHelper.EditorFor(firstItem, templateName, additionalViewData).ToString();
            var valueRegex = new Regex(CommonRegex.ValueRegex);
            editorText = valueRegex.Replace(editorText, x => " ");
            return KoForEachString(htmlHelper, expression, editorText, afterRender, afterAdd, beforeRemove, beforeMove, afterMove, isEmpty, list);
        }

        /// <summary>
        /// Make this function can be Testable
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="editorText"></param>
        /// <param name="afterRender"></param>
        /// <param name="afterAdd"></param>
        /// <param name="beforeRemove"></param>
        /// <param name="beforeMove"></param>
        /// <param name="afterMove"></param>
        /// <param name="isEmpty"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static MvcHtmlString KoForEachString<TModel, TItem>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, IList<TItem>>> expression,
                                                                    string editorText, string afterRender, string afterAdd, string beforeRemove,
                                                                    string beforeMove, string afterMove, 
                                                                    bool isEmpty, IList<TItem> list)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            expressionText = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var inputRegex = new Regex(string.Format(@"<((input)|(select))[^>]*name=""{0}\[[0]][^""]*""[^>]*>", expressionText));
            var validationRegex =
                new Regex(string.Format(@"<[^>]*data-valmsg-for=""[^""]*{0}\[[0]][^""]*""[^>]*>", expressionText));
            var inputTags = inputRegex.Matches(editorText);
            var validationMessages = validationRegex.Matches(editorText);
            var bindingContext = new KoBindings<object>();
            if (isEmpty)
            {
                list.Clear();
            }
            foreach (Match inputTag in inputTags)
            {
                #region Finding the name of the element

                var nameRegex = new Regex(string.Format(@"name=""{0}\[[0]][^>""]*""", expressionText));
                var nameAttribute = nameRegex.Match(inputTag.Value);
                var elementName = nameAttribute.Value.Substring(6, nameAttribute.Value.Length - 7);

                //Knockout binding value for element name
                var index = elementName.IndexOf("[0]");
                var startPart = elementName.Substring(0, index);
                var endPart = elementName.Substring(index + 3, elementName.Length - index - 3);

                var knockoutName = string.Format("'{0}[' + {1}() + ']{2}'", startPart, KnockoutContants.Index, endPart);

                //Adding attr knockout binding for this input element
                var temp = new MvcHtmlString(inputTag.Value);
                    //Create temporary html string holding input tag for ease of adding binding
                var attrBindings = new KoBindings<object>().AddLazy("name", knockoutName);

                #endregion

                #region Finding the id of the element

                //Finding the id of the element
                var idRegex = new Regex(string.Format(@"id=""{0}_0__[^>""]*""", expressionText));
                var idAttribute = idRegex.Match(inputTag.Value);
                if (!string.IsNullOrWhiteSpace(idAttribute.Value))
                {
                    var elementId = idAttribute.Value.Substring(4, idAttribute.Value.Length - 5);

                    //Knockout binding value for element id                    
                    index = elementId.IndexOf("_0__");
                    startPart = elementId.Substring(0, index);
                    endPart = elementId.Substring(index + 4, elementName.Length - index - 4);

                    var knockoutId = string.Format("'{0}_' + {1}() + '__{2}'", startPart, KnockoutContants.Index,
                                                   endPart);

                    //Adding attr knockout binding for this input element
                    attrBindings.AddLazy("id", knockoutId);
                }

                #endregion

                bindingContext.Add<AttrBinding>(attrBindings);
                editorText = editorText.Replace(inputTag.Value, temp.DataBind(bindingContext, DataBindOptions.Merge).ToString());
                bindingContext.Clear();
            }
            foreach (Match validationMessage in validationMessages)
            {
                //Finding the name of the element that this validation message is for
                var idRegex = new Regex(string.Format(@"data-valmsg-for=""[^""]*{0}\[[0]][^""]*""", expressionText));
                var messageForAttr = idRegex.Match(validationMessage.Value);
                var elementName = messageForAttr.Value.Substring(17, messageForAttr.Value.Length - 18);

                //Knockout binding value for element name
                var index = elementName.IndexOf("[0]");
                var startPart = elementName.Substring(0, index);
                var endPart = elementName.Substring(index + 3, elementName.Length - index - 3);
                var knockoutId = string.Format("'{0}[' + {1}() + ']{2}'", startPart, KnockoutContants.Index, endPart);

                //Adding attr knockout binding for this validation message
                var temp = new MvcHtmlString(validationMessage.Value);
                    //Create temporary html string holding input tag for ease of adding binding
                bindingContext.Add<AttrBinding>(new KoBindings<object>().AddLazy("'data-valmsg-for'", knockoutId));
                editorText = editorText.Replace(validationMessage.Value,
                                                temp.DataBind(bindingContext, DataBindOptions.Merge).ToString());
                bindingContext.Clear();
            }
            var knockoutEditorText = new StringBuilder();
            var eventHandlers = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(afterRender))
            {
                eventHandlers.Append(", afterRender: " + afterRender);
            }
            if (!string.IsNullOrWhiteSpace(afterAdd))
            {
                eventHandlers.Append(", afterAdd: " + afterAdd);
            }
            if (!string.IsNullOrWhiteSpace(beforeRemove))
            {
                eventHandlers.Append(", beforeRemove: " + beforeRemove);
            }
            if (!string.IsNullOrWhiteSpace(beforeMove))
            {
                eventHandlers.Append(", beforeMove: " + beforeMove);
            }
            if (!string.IsNullOrWhiteSpace(afterMove))
            {
                eventHandlers.Append(", afterMove: " + afterMove);
            }
            knockoutEditorText.Append(string.Format("<div {0}>", bindingContext.AddLazy("foreach",
                                                                                        string.Format("{{data: {0} {1}}}",
                                                                                                      KnockoutHelper.
                                                                                                          ExtractExpression(
                                                                                                              expression),
                                                                                                      eventHandlers))
                                          ));
            knockoutEditorText.Append(editorText);
            knockoutEditorText.Append("</div>");
            return new MvcHtmlString(knockoutEditorText.ToString());
        }

        public static MvcHtmlString CreateForeachTemplate<TModel, TItem>(
            this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, IList<TItem>>> expression,
            string templateName, 
            object additionalViewData = null
        )
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            //Check whether the templateName is null or empty.
            //If true, then throw exception because we want to reuse mvc built-in editor for
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentNullException(templateName, "templateName must be specified");
            }
            //Expression<Func<TModel, TItem>> realExpression = x => expression.Compile()().GetFirst;
            var list = (IList<TItem>)expression.Compile().DynamicInvoke(htmlHelper.ViewData.Model);
            var isEmpty = false;
            if (list.Count == 0)
            {
                isEmpty = true;
                var dummyItem = Activator.CreateInstance<TItem>();
                list.Add(dummyItem);
            }
            var getItemMethod = typeof(IList<TItem>).GetMethod("get_Item");
            var getItemBody = Expression.Call(expression.Body, getItemMethod, new Expression[] { Expression.Constant(0) });
            var firstItem = Expression.Lambda<Func<TModel, TItem>>(getItemBody, expression.Parameters);

            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var editorText = htmlHelper.EditorFor(firstItem, templateName, additionalViewData).ToString();
            expressionText = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var inputRegex = new Regex(string.Format(@"<((input)|(select))[^>]*name=""{0}\[[0]][^""]*""[^>]*>", expressionText));
            var validationRegex = new Regex(string.Format(@"<[^>]*data-valmsg-for=""[^""]*{0}\[[0]][^""]*""[^>]*>", expressionText));
            var inputTags = inputRegex.Matches(editorText);
            var validationMessages = validationRegex.Matches(editorText);
            var bindingContext = new KoBindings<object>();
            if (isEmpty)
            {
                list.Clear();
            }
            foreach (Match inputTag in inputTags)
            {
                #region Finding the name of the element
                var nameRegex = new Regex(string.Format(@"name=""{0}\[[0]][^>""]*""", expressionText));
                var nameAttribute = nameRegex.Match(inputTag.Value);
                var elementName = nameAttribute.Value.Substring(6, nameAttribute.Value.Length - 7);

                //Knockout binding value for element name
                var index = elementName.IndexOf("[0]");
                var startPart = elementName.Substring(0, index);
                var endPart = elementName.Substring(index + 3, elementName.Length - index - 3);

                var knockoutName = string.Format("'{0}[' + {1}() + ']{2}'", startPart, KnockoutContants.Index, endPart);

                //Adding attr knockout binding for this input element
                var temp = new MvcHtmlString(inputTag.Value); //Create temporary html string holding input tag for ease of adding binding
                var attrBindings = new KoBindings<object>().AddLazy("name", knockoutName);
                #endregion

                #region Finding the id of the element
                //Finding the id of the element
                var idRegex = new Regex(string.Format(@"id=""{0}_0__[^>""]*""", expressionText));
                var idAttribute = idRegex.Match(inputTag.Value);
                if (!string.IsNullOrWhiteSpace(idAttribute.Value))
                {
                    var elementId = idAttribute.Value.Substring(4, idAttribute.Value.Length - 5);

                    //Knockout binding value for element id                    
                    index = elementId.IndexOf("_0__");
                    startPart = elementId.Substring(0, index);
                    endPart = elementId.Substring(index + 4, elementName.Length - index - 4);

                    var knockoutId = string.Format("'{0}_' + {1}() + '__{2}'", startPart, KnockoutContants.Index,
                                                   endPart);

                    //Adding attr knockout binding for this input element
                    attrBindings.AddLazy("id", knockoutId);
                }

                #endregion

                bindingContext.Add<AttrBinding>(attrBindings);
                editorText = editorText.Replace(inputTag.Value, temp.DataBind(bindingContext, DataBindOptions.Merge).ToString());
                bindingContext.Clear();
            }
            foreach (Match validationMessage in validationMessages)
            {
                //Finding the name of the element that this validation message is for
                var idRegex = new Regex(string.Format(@"data-valmsg-for=""[^""]*{0}\[[0]][^""]*""", expressionText));
                var messageForAttr = idRegex.Match(validationMessage.Value);
                var elementName = messageForAttr.Value.Substring(17, messageForAttr.Value.Length - 18);

                //Knockout binding value for element name
                var index = elementName.IndexOf("[0]");
                var startPart = elementName.Substring(0, index);
                var endPart = elementName.Substring(index + 3, elementName.Length - index - 3);
                var knockoutId = string.Format("'{0}[' + {1}() + ']{2}'", startPart, KnockoutContants.Index, endPart);

                //Adding attr knockout binding for this validation message
                var temp = new MvcHtmlString(validationMessage.Value); //Create temporary html string holding input tag for ease of adding binding
                bindingContext.Add<AttrBinding>(new KoBindings<object>().AddLazy("'data-valmsg-for'", knockoutId));
                editorText = editorText.Replace(validationMessage.Value, temp.DataBind(bindingContext, DataBindOptions.Merge).ToString());
                bindingContext.Clear();
            }
            return new MvcHtmlString(editorText);
        }


        #endregion
    }
}
