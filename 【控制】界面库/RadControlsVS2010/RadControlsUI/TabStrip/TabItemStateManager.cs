using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class TabItemStateManagerFactory: ItemStateManagerFactory
    {
        public override StateNodeBase CreateRootState()
        {
            StateNodeWithCondition enabledState = new StateNodeWithCondition("Enabled", new SimpleCondition(RadElement.EnabledProperty, true));

            CompositeStateNode tabItemStates = new CompositeStateNode("TabItem states");
            enabledState.TrueStateLink = tabItemStates;
            enabledState.FalseStateLink = new StatePlaceholderNode("Disabled");

            tabItemStates.AddState(new StateNodeWithCondition("Selected", new SimpleCondition(RadTabStripElement.IsSelectedProperty, true)));
            tabItemStates.AddState(this.CreateSpecificStates());

            //mouseStateTree.FalseStateLink.FalseStateLink = new StateNodeWithCondition("Focused", new SimpleCondition(RadTabStripElement.IsFocusedProperty, true));

            return enabledState;
        }

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition mouseStateTree = new StateNodeWithCondition("Pressed", new SimpleCondition(TabItem.IsPressedProperty, true));
            mouseStateTree.FalseStateLink = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));

            return mouseStateTree;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Pressed");
            sm.AddDefaultVisibleState("Selected");
            sm.AddDefaultVisibleState("Selected.Pressed");
            sm.AddDefaultVisibleState("Selected.MouseOver");
        }
    }
}
