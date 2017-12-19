using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines possible modes for dragging items within a <see cref="RadPageView">RadPageView</see> instance
    /// </summary>
    public enum PageViewItemDragMode
    {
        /// <summary>
        /// Item dragging is disabled.
        /// </summary>
        None,
        /// <summary>
        /// A preview is generated, indicating where the item will be inserted when dropped. This mode is cancelable.
        /// </summary>
        Preview,
        /// <summary>
        /// The item is immediately reordered when moved to a different position.
        /// </summary>
        Immediate,
    }
}
