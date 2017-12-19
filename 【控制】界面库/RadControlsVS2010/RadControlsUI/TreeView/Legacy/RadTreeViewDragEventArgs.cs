using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewDragEventArgs : RadTreeViewEventArgs
    {
        private RadTreeNode targetNode;
        public RadTreeViewDragEventArgs(RadTreeNode node, RadTreeNode targetNode)
            : base(node)
        {
            this.targetNode = targetNode;
        }

        public RadTreeNode TargetNode
        {
            get
            {
                return this.targetNode;
            }
        }
    }
}
