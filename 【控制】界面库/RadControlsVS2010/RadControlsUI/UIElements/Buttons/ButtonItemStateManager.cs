using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ButtonItemStateManagerFactory : ItemStateManagerFactory
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
            CompositeStateNode mouseStates = new CompositeStateNode("Mouse states");
            StateNodeBase mouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));
            StateNodeBase mouseDown = new StateNodeWithCondition("MouseDown", new SimpleCondition(RadElement.IsMouseDownProperty, true));
            StateNodeBase pressed = new StateNodeWithCondition("Pressed", new SimpleCondition(RadButtonItem.IsPressedProperty, true));
            StateNodeBase isDefault = new StateNodeWithCondition("IsDefault", new SimpleCondition(RadButtonItem.IsDefaultProperty, true));
            mouseStates.AddState(mouseOver);
            mouseStates.AddState(mouseDown);
            mouseStates.AddState(pressed);
            mouseStates.AddState(isDefault);

            return mouseStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("MouseOver");
            sm.AddDefaultVisibleState("MouseDown");
            sm.AddDefaultVisibleState("Pressed");
            sm.AddDefaultVisibleState("IsDefault");
            sm.AddDefaultVisibleState("Disabled");
        }
    }
}
