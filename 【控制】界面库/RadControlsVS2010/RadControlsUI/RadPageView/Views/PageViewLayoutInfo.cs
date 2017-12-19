using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class contains layout information about a <see cref="RadPageViewItem"/> and
    /// performs base layout operations over an item like measuring.
    /// </summary>
    internal class PageViewLayoutInfo
    {
        #region Fields

        public bool vertical;
        public int itemSpacing;
        public int itemCount;
        public SizeF previousSize;
        public SizeF availableSize;
        public SizeF measuredSize;
        public float maxWidth;
        public float maxHeight;
        public float availableLength;
        public float paddingLength;
        public float borderLength;
        public float layoutLength;
        public PageViewItemSizeMode sizeMode;
        public PageViewContentOrientation contentOrientation;
        public LightVisualElement itemLayout;
        public List<PageViewItemSizeInfo> items;

        #endregion

        #region Ctor

        public PageViewLayoutInfo(LightVisualElement layout, SizeF available)
        {
            this.items = new List<PageViewItemSizeInfo>();
            this.itemLayout = layout;
            this.availableSize = available;
            this.previousSize = layout.PreviousConstraint;
            this.Update();
        }

        #endregion

        #region Methods

        public virtual void Update()
        {
            this.sizeMode = (PageViewItemSizeMode)this.itemLayout.GetValue(RadPageViewElement.ItemSizeModeProperty);
            this.contentOrientation = (PageViewContentOrientation)this.itemLayout.GetValue(RadPageViewElement.ItemContentOrientationProperty);
            this.itemSpacing = (int)this.itemLayout.GetValue(RadPageViewElement.ItemSpacingProperty);

            this.vertical = this.GetIsVertical();
            this.availableLength = this.vertical ? this.availableSize.Height : this.availableSize.Width;
            this.paddingLength = this.vertical ? this.itemLayout.Padding.Vertical : this.itemLayout.Padding.Horizontal;
            Padding border = this.itemLayout.GetBorderThickness(true);
            this.borderLength = this.vertical ? border.Vertical : border.Horizontal;
            int itemIndexCounter = 0;
            foreach (RadElement element in this.itemLayout.Children)
            {
                RadPageViewItem item = element as RadPageViewItem;
                if (item == null)
                {
                    continue;
                }

                if (item.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }
                //TODO: Optimization - cache desired size per item and measure only when a layout property is changed
                item.Measure(this.GetMeasureSizeConstraint(item));

                PageViewItemSizeInfo itemSizeInfo = this.CreateItemSizeInfo(item);

                this.maxWidth = Math.Max(itemSizeInfo.desiredSize.Width, this.maxWidth);
                this.maxHeight = Math.Max(itemSizeInfo.desiredSize.Height, this.maxHeight);

                this.items.Add(itemSizeInfo);

                itemSizeInfo.itemIndex = itemIndexCounter++;
            }

            this.itemCount = this.items.Count;
        }

        public virtual PageViewItemSizeInfo CreateItemSizeInfo(RadPageViewItem item)
        {
            return new PageViewItemSizeInfo(item, this.vertical);
        }

        public virtual SizeF GetMeasureSizeConstraint(RadItem item)
        {
            return LayoutUtils.InfinitySize;
        }

        public virtual bool GetIsVertical()
        {
            return false;
        }

        #endregion
    }
}
