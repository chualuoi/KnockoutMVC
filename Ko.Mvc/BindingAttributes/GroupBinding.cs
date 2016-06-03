namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the group binding
    /// </summary>
    public class GroupBinding : ComplexBinding
    {
        #region Overrides of KnockoutBinding

        public override bool VirtualElementAllowed
        {
            get { return true; }
        }

        #endregion

        #region Overrides of KnockoutBinding

        /// <summary>
        /// Returns "group" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "group"; }
        }

        #endregion
    }
}