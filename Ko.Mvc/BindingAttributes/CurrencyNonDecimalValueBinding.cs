namespace Ko.Mvc.BindingAttributes
{
    public class CurrencyNonDecimalValueBinding : ComplexBinding
    {
        /// <summary>
        /// Returns "CurrencyValue" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "CurrencyNonDecimalValue"; }
        }
    }
}