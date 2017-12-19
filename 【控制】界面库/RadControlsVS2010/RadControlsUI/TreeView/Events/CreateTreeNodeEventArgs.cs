using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void CreateTreeNodeEventHandler(object sender, CreateTreeNodeEventArgs e);

    public class CreateTreeNodeEventArgs : RadTreeViewEventArgs
    {
        public CreateTreeNodeEventArgs()
            : base(null)
        {
        }

        public override RadTreeNode Node
        {
            get
            {
                return this.node;
            }
            set
            {
                this.node = value;
            }
        }
    }
}
