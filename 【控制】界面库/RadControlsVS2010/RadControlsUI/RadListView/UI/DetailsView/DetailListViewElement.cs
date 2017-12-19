using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Themes;

namespace Telerik.WinControls.UI
{
    public class DetailListViewElement : BaseListViewElement
    {
        #region RadProperties

        public static RadProperty ColumnDragHintProperty = RadProperty.Register(
             "ColumnDragHint", typeof(RadImageShape), typeof(DetailListViewElement), new RadElementPropertyMetadata(
                 null, ElementPropertyOptions.None));

        #endregion

        #region Fields

        private DetailListViewColumnContainer columnContainer;
        private DetailListViewColumnScroller columnScroller;
        private DetailListViewDragDropService columnDragDropService;
        private RadScrollBarElement columnsScrollBar;

        private float cornerCellWidth = 0; 

        #endregion

        #region Ctor

        public DetailListViewElement(RadListViewElement owner)
            : base(owner)
        {
            this.columnScroller.Traverser = new ListViewColumnTraverser(this.owner.Columns);
            this.columnContainer.DataProvider = this.columnScroller;
            this.columnScroller.ScrollerUpdated += new EventHandler(columnScroller_ScrollerUpdated);

            this.columnDragDropService = new DetailListViewDragDropService(this);

            this.scrollBehavior.ScrollServices.Add(new ScrollService(this.ViewElement, this.columnsScrollBar));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="RadImageShape">RadImageShape</see> instance which describes the hint that indicates where a column will be dropped after a drag operation.
        /// </summary>
        [Browsable(false)]
        [VsbBrowsable(true)]
        public RadImageShape ColumnDragHint
        {
            get
            {
                return (RadImageShape)this.GetValue(ColumnDragHintProperty);
            }
            set
            {
                this.SetValue(ColumnDragHintProperty, value);
            }
        }

        public DetailListViewColumnContainer ColumnContainer
        {
            get
            {
                return this.columnContainer;
            }
        }

        public DetailListViewDragDropService ColumnDragDropService
        {
            get
            {
                return columnDragDropService;
            }
        }

        public DetailListViewColumnScroller ColumnScroller
        {
            get
            {
                return columnScroller;
            }
        }

        public RadScrollBarElement ColumnScrollBar
        {
            get { return this.columnsScrollBar; }
        }

        #endregion

        #region Overrides

        protected override VirtualizedStackContainer<ListViewDataItem> CreateViewElement()
        {
            return new DetailsListViewContainer(this);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.columnContainer = new DetailListViewColumnContainer(this);
            this.columnContainer.StretchHorizontally = true;
            this.columnContainer.StretchVertically = true;
            this.columnContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.columnContainer.ElementProvider = new DetailListViewHeaderCellElementProvider();

            this.Children.Add(this.columnContainer);

            this.columnsScrollBar = new RadScrollBarElement();
            this.columnsScrollBar.ScrollType = ScrollType.Horizontal;
            this.columnsScrollBar.MinSize = new Size(0, SystemInformation.HorizontalScrollBarHeight);
            this.columnsScrollBar.ScrollTimerDelay = 1;
            this.Children.Add(columnsScrollBar);

            this.columnScroller = new DetailListViewColumnScroller();
            this.columnScroller.ScrollMode = ItemScrollerScrollModes.Smooth;
            this.columnScroller.ElementProvider = this.columnContainer.ElementProvider;
            this.columnScroller.Scrollbar = this.columnsScrollBar;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            RectangleF clientRect = this.GetClientRectangle(availableSize);

            this.MeasureChildElements(clientRect);
             
            ElementVisibility oldVScrollVisibility = this.VScrollBar.Visibility;

            this.ViewElement.Measure(new SizeF(clientRect.Width - this.VScrollBar.DesiredSize.Width,
                availableSize.Height - this.columnsScrollBar.DesiredSize.Height - this.columnContainer.DesiredSize.Height));

            if (this.UpdateOnMeasure(availableSize) || oldVScrollVisibility != this.VScrollBar.Visibility)
            {
                this.MeasureChildElements(clientRect);

                this.ViewElement.Measure(new SizeF(clientRect.Width - this.VScrollBar.DesiredSize.Width,
                      availableSize.Height - this.columnsScrollBar.DesiredSize.Height - this.columnContainer.DesiredSize.Height));
            }

            CalculateCornerCellWidth();

            if (cornerCellWidth > 0 && this.owner.ShowColumnHeaders)
            {
                this.columnContainer.Measure(new SizeF(clientRect.Width - this.VScrollBar.DesiredSize.Width - cornerCellWidth, clientRect.Height));
            }

            SizeF measuredSize = this.ViewElement.DesiredSize;

            measuredSize.Height += this.columnContainer.DesiredSize.Height + this.columnsScrollBar.DesiredSize.Height;
            measuredSize.Width += this.VScrollBar.DesiredSize.Width;

            return measuredSize;
        }
  
        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = this.GetClientRectangle(finalSize);

            if (this.columnContainer.Visibility != ElementVisibility.Collapsed)
            {
                this.columnContainer.Arrange(new System.Drawing.RectangleF(clientRect.X + cornerCellWidth, clientRect.Y,
                    clientRect.Width - this.VScrollBar.DesiredSize.Width - cornerCellWidth, this.columnContainer.DesiredSize.Height));
            }

            RectangleF viewElementRect = new RectangleF(clientRect.X, clientRect.Y + this.columnContainer.DesiredSize.Height,
                clientRect.Width - VScrollBar.DesiredSize.Width, clientRect.Height - this.columnContainer.DesiredSize.Height);

            this.layoutManagerPart.Arrange(clientRect);
            RectangleF hscrollBarRect = ArrangeHScrollBar(ref viewElementRect, clientRect);

            ArrangeVScrollBar(ref viewElementRect, hscrollBarRect, clientRect);

            viewElementRect.Width = Math.Max(1, viewElementRect.Width);
            viewElementRect.Height = Math.Max(1, viewElementRect.Height);

            ViewElement.Arrange(viewElementRect);

            return finalSize;
        }

