using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ProgressIndicatorStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition isVertical = new StateNodeWithCondition("IsVertical", 
                new SimpleCondition(ProgressIndicatorElement.IsVerticalProperty, true));

            CompositeStateNode all = new CompositeStateNode("progressindicator item states");
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
