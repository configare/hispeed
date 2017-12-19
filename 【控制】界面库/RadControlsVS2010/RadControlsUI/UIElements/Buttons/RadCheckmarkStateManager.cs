using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadCheckmarkStateManager : ItemStateManagerBase
    {
        public RadCheckmarkStateManager()
        {
            this.AddDefaultVisibleState("ToggleState=On");
            this.AddDefaultVisibleState("ToggleState=Indeterminate");
            this.AddDefaultVisibleState("Disabled");
        }

        protected override void AttachToItemOverride(StateManagerAttachmentData attachData, RadItem item)
        {
            List<RadProperty> list = new List<RadProperty>(2);
            list.Add(RadCheckmark.CheckStateProperty);
            list.Add(RadElement.EnabledProperty);
            attachData.AddEventHandlers(list);
        }

        public override void ItemStateChanged(RadItem item, RadPropertyChangedEventArgs changeArgs)
        {
            string state = string.Empty;
            if (!item.Enabled)
            {
                state = "Disabled";
            }

            RadCheckmark check = item as RadCheckmark;
            if (check.CheckState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                if (!string.IsNullOrEmpty(state))
                {
                    state += ItemStateManagerBase.stateDelimiter;
                }
                state += "ToggleState=On";
            }
            if (check.CheckState == Telerik.WinControls.Enumerations.ToggleState.Indeterminate)
            {                
                if (!string.IsNullOrEmpty(state))
                {
                    state += ItemStateManagerBase.stateDelimiter;
                }
                state += "ToggleState=Indeterminate";
            }

            base.SetItemState(item, state);
        }

        public override StateDescriptionNode GetAvailableStates(string itemRoleName)
        {
            StateDescriptionNode node = new StateDescriptionNode(itemRoleName);
            node.AddNode("ToggleState=On");
            node.AddNode("ToggleState=Indeterminate");
            node.AddNode("Disabled");

            return node;
        }
    }
}
