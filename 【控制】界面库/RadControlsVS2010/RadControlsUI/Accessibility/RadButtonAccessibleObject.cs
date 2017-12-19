using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadButtonAccessibleObject : Control.ControlAccessibleObject
    {
        public RadButtonAccessibleObject(Control owner)
            : base(owner)
        {
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void DoDefaultAction()
        {
            ((RadButton)base.Owner).CallOnClick(EventArgs.Empty);
        }



        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = base.State;
                RadButton owner = (RadButton)base.Owner;
                if (owner.ButtonElement.IsMouseDown)
                {
                    state |= AccessibleStates.Pressed;
                }

                return state;
            }
        }


        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.PushButton;
            }
        }

        public override string Description
        {
            get
            {
                return Owner.Text;
            }
        }

        public override string Name
        {
            get
            {
                return Owner.Name;
            }
            set
            {
                Owner.Name = value;
            }
        }

        //public override AccessibleObject Parent
        //{
        //    get
        //    {
        //        return Owner.Parent.AccessibilityObject;
        //    }
        //}


    }
}
