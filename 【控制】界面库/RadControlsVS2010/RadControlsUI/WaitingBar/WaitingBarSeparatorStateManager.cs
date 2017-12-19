using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class WaitingBarSeparatorStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition isVertical = new StateNodeWithCondition("IsVertical",
                new SimpleCondition(WaitingBarSeparatorElement.IsVerticalProperty, true));

            CompositeStateNode all = new CompositeStateNode("WaitingSeparator item states");
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
