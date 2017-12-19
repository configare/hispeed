using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines which internal buttons will be present for a RadPageViewStripElement instance.
    /// </summary>
    public enum StripViewButtons
    {
        /// <summary>
        /// No buttons are available.
        /// </summary>
        None = -1,
        /// <summary>
        /// Buttons are automatically displayed when needed.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Allows strip to be scrolled left when not enough space is available.
        /// </summary>
        LeftScroll = 1,
        /// <summary>
        /// Allows strip to be scrolled right when not enough space is available.
        /// </summary>
        RightScroll = LeftScroll << 1,
        /// <summary>
        /// Allows currently selected item to be closed.
        /// </summary>
        Close = RightScroll << 1,
        /// <summary>
        /// Displays all available items in a drop-down manner.
        /// </summary>
        ItemList = Close << 1,
        /// <summary>
        /// Both left and right scroll buttons are present.
        /// </summary>
        Scroll = LeftScroll | RightScroll,
        /// <summary>
        /// Both scroll buttons and Close button are present.
        /// </summary>
        VS2005Style = Close | Scroll,
        /// <summary>
        /// ItemList and Close buttons are present.
        /// </summary>
        VS2008Style = Close | ItemList,
        /// <summary>
        /// All buttons are present.
        /// </summary>
        All = LeftScroll | RightScroll | Close | ItemList,
    }
}
