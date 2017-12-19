using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	public class ToolStripChangedEventArgs : EventArgs
	{
		private RadToolStripElement oldValue;
		private RadToolStripElement newValue;

		public ToolStripChangedEventArgs(RadToolStripElement oldValue, RadToolStripElement newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public RadToolStripElement OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public RadToolStripElement NewValue
		{
			get
			{
				return this.newValue;
			}
		}
	}

	public delegate void ToolStripChangeEventHandler(object sender, ToolStripChangedEventArgs args);
}
