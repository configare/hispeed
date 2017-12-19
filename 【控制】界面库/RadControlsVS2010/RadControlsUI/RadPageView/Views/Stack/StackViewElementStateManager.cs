using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class StackViewElementStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode states = new CompositeStateNode("StackPositions");
            StateNodeWithCondition bottomPosition = new StateNodeWithCondition("BottomStackPosition", new SimpleCondition(RadPageViewStackElement.StackPositionProperty, StackViewPosition.Bottom));
            StateNodeWithCondition topPosition = new StateNodeWithCondition("TopStackPosition", new SimpleCondition(RadPageViewStackElement.StackPositionProperty, StackViewPosition.Top));
            StateNodeWithCondition leftPosition = new StateNodeWithCondition("LeftStackPosition", new SimpleCondition(RadPageViewStackElement.StackPositionProperty, StackViewPosition.Left));
            StateNodeWithCondition rightPosition = new StateNodeWithCondition("RightStackPosition", new SimpleCondition(RadPageViewStackElement.StackPositionProperty, StackViewPosition.Right));

            states.AddState(leftPosition);
            states.AddState(topPosition);
            states.AddState(rightPosition);
            states.AddState(bottomPosition);

            return states;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("LeftStackPosition");
            sm.AddDefaultVisibleState("TopStackPosition");
            sm.AddDefaultVisibleState("RightStackPosition");
            sm.AddDefaultVisibleState("BottomStackPosition");
        }

        #endregion
    }
}
