using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI.StateManagers
{
    public class TreeNodeElementStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeBase rootNode = new StateNodeWithCondition("IsRootNode", new SimpleCondition(TreeNodeElement.IsRootNodeProperty, true));
            StateNodeBase hasChildren = new StateNodeWithCondition("HasChildren", new SimpleCondition(TreeNodeElement.HasChildrenProperty, true));
            StateNodeBase controlInactive = new StateNodeWithCondition("ControlInactive", new SimpleCondition(TreeNodeElement.IsControlInactiveProperty, true));
            StateNodeBase fullRowSelect = new StateNodeWithCondition("FullRowSelect", new SimpleCondition(TreeNodeElement.FullRowSelectProperty, true));
            StateNodeBase selected = new StateNodeWithCondition("Selected", new SimpleCondition(TreeNodeElement.IsSelectedProperty, true));
            StateNodeBase current = new StateNodeWithCondition("Current", new SimpleCondition(TreeNodeElement.IsCurrentProperty, true));
            StateNodeBase mouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(TreeNodeElement.HotTrackingProperty, true));
            StateNodeBase expanded = new StateNodeWithCondition("Expanded", new SimpleCondition(TreeNodeElement.IsExpandedProperty, true));
          
            CompositeStateNode compositeState = new CompositeStateNode("TreeNodeElement states");

            compositeState.AddState(rootNode);
            compositeState.AddState(hasChildren);
            compositeState.AddState(controlInactive);
            compositeState.AddState(fullRowSelect);
            compositeState.AddState(selected);
            compositeState.AddState(current);
            compositeState.AddState(mouseOver);
            compositeState.AddState(expanded);
            
            return compositeState;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("FullRowSelect.Current");
            sm.AddDefaultVisibleState("FullRowSelect.Selected");
            sm.AddDefaultVisibleState("FullRowSelect.Selected.Current");
            sm.AddDefaultVisibleState("FullRowSelect.MouseOver");
            sm.AddDefaultVisibleState("ControlInactive.FullRowSelect.Selected.Current");            
        }
    }
}
