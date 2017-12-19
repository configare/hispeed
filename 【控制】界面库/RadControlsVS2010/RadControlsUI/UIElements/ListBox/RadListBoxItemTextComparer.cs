using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadListBoxItemTextComparer : System.Collections.IComparer
    {
        protected bool caseSensitive;
        protected bool descending;

        public RadListBoxItemTextComparer(bool caseSensitive, bool descending)
        {
            this.caseSensitive = caseSensitive;
            this.descending = descending;
        }

        public virtual int Compare(RadItem x, RadItem y)
        {
            return string.Compare(x.Text, y.Text, !this.caseSensitive);
        }

        int System.Collections.IComparer.Compare(object x, object y)
        {
            if (!(x is RadItem && y is RadItem))
            {
                throw new ArgumentException("Cannot convert argument to RadItem");
            }
            else if (descending)
            {
                return this.Compare((RadItem)y, (RadItem)x);
            }
            else
            {
                return this.Compare((RadItem)x, (RadItem)y);
            }
        }
    }
}
