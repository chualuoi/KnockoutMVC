namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the if binding
    /// </summary>
    public class IfBinding : KnockoutBinding
    {
        #region Overrides

        public override bool VirtualElementAllowed
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Returns "if" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "if"; }
        }
    }
}