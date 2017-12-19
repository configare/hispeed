using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadMenuItemAccessibleObject : AccessibleObject, IDisposable
    {
        #region Fields

        private RadMenuItemBase owner;
        private bool disposed = false;

        #endregion

        #region Constructor

        public RadMenuItemAccessibleObject(RadMenuItemBase owner)
        {
            this.owner = owner;
            this.owner.MouseHover += new EventHandler(Owner_MouseHover);
            this.owner.DropDownOpened += new EventHandler(Owner_DropDownOpened);
            this.owner.DropDownClosed += new RadPopupClosedEventHandler(Owner_DropDownClosed);
        }

        private void Owner_DropDownOpened(object sender, EventArgs e)
        {
            Control.ControlAccessibleObject accesibilityObject = this.owner.DropDown.AccessibilityObject as Control.ControlAccessibleObject;
            accesibilityObject.NotifyClients(AccessibleEvents.SystemMenuPopupStart);
        }

        private void Owner_DropDownClosed(object sender, RadPopupClosedEventArgs args)
        {
            Control.ControlAccessibleObject accesibilityObject = this.owner.DropDown.AccessibilityObject as Control.ControlAccessibleObject;
            accesibilityObject.NotifyClients(AccessibleEvents.SystemMenuPopupEnd);
        }

        private void Owner_MouseHover(object sender, EventArgs e)
        {
            Control.ControlAccessibleObject accesibilityObject = this.owner.ElementTree.Control.AccessibilityObject as Control.ControlAccessibleObject;
            accesibilityObject.NotifyClients(AccessibleEvents.Focus);
        }

        ~RadMenuItemAccessibleObject()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            this.owner.MouseHover -= new EventHandler(Owner_MouseHover);
            this.owner.DropDownOpened -= new EventHandler(Owner_DropDownOpened);
            this.owner.DropDownClosed -= new RadPopupClosedEventHandler(Owner_DropDownClosed);
        }

        #endregion

        #region Properties

        public override Rectangle Bounds
        {
            get
            {
                Point location = this.owner.ElementTree.Control.PointToScreen(this.owner.ControlBoundingRectangle.Location);
                Rectangle bounds = new Rectangle(location, this.owner.Size);
                return bounds;
            }
        }

        public override string Description
        {
            get
            {
                return this.owner.AccessibleDescription;
            }
        }

        public override string Name
        {
            get
            {
                return this.owner.AccessibleName;
            }
            set
            {
                this.owner.AccessibleName = value;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.MenuItem;
            }
        }

        public override AccessibleStates State
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                if (!this.owner.Enabled)
                {
                    return AccessibleStates.Unavailable;
                }

                AccessibleStates state = AccessibleStates.Focusable | AccessibleStates.Offscreen;

                if (owner.ContainsMouse)
                {
                    state |= AccessibleStates.HotTracked | AccessibleStates.Focused | AccessibleStates.Selected;
                }

                RadMenuItem menuItem = this.owner as RadMenuItem;

                if (menuItem != null && menuItem.IsChecked)
                {
                    state |= AccessibleStates.Checked;
                }


                return state;
            }
        }

        public override AccessibleObject Parent
        {
            [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                if (this.owner.IsOnDropDown)
                {
                    RadDropDownMenu dropDownMenu = this.owner.ElementTree.Control as RadDropDownMenu;
                    RadMenuItemBase menuItem = dropDownMenu.OwnerElement as RadMenuItemBase;

                    if (menuItem != null)
                    {
                        return menuItem.AccessibleObject;
                    }

                    if (dropDownMenu.OwnerElement != null)
                    {
                        return dropDownMenu.OwnerElement.ElementTree.Control.AccessibilityObject;
                    }
                }

                return this.owner.ElementTree.Control.AccessibilityObject;
            }
        }

        #endregion

        #region Methods

        public override void DoDefaultAction()
        {
            this.owner.PerformClick();
        }

        public override int GetChildCount()
        {
            if (this.owner == null)
            {
                return -1;
            }

            return this.owner.Items.Count;
        }

        public override AccessibleObject GetChild(int index)
        {
            RadDropDownMenu dropDownMenu = this.owner.DropDown;
            if (dropDownMenu != null && !dropDownMenu.IsLoaded)
            {
                dropDownMenu.LoadElementTree();
                dropDownMenu.SetTheme();
                dropDownMenu.RootElement.InvalidateMeasure(true);
                dropDownMenu.RootElement.UpdateLayout();
            }

            RadMenuItemBase menuItem = this.owner.Items[index] as RadMenuItemBase;
            return menuItem.AccessibleObject;
        }

        #endregion

    }

}
