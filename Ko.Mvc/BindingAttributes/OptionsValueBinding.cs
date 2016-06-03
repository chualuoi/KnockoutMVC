namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the optionsValue binding
    /// </summary>
    public class OptionsValueBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "optionsValue" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "optionsValue"; }
        }
    }
}