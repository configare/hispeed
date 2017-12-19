using System;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Summary description for CalendarView.
	/// </summary>
	public class MonthView : CalendarView
	{
        // Fields
        private int monthDays = 0;
        private DateTime viewInMonthDate = DateTime.MinValue;

		#region Constructors
			public MonthView(RadCalendar parent): base(parent)
			{
			}

			public MonthView(RadCalendar parent, DateTime inMonthDate): this(parent, inMonthDate, null)
			{
			}

			public MonthView(RadCalendar parent, DateTime inMonthDate, CalendarView parentView): base(parent, parentView)
			{
				this.viewInMonthDate = inMonthDate;
				this.Initialize();
			}
		#endregion

        #region Properties
        public override bool IsMultipleView
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the number of months displayed by a particular MonthView (in this case 1).
        /// </summary>
        internal protected virtual int MonthsInView
        {
            get
            {
                return 1;
            }
        } 
        #endregion

        #region Methods

		protected internal override CalendarView CreateView()
		{
			return new MonthView(this.Calendar);
		}

        internal override void Initialize()
        {
            base.Initialize();
            this.SetDateRange();
            this.TitleFormat = "MMMM";
        }

        internal protected override void EnsureRenderSettings()
        {
            int maxCells = 42;
            string errMessage = string.Empty;
            if ((maxCells%this.Columns!=0)
                ||(maxCells%this.Rows!=0)
                )
            {
                errMessage += "The product of (MonthColumns x MonthRows) differs from 42 which is the correct value.";
            }
            if ((this.Columns < 7) &&
                this.ShowColumnHeaders &&
                this.AllowColumnHeaderSelectors &&
                this.Orientation == Orientation.Horizontal)
            {
                errMessage += "The current combination of the properties: \n[ShowColumnHeaders, UseColumnHeadersAsSelectors, Orientation] and MonthColumns < 7 does not allow proper rendering of Telerik RadCalendar. Please correct.";
            }
            if ((this.Rows<7) &&
                this.ShowRowHeaders &&
                this.AllowRowHeaderSelectors &&
                this.Orientation == Orientation.Vertical)
            {
                errMessage += "The current combination of the properties: \n[ShowRowHeaders, UseRowHeadersAsSelectors, Orientation] and MonthRows < 7 does not allow proper rendering of Telerik RadCalendar. Please correct.";
            }
            if (errMessage != string.Empty)
            {
                throw new ArgumentException(errMessage);
            }
        }

        #region Formatting
        internal override string GetTitleContent()
        {
            if (this.Calendar != null)
            {
                if (this.IsRootView)
                {
                    return this.EffectiveVisibleDate().ToString(this.Calendar.TitleFormat, this.Calendar.Culture);
                }
                else
                {
                    return this.EffectiveVisibleDate().ToString(this.TitleFormat, this.Calendar.DateTimeFormat);
                }
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// Gets the string representation for a particular day in the week.
        /// </summary>
        /// <param name="weekDay">Specifies the day of the week.</param>
        /// <returns>the string representation for the specified day.</returns>
        internal protected virtual string GetDayHeaderString(int weekDay)
        {
            DateTimeFormatInfo dateLocalInfo = this.Calendar.DateTimeFormat;
            DayNameFormat dayFormat = this.Calendar.DayNameFormat;
            string dayString = String.Empty;
            switch (dayFormat)
            {
                case DayNameFormat.Full:
                    {
                        dayString = dateLocalInfo.GetDayName((DayOfWeek)weekDay);
                        break;
                    }
                case DayNameFormat.FirstLetter:
                    {
                        string str = dateLocalInfo.ShortestDayNames[weekDay];
                        TextElementEnumerator iter = StringInfo.GetTextElementEnumerator(str);
                        iter.MoveNext();
                        dayString = iter.Current.ToString();
                        break;
                    }
                case DayNameFormat.FirstTwoLetters:
                    {
                        string str = dateLocalInfo.ShortestDayNames[weekDay];
                        TextElementEnumerator iter = StringInfo.GetTextElementEnumerator(str);
                        iter.MoveNext();
                        StringBuilder ftl = new StringBuilder(iter.Current.ToString());
                        if (iter.MoveNext()) //in case of Arabic cultures
                        {
                            ftl.Append(iter.Current.ToString());
                        }
                        dayString = ftl.ToString();
                        break;
                    }
                case DayNameFormat.Short:
                    {
                        dayString = dateLocalInfo.GetAbbreviatedDayName((DayOfWeek)weekDay);
                        break;
                    }
                case DayNameFormat.Shortest:
                default:
                    {
                        dayString = dateLocalInfo.ShortestDayNames[weekDay];
                        break;
                    }
            }
            return dayString;
        }

        /// <summary>
        /// Retrieves the ToolTip text associated with a particular RadCalendarDay object.
        /// </summary>
        /// <param name="calendarDay">RadCalendarDay object</param>
        /// <returns>The retrieved ToolTip text associated with a particular RadCalendarDay object</returns>
        internal protected virtual string GetToolTip(RadCalendarDay calendarDay)
        {
            if (calendarDay.ToolTip != string.Empty)
            {
                return calendarDay.ToolTip;
            }
            else
            {
                return this.GetToolTip(calendarDay.Date);
            }
        }

        internal protected virtual string GetToolTip(DateTime processedDate)
        {
            if (this.CellToolTipFormat != string.Empty)
            {
                return processedDate.ToString(this.CellToolTipFormat, this.Calendar.Culture);
            }
            return processedDate.ToString();
        }

        /// <summary>
        /// Gets the RadCalendarDay object associated with a particular DateTime object if any.
        /// </summary>
        /// <param name="processedDate">DateTime object to be tested.</param>
        /// <returns>The retrieved RadCalendarDay object.</returns>
        internal protected virtual RadCalendarDay GetSpecialDay(DateTime processedDate)
        {
            if (this.Calendar != null)
            {
                RadCalendarDay tempDay = this.Calendar.SpecialDays[processedDate];
                if (tempDay == null)
                {
                    RecurringEvents matches = RecurringEvents.None;
                    for (int i = 0; i < this.Calendar.SpecialDays.Count; i++)
                    {
                        matches = this.Calendar.SpecialDays[i].IsRecurring(processedDate, this.Calendar.CurrentCalendar);
                        if (matches != RecurringEvents.None)
                        {
                            tempDay = this.Calendar.SpecialDays[i];
                            //this.Calendar.AddViewRepeatableDay(processedDate.ToString("yyyy_M_d"), tempDay.Date.ToString("yyyy_M_d"));
                            break;
                        }
                    }
                }
                return tempDay;
            }

            return null;
        } 
        #endregion

		protected override DateTime AddViewPeriods(DateTime startDate, int periods)
		{
            return this.Calendar.CurrentCalendar.AddMonths(startDate, periods * MonthsInView);
		}

		internal protected  override CalendarView CreateView(DateTime date)
		{
			MonthView view = new MonthView(this.Calendar, date);
			view.Initialize(this);
			return view;
		}

		internal override CalendarView GetPreviousView(int steps)
		{
            DateTime newDate = this.ViewStartDate.AddMonths(-(steps * this.MonthsInView));
            if (newDate < this.Calendar.RangeMinDate )
            {
                return CreateView(this.Calendar.RangeMinDate.AddDays(1));     
            }
            else
            {
                return CreateView(newDate);
            }
		}

		internal override CalendarView GetNextView(int steps)
		{
            DateTime newDate = this.ViewStartDate.AddMonths(steps * this.MonthsInView);
            if (newDate > this.Calendar.RangeMaxDate)
            {
                return CreateView(this.Calendar.RangeMaxDate);
            }
            else
            {
                return CreateView(newDate);
            }
		}

        protected override void SetDateRange()
        {
            this.ViewStartDate = this.EffectiveVisibleDate();
            this.monthDays = this.CurrentCalendar.GetDaysInMonth(this.ViewStartDate.Year, this.ViewStartDate.Month);
            this.ViewEndDate = this.CurrentCalendar.AddDays(this.ViewStartDate, this.monthDays - 1);
            this.ViewRenderStartDate = FirstCalendarDay(this.ViewStartDate);

            TimeSpan span = new TimeSpan((this.Rows * this.Columns - 1), 0, 0, 0);

            this.ViewRenderEndDate = (DateTime.MaxValue.Ticks > (span.Ticks + ViewStartDate.Ticks)) ?
                this.CurrentCalendar.AddDays(ViewStartDate, ( ( this.Rows * this.Columns ) - 1)) : DateTime.MaxValue;
        }

		internal protected override DateTime EffectiveVisibleDate()
		{
			DateTime tempDate = 
                (this.viewInMonthDate != DateTime.MinValue) ? 
                this.viewInMonthDate : base.EffectiveVisibleDate();
            // the first day of the visible month
            return this.CurrentCalendar.AddDays(tempDate, -(this.CurrentCalendar.GetDayOfMonth(tempDate) - 1));
		}

		internal /*protected*/ virtual DateTime FirstCalendarDay(DateTime visibleDate)
		{
            //visibleDate is the product of EffectiveVisibleDate
			DateTime time1 = visibleDate;
            int num1 = 
                ((int)this.Calendar.CurrentCalendar.GetDayOfWeek(time1)) - this.NumericFirstDayOfWeek();
			if (num1 <= 0)
			{
				num1 += 7;
			}

            if (time1 == DateTime.MinValue)
            {
                return DateTime.MinValue;
            }
            // the date that is the beginning of the rendered calendar
            return this.Calendar.CurrentCalendar.AddDays(time1, -num1);
		}

        /// <summary>
        /// Gets the month name.
        /// </summary>
        private /*internal protected virtual*/ string GetMonthName(int m, bool bFull)
		{
			if (bFull)
			{
				return this.Calendar.DateTimeFormat.GetMonthName(m);
			}
			return this.Calendar.DateTimeFormat.GetAbbreviatedMonthName(m);
		}

        private /*internal protected virtual*/ int NumericFirstDayOfWeek()
		{
			if (this.Calendar.FirstDayOfWeek != FirstDayOfWeek.Default)
			{
				return (int) this.Calendar.FirstDayOfWeek;
			}
			return (int) this.Calendar.DateTimeFormat.FirstDayOfWeek;
		}

        #region Keyboard Navigation
        protected override void HandlePageDownKey(Keys keys)
        {
            this.Calendar.FocusedDate = this.CurrentCalendar.AddMonths(this.Calendar.FocusedDate, 1);
        }

        protected override void HandlePageUpKey(Keys keys)
        {
            this.Calendar.FocusedDate = this.CurrentCalendar.AddMonths(this.Calendar.FocusedDate, -1);
        }

        protected override void HandleEndKey(Keys keys)
        {
            this.Calendar.FocusedDate = this.ViewEndDate;
        }

        protected override void HandleHomeKey(Keys keys)
        {
            this.Calendar.FocusedDate = this.ViewStartDate;
        }

        protected override void HandleDownKey(Keys keys)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                this.Calendar.FocusedDate = this.CurrentCalendar.AddDays(this.Calendar.FocusedDate, 1);
                return;
            }
            this.Calendar.FocusedDate = this.CurrentCalendar.AddWeeks(this.Calendar.FocusedDate, 1);
        }

        protected override void HandleUpKey(Keys keys)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                this.Calendar.FocusedDate = this.CurrentCalendar.AddDays(this.Calendar.FocusedDate, -1);
                return;
            }
            this.Calendar.FocusedDate = this.CurrentCalendar.AddWeeks(this.Calendar.FocusedDate, -1);
        }

        protected override void HandleLeftKey(Keys keys)
        {
            if ((keys & Keys.Control) == Keys.Control)
            {
                this.HandlePageUpKey(keys);
                return;
            }
            if (this.Orientation == Orientation.Vertical)
            {
                this.Calendar.FocusedDate = this.CurrentCalendar.AddWeeks(this.Calendar.FocusedDate, -1);
                return;
            }
            this.Calendar.FocusedDate = this.CurrentCalendar.AddDays(this.Calendar.FocusedDate, -1);
        }

        protected override void HandleRightKey(Keys keys)
        {
            if ((keys & Keys.Control) == Keys.Control)
            {
                this.HandlePageDownKey(keys);
                return;
            }
            if (this.Orientation == Orientation.Vertical)
            {
                this.Calendar.FocusedDate = this.CurrentCalendar.AddWeeks(this.Calendar.FocusedDate, 1);
                return;
            }
            this.Calendar.FocusedDate = this.CurrentCalendar.AddDays(this.Calendar.FocusedDate, 1);
        }

        protected override void ToggleSelection(Keys keys)
        {
            if (this.Calendar.ReadOnly)
            {
                return;
            }
            SelectionEventArgs args = this.Calendar.CallOnSelectionChanging(this.Calendar.SelectedDates);
            if (args.Cancel)
            {
                return;
            }
            this.Calendar.SelectedDates.BeginUpdate();

            if (!this.Calendar.AllowMultipleSelect)
            {
                this.Calendar.SelectedDates.Clear();
            }
            if (this.Calendar.SelectedDates.Contains(this.Calendar.FocusedDate))
            {
                this.Calendar.SelectedDates.Remove(this.Calendar.FocusedDate);
            }
            else
            {
                this.Calendar.SelectedDates.Insert(0, this.Calendar.FocusedDate);
            }
            this.Calendar.SelectedDates.EndUpdate();
            this.Calendar.CallOnSelectionChanged();
        } 
        #endregion

        #endregion
	}
}