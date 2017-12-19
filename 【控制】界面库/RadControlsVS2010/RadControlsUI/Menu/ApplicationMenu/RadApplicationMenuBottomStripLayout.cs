using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadApplicationMenuBottomStripLayout : StackLayoutPanel
    {
        private bool autoHideMenuOnClick = true;

        [DefaultValue(true)]
        public bool AutoHideMenuOnClick
        {
            get { return this.autoHideMenuOnClick; }
            set { this.autoHideMenuOnClick = value; }
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            if (changeOperation == ItemsChangeOperation.Inserted)
            {
                RadItem item = child as RadItem;
                if (item != null)
                {
                    item.Click += new EventHandler(item_Click);
                }
            }
            if (changeOperation == ItemsChangeOperation.Removed)
            {
                RadItem item = child as RadItem;
                if (item != null)
                {
                    item.Click -= new EventHandler(item_Click);
                }
            }
            if (changeOperation == ItemsChangeOperation.Clearing)
            {
                foreach (RadElement element in this.Children)
                {
                    RadItem item = element as RadItem;
                    if (item != null)
                    {
                        item.Click -= new EventHandler(item_Click);
                    }
                }
            }
        }

        private void item_Click(object sender, EventArgs e)
        {
            RadApplicationMenu menu = this.ElementTree.Control as RadApplicationMenu;
            if (menu != null && this.autoHideMenuOnClick)
            {
                menu.DropDownButtonElement.DropDownMenu.ClosePopup(RadPopupCloseReason.CloseCalled);
            }
        }
    }
}
