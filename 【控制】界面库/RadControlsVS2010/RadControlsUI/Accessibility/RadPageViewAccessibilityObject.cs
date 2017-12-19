using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadPageViewAccessibilityObject : Control.ControlAccessibleObject
    {
        protected RadPageView ownerPageView;

        public RadPageViewAccessibilityObject(RadPageView owner)
            : base(owner)
        {
            this.ownerPageView = owner;
            this.ownerPageView.SelectedPageChanged += new EventHandler(owner_SelectedPageChanged);
        }

        public RadPageView OwnerPageView
        {
            get
            {
                return this.ownerPageView;
            }
        }

        void owner_SelectedPageChanged(object sender, EventArgs e)
        {
            this.NotifyClients(AccessibleEvents.Focus);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override int GetChildCount()
        {
            return this.ownerPageView.Pages.Count;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject GetChild(int index)
        {
            return new RadPageViewPageAccessibilityObject(this.ownerPageView.Pages[index], this);
        }


        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.PageTabList;
            }
        }
    }
}
