using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.UIElements.ListBox
{
    class RadListBoxItemDesignTimeTextComparer : RadListBoxItemTextComparer
    {
        public RadListBoxItemDesignTimeTextComparer(bool caseSensitive, bool descending)
            : base(caseSensitive, descending)
        {
        }

        public override int Compare(RadItem x, RadItem y)
        {
            bool xAddNew = (bool)x.GetValue(RadItem.IsAddNewItemProperty);
            bool yAddNew = (bool)y.GetValue(RadItem.IsAddNewItemProperty);
            // This condition does not mean that there can be more than one "AddNewItem" items. It means that 
            // the Sort method calls x.Compare(x); and throws an exception if this method does not return 0.
            if (xAddNew && yAddNew)
            {
                return 0;
            } 
            else if (xAddNew || yAddNew)
            {
                return -1;
            }

            return base.Compare(x, y);
        }
    }
}
