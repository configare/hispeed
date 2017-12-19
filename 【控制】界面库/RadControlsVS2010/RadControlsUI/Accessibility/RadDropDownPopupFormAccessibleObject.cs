using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Telerik.WinControls.UI.Data;

namespace Telerik.WinControls.UI
{
    public class RadDropDownPopupFormAccessibleObject : Control.ControlAccessibleObject
    {
        #region Constructor

        public RadDropDownPopupFormAccessibleObject(DropDownPopupForm form)
            : base(form)
        {
            form.OwnerDropDownListElement.ListElement.SelectedIndexChanged += new PositionChangedEventHandler(ListElement_SelectedIndexChanged);
        }

        private void ListElement_SelectedIndexChanged(object sender, Data.PositionChangedEventArgs e)
        {
            this.NotifyClients(AccessibleEvents.Selection, e.Position);
        }

        #endregion

        #region Properties

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.List;
            }
        }

        protected DropDownPopupForm DropDown
        {
            get
            {
                return this.Owner as DropDownPopupForm;
            }
        }

        protected RadDropDownListElement List
        {
            get
            {
                return this.DropDown.OwnerElement as RadDropDownListElement;
            }
        }

        #endregion

        #region Methods

        public override int GetChildCount()
        {
            RadDropDownListElement element = this.DropDown.OwnerElement as RadDropDownListElement;
            return element.ListElement.Items.Count;
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point point = this.DropDown.PointToClient(new Point(x, y));
            RadListVisualItem visualItem = this.DropDown.ElementTree.GetElementAtPoint(point) as RadListVisualItem;

            if (visualItem != null)
            {
                return new RadListDataItemAccessibleObject(visualItem.Data);
            }

            return base.HitTest(x, y);
        }

        public override AccessibleObject GetChild(int index)
        {
            RadDropDownListElement element = this.DropDown.OwnerElement as RadDropDownListElement;
            return new RadListDataItemAccessibleObject(element.ListElement.Items[index]);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject Navigate(AccessibleNavigation direction)
        {
            return null;
        }

        #endregion
    }
}
