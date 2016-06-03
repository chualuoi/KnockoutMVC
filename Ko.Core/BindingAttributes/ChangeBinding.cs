namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the change binding
    /// </summary>
    public class ChangeBinding : KnockoutBinding, IEventBinding
    {
        /// <summary>
        /// Returns "change" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "change"; }
        }
    }
}