namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the Lowercase binding
    /// </summary>
    public class LowercaseBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "Lowercase" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "Lowercase"; }
        }
    }
}