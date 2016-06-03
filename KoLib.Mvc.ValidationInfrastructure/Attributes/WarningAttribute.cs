using System;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class WarningAttribute : Attribute
    {
        #region Properties & Fields

        public virtual string Message { get; protected set; }

        #endregion

        #region Ctors

        public WarningAttribute(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Invalid message");
            }

            Message = message;
        }

        #endregion

    }
}
