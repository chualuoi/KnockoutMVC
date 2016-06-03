namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the integer binding
    /// </summary>
    public class IntegerBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "integer" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "integer"; }
        }
    }
}