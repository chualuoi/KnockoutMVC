namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Models the attr binding
    /// </summary>
    public class AttrBinding : ComplexBinding
    {
        /// <summary>
        /// Returns "attr" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "attr"; }
        }
    }
}