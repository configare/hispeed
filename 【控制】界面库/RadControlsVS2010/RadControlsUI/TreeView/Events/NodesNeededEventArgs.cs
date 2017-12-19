using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void NodesNeededEventHandler(object sender, NodesNeededEventArgs e);

    public class NodesNeededEventArgs : EventArgs
    {
        private RadTreeNode parent;
        private IList<RadTreeNode> nodes;

        public NodesNeededEventArgs(RadTreeNode parent, IList<RadTreeNode> nodes)
        {
            this.parent = parent;
            this.nodes = nodes;
        }

        public RadTreeNode Parent
        {
            get { return parent; }
        }
        
        public IList<RadTreeNode> Nodes
        {
            get { return nodes; }
        }
    }
}
