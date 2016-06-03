namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the CardDateBinding binding
    /// </summary>
    public class CardDateBinding : KnockoutBinding, IValueBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "postcode" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "cardDate"; }
        }

        #endregion
    }
}