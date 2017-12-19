using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI.StateManagers
{
    public class TreeNodeContentElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeBase rootNode = new StateNodeWithCondition("IsRootNode", new SimpleCondition(TreeNodeContentElement.IsRootNodeProperty, true));
            StateNodeBase hasChildren = new StateNodeWithCondition("HasChildren", new SimpleCondition(TreeNodeContentElement.HasChildrenProperty, true));
            StateNodeBase mouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(TreeNodeContentElement.HotTrackingProperty, true));
            StateNodeBase selected = new StateNodeWithCondition("Selected", new SimpleCondition(TreeNodeContentElement.IsSelectedProperty, true));
            StateNodeBase current = new StateNodeWithCondition("Current", new SimpleCondition(TreeNodeContentElement.IsCurrentProperty, true));
            StateNodeBase expanded = new StateNodeWithCondition("Expanded", new SimpleCondition(TreeNodeContentElement.IsExpandedProperty, true));
            StateNodeBase controlInactive = new StateNodeWithCondition("ControlInactive", new SimpleCondition(TreeNodeContentElement.IsControlInactiveProperty, true));
            StateNodeBase fullRowSelect = new StateNodeWithCondition("FullRowSelect", new SimpleCondition(TreeNodeContentElement.FullRowSelectProperty, true));

            CompositeStateNode compositeState = new CompositeStateNode("TreeNodeContentElement states");

            compositeState.AddState(hasChildren);
            compositeState.AddState(rootNode);
            compositeState.AddState(mouseOver);
            compositeState.AddState(controlInactive);
            compositeState.AddState(selected);
            compositeState.AddState(current);
            compositeState.AddState(expanded);
            compositeState.AddState(fullRowSelect);

            return compositeState;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Current");
            sm.AddDefaultVisibleState("Selected");
            sm.AddDefaultVisibleState("Selected.Current");
            sm.AddDefaultVisibleState("MouseOver");
            sm.AddDefaultVisibleState("FullRowSelect");
            sm.AddDefaultVisibleState("ControlInactive.Current");
        }
    }
}
