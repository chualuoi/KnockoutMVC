namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the dateOfBirth binding
    /// </summary>
    public class DoBBinding : KnockoutBinding, IValueBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "dateOfBirth" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "dateOfBirth"; }
        }

        #endregion
    }
}