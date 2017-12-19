using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewNodeElementEventArgs : RadTreeViewEventArgs
    {
        TreeNodeElement nodeElement;

        public RadTreeViewNodeElementEventArgs(TreeNodeElement nodeElement)
            : base(nodeElement.Data)
        {
            this.nodeElement = nodeElement;
        }

        public TreeNodeElement NodeElement
        {
            get
            {
                return this.nodeElement;
            }
        }
    }
}
