namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the Titlecase binding
    /// </summary>
    public class TitlecaseBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "change" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "Titlecase"; }
        }
    }
}