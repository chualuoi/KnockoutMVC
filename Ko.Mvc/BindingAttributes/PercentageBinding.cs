namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the percentage binding
    /// </summary>
    public class PercentageBinding : KnockoutBinding, IValueBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "percentage" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "percentage"; }
        }

        #endregion
    }
}