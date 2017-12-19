using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewColumnEventHandler(object sender, ListViewColumnEventArgs e);

    public class ListViewColumnEventArgs : EventArgs
    {
        protected ListViewDetailColumn column;

        public ListViewColumnEventArgs(ListViewDetailColumn column)
        {
            this.column = column;
        }

        public ListViewDetailColumn Column
        {
            get
            {
                return column;
            }
        }

        public RadListViewElement ListViewElement
        {
            get
            {
                return column.Owner;
            }
        }
    }
}
