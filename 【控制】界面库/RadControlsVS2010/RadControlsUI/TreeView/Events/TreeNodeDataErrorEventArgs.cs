using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void TreeNodeDataErrorEventHandler(object sender, TreeNodeDataErrorEventArgs e);

    public class TreeNodeDataErrorEventArgs : RadTreeViewEventArgs
    {
        private string errorText;

        public TreeNodeDataErrorEventArgs(RadTreeNode node)
            :base(node)
        {

        }

        public TreeNodeDataErrorEventArgs(string errorText, RadTreeNode node)
            : base(node)
        {
            this.errorText = errorText;
        }

        /// <summary>
        /// Gets the error text.
        /// </summary>
        /// <value>The error text.</value>
        public string ErrorText
        {
            get
            {
                return this.errorText;
            }
        }
    }
}
