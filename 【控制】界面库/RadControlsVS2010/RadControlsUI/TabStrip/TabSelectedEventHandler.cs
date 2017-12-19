using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents event data for the following events:
    ///     <see cref="RadTabStripElement.TabSelected">TabSelected</see>,
    ///     <see cref="RadTabStripElement.TabHovered">TabHovered</see>,
    ///     <see cref="RadTabStripElement.TabDragStarted">TabDragStarted</see>,
    ///     <see cref="RadTabStripElement.TabDragStarting">TabDragStarting</see>
    /// </summary>
	public class TabEventArgs : EventArgs
	{
		private RadElement tabItem;

        /// <summary>
        /// Gets the affected item.
        /// </summary>
		public RadElement TabItem
		{
			get
			{
				return this.tabItem;
			}
		}
        /// <summary>
        /// Initializes a new instance of the TabEventArgs class using the 
        /// affected item.
        /// </summary>
        /// <param name="tabItem"></param>
		public TabEventArgs(RadElement tabItem)
		{
			this.tabItem = tabItem;
		}
	}


    /// <summary>
    ///     Represents the method that will handle the following events:
    ///     <see cref="RadTabStripElement.TabSelected">TabSelected</see>,
    ///     <see cref="RadTabStripElement.TabHovered">TabHovered</see>,
    ///     <see cref="RadTabStripElement.TabDragStarting">TabDragStarting</see>,
    ///     <see cref="RadTabStripElement.TabDragStarted">TabDragStarted</see>
    /// </summary>
    /// <param name="sender">Represents the event sender.</param>
    /// <param name="args">Represents the <see cref="TabEventArgs">event arguments</see>.</param>
	public delegate void TabEventHandler(object sender, TabEventArgs args);

	
}