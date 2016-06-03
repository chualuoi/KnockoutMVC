namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the selectedOptions binding
    /// </summary>
    public class SelectedOptionsBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "selectedOptions" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "selectedOptions"; }
        }
    }
}