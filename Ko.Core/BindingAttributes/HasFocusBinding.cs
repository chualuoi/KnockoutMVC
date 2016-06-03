namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the hasfocus binding
    /// </summary>
    public class HasFocusBinding : KnockoutBinding, IEventBinding
    {
        /// <summary>
        /// Returns "hasfocus" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "hasfocus"; }
        }
    }
}