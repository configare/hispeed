using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents the method that will handle the following events:
	///     <see cref="RadTabStripElement.TabDragStarting">TabDragEnding</see>,
	///     <see cref="RadTabStripElement.TabDragStarted">TabDragEnded</see>
	/// </summary>
	/// <param name="sender">Represents the event sender.</param>
	/// <param name="args">Represents the <see cref="TabDragEventArgs">event arguments</see>.</param>
	public delegate void TabDragEventHandler(object sender, TabDragEventArgs args);
	
}
