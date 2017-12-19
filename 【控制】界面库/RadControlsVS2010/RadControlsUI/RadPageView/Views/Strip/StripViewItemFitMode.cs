using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines possible modes to fit items within a RadPageViewStripElement instance.
    /// </summary>
    [Flags]
    public enum StripViewItemFitMode
    {
        /// <summary>
        /// Each item uses its desired size.
        /// </summary>
        None = 0,
        /// <summary>
        /// Items are shrinked if their size exceeds the available one.
        /// </summary>
        Shrink = 1,
        /// <summary>
        /// Items are expanded if their size is less than the available one.
        /// </summary>
        Fill = Shrink << 1,
        /// <summary>
        /// Items are either shrinked or expanded when needed.
        /// </summary>
        ShrinkAndFill = Shrink | Fill,
        /// <summary>
        /// Items are stretched in the available height of their parent container.
        /// </summary>
        FillHeight = Fill << 1,
    }
}
