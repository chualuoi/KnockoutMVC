namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the data binding
    /// </summary>
    public class DataBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "data" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "data"; }
        }
    }
}