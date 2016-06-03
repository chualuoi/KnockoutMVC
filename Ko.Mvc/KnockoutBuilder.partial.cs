using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ko.Core;
using Ko.Core.BindingAttributes;

namespace Ko.Mvc
{
    /// <summary>
    /// Hodls common HTML controls for our systems
    /// </summary>
    /// <typeparam name="TRoot"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public partial class KnockoutBuilder<TRoot, TParent, TModel>
    {
        #region Supports
        
        public KnockoutControl<IKoContext<TRoot, TParent, TModel>> CreateControl<TProperty, TBinding>(
            ControlGeneratorDelegate<TModel, TProperty> controlGenerator, 
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> addtionalHtmlAttributes
            ) where TBinding : KnockoutBinding, new()
        {
            return new KnockoutControl<IKoContext<TRoot, TParent, TModel>, TModel, TProperty, TBinding>(controlGenerator, Html, expression, addtionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>> CreateControl<TBinding>(
            ControlGeneratorDelegate controlGenerator, 
            string expression,
            IDictionary<string, object> addtionalHtmlAttributes
            ) where TBinding : KnockoutBinding, new()
        {
            return new KnockoutControl<IKoContext<TRoot, TParent, TModel>, TBinding>(controlGenerator, Html, expression, addtionalHtmlAttributes);
        }

        #endregion

        #region TextBox

        #region Generic Binding

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>> TextBoxFor<TBinding, TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate<TModel, TProperty>(InputExtensions.TextBoxFor);

            var control = CreateControl<TProperty, TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>> TextBox<TBinding>(
            string expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate(InputExtensions.TextBox);

            var control = CreateControl<TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        #endregion

        #region Normal

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<ValueBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<ValueBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<ValueBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextBox(
            string name
            )
        {
            return TextBox<ValueBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextBox(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<ValueBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextBox(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<ValueBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Percentage

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  PercentageFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<PercentageBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  PercentageFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<PercentageBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  PercentageFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<PercentageBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Percentage(
            string name
            )
        {
            return TextBox<PercentageBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Percentage(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<PercentageBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Percentage(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<PercentageBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Currency

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<CurrencyBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<CurrencyBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<CurrencyBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Currency(
            string name
            )
        {
            return TextBox<CurrencyBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Currency(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<CurrencyBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Currency(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<CurrencyBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region CurrencyNoDecimal

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyNoDecimalFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<CurrencyNoDecimalBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyNoDecimalFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<CurrencyNoDecimalBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyNoDecimalFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<CurrencyNoDecimalBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyNoDecimal(
            string name
            )
        {
            return TextBox<CurrencyNoDecimalBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyNoDecimal(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<CurrencyNoDecimalBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CurrencyNoDecimal(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<CurrencyNoDecimalBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Date

        #region Supports

        private IDictionary<string, object> AddClassAttribute(IDictionary<string, object> additionalHtmlAttributes)
        {
            additionalHtmlAttributes = additionalHtmlAttributes ?? new Dictionary<string, object>();
            var classAttribute = new KeyValuePair<string, object>("class", " datePicker");
            if (additionalHtmlAttributes.ContainsKey(classAttribute.Key))
            {
                var xclassAttribute = additionalHtmlAttributes["class"] as KeyValuePair<string, object>?;
                if (xclassAttribute.HasValue)
                {
                    additionalHtmlAttributes.Remove("class");
                    classAttribute = new KeyValuePair<string, object>(classAttribute.Key,
                                                                      (xclassAttribute.Value.Value as string) +
                                                                      classAttribute.Value);
                }
            }

            additionalHtmlAttributes.Add(classAttribute);

            return additionalHtmlAttributes;
        }

        private KnockoutControl<IKoContext<TRoot, TParent, TModel>> DateFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            additionalHtmlAttributes = AddClassAttribute(additionalHtmlAttributes);

            return TextBoxFor<DateBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        private KnockoutControl<IKoContext<TRoot, TParent, TModel>> Date(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            additionalHtmlAttributes = AddClassAttribute(additionalHtmlAttributes);

            return TextBox<DateBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DatePickerFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return DateFor(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DatePickerFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return DateFor(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DatePickerFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return DateFor(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DatePicker(
            string name
            )
        {
            return Date(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DatePicker(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return Date(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DatePicker(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return Date(name, additionalHtmlAttributes);
        }

        #endregion

        #region Integer

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  IntegerFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<IntegerBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  IntegerFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<IntegerBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  IntegerFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<IntegerBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Integer(
            string name
            )
        {
            return TextBox<IntegerBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Integer(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<IntegerBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Integer(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<IntegerBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        //#region Decimal

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DecimalFor<TProperty>(
        //    Expression<Func<TModel, TProperty>> expression,
        //    )
        //{
        //    return TextBoxFor<DecimalBinding, TProperty>(expression, null);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DecimalFor<TProperty>(
        //    Expression<Func<TModel, TProperty>> expression,
        //    object additionalHtmlAttributes,
        //    )
        //{
        //    var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
        //    return TextBoxFor<DecimalBinding, TProperty>(expression, xadditionalHtmlAttributes);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DecimalFor<TProperty>(
        //    Expression<Func<TModel, TProperty>> expression,
        //    IDictionary<string, object> additionalHtmlAttributes,
        //    )
        //{
        //    return TextBoxFor<DecimalBinding, TProperty>(expression, additionalHtmlAttributes);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Decimal(
        //    string name,
        //    )
        //{
        //    return TextBox<DecimalBinding>(name, null);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Decimal(
        //    string name,
        //    object additionalHtmlAttributes,
        //    )
        //{
        //    var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
        //    return TextBox<DecimalBinding>(name, xadditionalHtmlAttributes);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Decimal(
        //    string name,
        //    IDictionary<string, object> additionalHtmlAttributes,
        //    )
        //{
        //    return TextBox<DecimalBinding>(name, additionalHtmlAttributes);
        //}

        //#endregion

        #region Uppercase

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  UppercaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<UppercaseBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  UppercaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<UppercaseBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  UppercaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<UppercaseBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Uppercase(
            string name
            )
        {
            return TextBox<UppercaseBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Uppercase(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<UppercaseBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Uppercase(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<UppercaseBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Lowercase

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  LowercaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<LowercaseBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  LowercaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<LowercaseBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  LowercaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<LowercaseBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Lowercase(
            string name
            )
        {
            return TextBox<LowercaseBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Lowercase(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<LowercaseBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Lowercase(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<LowercaseBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Titlecase

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TitlecaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<TitlecaseBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TitlecaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<TitlecaseBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TitlecaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<TitlecaseBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Titlecase(
            string name
            )
        {
            return TextBox<TitlecaseBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Titlecase(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<TitlecaseBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Titlecase(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<TitlecaseBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Sentencecase

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  SentencecaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<SentencecaseBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  SentencecaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<SentencecaseBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  SentencecaseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<SentencecaseBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Sentencecase(
            string name
            )
        {
            return TextBox<SentencecaseBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Sentencecase(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<SentencecaseBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Sentencecase(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<SentencecaseBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Capitalise

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CapitaliseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<CapitaliseBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CapitaliseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<CapitaliseBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  CapitaliseFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<CapitaliseBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Capitalise(
            string name
            )
        {
            return TextBox<CapitaliseBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Capitalise(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<CapitaliseBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Capitalise(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<CapitaliseBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Html

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<HtmlBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<HtmlBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextBoxFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<HtmlBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextBox(
            string name
            )
        {
            return TextBox<HtmlBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextBox(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<HtmlBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextBox(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<HtmlBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #endregion
        
        #region TextArea

        #region Generic Binding

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextAreaFor<TBinding, TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate<TModel, TProperty>(TextAreaExtensions.TextAreaFor);

            var control = CreateControl<TProperty, TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextArea<TBinding>(
            string expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate(
                (html, s, o, objects) => html.TextArea(s, string.Empty, objects));

            var control = CreateControl<TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        #endregion

        #region Normal

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextAreaFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextAreaFor<ValueBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextAreaFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextAreaFor<ValueBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextAreaFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextAreaFor<ValueBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextArea(
            string name
            )
        {
            return TextArea<ValueBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextArea(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextArea<ValueBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  TextArea(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextArea<ValueBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #region Html

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextAreaFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return TextBoxFor<HtmlBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextAreaFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBoxFor<HtmlBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextAreaFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBoxFor<HtmlBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextArea(
            string name
            )
        {
            return TextBox<HtmlBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextArea(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return TextBox<HtmlBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RichTextArea(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return TextBox<HtmlBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #endregion
        
        #region Password

        #region Generic Binding

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  PasswordFor<TBinding, TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate<TModel, TProperty>(InputExtensions.PasswordFor);

            var control = CreateControl<TProperty, TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Password<TBinding>(
            string expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate(InputExtensions.Password);

            var control = CreateControl<TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        #endregion

        #region Normal

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  PasswordFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return PasswordFor<ValueBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  PasswordFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return PasswordFor<ValueBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  PasswordFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return PasswordFor<ValueBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Password(
            string name
            )
        {
            return Password<ValueBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Password(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return Password<ValueBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Password(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return Password<ValueBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #endregion
        
        #region RadioButton

        #region Generic Binding

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButtonFor<TBinding, TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate<TModel, TProperty>(InputExtensions.RadioButtonFor);

            var control = CreateControl<TProperty, TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButton<TBinding>(
            string expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate(InputExtensions.RadioButton);

            var control = CreateControl<TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        #endregion

        #region Normal

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButtonFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return RadioButtonFor<ValueBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButtonFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return RadioButtonFor<ValueBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButtonFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return RadioButtonFor<ValueBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButton(
            string name
            )
        {
            return RadioButton<ValueBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButton(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return RadioButton<ValueBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  RadioButton(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return RadioButton<ValueBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #endregion
        
        #region Display

        #region Generic Binding

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DisplayFor<TBinding, TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate<TModel, TProperty>(DisplayExtensions.DisplayFor);

            var control = CreateControl<TProperty, TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Display<TBinding>(
            string expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
            where TBinding : KnockoutBinding, IValueBinding, new()
        {
            var controlGenerator = new ControlGeneratorDelegate(
                (html, s, o, objects) => html.Display(s));

            var control = CreateControl<TBinding>(controlGenerator, expression, additionalHtmlAttributes);

            return control;
        }

        #endregion

        #region Normal

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DisplayFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression
            )
        {
            return DisplayFor<ValueBinding, TProperty>(expression, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DisplayFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return DisplayFor<ValueBinding, TProperty>(expression, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DisplayFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return DisplayFor<ValueBinding, TProperty>(expression, additionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Display(
            string name
            )
        {
            return Display<ValueBinding>(name, null);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Display(
            string name,
            object additionalHtmlAttributes
            )
        {
            var xadditionalHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalHtmlAttributes);
            return Display<ValueBinding>(name, xadditionalHtmlAttributes);
        }

        public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  Display(
            string name,
            IDictionary<string, object> additionalHtmlAttributes
            )
        {
            return Display<ValueBinding>(name, additionalHtmlAttributes);
        }

        #endregion

        #endregion

        //#region DropDownList

        //#region Normal

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownListFor<TProperty>(
        //    Expression<Func<TModel, TProperty>> expression,
        //    bool useDefaultOptionsText = true,
        //    bool useDefaultOptionsCaption = true,
        //    bool useDefaultOptionsValue = true)
        //{
        //    return DropDownListFor(expression, null, useDefaultOptionsCaption, useDefaultOptionsText, useDefaultOptionsValue);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownListFor<TProperty>(
        //    Expression<Func<TModel, TProperty>> expression,
        //    object htmlAttributes,
        //    bool useDefaultOptionsText = true,
        //    bool useDefaultOptionsCaption = true,
        //    bool useDefaultOptionsValue = true)
        //{
        //    return DropDownListFor(expression, null, useDefaultOptionsCaption, useDefaultOptionsText, useDefaultOptionsValue);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownListFor<TProperty>(
        //    Expression<Func<TModel, TProperty>> expression,
        //    IDictionary<string, object> htmlAttributes,
        //    bool useDefaultOptionsText = true,
        //    bool useDefaultOptionsCaption = true,
        //    bool useDefaultOptionsValue = true)
        //{
        //    htmlString = Html.DropDownListFor(expression, new SelectList(new List<string>()), htmlAttributes);

        //    var bindingValue = KnockoutHelper.ExtractExpression(expression);

        //    Bindings
        //        .Clear()
        //        .AddChild(new ValueBinding { Value = bindingValue });

        //    SetDropDownListDefaults(useDefaultOptionsCaption, useDefaultOptionsText, useDefaultOptionsValue);

        //    return this;
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownList(
        //    string name,
        //    bool useDefaultOptionsText = true,
        //    bool useDefaultOptionsCaption = true,
        //    bool useDefaultOptionsValue = true)
        //{
        //    return DropDownList(name, null, useDefaultOptionsCaption, useDefaultOptionsText, useDefaultOptionsValue);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownList(
        //    string name,
        //    object htmlAttributes,
        //    bool useDefaultOptionsText = true,
        //    bool useDefaultOptionsCaption = true,
        //    bool useDefaultOptionsValue = true)
        //{

        //    return DropDownList(name, null, useDefaultOptionsCaption, useDefaultOptionsText, useDefaultOptionsValue);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownList(
        //    string name,
        //    IDictionary<string, object> htmlAttributes,
        //    bool useDefaultOptionsText = true,
        //    bool useDefaultOptionsCaption = true,
        //    bool useDefaultOptionsValue = true)
        //{
        //    htmlString = Html.DropDownList(name, new SelectList(new List<string>()), htmlAttributes);

        //    var bindingValue = KnockoutHelper.ReformatMemberCallingChain(name, "x");

        //    var binding = new ValueBinding { Value = bindingValue };

        //    Bindings.Clear().AddChild(binding);

        //    SetDropDownListDefaults(useDefaultOptionsCaption, useDefaultOptionsText, useDefaultOptionsValue);

        //    return this;
        //}

        //#endregion

        //#region Boolean

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownListFor(Expression<Func<TModel, bool>> expression)
        //{
        //    return DropDownListFor(expression, null);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownListFor(Expression<Func<TModel, bool>> expression,
        //                                                  object htmlAttributes)
        //{
        //    return DropDownListFor(expression, null);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  DropDownListFor(Expression<Func<TModel, bool>> expression,
        //                                                  IDictionary<string, object> htmlAttributes)
        //{
        //    htmlString = Html.DropDownListFor(expression, new SelectList(new List<string>()), htmlAttributes);

        //    var bindingValue = KnockoutHelper.ExtractExpression(expression);

        //    Bindings
        //        .Clear()
        //        .AddChild(new BooleanBinding { Value = bindingValue })
        //        .AddChild(new OptionsBinding { Value = BooleanOptionsKey })
        //        .AddChild(new OptionsTextBinding { Value = KnockoutContants.OptionsText })
        //        .AddChild(new OptionsValueBinding { Value = KnockoutContants.OptionsValue })
        //        .AddChild(new OptionsCaptionBinding { Value = KnockoutContants.OptionsCaption });

        //    return this;
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  BooleanDropDownList(string name)
        //{
        //    return BooleanDropDownList(name, null);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  BooleanDropDownList(string name, object htmlAttributes)
        //{
        //    return BooleanDropDownList(name, null);
        //}

        //public KnockoutControl<IKoContext<TRoot, TParent, TModel>>  BooleanDropDownList(string name, IDictionary<string, object> htmlAttributes)
        //{
        //    htmlString = Html.DropDownList(name, new SelectList(new List<string>()), htmlAttributes);

        //    var bindingValue = KnockoutHelper.ReformatMemberCallingChain(name, "x");

        //    var binding = new ValueBinding { Value = bindingValue };

        //    Bindings
        //        .Clear()
        //        .AddChild(binding)
        //        .AddChild(new OptionsBinding { Value = BooleanOptionsKey })
        //        .AddChild(new OptionsTextBinding { Value = KnockoutContants.OptionsText })
        //        .AddChild(new OptionsValueBinding { Value = KnockoutContants.OptionsValue })
        //        .AddChild(new OptionsCaptionBinding { Value = KnockoutContants.OptionsCaption });

        //    return this;
        //}

        //#endregion

        //#endregion

        //private const string BooleanOptionsKey = "ko.options.BooleanOptions";
    }
}