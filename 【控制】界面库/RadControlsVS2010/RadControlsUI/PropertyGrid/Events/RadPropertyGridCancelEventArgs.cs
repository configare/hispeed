using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void RadPropertyGridCancelEventHandler(object sender, RadPropertyGridCancelEventArgs e);

    public class RadPropertyGridCancelEventArgs : CancelEventArgs
    {
        private PropertyGridItemBase item;

        public RadPropertyGridCancelEventArgs(PropertyGridItemBase item)
            : base()
        {
            this.item = item;
        }

        public RadPropertyGridCancelEventArgs(PropertyGridItemBase item, bool cancel)
            : base(cancel)
        {
            this.item = item;
        }

        public PropertyGridItemBase Item
        {
            get
            {
                return this.item;
            }
        }
    }
}
