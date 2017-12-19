using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides data for the ToolTipTextNeeded event used in ItemScroller
    /// </summary>
    public class ItemScrollerToolTipTextNeededEventArgs<T> : ToolTipTextNeededEventArgs
    {
        private int itemIndex;
        private T item;

        /// <summary>
        /// Gets the item index of the first visible item.
        /// </summary>
        public int ItemIndex
        {
            get { return this.itemIndex; }
        }

        /// <summary>
        /// Gets the item associated with this ToolTip.
        /// </summary>
        public T Item
        {
            get { return this.item; }
        }

        /// <summary>
        /// Initializes a new instance of the GridElementToolTipTextNeededEventArgs class.
        /// </summary>
        /// <param name="itemIndex">The row index of the first visible item.</param>
        /// <param name="item">The first visible item.</param>
        /// <param name="tooltipText">The default tooltip text.</param>
        public ItemScrollerToolTipTextNeededEventArgs(int itemIndex, T item, string tooltipText)
            : base(tooltipText)
        {
            this.itemIndex = itemIndex;
            this.item = item;
        }
    }
}
