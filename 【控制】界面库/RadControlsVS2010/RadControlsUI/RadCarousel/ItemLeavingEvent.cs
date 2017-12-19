using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ItemLeavingEventHandler(object sender, ItemLeavingEventArgs e);

    public class ItemLeavingEventArgs : EventArgs
    {
        private int itemIndex;

        public ItemLeavingEventArgs(int itemIndex)
        {
            this.itemIndex = itemIndex;
        }

        public int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = value; }
        }
    }
}
