using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadMenuItemBaseStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode selectedComposite = new CompositeStateNode("Selected");
            StateNodeWithCondition isSelected = new StateNodeWithCondition("Selected", new SimpleCondition(RadMenuItemBase.SelectedProperty, true));
            selectedComposite.AddState(isSelected);
            StateNodeWithCondition isPopupShown = new StateNodeWithCondition("IsPopupShown", new SimpleCondition(RadMenuItemBase.IsPopupShownProperty, true));
            selectedComposite.AddState(isPopupShown);
            return selectedComposite;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            base.AddDefaultVisibleStates(sm);
            sm.AddDefaultVisibleState("Selected");
            sm.AddDefaultVisibleState("Selected.IsPopupShown");
        }
    }
}
