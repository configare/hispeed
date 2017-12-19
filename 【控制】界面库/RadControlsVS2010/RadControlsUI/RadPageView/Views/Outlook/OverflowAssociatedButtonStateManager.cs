using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class OverflowAssociatedButtonStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition selectedState = new StateNodeWithCondition(
                "AssociatedItemSelected",
                new SimpleCondition(OverflowItemsContainer.ItemSelectedProperty, true));
            return selectedState;
        }

        #endregion
    }
}
