namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the Capitalise binding
    /// </summary>
    public class CapitaliseBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "Capitalise" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "Capitalise"; }
        }
    }
}