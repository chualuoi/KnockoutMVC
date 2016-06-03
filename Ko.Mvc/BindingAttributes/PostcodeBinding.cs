namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the postcode binding
    /// </summary>
    public class PostcodeBinding : KnockoutBinding, IValueBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "postcode" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "postcode"; }
        }

        #endregion
    }
}