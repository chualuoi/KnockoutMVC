namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the DateValue binding
    /// </summary>
    public class DateValueBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "DateValue" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "DateValue"; }
        }

        #endregion
    }
}