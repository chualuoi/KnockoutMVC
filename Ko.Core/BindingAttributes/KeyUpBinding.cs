namespace Ko.Core.BindingAttributes
{
    public class KeyUpBinding : KnockoutBinding, IEventBinding
    {
        /// <summary>
        /// Returns "keyup" as the binding name
        /// </summary>
        public override string Name
        {
            get { return "keyup"; }
        }
    }
}