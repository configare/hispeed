using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadPanelBarGroupElementStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            return new StateNodeWithCondition("Selected", new SimpleCondition(RadPanelBarGroupElement.SelectedProperty, true));
        }

        protected override ItemStateManagerBase CreateStateManager()
        {
            ItemStateManagerBase sm = base.CreateStateManager();
            sm.AddDefaultVisibleState("Selected");

            return sm;
        }
    }
}