        protected override RectangleF ArrangeHScrollBar(ref RectangleF viewElementRect, RectangleF clientRect)
        {
            RectangleF hscrollBarRect = RectangleF.Empty;

            if (this.columnsScrollBar.Visibility != ElementVisibility.Collapsed)
            {
                int scrollbarHeight = (int)this.columnsScrollBar.DesiredSize.Height;
                if (scrollbarHeight == 0)
                {
                    scrollbarHeight = SystemInformation.HorizontalScrollBarHeight;
                }
                float y = clientRect.Bottom - scrollbarHeight;
                float width = clientRect.Width - this.VScrollBar.DesiredSize.Width;
                hscrollBarRect = new RectangleF(clientRect.X, y, width, scrollbarHeight);
                if (RightToLeft && this.columnsScrollBar.Visibility != ElementVisibility.Collapsed)
                {
                    hscrollBarRect.X += this.columnsScrollBar.DesiredSize.Width;
                }
                this.columnsScrollBar.Arrange(hscrollBarRect);
                viewElementRect.Height -= this.columnsScrollBar.DesiredSize.Height;
            }

            return hscrollBarRect;
        }

        protected override bool UpdateOnMeasure(SizeF availableSize)
        {
            RectangleF clientRect = GetClientRectangle(availableSize);

            RadScrollBarElement hscrollbar = this.columnsScrollBar;
            RadScrollBarElement vscrollbar = this.VScrollBar;

            ElementVisibility hVisibility = hscrollbar.Visibility;
            ElementVisibility vVisibility = vscrollbar.Visibility;

            SizeF clientSize = clientRect.Size;
            clientSize.Height -= this.columnContainer.DesiredSize.Height;

            if (hscrollbar.Visibility == ElementVisibility.Visible)
            {
                clientSize.Height -= columnsScrollBar.DesiredSize.Height;
            }

            this.Scroller.ClientSize = clientSize;
            this.columnScroller.ClientSize = clientSize;

            this.columnScroller.UpdateScrollRange();
            this.Scroller.UpdateScrollRange();

            if (vscrollbar.Visibility == ElementVisibility.Visible)
            {
                clientSize.Width -= vscrollbar.DesiredSize.Width;
            }

            this.Scroller.ClientSize = clientSize;
            this.columnScroller.ClientSize = clientSize;

            this.columnScroller.UpdateScrollRange();
            this.Scroller.UpdateScrollRange();

            return hVisibility != hscrollbar.Visibility || vVisibility != columnsScrollBar.Visibility;
        }

        public override BaseListViewVisualItem GetVisualItemAt(Point location)
        {
            RadElement element = this.ElementTree.GetElementAtPoint(location);

            while (element != null && !(element is BaseListViewVisualItem))
            {
                element = element.Parent;
            }

            return element as BaseListViewVisualItem;
        }

