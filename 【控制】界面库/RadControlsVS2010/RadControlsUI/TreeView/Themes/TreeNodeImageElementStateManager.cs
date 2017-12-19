
using Telerik.WinControls.Styles;
namespace Telerik.WinControls.UI
{
    public class TreeNodeImageElementStateManager : ExpanderItemStateManager
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeBase rootNode = new StateNodeWithCondition("IsRootNode", new SimpleCondition(TreeNodeImageElement.IsRootNodeProperty, true));
            StateNodeBase hasChildren = new StateNodeWithCondition("HasChildren", new SimpleCondition(TreeNodeImageElement.HasChildrenProperty, true));
            StateNodeBase selected = new StateNodeWithCondition("IsSelected", new SimpleCondition(TreeNodeImageElement.IsSelectedProperty, true));
            StateNodeBase current = new StateNodeWithCondition("IsCurrent", new SimpleCondition(TreeNodeImageElement.IsCurrentProperty, true));
            StateNodeBase expanded = new StateNodeWithCondition("IsExpanded", new SimpleCondition(TreeNodeImageElement.IsExpandedProperty, true));
            StateNodeWithCondition hotTrackingState = new StateNodeWithCondition("HotTracking", new SimpleCondition(TreeNodeImageElement.HotTrackingProperty, true));

            CompositeStateNode compositeState = new CompositeStateNode("TreeNodeImageElement states");

            compositeState.AddState(hasChildren);
            compositeState.AddState(rootNode);
            compositeState.AddState(selected);
            compositeState.AddState(current);
            compositeState.AddState(hotTrackingState);
            compositeState.AddState(expanded);

            return compositeState;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("IsRootNode");
            sm.AddDefaultVisibleState("IsCurrent");
            sm.AddDefaultVisibleState("IsSelected");
            sm.AddDefaultVisibleState("IsSelected.IsCurrent");
            sm.AddDefaultVisibleState("IsExpanded");
            sm.AddDefaultVisibleState("HasChildren");
        }
    }
}
