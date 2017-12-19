using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemValueChangingEventHandler(object sender, ListViewItemValueChangingEventArgs e);

    public class ListViewItemValueChangingEventArgs: ValueChangingEventArgs
    {
        ListViewDataItem item;

        public ListViewItemValueChangingEventArgs(ListViewDataItem item, object newValue, object oldValue)
        : base(newValue, oldValue)
        {
            this.item = item;
        }

        public ListViewDataItem Item
        {
            get
            {
                return this.item;
            }
        }

        public RadListViewElement ListViewElement
        {
            get
            {
                return this.Item.Owner;
            }
        } 
    }
}