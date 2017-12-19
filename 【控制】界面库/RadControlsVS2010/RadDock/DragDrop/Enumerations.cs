using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the supported drag-and-drop modes by a DragDropService.
    /// </summary>
    public enum DragDropMode
    {
        /// <summary>
        /// The associated RadDock instance decides which is the appropriate mode, depending on the drag context.
        /// </summary>
        Auto,
        /// <summary>
        /// Upon a drag-and-drop request, all the associated windows' state will immediately chang to "Floating"
        /// </summary>
        Immediate,
        /// <summary>
        /// A preview mode, which does not change the state of the dragged windows
        /// and simply hints where the windows will be positioned upon a commit.
        /// </summary>
        Preview,
    }

    /// <summary>
    /// Specifies the possible drag-and-drop contexts.
    /// </summary>
    public enum DragDropContext
    {
        /// <summary>
        /// The drag context is a FloatingWindow instance.
        /// </summary>
        FloatingWindow,
        /// <summary>
        /// The drag context is a ToolTabStrip instance.
        /// </summary>
        ToolTabStrip,
        /// <summary>
        /// The drag context is a ToolWindow
        /// </summary>
        ToolWindow,
        /// <summary>
        /// The drag context is a DockWindow, residing on a DocumentTabStrip
        /// </summary>
        DocumentWindow,
        /// <summary>
        /// The drag context is invalid.
        /// </summary>
        Invalid,
    }

    /// <summary>
    /// Defines the behavior of a started <see cref="DragDropService">DragDropService</see> instance.
    /// </summary>
    public enum DragDropBehavior
    {
        /// <summary>
        /// When the <see cref="DragDropService">DragDropService</see> is started, it will automatically handle user input and perform actions upon it.
        /// </summary>
        Auto,
        /// <summary>
        /// When the <see cref="DragDropService">DragDropService</see> is started, it will not handle user input and will rely on explicit manual commands to perform action.
        /// </summary>
        Manual,
    }
}
