using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void ViewTypeChangingEventHandler(object sender, ViewTypeChangingEventArgs e);

    public class ViewTypeChangingEventArgs : CancelEventArgs
    {
        ListViewType newViewType, oldViewType;

        public ViewTypeChangingEventArgs(ListViewType oldViewType, ListViewType newViewType)
        {
            this.newViewType = newViewType;
            this.oldViewType = oldViewType;
        }

        public ListViewType NewViewType
        {
            get
            {
                return newViewType;
            } 
        }

        public ListViewType OldViewType
        {
            get
            {
                return oldViewType;
            } 
        }
    }
}
