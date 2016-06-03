namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the ifnot binding
    /// </summary>
    public class IfNotBinding : KnockoutBinding
    {
        #region Overrides

        public override bool VirtualElementAllowed
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Returns "ifnot" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "ifnot"; }
        }
    }
}