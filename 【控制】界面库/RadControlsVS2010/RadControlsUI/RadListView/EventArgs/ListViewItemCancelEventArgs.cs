using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemCancelEventHandler(object sender, ListViewItemCancelEventArgs e);

    public class ListViewItemCancelEventArgs : CancelEventArgs
    {
        protected ListViewDataItem item;

        public ListViewItemCancelEventArgs(ListViewDataItem item)
        {
            this.item = item;
        }

        public ListViewItemCancelEventArgs(ListViewDataItem item, bool cancel) : base(cancel)
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
