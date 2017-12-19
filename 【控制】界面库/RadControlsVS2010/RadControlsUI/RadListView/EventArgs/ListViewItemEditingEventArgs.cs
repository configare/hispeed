using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemEditingEventHandler(object sender, ListViewItemEditingEventArgs e);

    public class ListViewItemEditingEventArgs : ListViewItemCancelEventArgs
    { 
        private IValueEditor editor;

        public ListViewItemEditingEventArgs(ListViewDataItem item, IValueEditor editor) : base(item)
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
