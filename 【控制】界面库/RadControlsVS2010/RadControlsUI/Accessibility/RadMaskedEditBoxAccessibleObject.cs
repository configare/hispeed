using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadMaskedEditBoxAccessibleObject : Control.ControlAccessibleObject
    {
        protected RadMaskedEditBox owner = null;

        public RadMaskedEditBoxAccessibleObject(RadMaskedEditBox owner):base(owner)
        {
            this.owner = owner;
            this.owner.MaskedEditBoxElement.TextChanged += new EventHandler(owner_TextChanged);
        }

        void owner_TextChanged(object sender, EventArgs e)
        {
            this.NotifyClients(AccessibleEvents.Focus);
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
