using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents the drag cancel event arguments of rad tab strip
	/// </summary>
	public class TabDragCancelEventArgs : TabDragEventArgs
	{
		public TabDragCancelEventArgs(TabItem draggedItem, TabItem replacedItem, bool cancel)
			: base(draggedItem, replacedItem)
		{
			this.cancel = cancel;
		}


		private bool cancel;

		/// <summary>
		/// Gets or sets whether the execution should be cancelled.
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
}
