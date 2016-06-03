namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the FormattedText binding
    /// </summary>
    public class FormattedTextBinding : ComplexBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "FormattedText" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "FormattedText"; }
        }

        #endregion
    }
}