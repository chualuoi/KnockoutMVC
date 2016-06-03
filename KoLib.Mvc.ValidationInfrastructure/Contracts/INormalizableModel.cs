using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoLib.Mvc.ValidationInfrastructure.Contracts
{
    /// <summary>
    /// Interface contains function needed to normalize a model (eg. Compute computed properties with does not have logic inside getter)
    /// </summary>
    public interface INormalizableModel
    {
        /// <summary>
        /// Normalize a model (eg. Compute computed properties with does not have logic inside getter)
        /// </summary>
        void Normalize();
    }
}
