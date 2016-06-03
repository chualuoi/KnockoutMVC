namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the with binding
    /// </summary>
    public class WithBinding : KnockoutBinding
    {
        #region Overrides

        /// <summary>
        /// Indicates that the complex binding is allowed in virtual element if and only if all of its children are allowed to be
        /// </summary>
        public override bool VirtualElementAllowed
        {
            get { return true; }
        }

        /// <summary>
        /// Returns "with" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "with"; }
        }

        #endregion
    }
}