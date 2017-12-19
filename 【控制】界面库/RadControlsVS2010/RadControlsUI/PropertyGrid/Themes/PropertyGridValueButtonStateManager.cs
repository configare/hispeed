using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class PropertyGridValueButtonStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("PropertyValueButton element states");

            StateNodeWithCondition modifiedState = new StateNodeWithCondition("IsModified", new SimpleCondition(PropertyValueButtonElement.IsModifiedProperty, true));

            compositeNode.AddState(modifiedState);

            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsModified");
        }
    }
}
