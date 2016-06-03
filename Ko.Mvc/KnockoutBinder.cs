using System.Linq;

namespace Ko.Mvc
{
    /// <summary>
    /// Wraps a KoBindings&lt;TContext&gt; instance to avoid error on using that instance directly.
    /// But should use the KoBindings&lt;TContext&gt; instance directly for better performance
    /// </summary>
    /// <typeparam name="TContext">
    /// The Knockout binding context
    /// </typeparam>
    public class KnockoutBinder<TContext>
    {
        /// <summary>
        /// Create KoBindings&lt;TContext&gt; instance
        /// </summary>
        public KoBindings<TContext> Bind { get { return new KoBindings<TContext>(); } }
    }
}