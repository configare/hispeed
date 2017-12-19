using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ItemEnteringEventHandler(object sender, ItemEnteringEventArgs e);

    public class ItemEnteringEventArgs : EventArgs
    {
        private int itemIndex;

        public ItemEnteringEventArgs(int itemIndex)
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
