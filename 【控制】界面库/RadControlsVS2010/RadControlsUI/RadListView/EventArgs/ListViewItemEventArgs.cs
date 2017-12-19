using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemEventHandler(object sender, ListViewItemEventArgs e);

    public class ListViewItemEventArgs : EventArgs
    {
        protected ListViewDataItem item;

        public ListViewItemEventArgs(ListViewDataItem item)
        {
            this.item = item;
        }

        public ListViewDataItem Item
        {
            get
            {
                return item;
            }
        }

        public RadListViewElement ListViewElement
        {
            get
            {
                return item.Owner;
            }
        }
    }
}
