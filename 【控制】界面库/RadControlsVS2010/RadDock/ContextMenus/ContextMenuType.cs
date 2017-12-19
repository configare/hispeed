using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the different types of context menus, displayed within a RadDock instance.
    /// </summary>
    public enum ContextMenuType
    {
        /// <summary>
        /// Context menu, associated with a DockWindow instance.
        /// </summary>
        DockWindow,
        /// <summary>
        /// Context menu, listing all opened documents within a document strip.
        /// </summary>
        ActiveWindowList
    }
}
