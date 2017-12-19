using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RibbonFormElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition stateNode = new StateNodeWithCondition("IsRibbonFormActive", new SimpleCondition(RibbonFormElement.IsFormActiveProperty, true));
            return stateNode;
        }
    }
}
