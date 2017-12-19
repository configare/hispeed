using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Menu
{
    public class RadMenuItemPopup : RadPopupControlBase
    {
        /// <summary>
        /// Creates an instance of the RadMenuItemPopup class.
        /// This class represents the popup which is used to display menu
        /// items in the RadMenu control.
        /// </summary>
        /// <param name="owner">An instance of the RadItem class which represents the
        /// owner of the popup.</param>
        public RadMenuItemPopup(RadItem owner) : base(owner)
        {
        }
    }
}
