using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewColumnCreatingEventHandler(object sender, ListViewColumnCreatingEventArgs e);

    public class ListViewColumnCreatingEventArgs : EventArgs
    {
        private ListViewDetailColumn column;

        public ListViewColumnCreatingEventArgs(ListViewDetailColumn column)
        {
            this.column = column;
        }

        public ListViewDetailColumn Column
        {
            get
            {
                return column;
            }
            set
            {
                column = value;
            }
        }
    }
}
