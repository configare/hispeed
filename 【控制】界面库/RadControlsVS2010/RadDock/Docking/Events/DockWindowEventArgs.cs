using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template that is used to handle all <see cref="DockWindow">DockWindow</see> events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DockWindowEventHandler(object sender, DockWindowEventArgs e);

    /// <summary>
    /// Represents the arguments associated with all <see cref="DockWindow">DockWindow</see> events.
    /// </summary>
    public class DockWindowEventArgs : EventArgs
    {
        private DockWindow dockWindow;

        /// <summary>
        /// Constructs a new instance of the <see cref="DockWindowEventArgs">DockWindowEventArgs</see> class.
        /// </summary>
        /// <param name="dockWindow">The <see cref="DockWindow">DockWindow</see> instance associated with the event.</param>
        public DockWindowEventArgs(DockWindow dockWindow)
        {
            this.dockWindow = dockWindow;
        }

        /// <summary>
        /// Gets the <see cref="DockWindow">DockWindow</see> instance associated with the event.
        /// </summary>
        public DockWindow DockWindow
        {
            get 
            { 
                return dockWindow; 
            }
        }
    }
}
