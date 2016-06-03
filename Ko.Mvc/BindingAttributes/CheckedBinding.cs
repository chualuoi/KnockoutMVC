namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the checked binding
    /// </summary>
    public class CheckedBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "checked" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "checked"; }
        }
    }
}