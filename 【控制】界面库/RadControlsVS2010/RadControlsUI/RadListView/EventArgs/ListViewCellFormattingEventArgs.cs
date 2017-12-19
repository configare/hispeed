using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewCellFormattingEventHandler(object sender, ListViewCellFormattingEventArgs e);

    public class ListViewCellFormattingEventArgs : EventArgs
    {
        private DetailListViewCellElement cellElement;

        public ListViewCellFormattingEventArgs(DetailListViewCellElement cellElement)
        {
            this.cellElement = cellElement;
        }

        public DetailListViewCellElement CellElement
        {
            get
            {
                return cellElement;
            }
        }
    }
}
