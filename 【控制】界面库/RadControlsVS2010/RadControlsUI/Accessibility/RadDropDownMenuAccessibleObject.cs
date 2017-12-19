using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadDropDownMenuAccessibleObject : Control.ControlAccessibleObject
    {
        #region Fields

        private RadDropDownMenu owner;

        #endregion

        #region Constructor

        public RadDropDownMenuAccessibleObject(RadDropDownMenu owner) :
            base(owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.MenuPopup;
            }
        }

        public override string Name
        {
            get
            {
                if (this.owner.OwnerElement==null)
                {
                    return "DropDown";
                }

                RadItem baseItem = this.owner.OwnerElement as RadItem;                
                return baseItem.AccessibleName + "DropDown";
            }
            set
            {
                RadItem baseItem = this.owner.OwnerElement as RadItem;
                baseItem.AccessibleName = value;
            }
        }

        #endregion

        #region Methods

        public override int GetChildCount()
        {
            return owner.Items.Count;
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point point = this.owner.PointToClient(new Point(x, y));
            RadMenuItemBase menuItem = this.owner.ElementTree.GetElementAtPoint(point) as RadMenuItemBase;

            if (menuItem != null)
            {
                return menuItem.AccessibleObject;
            }

            return base.HitTest(x, y);
        }

        public override AccessibleObject GetChild(int index)
        {
            RadMenuItemBase menuItem = this.owner.Items[index] as RadMenuItemBase;
            return menuItem.AccessibleObject;
        }

        #endregion
    }
}
