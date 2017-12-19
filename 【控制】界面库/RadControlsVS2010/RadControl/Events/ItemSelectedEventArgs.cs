using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs args);

    public class ItemSelectedEventArgs : EventArgs
    {
        #region Fields

        private RadItem item;

        #endregion

        #region Ctor

        public ItemSelectedEventArgs(RadItem item)
        {
            this.item = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item affected by the operation.
        /// </summary>
        public RadItem Item
        {
            get
            {
                return this.item;
            }
        }

        #endregion

    }
}
