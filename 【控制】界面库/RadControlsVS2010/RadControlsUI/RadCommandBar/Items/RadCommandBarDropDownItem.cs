using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using Telerik.WinControls.UI.Properties;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a drop down button in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarDropDownButton : RadCommandBarBaseItem
    {
        #region Static members
        static CommandBarDropDownButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new DropDownButtonStateManagerFatory(), typeof(CommandBarDropDownButton));
        }

        #endregion

        public CommandBarDropDownButton()
        {
            this.Image = Resources.DefaultButton;
        }

        #region Fields

        protected RadCommandBarArrowButton arrowButton;
        protected RadDropDownMenu dropDownMenu;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the arrow part of the button.
        /// </summary>
        public RadCommandBarArrowButton ArrowPart
        {
            get
            {
                return arrowButton;
            }
        }

        /// <summary>
        /// Gets or sets the drop down menu, opened on click.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadDropDownMenu DropDownMenu
        {
            get
            {
                return dropDownMenu;
            }
            set
            {
                dropDownMenu = value;
            }
        }

        /// <summary>
        /// Gets menu items collection
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.dropDownMenu.Items;
            }
        }

        #endregion

        #region Event handlers

        void dropDownMenu_PopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            this.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, false);
        }

        void dropDownMenu_PopupOpened(object sender, EventArgs args)
        {
            this.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, true);
        }   

        void arrowButton_MouseLeave(object sender, EventArgs e)
        {
            this.SetValue(RadDropDownButtonElement.MouseOverStateProperty, DropDownButtonMouseOverState.None);
        }

        void arrowButton_MouseEnter(object sender, EventArgs e)
        {
            this.SetValue(RadDropDownButtonElement.MouseOverStateProperty, DropDownButtonMouseOverState.OverArrowButton);
        }

        #endregion

        #region Overrides

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.SetValue(RadDropDownButtonElement.MouseOverStateProperty, DropDownButtonMouseOverState.OverActionButton);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.SetValue(RadDropDownButtonElement.MouseOverStateProperty, DropDownButtonMouseOverState.None);
            this.SetValue(RadButtonItem.IsPressedProperty, false);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.SetValue(RadButtonItem.IsPressedProperty, true);
                ShowDropdown();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.SetValue(RadButtonItem.IsPressedProperty, false);
        }

        public virtual void ShowDropdown()
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            RadControl parentControl = (this.ElementTree.Control as RadControl);
            if (parentControl != null)
            {
                dropDownMenu.ThemeName = parentControl.ThemeName;
            }

            dropDownMenu.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            dropDownMenu.Show(this, 0, this.Size.Height);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements(); 
            dropDownMenu = new RadDropDownMenu();
            dropDownMenu.PopupOpening += delegate(object sender, CancelEventArgs args)
            {
                if (!this.RightToLeft)
                {
                    return;
                }

                RadPopupOpeningEventArgs e = args as RadPopupOpeningEventArgs;
                if (e != null)
                {
                    e.CustomLocation = new Point(e.CustomLocation.X - (this.dropDownMenu.Size.Width - this.Size.Width), e.CustomLocation.Y);
                }
            };

            dropDownMenu.PopupOpened += new RadPopupOpenedEventHandler(dropDownMenu_PopupOpened);
            dropDownMenu.PopupClosed += new RadPopupClosedEventHandler(dropDownMenu_PopupClosed);
            this.arrowButton = new RadCommandBarArrowButton();
            //this is needed because RadCommandBarSplitButton inherits this class
            if (this.GetType() == typeof(CommandBarDropDownButton))
            {
                this.arrowButton.Class = "CommandBarDropDownButtonArrow";
            }
            
            arrowButton.MouseEnter += new EventHandler(arrowButton_MouseEnter);
            arrowButton.MouseLeave += new EventHandler(arrowButton_MouseLeave);
            this.Children.Add(arrowButton);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF totalSize = base.MeasureOverride(availableSize);

            totalSize.Width += this.arrowButton.DesiredSize.Width;
            
            return totalSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientArea = GetClientRectangle(finalSize);            
            float arrowButtonWidth = this.arrowButton.DesiredSize.Width;   
            float arrowLeftPos = clientArea.Left + ((this.RightToLeft) ? 0 : clientArea.Width - arrowButtonWidth);
            RectangleF arrowArea = new RectangleF(arrowLeftPos, clientArea.Top, arrowButtonWidth, clientArea.Height);
            this.arrowButton.Arrange(arrowArea);

            float layoutMngrLeftPos = clientArea.Left + ((this.RightToLeft) ? arrowButtonWidth : 0);
            RectangleF managerArea = new RectangleF(layoutMngrLeftPos, clientArea.Top, clientArea.Width - arrowButtonWidth, clientArea.Height);
            this.layoutManagerPart.Arrange(managerArea);

            return finalSize;
        }
        #endregion
    }
}
