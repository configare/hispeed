using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides data for the RadPageViewItemsChanged event.
    /// </summary>
    public class RadPageViewItemsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the changed item.
        /// </summary>
        public readonly RadPageViewItem ChangedItem;

        /// <summary>
        /// Gets the change operation.
        /// </summary>
        public readonly ItemsChangeOperation Operation;

        /// <summary>
        /// Initializes a new instance of the RadPageViewItemsChangedEventArgs class.
        /// </summary>
        /// <param name="changedItem">The changed item.</param>
        /// <param name="operation">The change operation.</param>
        public RadPageViewItemsChangedEventArgs(RadPageViewItem changedItem, ItemsChangeOperation operation)
        {
            this.ChangedItem = changedItem;
            this.Operation = operation;
        }
    }
}