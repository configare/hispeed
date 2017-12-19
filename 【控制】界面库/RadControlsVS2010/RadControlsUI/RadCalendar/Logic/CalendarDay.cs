using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// RadCalendarDay represents a object that maps date value to corresponding visual settings.
	/// Also the object implements Boolean properties that represent the nature of the selected date - 
	/// whether it is a weekend, disabled or selected in the context of the calendar. Mostly the values
	/// of those properties are set at runtime when a RadCalendarDay instance is constructed and passed
	/// to the DayRender event.
	/// </summary>
	public  class RadCalendarDay : INotifyPropertyChanged
	{
        // Fields
        private string toolTip = string.Empty;
        private RecurringEvents recurring = RecurringEvents.None;
        private bool isWeekend = false;
        private bool isToday = false;
        private bool disabled = false;
        private bool selected = false;
        private bool selectable = false;
        private DateTime date = DateTime.MinValue;
        private bool dirtyLayout;
        private bool dirtyPaint;
        private CalendarDayCollection owner;
		private RadHostItem templateItem = null;
		private Image image = null;
		private bool isTemplateSet;

        #region Constructors

        public RadCalendarDay()
        {
        }

        public RadCalendarDay(DateTime date)
            : this(date, null)
        {
            this.date = date;
        }

        public RadCalendarDay(CalendarDayCollection owner)
            : this(DateTime.MinValue, owner)
        {
        }

        public RadCalendarDay(DateTime date, CalendarDayCollection owner)
        {
            this.owner = owner;
            this.date = date;
        }

        //public RadCalendarDay(RadCalendar calendar)
        //{
        //    this.calendar = calendar;
        //} 

        #endregion

		internal virtual bool IsTemplateSet
		{
			get
			{
				return this.isTemplateSet;
			}
			set
			{
				this.isTemplateSet = value;
				this.OnNotifyPropertyChanged("IsTemplateSet");
			}
		}

		/// <summary>
		/// Gets or sets the image associated with a particular RadCalendarDay object.
		/// </summary>
		[NotifyParentProperty(true)]
		[DefaultValue(null)]
		[Browsable(true)]
		public virtual Image Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.OnNotifyPropertyChanged("Image");
			}
		}

		/// <summary>
        /// Gets or sets the template associated with a particular RadCalendarDay object. 
        /// The template must inherit from RadHostItem.
		/// </summary>
		[NotifyParentProperty(true)]
		[DefaultValue(null)]
		[Browsable(true)]
		public virtual RadHostItem TemplateItem
		{
			get
			{
				return this.templateItem;
			}

			set
			{
				this.templateItem = value;
				this.OnNotifyPropertyChanged("TemplateItem");
			}
		}

        /// <summary>
        /// Gets or sets the date represented by this RadCalendarDay.
        /// </summary>
        [NotifyParentProperty(true)]
        [DefaultValue(null)]
        public virtual DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                this.date = value;
				this.OnNotifyPropertyChanged("Date");
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the RadCalendarDay is qualified as available for selection. 
        /// </summary>
        [NotifyParentProperty(true)]
        [DefaultValue(true)]
        public bool Selectable
        {
            get
            {
                return this.selectable;
            }
            set
            {
                if (this.selectable != value)
                {
                    this.selectable = value;
                    this.OnNotifyPropertyChanged("Selectable");
                    this.DirtyPaint = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadCalendarDay is selected
        /// </summary>
        [NotifyParentProperty(true)]
        [DefaultValue(false)]
        public bool Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    this.OnNotifyPropertyChanged("Selected");
                    this.DirtyPaint = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadCalendarDay is disabled
        /// </summary>
        [NotifyParentProperty(true)]
        [DefaultValue(false)]
        public bool Disabled
        {
            get
            {
                return this.disabled;
            }
            set
            {
                if (this.disabled != value)
                {
                    this.disabled = value;
                    this.OnNotifyPropertyChanged("Disabled");
                    this.DirtyPaint = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadCalendarDay represents the current date.
        /// </summary>
        [NotifyParentProperty(true)]
        [DefaultValue(false)]
        public bool IsToday
        {
            get
            {
                return this.isToday;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadCalendarDay settings are repeated/recurring through out the valid
        /// date range displayed by the calendar.
        /// </summary>
        /// <remarks>
        /// The RecurringEvents enumeration determines which part of the date is handled (day or day and month).
        /// </remarks>
        [NotifyParentProperty(true)]
        [DefaultValue(RecurringEvents.None)]
        public RecurringEvents Recurring
        {
            get
            {
                return this.recurring;
            }
            set
            {
                if (this.recurring != value)
                {
                    this.recurring = value;
                    this.OnNotifyPropertyChanged("Recurring");
                    this.DirtyPaint = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadCalendarDay is mapped to a date that represents a non working
        /// day/weekend.
        /// </summary>
        [NotifyParentProperty(true)]
        [DefaultValue(false)]
        public bool IsWeekend
        {
            get
            {
                return this.isWeekend;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed when the mouse pointer hovers over the calendar day.
        /// </summary>
        [NotifyParentProperty(true)]
        [DefaultValue("")]
        [Localizable(true)]
        public string ToolTip
        {
            get
            {
                return this.toolTip;
            }
            set
            {
                if (this.toolTip != value)
                {
                    this.toolTip = value;
                    this.OnNotifyPropertyChanged("ToolTip");
                }
            }
        }

        /// <summary>
        /// The owner of RadCalendarDay object. 
        /// </summary>
        [DefaultValue(null)]
        internal protected CalendarDayCollection Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                if (this.owner != value)
                {
                    this.owner = value;
                    this.OnNotifyPropertyChanged("Owner");
                }
            }
        }

        /// <summary>
        /// Sets whether RadCalendarDay object is associated with a DateTime equal to today's date.
        /// </summary>
        /// <param name="value">True if RadCalendarDay object is associated with today's date.</param>
        internal protected void SetToday(bool value)
        {
            if (this.isToday != value)
            {
                this.isToday = value;
                this.DirtyPaint = true;
            }
        }

        /// <summary>
        /// Sets whether RadCalendarDay object is associated with a DateTime that represents a weekend day.
        /// </summary>
        /// <param name="value">True if RadCalendarDay object is associated with a DateTime that represents a weekend day.</param>
        internal protected void SetWeekend(bool value)
        {
            if (this.isWeekend != value)
            {
                this.isWeekend = value;
                this.DirtyPaint = true;
            }
        }

        /// <summary>
        /// Checks whether RadCalendarDay object is associated with a DateTime that represents a recurring event.
        /// </summary>
        /// <param name="compareTime">the DateTime to compare.</param>
        /// <param name="processCalendar">the System.Globalization.Calendar object used to check whether the DateTime 
        /// represents a recurring event.</param>
        /// <returns></returns>
        internal protected virtual RecurringEvents IsRecurring(DateTime compareTime, System.Globalization.Calendar processCalendar)
        {
            if (Recurring != RecurringEvents.None)
            {
                switch (Recurring)
                {
                    case RecurringEvents.DayInMonth:
                        {
                            int firstCompare = processCalendar.GetDayOfMonth(compareTime);
                            int secondCompare = processCalendar.GetDayOfMonth(this.Date);
                            if (firstCompare.Equals(secondCompare))
                            {
                                return Recurring;
                            }
                        }
                        break;
                    case RecurringEvents.Today:
                        if (compareTime.Equals(DateTime.Today))
                        {
                            return Recurring;
                        }
                        break;
                    case RecurringEvents.DayAndMonth:
                        {
                            int FirstCompare = processCalendar.GetDayOfMonth(compareTime);
                            int SecondCompare = processCalendar.GetDayOfMonth(this.Date);
                            int FirstMonthCompare = processCalendar.GetMonth(compareTime);
                            int SecondMonthCompare = processCalendar.GetMonth(this.Date);
                            if (FirstCompare.Equals(SecondCompare) && FirstMonthCompare.Equals(SecondMonthCompare))
                            {
                                return Recurring;
                            }
                        }
                        break;
                    case RecurringEvents.WeekAndMonth:
                        {
                            DayOfWeek FirstCompare = processCalendar.GetDayOfWeek(compareTime);
                            DayOfWeek SecondCompare = processCalendar.GetDayOfWeek(this.Date);
                            int FirstMonthCompare = processCalendar.GetMonth(compareTime);
                            int SecondMonthCompare = processCalendar.GetMonth(this.Date);
                            if (FirstCompare.Equals(SecondCompare) && FirstMonthCompare.Equals(SecondMonthCompare))
                            {
                                return Recurring;
                            }
                            break;
                        }
                    case RecurringEvents.Week:
                        {
                            DayOfWeek FirstCompare = processCalendar.GetDayOfWeek(compareTime);
                            DayOfWeek SecondCompare = processCalendar.GetDayOfWeek(this.Date);
                            if (FirstCompare.Equals(SecondCompare))
                            {
                                return Recurring;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            return RecurringEvents.None;
        }

        /// <summary>
        /// Removes the time component of a DateTime object, thus leaving only the date part.
        /// </summary>
        /// <param name="value">the DateTime object to be processed.</param>
        /// <returns>the DateTime object containing only the date part of the original DateTime object.</returns>
        public static DateTime TruncateTimeComponent(DateTime value)
        {
            return value.Subtract(value.TimeOfDay);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">the DateTime object associated with this particular RadCalendarDay.</param>
        /// <returns></returns>
        public static RadCalendarDay CreateDay(DateTime date)
        {
            return new RadCalendarDay(date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">the DateTime object associated with this particular RadCalendarDay.</param>
        /// <param name="owner">the CalendarDayCollection that contains this particular RadCalendarDay.</param>
        /// <returns></returns>
        internal protected static RadCalendarDay CreateDay(DateTime date, CalendarDayCollection owner)
        {
            RadCalendarDay temp = new RadCalendarDay(date);
            temp.Owner = owner;
            return temp;
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Used to handle all requests for layout invalidation through a single place
        /// </summary>
        [Browsable(false)]
        internal bool DirtyLayout
        {
            get
            {
                return this.dirtyLayout;
            }
            set
            {
                if (this.dirtyLayout != value)
                {
                    this.dirtyLayout = value;
                    this.OnNotifyPropertyChanged("DirtyLayout");
                }
            }
        }

        /// <summary>
        /// Used to handle all requests for repainting through a single place
        /// </summary>
        [Browsable(false)]
        internal bool DirtyPaint
        {
            get
            {
                return this.dirtyPaint;
            }
            set
            {
                if (this.dirtyPaint != value)
                {
                    this.dirtyPaint = value;
                    this.OnNotifyPropertyChanged("DirtyPaint");
                }
            }
        }

        /// <summary>
        /// Occurs when when a property of an object changes change. 
        /// Calling the event is developer's responsibility.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="e">PropertyChangedEventArgs instance containing the name of the property.</param>
        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            //Code extracted from all the properties and refactored
            switch (e.PropertyName)
            {
				case "Selectable":
			//		this.owner.Calendar.SetFocusedDateView();
					break;
				case "Selected":
			//		this.owner.Calendar.SetFocusedDateView();
					break;
				case "Disabled":
			//		this.owner.Calendar.SetFocusedDateView();
					break;
				case "Recurring":
			//		this.owner.Calendar.SetFocusedDateView();
					break;
				case "ToolTip":
			//		this.owner.Calendar.SetFocusedDateView();
					break;
				case "TemplateItem":
				case "Image":
                    if (this.owner != null && this.owner.Calendar != null)
                    {
                        this.owner.Calendar.SetFocusedDateView();
                    } break;

                case "DirtyPaint":
                case "DirtyLayout":
                    break;
                default:
                    break;
            }
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
        #endregion
	}
}