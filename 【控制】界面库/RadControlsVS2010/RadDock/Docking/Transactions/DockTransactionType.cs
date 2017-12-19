using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the possible transactions in a RadDock instance.
    /// </summary>
    public enum DockTransactionType
    {
        /// <summary>
        /// The transaction is initiated from a successful drag-and-drop operation.
        /// </summary>
        DragDrop,
        /// <summary>
        /// The transaction is initiated from a successful redock operation.
        /// </summary>
        Redock,
        /// <summary>
        /// The transaction is initiated from a successful DockWindow or AddDocument request.
        /// </summary>
        DockWindow,
        /// <summary>
        /// The transaction is initiated from an auto-hide request.
        /// </summary>
        AutoHide,
        /// <summary>
        /// The transaction is initiated from a CloseWindow request.
        /// </summary>
        Close,
        /// <summary>
        /// The transaction is initiated from a FloatWindow request.
        /// </summary>
        Float,
    }
}
