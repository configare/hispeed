using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{



    [ComVisible(true)]
    public class RadRibbonBarItemAccessibleObject : AccessibleObject
    {
        RadItem owner;
        RadRibbonBarAccessibleObject parent;

        public RadRibbonBarItemAccessibleObject(RadItem owner, RadRibbonBarAccessibleObject parent)
        {
            this.owner = owner;
            this.parent = parent;
            this.owner.MouseDown += new MouseEventHandler(owner_MouseDown);
        }

        void owner_MouseDown(object sender, MouseEventArgs e)
        {
            this.parent.NotifyClients(AccessibleEvents.Focus);
        }


        public override string Name
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.owner.Name;
            }
            set
            {
                this.owner.Name = value;
            }
        }


        public override string Description
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return owner.Text;
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