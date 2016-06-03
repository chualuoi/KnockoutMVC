namespace Ko.Core.BindingAttributes
{
    /// <summary>
    /// Base class for all Knockout data-binding's bindings
    /// </summary>
    public abstract class KnockoutBinding
    {
        /// <summary>
        /// Indicates the attribute is allowed to use in virtual element
        /// </summary>
        public virtual bool VirtualElementAllowed { get { return false; } }

        /// <summary>
        /// The binding name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The binding name
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Generates the attribute in format {attribute-name}:{attribute-value}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(Name))
            {
                return string.Empty;
            }
            return string.Format("{0}:{1}", Name, Value);
        }
    }
}
