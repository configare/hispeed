using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines the possible alignment of the TabStripElement in a <see cref="TabStripPanel">TabStripPanel</see>.
    /// </summary>
    public enum TabStripAlignment
    {
        /// <summary>
        /// The panel itself decides where the element is positioned.
        /// </summary>
        Default,
        /// <summary>
        /// The element is positioned vertically on the left edge.
        /// </summary>
        Left,
        /// <summary>
        /// The element is positioned horizontally on the top edge.
        /// </summary>
        Top,
        /// <summary>
        /// The element is positioned vertically on the right edge.
        /// </summary>
        Right,
        /// <summary>
        /// The element is positioned horizontally on the bottom edge.
        /// </summary>
        Bottom,
    }

    /// <summary>
    /// Defines the possible orientation of text within a TabStripPanel.
    /// </summary>
    public enum TabStripTextOrientation
    {
        /// <summary>
        /// Default orientation is used, depending on the alignment of the TabStrip.
        /// </summary>
        Default,
        /// <summary>
        /// Text is oriented horizontally.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Text is oriented vertically.
        /// </summary>
        Vertical,
    }
}
