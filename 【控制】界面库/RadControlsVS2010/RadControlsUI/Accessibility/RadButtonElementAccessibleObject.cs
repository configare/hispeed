using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadButtonElementAccessibleObject : AccessibleObject
    {
        RadButtonItem owner;

        public RadButtonElementAccessibleObject(RadButtonItem owner)
        {
            this.owner = owner;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void DoDefaultAction()
        {
            owner.CallDoClick(EventArgs.Empty);
        }

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = base.State;
                if (owner.IsMouseDown)
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
                return owner.Text;
            }
        }

        public override string Name
        {
            get
            {
                return owner.Name;
            }
            set
            {
                owner.Name = value;
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
