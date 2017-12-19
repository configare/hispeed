using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewCellElementCreatingEventHandler(object sender, ListViewCellElementCreatingEventArgs e);

    public class ListViewCellElementCreatingEventArgs : EventArgs
    {
        private DetailListViewCellElement cellElement;

        public ListViewCellElementCreatingEventArgs(DetailListViewCellElement cellElement)
        {
            this.cellElement = cellElement;
        }

        public DetailListViewCellElement CellElement
        {
            get
            {
                return cellElement;
            }
            set
            {
                cellElement = value;
            }
        }
    }
}
