using System;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Summary description for RecurringEvents.
    /// DayInMonth - Only the day part of the date is taken into account. That gives the ability to serve events repeated every month on the same day.
    /// DayAndMonth - The month and the day part of the date is taken into account. That gives the ability to serve events repeated in a specific month on the same day.
    /// Today - gives the ability to control the visual appearace of today's date.
    /// None - Default value, means that the day in question is a single point event, no recurrences.
    /// </summary>
    public enum RecurringEvents 
    {
		/// <summary>
		/// Only the day part of the date is taken into account. That gives the ability to serve events repeated every month on the same day.
		/// </summary>
		/// <value>1</value>
        DayInMonth = 1,
		/// <summary>
		/// The month and the day part of the date are taken into account. That gives the ability to serve events repeated in a specific month on the same day.
		/// </summary>
		/// <value>2</value>
        DayAndMonth = 2,
        /// <summary>
        /// The week day is taken into account. That gives the ability to serve events repeated in a specific day of the week.
        /// </summary>
        /// <value>4</value>
        Week = 4,
        /// <summary>
        /// The week day and the month are taken into account. That gives the ability to serve events repeated in a specific week day in a specific month.
        /// </summary>
        /// <value>8</value>
        WeekAndMonth = 8,
		/// <summary>
		///  Gives the ability to control the visual appearace of today's date.
		/// </summary>
		/// <value>16</value>
        Today = 16,
		/// <summary>
		/// Default value, means that the day in question is a single point event, no recurrence.
		/// </summary>
		/// <value>32</value>
		None = 32
    }
}