using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class SplitContainerElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateEnabledStates()
        {
            CompositeStateNode baseStates = new CompositeStateNode("Base States");
            StateNodeBase isVertical = new StateNodeWithCondition("IsVertical", new SimpleCondition(SplitContainerElement.IsVerticalProperty, true));
            baseStates.AddState(isVertical);

            return baseStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsVertical");
        }
    }
}
