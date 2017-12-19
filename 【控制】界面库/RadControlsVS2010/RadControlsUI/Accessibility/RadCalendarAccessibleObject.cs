using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadCalendarAccessibleObject : Control.ControlAccessibleObject
    {
        private RadCalendar owner;
        public RadCalendarAccessibleObject(RadCalendar owner): base(owner)
        {
            this.owner = owner;
            this.owner.SelectionChanged += new EventHandler(owner_SelectionChanged);
        }

        public RadCalendar Owner
        {
            get
            {
                return owner;
            }            
        }

        void owner_SelectionChanged(object sender, EventArgs e)
        {
            this.NotifyClients(AccessibleEvents.Focus, 0);
        }

        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.Table;
            }
        }

         [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override int GetChildCount()
        {
            return this.owner.CalendarElement.View.Rows * this.owner.CalendarElement.View.Columns ;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override AccessibleObject GetChild(int index)
        {
            return new RadCalendarCellAccessibleObject(this, index / this.owner.CalendarElement.View.Columns, index % this.owner.CalendarElement.View.Columns);
        }
    }

    public class RadCalendarCellAccessibleObject : AccessibleObject
    {
        private RadCalendarAccessibleObject ownerAccessibleObject;
        int column;
        int row;
        public RadCalendarCellAccessibleObject(RadCalendarAccessibleObject ownerAccessibleObject, int column, int row)           
        {
            this.ownerAccessibleObject = ownerAccessibleObject;
            this.column = column;
            this.row = row;
        }

        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.Cell;
            }
        }

        public override string Name
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.ownerAccessibleObject.Owner.SelectedDate.ToString();
            }
            set
            {
                //this.cell.Value = value;
            }
        }

        public override string Description
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.Name;
            }
        }

        //public override string Help
        //{
        //    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        //    get
        //    {
        //        return "Currently there is no help available";
        //    }
        //}

        public override AccessibleObject Parent
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this.ownerAccessibleObject;
            }
        }

        public override AccessibleStates State
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                AccessibleStates states = AccessibleStates.Selectable | AccessibleStates.Focusable;
                if (this.ownerAccessibleObject.Owner.CurrentViewColumn == this.column &&
                this.ownerAccessibleObject.Owner.CurrentViewRow == this.row)
                {
                    states |= AccessibleStates.Focused | AccessibleStates.Selected;
                }

                return states;
            }
        }

    }
}
