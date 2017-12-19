using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class StripViewElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateEnabledStates()
        {
            CompositeStateNode baseStates = new CompositeStateNode("Base States");
            StateNodeBase leftAlign = new StateNodeWithCondition("LeftAlign", new SimpleCondition(RadPageViewStripElement.StripAlignmentProperty, StripViewAlignment.Left));
            StateNodeBase rightAlign = new StateNodeWithCondition("RightAlign", new SimpleCondition(RadPageViewStripElement.StripAlignmentProperty, StripViewAlignment.Right));
            StateNodeBase bottomAlign = new StateNodeWithCondition("BottomAlign", new SimpleCondition(RadPageViewStripElement.StripAlignmentProperty, StripViewAlignment.Bottom));
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
