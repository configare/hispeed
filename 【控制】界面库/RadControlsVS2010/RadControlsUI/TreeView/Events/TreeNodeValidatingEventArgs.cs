using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeValidatingEventHandler(object sender, TreeNodeValidatingEventArgs e);

    public class TreeNodeValidatingEventArgs: RadTreeViewCancelEventArgs
    {
        private object oldValue;
        private object newValue;

        public TreeNodeValidatingEventArgs(TreeNodeElement nodeElement, object oldValue, object newValue)
            : base(nodeElement.Data)
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
