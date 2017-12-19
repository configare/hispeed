using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the available constants used by a RadDock instance to store its services.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Used to store the DragDropService.
        /// </summary>
        public const int DragDrop = 1;
        /// <summary>
        /// Used to store the RedockService.
        /// </summary>
        public const int Redock = DragDrop + 1;
        /// <summary>
        /// Used to store the ContextMenuService
        /// </summary>
        public const int ContextMenu = Redock + 1;
    }
}
