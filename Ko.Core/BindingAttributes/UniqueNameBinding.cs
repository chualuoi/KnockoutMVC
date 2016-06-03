namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the uniqueName binding
    /// </summary>
    public class UniqueNameBinding : KnockoutBinding
    {
        /// <summary>
        /// Returns "uniqueName" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "uniqueName"; }
        }
    }
}