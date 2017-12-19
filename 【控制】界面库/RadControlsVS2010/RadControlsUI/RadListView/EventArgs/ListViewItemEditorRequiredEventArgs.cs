using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemEditorRequiredEventHandler(object sender, ListViewItemEditorRequiredEventArgs e);

    public class ListViewItemEditorRequiredEventArgs: EditorRequiredEventArgs
    {
        ListViewDataItem item;

        public ListViewItemEditorRequiredEventArgs(ListViewDataItem item, Type editorType)
            : base(editorType)
        {
            this.item = item;
        }

        public ListViewDataItem Item
        {
            get { return this.item; }
        }

        public RadListViewElement ListViewElement
        {
            get { return this.item.Owner; }
        } 
    }
}
