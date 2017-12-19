using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the drop down button which is shown when 
    /// a <see cref="RadRibbonBarGroup"/>is collapsed. This drop down button
    /// holds the content of the collapsed group in its popup.
    /// </summary>
    public class RadRibbonBarGroupDropDownButtonElement : RadDropDownButtonElement
    {
        #region Properties

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadDropDownButtonElement);
            }
        }

        #endregion

        #region Methods

        protected override RadDropDownButtonPopup CreateDropDown()
        {
            RadRibbonBarGroupDropDownMenu ddMenu = new RadRibbonBarGroupDropDownMenu(this);
            return ddMenu;
        }

        #endregion
    }
}
