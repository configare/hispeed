using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class PropertyGridHeaderElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("Header element states");

            StateNodeWithCondition isInEditModeState = new StateNodeWithCondition("IsInEditMode", new SimpleCondition(PropertyGridRowHeaderElement.IsInEditModeProperty, true));
            StateNodeWithCondition hasChildrenState = new StateNodeWithCondition("IsRootItemWithChildren", new SimpleCondition(PropertyGridRowHeaderElement.IsRootItemWithChildrenProperty, true));

            compositeNode.AddState(isInEditModeState);
            compositeNode.AddState(hasChildrenState);

            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsInEditMode");
            sm.AddDefaultVisibleState("IsRootItemWithChildren");
        }
    }
}
