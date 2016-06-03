using System.Collections.Generic;

namespace KoLib.T4Helpers
{
   /// <summary>
   /// Contains configuration information of one specific namespace
   /// </summary>
   public class NamespaceConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceConfig"/> class.
        /// </summary>
       public NamespaceConfig()
       {
           TypeCollection = new List<string>();
       }

       /// <summary>
        /// Gets or sets the type collection.
        /// </summary>
        /// <value>
        /// The type collection.
        /// </value>
       public List<string> TypeCollection { get; set; }

       /// <summary>
       /// Gets or sets the name.
       /// </summary>
       /// <value>
       /// The name.
       /// </value>
       public string Name { get; set; }
    }
}