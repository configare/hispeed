using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;
using System.ComponentModel;
using ComponentModel_DesignerSerializationVisibility=System.ComponentModel.DesignerSerializationVisibility;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel.Design;
using System.Collections;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents an application drop down menu in Office 2007 style.
    /// </summary>
	[Description("Builds attractive application menu")]
	[DefaultBindingProperty("Items"), DefaultProperty("Items")]
	[ToolboxItem(false)]
    [RadToolboxItem(false)]
    public class RadApplicationMenuDropDown : RadDropDownButtonPopup
    {
        private RadItemOwnerCollection buttonItems;
        private RadItemOwnerCollection rightColumnItems;

        #region Constructors

        public RadApplicationMenuDropDown(RadApplicationMenuButtonElement ownerElement)
            : base(ownerElement)
        {
            this.FadeAnimationType = FadeAnimationType.FadeOut;
            this.DropShadow = true;
            this.HorizontalAlignmentCorrectionMode = AlignmentCorrectionMode.Smooth;
        }

        protected override void Construct()
        {
            this.buttonItems = new RadItemOwnerCollection();
            this.buttonItems.ItemTypes = new Type[] { typeof(RadMenuItemBase) };
            this.buttonItems.DefaultType = typeof(RadMenuItem);
            this.buttonItems.ItemsChanged += new ItemChangedDelegate(ItemsChanged);

            this.rightColumnItems = new RadItemOwnerCollection();
            this.rightColumnItems.ItemTypes = new Type[] { typeof(RadMenuItemBase) };
            this.rightColumnItems.DefaultType = typeof(RadMenuButtonItem);
            this.rightColumnItems.ItemsChanged += new ItemChangedDelegate(ItemsChanged);

            base.Construct();
        }

        #endregion

        #region Properties


        public override string ThemeClassName
        {
            get
            {
                return typeof(RadApplicationMenuDropDown).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets the right column width
        /// </summary>
        [DefaultValue(300)]
        public int RightColumnWidth
        {
            get
            {
                RadApplicationMenuDropDownElement applicationMenuElement = PopupElement as RadApplicationMenuDropDownElement;
                if (applicationMenuElement != null)
                {
                    return applicationMenuElement.RightColumnWidth;
                }
                return 0;
            }
            set
            {
                RadApplicationMenuDropDownElement applicationMenuElement = PopupElement as RadApplicationMenuDropDownElement;
                if (applicationMenuElement != null)
                {
                    applicationMenuElement.RightColumnWidth = value;
                    this.ElementTree.PerformInnerLayout(true, 0, 0, this.DefaultSize.Width, this.DefaultSize.Height);
                }
            }
        }

        /// <summary>
        /// Gets a collection representing the right column items of RadApplicationMenu.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual RadItemOwnerCollection RightColumnItems
        {
            get
            {
                return this.rightColumnItems;
            }
        }

        /// <summary>
        /// Gets a collection representing the button items of RadApplicationMenu.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual RadItemOwnerCollection ButtonItems
        {
            get
            {
                return this.buttonItems;
            }
        }

        #endregion

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element.GetType().Equals(typeof(RadMenuButtonItem)))
            {
                return false;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        protected override RadElement CreatePopupElement()
        {
            RadApplicationMenuDropDownElement popupElement = new RadApplicationMenuDropDownElement();
            this.Items.Owner = popupElement.MenuElement.LayoutPanel;
            this.RightColumnItems.Owner = popupElement.TopRightContentElement.Layout;
            this.buttonItems.Owner = popupElement.BottomContentElement.Layout;
            return popupElement;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.buttonItems.ItemsChanged -= new ItemChangedDelegate(ItemsChanged);
                this.rightColumnItems.ItemsChanged -= new ItemChangedDelegate(ItemsChanged);
            }

            base.Dispose(disposing);
        }

        protected override void ShowCore(Point point, int ownerOffset, RadDirection popupDirection)
        {
            RadApplicationMenuDropDown dropDown = (RadApplicationMenuDropDown)this.ElementTree.Control;
            RadApplicationMenuDropDownElement menuElement = dropDown.PopupElement as RadApplicationMenuDropDownElement;
            if (menuElement != null && !this.IsDesignMode)
            {
                Size size = menuElement.TopRightContentElement.Layout.Size;
                size.Width -= 3;
                size.Height -= 3;
                this.PopupElement.MinSize = size;
                this.PopupElement.SetValue(RadDropDownMenuElement.DropDownPositionProperty, DropDownPosition.RightPopupContent);
            }
           
            base.ShowCore(point, ownerOffset, popupDirection);
        }
      
        private void ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            RadMenuItemBase menuItem = target as RadMenuItemBase;

            if (menuItem != null)
            {
                if (operation == ItemsChangeOperation.Inserted || operation == ItemsChangeOperation.Set)
                {
                    menuItem.HierarchyParent = this.OwnerElement as IHierarchicalItem;
                    menuItem.Owner = this.OwnerElement;
                }
                else if (operation == ItemsChangeOperation.Removed)
                {
                    menuItem.HierarchyParent = null;
                }
            }           
        }

        public override bool OnMouseWheel(Control target, int delta)
        {
            RadApplicationMenuDropDownElement element = this.PopupElement as RadApplicationMenuDropDownElement;

            if (element == null)
                return false;

            if (element.MenuElement.ScrollPanel.VerticalScrollBar.Visibility != ElementVisibility.Visible)
            {
                return true;
            }

            if (delta > 0)
            {
                element.MenuElement.ScrollPanel.LineUp();
            }
            else
            {
                element.MenuElement.ScrollPanel.LineDown();
            }

            return true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);


            IntPtr hRgn = NativeMethods.CreateRoundRectRgn(
                0,
                0,
                this.Bounds.Right - this.Bounds.Left + 1,
                this.Bounds.Bottom - this.Bounds.Top + 1, 4, 4);

            this.Region = Region.FromHrgn(hRgn);
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            //added by ittodorov, requested by stefanov
            if (this.OwnerElement != null && this.OwnerElement.IsInValidState(true))
            {
                RadRibbonBar ownerRibbonBar = (this.OwnerElement.ElementTree.Control as RadRibbonBar);
                if (ownerRibbonBar != null && ownerRibbonBar.IsDesignMode)
                {
                    Point screenLocation = ownerRibbonBar.Parent.PointToScreen(ownerRibbonBar.Location);
                    if (new Rectangle(screenLocation, ownerRibbonBar.Size).Contains(MousePosition))
                    {
                        Point appButtonLocation = screenLocation;
                        appButtonLocation.Offset(ownerRibbonBar.RibbonBarElement.ApplicationButtonElement.ControlBoundingRectangle.Location);
                        Rectangle appButtonRect = new Rectangle(appButtonLocation, ownerRibbonBar.RibbonBarElement.ApplicationButtonElement.ControlBoundingRectangle.Size);
                        return !appButtonRect.Contains(MousePosition);
                    }
                }
            }

            if (this.OwnerElement != null && this.OwnerElement.IsInValidState(true)
                && (this.OwnerElement.ElementTree.Control is RadApplicationMenu)
                && (this.OwnerElement.ElementTree.Control as RadApplicationMenu).IsDesignMode)
            {
                Point screenLocation = this.OwnerElement.ElementTree.Control.Parent.PointToScreen(this.OwnerElement.ElementTree.Control.Location);
                if (new Rectangle(screenLocation, this.OwnerElement.ElementTree.Control.Size).Contains(MousePosition))
                {
                    return false;
                }
            }
            return base.CanClosePopup(reason);
        }
    }
}
