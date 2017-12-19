using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemChangingEventHandler(object sender, ListViewItemChangingEventArgs e);

    public class ListViewItemChangingEventArgs : CancelEventArgs
    {
        protected ListViewDataItem oldItem;
        protected ListViewDataItem newItem;

        public ListViewItemChangingEventArgs(ListViewDataItem oldItem, ListViewDataItem newItem)
        {
            this.oldItem = oldItem;
            this.newItem = newItem;
            this.Cancel = false;
        }

        public ListViewDataItem OldItem
        {
            get
            {
                return oldItem;
            }
        }

        public ListViewDataItem NewItem
        {
            get
            {
                return newItem;
            }
        }
    }
}