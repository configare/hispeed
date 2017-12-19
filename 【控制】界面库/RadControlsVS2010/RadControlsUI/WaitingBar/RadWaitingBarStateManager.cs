using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadWaitingBarStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition isVertical = new StateNodeWithCondition("IsVertical",
                new SimpleCondition(RadWaitingBarElement.IsVerticalProperty, true));

            CompositeStateNode all = new CompositeStateNode("waitingbar item states");
            all.AddState(isVertical);

            return all;
        }

        protected override ItemStateManagerBase CreateStateManager()
        {
            ItemStateManagerBase sm = base.CreateStateManager();

            sm.AddDefaultVisibleState("IsVertical");

            return sm;
        }
    }
}
