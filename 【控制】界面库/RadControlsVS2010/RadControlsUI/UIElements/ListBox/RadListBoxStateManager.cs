using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadListBoxStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition selected = new StateNodeWithCondition("Selected", new SimpleCondition(RadListBoxItem.SelectedProperty, true));
            StateNodeWithCondition active = new StateNodeWithCondition("Active", new SimpleCondition(RadListBoxItem.ActiveProperty, true));

            CompositeStateNode all = new CompositeStateNode("listbox item states");
            all.AddState(selected);
            all.AddState(active);
            return all;
        }

        protected override ItemStateManagerBase CreateStateManager()
        {
            ItemStateManagerBase sm = base.CreateStateManager();

            sm.AddDefaultVisibleState("Selected");
            sm.AddDefaultVisibleState("Active");

            return sm;
        }
    }
}
