using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI.StateManagers
{
    public class TreeNodeExpanderItemStateManager : ExpanderItemStateManager
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("ExpanderItemStates");
            StateNodeWithCondition expandedState = new StateNodeWithCondition("IsExpanded", new SimpleCondition(ExpanderItem.ExpandedProperty, true));
            StateNodeWithCondition selectedState = new StateNodeWithCondition("IsSelected", new SimpleCondition(TreeNodeExpanderItem.IsSelectedProperty, true));
            StateNodeWithCondition hotTrackingState = new StateNodeWithCondition("HotTracking", new SimpleCondition(TreeNodeExpanderItem.HotTrackingProperty, true));

            compositeNode.AddState(hotTrackingState);
            compositeNode.AddState(selectedState);
            compositeNode.AddState(expandedState);

            return compositeNode;
        }

        protected override ItemStateManager CreateStateManagerCore()
        {
            return new TreeExpanderStateManager(this.RootState);
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsExpanded");
            sm.AddDefaultVisibleState("IsSelected");
            sm.AddDefaultVisibleState("IsSelected.IsExpanded");
            sm.AddDefaultVisibleState("HotTracking");
            sm.AddDefaultVisibleState("HotTracking.IsExpanded");
        }
    }
}
