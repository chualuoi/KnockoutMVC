namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the visible binding
    /// </summary>
    public class VisibleBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "visible" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "visible"; }
        }
    }
}