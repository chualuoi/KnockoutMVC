namespace Ko.Mvc.BindingAttributes
{
    public class OptionsBooleanBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "optionsBoolean" as binding name
        /// </summary>
        public override string Name
        {
            get { return "optionsBoolean"; }
        }

        #endregion
    }
}