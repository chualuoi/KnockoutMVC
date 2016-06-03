using System.Collections.Generic;

namespace KoLib.T4Helpers
{
    /// <summary>
    /// Contains config information about specific assembly
    /// </summary>
    public class AssemblyConfig
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyConfig"/> class.
        /// </summary>
        public AssemblyConfig()
        {
            NamespaceCollection = new List<NamespaceConfig>();
        }

        /// <summary>
        /// Gets or sets the namespace collection of this assembly.
        /// </summary>
        /// <value>
        /// The namespace collection.
        /// </value>
        public List<NamespaceConfig> NamespaceCollection { get; set; }

        /// <summary>
        /// Gets or sets the name of this assembly.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}