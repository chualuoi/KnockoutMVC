namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the disable binding
    /// </summary>
    public class DisableBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "disable" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "disable"; }
        }
    }
}