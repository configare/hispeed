using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	public class ToolStripOrientationEventArgs : EventArgs
	{
		private Orientation oldValue;
		private Orientation newValue;
		private bool cancel;

		public ToolStripOrientationEventArgs(Orientation oldValue, Orientation newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public Orientation OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public Orientation NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		public bool Cancel
		{
			get
			{
				return this.cancel;
			}
			set
			{
				this.cancel = value;
			}
		}
	}

	public delegate void ToolStripOrientationEventHandler(object sender, ToolStripOrientationEventArgs args);


}
  
