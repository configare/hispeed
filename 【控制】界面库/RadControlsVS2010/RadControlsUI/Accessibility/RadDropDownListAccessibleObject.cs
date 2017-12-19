using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadDropDownListAccessibleObject : Control.ControlAccessibleObject
    {
        #region Constructor

        public RadDropDownListAccessibleObject(RadDropDownList owner)
            : base(owner)
        {

        }

        #endregion

        #region Properties

        public override AccessibleRole Role
        {
            get
            {
                if (this.Owner.AccessibleRole != AccessibleRole.Default)
                {
                    return this.Owner.AccessibleRole;
                }

                return AccessibleRole.ComboBox;
            }
        }

        protected RadDropDownList List
        {
            get
            {
                return this.Owner as RadDropDownList;
            }
        }

        public override string Name
        {
            get
            {
                return this.List.Name;
            }
            set
            {
                this.List.Name = value;
            }
        }

        #endregion

        #region Methods

        public override AccessibleObject GetChild(int index)
        {
            if (index >= this.List.Items.Count)
            {
                return this.List.DropDownListElement.TextBox.TextBoxItem.HostedControl.AccessibilityObject;
            }

            return new RadListDataItemAccessibleObject(this.List.Items[index]);
        }

        public override int GetChildCount()
        {
            if (this.List.IsDisposed)
            {
                return 0;
            }

            return this.List.Items.Count + 1;
        }

        #endregion
    }
}
