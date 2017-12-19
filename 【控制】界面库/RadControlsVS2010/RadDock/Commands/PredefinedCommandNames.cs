using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Contains the names of all predefined commands available in a RadDock instance.
    /// </summary>
    public static class PredefinedCommandNames
    {
        /// <summary>
        /// The name for the command that displays the QuickNavigator in a RadDock instance.
        /// </summary>
        public static readonly string DisplayQuickNavigator = "DisplayQuickNavigator";
        /// <summary>
        /// The command that closes the active document in a RadDock instance.
        /// </summary>
        public static readonly string CloseActiveDocument = "CloseActiveDocument";
        /// <summary>
        /// The command that activates the next document in a RadDock instance.
        /// </summary>
        public static readonly string NextDocument = "NextDocument";
        /// <summary>
        /// The command that activates the previous document in a RadDock instance.
        /// </summary>
        public static readonly string PreviousDocument = "PreviousDocument";
    }
}
