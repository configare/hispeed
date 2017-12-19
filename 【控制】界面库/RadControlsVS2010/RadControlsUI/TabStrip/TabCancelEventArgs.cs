using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents the method that will handle the following events:
	///     <see cref="RadTabStripElement.TabSelecting">TabSelecting</see>,
	/// </summary>
	/// <param name="sender">Represents the event sender.</param>
	/// <param name="args">Represents the <see cref="TabCancelEventHandler">event arguments</see>.</param>
	public delegate void TabCancelEventHandler(object sender, TabCancelEventArgs args);

	public class TabCancelEventArgs : CancelEventArgs
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
		public TabCancelEventArgs(RadElement tabItem)
		{
			this.tabItem = tabItem;
		}
	}
}
