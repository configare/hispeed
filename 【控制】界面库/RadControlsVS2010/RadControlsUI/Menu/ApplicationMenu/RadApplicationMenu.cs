using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.ComponentModel;
using Telerik.WinControls.Themes.Design;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents an application drop down menu in Office 2007 style.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.MenuAndToolbarsGroup)]
    [Description("Builds attractive application menu")]
    [DefaultBindingProperty("Items"), DefaultProperty("Items")]
    [ToolboxItem(true)]
    [RadToolboxItem(true)]
    [ToolboxBitmap(typeof(Telerik.WinControls.UI.RadContextMenu), "RadApplicationMenu.bmp")]
    [Designer(DesignerConsts.RadApplicationMenuDesignerString)]
    public class RadApplicationMenu : RadDropDownButton
    {
        public RadApplicationMenu()
        {
            this.DisplayStyle = DisplayStyle.Image;
        }

        #region Properties

        /// <commentsfrom cref="RadButtonItem.DisplayStyle" filter=""/>
        [DefaultValue(DisplayStyle.Image)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category(RadDesignCategory.AppearanceCategory)]
        public override DisplayStyle DisplayStyle
        {
            get
            {
                return base.DisplayStyle;
            }
            set
            {
                base.DisplayStyle = value;
            }
        }

        /// <summary>
        /// Gets a collection representing the right column items of RadApplicationMenu.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection RightColumnItems
        {
            get
            {
                RadApplicationMenuDropDown dropDown =  this.DropDownButtonElement.DropDownMenu as RadApplicationMenuDropDown;
                if (dropDown != null)
                {
                    return dropDown.RightColumnItems;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a collection representing the button items of RadApplicationMenu.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection ButtonItems
        {
            get
            {
                RadApplicationMenuDropDown dropDown =  this.DropDownButtonElement.DropDownMenu as RadApplicationMenuDropDown;
                if (dropDown != null)
                {
                    return dropDown.ButtonItems;
                }
                return null;
            }
        }

        /// <summary>
        ///  Gets or sets the right column width
        /// </summary>
        [DefaultValue(300)]
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RightColumnWidth
        {
            get
            {
                return ((RadApplicationMenuDropDown)this.DropDownButtonElement.DropDownMenu).RightColumnWidth;
            }
            set
            {
                ((RadApplicationMenuDropDown)this.DropDownButtonElement.DropDownMenu).RightColumnWidth = value;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(48, 48);
            }
        }

        #endregion

        protected override RadDropDownButtonElement CreateButtonElement()
        {
            return new RadApplicationMenuButtonElement();
        }

        /// <summary>
        /// Gets or sets the whether RadApplicationMenu will have TwoColumnDropDownMenu.
        /// </summary>  
        [DefaultValue(true)]
        [Category("Behavior"), Description("Gets or sets the whether RadApplicationMenu will have TwoColumnDropDownMenu.")]
        public bool ShowTwoColumnDropDownMenu
        {
            get
            {
                return ((RadApplicationMenuButtonElement)this.DropDownButtonElement).ShowTwoColumnDropDownMenu;
            }
            set
            {
                ((RadApplicationMenuButtonElement)this.DropDownButtonElement).ShowTwoColumnDropDownMenu = value;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (this.DropDownButtonElement.DropDownMenu != null)
            {
                if (this.DropDownButtonElement.DropDownMenu.Bounds.Contains(MousePosition))
                {
                    return;
                }

                this.DropDownButtonElement.DropDownMenu.ClosePopup(RadPopupCloseReason.AppFocusChange);
            }
        }
    }
}
