using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadCommandBarItemAccessibleObject : AccessibleObject
    {
        private RadCommandBarBaseItem owner;
        private RadCommandBarAccessibleObject parent;

        public RadCommandBarItemAccessibleObject(RadCommandBarBaseItem owner, RadCommandBarAccessibleObject parent)
        {
            this.owner = owner;
            this.parent = parent;
            this.owner.MouseDown += new MouseEventHandler(owner_MouseDown);
        }

        void owner_MouseDown(object sender, MouseEventArgs e)
        {
            this.parent.NotifyClients(AccessibleEvents.Focus);
        }

        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.PushButton;
            }
        }

        public override string Name
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return owner.Name;
            }
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            set
            {
                owner.Name = value;
            }
        }

        public override AccessibleStates State
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                AccessibleStates state = AccessibleStates.Focusable;
                if (this.owner.ContainsMouse)
                {
                    state |= AccessibleStates.Focused;
                }

                return state;
            }
        }

        public override AccessibleObject Parent
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.parent;
            }
        }
    }
}
