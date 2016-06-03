namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the currencyNoDecimal binding
    /// </summary>
    public class CurrencyNoDecimalBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "currencyNoDecimal" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "currencyNoDecimal"; }
        }
    }
}