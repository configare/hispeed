using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadSpinEditorAccessibleObject : Control.ControlAccessibleObject
    {
        RadSpinEditor owner;
        // Methods
        public RadSpinEditorAccessibleObject(RadSpinEditor owner)
            : base(owner)
        {
            this.owner = owner;
        }

        public override AccessibleObject GetChild(int index)
        {
            if ((index >= 0) && (index < this.GetChildCount()))
            {
                if (index == 0)
                {
                    return new RadTextBoxElementAccessibleObject(owner.SpinElement.TextBoxItem);
                }
                if (index == 1)
                {
                    return new RadButtonElementAccessibleObject(owner.SpinElement.ButtonUp);
                }
                if (index == 2)
                {
                    return new RadButtonElementAccessibleObject(owner.SpinElement.ButtonDown);
                }
            }
            return null;
        }

        public override int GetChildCount()
        {
            return 3;
        }

        // Properties
        public override AccessibleRole Role
        {
            get
            {
                AccessibleRole accessibleRole = base.Owner.AccessibleRole;
                if (accessibleRole != AccessibleRole.Default)
                {
                    return accessibleRole;
                }
                return AccessibleRole.ComboBox;
            }
        }
    }
}
