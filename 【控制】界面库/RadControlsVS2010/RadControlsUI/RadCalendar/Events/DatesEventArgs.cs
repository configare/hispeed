using System;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Arguments class used when the SelectionChanging event is fired.
	/// </summary>
    public sealed class SelectionEventArgs : CancelEventArgs
    {
        // Fields
        private DateTimeCollection dates;

        // Constructors
        public SelectionEventArgs(DateTimeCollection dates)
        {
            this.dates = dates;
        }

		/// <summary>
		/// Gets a refference to the SelectedDates collection, represented by the Telerik RadCalendar component
        /// that rise the SelectionChanging event.
		/// </summary>
        public DateTimeCollection Dates
        {
            get
            {
                return this.dates;
            }
        }
    }
}

