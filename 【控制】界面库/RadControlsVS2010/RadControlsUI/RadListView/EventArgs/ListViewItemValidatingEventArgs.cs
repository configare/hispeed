using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void ListViewItemValidatingEventHandler(object sender, ListViewItemValidatingEventArgs e);

    public class ListViewItemValidatingEventArgs : ListViewItemCancelEventArgs
    {
        private object oldValue;
        private object newValue;

        public ListViewItemValidatingEventArgs(BaseListViewVisualItem visualItem, object oldValue, object newValue)
            : base(visualItem.Data)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public object OldValue
        {
            get 
            {
                return oldValue; 
            }
        }

        public object NewValue
        {
            get 
            {
                return newValue; 
            }
            set
            {
                newValue = value;
            }
        }
    }
}
