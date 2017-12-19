using System;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadDropDownButtonElementAccessibleObject : AccessibleObject
    {
        #region Fields

        private RadDropDownButtonElement buttonElement;

        #endregion

        #region Constructor

        public RadDropDownButtonElementAccessibleObject(RadDropDownButtonElement buttonElement)
        {
            this.buttonElement = buttonElement;
            this.buttonElement.DropDownOpened += new EventHandler(buttonElement_DropDownOpened);
            this.buttonElement.DropDownClosed += new EventHandler(buttonElement_DropDownClosed);
        }

        #endregion

        #region Properties

        protected RadDropDownButtonElement Owner
        {
            get
            {
                return this.buttonElement;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.ButtonDropDown;
            }
        }

        public override System.Drawing.Rectangle Bounds
        {
            get
            {
                Point location = this.buttonElement.ControlBoundingRectangle.Location;
                location = this.buttonElement.ElementTree.Control.PointToScreen(location);
                return new Rectangle(location, this.buttonElement.ControlBoundingRectangle.Size);
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                return this.buttonElement.ElementTree.Control.AccessibilityObject;
            }
        }

        #endregion

        #region Event Handler

        private void buttonElement_DropDownOpened(object sender, EventArgs e)
        {
            Control.ControlAccessibleObject accessibilityObject = this.buttonElement.DropDownMenu.AccessibilityObject as Control.ControlAccessibleObject;

            accessibilityObject.NotifyClients(AccessibleEvents.SystemMenuPopupStart);
        }

        private void buttonElement_DropDownClosed(object sender, EventArgs e)
        {
            Control.ControlAccessibleObject accessibilityObject = this.buttonElement.DropDownMenu.AccessibilityObject as Control.ControlAccessibleObject;

            accessibilityObject.NotifyClients(AccessibleEvents.SystemMenuPopupEnd);
        }

        #endregion

        #region Methods

        public override int GetChildCount()
        {
            return this.buttonElement.Items.Count;
        }

        public override AccessibleObject GetChild(int index)
        {
            RadDropDownMenu dropDownMenu = this.buttonElement.DropDownMenu;

            if (dropDownMenu != null && !dropDownMenu.IsLoaded)
            {
                dropDownMenu.LoadElementTree();
                dropDownMenu.SetTheme();
                dropDownMenu.RootElement.InvalidateMeasure(true);
                dropDownMenu.RootElement.UpdateLayout();
            }

            RadMenuItemBase menuItem = this.buttonElement.Items[index] as RadMenuItemBase;
            return menuItem.AccessibleObject;
        }

        #endregion
    }
}
