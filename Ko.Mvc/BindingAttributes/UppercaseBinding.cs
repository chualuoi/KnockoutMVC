namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the change binding
    /// </summary>
    public class UppercaseBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "change" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "Uppercase"; }
        }
    }
}