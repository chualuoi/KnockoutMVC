using System;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class WarningParameterizedAttribute : WarningAttribute
    {
        #region Properties & Fields

        /// <summary>
        /// Parameters for custom error message on client
        /// </summary>
        public virtual string[] Paramerters { get; protected set; }

        #endregion

        #region Ctors

        public WarningParameterizedAttribute(string message, string [] parameters) : base(message)
        {
            if (parameters == null || parameters.Length == 0)
            {
                throw new ArgumentException("Invalid message parameters");
            }

            Paramerters = parameters;
        }

        #endregion

    }
}