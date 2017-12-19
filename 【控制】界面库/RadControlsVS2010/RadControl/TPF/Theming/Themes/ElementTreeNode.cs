using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls;


namespace Telerik.WinControls.Tools
{
    public class ElementTreeNode : TreeNode
    {
        private RadElement element = null;

        public ElementTreeNode(RadElement element, string text)
        {
			 
            this.Element = element;
            this.Text = element.GetType().Name + text;
        }

        public ElementTreeNode(RadElement element)
        {
            this.Element = element;
            this.Text = element.GetType().Name;
        }

        public RadElement Element
        {
            get { return element; }
            set { element = value; }
        }
    }
}
