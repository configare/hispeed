using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridMouseEventHandler(object sender, PropertyGridMouseEventArgs e);

    public class PropertyGridMouseEventArgs : RadPropertyGridEventArgs
    { 
        private MouseEventArgs originalEventArgs;

        public MouseEventArgs OriginalEventArgs
        {
            get
            {
                return this.originalEventArgs;
            }
        }

        public PropertyGridMouseEventArgs(PropertyGridItemBase item)
        : base(item)
        {
        }

        public PropertyGridMouseEventArgs(PropertyGridItemBase item, MouseEventArgs originalArgs)
        : base(item)
        {
            this.originalEventArgs = originalArgs;
        }
    }
}