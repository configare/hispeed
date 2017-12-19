using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ScrollBarThumbStateManager : ItemStateManagerFactory
    {
        public override StateNodeBase CreateRootState()
        {
            StateNodeWithCondition enabledState = new StateNodeWithCondition("Enabled", new SimpleCondition(RadElement.EnabledProperty, true));
            enabledState.TrueStateLink = this.CreateSpecificStates();
            enabledState.FalseStateLink = new StatePlaceholderNode("Disabled");

            return enabledState;
        }

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition mouseStateTree = new StateNodeWithCondition("Pressed", new SimpleCondition(ScrollBarThumb.IsPressedProperty, true));
            mouseStateTree.FalseStateLink = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));

            CompositeStateNode scrollTypeStates = new CompositeStateNode("ScrollType");

            StateNodeWithCondition scrollTypeVertical = new StateNodeWithCondition("ScrollType=Vertical", new SimpleCondition(RadScrollBarElement.ScrollTypeProperty, ScrollType.Vertical));
            scrollTypeStates.AddState(scrollTypeVertical);
            scrollTypeStates.AddState(mouseStateTree);


            return scrollTypeStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager res)
        {
            res.AddDefaultVisibleState("MouseOver");
            res.AddDefaultVisibleState("Pressed");

            res.AddDefaultVisibleState("ScrollType=Vertical");
            res.AddDefaultVisibleState("ScrollType=Vertical.MouseOver");
            res.AddDefaultVisibleState("ScrollType=Vertical.Pressed");

            res.AddDefaultVisibleState("Disabled");
        }
    }
}
