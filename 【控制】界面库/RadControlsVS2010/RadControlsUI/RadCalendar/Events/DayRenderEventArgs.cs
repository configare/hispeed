using System;

namespace Telerik.WinControls.UI
{
	/// <summary>
    /// Arguments class used with the ElementRender event.
	/// </summary>
    public sealed class RenderElementEventArgs : EventArgs
    {
        // Fields
        private LightVisualElement element;
        private RadCalendarDay day;
        private CalendarView view;

        // Constructors
        public RenderElementEventArgs(LightVisualElement cell, RadCalendarDay day, CalendarView currentView)
        {
            this.day = day;
            this.element = cell;
            this.view = currentView;
        }

        #region Properties
        /// <summary>
        /// Gets a refference to the LightVisualElement object that represents visually the specified day to render.
        /// </summary>
        public LightVisualElement Element
        {
            get
            {
                return this.element;
            }
        }

        /// <summary>
        /// Gets a refference to the RadCalendarDay logical object that represents the specified day to render.
        /// </summary>
        public RadCalendarDay Day
        {
            get
            {
                return this.day;
            }
        }

        /// <summary>
        /// Gets a refference to the CalendarView object currently displayed by RadCalendar, 
        /// that contains the specified day to render.
        /// </summary>
        public CalendarView View
        {
            get
            {
                return this.view;
            }
        } 
        #endregion
    }
}

