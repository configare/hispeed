using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides data for the RadPageViewItemSelected event.
    /// </summary>
    public class RadPageViewItemSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the previous selected item of RadPageView.
        /// </summary>
        public readonly RadPageViewItem PreviousItem;

        /// <summary>
        /// Gets the selected item of RadPageView.
        /// </summary>
        public readonly RadPageViewItem SelectedItem;

        /// <summary>
        /// Initializes a new instance of the RadPageViewItemSelectedEventArgs class.
        /// </summary>
        /// <param name="previousItem">The previous selected item of RadPageView.</param>
        /// <param name="selectedItem">The selected item of RadPageView.</param>
        public RadPageViewItemSelectedEventArgs(RadPageViewItem previousItem, RadPageViewItem selectedItem)
        {
            this.PreviousItem = previousItem;
            this.SelectedItem = selectedItem;
        }
    }
}