using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class EditorElementStateManager : ItemStateManagerFactory
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
            CompositeStateNode enabledStates = new CompositeStateNode("Enabled States");
            StateNodeBase isFocused = new StateNodeWithCondition("ContainsFocus", new SimpleCondition(RadElement.ContainsFocusProperty, true));
            enabledStates.AddState(isFocused);

            StateNodeBase containsMouse = new StateNodeWithCondition("ContainsMouse", new SimpleCondition(RadElement.ContainsMouseProperty, true));
            enabledStates.AddState(containsMouse);

            return enabledStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("ContainsFocus");
            sm.AddDefaultVisibleState("ContainsMouse");
            sm.AddDefaultVisibleState("Disabled");
        }
    }
}
