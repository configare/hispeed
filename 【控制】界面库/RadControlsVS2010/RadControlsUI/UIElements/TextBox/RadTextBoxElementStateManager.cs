using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadTextBoxElementStateManager : EditorElementStateManager
    {
        protected override Telerik.WinControls.Styles.StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition isNullText = new StateNodeWithCondition("IsNullText", new SimpleCondition(RadTextBoxItem.IsNullTextProperty, true));

            CompositeStateNode specificStates = (CompositeStateNode)base.CreateSpecificStates();
            specificStates.AddState(isNullText);
            return specificStates;
        }
    }
}
