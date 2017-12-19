using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadTextBoxAccessibleObject : Control.ControlAccessibleObject
    {
        private RadTextBox owner;

        public RadTextBoxAccessibleObject(RadTextBox owner)
            : base(owner)
        {
            this.owner = owner;
        }

        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.Text;
            }
        }


        public override string Name
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return owner.Name;
            }
            set
            {
                owner.Name = value;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override int GetChildCount()
        {
            return 0;
        }

        public override string Description
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.Name;
            }
        }

        public override string Value
        {
            get
            {
                return this.owner.Text;
            }
            set
            {
                this.owner.Text = value;
            }
        }
    }
}
