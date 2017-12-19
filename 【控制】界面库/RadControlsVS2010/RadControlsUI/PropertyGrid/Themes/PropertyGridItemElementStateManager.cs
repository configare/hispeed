using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class PropertyGridItemElementStateManager : PropertyGridItemElementBaseStateManager
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = (CompositeStateNode)base.CreateSpecificStates();

            StateNodeWithCondition childState = new StateNodeWithCondition("IsChildItem", new SimpleCondition(PropertyGridItemElement.IsChildItemProperty, true));
            StateNodeWithCondition modifiedState = new StateNodeWithCondition("IsModified", new SimpleCondition(PropertyGridItemElement.IsModifiedProperty, true));

            compositeNode.AddState(childState);
            compositeNode.AddState(modifiedState);

            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            base.AddDefaultVisibleStates(sm);

            sm.AddDefaultVisibleState("IsChildItem");
            sm.AddDefaultVisibleState("IsModified");
        }
    }
}
