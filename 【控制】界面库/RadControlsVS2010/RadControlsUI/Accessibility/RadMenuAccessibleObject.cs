using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadMenuAccessibleObject : Control.ControlAccessibleObject
    {
        #region Fields

        private RadMenu owner;

        #endregion

        #region Constructor

        public RadMenuAccessibleObject(RadMenu owner)
            : base(owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties

        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                AccessibleRole role = base.Owner.AccessibleRole;

                if (role != AccessibleRole.Default)
                {
                    return role;
                }

                Trace.WriteLine("RadMenuAccessibleObject GetRole: " + AccessibleRole.MenuBar);

                return AccessibleRole.MenuBar;
            }
        }

        #endregion

        #region Methods

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override int GetChildCount()
        {
            return owner.Items.Count;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject GetChild(int index)
        {
            RadMenuItemBase menuItem = this.owner.Items[index] as RadMenuItemBase;
            return menuItem.AccessibleObject;
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point point = this.owner.PointToClient(new Point(x, y));
            RadMenuItemBase itemAt = this.owner.ElementTree.GetElementAtPoint(point) as RadMenuItemBase;

            if (itemAt != null)
            {
                return itemAt.AccessibleObject;
            }

            return base.HitTest(x, y);
        }

        #endregion
    }
}
