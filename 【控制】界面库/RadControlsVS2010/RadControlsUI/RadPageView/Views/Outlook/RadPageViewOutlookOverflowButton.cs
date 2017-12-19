using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewOutlookOverflowButton : RadPageViewButtonElement
    {
        #region Ctor

        static RadPageViewOutlookOverflowButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new OverflowButtonStateManager(), typeof(RadPageViewOutlookOverflowButton));
        }

        #endregion

        #region RadProperties

        public static RadProperty OverflowMenuOpenedProperty = RadProperty.Register(
            "OverflowMenuOpened",
            typeof(bool),
            typeof(RadPageViewOutlookOverflowButton),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay)
            );

        #endregion
    }
}
