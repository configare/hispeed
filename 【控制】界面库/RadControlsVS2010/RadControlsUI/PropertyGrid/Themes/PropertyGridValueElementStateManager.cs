using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class PropertyGridValueElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("PropertyValueElement element states");

            StateNodeWithCondition modifiedState = new StateNodeWithCondition("IsModified", new SimpleCondition(PropertyGridValueElement.IsModifiedProperty, true));
            StateNodeWithCondition errorState = new StateNodeWithCondition("ContainsError", new SimpleCondition(PropertyGridValueElement.ContainsErrorProperty, true));

            compositeNode.AddState(modifiedState);
            compositeNode.AddState(errorState);

            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsModified");
            sm.AddDefaultVisibleState("ContainsError");
        }
    }
}
