using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemMouseEventHandler(object sender, ListViewItemMouseEventArgs e);

    public class ListViewItemMouseEventArgs : ListViewItemEventArgs
    {
        private MouseEventArgs originalEventArgs;

        public MouseEventArgs OriginalEventArgs
        {
            get { return this.originalEventArgs; }
            set { this.originalEventArgs = value; }
        }

        public ListViewItemMouseEventArgs(ListViewDataItem item, MouseEventArgs originalEventArgs)
            : base(item)
        {
            this.originalEventArgs = originalEventArgs;
        }

    }
}
