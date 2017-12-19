using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
	/// <summary>
	/// Notifies listeners of dynamic changes, such as when items get added and removed or the whole list is refreshed. 
	/// </summary>
	/// <remarks>
	/// You can enumerate over any collection that implements the IEnumerable interface. However, to set up dynamic bindings so that insertions or deletions in the collection update the UI automatically, the collection must implement the INotifyCollectionChanged interface. This interface exposes the CollectionChanged event that must be raised whenever the underlying collection changes.
	/// </remarks>
	public interface INotifyCollectionChanging
	{
		/// <summary>
		/// Occurs before the collection changes.
		/// </summary>
		event NotifyCollectionChangingEventHandler CollectionChanging;
	}
}
