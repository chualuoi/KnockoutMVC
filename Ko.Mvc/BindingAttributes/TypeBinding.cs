namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the type binding
    /// </summary>
    public class TypeBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "type" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "type"; }
        }

        #endregion
    }
}