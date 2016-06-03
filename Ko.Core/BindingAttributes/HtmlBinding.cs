namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the html binding
    /// </summary>
    public class HtmlBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "html" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "html"; }
        }
    }
}