using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    internal class ExplorerBarLayoutInfo : StackViewLayoutInfo
    {
        #region Fields

        public int expandedItemsCount = 0;
        public float fullLayoutLength = 0;
        public List<ExplorerBarItemSizeInfo> ExpandedItems;

        #endregion

        #region Ctor

        public ExplorerBarLayoutInfo(RadPageViewExplorerBarElement layout, SizeF availableSize)
            : base(layout, availableSize)
        {
        }

        #endregion

        #region Methods

        public override void Update()
        {
            this.expandedItemsCount = 0;

            if (this.ExpandedItems == null)
            {
                this.ExpandedItems = new List<ExplorerBarItemSizeInfo>();
            }
            else
            {
                this.ExpandedItems.Clear();
            }
            base.Update();
        }

        public override PageViewItemSizeInfo CreateItemSizeInfo(RadPageViewItem item)
        {
            RadPageViewExplorerBarItem explorerBarItem = item as RadPageViewExplorerBarItem;
            bool isVertical = this.GetIsVertical();
            bool isItemExpanded = explorerBarItem.IsExpanded;
            ExplorerBarItemSizeInfo sizeInfo = new ExplorerBarItemSizeInfo(explorerBarItem, isVertical, isItemExpanded);

            if (isItemExpanded)
            {
                this.ExpandedItems.Add(sizeInfo);
                this.expandedItemsCount++;
            }

            return sizeInfo;
        }

        #endregion
    }
}
