using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadStatusStripItemAccessibleObject : AccessibleObject
    {
        private RadItem owner;
        private RadStatusStripAccessibleObject parent;

        public RadStatusStripItemAccessibleObject(RadItem owner, RadStatusStripAccessibleObject parent)
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
                return AccessibleRole.StaticText;
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
