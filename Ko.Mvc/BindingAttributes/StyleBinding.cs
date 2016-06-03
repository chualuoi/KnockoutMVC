namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the style binding
    /// </summary>
    public class StyleBinding : ComplexBinding
    {
        /// <summary>
        /// Returns "style" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "style"; }
        }
    }
}