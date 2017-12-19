using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI.StateManagers
{
    public class PropertyGridExpanderElementStateManager : ExpanderItemStateManager
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("ExpanderItemStates");
            StateNodeWithCondition expandedState = new StateNodeWithCondition("IsExpanded", new SimpleCondition(PropertyGridExpanderElement.IsExpandedProperty, true));
            StateNodeWithCondition selectedState = new StateNodeWithCondition("IsSelected", new SimpleCondition(PropertyGridExpanderElement.IsSelectedProperty, true));
            StateNodeWithCondition childState = new StateNodeWithCondition("IsChildItem", new SimpleCondition(PropertyGridItemElement.IsChildItemProperty, true));
            StateNodeWithCondition inactiveState = new StateNodeWithCondition("IsInactive", new SimpleCondition(PropertyGridExpanderElement.IsControlInactiveProperty, true));
            StateNodeWithCondition isInEditMode = new StateNodeWithCondition("IsInEditMode", new SimpleCondition(PropertyGridExpanderElement.IsInEditModeProperty, true));

            compositeNode.AddState(selectedState);
            compositeNode.AddState(expandedState);
            compositeNode.AddState(childState);
            compositeNode.AddState(inactiveState);
            compositeNode.AddState(isInEditMode);

            return compositeNode;
        }

        protected override ItemStateManager CreateStateManagerCore()
        {
            return new PropertyGridExpanderStateManager(this.RootState);
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            base.AddDefaultVisibleStates(sm);

            sm.AddDefaultVisibleState("MouseOver");
            sm.AddDefaultVisibleState("MouseDown");
            sm.AddDefaultVisibleState("Disabled");
            sm.AddDefaultVisibleState("IsInactive");
            sm.AddDefaultVisibleState("IsChildItem");
            sm.AddDefaultVisibleState("IsSelected");
            sm.AddDefaultVisibleState("IsSelected" + ItemStateManagerBase.stateDelimiter + "IsExpanded");
            sm.AddDefaultVisibleState("IsInEditMode");
        }
    }
}
