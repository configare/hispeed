using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewMouseEventArgs : RadTreeViewEventArgs
    {
        //fields
        private MouseEventArgs originalEventArgs;

        public MouseEventArgs OriginalEventArgs
        {
            get { return this.originalEventArgs; }
            set { this.originalEventArgs = value; }
        }

        public RadTreeViewMouseEventArgs(RadTreeNode node)
            : base(node)
        {
        }

        public RadTreeViewMouseEventArgs(RadTreeNode node, TreeViewAction action)
            : base(node)
        {
        }

        public RadTreeViewMouseEventArgs(RadTreeNode node, MouseEventArgs originalArgs)
            : base(node)
        {
            this.OriginalEventArgs = originalArgs;
        }
    }
}
