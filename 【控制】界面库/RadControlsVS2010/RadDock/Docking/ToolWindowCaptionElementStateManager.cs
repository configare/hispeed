using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI.Docking
{
    public class ToolWindowCaptionElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition node = new StateNodeWithCondition("IsToolWindowActive", new SimpleCondition(ToolWindowCaptionElement.IsActiveProperty, true));
            return node;
        }
    }
}
