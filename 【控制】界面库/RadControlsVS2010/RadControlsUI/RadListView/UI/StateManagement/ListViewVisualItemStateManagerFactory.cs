using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ListViewVisualItemStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition hotTrackingState = new StateNodeWithCondition("IsHotTracking", new SimpleCondition(BaseListViewVisualItem.HotTrackingProperty, true));
            StateNodeWithCondition currentState = new StateNodeWithCondition("Current", new SimpleCondition(BaseListViewVisualItem.CurrentProperty, true));
            StateNodeWithCondition selectedState = new StateNodeWithCondition("Selected", new SimpleCondition(BaseListViewVisualItem.SelectedProperty, true));
            StateNodeWithCondition controlInactiveState = new StateNodeWithCondition("ControlInactive", new SimpleCondition(BaseListViewVisualItem.IsControlInactiveProperty, true));

            CompositeStateNode all = new CompositeStateNode("ListViewVisualItem states");

            all.AddState(hotTrackingState);
            all.AddState(controlInactiveState);
            all.AddState(currentState);
            all.AddState(selectedState);
            
            return all;
        }
    }
}
