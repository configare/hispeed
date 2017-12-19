using System.Collections.Generic;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	public class CalendarMultiMonthViewTableElement : CalendarTableElement
	{
        #region Fields
        private int horizontalCellSpacing = 5;
        private int verticalCellSpacing = 5; 
        #endregion

        #region Constructors
        internal protected CalendarMultiMonthViewTableElement(CalendarVisualElement owner, RadCalendar calendar, CalendarView view)
            : base(owner, calendar, view)
        {
            this.view = view;
            this.view.ShowColumnHeaders = false;
            this.view.ShowRowHeaders = false;
            this.view.ShowSelector = false;
        }

        internal protected CalendarMultiMonthViewTableElement(CalendarVisualElement owner)
            : this(owner, null, null)
        {
        } 
        #endregion

        #region Properties
        public override int Columns
        {
            get
            {
                return this.View.MultiViewColumns;
            }
        }

        public override int Rows
        {
            get
            {
                return this.View.MultiViewRows;
            }
        }

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
                    this.view = value;
                    this.ResetVisuals();
                    this.view.ShowColumnHeaders = false;
                    this.view.ShowRowHeaders = false;
                    this.view.ShowSelector = false;
                }
            }
        }
        #endregion

        #region Methods
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AllowFadeAndAnimation = false;
        }

        protected override void ArrangeHeaders(int xCellSize, int yCellSize, int xOffset, int yOffset)
        {
        }

        protected internal override void CreateVisuals()
        {
            this.Initialize(this.Rows, this.Columns);
        }

        internal protected override void RefreshVisuals(bool unconditional)
        {
            foreach (MonthViewElement monthElement in VisualElements)
            {
                monthElement.RefreshVisuals(unconditional);
            }
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (this.View == null)
                return finalSize;

            if (this.visualElements.Count == 0)
                return finalSize;

            if (this.Columns != 0 && this.Rows != 0)
            {
                int xCellSize;
                if (this.StretchHorizontally)
                {
                    xCellSize = (int)(finalSize.Width - (horizontalCellSpacing * this.Columns)) / this.Columns;
                }
                else
                {
                    xCellSize = (int)this.visualElements[0].DesiredSize.Width;
                }

                int yCellSize;
                if (this.StretchHorizontally)
                {
                    yCellSize = ((int)finalSize.Height - (verticalCellSpacing * this.Rows)) / this.Rows;
                }
                else
                {
                    yCellSize = (int)this.visualElements[0].DesiredSize.Height;
                }

                int xOffset = 0;
                int yOffset = 0;

                ArrangeContentCells(xCellSize, yCellSize, xOffset, yOffset);
            }

            return finalSize;
        }

        protected override void ArrangeContentCells(int xCellSize, int yCellSize, int xOffset, int yOffset)
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

                    cell = this.GetElement(i, j);

                    if (cell == null)
                        continue;

                    position.X += j > 0 ? xCellSize : xOffset;
                    position.X += this.horizontalCellSpacing;

                    cell.Arrange(new RectangleF(position, new SizeF(xCellSize, yCellSize)));
                }
            }
        }

        protected internal override LightVisualElement GetElement(int row, int column)
        {
            for (int i = 0; i < this.visualElements.Count; i++)
            {
                MonthViewElement element = this.visualElements[i] as MonthViewElement;

                if (element.Column == column &&
                    element.Row == row)
                {
                    return element;
                }

            }

            return null;
        }

        protected internal override void Recreate()
        {
            this.SuspendLayout();
            this.VisualElements.Clear();
            this.view = this.View;
            if (this.Calendar != null)
            {
                this.Initialize(this.Calendar.MultiViewRows, this.Calendar.MultiViewColumns);
                this.InitializeChildren();
            }

            this.ResumeLayout(true);
        }

        public override void ResetVisuals()
        {
            this.view = this.View;
            int i = 0;

            foreach (MonthViewElement monthView in this.Children)
            {
                monthView.TableElement.View = this.view.Children[i];
                monthView.TitleElement.View = this.view.Children[i];
                i++;
            }
        }

        protected override void InitializeChildren()
        {
            this.DisposeChildren();
            for (int i = 0; i < this.VisualElements.Count; ++i)
            {
                this.Children.Add(visualElements[i]);
            }
        }

        protected override void SetBehaviorOnPropertyChange(string propertyName)
        {
        }

        public override void Initialize(int rows, int columns)
        {
            this.visualElements = new List<LightVisualElement>(this.view.Children.Count);
            this.visualElements.Clear();

            int currentRow = 0;
            int currentColumn = 0;

            for (int i = 0; i < this.view.Children.Count; i++)
            {
                CalendarView monthView = this.view.Children[i];
                MonthViewElement monthViewElement = new MonthViewElement(this.Calendar, monthView);

                monthView.ShowHeader = true;
                monthViewElement.TitleElement.Visibility = ElementVisibility.Visible;

                monthViewElement.Row = currentRow;
                monthViewElement.Column = currentColumn;


                currentColumn++;
                if (currentColumn == columns)
                {
                    currentColumn = 0;
                    currentRow++;
                }

                this.visualElements.Add(monthViewElement);
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            return availableSize;
        } 
        #endregion
	}
}
