using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.UIElements.ListBox
{
    public class RadListBoxSelectionChangeEventArgs : EventArgs
    {
        private int selectedIndex = -1;
        private RadItem selectedItem = null;

        public RadListBoxSelectionChangeEventArgs(int index, RadItem item)
        {
            this.selectedIndex = index;
            this.selectedItem = item;
        }

        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }
        }

        public RadItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
        }

    }
}
