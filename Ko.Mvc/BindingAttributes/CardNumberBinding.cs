namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the Card Number binding
    /// </summary>
    public class CardNumberBinding : KnockoutBinding, IValueBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "cardNumber" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "cardNumber"; }
        }

        #endregion
    }
}