namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the date binding
    /// </summary>
    public class DateBinding : KnockoutBinding, IValueBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "DateText" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "date"; }
        }

        #endregion
    }
}