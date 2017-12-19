using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Commands
{
	/// <summary>
	/// Represents the method that will handle HandleExecute, and Executed events.
	/// </summary>
	/// <param name="sender">Initializes the event sender.</param>
	/// <param name="e">Initializes the event argument data.</param>
	public delegate void CommandEventHandler(object sender, CommandEventArgs e);
}
