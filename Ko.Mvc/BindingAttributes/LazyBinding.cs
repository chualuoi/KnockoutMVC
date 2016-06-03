using System;
using System.Text.RegularExpressions;

namespace Ko.Mvc.BindingAttributes
{
    /// <summary>
    /// Uses for Knockout bindings that is not necessary to model classes for them.
    /// such as child bindings of css binding or attr binding.
    /// It is called lazy because it could be use for any other bindings without defining corresponding classes.
    /// </summary>
    public class LazyBinding : ComplexBinding
    {
        #region Overrides of KnockoutBinding

        private readonly string name;

        public override string Name
        {
            get { return name; }
        }

        private readonly bool virtualElementAllowed;
        public override bool VirtualElementAllowed
        {
            get { return virtualElementAllowed; }
        }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes internal members
        /// </summary>
        /// <param name="name"></param>
        /// <param name="virtualElementAllowed">
        /// Indicates the binding is allowed to use in the virtual element
        /// </param>
        public LazyBinding(string name, bool virtualElementAllowed=false)
        {
            var regex = new Regex(CommonRegex.KnockValidBindingName);
            var isValid = true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                //Allows spaces at beginning and end of string
                name = name.Trim();
            }
            else
            {
                isValid = false;
            }
            if (!isValid || regex.Matches(name).Count > 1)
            {
                throw new ArgumentException("Invalid binding name!");
            }
            this.name = name;
            this.virtualElementAllowed = virtualElementAllowed;
        }

        #endregion

    }
}