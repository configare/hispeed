using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class ListViewDataCellStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition currentState = new StateNodeWithCondition("Current", new SimpleCondition(DetailListViewCellElement.CurrentProperty, true));
            StateNodeWithCondition selectedState = new StateNodeWithCondition("Selected", new SimpleCondition(DetailListViewDataCellElement.SelectedProperty, true));
            StateNodeWithCondition currentRowState = new StateNodeWithCondition("CurrentRow", new SimpleCondition(DetailListViewDataCellElement.CurrentRowProperty, true));

            CompositeStateNode all = new CompositeStateNode("ListViewCellElement states");
            all.AddState(currentState);
            all.AddState(selectedState);
            all.AddState(currentRowState);

            return all;
        }
    }
}
