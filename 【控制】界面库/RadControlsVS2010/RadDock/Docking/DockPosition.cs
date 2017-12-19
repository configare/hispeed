using System;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the docking position of a DockWindow.
    /// </summary>
    public enum DockPosition
    {
        /// <summary>
        /// Indicates that the DockWindow will be docked to the left side of the target.
        /// </summary>
        Left,
        /// <summary>
        /// Indicates that the DockWindow will be docked to the top side of the target.
        /// </summary>
        Top,
        /// <summary>
        /// Indicates that the DockWindow will be docked to the right side of the target.
        /// </summary>
        Right,
        /// <summary>
        /// Indicates that the DockWindow will be docked to the bottom side of the target.
        /// </summary>
        Bottom,
        /// <summary>
        /// Indicates that the DockWindow will fill the target.
        /// </summary>
        Fill,
    }
}
