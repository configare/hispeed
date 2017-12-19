using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the possible edges for a DockWindow to become auto-hidden.
    /// </summary>
    public enum AutoHidePosition
    {
        /// <summary>
        /// Left edge of RadDock's bounds.
        /// </summary>
        Left,
        /// <summary>
        /// Top edge of RadDock's bounds.
        /// </summary>
        Top,
        /// <summary>
        /// Right edge of RadDock's bounds.
        /// </summary>
        Right,
        /// <summary>
        /// Bottom edge of RadDock's bounds.
        /// </summary>
        Bottom,
        /// <summary>
        /// The edge is automatically chosen depending on the current alignment
        /// of the DockWindow against the MainDocumentContainer that is hosted on RadDock.
        /// </summary>
        Auto,
    }
}
