namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the options binding
    /// </summary>
    public class OptionsBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "options" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "options"; }
        }
    }
}