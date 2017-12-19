using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    class ListViewGroupVisualItemStateManagerFactory : ListViewVisualItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition expandedState = new StateNodeWithCondition("Collapsed", new SimpleCondition(BaseListViewGroupVisualItem.ExpandedProperty, false));
            
            CompositeStateNode all = base.CreateSpecificStates() as CompositeStateNode;

            all.AddState(expandedState); 

            return all;
        }
    }
}
