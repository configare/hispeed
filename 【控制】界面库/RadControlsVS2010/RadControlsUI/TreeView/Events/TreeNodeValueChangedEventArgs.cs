using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeValueChangedEventHandler(object sender, TreeNodeValueChangedEventArgs e);

    public class TreeNodeValueChangedEventArgs: RadTreeViewEventArgs
    {
        public TreeNodeValueChangedEventArgs(RadTreeNode node): base(node)
        { 
        }
    }
}
