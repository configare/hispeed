using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class TabStripElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateEnabledStates()
        {
            CompositeStateNode baseStates = new CompositeStateNode("Base States");
            StateNodeBase leftAlign = new StateNodeWithCondition("LeftAlign", new SimpleCondition(RadTabStripElement.TabsPositionProperty, TabPositions.Left));
            StateNodeBase rightAlign = new StateNodeWithCondition("RightAlign", new SimpleCondition(RadTabStripElement.TabsPositionProperty, TabPositions.Right));
            StateNodeBase bottomAlign = new StateNodeWithCondition("BottomAlign", new SimpleCondition(RadTabStripElement.TabsPositionProperty, TabPositions.Bottom));
            baseStates.AddState(leftAlign);
            baseStates.AddState(rightAlign);
            baseStates.AddState(bottomAlign);

            return baseStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("LeftAlign");
            sm.AddDefaultVisibleState("RightAlign");
            sm.AddDefaultVisibleState("BottomAlign");
            sm.AddDefaultVisibleState("Disabled");
        }
    }
}
