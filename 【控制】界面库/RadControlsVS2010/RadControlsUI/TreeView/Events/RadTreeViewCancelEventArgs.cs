using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewCancelEventArgs : CancelEventArgs
    {
        private RadTreeNode node;

        public RadTreeViewCancelEventArgs(RadTreeNode node)
            : base()
        {
            this.node = node;
        }

        public RadTreeViewCancelEventArgs(RadTreeNode node, bool cancel)
            : base(cancel)
        {
            this.node = node;
        }

        public RadTreeNode Node
        {
            get
            {
                return this.node;
            }
        }

        public RadTreeViewElement TreeElement
        {
            get { return node.TreeViewElement; }
        }

        public RadTreeView TreeView
        {
            get { return node.TreeView; }
        }
    }
}
