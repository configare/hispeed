using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template that is used to handle all cancelable events, associated with a <see cref="DockWindow">DockWindow</see> instance.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DockWindowCancelEventHandler(object sender, DockWindowCancelEventArgs e);

    /// <summary>
    /// Represents the arguments associated with all cancelable events, associated with a <see cref="DockWindow">DockWindow</see> instance.
    /// </summary>
    public class DockWindowCancelEventArgs : CancelEventArgs
    {
        private DockWindow newWindow;
        private DockWindow oldWindow;

        /// <summary>
        /// Constructs a new instance of the <see cref="DockWindowCancelEventArgs">DockWindowCancelEventArgs</see> class.
        /// </summary>
        /// <param name="dockWindow"></param>
        public DockWindowCancelEventArgs(DockWindow dockWindow)
        {
            this.newWindow = dockWindow;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DockWindowCancelEventArgs"/> class.
        /// </summary>
        /// <param name="oldWindow">The old dock window.</param>
        /// <param name="newWindow">The new dock window.</param>
        public DockWindowCancelEventArgs(DockWindow oldWindow, DockWindow newWindow)
        {
            this.oldWindow = oldWindow;
            this.newWindow = newWindow;
        }

        /// <summary>
        /// Gets the <see cref="DockWindow">DockWindow</see> instance that is associated with the event.
        /// </summary>
        [Obsolete("This method is obsolete and will be removed in the next release. Please use NewWindow property instead.")]
        public DockWindow DockWindow
        {
            get 
            { 
                return newWindow; 
            }
        }

        /// <summary>
        /// Gets the old window.
        /// </summary>
        /// <value>The old window.</value>
        public DockWindow OldWindow
        {
            get { return oldWindow; }
        }

        /// <summary>
        /// Gets the new window.
        /// </summary>
        /// <value>The new window.</value>
        public DockWindow NewWindow
        {
            get { return newWindow; }
        }
    }
}
