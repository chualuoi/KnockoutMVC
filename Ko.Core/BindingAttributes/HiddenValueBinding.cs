namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the hiddenValue binding
    /// </summary>
    public class HiddenValueBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "hiddenValue" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "hiddenValue"; }
        }

        #endregion
    }
}