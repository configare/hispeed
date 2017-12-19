using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadTreeNodeAccessibleObject : AccessibleObject
    {
        RadTreeNode owner;

        public RadTreeNodeAccessibleObject(RadTreeNode owner)
        {
            this.owner = owner;
        }

        public override int GetChildCount()
        {
            return this.owner.Nodes.Count;
        }

        public override AccessibleObject GetChild(int index)
        {
            return new RadTreeNodeAccessibleObject(this.owner.Nodes[index]);
        }

        public override AccessibleObject Parent
        {
            get
            {
                if (this.owner.Parent == null)
                {                   
                    return this.owner.TreeView.AccessibilityObject;
                }
                                
                return new RadTreeNodeAccessibleObject(this.owner.Parent);
            }
        }

        public override System.Drawing.Rectangle Bounds
        {
            get
            {
                TreeNodeElement nodeElement = this.owner.TreeViewElement.GetElement(this.owner);
                if (nodeElement != null)
                {
                    Point location = this.owner.TreeView.PointToScreen(nodeElement.ControlBoundingRectangle.Location);
                    return new Rectangle(location, nodeElement.Size);
                }

                return Rectangle.Empty;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.OutlineItem;
            }
        }


        public override string Name
        {
            get
            {
                return this.owner.Text;
            }
            set
            {
                base.Name = value;
            }
        }

        public override string Description
        {
            get
            {
                return base.Description;
            }
        }
    }
}
