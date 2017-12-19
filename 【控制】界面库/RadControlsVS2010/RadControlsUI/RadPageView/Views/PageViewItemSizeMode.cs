using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines how an item is sized within a RadPageViewElement instance.
    /// </summary>
    [Flags]
    public enum PageViewItemSizeMode
    {
        /// <summary>
        /// Each item's desired size is applied.
        /// </summary>
        Individual = 0,
        /// <summary>
        /// All items are with equal width.
        /// </summary>
        EqualWidth = 1,
        /// <summary>
        /// All items are with equal height.
        /// </summary>
        EqualHeight = EqualWidth << 1,
        /// <summary>
        /// All items are with equal size.
        /// </summary>
        EqualSize = EqualWidth | EqualHeight,
    }
}
