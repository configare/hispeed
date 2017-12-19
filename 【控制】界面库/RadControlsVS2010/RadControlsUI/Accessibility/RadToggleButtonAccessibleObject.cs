using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    [ComVisible(true)]
    public class RadToggleButtonAccessibleObject : Control.ControlAccessibleObject
    {
        // Methods
        public RadToggleButtonAccessibleObject(RadToggleButton owner)
            : base(owner)
        {
            owner.ToggleStateChanged += new StateChangedEventHandler(owner_ToggleStateChanged);
        }

        void owner_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            this.NotifyClients(AccessibleEvents.Focus);
        }

        public override AccessibleRole Role
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return AccessibleRole.CheckButton;
            }
        }



        public override string Name
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return base.Owner.Text;
            }
            set
            {
                base.Owner.Text = value;
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

        public override AccessibleStates State
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                AccessibleStates result = base.State;
                switch (((RadToggleButton)base.Owner).ToggleState)
                {
                    case ToggleState.On:
                        result |= AccessibleStates.Checked;
                        break;

                    case ToggleState.Indeterminate:
                        result |= AccessibleStates.Mixed;
                        break;
                }

                if (((RadToggleButton)base.Owner).RootElement.IsMouseDown)
                {
                    result |= AccessibleStates.Pressed;
                }

                return result;
            }
        }
    }
}
