using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemEditorInitializedEventHandler(object sender, ListViewItemEditorInitializedEventArgs e);

    public class ListViewItemEditorInitializedEventArgs : ListViewVisualItemEventArgs
    { 
        private IValueEditor editor;

        public ListViewItemEditorInitializedEventArgs(BaseListViewVisualItem visualItem, IValueEditor editor) : base(visualItem)
        { 
            this.editor = editor;
        }

        public IValueEditor Editor
        {
            get
            {
                return editor;
            }
        }
    }
}
