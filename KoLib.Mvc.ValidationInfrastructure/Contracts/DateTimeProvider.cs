// -----------------------------------------------------------------------
// <copyright file="DateTimeProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace KoLib.Mvc.ValidationInfrastructure.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DateTimeProvider: IDateTimeProvider
    {

        public DateTime Today
        {
            get { return DateTime.Today; }
        }
    }
}
