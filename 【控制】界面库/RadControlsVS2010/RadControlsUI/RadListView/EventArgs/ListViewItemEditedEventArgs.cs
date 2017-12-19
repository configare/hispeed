using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemEditedEventHandler(object sender, ListViewItemEditedEventArgs e);

    public class ListViewItemEditedEventArgs : ListViewVisualItemEventArgs
    {
        IValueEditor editor;
        bool canceled; 

        public ListViewItemEditedEventArgs(BaseListViewVisualItem visualItem, IValueEditor editor, bool canceled) : base(visualItem)
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
