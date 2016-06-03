namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the bankSortCode binding
    /// </summary>
    public class BankSortCodeBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "bankSortCode" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "bankSortCode"; }
        }
    }
}