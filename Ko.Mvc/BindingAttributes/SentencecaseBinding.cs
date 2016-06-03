namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the Sentencecase binding
    /// </summary>
    public class SentencecaseBinding : KnockoutBinding, IValueBinding
    {
        /// <summary>
        /// Returns "Sentencecase" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "Sentencecase"; }
        }
    }
}