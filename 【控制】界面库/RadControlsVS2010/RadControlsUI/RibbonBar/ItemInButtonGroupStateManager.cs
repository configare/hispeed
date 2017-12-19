using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ItemInButtonGroupStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("GroupStates");
            StateNodeWithCondition isAtEndIndexState = new StateNodeWithCondition("IsAtEndIndex", new SimpleCondition(RadRibbonBarButtonGroup.IsItemAtEndIndexProperty, true));
            compositeNode.AddState(isAtEndIndexState);
            StateNodeWithCondition groupOrientationHState = new StateNodeWithCondition("GroupOrientationHorizontal", new SimpleCondition(RadRibbonBarButtonGroup.InternalOrientationProperty, Orientation.Horizontal));
            compositeNode.AddState(groupOrientationHState);
            StateNodeWithCondition groupOrientationVState = new StateNodeWithCondition("GroupOrientationVertical", new SimpleCondition(RadRibbonBarButtonGroup.InternalOrientationProperty, Orientation.Vertical));
            compositeNode.AddState(groupOrientationVState);
            return compositeNode;
        }
    }
}
