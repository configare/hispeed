using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ExpanderItemStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("ExpanderItemStates");
            StateNodeWithCondition expandedState = new StateNodeWithCondition("IsExpanded", new SimpleCondition(ExpanderItem.ExpandedProperty, true));
            compositeNode.AddState(expandedState);
            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsExpanded");
        }

        #endregion
    }
}
