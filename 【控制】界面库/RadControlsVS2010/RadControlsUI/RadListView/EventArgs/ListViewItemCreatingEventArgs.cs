using System;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemCreatingEventHandler(object sender,ListViewItemCreatingEventArgs e);

    public class ListViewItemCreatingEventArgs : EventArgs
    {
        private ListViewDataItem item;

        public ListViewItemCreatingEventArgs(ListViewDataItem item)
        {
            this.item = item;
        }

        public ListViewDataItem Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }
    }
}
