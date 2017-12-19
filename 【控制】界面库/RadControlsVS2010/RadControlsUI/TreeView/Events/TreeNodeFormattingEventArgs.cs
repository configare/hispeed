using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeFormattingEventHandler(object sender, TreeNodeFormattingEventArgs e);

    public class TreeNodeFormattingEventArgs : RadTreeViewNodeElementEventArgs
    {
        public TreeNodeFormattingEventArgs(TreeNodeElement nodeElement)
            : base(nodeElement)
        {
        }
    }
}
