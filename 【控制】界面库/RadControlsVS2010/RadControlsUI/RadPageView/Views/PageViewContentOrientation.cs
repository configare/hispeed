using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines the content orientation of in RadPageViewItem.
    /// </summary>
    public enum PageViewContentOrientation
    {
        /// <summary>
        /// Orientation is automatically selected depending on the item alignment within the owning RadPageViewElement. 
        /// </summary>
        Auto,
        /// <summary>
        /// Item's content is horizontally oriented.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Item's content is rotated by 180 degrees.
        /// </summary>
        Horizontal180,
        /// <summary>
        /// Item's content is rotated by 90 degrees.
        /// </summary>
        Vertical90,
        /// <summary>
        /// Item's content is rotated 270 degrees.
        /// </summary>
        Vertical270,
    }
}
