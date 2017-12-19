using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemValueChangedEventHandler(object sender, ListViewItemValueChangedEventArgs e);

    public class ListViewItemValueChangedEventArgs : ListViewItemEventArgs
    {
        public ListViewItemValueChangedEventArgs(ListViewDataItem item)
            : base(item)
        {
        }
    }
}
