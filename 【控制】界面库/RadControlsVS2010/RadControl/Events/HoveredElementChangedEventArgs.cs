using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents the method that will handle the 
    /// %HoveredElementChanged:HoveredElementChanged% event.
    /// </summary>
    /// <param name="source">Initializes the event sender.</param>
    /// <param name="args">Initializes the %event arguments:HoveredElementChangedEventArgs%.</param>
    public delegate void HoveredElementChangedEventHandler(object source, HoveredElementChangedEventArgs args);

    /// <summary>
    /// Represents event data for the HoveredElementChanged event.
    /// </summary>
    public class HoveredElementChangedEventArgs : EventArgs
    {
        public readonly RadElement HoveredElement;

        /// <summary>
        /// Initializes a new instance of the HoveredElementChangedEventArgs class.
        /// </summary>
        /// <param name="hoveredElement"></param>
        public HoveredElementChangedEventArgs(RadElement hoveredElement)
        {
            this.HoveredElement = hoveredElement;
        }
    }
}
