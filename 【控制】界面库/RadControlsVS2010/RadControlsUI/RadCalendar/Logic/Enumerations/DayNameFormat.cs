using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Specifies the display formats for the days of the week used as selectors by
    /// <strong>RadCalendar</strong>.You can specify whether the days of the week are displayed as
    /// the full name, short (abbreviated) name, first letter of the day, or first two letters of the day.
    /// </summary>
    public enum DayNameFormat
    {
        /// <summary>
        /// The days of the week displayed in full format. For example, <strong>Tuesday</strong>.
        /// </summary>
        Full,
        /// <summary>
        /// The days of the week displayed in abbreviated format. For example, <strong>Tues</strong>.
        /// </summary>
        Short,
        /// <summary>
        /// The days of the week displayed with just the first letter. For example, <strong>T</strong>.
        /// </summary>
        FirstLetter,
        /// <summary>
        /// The days of the week displayed with just the first two letters. For example, <strong>Tu</strong>.
        /// </summary>
        FirstTwoLetters,
        /// <summary>
        /// The shortest unique abbreviated day names associated with the current DateTimeFormatInfo object.
        /// </summary>
        Shortest
    }
}
