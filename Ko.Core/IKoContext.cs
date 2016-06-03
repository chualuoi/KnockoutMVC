namespace Ko.Core
{
    /// <summary>
    /// Models the Knockout binding context.
    /// No implementation is required for this interface.
    /// Its purpose is to mimic the Knockout binding context with C# strongly type 
    /// which we can make use of IDE's auto-complete intellisense
    /// </summary>
    /// <typeparam name="TRoot">
    /// C# root type
    /// </typeparam>
    /// <typeparam name="TParent">
    /// C# parent type
    /// </typeparam>
    /// <typeparam name="TData">
    /// C# data type
    /// </typeparam>
    public interface IKoContext<TRoot, TParent, TData>
    {
        /// <summary>
        /// Knockout root context
        /// </summary>
        TRoot KoRoot { get; set; }

        /// <summary>
        /// Knockout parent context
        /// </summary>
        TParent KoParent { get; set; }

        /// <summary>
        /// Knockout data context
        /// </summary>
        TData KoData { get; set; }

        /// <summary>
        /// Knockout index variable
        /// </summary>
        int KoIndex { get; set; }
    }
}