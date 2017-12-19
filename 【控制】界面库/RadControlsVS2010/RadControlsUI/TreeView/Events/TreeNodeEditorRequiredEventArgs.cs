using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class TreeNodeEditorRequiredEventArgs: EditorRequiredEventArgs
    {
        RadTreeNode node;

        public TreeNodeEditorRequiredEventArgs(RadTreeNode node, Type editorType)
            : base(editorType)
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
