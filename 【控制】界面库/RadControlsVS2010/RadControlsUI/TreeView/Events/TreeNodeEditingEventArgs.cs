using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeEditingEventHandler(object sender, TreeNodeEditingEventArgs e);

    public class TreeNodeEditingEventArgs: RadTreeViewCancelEventArgs
    {
        IValueEditor editor;

        public TreeNodeEditingEventArgs(RadTreeNode node, IValueEditor editor)
            : base(node)
        {
            this.editor = editor;
        }

        public IValueEditor Editor
        {
            get { return this.editor; }
        }
    }
}
