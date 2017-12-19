using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using Telerik.WinControls.UI.Docking;

namespace Telerik.WinControls.UI
{
    class QuickNavigatorListItemStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition isActive = new StateNodeWithCondition("IsActive", new SimpleCondition(QuickNavigatorListItem.IsActiveProperty, true));
            return isActive;
        }

        protected override StateNodeBase CreateEnabledStates()
        {
            StateNodeWithCondition mouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));
            return mouseOver;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("MouseOver");
            sm.AddDefaultVisibleState("IsActive");
        }
    }
}
