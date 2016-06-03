namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the pattern binding
    /// </summary>
    public class PatternBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "pattern" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "pattern"; }
        }

        #endregion
    }
}