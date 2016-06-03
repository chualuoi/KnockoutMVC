namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the FormattedValue binding
    /// </summary>
    public class FormattedValueBinding : ComplexBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "FormattedValue" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "FormattedValue"; }
        }

        #endregion
    }
}