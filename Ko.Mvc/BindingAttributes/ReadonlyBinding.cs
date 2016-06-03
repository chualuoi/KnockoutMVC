namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Models the readOnly binding
    /// </summary>
    public class ReadonlyBinding : KnockoutBinding
    {
        #region Overrides of KnockoutBinding

        public override string Name
        {
            get { return "readOnly"; }
        }

        #endregion
    }
}