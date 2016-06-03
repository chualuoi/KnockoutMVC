namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the submit binding
    /// </summary>
    public class SubmitBinding : KnockoutBinding, IEventBinding
    {
        /// <summary>
        /// Returns "submit" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "submit"; }
        }
    }
}