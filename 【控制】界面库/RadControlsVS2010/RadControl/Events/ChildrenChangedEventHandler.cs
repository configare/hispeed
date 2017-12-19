using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public delegate void ChildrenChangedEventHandler(object sender, ChildrenChangedEventArgs e);

    public class ChildrenChangedEventArgs
    {
        public readonly RadElement Child;
        public readonly ItemsChangeOperation ChangeOperation;

        public ChildrenChangedEventArgs(RadElement child, ItemsChangeOperation changeOperation)
        {
            this.Child = child;
            this.ChangeOperation = changeOperation;
        }
    }
}
