using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadRibbonBarElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition newState = new StateNodeWithCondition("IsRibbonFormActive", new SimpleCondition(RadRibbonBarElement.IsRibbonFormActiveProperty, true));
            return newState;
        }
    }
}
