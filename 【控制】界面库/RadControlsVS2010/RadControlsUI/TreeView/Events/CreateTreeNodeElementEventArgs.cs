using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void CreateTreeNodeElementEventHandler(object sender, CreateTreeNodeElementEventArgs e);

    public class CreateTreeNodeElementEventArgs : RadTreeViewEventArgs
    {
        TreeNodeElement nodeElement;

        public CreateTreeNodeElementEventArgs(RadTreeNode node)
            : base(node)
        {
        }

        public TreeNodeElement NodeElement
        {
            get
            {
                return this.nodeElement;
            }
            set
            {
                this.nodeElement = value;
            }
        }
    }

}
