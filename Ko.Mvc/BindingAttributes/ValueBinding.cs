namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the value binding
    /// </summary>
    public class ValueBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "value" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "value"; }
        }
    }
}