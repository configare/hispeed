using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Summary description for CalendarView.
	/// </summary>
    public class MultiMonthView : MonthView
    {
        #region Fields
        private int previousViews = 0;
        private int nextViews = 0;
        #endregion

        #region Constructors
        public MultiMonthView(RadCalendar parent)
            : base(parent)
        {
        }

        public MultiMonthView(RadCalendar parent, DateTime inMonthDate)
            : this(parent, inMonthDate, null)
        {
        }

        public MultiMonthView(RadCalendar parent, DateTime inMonthDate, CalendarView parentView)
            : base(parent, inMonthDate, parentView)
        {

        }
        #endregion

        #region Properties
        public override bool IsMultipleView
        {
            get
            {
                return true;
            }
        }

        internal protected override int MonthsInView
        {
            get
            {
                this.EnsureChildViews();
                return this.children.Count;
            }
        }

        #endregion

        #region Methods

        protected internal override CalendarView CreateView()
        {
            return new MultiMonthView(this.Calendar);
        }

        internal override void Initialize()
        {
            this.EnsureChildViews();
            this.children.Clear();
            base.Initialize();
        }

        internal protected override void EnsureRenderSettings()
        {
            if (!this.Equals(this.Calendar.DefaultView))
            {
                throw new FormatException("Multiview mode is allowed only for top calendar views (not for their descendants).");
            }
        }

        internal override string GetTitleContent()
        {
            string begin = string.Empty;
            string end = string.Empty;
            string separator = "-";

            if (this.IsRootView)
            {
                begin = this.ViewStartDate.ToString(this.Calendar.TitleFormat, this.Calendar.Culture);
                end = this.ViewEndDate.ToString(this.Calendar.TitleFormat, this.Calendar.Culture);
            }
            else
            {
                begin = this.ViewStartDate.ToString(this.TitleFormat, this.Calendar.DateTimeFormat);
                end = this.ViewEndDate.ToString(this.TitleFormat, this.Calendar.DateTimeFormat);
            }
            if (this.Calendar != null)
            {
                separator = this.Calendar.DateRangeSeparator;
            }
            return begin + separator + end;
        }

        protected internal override CalendarView CreateView(DateTime date)
        {
            MultiMonthView view = new MultiMonthView(this.Calendar, date);
            view.Initialize(this);
            return view;
        }

        protected override void SetDateRange()
        {
            // initialize child views
            this.EnsureChildViews();
            this.InitializeMultiViewData();

            if (this.children.Count > 0)
            {
                this.ViewStartDate = this.children[0].ViewStartDate;
                this.ViewRenderStartDate = this.children[0].ViewRenderStartDate;
                this.ViewEndDate = this.children[this.children.Count - 1].ViewEndDate;
                this.ViewRenderEndDate =
                    this.children[this.children.Count - 1].ViewRenderEndDate;
            }
        }

        /// <summary>
        /// Calculates the correct position of the CalendarView
        /// </summary>
        protected virtual void InitializeFocusedViewPosition()
        {
            this.previousViews =
                this.Calendar.CurrentViewColumn +
                (this.Calendar.CurrentViewRow * this.Calendar.MultiViewColumns);

            this.nextViews =
                this.Calendar.MultiViewColumns - (this.Calendar.CurrentViewColumn + 1)
                + (this.Calendar.MultiViewRows - (this.Calendar.CurrentViewRow + 1)) *
                    this.Calendar.MultiViewColumns;
        }

        internal void InitializeMultiViewData()
        {
            this.InitializeFocusedViewPosition();

            if (this.previousViews < 0 || this.nextViews < 0)
                return;

            this.EnsureChildViews();
            this.children.Clear();


            MonthView[] previousViewsArray = new MonthView[this.previousViews];
            MonthView[] nextViewsArray = new MonthView[this.nextViews];

            DateTime startDate = this.EffectiveVisibleDate();
            MonthView defaultView = new MonthView(this.Calendar, startDate, this);

            // populating the next views
            for (int i = 0; i < this.nextViews; i++)
            {
                DateTime calculatedViewDate = this.Calendar.CurrentCalendar.AddMonths(startDate, (i + 1));
                nextViewsArray[i] = new MonthView(this.Calendar, calculatedViewDate, this);
            }
            // populating the previous views
            int step = 1;
            for (int i = this.previousViews - 1; 0 <= i; i--)
            {
                DateTime calculatedViewDate = this.Calendar.CurrentCalendar.AddMonths(startDate, -step);
                previousViewsArray[i] = new MonthView(this.Calendar, calculatedViewDate, this);
                step += 1;
            }
            //Children collection population
            for (int i = 0; i < this.previousViews; i++)
            {
                this.children.Add(previousViewsArray[i]);
            }
            defaultView.Initialize();
            this.children.Add(defaultView);
            for (int i = 0; i < this.nextViews; i++)
            {
                this.children.Add(nextViewsArray[i]);
            }
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
                this.HandlePageDownKey(keys);
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
                this.HandlePageUpKey(keys);
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