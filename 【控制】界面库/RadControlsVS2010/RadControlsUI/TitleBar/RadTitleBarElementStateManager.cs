using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadTitleBarElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition node = new StateNodeWithCondition("IsFormActive", new SimpleCondition(RadFormElement.IsFormActiveProperty, true));
            return node;
        }
    }
}
