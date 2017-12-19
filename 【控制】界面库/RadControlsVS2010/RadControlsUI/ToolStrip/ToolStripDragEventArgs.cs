using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	public class ToolStripDragEventArgs : EventArgs
	{
		private RadToolStripElement currentRow;
		private RadToolStripItem currentToolStripItem;
		private bool cancel = false;

		public ToolStripDragEventArgs(RadToolStripElement currentRow, RadToolStripItem currentToolStripItem)
		{
			this.currentRow = currentRow;
			this.currentToolStripItem = currentToolStripItem;
		}
        /// <summary>
        /// Gets the current row.
        /// </summary>
		public RadToolStripElement CurrentRow
		{
			get
			{
				return this.currentRow;
			}
		}

        /// <summary>
        /// Gets the current tool strip item.
        /// </summary>
		public RadToolStripItem CurrentToolStripItem
		{
			get
			{
				return this.currentToolStripItem;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether the action should be canceled. 
        /// </summary>
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

	public delegate void ToolStripDragEventHandler(object sender, ToolStripDragEventArgs e);
}
