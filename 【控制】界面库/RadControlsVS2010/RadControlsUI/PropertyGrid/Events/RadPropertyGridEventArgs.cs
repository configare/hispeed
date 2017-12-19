using System;

namespace Telerik.WinControls.UI
{
    public delegate void RadPropertyGridEventHandler(object sender, RadPropertyGridEventArgs e);

    public class RadPropertyGridEventArgs : EventArgs
    {
        protected PropertyGridItemBase item;

        public RadPropertyGridEventArgs(PropertyGridItemBase item)
        : base()
        {
            this.item = item;
        }

        public virtual PropertyGridItemBase Item
        {
            get
            {
                return this.item;
            }
        }
    }
}