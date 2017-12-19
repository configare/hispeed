using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeEditorInitializedEventHandler(object sender, TreeNodeEditorInitializedEventArgs e);

    public class TreeNodeEditorInitializedEventArgs: RadTreeViewNodeElementEventArgs
    {
        IValueEditor editor;

        public TreeNodeEditorInitializedEventArgs(TreeNodeElement nodeElement, IValueEditor editor)
            : base(nodeElement)
        {
            this.editor = editor;
        }

        public IValueEditor Editor
        {
            get { return this.editor; }
        }
    }
}
