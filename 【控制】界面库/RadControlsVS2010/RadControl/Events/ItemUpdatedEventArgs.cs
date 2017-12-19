using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents event data for the ItemUpdated event.
    /// </summary>
    public class ItemUpdatedEventArgs : EventArgs
    {
        private RadItem item;

        /// <summary>
        /// Initializes a new instance of the ItemUpdatedEventArgs class using the RadItem.
        /// </summary>
        /// <param name="item"></param>
        public ItemUpdatedEventArgs(RadItem item)
        {
            this.item = item;
        }

        /// <summary>
        /// Gets the RadItem that is updated.
        /// </summary>
        public RadItem Item
        {
            get
            {
                return this.item;
            }
        }
    }
}
