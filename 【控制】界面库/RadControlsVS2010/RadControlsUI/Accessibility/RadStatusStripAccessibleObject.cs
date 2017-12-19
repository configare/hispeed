using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;


namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadStatusStripAccessibleObject : Control.ControlAccessibleObject
    {
        private RadStatusStrip owner;

        public RadStatusStripAccessibleObject(RadStatusStrip owner)
            : base(owner)
        {
            this.owner = owner;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject GetChild(int index)
        {
            return new RadStatusStripItemAccessibleObject( this.owner.Items[index], this);        
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override int GetChildCount()
        {   
            return this.owner.Items.Count;
        }

     
        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.StatusBar;
            }
        }
    } 
}
