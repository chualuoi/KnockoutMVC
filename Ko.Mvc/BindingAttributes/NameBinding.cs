namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the enable binding
    /// </summary>
    public class NameBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "enable" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "name"; }
        }
    }
}
