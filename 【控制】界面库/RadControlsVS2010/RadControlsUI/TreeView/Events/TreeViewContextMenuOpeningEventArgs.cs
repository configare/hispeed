using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeViewContextMenuOpeningEventHandler(object sender, TreeViewContextMenuOpeningEventArgs e);

    public class TreeViewContextMenuOpeningEventArgs: RadTreeViewCancelEventArgs
    {
        private RadContextMenu menu;

        public TreeViewContextMenuOpeningEventArgs(RadTreeNode node, RadContextMenu contextMenu)
            : base(node)
        {
            this.menu = contextMenu;
        }

        public RadContextMenu Menu
        {
            get { return menu; }
        }
    }
}
