using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides data for the RadPageViewItemSelecting event.
    /// </summary>
    public class RadPageViewItemSelectingEventArgs : CancelEventArgs
    {
         /// <summary>
        /// Gets the selected item of RadPageView.
        /// </summary>
        public readonly RadPageViewItem SelectedItem;

        /// <summary>
        /// Gets the item to be selected.
        /// </summary>
        public readonly RadPageViewItem NextItem;

        /// <summary>
        /// Initializes a new instance of the RadPageViewItemSelectingEventArgs class.
        /// </summary>
        /// <param name="selectedItem">The selected item of RadPageView./param>
        /// <param name="nextItem">The item to be selected.</param>
        public RadPageViewItemSelectingEventArgs(RadPageViewItem selectedItem, RadPageViewItem nextItem)
        {
            this.SelectedItem = selectedItem;
            this.NextItem = nextItem;
        }
    }
}