namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the foreach binding
    /// </summary>
    public class ForeachBinding : ComplexBinding
    {
        #region Overrides

        /// <summary>
        /// Indicates that the complex binding is allowed in virtual element if and only if all of its children are allowed to be
        /// </summary>
        public override bool VirtualElementAllowed
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns "foreach" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "foreach"; }
        }

        #endregion

    }
}