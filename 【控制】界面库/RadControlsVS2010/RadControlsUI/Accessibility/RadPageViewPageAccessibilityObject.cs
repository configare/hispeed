using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadPageViewPageAccessibilityObject : AccessibleObject
    {
        protected RadPageViewPage owner;
        protected RadPageViewAccessibilityObject parent;

        public RadPageViewPageAccessibilityObject(RadPageViewPage owner, RadPageViewAccessibilityObject parent)
        {
            this.owner = owner;
            this.parent = parent;
        }


        public override AccessibleObject Parent
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.parent;
            }
        }

        public override AccessibleStates State
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                AccessibleStates state = AccessibleStates.Focusable;
                if (this.parent.OwnerPageView.SelectedPage == this.owner)
                {
                    state |= AccessibleStates.Focused;
                }

                return state;
            }
        }


        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.PageTab;
            }
        }
    }
}