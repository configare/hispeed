using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadPageViewItemStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateEnabledStates()
        {
            CompositeStateNode baseStates = new CompositeStateNode("Base States");
            StateNodeBase selected = new StateNodeWithCondition("Selected", new SimpleCondition(RadPageViewItem.IsSelectedProperty, true));
            StateNodeBase mouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));
            StateNodeBase mouseDown = new StateNodeWithCondition("MouseDown", new SimpleCondition(RadElement.IsMouseDownProperty, true));
            baseStates.AddState(selected);
            baseStates.AddState(mouseOver);
            baseStates.AddState(mouseDown);

            return baseStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Selected");
            sm.AddDefaultVisibleState("MouseOver");
            sm.AddDefaultVisibleState("MouseDown");
            sm.AddDefaultVisibleState("Disabled");
        }
    }
}
