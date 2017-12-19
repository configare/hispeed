using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class OverflowButtonStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition state = new StateNodeWithCondition("OverflowMenuOpened", new SimpleCondition(RadPageViewOutlookOverflowButton.OverflowMenuOpenedProperty, true));
            return state;
        }

        #endregion
    }
}
