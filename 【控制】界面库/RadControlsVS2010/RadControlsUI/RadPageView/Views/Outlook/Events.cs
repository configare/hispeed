using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void OutlookViewItemEventHandler(object sender, OutlookViewItemEventArgs args);

    public class OutlookViewItemEventArgs : EventArgs
    {
        private RadPageViewOutlookItem item;

        public OutlookViewItemEventArgs(RadPageViewOutlookItem item)
        {
            this.item = item;
        }

        public RadPageViewOutlookItem Item
        {
            get
            {
                return this.item;
            }
        }
    }
}
