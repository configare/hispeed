using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    internal class RadDropDownButtonAccessibleObject : Control.ControlAccessibleObject
    {
        #region Constructor

        public RadDropDownButtonAccessibleObject(RadDropDownButton owner)
            : base(owner)
        {

        }

        #endregion

        #region Properties

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.ButtonDropDown;
            }
        }

        #endregion

        #region Methods

        public override int GetChildCount()
        {
            return 1;
        }

        public override AccessibleObject GetChild(int index)
        {
            RadDropDownButton button = this.Owner as RadDropDownButton;

            if (button is RadApplicationMenu)
            {
                return new RadApplicationMenuButtonElementAccessibleObject(button.DropDownButtonElement as RadApplicationMenuButtonElement);
            }

            return new RadDropDownButtonElementAccessibleObject(button.DropDownButtonElement);
        }

        #endregion
    }


}
