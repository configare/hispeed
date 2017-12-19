using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewAccessibleObject : Control.ControlAccessibleObject
    {
        #region Fields
        private RadTreeView owner;
        #endregion

        #region Overrides

        public RadTreeViewAccessibleObject(RadTreeView owner)
            : base(owner)
        {
            this.owner = owner;
            this.owner.SelectedNodeChanged += owner_SelectedNodeChanged;
        }

        void owner_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            this.NotifyClients(AccessibleEvents.Focus | AccessibleEvents.Selection, 0); 
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Outline;
            }
        }

        public override int GetChildCount()
        {
            //return this.GetChildCount(this.owner.Nodes);
            return this.owner.Nodes.Count;
        }

        private int GetChildCount(RadTreeNodeCollection nodes)
        {
            int result = 0;

            foreach (RadTreeNode node in nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    result += this.GetChildCount(node.Nodes);
                }
            }

            return result;
        }


        public override AccessibleObject GetChild(int index)
        {
            //return this.GetChild(this.owner.Nodes, index);
            return new RadTreeNodeAccessibleObject(this.owner.Nodes[index]);
        }

        private AccessibleObject GetChild(RadTreeNodeCollection nodes, int index)
        {
            if (index > nodes.Count)
            {
                foreach (RadTreeNode node in nodes)
                {
                    if (node.Nodes.Count > 0)
                    {
                        return this.GetChild(node.Nodes, index - nodes.Count - 1);
                    }
                }
            }

            return new RadTreeNodeAccessibleObject(nodes[index]);
        }

        #endregion
    }   
}
