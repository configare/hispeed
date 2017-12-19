using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadPageViewOutlookAssociatedButton : RadPageViewButtonElement
    {
        #region Ctor/Initialization

        static RadPageViewOutlookAssociatedButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new OverflowAssociatedButtonStateManager(),typeof(RadPageViewOutlookAssociatedButton));
        }

        #endregion
    }
}
