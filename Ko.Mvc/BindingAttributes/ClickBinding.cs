namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the click binding
    /// </summary>
    public class ClickBinding : KnockoutBinding, IEventBinding
    {
        /// <summary>
        /// Returns "click" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "click"; }
        }
    }
}