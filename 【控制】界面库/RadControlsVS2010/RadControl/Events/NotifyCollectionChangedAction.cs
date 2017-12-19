using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    /// <summary>
    /// Describes the action that caused a CollectionChanged event. 
    /// </summary>
	public enum NotifyCollectionChangedAction
    {
        /// <summary>
        /// One or more items were added to the collection. 
        /// </summary>
        Add,
        /// <summary>
        /// One or more items were removed from the collection. 
        /// </summary>
        Remove,
        /// <summary>
        /// One or more items were replaced in the collection. 
        /// </summary>
        Replace,
        /// <summary>
        /// One or more items were moved within the collection.
        /// </summary>
        Move,
        /// <summary>
        /// The content of the collection changed dramatically.  
        /// </summary>
        Reset,
		/// <summary>
		/// The collection has been updated in a batch operation.
		/// </summary>
		Batch,
        /// <summary>
        /// An item in the collection is about to change.
        /// </summary>
        ItemChanging,
		/// <summary>
		/// An item in the collection has changed.
		/// </summary>
		ItemChanged,
    }
}
