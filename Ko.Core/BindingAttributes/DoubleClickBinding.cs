namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the click binding
    /// </summary>
    public class DoubleClickBinding : KnockoutBinding, IEventBinding
    {
        /// <summary>
        /// Returns "click" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "dblclick"; }
        }
    }
}