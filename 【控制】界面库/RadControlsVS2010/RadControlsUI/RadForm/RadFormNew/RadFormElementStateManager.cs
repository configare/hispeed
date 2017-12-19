using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadFormElementStateManager : ItemStateManagerFactory
    {
        #region Methods

        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition newState = new StateNodeWithCondition("IsFormActive", new SimpleCondition(RadFormElement.IsFormActiveProperty, true));
            return newState;
        }

        #endregion
    }
}
