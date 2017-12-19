using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class PropertyGridItemElementBaseStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("PropertyGridItemElementBaseItemStates");

            StateNodeWithCondition selectedState = new StateNodeWithCondition("IsSelected", new SimpleCondition(PropertyGridItemElementBase.IsSelectedProperty, true));
            StateNodeWithCondition expandedState = new StateNodeWithCondition("IsExpanded", new SimpleCondition(PropertyGridItemElementBase.IsExpandedProperty, true));
            StateNodeWithCondition inactiveState = new StateNodeWithCondition("IsInactive", new SimpleCondition(PropertyGridItemElementBase.IsControlInactiveProperty, true));

            compositeNode.AddState(selectedState);
            compositeNode.AddState(expandedState);
            compositeNode.AddState(inactiveState);

            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            base.AddDefaultVisibleStates(sm);

            sm.AddDefaultVisibleState("IsSelected");
            sm.AddDefaultVisibleState("IsExpanded");
            sm.AddDefaultVisibleState("IsInactive");
        }

        #endregion
    }
}
