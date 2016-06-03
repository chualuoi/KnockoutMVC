namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the currency binding
    /// </summary>
    public class CurrencyBinding : KnockoutBinding, IValueBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "currency" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "currency"; }
        }

        #endregion
    }
}