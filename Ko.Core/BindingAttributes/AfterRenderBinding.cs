namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the afterRender binding
    /// </summary>
    public class AfterRenderBinding : KnockoutBinding, IEventBinding
    {
        /// <summary>
        /// Returns "afterRender" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "afterRender"; }
        }
    }
}