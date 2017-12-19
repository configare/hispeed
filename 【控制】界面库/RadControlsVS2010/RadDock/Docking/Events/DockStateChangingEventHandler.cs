using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template that is used to handle a <see cref="DockState">DockState</see> changing event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DockStateChangingEventHandler(object sender, DockStateChangingEventArgs e);

    /// <summary>
    /// Represents the arguments associated with a DockStateChanging event.
    /// </summary>
    public class DockStateChangingEventArgs : DockWindowCancelEventArgs
    {
        private DockState newDockState;

        /// <summary>
        /// Constructs a new instance of the <see cref="DockStateChangingEventArgs">DockStateChangingEventArgs</see> class.
        /// </summary>
        /// <param name="dockWindow">The <see cref="DockWindow">DockWindow</see> instance which <see cref="DockWindow.DockState">DockState</see> is about to change.</param>
        /// <param name="newDockState">The <see cref="DockState">DockState</see> value that is about to be applied to the window.</param>
        public DockStateChangingEventArgs(DockWindow dockWindow, DockState newDockState)
            : base(dockWindow)
        {
            this.newDockState = newDockState;
        }

        /// <summary>
        /// Gets the <see cref="DockState">DockState</see> that is about to be applied to the associated <see cref="DockWindow">DockWindow</see> instance.
        /// </summary>
        public DockState NewDockState
        {
            get 
            { 
                return newDockState; 
            }
        }
    }
}
