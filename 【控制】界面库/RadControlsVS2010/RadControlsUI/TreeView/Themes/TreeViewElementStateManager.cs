using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI.StateManagers
{
    public class TreeViewElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeBase isFocused = new StateNodeWithCondition("Focused", new SimpleCondition(RadElement.IsFocusedProperty, true));
            StateNodeBase isMouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));
            

            CompositeStateNode compositeState = new CompositeStateNode("TreeViewElement states");
            compositeState.AddState(isFocused);
            compositeState.AddState(isMouseOver);
            
            return compositeState;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Focused");
            sm.AddDefaultVisibleState("MouseOver");
        }
    }
}
