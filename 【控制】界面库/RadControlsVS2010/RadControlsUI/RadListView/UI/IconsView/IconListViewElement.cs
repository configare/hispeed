using System.Windows.Forms;
using System;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class IconListViewElement : BaseListViewElement
    {
        #region Ctor

        public IconListViewElement(RadListViewElement owner)
            : base(owner)
        {
        }

        #endregion

        #region Virtual Methods

        protected virtual ListViewDataItem GetUpperItem(ListViewDataItem currentItem)
        {
            ListViewTraverser enumerator = this.Scroller.Traverser.GetEnumerator() as ListViewTraverser;
            enumerator.Position = currentItem;
            int currentItemX = 0;
            while (enumerator.MovePrevious() && 
                enumerator.Current != null && !enumerator.Current.IsLastInRow)
            {
                currentItemX += enumerator.Current.ActualSize.Width;
            }

            if (!(currentItem is ListViewDataItemGroup) && currentItem != null)
            {
                currentItemX += currentItem.ActualSize.Width / 2;
            }

            if (enumerator.Position == null)
            {
                return null;
            }

            while (enumerator.MovePrevious() && 
                enumerator.Current != null && !enumerator.Current.IsLastInRow)
            {

            }

            int x = 0;

            while (enumerator.MoveNext() && !enumerator.Current.IsLastInRow)
            {
                if (x + enumerator.Current.ActualSize.Width >= currentItemX)
                    return enumerator.Current;

                x += enumerator.Current.ActualSize.Width;
            }

            return enumerator.Current;
        }

        protected virtual ListViewDataItem GetDownerItem(ListViewDataItem currentItem)
        {
            ListViewTraverser enumerator = this.Scroller.Traverser.GetEnumerator() as ListViewTraverser;
            enumerator.Position = currentItem;
            int currentItemX = 0;
            while (enumerator.MovePrevious() && 
                enumerator.Current != null && !enumerator.Current.IsLastInRow)
            {
                currentItemX += enumerator.Current.ActualSize.Width;
            }

            if (!(currentItem is ListViewDataItemGroup) && currentItem != null)
            {
                currentItemX += currentItem.ActualSize.Width / 2;
            }

            enumerator.Position = currentItem;

            while (currentItem!= null &&
                !currentItem.IsLastInRow && 
                enumerator.MoveNext() && 
                !enumerator.Current.IsLastInRow)
            {
            }

            int x = 0;

            while (enumerator.MoveNext())
            {
                if (x + enumerator.Current.ActualSize.Width >= currentItemX)
                    return enumerator.Current;

                x += enumerator.Current.ActualSize.Width;

                if (enumerator.Current.IsLastInRow)
                {
                    break;
                }
            }

            return null;
        }

        #endregion

        #region Overrides

        public override bool FullRowSelect
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.ItemSize = new System.Drawing.Size(64, 64); 
        }

        protected override VirtualizedStackContainer<ListViewDataItem> CreateViewElement()
        {
            return new IconListViewContainer(this);
        }

        protected override ItemScroller<ListViewDataItem> CreateItemScroller()
        {
            return new IconListViewScroller();
        }

        protected override void HandleDownKey(KeyEventArgs e)
        {
            ListViewDataItem downerItem = GetDownerItem(this.owner.CurrentItem);
            if (downerItem != null)
            {
                this.ProcessSelection(downerItem, Control.ModifierKeys, false);
            }
        }

        protected override void HandleLeftKey(KeyEventArgs e)
        {
            ListViewDataItemGroup group = this.owner.CurrentItem as ListViewDataItemGroup;

            if (group != null)
            {
                group.Expanded = false;
            }
            else
            {
                ListViewDataItem previousItem = GetPreviousItem(this.owner.CurrentItem);
                if (previousItem != null)
                {
                    this.ProcessSelection(previousItem, Control.ModifierKeys, false);
                }
            }
        }

        protected override void HandleRightKey(KeyEventArgs e)
        {
            ListViewDataItemGroup group = this.owner.CurrentItem as ListViewDataItemGroup;

            if (group != null)
            {
                group.Expanded = true;
            }
            else
            {
                ListViewDataItem nextItem = GetNextItem(this.owner.CurrentItem);
                if (nextItem != null)
                {
                    this.ProcessSelection(nextItem, Control.ModifierKeys, false);
                }
            }
        }

        protected override void HandleUpKey(KeyEventArgs e)
        {
            ListViewDataItem upperItem = GetUpperItem(this.owner.CurrentItem);
            if (upperItem != null)
            {
                this.ProcessSelection(upperItem, Control.ModifierKeys, false);
            }
        }

        protected override void EnsureItemVisibleVerticalCore(ListViewDataItem item)
        {
            int offset = 0;
            int maxHeight = 0;
            ListViewTraverser traverser = (ListViewTraverser)this.Scroller.Traverser.GetEnumerator();

            BaseListViewVisualItem visualItem = null;

            while (traverser.MoveNext())
            {
                maxHeight = Math.Max(maxHeight, (int)ViewElement.ElementProvider.GetElementSize(traverser.Current).Height + this.ItemSpacing);

                if (!traverser.Current.IsLastInRow)
                {
                    continue;
                }

                this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + offset);
                this.UpdateLayout();
                visualItem = this.GetElement(item);

                if (visualItem != null)
                {
                    if (visualItem.ControlBoundingRectangle.Bottom > this.ViewElement.ControlBoundingRectangle.Bottom)
                    {
                        this.EnsureItemVisible(item);
                    }

                    break;
                }

                offset += maxHeight;
            }
        }

        protected override void OnOrientationChanged()
        {
            this.scrollBehavior.ScrollServices.Clear();

            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                this.VScrollBar.ValueChanged -= new EventHandler(HScrollBar_ValueChanged);
                this.Scroller.Scrollbar = this.VScrollBar;
                this.HScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);
            }
            else
            {
                this.HScrollBar.ValueChanged -= new EventHandler(HScrollBar_ValueChanged);
                this.Scroller.Scrollbar = this.HScrollBar;
                this.VScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);
            }

            this.scrollBehavior.ScrollServices.Add(new ScrollService(this,this.Scroller.Scrollbar));

            this.UpdateFitToSizeMode();
            this.Scroller.UpdateScrollRange();
            this.UpdateLayout();

            base.OnOrientationChanged();
        }

        protected override bool UpdateOnMeasure(SizeF availableSize)
        {
            RectangleF clientRect = GetClientRectangle(availableSize);

            RadScrollBarElement hscrollbar = this.HScrollBar;
            RadScrollBarElement vscrollbar = this.VScrollBar;

            if (this.Orientation == Orientation.Horizontal)
            {
                hscrollbar = this.VScrollBar;
                vscrollbar = this.HScrollBar;
            }

            ElementVisibility visibility = hscrollbar.Visibility;
            if (FitItemsToSize)
            {
                hscrollbar.Visibility = ElementVisibility.Collapsed;
            }
            else if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                hscrollbar.LargeChange = (int)(clientRect.Width - vscrollbar.DesiredSize.Width - this.ViewElement.Margin.Horizontal);
                hscrollbar.SmallChange = hscrollbar.LargeChange / 10;
                hscrollbar.Visibility = hscrollbar.LargeChange < hscrollbar.Maximum ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
            else
            {
                hscrollbar.LargeChange = (int)(clientRect.Height - vscrollbar.DesiredSize.Height - this.ViewElement.Margin.Vertical);
                hscrollbar.SmallChange = hscrollbar.LargeChange / 10;
                hscrollbar.Visibility = hscrollbar.LargeChange < hscrollbar.Maximum ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }

            SizeF clientSize = clientRect.Size;
            if (hscrollbar.Visibility == ElementVisibility.Visible)
            {
                clientSize.Height -= hscrollbar.DesiredSize.Width;
            }

            this.Scroller.ClientSize = clientSize;

            return visibility != hscrollbar.Visibility;
        }

        protected override void UpdateFitToSizeMode()
        {
            RadScrollBarElement scrollBar = this.Orientation == Orientation.Vertical ? this.HScrollBar : this.VScrollBar;
            scrollBar.Maximum = 0;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BoundsProperty)
            {
                this.Scroller.UpdateScrollRange();
            }
        }
         
        protected override void scroller_ScrollerUpdated(object sender, EventArgs e)
        {
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                this.ViewElement.ScrollOffset = new SizeF(this.ViewElement.ScrollOffset.Width, -this.Scroller.ScrollOffset);
            }
            else
            {
                this.ViewElement.ScrollOffset = new SizeF(-this.Scroller.ScrollOffset, this.ViewElement.ScrollOffset.Height);
            }
        }

        protected override void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                this.ViewElement.ScrollOffset = new SizeF(-this.HScrollBar.Value, this.ViewElement.ScrollOffset.Height);
            }
            else
            {
                this.ViewElement.ScrollOffset = new SizeF(this.ViewElement.ScrollOffset.Width , - this.VScrollBar.Value);
            }

            this.ViewElement.InvalidateMeasure();
        }

        protected override void Scroller_ScrollerUpdated(object sender, EventArgs e)
        {
            base.Scroller_ScrollerUpdated(sender, e);
            this.ViewElement.Invalidate();
        }

        #endregion
    }
}