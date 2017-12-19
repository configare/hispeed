using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeValueChangingEventHandler(object sender, TreeNodeValueChangingEventArgs e);

    public class TreeNodeValueChangingEventArgs: ValueChangingEventArgs
    {
        RadTreeNode node;

        public TreeNodeValueChangingEventArgs(RadTreeNode node, object newValue, object oldValue)
            : base(newValue, oldValue)
        {
            this.node = node;
        }

        public RadTreeNode Node
        {
            get { return this.node; }
        }

        public RadTreeViewElement TreeElement
        {
            get { return this.node.TreeViewElement; }
        }

        public RadTreeView TreeView
        {
            get { return this.node.TreeView; }
        }
    }
}
