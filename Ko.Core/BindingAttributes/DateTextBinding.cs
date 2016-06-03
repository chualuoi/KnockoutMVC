namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the DateText binding
    /// </summary>
    public class DateTextBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "DateText" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "DateText"; }
        }

        #endregion
    }
}