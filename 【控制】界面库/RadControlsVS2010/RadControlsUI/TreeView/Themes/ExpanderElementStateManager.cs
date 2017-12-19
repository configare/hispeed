using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ExpanderElementStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition expandedState = new StateNodeWithCondition("IsExpanded", new SimpleCondition(ExpanderItem.ExpandedProperty, true));
            StateNodeWithCondition hotTracking = new StateNodeWithCondition("HotTracking", new SimpleCondition(TreeNodeElement.HotTrackingProperty, true));
         
            CompositeStateNode compositeNode = new CompositeStateNode("ExpanderElement States");
            compositeNode.AddState(expandedState);
            compositeNode.AddState(hotTracking);
            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsExpanded");
            sm.AddDefaultVisibleState("HotTracking");
        }

        #endregion
    }
}
