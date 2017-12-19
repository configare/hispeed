using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the popup of the
    /// <see cref="Telerik.WinControls.UI.RadDropDownButton"/> control.
    /// </summary>
    [ToolboxItem(false)]
    public class RadDropDownButtonPopup : RadDropDownMenu
    {
        #region Fields

        #endregion

        #region Ctor

        public RadDropDownButtonPopup(RadDropDownButtonElement ownerElement) : base(ownerElement)
        {

        }

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get
            {
                return typeof(RadDropDownMenu).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        #endregion

        #region Methods

        protected override void OnDropDownOpening(CancelEventArgs args)
        {
            base.OnDropDownOpening(args);

            foreach (RadItem item in this.Items)
            {
                if (item is RadMenuItem)
                {
                    this.ShowItemCues(item as RadMenuItem);
                }
            }
        }

        protected virtual void AdjustDropDownAlignmentForPopupDirection(RadDirection popupDirection)
        {
            switch (popupDirection)
            {
                case RadDirection.Down:
                    {
                        bool isRTL = this.OwnerElement != null ? this.OwnerElement.RightToLeft : false;
                        this.HorizontalPopupAlignment = !isRTL ? HorizontalPopupAlignment.LeftToLeft : HorizontalPopupAlignment.RightToRight;
                        this.VerticalPopupAlignment = VerticalPopupAlignment.TopToBottom;
                        break;
                    }
                case RadDirection.Left:
                    {
                        this.HorizontalPopupAlignment = HorizontalPopupAlignment.RightToLeft;
                        this.VerticalPopupAlignment = VerticalPopupAlignment.TopToTop;
                        break;
                    }
                case RadDirection.Right:
                    {
                        this.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToRight;
                        this.VerticalPopupAlignment = VerticalPopupAlignment.TopToTop;
                        break;
                    }
                case RadDirection.Up:
                    {
                        bool isRTL = this.OwnerElement != null ? this.OwnerElement.RightToLeft : false;
                        this.HorizontalPopupAlignment = !isRTL ? HorizontalPopupAlignment.LeftToLeft : HorizontalPopupAlignment.RightToRight;
                        this.VerticalPopupAlignment = VerticalPopupAlignment.BottomToTop;
                        break;
                    }
            }
        }

        protected override void ShowCore(Point point, int ownerOffset, RadDirection popupDirection)
        {
            this.AdjustDropDownAlignmentForPopupDirection(popupDirection);
            base.ShowCore(point, ownerOffset, popupDirection);
        }

        private void ShowItemCues(RadMenuItem item)
        {
            foreach (RadItem currentItem in item.Items)
            {
                if (currentItem is RadMenuItem)
                {
                    this.ShowItemCues(currentItem as RadMenuItem);
                }
            }

            RadMenuItem menuItem = item as RadMenuItem;

            if (menuItem != null)
            {
                menuItem.ShowKeyboardCue = true;
            }
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            if (this.OwnerElement.IsDesignMode)
            {
                Point screenLocation = this.OwnerElement.ElementTree.Control.Parent.PointToScreen(this.OwnerElement.ElementTree.Control.Location);
                if (new Rectangle(screenLocation, this.OwnerElement.ElementTree.Control.Size).Contains(MousePosition))
                {
                    return false;
                }
            }

            return base.CanClosePopup(reason);
        }

        protected override void OnItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            base.OnItemsChanged(changed, target, operation);

            if (operation == ItemsChangeOperation.Inserted)
            {
                RadMenuItemBase menuItemBase = target as RadMenuItemBase;

                if (menuItemBase != null)
                {
                    menuItemBase.IsMainMenuItem = false;
                }
            }
        }

        #endregion
    }
}
