using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.UIElements.ListBox
{
    class RadListBoxItemDesignTimeIndexComparer : RadListBoxItemIndexComparer
    {
        public RadListBoxItemDesignTimeIndexComparer(RadItemCollection col) : base(col)
        {

        }

        public override int Compare(object x, object y)
        {
            if (x is RadItem && y is RadItem)
            {
                bool xAddNew = (bool)((x as RadItem).GetValue(RadItem.IsAddNewItemProperty));
                bool yAddNew = (bool)((y as RadItem).GetValue(RadItem.IsAddNewItemProperty));

                if (xAddNew && yAddNew)
                {
                    return 0;
                }

                if (xAddNew)
                {
                    return 1;
                }

                if (yAddNew)
                {
                    return -1;
                }
            }

            return base.Compare(x, y);
        }
    }
}
