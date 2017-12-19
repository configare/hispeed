using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadCommandBarAccessibleObject : Control.ControlAccessibleObject
    {
        private RadCommandBar owner;

        public RadCommandBarAccessibleObject(RadCommandBar owner)
            : base(owner)
        {
            this.owner = owner;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject GetChild(int index)
        {
            int currentPos = 0;
            foreach (CommandBarRowElement row in owner.Rows)
            {
            	foreach (CommandBarStripElement strip in row.Strips)
                {
                	foreach (RadCommandBarBaseItem item in strip.Items)
                    {
                    	if (index == currentPos)
                        {
                            return new RadCommandBarItemAccessibleObject( item, this);
                        }

                        ++currentPos;
                    }
                }
            }

            return null;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override int GetChildCount()
        {
            int count = 0;
            foreach (CommandBarRowElement row in owner.Rows)
            {
                foreach (CommandBarStripElement strip in row.Strips)
                {
                    count += strip.Items.Count;                    
                }
            }

            return count;
        }

     
        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.ToolBar;
            }
        }
    } 
}
