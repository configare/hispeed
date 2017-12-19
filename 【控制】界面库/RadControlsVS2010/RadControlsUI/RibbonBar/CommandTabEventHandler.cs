using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents event data for the following events: OnTabSelected, OnTabHovered, 
	/// OnTabDragStarted, OnTabDragStarting, OnTabDragEnding, and OnTabDragEnded.
	/// </summary>
	public class CommandTabEventArgs : EventArgs
	{
		private RibbonTab commandTab;

		/// <summary>
		/// Gets the affected command tab.
		/// </summary>
		public RibbonTab CommandTab
		{
			get
			{
				return this.commandTab;
			}
		}
		/// <summary>
		/// Initializes a new instance of the CommandTabEventArgs class using the 
		/// affected command tab.
		/// </summary>
		/// <param name="commandTab"></param>
		public CommandTabEventArgs(RibbonTab commandTab)
		{
			this.commandTab = commandTab;
		}
	}
	/// <summary>
	///     Represents the method that will handle the following event:
	///     <see cref="RadRibbonBarElement.CommandTabSelected">CommandTabSelected</see>.
	/// </summary>
	public delegate void CommandTabEventHandler(object sender, CommandTabEventArgs args);
}
