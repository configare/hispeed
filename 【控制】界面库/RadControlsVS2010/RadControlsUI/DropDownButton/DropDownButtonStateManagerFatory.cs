using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class DropDownButtonStateManagerFatory : ItemStateManagerFactory
    {
        public override StateNodeBase CreateRootState()
        {
            StateNodeWithCondition enabledState = new StateNodeWithCondition("Enabled", new SimpleCondition(RadElement.EnabledProperty, true));

            //CompositeStateNode toggleButtonStates = new CompositeStateNode("DropDownOpened");
            
            enabledState.FalseStateLink = new StatePlaceholderNode("Disabled");

            StateNodeWithCondition dropDownOpened = new StateNodeWithCondition("DropDownOpened", new SimpleCondition(RadDropDownButtonElement.IsDropDownShownProperty, true));

            enabledState.TrueStateLink = dropDownOpened;

            dropDownOpened.FalseStateLink = this.CreateSpecificStates();


            return enabledState;
        }

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition mouseStateTree = new StateNodeWithCondition("Pressed", new SimpleCondition(RadButtonItem.IsPressedProperty, true));
            CompositeStateNode mouseOverComposite = new CompositeStateNode("MouseOverStates");
            mouseStateTree.FalseStateLink = mouseOverComposite;
            mouseStateTree.FalseStateLink.FalseStateLink = new StateNodeWithCondition("Focused", new SimpleCondition(RadTabStripElement.IsFocusedProperty, true));

            mouseOverComposite.AddState(new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true)));
            StateNodeWithCondition mouseOverAction = new StateNodeWithCondition("ActionPart", new SimpleCondition(RadDropDownButtonElement.MouseOverStateProperty, DropDownButtonMouseOverState.OverActionButton));
            mouseOverComposite.AddState(mouseOverAction);
            mouseOverAction.FalseStateLink = new StateNodeWithCondition("ArrowPart", new SimpleCondition(RadDropDownButtonElement.MouseOverStateProperty, DropDownButtonMouseOverState.OverArrowButton));
            //mouseStateTree.FalseStateLink.TrueStateLink.FalseStateLink.FalseStateLink = new StatePlaceholderNode("MouseOver");

            return mouseStateTree;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager res)
        {
            res.AddDefaultVisibleState("MouseOver.ActionPart");
            res.AddDefaultVisibleState("MouseOver.ArrowPart");
            res.AddDefaultVisibleState("Pressed");
            res.AddDefaultVisibleState("DropDownOpened");
        }
    }
}