        protected override void HandleLeftKey(System.Windows.Forms.KeyEventArgs e)
        {
            int columnIndex = this.owner.Columns.IndexOf(this.owner.CurrentColumn);
            ITraverser<ListViewDetailColumn> enumerator = (ITraverser<ListViewDetailColumn>)this.columnScroller.Traverser.GetEnumerator();
            enumerator.Position = columnIndex;
            if (enumerator.MovePrevious())
            {
                this.owner.CurrentColumn = enumerator.Current;
            }
        }

        protected override void HandleRightKey(System.Windows.Forms.KeyEventArgs e)
        {
            int columnIndex = this.owner.Columns.IndexOf(this.owner.CurrentColumn);

            ITraverser<ListViewDetailColumn> enumerator = (ITraverser<ListViewDetailColumn>)this.columnScroller.Traverser.GetEnumerator();
            enumerator.Position = columnIndex;

            if (enumerator.MoveNext())
            {
                this.owner.CurrentColumn = enumerator.Current;
            }
        }

        internal override bool ProcessMouseMove(MouseEventArgs e)
        {
            bool result = base.ProcessMouseMove(e);

            this.owner.ColumnResizingBehavior.HandleMouseMove(e.Location);

            DetailListViewHeaderCellElement headerCell = this.ElementTree.GetElementAtPoint(e.Location) as DetailListViewHeaderCellElement;

            if (this.owner.ColumnResizingBehavior.IsResizing)
            {
                return result;
            }

            if (headerCell != null && headerCell.IsInResizeLocation(e.Location))
            {
                this.ElementTree.Control.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.ElementTree.Control.Cursor = Cursors.Default;
            }

            return result;
        }

        internal override bool ProcessMouseUp(MouseEventArgs e)
        {
            if (this.owner.EnableKineticScrolling && this.ViewElement.ContainsMouse)
            {
                this.scrollBehavior.MouseUp(e.Location);
            }
            else
            {
                this.scrollBehavior.Stop();
            }

            this.owner.ColumnResizingBehavior.EndResize();

            if (e.Button == MouseButtons.Left)
            {
                ListViewDataItem itemUnderMouse = this.GetItemAt(e.Location);
                DetailListViewCellElement cellUnderMouse = this.ElementTree.GetElementAtPoint(e.Location) as DetailListViewCellElement;

                if (itemUnderMouse == null || !itemUnderMouse.Enabled)
                {
                    this.lastModifierKeys = Keys.None;
                    this.groupSelectionTimer.Stop();
                    this.beginEditTimer.Stop();
                    this.lastClickedItem = null;
                    return false;
                }

                if (itemUnderMouse is ListViewDataItemGroup)
                {
                    if (itemUnderMouse != null && !this.disableGroupSelectOnMouseUp)
                    {
                        this.lastClickedItem = itemUnderMouse;
                        this.lastModifierKeys = Control.ModifierKeys;
                        this.groupSelectionTimer.Start();
                    }
                    else
                    {
                        this.lastClickedItem = null;
                        this.lastModifierKeys = Keys.None;
                        this.groupSelectionTimer.Stop();
                    }

                    return false;
                }

                this.lastClickedItem = null;
                this.lastModifierKeys = Keys.None;
                this.groupSelectionTimer.Stop();

                if (itemUnderMouse != null && !disableEditOnMouseUp &&
                    itemUnderMouse == this.owner.SelectedItem &&
                    Control.ModifierKeys == Keys.None &&
                    this.lastClickedItem == null &&
                    cellUnderMouse != null &&
                    cellUnderMouse.Data == this.owner.CurrentColumn)
                {
                    this.lastClickedItem = itemUnderMouse;
                    this.beginEditTimer.Start();
                }
                else
                {
                    this.beginEditTimer.Stop();
                    this.lastClickedItem = null;

                    if (Control.ModifierKeys != Keys.None || itemUnderMouse != this.owner.SelectedItem)
                    {
                        this.ProcessSelection(itemUnderMouse, Control.ModifierKeys, true);
                    }

                    if (cellUnderMouse != null)
                    {
                        cellUnderMouse.Data.Current = true;
                    }
                }
            }
            else
            {
                this.lastModifierKeys = Keys.None;
                this.groupSelectionTimer.Stop();
                this.beginEditTimer.Stop();
                this.lastClickedItem = null;
            }

            return false;
        }

        #endregion

        #region Methods

