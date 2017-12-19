using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the allowed dock positions for a DockWindow.
    /// </summary>
    [Flags]
    public enum AllowedDockPosition
    {
        /// <summary>
        /// No dock allowed.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that the DockWindow will be docked to the left edge of the drop target.
        /// </summary>
        Left = 1,
        /// <summary>
        /// Indicates that the DockWindow will be docked to the top edge of the drop target.
        /// </summary>
        Top = 2,
        /// <summary>
        /// Indicates that the DockWindow will be docked to the right edge of the drop target.
        /// </summary>
        Right = 4,
        /// <summary>
        /// Indicates that the DockWindow will be docked to the bottom edge of the drop target.
        /// </summary>
        Bottom = 8,
        /// <summary>
        /// Indicates that the DockWindow will be added as a child of the drop target.
        /// </summary>
        Fill = 16,
        /// <summary>
        /// All dock positions are defined.
        /// </summary>
        All = Left | Top | Right | Bottom | Fill
    }
}
