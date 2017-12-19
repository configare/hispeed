using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Indicates the first day of the week to use when calling date-related functions. 
    /// </summary>
    public enum FirstDayOfWeek
    {
        /// <summary>
        /// Sunday
        /// </summary>
        Sunday,
        /// <summary>
        /// Monday
        /// </summary>
        Monday,
        /// <summary>
        /// Tuesday
        /// </summary>
        Tuesday,
        /// <summary>
        /// Wednesday
        /// </summary>
        Wednesday,
        /// <summary>
        /// Thursday
        /// </summary>
        Thursday,
        /// <summary>
        /// Friday
        /// </summary>
        Friday,
        /// <summary>
        /// Saturday
        /// </summary>
        Saturday,
        /// <summary>
        /// Handled by the current System.Globalization.Calendar object.
        /// </summary>
        Default
    }
}
