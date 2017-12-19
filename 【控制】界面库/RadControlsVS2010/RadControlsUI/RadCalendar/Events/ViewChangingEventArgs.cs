using System;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	/// <summary>
    /// Arguments class used when the ViewChangingEvent event is fired.
	/// </summary>
    public sealed class ViewChangingEventArgs : CancelEventArgs
    {
        // Fields
        private CalendarView view;

        // Constructors
        public ViewChangingEventArgs(CalendarView newView)
        {
            this.view = newView;
        }

		/// <summary>
        /// Gets the  new CalendarView instance that will substitute the view currently displayed by RadCalendar.
		/// </summary>
        public CalendarView View
        {
            get
            {
                return this.view;
            }
        }
    }
}

