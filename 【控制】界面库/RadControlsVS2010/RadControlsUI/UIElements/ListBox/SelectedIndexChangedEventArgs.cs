using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents the method that will handle the SelectedIndexChanged event.
	///     <param name="args">A SelectedIndexChangedEventArgs that contains the event data.</param>
	/// 	<param name="sender">The source of the event.</param>
	/// </summary>
	public delegate void RadSelectedIndexChangedEventHandler(object sender, SelectedIndexChangedEventArgs args);

	/// <summary>
	/// Represents event data of the SelectedIndexChanged event.
	/// </summary>
	public class SelectedIndexChangedEventArgs : EventArgs
	{
		private int oldIndex;
		private int newIndex;

		/// <summary>
		/// Initializes a new instance of the SelectedIndexChangedEventArgs class.
		/// </summary>		
		public SelectedIndexChangedEventArgs(int oldIndex, int newIndex)
		{
			this.oldIndex = oldIndex;
			this.newIndex = newIndex;
		}

		/// <summary>
		/// Gets the instance of previously selected item.
		/// </summary>
		public int OldIndex
		{
			get 
			{ 
				return this.oldIndex; 
			}
		}

		/// <summary>
		/// Gets the instance of currently selected item.
		/// </summary>
		public int NewIndex
		{
			get
			{
				return this.newIndex;
			}
		}
	}
}
