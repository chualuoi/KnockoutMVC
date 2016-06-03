namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the text binding
    /// </summary>
    public class TextBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "text" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "text"; }
        }
    }
}