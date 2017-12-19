using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class SearchBarTextBoxButtonStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode compositeNode = new CompositeStateNode("SearchBarTextBoxButtonItemStates");

            StateNodeWithCondition searchingState = new StateNodeWithCondition("IsSearching", new SimpleCondition(ToolbarTextBoxButton.IsSearchingProperty, true));

            compositeNode.AddState(searchingState);

            return compositeNode;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            base.AddDefaultVisibleStates(sm);

            sm.AddDefaultVisibleState("IsSearching");
        }

        #endregion
    }
}
