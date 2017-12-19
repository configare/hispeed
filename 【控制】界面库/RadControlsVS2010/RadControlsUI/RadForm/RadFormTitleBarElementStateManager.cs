using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadFormTitleBarElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition condition = new StateNodeWithCondition("IsFormActive", new SimpleCondition(RadFormElement.IsFormActiveProperty, true));
            return condition;
        }
    }
}
