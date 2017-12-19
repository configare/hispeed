using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadApplicationMenuButtonElementAccessibleObject : RadDropDownButtonElementAccessibleObject
    {
        #region Constructor

        public RadApplicationMenuButtonElementAccessibleObject(RadApplicationMenuButtonElement owner) :
            base(owner)
        {

        }

        #endregion

        #region Methods

        public override int GetChildCount()
        {
            RadApplicationMenuButtonElement owner = this.Owner as RadApplicationMenuButtonElement;
            RadApplicationMenuDropDown dropDown = owner.DropDownMenu as RadApplicationMenuDropDown;
            return base.GetChildCount() + dropDown.RightColumnItems.Count + dropDown.ButtonItems.Count;
        }

        public override AccessibleObject GetChild(int index)
        {
            RadApplicationMenuButtonElement owner = this.Owner as RadApplicationMenuButtonElement;
            RadApplicationMenuDropDown dropDown = owner.DropDownMenu as RadApplicationMenuDropDown;
            int count = base.GetChildCount();

            if (index < count)
            {
                return base.GetChild(index);
            }

            index -= count;
            count = dropDown.RightColumnItems.Count;

            if (index < count)
            {
                RadMenuItemBase menuItemBase = dropDown.RightColumnItems[index] as RadMenuItemBase;
                return menuItemBase.AccessibleObject;
            }

            index -= count;
            RadMenuItemBase button = dropDown.ButtonItems[index] as RadMenuItemBase;
            return button.AccessibleObject;
        }

        #endregion
    }
}
