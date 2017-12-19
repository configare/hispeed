using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewVisualItemEventHandler(object sender, ListViewVisualItemEventArgs e);

    public class ListViewVisualItemEventArgs : EventArgs
    {
        protected BaseListViewVisualItem item;

        public ListViewVisualItemEventArgs(BaseListViewVisualItem item)
        {
            this.item = item;
        }

        public BaseListViewVisualItem VisualItem
        {
            get
            {
                return item;
            }
        }
    }
}
