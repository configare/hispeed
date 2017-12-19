using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Paint;
using System.Runtime.InteropServices;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the element that implements the ExplorerBar view of the <see cref="RadPageView"/> control.
    /// This view allows for multiple visible pages, whereby items can be expanded/collapsed to show their content in an associated page.
    /// </summary>
    [Description("Represents the element that implements the ExplorerBar view of the RadPageView control."
        +" This view allows for multiple visible pages, whereby items can be expanded/collapsed to show their content in an associated page.")]
    public class RadPageViewExplorerBarElement : RadPageViewStackElement
    {
        #region Fields

        private RadScrollBarElement scrollbar;

        #endregion

        #region Ctors
        static RadPageViewExplorerBarElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new StackViewElementStateManager(), typeof(RadPageViewExplorerBarElement));
            new ControlDefault_RadPageView_Telerik_WinControls_UI_RadPageViewExplorerBarElement().DeserializeTheme();
        }
        #endregion

        #region Delegates

        public delegate void PageViewExplorerBarDelegate(RadPageViewExplorerBarItem item);
        
        #endregion

        #region Rad Properties

        public static readonly RadProperty ContentSizeModeProperty = RadProperty.Register(
            "ContentSizeMode",
            typeof(ExplorerBarContentSizeMode),
            typeof(RadPageViewExplorerBarElement),
            new RadElementPropertyMetadata(
                ExplorerBarContentSizeMode.FixedLength,
                ElementPropertyOptions.AffectsMeasure)
            );

        #endregion

        #region Events

        public event EventHandler<RadPageViewExpandedChangedEventArgs> ExpandedChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="RadScrollBarElement"/> that represents
        /// the scrollbar of the <see cref="RadPageViewExplorerBarElement"/>.
        /// </summary>
        [Browsable(false)]
        public RadScrollBarElement Scrollbar
        {
            get
            {
                return this.scrollbar;
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="ExplorerBarContentSizeMode"/> enum
        /// that defines how the content areas for each item are sized.
        /// </summary>
        [Description("Gets or sets a value that defines how the content areas for each item are sized.")]
        [DefaultValue(typeof(ExplorerBarContentSizeMode), "FixedLength")]
        public ExplorerBarContentSizeMode ContentSizeMode
        {
            get
            {
                return (ExplorerBarContentSizeMode)this.GetValue(ContentSizeModeProperty);
            }
            set
            {
                this.SetValue(ContentSizeModeProperty, value);
            }
        }

        //protected override Type ThemeEffectiveType
        //{
        //    get
        //    {
        //        return typeof(RadPageViewStackElement);
        //    }
        //}

        protected override ValueUpdateResult SetValueCore(RadPropertyValue propVal, object propModifier, object newValue, ValueSource source)
        {
            RadProperty property = propVal.Property;
            if (property == RadPageViewStackElement.ItemSelectionModeProperty
                && (StackViewItemSelectionMode)newValue != StackViewItemSelectionMode.ContentAfterSelected)
            {
                return ValueUpdateResult.Canceled;
            }

            if (property == RadPageViewStackElement.StackPositionProperty &&
                ((StackViewPosition)newValue == StackViewPosition.Bottom || (StackViewPosition)newValue == StackViewPosition.Right))
            {
                return ValueUpdateResult.Canceled;
            }

            return base.SetValueCore(propVal, propModifier, newValue, source);
        }

        #endregion

        #region Methods

        #region Initialization

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.Header.RadPropertyChanged += this.OnNCElement_PropertyChanged;
            this.Footer.RadPropertyChanged += this.OnNCElement_PropertyChanged;
            this.scrollbar = new RadScrollBarElement();
            this.scrollbar.Visibility = ElementVisibility.Collapsed;
            this.scrollbar.ScrollType = ScrollType.Vertical;
            this.scrollbar.Scroll += this.OnScrollbar_Scroll;
            this.scrollbar.SmallChange = 5;
            this.scrollbar.ZIndex = 3;
            this.Children.Add(this.scrollbar);
        }

        protected override void DisposeManagedResources()
        {
            this.scrollbar.Scroll -= this.OnScrollbar_Scroll;
            this.Header.RadPropertyChanged -= this.OnNCElement_PropertyChanged;
            this.Footer.RadPropertyChanged -= this.OnNCElement_PropertyChanged;
            base.DisposeManagedResources();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.ItemSelectionMode = StackViewItemSelectionMode.ContentAfterSelected;
            this.StackPosition = StackViewPosition.Top;
        }

        protected override void OnLoaded()
        {
            foreach (RadPageViewExplorerBarItem item in this.Items)
            {
                this.SetPageVisibility(item);
            }
        }

        #endregion

        #region Selection/Content

        protected internal override void OnSelectedPageChanged(RadPageViewEventArgs e)
        {
            base.OnSelectedPageChanged(e);
            this.RefreshNCArea();
        }

        protected override void SetSelectedContent(RadPageViewItem item)
        {
          
        }

        public override RadPageViewContentAreaElement GetContentAreaForItem(RadPageViewItem item)
        {
            if (item == null)
                return this.ContentArea as RadPageViewContentAreaElement;

            return (item as RadPageViewExplorerBarItem).AssociatedContentAreaElement;
        }

        #endregion

        #region Scrolling

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            HandledMouseEventArgs eventArgs = e as HandledMouseEventArgs;
            if (eventArgs == null || eventArgs.Handled)
            {
                return;
            }

            int step = Math.Max(1, e.Delta / SystemInformation.MouseWheelScrollDelta);
            int delta = Math.Sign(e.Delta) * step * SystemInformation.MouseWheelScrollLines;

            eventArgs.Handled = true;

            int result = this.scrollbar.Value - delta * this.scrollbar.SmallChange;

            if (result > this.scrollbar.Maximum - this.scrollbar.LargeChange + 1)
            {
                result = this.scrollbar.Maximum - this.scrollbar.LargeChange + 1;
            }

            if (result < this.scrollbar.Minimum)
            {
                result = 0;
                eventArgs.Handled = false;
            }
            else if (result > this.scrollbar.Maximum)
            {
                result = this.scrollbar.Maximum;
                eventArgs.Handled = false;
            }

            this.scrollbar.Value = result;
            this.InvalidateMeasure();
        }

        public virtual bool ScrollToItem(RadPageViewExplorerBarItem item)
        {
            if (this.layoutInfo == null)
            {
                return false;
            }

            RectangleF clientRect = this.GetClientRectangle(this.Size);
            Padding ncMargin = this.GetNCMetrics();

            clientRect.Y += ncMargin.Top;
            clientRect.X += ncMargin.Left;
            clientRect.Width -= ncMargin.Horizontal;
            clientRect.Height -= ncMargin.Vertical;

            RectangleF itemRectangle = item.BoundingRectangle;
            RectangleF intersectionRect = RectangleF.Intersect(clientRect, itemRectangle);

            switch (this.layoutInfo.position)
            {
                case StackViewPosition.Top:
                    if (intersectionRect.Height == itemRectangle.Height)
                    {
                        return false;
                    }
                    break;
                case StackViewPosition.Left:
                    if (intersectionRect.Width == itemRectangle.Width)
                    {
                        return false;
                    }
                    break;
            }

            switch (this.layoutInfo.position)
            {
                case StackViewPosition.Top:
                    this.initialLayoutOffset += (int)(clientRect.Top - itemRectangle.Top);
                    break;
                case StackViewPosition.Left:
                    this.initialLayoutOffset += (int)(clientRect.Left - itemRectangle.Left);
                    break;
            }

            this.initialLayoutOffset = this.CorrectLayoutOffset();
            this.scrollbar.Value = -this.initialLayoutOffset;
            this.InvalidateMeasure();
            return true;
        }

        protected override float GetInitialItemsOffset(RectangleF clientRect)
        {
            return this.initialLayoutOffset;
        }

        private int initialLayoutOffset = 0;

        private void OnScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            this.InvalidateMeasure();
        }

        private int GetOffsetValueAccordingToStackPosition()
        {
            if (this.layoutInfo == null)
                return 0;
            switch (this.layoutInfo.position)
            {
                case StackViewPosition.Top:
                case StackViewPosition.Left:
                    return -this.scrollbar.Value;
                case StackViewPosition.Right:
                case StackViewPosition.Bottom:
                    return this.scrollbar.Value - (this.scrollbar.Maximum - this.scrollbar.LargeChange + 1);
            }

            return 0;
        }

        private void SetInitialScrollbarParameters(StackViewPosition stackPosition)
        {
            bool isTopBottom = stackPosition == StackViewPosition.Top ||
                       stackPosition == StackViewPosition.Bottom;

            this.scrollbar.ScrollType = isTopBottom ? ScrollType.Vertical : ScrollType.Horizontal;
            this.scrollbar.Visibility = ElementVisibility.Visible;

            this.ResetLayoutOffset(true);
            this.InvalidateMeasure();
        }

        protected virtual void UpdateAndArrangeScrollbar(RectangleF clientRect)
        {
            if (this.CheckShowScrollbar(clientRect))
            {
                if (this.scrollbar.Visibility != ElementVisibility.Visible)
                {
                    this.SetInitialScrollbarParameters(this.layoutInfo.position);
                }
                this.ArrangeScrollbar(clientRect);
                this.initialLayoutOffset = this.GetOffsetValueAccordingToStackPosition();
                int correctedOffset = this.CorrectLayoutOffset();
                if (correctedOffset != this.initialLayoutOffset)
                {
                    this.initialLayoutOffset = correctedOffset;
                    this.ArrangeContentAndItems(clientRect);
                }
            }
            else
            {
                if (this.scrollbar.Visibility != ElementVisibility.Collapsed)
                {
                    this.ResetLayoutOffset(false);
                    this.scrollbar.Visibility = ElementVisibility.Collapsed;
                    this.InvalidateMeasure();
                }
            }
        }

        protected virtual RectangleF ArrangeScrollbar(RectangleF clientRect)
        {
            RectangleF scrollbarRect = Rectangle.Empty;
            switch (this.layoutInfo.position)
            {
                case StackViewPosition.Top:
                    if (!this.RightToLeft)
                    {
                        scrollbarRect = new RectangleF(
                        clientRect.Right,
                        clientRect.Top,
                        SystemInformation.VerticalScrollBarWidth, clientRect.Height);
                    }
                    else
                    {
                        scrollbarRect = new RectangleF(
                        clientRect.Left - SystemInformation.VerticalScrollBarWidth,
                        clientRect.Top,
                        SystemInformation.VerticalScrollBarWidth, clientRect.Height);
                    }
                    break;
                case StackViewPosition.Left:
                    scrollbarRect = new RectangleF(
                    clientRect.Left,
                    clientRect.Bottom,
                    clientRect.Width, SystemInformation.HorizontalScrollBarHeight);
                    break;
            }

            this.scrollbar.Arrange(scrollbarRect);
            this.UpdateScrollbarMetrics(clientRect);
            return clientRect;
        }

        #endregion

        #region Layout

        protected internal override void OnContentBoundsChanged()
        {
            if (this.Owner == null)
            {
                return;
            }
            
            foreach (RadPageViewPage page in this.Owner.Pages)
            {
                RadPageViewExplorerBarItem item = page.Item as RadPageViewExplorerBarItem;
                RadPageViewContentAreaElement contentArea = this.GetContentAreaForItem(item);
                page.Bounds = this.GetClientRectangleFromContentElement(contentArea);
            }

            this.Owner.Update();
        }
        
        internal override StackViewLayoutInfo CreateLayoutInfo(SizeF availableSize)
        {
            return new ExplorerBarLayoutInfo(this, availableSize);
        }

        internal override SizeF GetContentSizeForItem(PageViewItemSizeInfo sizeInfo, System.Drawing.RectangleF clientRect)
        {
            if (sizeInfo == null)
            {
                return SizeF.Empty;
            }
            ExplorerBarItemSizeInfo explorerBarSizeInfo = sizeInfo as ExplorerBarItemSizeInfo;
            if (!explorerBarSizeInfo.IsExpanded)
            {
                return SizeF.Empty;
            }

            SizeF result = SizeF.Empty;
            switch(this.layoutInfo.position)
            {
                case StackViewPosition.Top:
                    result.Width = clientRect.Width;
                    result.Height = this.GetContentLength(explorerBarSizeInfo, clientRect.Height);
                    break;
                case StackViewPosition.Left:
                    result.Width = this.GetContentLength(explorerBarSizeInfo, clientRect.Width);
                    result.Height = clientRect.Height;
                    break;
            }

            return result;
        }

        internal virtual float GetContentLength(ExplorerBarItemSizeInfo itemSizeInfo, float availableLength)
        {
            switch (this.ContentSizeMode)
            {
                case ExplorerBarContentSizeMode.FixedLength:
                    return itemSizeInfo.item.PageLength;
                case ExplorerBarContentSizeMode.AutoSizeToBestFit:

                    if (Owner == null)
                    {
                        switch (this.layoutInfo.position)
                        {
                            case StackViewPosition.Top:
                                return itemSizeInfo.contentSize.Height;
                            case StackViewPosition.Left:
                                return itemSizeInfo.contentSize.Width;
                        }
                    }
                    else
                    {
                        Size contentMetrics = this.GetItemContentMetrics(itemSizeInfo);
                        switch (this.layoutInfo.position)
                        {
                            case StackViewPosition.Top:
                                return contentMetrics.Height;
                            case StackViewPosition.Left:
                                return contentMetrics.Width;
                        }
                    }

                    break;
                case ExplorerBarContentSizeMode.EqualLength:
                    float itemLength = this.layoutInfo.layoutLength;
                    ExplorerBarLayoutInfo explorerLayoutInfo = this.layoutInfo as ExplorerBarLayoutInfo;
                    return (availableLength - itemLength) / explorerLayoutInfo.expandedItemsCount;
            }

            return availableLength;
        }

        internal virtual Size GetItemContentMetrics(ExplorerBarItemSizeInfo itemSizeInfo)
        {
            RadPageViewPage itemPage = itemSizeInfo.item.Page;
            RadPageViewExplorerBarItem explorerBarItem = itemSizeInfo.item as RadPageViewExplorerBarItem;
            Padding contentAreaPadding = explorerBarItem.AssociatedContentAreaElement.Padding;
            Size result = explorerBarItem.AssociatedContentAreaElement.MinSize;

            if (itemPage != null)
            {
                foreach (Control ctrl in itemPage.Controls)
                {
                    if (result.Width < ctrl.Left)
                        result.Width = ctrl.Left;

                    if (result.Height < ctrl.Bottom)
                        result.Height = ctrl.Bottom;
                }
            }

            result.Width += contentAreaPadding.Horizontal;
            result.Height += contentAreaPadding.Vertical;
            return result;
        }

        internal override float GetItemOffset(RectangleF clientRect, PageViewItemSizeInfo sizeInfo, float proposedOffset)
        {
            int itemSpacing = this.layoutInfo.itemSpacing;
            ExplorerBarItemSizeInfo itemSizeInfo = sizeInfo as ExplorerBarItemSizeInfo;
            SizeF contentSize = itemSizeInfo.contentSize;

            switch (this.layoutInfo.position)
            {
                case StackViewPosition.Bottom:
                case StackViewPosition.Top:
                    proposedOffset += contentSize.Height;
                    break;
                default:
                    proposedOffset += contentSize.Width;
                    break;
            }

            if (itemSizeInfo.IsExpanded)
            {
                proposedOffset -= itemSpacing;
            }

            ExplorerBarLayoutInfo info = this.layoutInfo as ExplorerBarLayoutInfo;
            info.fullLayoutLength = (int)proposedOffset;

            return proposedOffset;
        }

        private RectangleF GetCorrectedScrollbarClientRectangle(RectangleF currentRectangle)
        {
            if (this.scrollbar.Visibility != ElementVisibility.Visible)
            {
                return currentRectangle;
            }

            if (this.scrollbar.ScrollType == ScrollType.Horizontal)
            {
                currentRectangle.Height -= SystemInformation.HorizontalScrollBarHeight;
            }
            else
            {
                currentRectangle.Width -= SystemInformation.VerticalScrollBarWidth;

                if (this.RightToLeft)
                {
                    currentRectangle.X += SystemInformation.VerticalScrollBarWidth;
                }
            }

            return currentRectangle;
        }

        protected override SizeF MeasureItems(SizeF availableSize)
        {
            availableSize = this.GetCorrectedScrollbarClientRectangle(new RectangleF(Point.Empty, availableSize)).Size;

            SizeF desiredSize = base.MeasureItems(availableSize);
            //SizeF originalDesiredSize = desiredSize;

            if (this.Owner == null)
            {
                availableSize.Height -= desiredSize.Height;
            }
            
            foreach (ExplorerBarItemSizeInfo sizeInfo in this.layoutInfo.items)
            {
                if (this.Owner == null)
                {
                    availableSize.Height = float.PositiveInfinity;

                    RadPageViewExplorerBarItem explorerBarItem = (RadPageViewExplorerBarItem)sizeInfo.item;
                    if (explorerBarItem.AssociatedContentAreaElement != null && explorerBarItem.IsExpanded)
                    {
                        explorerBarItem.AssociatedContentAreaElement.Measure(availableSize);
                        if (StackPosition == StackViewPosition.Top)
                        {
                            desiredSize.Height += explorerBarItem.AssociatedContentAreaElement.DesiredSize.Height;
                            desiredSize.Width = Math.Max(desiredSize.Width, explorerBarItem.AssociatedContentAreaElement.DesiredSize.Width);
                            //availableSize.Height -= explorerBarItem.AssociatedContentAreaElement.DesiredSize.Height;
                        }
                        else
                        {                            
                            desiredSize.Width += explorerBarItem.AssociatedContentAreaElement.DesiredSize.Width;
                            desiredSize.Height = Math.Max(desiredSize.Height, explorerBarItem.AssociatedContentAreaElement.DesiredSize.Height);
                            //availableSize.Width -= explorerBarItem.AssociatedContentAreaElement.DesiredSize.Width;
                        }
                        sizeInfo.contentSize = explorerBarItem.AssociatedContentAreaElement.DesiredSize;
                    }
                }
                else
                {
                    sizeInfo.contentSize = this.GetContentSizeForItem(sizeInfo, new RectangleF(Point.Empty, availableSize));
                }
            }

            return desiredSize;
        }

        protected override SizeF MeasureContentArea(ref SizeF availableSize)
        {
            return SizeF.Empty;
        }

        protected virtual bool CheckShowScrollbar(RectangleF clientRect)
        {
            ExplorerBarLayoutInfo explorerBarLayoutInfo = this.layoutInfo as ExplorerBarLayoutInfo;
            switch (explorerBarLayoutInfo.position)
            {
                case StackViewPosition.Top:
                    if (clientRect.Height < explorerBarLayoutInfo.fullLayoutLength)
                        return true;
                    break;
                case StackViewPosition.Left:
                    if (clientRect.Width < explorerBarLayoutInfo.fullLayoutLength)
                        return true;
                    break;
            }

            return false;
        }

        public override RectangleF GetItemsRect()
        {
            RectangleF clientRect = this.GetClientRectangle(true, this.Size);
            clientRect.Y += this.Header.DesiredSize.Height + this.Header.Margin.Vertical;
            clientRect.Height -= (this.Header.DesiredSize.Height + this.Footer.DesiredSize.Height
                + this.Header.Margin.Vertical + this.Footer.Margin.Vertical);
            return clientRect;
        }

        protected virtual void UpdateScrollbarMetrics(RectangleF clientRect)
        {
            ExplorerBarLayoutInfo explorerBarLayoutInfo = this.layoutInfo as ExplorerBarLayoutInfo;
            switch (explorerBarLayoutInfo.position)
            {
                case StackViewPosition.Top:
                    this.scrollbar.LargeChange = (int)clientRect.Height;
                    break;
                case StackViewPosition.Left:
                    this.scrollbar.LargeChange = (int)clientRect.Width;
                    break;
            }

            this.scrollbar.Minimum = 0;

            if (explorerBarLayoutInfo.fullLayoutLength != this.scrollbar.Maximum)
            {
                this.scrollbar.Maximum = (int)explorerBarLayoutInfo.fullLayoutLength;
            }
        }

        private void ResetLayoutOffset(bool scrollbarVisible)
        {
            if (!scrollbarVisible)
            {
                this.initialLayoutOffset = 0;
            }
            else
            {
                this.initialLayoutOffset = this.GetOffsetValueAccordingToStackPosition();
            }
        }

        private int CorrectLayoutOffset()
        {
            int offset = this.scrollbar.Maximum - this.scrollbar.LargeChange + 1;
            int offsetFactor = -1;
            int currentOffset = this.initialLayoutOffset;
            if (currentOffset * offsetFactor > offset)
            {
                this.scrollbar.Value = offset;
                currentOffset = offsetFactor * offset;
            }
            else if (currentOffset * offsetFactor < 0)
            {
                currentOffset = 0;
            }
            return currentOffset;
        }

        protected override RectangleF PerformArrange(RectangleF clientRect)
        {
            clientRect = this.GetCorrectedScrollbarClientRectangle(clientRect);
            this.ArrangeContentAndItems(clientRect);
            this.UpdateAndArrangeScrollbar(clientRect);
            return clientRect;
        }

        private void ArrangeContentAndItems(RectangleF clientRect)
        {
            this.ArrangeItems(clientRect);
            this.ArrangeContent(clientRect);
        }

        protected override RectangleF ArrangeContent(RectangleF clientRect)
        {
            List<ContentAreaLayoutInfo> layoutInfos = this.GetContentAreaLayoutInfos(clientRect);
            ExplorerBarLayoutInfo explLayoutInfo = this.layoutInfo as ExplorerBarLayoutInfo;
            explLayoutInfo.fullLayoutLength = explLayoutInfo.layoutLength;
            foreach (ContentAreaLayoutInfo contentAreaLayoutInfo in layoutInfos)
            {
                RadPageViewExplorerBarItem item = (contentAreaLayoutInfo.AssociatedItem as RadPageViewExplorerBarItem);
                RadPageViewContentAreaElement contentAreaElement = item.AssociatedContentAreaElement;
                contentAreaElement.Arrange(contentAreaLayoutInfo.ContentAreaRectangle);

                switch (explLayoutInfo.position)
                {
                    case StackViewPosition.Top:
                        explLayoutInfo.fullLayoutLength += contentAreaLayoutInfo.ContentAreaRectangle.Height;
                        break;
                    case StackViewPosition.Left:
                        explLayoutInfo.fullLayoutLength += contentAreaLayoutInfo.ContentAreaRectangle.Width;
                        break;
                }                
            }

            this.OnContentBoundsChanged();

            switch (explLayoutInfo.position)
            {
                case StackViewPosition.Top:
                    if (this.Header.Visibility == ElementVisibility.Visible)
                    {
                        explLayoutInfo.fullLayoutLength += this.Header.Margin.Bottom;
                    }
                    if (this.Footer.Visibility == ElementVisibility.Visible)
                    {
                        explLayoutInfo.fullLayoutLength += this.Footer.Margin.Top;
                    }
                    break;
            }
            explLayoutInfo.fullLayoutLength -= explLayoutInfo.expandedItemsCount * explLayoutInfo.itemSpacing;
            return clientRect;
        }

        protected List<ContentAreaLayoutInfo> GetContentAreaLayoutInfos(RectangleF clientRect)
        {
            RectangleF contentRectangle = Rectangle.Empty;
            ExplorerBarLayoutInfo explorerLayoutInfo = this.layoutInfo as ExplorerBarLayoutInfo;
            List<ContentAreaLayoutInfo> layoutInfos = new List<ContentAreaLayoutInfo>();
            foreach (ExplorerBarItemSizeInfo sizeInfo in explorerLayoutInfo.items)
            {
                contentRectangle = this.GetContentWithSelectedContentRectangle(sizeInfo, clientRect);
                ContentAreaLayoutInfo info = new ContentAreaLayoutInfo();
                info.AssociatedItem = sizeInfo.item;
                info.ContentAreaRectangle = contentRectangle;
                layoutInfos.Add(info);
            }
            return layoutInfos;
        }

        #endregion

        #region Keyboard navigation

        protected internal override void ProcessKeyDown(KeyEventArgs e)
        {
            base.ProcessKeyDown(e);
            if (!(this.SelectedItem is RadPageViewExplorerBarItem))
            {
                return;
            }
            RadPageViewExplorerBarItem item = this.SelectedItem as RadPageViewExplorerBarItem;

            switch(this.layoutInfo.position)
            {
                case StackViewPosition.Top:
                    if (e.KeyData == Keys.Right)
                    {
                        item.IsExpanded = true;
                    }
                    else if (e.KeyData == Keys.Left)
                    {
                        item.IsExpanded = false;
                    }
                    break;
                case StackViewPosition.Left:
                    if (e.KeyData == Keys.Up)
                    {
                        item.IsExpanded = true;
                    }
                    else if (e.KeyData == Keys.Down)
                    {
                        item.IsExpanded = false;
                    }
                    break;
            }
        }

        protected internal override bool IsNextKey(System.Windows.Forms.Keys key)
        {
            StackViewPosition position = this.layoutInfo.position;
            switch (position)
            {
                case StackViewPosition.Top:
                    return key == System.Windows.Forms.Keys.Up;
                case StackViewPosition.Left:
                    return key == System.Windows.Forms.Keys.Left;
            }
            return base.IsNextKey(key);
        }

        protected internal override bool IsPreviousKey(System.Windows.Forms.Keys key)
        {
            StackViewPosition position = this.layoutInfo.position;
            switch (position)
            {
                case StackViewPosition.Top:
                    return key == System.Windows.Forms.Keys.Down;
                case StackViewPosition.Left:
                    return key == System.Windows.Forms.Keys.Right;
            }
            return base.IsPreviousKey(key);
        }

        #endregion

        #region Items Management

        protected override bool EnsureItemVisibleCore(RadPageViewItem item)
        {
            return this.ScrollToItem(item as RadPageViewExplorerBarItem);
        }

        protected internal virtual bool OnItemExpanding(RadPageViewExplorerBarItem item)
        {
            if (this.Owner != null)
            {
                RadPageViewCancelEventArgs cancelArgs = new RadPageViewCancelEventArgs(item.Page);

                this.Owner.OnPageExpanding(cancelArgs);
                return cancelArgs.Cancel;
            }

            return false;
        }

        protected internal virtual bool OnItemCollapsing(RadPageViewExplorerBarItem item)
        {
            if (this.Owner != null)
            {
                RadPageViewCancelEventArgs cancelArgs = new RadPageViewCancelEventArgs(item.Page);

                this.Owner.OnPageCollapsing(cancelArgs);
                return cancelArgs.Cancel;
            }

            return false;
        }

        internal void ExpandItem(RadPageViewExplorerBarItem item)
        {
            this.SynchronizeItemContentWithExpandedState(item);

            if (this.Owner != null)
            {
                this.Owner.OnPageExpanded(new RadPageViewEventArgs(item.Page));
            }
            OnExpandedChanged(new RadPageViewExpandedChangedEventArgs(item));
        }

        internal void CollapseItem(RadPageViewExplorerBarItem item)
        {
            this.SynchronizeItemContentWithExpandedState(item);

            if (this.Owner != null)
            {
                this.Owner.OnPageCollapsed(new RadPageViewEventArgs(item.Page));
            }
            OnExpandedChanged(new RadPageViewExpandedChangedEventArgs(item));
        }

        protected internal override void OnItemMouseUp(RadPageViewItem sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.OnItemMouseUp(sender, e);
            if (e.Button != MouseButtons.Left)
                return;
            RadPageViewExplorerBarItem explorerBarItem = sender as RadPageViewExplorerBarItem;
            explorerBarItem.IsExpanded = !explorerBarItem.IsExpanded;
        }

        private void SynchronizeItemContentWithExpandedState(RadPageViewExplorerBarItem item)
        {
            if (item.AssociatedContentAreaElement == null)
            {
            }
            if (this.Owner != null &&
                this.Owner.IsHandleCreated)
            {
                this.Owner.BeginInvoke(new PageViewExplorerBarDelegate(this.SetPageVisibility), item);
            }
            else
            {
                this.SetPageVisibility(item);
            }
        }

        private void SetPageVisibility(RadPageViewExplorerBarItem item)
        {
            if (this.Owner == null && item.AssociatedContentAreaElement != null)
            {
                if (item.IsExpanded)
                {
                    item.AssociatedContentAreaElement.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    item.AssociatedContentAreaElement.Visibility = ElementVisibility.Collapsed;
                }
                return;
            }
            if (item.Page == null)
                return;
            if (this.Owner != null && this.Owner.IsHandleCreated)
            {
                item.Page.Visible = item.IsExpanded;
            }
        }

        protected internal override void OnPageAdded(RadPageViewEventArgs e)
        {
            base.OnPageAdded(e);
            RadPageViewExplorerBarItem explorerBarItem = e.Page.Item as RadPageViewExplorerBarItem;
            e.Page.Dock = System.Windows.Forms.DockStyle.None;
            if (this.Items.Count > 1)
            {
                this.Children.Add(explorerBarItem.AssociatedContentAreaElement);
            }
        }

        protected override void AddItemCore(RadPageViewItem item)
        {
            base.AddItemCore(item);
            if (this.Owner == null)
            {
                //RadPageViewExplorerBarItem explorerBarItem =item as RadPageViewExplorerBarItem;
                //this.Children.Add(explorerBarItem.AssociatedContentAreaElement);
            }
        }

        protected internal override void OnPageRemoved(RadPageViewEventArgs e)
        {
            RadPageViewExplorerBarItem explorerBarItem = e.Page.Item as RadPageViewExplorerBarItem;
            if (this.Items.Count > 1)
            {
                explorerBarItem.AssociatedContentAreaElement.Dispose();
                explorerBarItem.AssociatedContentAreaElement = null;
            }
            else
            {
                this.ContentArea.Visibility = ElementVisibility.Collapsed;
            }

            base.OnPageRemoved(e);
        }

        protected override RadPageViewItem CreateItem()
        {
            RadPageViewExplorerBarItem item = new RadPageViewExplorerBarItem();

            if (this.Items.Count == 0)
            {
                item.AssociatedContentAreaElement = this.ContentArea as RadPageViewContentAreaElement;
            }
            else
            {
                RadPageViewContentAreaElement contentElement = new RadPageViewContentAreaElement();
                contentElement.Visibility = ElementVisibility.Collapsed;
                item.AssociatedContentAreaElement = contentElement;
                contentElement.Owner = this;
            }

            return item;
        }

        protected override void SetItemIndex(int currentIndex, int newIndex)
        {
            RadPageViewExplorerBarItem explorerBarItemToMove = this.Items[currentIndex] as RadPageViewExplorerBarItem;
            RadPageViewExplorerBarItem explorerBarItemTargetIndexItem = this.Items[newIndex] as RadPageViewExplorerBarItem;

            RadPageViewItem item = this.Items[currentIndex];
            RadPageViewReadonlyCollection<RadPageViewItem> items = this.Items as RadPageViewReadonlyCollection<RadPageViewItem>;
            items.RemoveAt(currentIndex);
            items.Insert(newIndex, item);
            int sourceChildrenIndex = this.ItemsParent.Children.IndexOf(explorerBarItemToMove);
            int targetChildrenIndex = this.ItemsParent.Children.IndexOf(explorerBarItemTargetIndexItem);
            this.ItemsParent.Children.Move(sourceChildrenIndex, targetChildrenIndex);
            this.ItemsParent.InvalidateMeasure();
        }

        #endregion

        #region NC handling

        internal void RefreshNCArea()
        {
            if (this.Owner != null && this.Owner.IsHandleCreated)
            {
                NativeMethods.SetWindowPos(
                    new HandleRef(null, this.Owner.Handle),
                    new HandleRef(null, IntPtr.Zero), 
                0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOOWNERZORDER | NativeMethods.SWP_FRAMECHANGED | NativeMethods.SWP_DRAWFRAME);
            }
        }

        protected internal override bool EnableNCModification
        {
            get
            {
                return true;
            }
        }

        protected internal override bool EnableNCPainting
        {
            get
            {
                return true;
            }
        }
        private Padding ncMetrics = Padding.Empty;
        private bool allowNCCALCSIZEProcessing = false;

        protected internal override Padding GetNCMetrics()
        {
            if (!allowNCCALCSIZEProcessing)
                return this.ncMetrics;
            return this.ncMetrics = this.CalculateNCMetrics();
        }

        private Padding CalculateNCMetrics()
        {
            Padding borderThickness = this.GetBorderThickness(false);
            int headerTopMargin = this.Header.Visibility == ElementVisibility.Visible ? this.Header.Margin.Top : 0;
            int footerBottomMargin = this.Footer.Visibility == ElementVisibility.Visible ? this.Footer.Margin.Bottom : 0;

            Padding headerPadding = new Padding(
                0,
                (int)this.Header.DesiredSize.Height + headerTopMargin,
                0,
               (int)this.Footer.DesiredSize.Height + footerBottomMargin);

            return Padding.Add(borderThickness, headerPadding);
        }
        
        protected override void PaintBorder(IGraphics graphics, float angle, SizeF scale)
        {
        }

        protected internal override Padding GetBorderThickness(bool checkDrawBorder)
        {
            if (checkDrawBorder)
            {
                return Padding.Empty;
            }
            return base.GetBorderThickness(checkDrawBorder);
        }

        private Bitmap PaintTopNCArea(Padding ncMetrics)
        {
            Size thisSize = this.DesiredSize.ToSize();
            int thisWidth = thisSize.Width;
            SizeF scaleSize = this.ScaleTransform;
            float angleTransform = this.AngleTransform;
            int bitmapWidth = thisWidth;
            int bitmapHeight = ncMetrics.Top;

            if (bitmapWidth <= 0 || bitmapHeight <= 0)
                return null;

            Padding borderThickness = this.GetBorderThickness(false);

            Bitmap topBitmap = new Bitmap(thisWidth, ncMetrics.Top);
            Graphics topGraphics = Graphics.FromImage(topBitmap);

            using (topGraphics)
            {
                Padding headerMargin = this.Header.Margin;
                RadGdiGraphics radGraphics = new RadGdiGraphics(topGraphics);
                using (SolidBrush backGroundBrush = new SolidBrush(this.BackColor))
                {
                    Bitmap headerBitmap = this.Header.GetAsBitmap(backGroundBrush, angleTransform, scaleSize);
                    if (headerBitmap != null)
                    {
                        topGraphics.DrawImage(headerBitmap, new Point(borderThickness.Left + headerMargin.Left, borderThickness.Top + headerMargin.Top));
                    }
                }

                this.BorderPrimitiveImpl.PaintBorder(radGraphics, angleTransform, scaleSize);
            }
            return topBitmap;
        }

        private Bitmap PaintBottomNCArea(Padding ncMetrics)
        {
            Size thisSize = this.DesiredSize.ToSize();
            int thisWidth = thisSize.Width;
            int thisHeight = thisSize.Height;
            SizeF scaleSize = this.ScaleTransform;
            float angleTransform = this.AngleTransform;

            int bitmapWidth = thisWidth;
            int bitmapHeight = ncMetrics.Bottom;

            if (bitmapWidth <= 0 || bitmapHeight <= 0)
                return null;

            Padding borderThickness = this.GetBorderThickness(false);

            Bitmap bottomBitmap = new Bitmap(thisWidth, ncMetrics.Bottom);
            Graphics bottomGraphics = Graphics.FromImage(bottomBitmap);

            using (bottomGraphics)
            {
                Padding footerMargin = this.Footer.Margin;
                RadGdiGraphics radGraphics = new RadGdiGraphics(bottomGraphics);

                using (SolidBrush backGroundBrush = new SolidBrush(this.BackColor))
                {
                    Bitmap footerBitmap = this.Footer.GetAsBitmap(backGroundBrush, angleTransform, scaleSize);
                    if (footerBitmap != null)
                    {
                        bottomGraphics.DrawImage(footerBitmap,
                            new Point(borderThickness.Left + footerMargin.Left, -footerMargin.Bottom - borderThickness.Bottom));
                    }
                }
                radGraphics.TranslateTransform(0, -(thisHeight - ncMetrics.Bottom));
                this.BorderPrimitiveImpl.PaintBorder(radGraphics, angleTransform, scaleSize);
            }

            return bottomBitmap;
        }

        private Bitmap PaintLeftNCArea(Padding ncMetrics)
        {
            Size thisSize = this.DesiredSize.ToSize();
            int thisHeight = thisSize.Height;
            SizeF scaleSize = this.ScaleTransform;
            float angleTransform = this.AngleTransform;

            int bitmapWidth = ncMetrics.Left;
            int bitmapHeight = thisHeight - ncMetrics.Vertical;
            if (bitmapWidth <= 0 || bitmapHeight <= 0)
                return null;

            Bitmap leftBitmap = new Bitmap(ncMetrics.Left, thisHeight - ncMetrics.Vertical);
            Graphics leftGraphics = Graphics.FromImage(leftBitmap);

            using (leftGraphics)
            {
                RadGdiGraphics radGraphics = new RadGdiGraphics(leftGraphics);
                radGraphics.TranslateTransform(0, -ncMetrics.Top);
                this.BorderPrimitiveImpl.PaintBorder(radGraphics, angleTransform, scaleSize);
            }
            return leftBitmap;
        }

        private Bitmap PaintRightNCArea(Padding ncMetrics)
        {
            Size thisSize = this.DesiredSize.ToSize();
            int thisWidth = thisSize.Width;
            int thisHeight = thisSize.Height;
            SizeF scaleSize = this.ScaleTransform;
            float angleTransform = this.AngleTransform;

            int bitmapWidth = ncMetrics.Right;
            int bitmapHeight = thisHeight - ncMetrics.Vertical;
            if (bitmapWidth <= 0 || bitmapHeight <= 0)
                return null;

            Bitmap rightBitmap = new Bitmap(ncMetrics.Right, thisHeight - ncMetrics.Vertical);

            Graphics rightGraphics = Graphics.FromImage(rightBitmap);

            using (rightGraphics)
            {
                RadGdiGraphics radGraphics = new RadGdiGraphics(rightGraphics);
                radGraphics.TranslateTransform(-(thisWidth - ncMetrics.Right), -ncMetrics.Top);
                this.BorderPrimitiveImpl.PaintBorder(radGraphics, angleTransform, scaleSize);
            }
            return rightBitmap;
        }

        protected internal override void OnNCPaint(Graphics g)
        {
            base.OnNCPaint(g);
            Padding ncMetrics = this.CalculateNCMetrics();

            Size thisSize = this.DesiredSize.ToSize();
            int thisWidth = thisSize.Width;
            int thisHeight = thisSize.Height;

            if (thisWidth <= 0 || thisHeight <= 0)
                return;

            Bitmap topBitmap = this.PaintTopNCArea(ncMetrics);
            if (topBitmap != null)
            {
                g.DrawImage(topBitmap, new Rectangle(Point.Empty, topBitmap.Size));
                topBitmap.Dispose();
            }

            Bitmap leftBitmap = this.PaintLeftNCArea(ncMetrics);
            if (leftBitmap != null)
            {
                g.DrawImage(leftBitmap, new Rectangle(new Point(0, ncMetrics.Top), leftBitmap.Size));
                leftBitmap.Dispose();
            }

            Bitmap rightBitmap = this.PaintRightNCArea(ncMetrics);
            if (rightBitmap != null)
            {
                g.DrawImage(rightBitmap, new Rectangle(new Point(thisWidth - ncMetrics.Right, ncMetrics.Top), rightBitmap.Size));
                rightBitmap.Dispose();
            }

            Bitmap bottomBitmap = this.PaintBottomNCArea(ncMetrics);
            if (bottomBitmap != null)
            {
                g.DrawImage(bottomBitmap, new Rectangle(new Point(0, thisHeight - ncMetrics.Bottom), bottomBitmap.Size));
                bottomBitmap.Dispose();
            }
        }

        protected override RectangleF GetClientRectangle(SizeF finalSize)
        {
            Padding ncMargin = this.CalculateNCMetrics();
            RectangleF result = base.GetClientRectangle(finalSize);
            if (this.Owner != null)
            {
                result.X -= ncMargin.Left;
                result.Y -= ncMargin.Top;
            }
            return result;
        }

        #endregion

        #region Event handling

        private void OnNCElement_PropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            RadElementPropertyMetadata metadata = e.Metadata as RadElementPropertyMetadata;
            if (metadata == null)
                return;

            if (metadata.AffectsDisplay)
            {
                this.RefreshNCArea();
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadPageViewExplorerBarElement.StackPositionProperty)
            {
                this.SetInitialScrollbarParameters((StackViewPosition)e.NewValue);
            }

            base.OnPropertyChanged(e);
        }

        protected override void OnBoundsChanged(RadPropertyChangedEventArgs e)
        {
            base.OnBoundsChanged(e);

            Padding currentNCMetrics = this.CalculateNCMetrics();
            if (this.ncMetrics != currentNCMetrics)
            {
                this.allowNCCALCSIZEProcessing = true;
                this.RefreshNCArea();
                this.allowNCCALCSIZEProcessing = false;
            }
        }

        protected virtual void OnExpandedChanged(RadPageViewExpandedChangedEventArgs e)
        { 
            EventHandler<RadPageViewExpandedChangedEventArgs> eventHandler = ExpandedChanged;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        
        #endregion

        #endregion
    }
}
