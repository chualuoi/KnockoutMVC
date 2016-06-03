namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the booleanValue binding
    /// </summary>
    public class BooleanBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "booleanValue" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "booleanValue"; }
        }
    }
}