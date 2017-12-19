using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadListDataItemAccessibleObject : AccessibleObject
    {
        #region Fields

        private RadListDataItem item;

        #endregion

        #region Constructors

        public RadListDataItemAccessibleObject(RadListDataItem item)
        {
            this.item = item;
        }

        #endregion

        #region Properties

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.ListItem;
            }
        }

        public override AccessibleStates State
        {
            get
            {

                AccessibleStates states = AccessibleStates.Selectable | AccessibleStates.Focusable;

                if (this.SelectedDataItem == this.item)
                {
                    states |= AccessibleStates.Focused | AccessibleStates.Selected;
                }

                if (this.item.VisualItem != null && this.item.VisualItem.ContainsMouse)
                {
                    states |= AccessibleStates.HotTracked;
                }

                if (this.Bounds == Rectangle.Empty)
                {
                    states |= AccessibleStates.Offscreen;
                }

                return states;
            }
        }

        public override string Description
        {
            get
            {
                return "This is a list item named: " + item.Text;
            }
        }

        public override string Help
        {
            get
            {
                return "Currently there is no help available";
            }
        }

        public override AccessibleObject Parent
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.item.OwnerControl.AccessibilityObject;
            }
        }

        public override string Value
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.item.Text;
            }
        }

        public override string Name
        {
            get
            {
                return item.Text;
            }
            set
            {
                item.Text = value;
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                if (this.item.VisualItem != null)
                {
                    Point location = this.item.OwnerControl.PointToScreen(this.item.VisualItem.ControlBoundingRectangle.Location);
                    return new Rectangle(location, this.item.VisualItem.ControlBoundingRectangle.Size);
                }

                return Rectangle.Empty;
            }
        }

        protected RadListDataItem SelectedDataItem
        {
            get
            {
                if (this.item.OwnerControl.IsDisposed)
                {
                    return null;
                }

                if (this.item.OwnerControl is RadListControl)
                {
                    return (this.item.OwnerControl as RadListControl).SelectedItem;
                }

                if (this.item.OwnerControl is RadDropDownList)
                {
                    return (this.item.OwnerControl as RadDropDownList).SelectedItem;
                }

                return null;
            }

            set
            {
                if (this.item.OwnerControl is RadListControl)
                {
                    (this.item.OwnerControl as RadListControl).SelectedItem = value;
                }

                if (this.item.OwnerControl is RadDropDownList)
                {
                    (this.item.OwnerControl as RadDropDownList).SelectedItem = value;
                }
            }
        }

        #endregion

        #region Methods

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void DoDefaultAction()
        {
            this.SelectedDataItem = this.item;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void Select(AccessibleSelection flags)
        {
            switch (flags)
            {
                case AccessibleSelection.AddSelection:
                case AccessibleSelection.ExtendSelection:
                case AccessibleSelection.TakeSelection:
                case AccessibleSelection.TakeFocus:
                    this.SelectedDataItem = this.item;
                    break;

                case AccessibleSelection.RemoveSelection:
                    break;
            }

            base.Select(flags);
        }

        #endregion
    }
}