        public virtual void EnsureColumnVisible(ListViewDetailColumn column)
        {
            DetailListViewHeaderCellElement cell = GetHeaderCell(column);
             
            if (cell == null)
            {
                this.columnContainer.UpdateLayout();

                if (this.columnContainer.Children.Count > 0)
                {

                    int columnIndex = this.owner.Columns.IndexOf(column);
                    int firstVisibleIndex = this.owner.Columns.IndexOf(
                        ((DetailListViewHeaderCellElement)this.columnContainer.Children[0]).Data);

                    if (columnIndex <= firstVisibleIndex)
                    {
                        this.columnScroller.ScrollToItem(column);
                    }
                    else
                    {
                        EnsureColumnVisibleCore(column);
                    }
                }
            }
            else
            {
                if (cell.ControlBoundingRectangle.Width > this.columnContainer.ControlBoundingRectangle.Width)
                {
                    int offset = this.columnContainer.ControlBoundingRectangle.Left - cell.ControlBoundingRectangle.Left;
                    this.SetScrollValue(this.ColumnScrollBar, this.ColumnScrollBar.Value - offset);
                }
                else if (cell.ControlBoundingRectangle.Right > this.columnContainer.ControlBoundingRectangle.Right)
                {
                    int offset = cell.ControlBoundingRectangle.Right - this.columnContainer.ControlBoundingRectangle.Right;
                    this.SetScrollValue(this.ColumnScrollBar, this.ColumnScrollBar.Value + offset);
                }
                else if (cell.ControlBoundingRectangle.Left < this.columnContainer.ControlBoundingRectangle.Left)
                {
                    int offset = this.columnContainer.ControlBoundingRectangle.Left - cell.ControlBoundingRectangle.Left;
                    this.SetScrollValue(this.ColumnScrollBar, this.ColumnScrollBar.Value - offset);
                }
            }

            this.ViewElement.InvalidateMeasure();
            this.UpdateLayout();
        }
          
        private DetailListViewHeaderCellElement GetHeaderCell(ListViewDetailColumn column)
        {
            foreach (DetailListViewHeaderCellElement cell in this.columnContainer.Children)
            {
                if (cell.Data == column)
                {
                    return cell;
                }
            }

            return null;
        }

        protected virtual void EnsureColumnVisibleCore(ListViewDetailColumn column)
        {
            bool start = false;
            int offset = 0;
            ListViewDetailColumn lastVisible = 
                ((DetailListViewHeaderCellElement)this.columnContainer.Children[this.columnContainer.Children.Count - 1]).Data;
            ItemsTraverser<ListViewDetailColumn> traverser = 
                (ItemsTraverser<ListViewDetailColumn>)this.ColumnScroller.Traverser.GetEnumerator();
            DetailListViewHeaderCellElement cell = null;

            while (traverser.MoveNext())
            {
                if (traverser.Current == column)
                {
                    this.SetScrollValue(this.ColumnScrollBar, this.ColumnScrollBar.Value + offset);
                    this.UpdateLayout();
                    cell = this.GetHeaderCell(column);

                    if (cell != null &&
                        cell.ControlBoundingRectangle.Right > this.columnContainer.ControlBoundingRectangle.Right)
                    {
                        this.EnsureColumnVisible(column);
                    }
                    break;
                }
                if (traverser.Current == lastVisible)
                {
                    start = true;
                }
                if (start)
                {
                    offset += (int)columnContainer.ElementProvider.GetElementSize(traverser.Current).Width + columnContainer.ItemSpacing;
                }
            }
        }
         
        private void CalculateCornerCellWidth()
        {
            cornerCellWidth = 0;

            foreach (RadElement element in this.ViewElement.Children)
            {
                DetailListViewVisualItem visualItem = (element as DetailListViewVisualItem);

                if (visualItem != null)
                {
                    cornerCellWidth = Math.Max(visualItem.ToggleElement.DesiredSize.Width, cornerCellWidth);
                }
            }
        }


        private void MeasureChildElements(RectangleF clientRect)
        {
            if (this.columnsScrollBar.Visibility != ElementVisibility.Collapsed)
            {
                this.columnsScrollBar.Measure(clientRect.Size);
            }

            if (this.VScrollBar.Visibility != ElementVisibility.Collapsed)
            {
                this.VScrollBar.Measure(clientRect.Size);
            }

            if (this.owner.ShowColumnHeaders)
            {
                this.columnContainer.Visibility = ElementVisibility.Visible;
                this.columnContainer.Measure(new SizeF(clientRect.Width - this.VScrollBar.DesiredSize.Width, clientRect.Height));
            }
            else
            {
                this.columnContainer.Visibility = ElementVisibility.Collapsed;
            }
        }
         
        #endregion

        #region Event Handlers

        void columnScroller_ScrollerUpdated(object sender, EventArgs e)
        {
            this.owner.EndEdit();
            this.columnContainer.InvalidateMeasure();
            this.ViewElement.InvalidateArrange();
        }

        #endregion
    }
}
