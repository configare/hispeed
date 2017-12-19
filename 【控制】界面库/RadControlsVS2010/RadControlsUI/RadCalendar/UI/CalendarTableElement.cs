using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    #region Enums
    public enum CellSelectionType
    {
        Row,
        Column,
        Cell,
        AllCells
    } 
    #endregion

    public class CalendarTableElement : CalendarVisualElement /*LightVisualElement*/
    {
        #region Fields
        protected int columns;
        protected int rows;

        protected int contentXShift = 0;
        protected int contentYShift = 0;

        protected List<LightVisualElement> visualElements;
        protected LightVisualElement selectedElement;
        private bool allowFadeAndAnimation;
        protected CalendarView view = null;

        private int zIndex;
        private DateTime selectionStartDate;
        private DateTime minSelectedDate;

        private CalendarCellElement lastSelectedCell;

        private DateTime selectionEndDate;
        #endregion

        #region Constructors
        internal protected CalendarTableElement(CalendarVisualElement owner, RadCalendar calendar, CalendarView view)
            : base(null, calendar, view)
        {
            this.Owner = owner;
            this.view = view;
            this.view.Rows = this.Calendar.Rows;
            this.view.Columns = this.Calendar.Columns;

            this.view.PropertyChanged += new PropertyChangedEventHandler(view_PropertyChanged);
            this.InitializeChildren();
        }

        internal protected CalendarTableElement(CalendarVisualElement owner)
            : this(owner, null, null)
        {
        } 
        #endregion

        #region Properties
        internal List<LightVisualElement> VisualElements
        {
            get
            {
                if (this.visualElements == null)
                {
                    this.CreateVisuals();
                }
                return this.visualElements;
            }
            set
            {
                visualElements = value;
            }
        }

        public virtual int Rows
        {
            get
            {
                return this.View.Rows;
            }
        }

        public virtual int Columns
        {
            get
            {
                return this.View.Columns;
            }
        }
        #endregion

        #region CalendarFunctionality

        private void calendar_MouseMove(object sender, MouseEventArgs e)
        {
            DraggingSelection(e);

            // Fish Eye functionality 
            if (e.Button == MouseButtons.Left && this.View.Children.Count == 0)
            {
                foreach (CalendarCellElement cell in this.VisualElements)
                {
                    // checks if the cell is containing the current cursor position 

                    bool isHeader = (bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                    if (isHeader)
                        continue;

                    if (cell.ControlBoundingRectangle.Contains(e.Location))
                    {
                        if (this.Calendar.AllowFishEye && !cell.isAnimating && cell.AutoSize)
                        {
                            cell.ZIndex = this.VisualElements.Count + zIndex;
                            zIndex++;
                            cell.PerformForwardAnimation();
                            cell.SetValue(CalendarCellElement.IsZoomingProperty, true);
                        }
                    }
                    else if (this.Calendar.AllowFishEye && (!cell.AutoSize))
                    {
                        // performs the zoom out animation
                        cell.PerformReverseAnimation();
                        cell.SetValue(CalendarCellElement.IsZoomingProperty, false);
                    }
                }
            }
        }

        private void DraggingSelection(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.Calendar.AllowMultipleSelect && this.Calendar.AllowSelect)
            {
                if (selectionStartDate != DateTime.MinValue)
                {
                    SelectOnMouseMove(e);
                }
            }
        }

        private void calendar_MouseDown(object sender, MouseEventArgs e)
        {
            // Applies the zooming when the mouse down event is fired and the selected cell's area  
            // contains the current mouse position

            if (this.Calendar.ReadOnly)
                return;

            if (this.View.Children.Count == 0)
            {
                foreach (CalendarCellElement cell in this.VisualElements)
                {
                    if (this.Calendar.AllowFishEye && cell.AutoSize &&
                        this.Calendar.FocusedDate.Equals(cell.Date) &&
                        cell.ControlBoundingRectangle.Contains(e.Location))
                    {
                        bool isHeader = (bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                        if (isHeader)
                        {
                            continue;
                        }

                        cell.ZIndex = this.VisualElements.Count + zIndex;
                        zIndex++;
                        cell.SetValue(CalendarCellElement.IsZoomingProperty, true);
                        cell.PerformForwardAnimation();
                    }
                }
            }

            SetCalendarCellSelectedState(e);
        }

        private void SetCalendarCellSelectedState(MouseEventArgs e)
        {
            if (this.Calendar.AllowSelect && this.View as MultiMonthView == null)
            {
                foreach (CalendarCellElement cell in this.VisualElements)
                {
                    if (cell.HitTest(e.Location))
                    {
                        int res = this.Calendar.MultiViewRows * this.Calendar.MultiViewColumns;
                        int realChildren = res;
                        this.GetInitializedChildrenCount(ref res, ref realChildren);

                        if (res == realChildren)
                        {
                            this.selectionStartDate = cell.Date;
                            this.selectionEndDate = cell.Date;
                            this.minSelectedDate = cell.Date;
                            this.SelectCell(cell);
                            break;
                        }
                    }
                }
            }
        }

        private void SelectOnMouseMove(MouseEventArgs e)
        {
            if (this.View as MultiMonthView == null)
            {
                foreach (CalendarCellElement cell in this.VisualElements)
                {
                    if (this.lastSelectedCell == null)
                    {
                        this.lastSelectedCell = cell;
                    }

                    if (cell.HitTest(e.Location) && this.lastSelectedCell.Date != cell.Date)
                    {
                        this.lastSelectedCell = cell;
                        int res = this.Calendar.MultiViewRows * this.Calendar.MultiViewColumns;
                        int realChildren = res;
                        GetInitializedChildrenCount(ref res, ref realChildren);

                        if (res == realChildren)
                        {
                            if (cell.OtherMonth)
                                return;

                            bool isHeader = (bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                            if (isHeader)
                                return;

                            SelectCellsRange(this.selectionStartDate, cell.Date);
                            break;
                        }
                    }
                }
            }
        }

        private void SelectCellsRange(DateTime selectionStart, DateTime selectionEnd)
        {
            if (!this.Calendar.AllowMultipleSelect || !this.Calendar.AllowSelect)
            {
                return;
            }            

            DateTime startDate = selectionStart;
            DateTime endDate = selectionEnd;

            if (selectionEndDate < endDate)
            {
                selectionEndDate = endDate;
            }

            if (this.minSelectedDate > endDate)
            {
                this.minSelectedDate = endDate;
            }

            if (startDate > endDate)
            {
                DateTime dummyTime = startDate;
                startDate = endDate;
                endDate = dummyTime;
            }

            bool foundCell = false;
            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Date >= startDate && calendarCell.Date <= endDate)
                {
                    bool isHeader = (bool)calendarCell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                    if (isHeader)
                    {
                        continue;
                    }

                    if (!calendarCell.Enabled)
                    {
                        continue;
                    }

                    foundCell = true;
                    break;
                }
            }

            if( !foundCell )
            {
                return;
            }

            this.Calendar.SelectedDates.BeginUpdate();

            SelectionEventArgs args = this.Calendar.CallOnSelectionChanging(this.Calendar.SelectedDates);

            if (args.Cancel)
            {
                this.Calendar.SelectedDates.EndUpdate();
                return;
            }

            ResetCellsSelectionInRange(this.minSelectedDate, this.selectionEndDate);
            ResetCellsSelectionLogicallyInRange(this.minSelectedDate, this.selectionEndDate);

            List<DateTime> dates = new List<DateTime>();

            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Date >= startDate && calendarCell.Date <= endDate)
                {
                    bool isHeader = (bool)calendarCell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                    if (isHeader)
                    {
                        continue;
                    }

                    if (!calendarCell.Enabled)
                    {
                        continue;
                    }

                    calendarCell.Selected = true;
                    dates.Add(calendarCell.Date);
                }
            }

            this.Calendar.SelectedDates.AddRange(dates.ToArray());

            this.Calendar.SelectedDates.EndUpdate();
            this.Calendar.CallOnSelectionChanged();
        }

        private void GetInitializedChildrenCount(ref int res, ref int realChildren)
        {
            MultiMonthViewElement multiMonthView = this.Calendar.CalendarElement.CalendarVisualElement as MultiMonthViewElement;

            if (multiMonthView != null)
            {
                realChildren = (multiMonthView.Children[0].Children[1] as CalendarMultiMonthViewTableElement).Children.Count;
            }

            if (!(this.Calendar.MultiViewColumns > 1 || this.Calendar.MultiViewRows > 1))
            {
                res = realChildren = 1;
            }
            else
            {
                RadElement parentElement = this.Parent;
                res = 1;
                realChildren = 3;

                while (parentElement != null)
                {
                    if (parentElement.GetType() == typeof(MultiMonthViewElement))
                    {
                        res = realChildren = 1;
                        break;
                    }

                    parentElement = parentElement.Parent;
                }
            }
        }

        private void calendar_MouseUp(object sender, MouseEventArgs e)
        {
            // UnApplies the zooming when the mouse up event is fired. 

            this.selectionStartDate = DateTime.MinValue;


            if (this.Calendar.MultiViewColumns > 1 || this.Calendar.MultiViewRows > 1)
            {
                foreach (MonthViewElement monthViewElement in (this.Calendar.CalendarElement.CalendarVisualElement as MultiMonthViewElement).GetMultiTableElement().Children)
                {
                    foreach (CalendarCellElement cell in monthViewElement.TableElement.VisualElements)
                    {
                        bool isHeader = (bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                        if (isHeader)
                            continue;

                        if (cell.AllowFishEye || this.Calendar.AllowFishEye)
                        {
                            cell.PerformReverseAnimation();
                            cell.SetValue(CalendarCellElement.IsZoomingProperty, false);
                        }
                    }
                }

                return;
            }

            if (this.View.Children.Count == 0)
            {
                foreach (CalendarCellElement cell in this.VisualElements)
                {
                    bool isHeader = (bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                    if (isHeader)
                        continue;

                    if (cell.AllowFishEye || this.Calendar.AllowFishEye)
                    {
                        cell.PerformReverseAnimation();
                        cell.SetValue(CalendarCellElement.IsZoomingProperty, false);
                    }
                }
            }

            RenderContent();
        }

        private void view_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            SetBehaviorOnPropertyChange(propertyName);
        }

        protected virtual void SetBehaviorOnPropertyChange(string propertyName)
        {
            DateTime VisibleDate;
            DateTime FirstDay;

            switch (propertyName)
            {
                case "AllowViewSelector":
                case "AllowRowHeaderSelectors":
                case "AllowColumnHeaderSelectors":
                    this.RefreshVisuals(true);
                    break;

                case "MonthLayout":
                case "Orientation":
                case "ShowColumnHeaders":
                case "ShowRowHeaders":
                case "HeaderHeight":
                case "HeaderWidth":
                case "MultiViewRows":
                case "MultiViewColumns":
                case "ShowViewSelector":
                case "Columns":
                case "Rows":
                case "ShowOtherMonthsDays":
                    this.ResetVisuals();
                    break;
                case "CellHorizontalSpacing":
                    this.CellHorizontalSpacing = this.View.CellHorizontalSpacing;
                    break;
                case "CellVerticalSpacing":
                    this.CellVerticalSpacing = this.View.CellVerticalSpacing;
                    break;

                case "ZoomFactor":
                    SetCellsProperty("ZoomFactor");
                    break;
                case "AllowFishEye":
                    SetCellsProperty("AllowFishEye");
                    break;
                case "ViewSelectorText":
                    if (this.Calendar.ShowViewSelector)
                    {
                        this.VisualElements[0].Text = this.view.ViewSelectorText;
                    }
                    break;
                case "ViewSelectorImage":
                    if (this.Calendar.ShowViewSelector)
                    {
                        this.VisualElements[0].Image = this.view.ViewSelectorImage;
                    }
                    break;
                case "RowHeaderImage":
                    VisibleDate = this.View.EffectiveVisibleDate();
                    FirstDay = (this.View as MonthView).FirstCalendarDay(VisibleDate);
                    this.InvalidateRowHeaders(FirstDay);
                    break;
                case "RowHeaderText":
                    VisibleDate = this.View.EffectiveVisibleDate();
                    FirstDay = (this.View as MonthView).FirstCalendarDay(VisibleDate);
                    this.InvalidateRowHeaders(FirstDay);
                    break;
                case "ColumnHeaderImage":
                    VisibleDate = this.View.EffectiveVisibleDate();
                    FirstDay = (this.View as MonthView).FirstCalendarDay(VisibleDate);
                    this.InvalidateColumnHeaders(FirstDay);
                    break;
                case "ColumnHeaderText":
                    VisibleDate = this.View.EffectiveVisibleDate();
                    FirstDay = (this.View as MonthView).FirstCalendarDay(VisibleDate);
                    this.InvalidateColumnHeaders(FirstDay);
                    break;
                case "CellPadding":
                    this.SetCellsProperty("CellPadding");
                    break;
                case "CellMargin":
                    this.SetCellsProperty("CellMargin");
                    break;
            }
        }

        private void SetCellsProperty(string propertyName)
        {
            // Sets a property to all cells
            foreach (CalendarCellElement cell in this.VisualElements)
            {
                switch (propertyName)
                {
                    case "ZoomFactor":
                        cell.ZoomFactor = this.view.ZoomFactor;
                        break;
                    case "AllowFishEye":
                        cell.AllowFishEye = this.Calendar.AllowFishEye;
                        break;
                    case "ShowOtherMonthsDays":
                        if (cell.OtherMonth)
                        {
                            if (this.view.ShowOtherMonthsDays)
                            {
                                cell.Visibility = ElementVisibility.Visible;
                            }
                            else
                            {
                                cell.Visibility = ElementVisibility.Hidden;
                            }
                        }
                        break;
                    case "CellMargin":
                        cell.Margin = this.view.CellMargin;
                        break;
                    case "CellPadding":
                        cell.Padding = this.view.CellPadding;
                        break;
                    case "Focused":
                        bool isFocused = cell.Date == Calendar.FocusedDate ? true : false;
                        cell.Focused = isFocused;
                        break;
                }
            }
        }
        #endregion

        #region Layout
        private CalendarCellElement rowHeaderVisualElement;
        private CalendarCellElement columnHeaderVisualElement;

        protected virtual void InitializeChildren()
        {
            this.DisposeChildren();

            this.rowHeaderVisualElement = new CalendarCellElement(this);
            this.columnHeaderVisualElement = new CalendarCellElement(this);

            this.rowHeaderVisualElement.Class = "RowHeaderVisualElement";
            this.columnHeaderVisualElement.Class = "ColumnHeaderVisualElement";

            this.rowHeaderVisualElement.SetDefaultValueOverride(LightVisualElement.DrawFillProperty, true);
            this.rowHeaderVisualElement.SetDefaultValueOverride(LightVisualElement.DrawBorderProperty, true);
            this.columnHeaderVisualElement.SetDefaultValueOverride(LightVisualElement.DrawFillProperty, true);
            this.columnHeaderVisualElement.SetDefaultValueOverride(LightVisualElement.DrawBorderProperty, true);

            this.Children.Add(this.rowHeaderVisualElement);
            this.Children.Add(this.columnHeaderVisualElement);

            for (int i = 0; i < this.VisualElements.Count; ++i)
            {
                this.visualElements[i].ZIndex = 2 + i;
                this.Children.Add(visualElements[i]);
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);

            Size res = Size.Empty;
            if (this.visualElements.Count > 0)
            {
                res.Width = (int)(this.visualElements[0].DesiredSize.Width * this.columns);
                res.Height = (int)(this.visualElements[0].DesiredSize.Height * this.rows);
            }

            return res;
        }

        private int horizontalCellSpacing = 0;
        private int verticalCellSpacing = 0;

        internal protected virtual int CellHorizontalSpacing
        {
            get
            {
                return this.horizontalCellSpacing;
            }
            set
            {
                this.horizontalCellSpacing = value;
                this.InvalidateArrange();
            }
        }

        internal protected virtual int CellVerticalSpacing
        {
            get
            {
                return this.verticalCellSpacing;
            }
            set
            {
                this.verticalCellSpacing = value;
                this.InvalidateArrange();
            }
        }

        /// <summary>
        /// create table for calendar
        /// calculate size of  each cell to fit propertly into final size (size of the control)
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (this.View == null)
                return finalSize;

            if (this.visualElements.Count == 0)
                return finalSize;

            if (this.columns != 0 && this.rows != 0)
            {
                int xCellSize;
                if (this.StretchHorizontally)
                {
                    xCellSize = ((int)finalSize.Width - (horizontalCellSpacing * this.columns)) / this.Columns;
                    if (this.View.ShowRowHeaders || this.View.ShowSelector)
                    {
                        xCellSize = (((int)finalSize.Width - this.Calendar.HeaderWidth - (horizontalCellSpacing * this.columns)) / this.Columns);
                    }
                }
                else
                {
                    xCellSize = (int)this.visualElements[0].DesiredSize.Width;
                }

                int yCellSize;
                if (this.StretchHorizontally)
                {
                    yCellSize = ((int)finalSize.Height - (verticalCellSpacing * this.rows)) / this.Rows;
                    if (this.View.ShowColumnHeaders || this.View.ShowSelector)
                    {
                        yCellSize = (((int)finalSize.Height - this.Calendar.HeaderHeight - (verticalCellSpacing * this.rows)) / this.Rows);
                    }
                }
                else
                {
                    yCellSize = (int)this.visualElements[0].DesiredSize.Height;
                }

                int xOffset = 0;
                int yOffset = 0;

                if (this.View.ShowRowHeaders || this.View.ShowSelector)
                    xOffset = this.Calendar.HeaderWidth;

                if (this.View.ShowColumnHeaders || this.View.ShowSelector)
                    yOffset = this.Calendar.HeaderHeight;

                ArrangeHeaders(xCellSize, yCellSize, xOffset, yOffset);
                ArrangeContentCells(xCellSize, yCellSize, xOffset, yOffset);
            }
            return finalSize;
        }

        protected virtual void ArrangeHeaders(int xCellSize, int yCellSize, int xOffset, int yOffset)
        {
            int bottomRight = 0;

            if (this.View.ShowColumnHeaders)
            {
                columnHeaderVisualElement.Arrange(new Rectangle(xOffset, 0, (xCellSize + this.horizontalCellSpacing) * this.Calendar.Columns, this.Calendar.HeaderHeight));

                int iterateTo = this.Columns;
                for (int i = 0; i < iterateTo; i++)
                {
                    LightVisualElement cell = this.GetElement(0, i + this.contentXShift);

                    Point position = Point.Empty;

                    position = new Point(i * xCellSize + xOffset, 0);
                    position.X += (i + 1) * this.horizontalCellSpacing;

                    ((CalendarCellElement)cell).ProposedBounds = new Rectangle(position, new Size(xCellSize, yOffset));
                    cell.Arrange(new RectangleF(position, new Size(xCellSize, yOffset)));
                    bottomRight = position.X + xCellSize;
                }
            }

            if (this.View.ShowRowHeaders)
            {
                rowHeaderVisualElement.Arrange(new RectangleF(0, yOffset, this.Calendar.HeaderWidth, (yCellSize + this.verticalCellSpacing) * (this.Calendar.Rows)));

                int iterateTo = this.Rows;

                for (int i = 0; i < iterateTo; i++)
                {
                    LightVisualElement cell = GetElement(i + this.contentYShift, 0);

                    Point position = Point.Empty;

                    position = new Point(0, i * yCellSize + yOffset);
                    position.Y += (i + 1) * this.verticalCellSpacing;

                    ((CalendarCellElement)cell).ProposedBounds = new Rectangle(position, new Size(xOffset, yCellSize));

                    cell.Arrange(new RectangleF(position, new Size(xOffset, yCellSize)));
                    bottomRight = position.Y + yCellSize;
                }
            }

            if (this.View.ShowSelector)
            {
                ((CalendarCellElement)this.VisualElements[0]).ProposedBounds = new Rectangle(Point.Empty, new Size(xOffset, yOffset));

                this.VisualElements[0].Arrange(new RectangleF(0, 0, xOffset, yOffset));
            }
        }

        protected virtual void ArrangeContentCells(int xCellSize, int yCellSize, int xOffset, int yOffset)
        {
            PointF position = PointF.Empty;

            for (int i = 0; i < this.Rows; i++)
            {
                position.Y += i > 0 ? yCellSize : yOffset;
                position.Y += this.verticalCellSpacing;
                position.X = 0;

                for (int j = 0; j < this.Columns; j++)
                {

                    LightVisualElement cell = null;
                    cell = this.GetContentElement(i, j);

                    position.X += j > 0 ? xCellSize : xOffset;
                    position.X += this.horizontalCellSpacing;

                    if (cell as CalendarCellElement != null)
                    {
                        ((CalendarCellElement)cell).ProposedBounds = new Rectangle(Point.Ceiling(position), new Size(xCellSize, yCellSize));
                    }

                    cell.Arrange(new RectangleF(position, new Size(xCellSize, yCellSize)));
                }
            }
        }
        #endregion

        #region UI Methods
        public override CalendarView View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (this.view != value)
                {
                    this.View.PropertyChanged -= new PropertyChangedEventHandler(view_PropertyChanged);
                    value.PropertyChanged += new PropertyChangedEventHandler(view_PropertyChanged);
                    this.view = value;
                    this.RefreshVisuals(true);
                }
            }
        }

        internal override protected CalendarVisualElement Owner
        {
            get
            {
                return base.Owner;
            }
            set
            {
                if (this.Owner != value)
                {
                    if (this.Owner != null)
                    {
                        this.UnwireCalendarEvents(this.Owner.Calendar);
                    }
                    base.Owner = value;
                    if (this.Owner != null)
                    {
                        this.UnwireCalendarEvents(this.Owner.Calendar);
                        this.WireCalendarEvents(this.Owner.Calendar);
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override RadCalendar Calendar
        {
            get
            {
                return base.Calendar;
            }
            internal set
            {
                if (this.Calendar != value)
                {
                    this.UnwireCalendarEvents(this.Calendar);
                    base.Calendar = value;
                    this.WireCalendarEvents(this.Calendar);
                }
            }
        }

        private void UnwireCalendarEvents(RadCalendar calendar)
        {
            if (calendar != null)
            {
                calendar.MouseUp -= new MouseEventHandler(calendar_MouseUp);
                calendar.MouseDown -= new MouseEventHandler(calendar_MouseDown);
                calendar.MouseMove -= new MouseEventHandler(calendar_MouseMove);
                calendar.PropertyChanged -= new PropertyChangedEventHandler(Calendar_PropertyChanged);
            }
        }

        private void WireCalendarEvents(RadCalendar calendar)
        {
            if (calendar != null)
            {
                calendar.MouseUp += new MouseEventHandler(calendar_MouseUp);
                calendar.MouseDown += new MouseEventHandler(calendar_MouseDown);
                calendar.MouseMove += new MouseEventHandler(calendar_MouseMove);
                calendar.PropertyChanged += new PropertyChangedEventHandler(Calendar_PropertyChanged);
            }
        }

        protected override void DisposeManagedResources()
        {
            this.UnwireCalendarEvents(this.Calendar);
            this.view.PropertyChanged -= new PropertyChangedEventHandler(view_PropertyChanged);

            int count = this.VisualElements.Count;
            for (int i = 0; i < count; ++i)
            {
                CalendarCellElement cell = this.VisualElements[i] as CalendarCellElement;
                if (cell != null)
                {
                    cell.Calendar = null;
                    cell.View = null;
                    cell.Dispose();
                    cell = null;
                }
            }

            this.view = null;
            base.DisposeManagedResources();
        }

        private void Calendar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Culture")
            {
                if (this.View != null && this.View.Calendar != null)
                {
                    this.InvalidateHeaders();
                }


                this.Invalidate();
            }
        }

        public LightVisualElement SelectedElement
        {
            get
            {
                return selectedElement;
            }
            set
            {
                selectedElement = value;
            }
        }

        /// <summary>
        /// enable or disable animation on mouse click 
        /// </summary>
        protected bool AllowFadeAndAnimation
        {
            get
            {
                return allowFadeAndAnimation;
            }
            set
            {
                allowFadeAndAnimation = value;
            }
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);

            if (args.RoutedEvent.EventName == "CellClickedEvent")
            {
                this.SelectedElement = sender as LightVisualElement;

                if (this.AllowFadeAndAnimation)
                {
                    Rectangle origRectangle = sender.Parent.Bounds;
                    this.AutoSize = false;

                    AnimatedPropertySetting animate2 = new AnimatedPropertySetting(RadElement.BoundsProperty, new Rectangle(origRectangle.Right / 2, origRectangle.Bottom / 2, (int)(0.1f * origRectangle.Width), (int)(0.1f * origRectangle.Height)), new Rectangle(origRectangle.Left, origRectangle.Top, origRectangle.Width, origRectangle.Height), 10, 20);
                    animate2.ApplyValue(sender.Parent);
                    animate2.AnimationFinished += delegate
                    {
                        this.AutoSize = true;
                    };
                    AnimatedPropertySetting animate3 = new AnimatedPropertySetting(LightVisualElement.OpacityProperty, 0d, 1d, 20, 40);
                    animate3.ApplyValue(sender.Parent);
                }
            }
        }
        #endregion

        #region Selection
        private void ResetCellsSelectionLogicallyInRange(DateTime startDate, DateTime endDate)
        {
            DateTime dummyDate = startDate;
            if (startDate > endDate)
            {
                startDate = endDate;
                endDate = dummyDate;
            }

            List<DateTime> dates = new List<DateTime>();
            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Date >= startDate && calendarCell.Date <= endDate)
                {
                    dates.Add(calendarCell.Date);
                }
            }

            this.Calendar.SelectedDates.RemoveRange(dates.ToArray());
        }

        private void ResetCellsSelectionInRange(DateTime startDate, DateTime endDate)
        {
            DateTime dummyDate = startDate;
            if (startDate > endDate)
            {
                startDate = endDate;
                endDate = dummyDate;
            }

            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Date >= startDate && calendarCell.Date <= endDate)
                {
                    calendarCell.Selected = false;
                }
            }
        }

        private void ResetCellsSelectionLogically()
        {
            List<DateTime> dates = new List<DateTime>();
            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Visibility == ElementVisibility.Visible)
                {
                    dates.Add(calendarCell.Date);
                }
            }

            this.Calendar.SelectedDates.RemoveRange(dates.ToArray());
        }

        internal void ResetCellsSelection()
        {
            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Visibility == ElementVisibility.Visible)
                {
                    calendarCell.Selected = false;
                }
            }
        }

        private void SelectCell(CalendarCellElement cell)
        {
            if (cell.Date > this.Calendar.RangeMaxDate || cell.Date < this.Calendar.RangeMinDate)
            {
                return;
            }

            this.lastSelectedCell = cell;

            bool isHeader = (bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty);

            CellSelectionType cellSelectionType = CellSelectionType.Cell;
            List<CalendarCellElement> selectionCells = new List<CalendarCellElement>();
            cellSelectionType = GetCellSelectionType(cell, isHeader);
            List<DateTime> selectionDates = new List<DateTime>();

            List<DateTime> previousSelectedDates = new List<DateTime>();
            foreach (DateTime selectedDate in this.Calendar.SelectedDates)
            {
                previousSelectedDates.Add(selectedDate);
            }

            if ((cellSelectionType == CellSelectionType.Cell && isHeader) ||
                (cell.Visibility != ElementVisibility.Visible))
            {
                return;
            }

            SelectionEventArgs args = this.Calendar.CallOnSelectionChanging(this.Calendar.SelectedDates);
            if (args.Cancel)
            {
                return;
            }
            this.Calendar.SelectedDates.BeginUpdate();
            this.Calendar.BeginUpdate();

            switch (cellSelectionType)
            {
                case CellSelectionType.AllCells:
                    HandleAllCellsSelection(cell, selectionCells);
                    break;
                case CellSelectionType.Column:
                    int column = cell.Column;
                    HandleColumnSelection(cell, selectionCells, column);
                    break;
                case CellSelectionType.Row:
                    int row = cell.Row;
                    HandleRowSelection(cell, selectionCells, row);
                    break;
                case CellSelectionType.Cell:
                    HandleSimpleCellSelection(cell, isHeader, selectionCells, previousSelectedDates);
                    break;
            }

            this.Calendar.SelectedDates.BeginUpdate();
            foreach (CalendarCellElement selectedCell in selectionCells)
            {
                selectionDates.Add(selectedCell.Date);
            }

            ResetCellsSelectionLogically();
            ResetCellsSelection();
            SelectCellsLogically(selectionDates, selectionCells);

            this.Calendar.SelectedDates.EndUpdate();
            this.Calendar.EndUpdate();
            this.Calendar.CallOnSelectionChanged();
        }

        private CellSelectionType GetCellSelectionType(CalendarCellElement cell, bool isHeader)
        {
            CellSelectionType selectionType = CellSelectionType.Cell;

            if (isHeader && this.Calendar.AllowMultipleSelect && this.Calendar.ShowViewSelector &&
                   this.Calendar.AllowViewSelector && cell.Row == 0 && cell.Column == 0)
            {
                selectionType = CellSelectionType.AllCells;
                cell.isChecked = !cell.isChecked;
            }
            else
            {
                if ((this.Calendar.ShowRowHeaders || this.Calendar.ShowViewSelector) &&
                    (cell.Row == 0 && cell.Column == 0) && this.Calendar.ShowColumnHeaders)
                {
                    return selectionType;
                }

                if (isHeader && this.Calendar.AllowMultipleSelect && this.Calendar.ShowColumnHeaders &&
                     this.Calendar.AllowColumnHeaderSelectors && cell.Row == 0)
                {
                    selectionType = CellSelectionType.Column;
                    cell.isChecked = !cell.isChecked;
                }
                else
                {
                    if (isHeader && this.Calendar.AllowMultipleSelect && this.Calendar.ShowRowHeaders &&
                           this.Calendar.AllowRowHeaderSelectors && cell.Column == 0)
                    {
                        selectionType = CellSelectionType.Row;
                        cell.isChecked = !cell.isChecked;
                    }
                }
            }

            return selectionType;
        }

        private void HandleAllCellsSelection(CalendarCellElement cell, List<CalendarCellElement> selectionCells)
        {
            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Visibility != ElementVisibility.Visible)
                {
                    continue;
                }

                if (!calendarCell.Enabled)
                {
                    continue;
                }

                if (calendarCell.Date > this.Calendar.RangeMaxDate || calendarCell.Date < this.Calendar.RangeMinDate)
                {
                    continue;
                }

                calendarCell.Selected = cell.isChecked;

                if (calendarCell != cell && calendarCell.Selected)
                {
                    bool isHeaderCell = (bool)calendarCell.GetValue(CalendarCellElement.IsHeaderCellProperty);
                    calendarCell.isChecked = false;
                    if (!isHeaderCell)
                    {
                        selectionCells.Add(calendarCell);
                    }
                }
            }
        }

        private void HandleColumnSelection(CalendarCellElement cell, List<CalendarCellElement> selectionCells, int column)
        {
            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Visibility != ElementVisibility.Visible)
                    continue;

                if (!calendarCell.Enabled)
                {
                    continue;
                }

                if (calendarCell.Date > this.Calendar.RangeMaxDate || calendarCell.Date < this.Calendar.RangeMinDate)
                {
                    continue;
                }

                calendarCell.Selected = cell.isChecked;

                if (calendarCell != cell && calendarCell.Selected)
                {
                    calendarCell.isChecked = false;

                    if (calendarCell.Column == column)
                    {
                        selectionCells.Add(calendarCell);
                    }
                }
            }
        }

        private void HandleRowSelection(CalendarCellElement cell, List<CalendarCellElement> selectionCells, int row)
        {
            foreach (CalendarCellElement calendarCell in this.VisualElements)
            {
                if (calendarCell.Visibility != ElementVisibility.Visible)
                    continue;

                if (!calendarCell.Enabled)
                {
                    continue;
                }

                if (calendarCell.Date > this.Calendar.RangeMaxDate || calendarCell.Date < this.Calendar.RangeMinDate)
                {
                    continue;
                }

                calendarCell.Selected = cell.isChecked;

                if (calendarCell != cell && calendarCell.Selected)
                {
                    calendarCell.isChecked = false;

                    if (calendarCell.Row == row)
                    {
                        selectionCells.Add(calendarCell);
                    }
                }
            }
        }

        public CalendarCellElement GetCellByDate(DateTime date)
        {
            foreach (CalendarCellElement cell in this.Children)
            {
                if (cell.Date.Equals(date))
                {
                    return cell;
                }
            }

            return null;
        }

        private void HandleSimpleCellSelection(CalendarCellElement cell, bool isHeader, List<CalendarCellElement> selectionCells, List<DateTime> previousSelectedDates)
        {
            if (isHeader)
                return;
            
            if (cell.OtherMonth && !Calendar.AllowMultipleView)
            {
                DateTime date = cell.Date;
                this.Calendar.FocusedDate = date;
                if (!previousSelectedDates.Contains(date))
                {
                    this.Calendar.SelectedDate = date;
                    CalendarCellElement selectedCell = this.GetCellByDate(date);
                    selectionCells.Add(selectedCell);
                }

                if (Calendar.AllowMultipleSelect)
                {
                    foreach (CalendarCellElement calendarCell in this.VisualElements)
                    {
                        if (calendarCell.Visibility != ElementVisibility.Visible)
                            continue;

                        if (!calendarCell.Enabled)
                            continue;

                        if (calendarCell.Date.Equals(date))
                            continue;
                            
                        if (previousSelectedDates.Contains( calendarCell.Date ))
                        {
                            calendarCell.Selected = true;
                            selectionCells.Add(calendarCell);
                        }
                    }
                }

                return;
            }

            if (cell.Enabled && cell.Date <= this.Calendar.RangeMaxDate && cell.Date >= this.Calendar.RangeMinDate)
            {
                cell.Selected = !cell.Selected;
            }

            if (cell.Selected)
            {
                selectionCells.Add(cell);
            }

            if (this.Calendar.AllowMultipleSelect)
            {
                foreach (CalendarCellElement calendarCell in this.VisualElements)
                {
                    if (calendarCell.Visibility != ElementVisibility.Visible)
                        continue;

                    if (!calendarCell.Enabled)
                        continue;

                    if (calendarCell != cell && calendarCell.Selected)
                    {
                        selectionCells.Add(calendarCell);
                    }
                }
            }
            else
            {
                foreach (CalendarCellElement calendarCell in this.VisualElements)
                {
                    if (calendarCell.Visibility != ElementVisibility.Visible)
                        continue;

                    if (!calendarCell.Enabled)
                        continue;

                    calendarCell.isChecked = false;
                }
            }

            if (cell.Selected)
            {
                this.Calendar.FocusedDate = cell.Date;

                if (this.Calendar.AllowMultipleView && !this.Calendar.AllowMultipleSelect && this.Calendar.CalendarElement.CalendarVisualElement is MultiMonthViewElement)
                {
                    selectionCells.Clear();
                    MultiMonthViewElement viewElement = this.Calendar.CalendarElement.CalendarVisualElement as MultiMonthViewElement;
                    foreach (MonthViewElement monthViewElement in viewElement.GetMultiTableElement().VisualElements)
                    {
                        monthViewElement.TableElement.ResetCellsSelection();
                        foreach (RadObject obj in monthViewElement.TableElement.VisualElements)
                        {
                            CalendarCellElement calCell = obj as CalendarCellElement;
                            if (calCell != null && 
                                !(bool)calCell.GetValue(CalendarCellElement.IsHeaderCellProperty) &&
                                calCell.Date == this.Calendar.FocusedDate)
                            {
                                selectionCells.Add(calCell);
                            }
                        }
                    }
                }
            }
        }

        private void SelectCellsLogically(List<DateTime> selectionDates, List<CalendarCellElement> selectionCells)
        {
            int i = 0;

            foreach (CalendarCellElement cell in selectionCells)
            {
                cell.Selected = true;
                cell.Date = selectionDates[i];
                i++;
            }

            if (!this.Calendar.AllowMultipleSelect)
                this.Calendar.SelectedDates.Clear();

            this.Calendar.SelectedDates.AddRange(selectionDates.ToArray());
        }
        #endregion

        #region Methods
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "CalendarTableElement";
            this.AllowFadeAndAnimation = false;
        }

        internal protected virtual void CreateVisuals()
        {
            this.View.GetContentOffset(out this.contentXShift, out this.contentYShift);
            this.View.GetViewRowsAndColumns(out this.rows, out this.columns);

            this.Initialize(this.rows, this.columns);
            this.SetViewProperties();
        }

        internal protected override void RefreshVisuals(bool unconditional)
        {
            foreach (CalendarCellElement processedCell in VisualElements)
            {
                processedCell.Focused = false;
                processedCell.Today = false;
                processedCell.OutOfRange = false;
                processedCell.SpecialDay = false;
                processedCell.Children.Clear();
                processedCell.Image = null;
                if (unconditional)
                {
                    processedCell.Selected = false;
                }
                else if (!(this.Calendar.SelectedDates.Contains(processedCell.Date) &&
                    this.View.IsDateInView(processedCell.Date)) && processedCell.Selected != false)
                {
                    processedCell.Selected = false;
                }
                processedCell.OtherMonth = false;
                processedCell.WeekEnd = false;

            }

            this.SetViewProperties();
        }

        internal virtual void SetViewProperties()
        {
            for (int i = 0; i < this.VisualElements.Count; i++)
            {
                this.VisualElements[i].Text = "" + i;
            }

            foreach (RadCalendarDay calendarDay in this.Calendar.SpecialDays)
            {
                calendarDay.IsTemplateSet = false;
            }

            ((MonthViewElement)this.Owner).TitleElement.Text = this.view.GetTitleContent();
            this.RenderContent();

            if (this.view.ShowRowHeaders ||
                this.view.ShowSelector ||
                this.view.ShowColumnHeaders)
            {
                this.InvalidateHeaders();
            }

            this.Calendar.Invalidate();
        }

        internal void InvalidateHeaders()
        {
            this.InvalidateHeaders(this.View.Orientation);
        }

        internal void InvalidateHeaders(Orientation orientation)
        {
            DateTime visibleDate = this.View.EffectiveVisibleDate();
            DateTime firstDay = (this.View as MonthView).FirstCalendarDay(visibleDate);

            // Boyko - 
            // Varnax proverkite dali sa aktivni headerite zashtoto ina4e 
            // edni i sashti kletki shte se renderirat po 2 pati 
            // t.e shte se renderirat tuk vupreki 4e ne sa header cells
            // posle shte se renderirat v renderContent

            if (this.View.ShowColumnHeaders)
            {
                this.InvalidateColumnHeaders(firstDay);
            }

            if (this.View.ShowRowHeaders)
            {
                this.InvalidateRowHeaders(firstDay);
            }

            this.InvalidateViewSelector();
        }

        internal void InvalidateViewSelector()
        {
            if (this.View.ShowSelector)
            {
                CalendarCellElement temp = this.GetElement(0, 0) as CalendarCellElement;
                this.SetHeaderCell(temp, HeaderType.View, this.View.ViewSelectorText, this.View.ViewSelectorImage);
            }
        }

        public virtual void Initialize()
        {
        }

        public virtual void Initialize(int rows, int columns)
        {
            int count = rows * columns;
            this.visualElements = new List<LightVisualElement>(count);

            int currentRow = 0;
            int currentColumn = 0;

            for (int i = 0; i < count; i++)
            {
                CalendarCellElement cell = new CalendarCellElement(this);
                if (this.View.ShowSelector)
                {
                    if (currentRow == 0 || currentColumn == 0)
                        cell.SetValue(CalendarCellElement.IsHeaderCellProperty, true);
                }
                else if (this.View.ShowRowHeaders)
                {
                    if (currentColumn == 0)
                    {
                        cell.SetValue(CalendarCellElement.IsHeaderCellProperty, true);
                    }
                }
                else
                {
                    if (this.View.ShowColumnHeaders)
                    {
                        if (currentRow == 0)
                        {
                            cell.SetValue(CalendarCellElement.IsHeaderCellProperty, true);
                        }
                    }
                }

                cell.Row = currentRow;
                cell.Column = currentColumn;

                currentColumn++;
                if (currentColumn == columns)
                {
                    currentColumn = 0;
                    currentRow++;
                }

                this.visualElements.Add(cell);
            }
        }

        public virtual void ResetVisuals()
        {
            this.view = this.View;
            this.visualElements.Clear();
            this.DisposeChildren();
            this.View.ReInitialize();
            this.CreateVisuals();
            this.InitializeChildren();
            this.ResetCellsProperties();
        }

        private void ResetCellsProperties()
        {
            // resets all current sets of cells properties 
            // and sets them to the calendar based properties.

            SetCellsProperty("ZoomFactor");
            SetCellsProperty("AllowFishEye");
            SetCellsProperty("ShowOtherMonthsDays");
            SetCellsProperty("CellMargin");
            SetCellsProperty("CellPadding");

            if (this.View.ShowSelector)
            {
                this.VisualElements[0].Text = this.Calendar.ViewSelectorText;
                this.VisualElements[0].Image = this.Calendar.ViewSelectorImage;
            }
        }

        internal protected virtual void RefreshSelectedDates()
        {
            //for (int i = 0; i < this.VisualElements.Count; i++)
            //{
            //    CalendarCellElement cell = this.VisualElements[i] as CalendarCellElement;

            //    if (!(bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty))
            //    {
            //        cell.Selected = false;

            //        if (this.Calendar.SelectedDates.Contains(cell.Date))
            //        {
            //            bool thisDateIsSelected = this.Calendar.SelectedDates.Contains(cell.Date);
            //            if (thisDateIsSelected && ShouldApplyStyle(cell.Date))
            //            {
            //                if (!cell.Selected)
            //                    cell.Selected = thisDateIsSelected;
            //            }
            //        }
            //    }
            //}

            for (int i = 0; i < this.VisualElements.Count; i++)
            {
                CalendarCellElement cell = this.VisualElements[i] as CalendarCellElement;

                if (!(bool)cell.GetValue(CalendarCellElement.IsHeaderCellProperty))
                {
                    cell.Selected = false;

                    if (this.Calendar.SelectedDates.Contains(cell.Date))
                    {
                        bool thisDateIsSelected = this.Calendar.SelectedDates.Contains(cell.Date);
                        if (thisDateIsSelected && ShouldApplyStyle(cell.Date))
                        {
                            if (!cell.Selected)
                                cell.Selected = thisDateIsSelected;
                        }
                    }
                }
            }
        }

        protected internal virtual void Recreate()
        {
        }

        internal protected virtual void RenderContent()
        {
            this.RenderContent(this.view);
        }

        internal protected virtual void RenderContent(CalendarView view)
        {
            DateTime dateToRender = view.ViewRenderStartDate /*firstDay*/;
            if (view.Orientation == Orientation.Horizontal)
            {
                for (int i = 0; i < this.Rows; i++)
                {
                    for (int j = 0; j < this.Columns; j++)
                    {
                        LightVisualElement tempCell = this.GetContentElement(i, j);

                        if (this.Calendar != null && this.Calendar.RTL)
                        {
                            tempCell = this.GetContentElement(i, this.Columns - 1 - j);
                        }

                        this.SetCalendarCell(tempCell, dateToRender);
                        TimeSpan span = new TimeSpan(1, 0, 0, 0);
                        if (DateTime.MaxValue.Ticks > (span.Ticks + dateToRender.Ticks))
                        {
                            dateToRender = this.Calendar.CurrentCalendar.AddDays(dateToRender, 1);
                        }
                        else
                        {
                            dateToRender = DateTime.MaxValue;
                        }
                    }
                }
            }
            else if (view.Orientation == Orientation.Vertical)
            {
                int addedDays = 0;
                for (int i = 0; i < this.Columns; i++)
                {
                    for (int j = 0; j < this.Rows; j++)
                    {
                        LightVisualElement tempCell = this.GetContentElement(j, i);

                        if (this.Calendar != null && this.Calendar.RTL)
                        {
                            tempCell = this.GetContentElement(this.Rows - 1 - j, i);
                        }

                        this.SetCalendarCell(tempCell, dateToRender);
                        addedDays += i + this.Rows;
                        dateToRender = this.Calendar.CurrentCalendar.AddDays(dateToRender, 1);
                    }
                }
            }
        }

        private void SetCellAlignment(LightVisualElement tempCell)
        {
            tempCell.TextAlignment = this.Calendar.CellAlign;
        }

        private bool ShouldApplyStyle(DateTime processedDate)
        {
            if (this.Calendar.ShowOtherMonthsDays ||
                (processedDate >= this.View.ViewStartDate &&
                processedDate <= this.View.ViewEndDate))
            {
                return true;
            }

            return false;
        }

        private void InvalidateColumnHeaders(DateTime firstDay)
        {
            if (!this.View.ShowColumnHeaders)
            {
                return;
            }

            int daycount = (int)this.Calendar.CurrentCalendar.GetDayOfWeek(firstDay);

            int iterateTo = this.Columns;

            for (int i = 0; i < iterateTo; i++)
            {
                if (this.View.Orientation == Orientation.Horizontal)
                {
                    string headerString = string.Empty;
                    Image headerImage = null;
                    int dayOfWeek = daycount % 7;
                    headerString = (this.View as MonthView).GetDayHeaderString(dayOfWeek);
                    if (!String.IsNullOrEmpty(this.Calendar.ColumnHeaderText))
                    {
                        headerString = this.Calendar.ColumnHeaderText;
                    }
                    if (this.Calendar.ColumnHeaderImage != null)
                    {
                        headerImage = this.Calendar.ColumnHeaderImage;
                    }

                    CalendarCellElement temp = (CalendarCellElement)this.GetElement(0, i + this.contentXShift);
                    SetCellAlignment(temp);

                    if (this.Calendar != null && this.Calendar.RTL)
                    {
                        int x = -1 + iterateTo - i + this.contentXShift;

                        temp = (CalendarCellElement)this.GetElement(0, x);
                    }

                    temp.ToolTipText = this.Calendar.DateTimeFormat.GetDayName((DayOfWeek)dayOfWeek);
                    this.SetHeaderCell(temp, HeaderType.Column, headerString, headerImage);

                    daycount += 1;
                }
                else
                {
                    CalendarCellElement temp = (CalendarCellElement)this.GetElement(0, i + this.contentXShift);

                    DayOfWeek convertedDay;
                    if (this.Calendar.FirstDayOfWeek == FirstDayOfWeek.Default)
                    {
                        convertedDay = this.Calendar.DateTimeFormat.FirstDayOfWeek;
                    }
                    else
                    {
                        convertedDay = (DayOfWeek)this.Calendar.FirstDayOfWeek;
                    }

                    int week = this.Calendar.CurrentCalendar.GetWeekOfYear(firstDay,
                        this.Calendar.DateTimeFormat.CalendarWeekRule, convertedDay);

                    int weekOffset = 1;

                    if (this.Calendar.MonthLayout == MonthLayout.Layout_14rows_x_3columns)
                        weekOffset = 2;

                    if (this.Calendar.MonthLayout == MonthLayout.Layout_21rows_x_2columns)
                        weekOffset = 3;

                    firstDay = this.Calendar.CurrentCalendar.AddWeeks(firstDay, weekOffset);

                    string headerString = string.Empty;
                    Image headerImage = null;
                    headerString = week.ToString();
                    if (!String.IsNullOrEmpty(this.Calendar.RowHeaderText))
                    {
                        headerString = this.Calendar.RowHeaderText;
                    }
                    if (this.Calendar.RowHeaderImage != null)
                    {
                        headerImage = this.Calendar.RowHeaderImage;
                    }

                    this.SetHeaderCell(temp, HeaderType.Column, headerString, headerImage);

                    temp.Visibility = ElementVisibility.Visible;
                    SetCellAlignment(temp);

                    int count = 0;

                    for (int j = 1; j < rows; j++)
                    {
                        CalendarCellElement contentCell = (CalendarCellElement)this.GetElement(j, i + this.contentXShift);
                        if (contentCell.Visibility != ElementVisibility.Visible)
                            count++;
                    }

                    if (count == this.Rows)
                    {
                        temp.Visibility = ElementVisibility.Collapsed;
                    }
                }
            }
        }

        private void InvalidateRowHeaders(DateTime firstDay)
        {
            if (!this.View.ShowRowHeaders)
            {
                return;
            }

            DayOfWeek convertedDay;
            if (this.Calendar.FirstDayOfWeek == FirstDayOfWeek.Default)
            {
                convertedDay = this.Calendar.DateTimeFormat.FirstDayOfWeek;
            }
            else
            {
                convertedDay = (DayOfWeek)this.Calendar.FirstDayOfWeek;
            }

            int iterateTo = this.Rows;

            int daycount = (int)this.Calendar.CurrentCalendar.GetDayOfWeek(firstDay);

            for (int i = 0; i < iterateTo; i++)
            {
                if (this.View.Orientation == Orientation.Horizontal)
                {
                    int week = this.Calendar.CurrentCalendar.GetWeekOfYear(firstDay,
                        this.Calendar.DateTimeFormat.CalendarWeekRule, convertedDay);

                    int weekOffset = 1;

                    if (this.Calendar.MonthLayout == MonthLayout.Layout_14columns_x_3rows)
                        weekOffset = 2;

                    if (this.Calendar.MonthLayout == MonthLayout.Layout_21columns_x_2rows)
                        weekOffset = 3;

                    firstDay = this.Calendar.CurrentCalendar.AddWeeks(firstDay, weekOffset);

                    string headerString = string.Empty;
                    Image headerImage = null;
                    headerString = week.ToString();
                    if (!String.IsNullOrEmpty(this.Calendar.RowHeaderText))
                    {
                        headerString = this.Calendar.RowHeaderText;
                    }
                    if (this.Calendar.RowHeaderImage != null)
                    {
                        headerImage = this.Calendar.RowHeaderImage;
                    }

                    int offset = this.contentYShift;

                    CalendarCellElement temp = (CalendarCellElement)this.GetElement(i + offset, 0);
                    SetCellAlignment(temp);

                    temp.ToolTipText = headerString;
                    this.SetHeaderCell(temp, HeaderType.Row, headerString, headerImage);
                    temp.Visibility = ElementVisibility.Visible;

                    int count = 0;

                    for (int j = 1; j < this.columns; j++)
                    {
                        CalendarCellElement contentCell = (CalendarCellElement)this.GetElement(i + offset, j);
                        if (contentCell.Visibility != ElementVisibility.Visible)
                            count++;
                    }

                    if (count == this.Columns)
                    {
                        temp.Visibility = ElementVisibility.Collapsed;
                    }
                }
                else
                {

                    string headerString = string.Empty;
                    Image headerImage = null;
                    int dayOfWeek = daycount % 7;
                    headerString = (this.View as MonthView).GetDayHeaderString(dayOfWeek);
                    if (!String.IsNullOrEmpty(this.Calendar.ColumnHeaderText))
                    {
                        headerString = this.Calendar.ColumnHeaderText;
                    }
                    if (this.Calendar.ColumnHeaderImage != null)
                    {
                        headerImage = this.Calendar.ColumnHeaderImage;
                    }

                    int offset = this.contentYShift;

                    CalendarCellElement temp = (CalendarCellElement)this.GetElement(i + offset, 0);

                    if (this.Calendar != null && this.Calendar.RTL)
                    {
                        int x = iterateTo - 1 - i + offset;

                        temp = (CalendarCellElement)this.GetElement(x, 0);
                    }

                    temp.ToolTipText = this.Calendar.DateTimeFormat.GetDayName((DayOfWeek)dayOfWeek);
                    SetCellAlignment(temp);

                    this.SetHeaderCell(temp, HeaderType.Column, headerString, headerImage);

                    daycount += 1;
                }
            }
        }

        internal void SetHeaderCell(CalendarCellElement cell, HeaderType type, string headerText, Image headerImage)
        {
            if (headerText != String.Empty &&
                (cell.Text != headerText))
            {
                cell.Text = headerText;
            }
            else if (headerText == String.Empty)
            {
                cell.Text = "x";
            }

            if (!cell.Date.Equals(this.View.ViewStartDate))
            {
                cell.isChecked = false;
            }

            cell.Date = this.View.ViewStartDate;
            cell.SetValue(CalendarCellElement.IsHeaderCellProperty, true);

            if (cell.Image != headerImage)
            {
                cell.Image = headerImage;
            }
        }

        internal protected virtual void SetCalendarCell(LightVisualElement processedCell, DateTime processedDate)
        {
            SetCellAlignment(processedCell);

            CultureInfo calendarCulture = this.Calendar.Culture;
            processedCell.Text = processedDate.ToString(this.Calendar.DayCellFormat, calendarCulture);
            ((CalendarCellElement)processedCell).Date = processedDate;
            bool thisDateIsSelected = this.Calendar.SelectedDates.Contains(processedDate);
            ((CalendarCellElement)processedCell).isChecked = thisDateIsSelected;

            //logic that enables the processing of weekend dates.
            int tempWeekDay = (int)this.Calendar.CurrentCalendar.GetDayOfWeek(processedDate);
            bool isWeekEnd = (tempWeekDay == 0) || (tempWeekDay == 6);
            bool isToday = (processedDate == DateTime.Today);
            bool isOutOfRange = (processedDate < this.Calendar.RangeMinDate || processedDate > this.Calendar.RangeMaxDate);
            //logic that enables the processing of other month dates.
            bool isOtherMonth = (processedDate < this.View.ViewStartDate) || (this.View.ViewEndDate < processedDate);

            // TO DO - focused expression
            bool isFocused = processedDate == Calendar.FocusedDate ? true : false;

            if (isOtherMonth)
                processedCell.Visibility = !this.Calendar.ShowOtherMonthsDays ? ElementVisibility.Hidden : ElementVisibility.Visible;
            else
                processedCell.Visibility = ElementVisibility.Visible;

            //set cell style
            if (thisDateIsSelected && ShouldApplyStyle(processedDate))
            {
                if (!((CalendarCellElement)processedCell).Selected)
                    ((CalendarCellElement)processedCell).Selected = thisDateIsSelected;
            }

            if (isFocused)
            {
                if (!((CalendarCellElement)processedCell).Focused)
                {
                    ((CalendarCellElement)processedCell).Focused = isFocused;
                }

                processedCell.SetFocus();
            }

            if (isToday)
            {
                if (!((CalendarCellElement)processedCell).Today)
                    ((CalendarCellElement)processedCell).Today = isToday;
            }

            if (!this.Calendar.Enabled)
            {
                if (!((CalendarCellElement)processedCell).Enabled)
                    ((CalendarCellElement)processedCell).Enabled = false;
            }

            if (isOutOfRange)
            {
                if (!((CalendarCellElement)processedCell).OutOfRange)
                    ((CalendarCellElement)processedCell).OutOfRange = isOutOfRange;
            }

            if (isOtherMonth)
            {
                if (!((CalendarCellElement)processedCell).OtherMonth)
                    ((CalendarCellElement)processedCell).OtherMonth = isOtherMonth;

                processedCell.Visibility = !this.Calendar.ShowOtherMonthsDays ? ElementVisibility.Hidden : ElementVisibility.Visible;
            }

            if (isWeekEnd)
            {
                if (!((CalendarCellElement)processedCell).WeekEnd)
                    ((CalendarCellElement)processedCell).WeekEnd = isWeekEnd;
            }
            // check for special settings
            else if (!isOutOfRange)
            {
                this.ProcessCalendarDays(processedDate);
            }

            RadCalendarDay specialDay = (this.View as MonthView).GetSpecialDay(processedDate);
            bool isTemplateDay = specialDay != null;

            if (isTemplateDay)
            {
                if (!((CalendarCellElement)processedCell).SpecialDay)
                {
                    ((CalendarCellElement)processedCell).SpecialDay = true;
                    processedCell.Image = specialDay.Image;

                    if (specialDay.TemplateItem != null)
                    {
                        if ((!isOtherMonth) && (!specialDay.IsTemplateSet))
                        {
                            specialDay.TemplateItem.UseNewLayoutSystem = true;
                            specialDay.IsTemplateSet = true;

                            processedCell.Children.Add(specialDay.TemplateItem);
                            processedCell.UpdateLayout();
                        }
                    }
                }
            }

            // rise the "last chance for update" event
            RadCalendarDay tempDay = new RadCalendarDay(processedDate, (this.Calendar != null) ? this.Calendar.SpecialDays : null);
            if (isToday)
            {
                tempDay.SetToday(true);
            }

            if (isWeekEnd)
            {
                tempDay.SetWeekend(true);
            }

            tempDay.Selectable = true;

            if (this.Calendar.SelectedDates.Contains(processedDate))
            {
                tempDay.Selected = true;
            }

            this.Calendar.CallOnElementRender(processedCell, tempDay, this.View);
        }

        internal virtual void ProcessCalendarDays(DateTime processedDate)
        {
        }

        internal protected virtual LightVisualElement GetContentElement(int row, int column)
        {
            return GetElement(row + this.contentYShift, column + this.contentXShift);
        }

        internal protected virtual LightVisualElement GetElement(int row, int column)
        {
            for (int i = 0; i < this.visualElements.Count; i++)
            {
                CalendarCellElement element = this.visualElements[i] as CalendarCellElement;

                if (element.Column == column &&
                    element.Row == row)
                {
                    return element;
                }
            }

            return null;
        }

        internal protected virtual List<CalendarCellElement> GetElementsByColumn(int column)
        {
            List<CalendarCellElement> elements = new List<CalendarCellElement>(this.Rows);
            for (int i = 0; i < this.visualElements.Count; i++)
            {
                CalendarCellElement element = this.visualElements[i] as CalendarCellElement;
                if (element.Column == column)
                {
                    elements.Add(element);
                }
            }

            return elements;
        }

        internal protected virtual List<CalendarCellElement> GetElementsByRow(int row)
        {
            List<CalendarCellElement> elements = new List<CalendarCellElement>(this.Columns);
            for (int i = 0; i < this.visualElements.Count; i++)
            {
                CalendarCellElement element = this.visualElements[i] as CalendarCellElement;
                if (element.Row == row)
                {
                    elements.Add(element);
                }
            }

            return elements;
        }

        internal protected virtual List<CalendarCellElement> GetViewElements()
        {
            List<CalendarCellElement> elements = new List<CalendarCellElement>(this.VisualElements.Count);
            for (int i = 0; i < this.VisualElements.Count; i++)
            {
                CalendarCellElement element = this.VisualElements[i] as CalendarCellElement;
                if (element != null &&
                    (element.Date != DateTime.MinValue))
                {
                    elements.Add(element);
                }
            }

            return elements;
        } 
        #endregion

        #region Enums
        /// <summary>Specifies the type of a selector sell.</summary>
        internal protected enum HeaderType
        {
            /// <summary>
            /// Rendered as the first cell in a row. When clicked if UseRowHeadersAsSelectors is true, 
            /// it will select the entire row.
            /// </summary>
            Row,
            /// <summary>
            /// Rendered as the first cell in a column. When clicked if UseColumnHeadersAsSelectors is true, 
            /// it will select the entire column.
            /// </summary>
            Column,
            /// <summary>
            /// Rendered in the top left corner of the calendar view. When clicked if EnableViewSelector is true, 
            /// it will select the entire view.
            /// </summary>
            View
        } 
        #endregion
    }
}
