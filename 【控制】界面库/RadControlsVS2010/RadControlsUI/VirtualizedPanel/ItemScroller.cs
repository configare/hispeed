using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public enum ItemScrollerScrollModes { Discrete, Smooth, Deferred };

    public class ItemScroller<T> : IEnumerable, IDisposable
    {
        #region Fields

        protected ITraverser<T> traverser;
        protected RadScrollBarElement scrollbar;
        private IVirtualizedElementProvider<T> elementProvider;
        private ItemScrollerScrollModes scrollMode;
        private ToolTip toolTip = null;
        private SizeF clientSize;
        private int itemSpacing = 0;
        private int itemHeight = 20;
        protected int maxItemWidth = -1;
        protected int currentItemWidth = 0;
        protected int scrollOffset;
        private bool scrollbarChanged;
        private ScrollState scrollState;
        private int oldScrollValue = -1;
        private int thumbDelta;
        private Timer thumbTimer;
        private bool suspendScrollbarValueChanged;
        private bool suspendScrollerUpdated = false;
        private bool disposed = false;
        private bool isToolTipVisible = false;
        private bool asynchronousScrolling = true;

        #endregion

        #region Properties

        public int MaxItemWidth
        {
            get
            {
                return this.maxItemWidth;
            }
        }

        public ScrollState ScrollState
        {
            get { return scrollState; }
            set
            {
                if (scrollState != value)
                {
                    scrollState = value;
                    SetScrollBarVisibility();
                }
            }
        }

        public ITraverser<T> Traverser
        {
            get
            {
                return this.traverser;
            }
            set
            {
                if (this.traverser != value)
                {
                    this.traverser = value;
                }
            }
        }

        public RadScrollBarElement Scrollbar
        {
            get
            {
                return this.scrollbar;
            }
            set
            {
                if (this.scrollbar != value)
                {
                    if (this.thumbTimer == null)
                    {
                        this.thumbTimer = new Timer();
                        this.thumbTimer.Interval = 10;
                        this.thumbTimer.Tick += new EventHandler(thumbTimer_Tick);
                    }
                    if (this.scrollbar != null)
                    {
                        this.scrollbar.Scroll -= new ScrollEventHandler(scrollbar_Scroll);
                        this.scrollbar.ValueChanged -= new EventHandler(scrollbar_ValueChanged);
                    }
                    this.scrollbar = value;
                    if (this.scrollbar != null)
                    {
                        this.scrollbar.Scroll += new ScrollEventHandler(scrollbar_Scroll);
                        this.scrollbar.ValueChanged += new EventHandler(scrollbar_ValueChanged);
                    }
                }
            }
        }

        public IVirtualizedElementProvider<T> ElementProvider
        {
            get
            {
                return this.elementProvider;
            }
            set
            {
                if (this.elementProvider != value)
                {
                    this.elementProvider = value;
                }
            }
        }

        public ItemScrollerScrollModes ScrollMode
        {
            get
            {
                return this.scrollMode;
            }
            set
            {
                if (this.scrollMode != value)
                {
                    this.scrollMode = value;

                    if (value != ItemScrollerScrollModes.Deferred)
                    {
                        this.DisposeToolTip();
                    }
                    UpdateScrollRange();
                }
            }
        }

        public SizeF ClientSize
        {
            get
            {
                return this.clientSize;
            }
            set
            {
                if (this.clientSize != value)
                {
                    this.clientSize = value;
                    UpdateScrollStep();
                }
            }
        }

        public int ItemHeight
        {
            get
            {
                return this.itemHeight;
            }
            set
            {
                if (this.itemHeight != value)
                {
                    this.itemHeight = value;
                    UpdateScrollStep();
                }
            }
        }

        public int ItemSpacing
        {
            get
            {
                return this.itemSpacing;
            }
            set
            {
                if (this.itemSpacing != value)
                {
                    this.itemSpacing = value;
                    UpdateScrollRange();
                }
            }
        }

        public int ScrollOffset
        {
            get
            {
                return this.scrollOffset;
            }
            set
            {
                if (scrollOffset != value)
                {
                    this.scrollOffset = value;
                    this.OnScrollerUpdated(EventArgs.Empty);
                }
            }
        }

        public object Position
        {
            get
            {
                return this.traverser.Position;
            }
        }

        public ToolTip ToolTip
        {
            get
            {
                if (this.isToolTipVisible)
                {
                    return this.toolTip;
                }

                return null;
            }
            protected set
            {
                this.toolTip = value;
            }
        }

        public bool AsynchronousScrolling
        {
            get
            {
                return this.asynchronousScrolling;
            }
            set
            {
                if (this.asynchronousScrolling != value)
                {
                    this.asynchronousScrolling = value;
                    if (this.thumbTimer != null)
                    {
                        this.thumbTimer.Enabled = value;
                    }
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler ScrollerUpdated;
        public event ToolTipTextNeededEventHandler ToolTipTextNeeded;

        #endregion

        #region Event handlers

        protected virtual void OnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            ToolTipTextNeededEventHandler handler = ToolTipTextNeeded;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void OnScrollerUpdated(EventArgs e)
        {
            EventHandler handler = this.ScrollerUpdated;

            if (handler != null && !suspendScrollerUpdated)
            {
                handler(this, e);
            }
        }

        private void scrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            scrollbarChanged = this.UpdateOnScroll(e);
        }

        private void scrollbar_ValueChanged(object sender, EventArgs e)
        {
            if (this.suspendScrollbarValueChanged)
            {
                return;
            }
            if (!this.scrollbarChanged)
            {
                if (oldScrollValue == -1)
                {
                    ScrollTo(this.scrollbar.Value);
                }
                else if (oldScrollValue < this.scrollbar.Value)
                {
                    ScrollDown(this.scrollbar.Value - oldScrollValue);
                }
                else
                {
                    if (this.scrollbar.Value == this.scrollbar.Minimum)
                    {
                        ScrollToBegin();
                    }
                    else
                    {
                        ScrollUp(oldScrollValue - this.scrollbar.Value);
                    }
                }
            }

            this.OnScrollerUpdated(EventArgs.Empty);

            this.scrollbarChanged = false;
            this.oldScrollValue = this.scrollbar.Value;
        }

        private void thumbTimer_Tick(object sender, EventArgs e)
        {
            if (thumbDelta != 0)
            {
                if (thumbDelta > 0)
                {
                    this.ScrollDown(thumbDelta);
                }
                else
                {
                    this.ScrollUp(-thumbDelta);
                }

                thumbDelta = 0;
                thumbTimer.Stop();

                if (this.isToolTipVisible)
                {
                    this.ShowToolTip();
                }

                this.OnScrollerUpdated(EventArgs.Empty);
                this.oldScrollValue = this.scrollbar.Value;
                this.suspendScrollbarValueChanged = false;
                this.suspendScrollerUpdated = false;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.traverser != null)
            {
                return this.traverser.GetEnumerator();
            }
            return null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed || !disposing)
            {
                return;
            }

            if (this.scrollbar != null)
            {
                this.scrollbar.Scroll -= new ScrollEventHandler(scrollbar_Scroll);
                this.scrollbar.ValueChanged -= new EventHandler(scrollbar_ValueChanged);
                this.scrollbar = null;
            }
            if (this.thumbTimer != null)
            {
                this.thumbTimer.Dispose();
                this.thumbTimer = null;
            }

            this.DisposeToolTip();
            this.disposed = true;
        }

        private void DisposeToolTip()
        {
            if (this.toolTip != null)
            {
                this.toolTip.Dispose();
                this.toolTip = null;
            }
        }

        #endregion

        #region Synchronize with scrollbar

        public virtual bool ScrollToItem(T item)
        {
            if (scrollbar == null || scrollbar.Visibility == ElementVisibility.Collapsed)
            {
                return false;
            }

            if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.ScrollMode == ItemScrollerScrollModes.Deferred)
            {
                int maximum = this.scrollbar.Maximum - this.scrollbar.LargeChange + 1;
                if (maximum <= this.scrollbar.Minimum)
                {
                    return false;
                }

                if (item == null)
                {
                    this.scrollbar.Value = this.scrollbar.Minimum;
                    return true;
                }

                object tempPos = this.traverser.Position;
                int offset = 0;

                this.traverser.Reset();
                while (this.traverser.MoveNext())
                {
                    if (item.Equals(this.traverser.Current))
                    {
                        this.traverser.MovePrevious();
                        this.scrollOffset = 0;
                        this.scrollbarChanged = true;
                        if (this.scrollbar.Value <= this.scrollbar.Maximum && offset < this.scrollbar.Maximum)
                        {
                            this.scrollbar.Value = offset;
                        }
                        return true;
                    }
                    int itemHeight = this.GetScrollHeight(this.traverser.Current) + itemSpacing;
                    if (offset + itemHeight >= maximum)
                    {
                        this.traverser.MovePrevious();
                        this.scrollOffset = maximum - offset;
                        this.scrollbarChanged = true;
                        this.scrollbar.Value = maximum;
                        return true;
                    }
                    offset += itemHeight;
                }

                this.traverser.Position = tempPos;
                return false;
            }
            else
            {
                int newValue = 0;
                this.traverser.Reset();
                while (this.traverser.MoveNext())
                {
                    if (item.Equals(traverser.Current))
                    {
                        if (newValue < this.scrollbar.Maximum - this.scrollbar.LargeChange + 1)
                        {
                            this.scrollbar.Value = newValue;
                        }
                        else if (this.Scrollbar.Minimum < this.scrollbar.Maximum - this.scrollbar.LargeChange + 1)
                        {
                            this.scrollbar.Value = this.scrollbar.Maximum - this.scrollbar.LargeChange + 1;
                        }
                        else
                        {
                            return false;
                        }
                        return true;
                    }
                    newValue++;
                }
            }

            return false;
        }

        protected virtual bool ScrollToBegin()
        {
            this.scrollOffset = 0;
            this.traverser.Reset();
            return true;
        }

        protected virtual bool ScrollToEnd()
        {
            return false;
        }

        protected virtual bool ScrollTo(int position)
        {
            this.ScrollToBegin();
            return this.ScrollDown(position);
        }

        protected virtual bool ScrollDown(int step)
        {
            if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.scrollMode == ItemScrollerScrollModes.Deferred)
            {
                object tempPos = this.traverser.Position;

                while (step > 0 && this.traverser.MoveNext())
                {
                    int height = this.GetScrollHeight(this.traverser.Current) + itemSpacing;
                    int diff = height - this.scrollOffset;

                    if (diff > step)
                    {
                        this.scrollOffset += step;
                        break;
                    }
                    else
                    {
                        step -= diff;
                        this.scrollOffset = 0;
                        tempPos = this.traverser.Position;
                    }
                }

                this.traverser.Position = tempPos;

                return true;
            }

            while (step-- > 0)
            {
                this.traverser.MoveNext();
            }

            return true;
        }

        public virtual int GetScrollHeight(T item)
        {
            int height = 0;
            if (this.ElementProvider != null)
            {
                SizeF size = ElementProvider.GetElementSize(item);
                if (this.scrollbar.ScrollType == ScrollType.Vertical)
                {
                    height = (int)size.Height;
                    this.currentItemWidth = (int)size.Width;
                }
                else
                {
                    height = (int)size.Width;
                    this.currentItemWidth = (int)size.Height;
                }
            }

            return height;
        }

        protected virtual bool ScrollUp(int step)
        {
            if (this.scrollMode == ItemScrollerScrollModes.Smooth || this.scrollMode == ItemScrollerScrollModes.Deferred)
            {
                while (step > 0)
                {
                    if (this.traverser.Current == null)
                    {
                        if (scrollOffset > step)
                        {
                            scrollOffset -= step;
                        }
                        else
                        {
                            scrollOffset = 0;
                        }
                        return true;
                    }

                    int height = this.GetScrollHeight(this.traverser.Current) + itemSpacing;
                    if (scrollOffset >= step)
                    {
                        scrollOffset -= step;
                        return true;
                    }
                    else
                    {
                        step -= scrollOffset;
                        scrollOffset = 0;
                        if (height >= step)
                        {
                            scrollOffset = height - step;
                            step = 0;
                        }
                        else
                        {
                            step -= height;
                        }
                        if (!this.traverser.MovePrevious())
                        {
                            return true;
                        }
                    }
                }

                return true;
            }
            else if (step > 0 && traverser.Current != null)
            {
                while (step-- > 0)
                {
                    this.traverser.MovePrevious();
                }
                return true;
            }
            return false;
        }

        protected virtual bool UpdateOnScroll(ScrollEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                switch (e.Type)
                {
                    case ScrollEventType.First:
                        return ScrollToBegin();

                    case ScrollEventType.Last:
                        return ScrollToEnd();

                    case ScrollEventType.SmallIncrement:
                    case ScrollEventType.LargeIncrement:
                        return ScrollDown(e.NewValue - e.OldValue);

                    case ScrollEventType.SmallDecrement:
                    case ScrollEventType.LargeDecrement:
                        return ScrollUp(e.OldValue - e.NewValue);

                    case ScrollEventType.ThumbTrack:

                        if (this.asynchronousScrolling)
                        {
                            if (thumbDelta == 0)
                            {
                                this.suspendScrollerUpdated = this.ScrollMode == ItemScrollerScrollModes.Deferred;
                                this.thumbTimer.Start();
                            }

                            this.thumbDelta += e.NewValue - e.OldValue;
                            this.suspendScrollbarValueChanged = true;
                        }
                        else
                        {
                            int delta = e.NewValue - e.OldValue;
                            if (delta != 0)
                            {
                                if (delta > 0)
                                {
                                    this.ScrollDown(delta);
                                }
                                else if (delta < 0)
                                {
                                    this.ScrollUp(-delta);
                                }
                            }
                        }
                        return true;
                }
            }
            else if (this.ScrollMode == ItemScrollerScrollModes.Deferred)
            {
                if (e.Type == ScrollEventType.ThumbTrack)
                {
                    this.ShowToolTip();
                }
                else if (e.Type == ScrollEventType.EndScroll)
                {
                    this.HideToolTip();
                    this.suspendScrollerUpdated = false;
                    this.OnScrollerUpdated(EventArgs.Empty);
                }
            }

            return false;
        }

        #endregion

        #region Synchronize scrollbar parameters

        public virtual void UpdateScrollRange()
        {
            if (this.scrollbar != null)
            {
                if (this.traverser != null)
                {
                    int height = 0;
                    int count = 0;
                    object oldPosition = this.traverser.Position;
                    this.traverser.Reset();
                    this.maxItemWidth = 0;
                    while (this.traverser.MoveNext())
                    {
                        int currentItemHeight = this.GetScrollHeight(this.traverser.Current) + this.itemSpacing;
                        this.maxItemWidth = Math.Max(this.maxItemWidth, this.currentItemWidth);
                        height += currentItemHeight;
                        count++;
                    }

                    if (count > 0)
                    {
                        if (itemSpacing > 0)
                        {
                            height -= itemSpacing;
                        }
                        if (itemSpacing == 0)
                        {
                            height--;
                        }
                    }

                    this.traverser.Position = oldPosition;

                    if (height < 0)
                    {
                        return;
                    }

                    if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.ScrollMode == ItemScrollerScrollModes.Deferred)
                    {
                        if (this.scrollbar.Maximum != height)
                        {
                            this.scrollbar.Maximum = height;
                            this.SetScrollBarVisibility();
                            this.UpdateScrollValue();
                        }
                    }
                    else
                    {
                        if (this.scrollbar.Maximum != count - 1 && count - 1 > 0)
                        {
                            this.scrollbar.Maximum = count - 1;
                            this.UpdateScrollStep();
                            this.SetScrollBarVisibility();
                        }
                    }
                }
            }
        }

        protected void UpdateScrollValue()
        {
            int scrollBarValue = this.scrollbar.Value;
            int maxValue = this.scrollbar.Maximum - this.scrollbar.LargeChange + 1;

            this.oldScrollValue = -1;
            this.scrollbarChanged = false;

            if (maxValue > 0 && scrollBarValue > maxValue &&
                scrollBarValue > this.scrollbar.Minimum)
            {
                this.scrollbar.Value = maxValue;
            }
            else
            {
                if (this.scrollbar.Visibility != ElementVisibility.Visible)
                {
                    this.scrollbar.Value = this.scrollbar.Minimum;
                    return;
                }

                this.ScrollTo(scrollBarValue);
            }
        }

        public virtual void UpdateScrollRange(int width, bool updateScrollValue)
        {
            if (this.scrollbar != null)
            {
                if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.ScrollMode == ItemScrollerScrollModes.Deferred)
                {
                    if (this.scrollbar.Maximum != width)
                    {
                        this.scrollbar.Maximum = width;
                        SetScrollBarVisibility();
                        if (updateScrollValue)
                        {
                            UpdateScrollValue();
                        }
                        else
                        {
                            int maxValue = this.scrollbar.Maximum - this.scrollbar.LargeChange + 1;
                            if (maxValue > 0 && this.scrollbar.Value > maxValue && maxValue > this.scrollbar.Minimum)
                            {
                                this.scrollbar.Value = maxValue;
                            }
                        }
                    }
                }
                else
                {
                    UpdateScrollRange();
                }
            }
        }

        protected virtual void UpdateScrollStep()
        {
            if (this.traverser == null)
            {
                return;
            }

            if (this.scrollbar != null && ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                float largeChange = ClientSize.Height;

                if (this.scrollbar.ScrollType == ScrollType.Horizontal)
                {
                    largeChange = ClientSize.Width;
                }

                if (this.scrollMode == ItemScrollerScrollModes.Smooth || this.scrollMode == ItemScrollerScrollModes.Deferred)
                {
                    this.scrollbar.SmallChange = this.ItemHeight + this.ItemSpacing;
                    if (float.IsPositiveInfinity(largeChange))
                    {
                        this.scrollbar.LargeChange = 0;
                    }
                    else
                    {
                        this.scrollbar.LargeChange = (int)largeChange;
                    }
                    this.SetScrollBarVisibility();
                    this.UpdateScrollValue();
                }
                else
                {
                    int height = 0;
                    int count = 0;
                    object oldPosition = this.traverser.Position;
                    while (this.traverser.MoveNext())
                    {
                        height += this.GetScrollHeight(this.traverser.Current) + this.itemSpacing;
                        if (height >= this.clientSize.Height)
                        {
                            break;
                        }
                        count++;
                    }
                    this.traverser.Position = oldPosition;

                    this.scrollbar.SmallChange = 1;

                    while (this.scrollbar.Value + count > this.scrollbar.Maximum - 1)
                    {
                        count--;
                    }

                    if (this.scrollbar.Value + count < this.scrollbar.Maximum - 1 && count > 1)
                    {
                        this.scrollbar.LargeChange = count;
                    }

                    this.SetScrollBarVisibility();
                }
            }
        }

        protected void SetScrollBarVisibility()
        {
            if ((this.scrollbar.ScrollType == ScrollType.Horizontal && float.IsPositiveInfinity(this.clientSize.Width)) ||
                (this.scrollbar.ScrollType == ScrollType.Vertical && float.IsPositiveInfinity(this.clientSize.Height)))
            {
                this.scrollbar.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                if (this.scrollState == ScrollState.AlwaysShow ||
                    (this.scrollState == ScrollState.AutoHide && this.scrollbar.LargeChange < this.scrollbar.Maximum))
                {
                    this.scrollbar.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.scrollbar.Visibility = ElementVisibility.Collapsed;
                }
            }
        }

        #endregion

        #region Tip Window Management

        /// <summary>
        /// Shows scroller's tool tip
        /// </summary>
        protected virtual void ShowToolTip()
        {
            Control control = this.scrollbar.ElementTree.Control;

            if (control == null)
            {
                return;
            }

            if (this.toolTip == null)
            {
                this.toolTip = new ToolTip();
            }

            string title = this.GetToolTipText();

            const int horizontalOffset = 20;
            const int verticalOffset = 10;
            int x = 0;
            int y = 0;

            using (Graphics g = control.CreateGraphics())
            {
                SizeF titleSize = g.MeasureString(title, SystemFonts.DialogFont);
                if (this.scrollbar.ScrollType == ScrollType.Vertical)
                {
                    int width = (int)titleSize.Width;
                    x = this.scrollbar.ControlBoundingRectangle.Left - width - horizontalOffset;
                    y = this.scrollbar.ThumbElement.ControlBoundingRectangle.Y +
                        this.scrollbar.ThumbElement.Size.Height / 2 - verticalOffset;
                }
                else
                {
                    int height = (int)titleSize.Height;
                    x = this.scrollbar.ThumbElement.ControlBoundingRectangle.X +
                        this.scrollbar.ThumbElement.Size.Width / 2 - horizontalOffset;
                    y = this.scrollbar.ControlBoundingRectangle.Top - height - verticalOffset;
                }
            }

            this.toolTip.Show(title, control, x, y);
            this.isToolTipVisible = true;
        }

        /// <summary>
        /// Determines the ToolTip text
        /// </summary>
        /// <returns>Returns the ToolTip's text</returns>
        protected virtual string GetToolTipText()
        {
            int itemIndex = this.GetCurrentItemIndex();
            ItemScrollerToolTipTextNeededEventArgs<T> e = new ItemScrollerToolTipTextNeededEventArgs<T>(itemIndex, this.traverser.Current, "Position: " + itemIndex);
            this.OnToolTipTextNeeded(this, e);
            return e.ToolTipText;
        }

        /// <summary>
        /// Determines the traverser's current item index
        /// </summary>
        /// <returns>The Index of the current item</returns>
        protected virtual int GetCurrentItemIndex()
        {
            object current = this.traverser.Current;

            if (current == null)
            {
                return 0;
            }

            object currentPosition = this.traverser.Position;
            this.traverser.Reset();
            int index = -1;

            while (this.traverser.MoveNext())
            {
                index++;
                if (Object.Equals(this.traverser.Current, current))
                {
                    this.traverser.Position = currentPosition;
                    // We increase the index with +1 because we are always back to a row before the current row
                    // This give the real index of the current item
                    return index + 1;
                }
            }

            this.traverser.Position = currentPosition;
            return -1;
        }

        /// <summary>
        /// Hides scroller's tooltip
        /// </summary>
        protected virtual void HideToolTip()
        {
            if (this.toolTip != null)
            {
                this.toolTip.Hide(this.scrollbar.ElementTree.Control);
            }

            this.isToolTipVisible = false;
        }

        #endregion
    }
}
