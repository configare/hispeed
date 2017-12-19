using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the possible start-up positions for a RadDock's QuickNavigator.
    /// </summary>
    public enum QuickNavigatorDisplayPosition
    {
        /// <summary>
        /// The navigator is centered against its owning RadDock's screen bounds.
        /// </summary>
        CenterDockManager,
        /// <summary>
        /// The navigator is centered against its owning Form's screen bounds.
        /// </summary>
        CenterMainForm,
        /// <summary>
        /// The navigator is centered in the screen working area.
        /// </summary>
        CenterScreen,
        /// <summary>
        /// Manual position is defined for the navigator.
        /// </summary>
        Manual,
    }
}
