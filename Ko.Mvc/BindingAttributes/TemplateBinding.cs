namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the template binding
    /// </summary>
    public class TemplateBinding : ComplexBinding
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
        /// Returns "template" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "template"; }
        }

        #endregion
    }
}