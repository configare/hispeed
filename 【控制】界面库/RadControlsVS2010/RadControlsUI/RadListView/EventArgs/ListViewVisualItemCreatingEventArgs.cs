using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewVisualItemCreatingEventHandler(object sender,ListViewVisualItemCreatingEventArgs e);

    public class ListViewVisualItemCreatingEventArgs : EventArgs
    {
        private BaseListViewVisualItem visualItem;
        private ListViewType listViewType;

        public ListViewVisualItemCreatingEventArgs(BaseListViewVisualItem visualItem, ListViewType viewType)
        {
            this.visualItem = visualItem;
            this.listViewType = viewType;
        }

        public ListViewType ListViewType
        {
            get
            {
                return listViewType;
            }
        }

        public BaseListViewVisualItem VisualItem
        {
            get
            {
                return visualItem;
            }
            set
            {
                visualItem = value;
            }
        }
    }
}
