namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the event binding
    /// </summary>
    public class EventBinding : ComplexBinding
    {
        #region Ctors

        #endregion

        /// <summary>
        /// Returns "event" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "event"; }
        }
    }
}