using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewVisualItemCancelEventHandler(object sender, ListViewVisualItemCancelEventArgs e);

    public class ListViewVisualItemCancelEventArgs : CancelEventArgs
    {
        protected BaseListViewVisualItem visualItem;

        public ListViewVisualItemCancelEventArgs(BaseListViewVisualItem visualItem)
        {
            this.visualItem = visualItem;
        }

        public ListViewVisualItemCancelEventArgs(BaseListViewVisualItem visualItem, bool cancel)
            : base(cancel)
        {
            this.visualItem = visualItem;
        }

        public BaseListViewVisualItem VisualItem
        {
            get
            {
                return visualItem;
            }
        } 
    }
}
