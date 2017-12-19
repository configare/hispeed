using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class GalleryItemStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition newState = new StateNodeWithCondition("IsSelected", new SimpleCondition(RadGalleryItem.IsSelectedProperty, true));
            return newState;
        }
    }
}
