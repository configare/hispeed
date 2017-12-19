using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	

	

	/// <summary>
	///     Represents event data for the following events:
	///     <see cref="RadTabStripElement.TabDragEnding">TabDragEnding</see> and
	///     <see cref="RadTabStripElement.TabDragEnded">TabDragEnded</see>.
	/// </summary>
	public class TabDragEventArgs : EventArgs
	{
		private TabItem draggedItem;
		private TabItem replacedItem;

		/// <summary>
		/// Gets the affected item.
		/// </summary>
		public TabItem DraggedItem
		{
			get
			{
				return this.draggedItem;
			}
		}

		/// <summary>
		/// Gets the replaced item.
		/// </summary>
		public TabItem ReplacedItem
		{
			get
			{
				return this.replacedItem;
			}
		}

		/// <summary>
		/// Initializes a new instance of the TabDragEndEventArgs class using the 
		/// affected item and the replaced item.
		/// </summary>
		/// <param name="draggedItem"></param>
		/// /// <param name="replacedItem"></param>
		public TabDragEventArgs(TabItem draggedItem, TabItem replacedItem)
		{
			this.draggedItem = draggedItem;
			this.replacedItem = replacedItem;
		}
	}
}
