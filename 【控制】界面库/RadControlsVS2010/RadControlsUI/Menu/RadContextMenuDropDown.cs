using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the drop-down menu
    /// used in the <see cref="Telerik.WinControls.UI.RadContextMenu"/>
    /// component.
    /// </summary>
    [ToolboxItem(false)]
    public class RadContextMenuDropDown : RadDropDownMenu
    {
        #region Methods

        protected override void OnDropDownOpening(CancelEventArgs args)
        {
            base.OnDropDownOpening(args);

            foreach (RadMenuItemBase item in this.Items)
            {
                this.ShowItemCues(item);
            }
        }

        private void ShowItemCues(RadMenuItemBase item)
        {
            foreach (RadMenuItemBase currentItem in item.Items)
            {
                this.ShowItemCues(currentItem);
            }

            RadMenuItem menuItem = item as RadMenuItem;

            if (menuItem != null)
            {
                menuItem.ShowKeyboardCue = true;
            }
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            return true;
        }

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get
            {
                return typeof(RadDropDownMenu).FullName;
            }
        }

        #endregion
    }
}
