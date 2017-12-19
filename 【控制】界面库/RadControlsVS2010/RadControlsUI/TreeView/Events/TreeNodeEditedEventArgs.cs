using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeEditedEventHandler(object sender, TreeNodeEditedEventArgs e);

    public class TreeNodeEditedEventArgs: RadTreeViewNodeElementEventArgs
    {
        IValueEditor editor;
        bool canceled;

        public TreeNodeEditedEventArgs(TreeNodeElement nodeElement, IValueEditor editor, bool canceled)
            : base(nodeElement)
        {
            this.editor = editor;
            this.canceled = canceled;
        }

        public bool Canceled
        {
            get { return this.canceled; }
        }

        public IValueEditor Editor
        {
            get { return this.editor; }
        }
    }
}
