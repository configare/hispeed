using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadListBoxItemIndexComparer : System.Collections.IComparer
    {
        private RadItemCollection items;

        public RadListBoxItemIndexComparer(RadItemCollection items)
        {
            this.items = items;
        }

        public virtual int Compare(object x, object y)
        {
            int ix = items.IndexOf(x as RadItem);
            int iy = items.IndexOf(y as RadItem);
            if (ix != -1 && iy != -1)
            {
                return ix.CompareTo(iy);
            }
            else
            {
                if (ix == -1 && iy != -1)
                    return 1;
                else if (ix != -1 && iy == -1)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
