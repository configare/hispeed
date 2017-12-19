using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ToggleButtonStateManagerFactory: ItemStateManagerFactory
    {
        public override StateNodeBase CreateRootState()
        {
            StateNodeWithCondition enabledState = new StateNodeWithCondition("Enabled", new SimpleCondition(RadElement.EnabledProperty, true));
            enabledState.TrueStateLink = this.CreateSpecificStates();
            enabledState.FalseStateLink = new StatePlaceholderNode("Disabled");

            return enabledState;
        }

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition mouseStateTree = new StateNodeWithCondition("Pressed", new SimpleCondition(RadButtonItem.IsPressedProperty, true));
            mouseStateTree.FalseStateLink = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));

            CompositeStateNode toggleButtonStates = new CompositeStateNode("ToggleState");

            StateNodeWithCondition toggleStateOn = new StateNodeWithCondition("ToggleState=On", new SimpleCondition(RadToggleButtonElement.ToggleStateProperty, Enumerations.ToggleState.On));
            toggleButtonStates.AddState(toggleStateOn);

            toggleButtonStates.AddState(mouseStateTree);

            toggleStateOn.FalseStateLink = new StateNodeWithCondition("ToggleState=Intermediate", new SimpleCondition(RadToggleButtonElement.ToggleStateProperty, Enumerations.ToggleState.Indeterminate));

            return toggleButtonStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager res)
        {
            res.AddDefaultVisibleState("MouseOver");
            res.AddDefaultVisibleState("Pressed");

            res.AddDefaultVisibleState("ToggleState=On");
            res.AddDefaultVisibleState("ToggleState=On.MouseOver");
            res.AddDefaultVisibleState("ToggleState=On.Pressed");

            res.AddDefaultVisibleState("ToggleState=Intermediate");
            res.AddDefaultVisibleState("ToggleState=Intermediate.MouseOver");
            res.AddDefaultVisibleState("ToggleState=Intermediate.Pressed");

            res.AddDefaultVisibleState("Disabled");
        }
    }
}
