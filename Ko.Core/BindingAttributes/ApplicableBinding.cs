namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the applicable binding
    /// </summary>
    public class ApplicableBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "applicable" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "applicable"; }
        }

        #endregion
    }
}