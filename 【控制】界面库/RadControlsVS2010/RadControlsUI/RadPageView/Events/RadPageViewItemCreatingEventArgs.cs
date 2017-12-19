namespace Telerik.WinControls.UI
{
    public class RadPageViewItemCreatingEventArgs : RadPageViewEventArgs
    {
        private RadPageViewItem item;

        public RadPageViewItemCreatingEventArgs(RadPageViewPage page)
            : base(page)
        {
            this.item = null;
        }

        public RadPageViewItem Item
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }
    }
}