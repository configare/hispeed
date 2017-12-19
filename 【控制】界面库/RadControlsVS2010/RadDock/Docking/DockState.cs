using System;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the possible valid states for IDockWindow instances.
    /// </summary>
    public enum DockState
    {
        /// <summary>
        /// Indicates that the IDockWindow instance is docked.
        /// </summary>
        Docked,
        /// <summary>
        /// Indicates that the IDockWindow instance is docked inside a TabbedDocument interface.
        /// </summary>
        TabbedDocument,
        /// <summary>
        /// Indicates that the IDockWindow instance is in a hidden/removed state.
        /// </summary>
        Hidden,
        /// <summary>
        /// Indicates that the IDockWindow instance is managed by the auto-hide sub-system of the docking system.
        /// </summary>
        AutoHide,
        /// <summary>
        /// Indicates that the IDockWindow instance is floating.
        /// </summary>
        Floating
    }

    /// <summary>
    /// Defines the states, allowed per DockWindow instance.
    /// </summary>
    [Flags]
    public enum AllowedDockState
    {
        /// <summary>
        /// Indicates that the IDockWindow instance is docked.
        /// </summary>
        Docked = 1,
        /// <summary>
        /// Indicates that the IDockWindow instance is docked inside a TabbedDocument interface.
        /// </summary>
        TabbedDocument = 2,
        /// <summary>
        /// Indicates that the IDockWindow instance is in a hidden/removed state.
        /// </summary>
        Hidden = 4,
        /// <summary>
        /// Indicates that the IDockWindow instance is managed by the auto-hide sub-system of the docking system.
        /// </summary>
        AutoHide = 8,
        /// <summary>
        /// Indicates that the IDockWindow instance is floating.
        /// </summary>
        Floating = 16,

        /// <summary>
        /// All dock states are defined.
        /// </summary>
        All = Docked | TabbedDocument | Hidden | AutoHide | Floating
    }
   
}
