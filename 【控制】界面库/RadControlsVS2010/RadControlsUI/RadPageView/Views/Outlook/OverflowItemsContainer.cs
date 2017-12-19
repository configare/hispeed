using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class OverflowItemsContainer : RadPageViewElementBase
    {
        #region Fields

        private RadDropDownMenu overflowMenu;
        private RadMenuItem showMoreButtons;
        private RadMenuItem showFewerButtons;
        private RadMenuItem addRemoveButtons;
        private RadPageViewOutlookOverflowButton overflowButtonElement;
        private StackLayoutPanel buttonsContainer;
        private RadPageViewOutlookElement owner;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="StackLayoutPanel"/> class which is
        /// the layout panel that holds instances of the <see cref="RadPageViewOutlookAssociatedButton"/>
        /// class representing items currently collapsed by using the overflow grip.
        /// </summary>
        [Browsable(false)]
        public StackLayoutPanel OverflowButtonsContainer
        {
            get
            {
                return this.buttonsContainer;
            }
        }

        /// <summary>
        /// Gets the overflow menu button.
        /// </summary>
        [Browsable(false)]
        public RadPageViewOutlookOverflowButton OverflowButtonElement
        {
            get
            {
                return this.overflowButtonElement;
            }
        }

        /// <summary>
        /// Gets the overflow drop-down menu.
        /// </summary>
        [Browsable(false)]
        public RadDropDownMenu OverflowMenu
        {
            get
            {
                return this.overflowMenu;
            }
        }

        /// <summary>
        /// Gets the overflow menu item used to show fewer items in the stack.
        /// </summary>
        [Browsable(false)]
        public RadMenuItem ShowFewerButtonsItem
        {
            get
            {
                return this.showFewerButtons;
            }
        }

        /// <summary>
        /// Gets the overflow menu item used to show more buttons in the stack.
        /// </summary>
        [Browsable(false)]
        public RadMenuItem ShowMoreButtonsItem
        {
            get
            {
                return this.showMoreButtons;
            }
        }

        /// <summary>
        /// Gets the overflow menu item used to add/remove items in the stack.
        /// </summary>
        [Browsable(false)]
        public RadMenuItem AddRemoveButtonsItem
        {
            get
            {
                return this.addRemoveButtons;
            }
        }

        #endregion

        #region RadProperties

        public static RadProperty ItemSelectedProperty = RadProperty.Register(
            "ItemSelectedProperty",
            typeof(bool),
            typeof(RadPageViewButtonElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay)
            );

        #endregion

        #region Ctor

        public OverflowItemsContainer(RadPageViewOutlookElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.overflowButtonElement = new RadPageViewOutlookOverflowButton();
            this.overflowButtonElement.ThemeRole = "OverflowMenuButton";
            this.overflowButtonElement.StretchVertically = true;
            this.overflowMenu = new RadDropDownMenu();
            this.buttonsContainer = new StackLayoutPanel();

            RadPageViewLocalizationProvider localizationProvider = RadPageViewLocalizationProvider.CurrentProvider;

            this.showMoreButtons = new RadMenuItem(localizationProvider.GetLocalizedString(RadPageViewStringId.ShowMoreButtonsItemCaption));
            this.showMoreButtons.SetDefaultValueOverride(RadButtonItem.ImageAlignmentProperty, ContentAlignment.MiddleCenter);
            this.showFewerButtons = new RadMenuItem(localizationProvider.GetLocalizedString(RadPageViewStringId.ShowFewerButtonsItemCaption));
            this.showFewerButtons.SetDefaultValueOverride(RadButtonItem.ImageAlignmentProperty, ContentAlignment.MiddleCenter);
            this.addRemoveButtons = new RadMenuItem(localizationProvider.GetLocalizedString(RadPageViewStringId.AddRemoveButtonsItemCaption));

            this.overflowMenu.Items.Add(this.showMoreButtons);
            this.overflowMenu.Items.Add(this.showFewerButtons);
            this.overflowMenu.Items.Add(this.addRemoveButtons);

            this.WireEvents();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.buttonsContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.buttonsContainer.StretchVertically = true;
            this.buttonsContainer.Children.Insert(this.buttonsContainer.Children.Count, this.overflowButtonElement);
            this.Children.Add(this.buttonsContainer);
        }

        private void WireEvents()
        {
            this.overflowButtonElement.Click += this.OnOverflowButtonElement_Click;
            this.showMoreButtons.Click += this.OnShowMoreButtons_Click;
            this.showFewerButtons.Click += this.OnShowFewerButtons_Click;
            this.overflowMenu.PopupOpened += this.OnOverflowMenu_Shown;
            this.overflowMenu.PopupClosed += this.overflowMenu_PopupClosed;
            RadPageViewLocalizationProvider.CurrentProviderChanged += new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        private void UnwireEvents()
        {
            this.overflowButtonElement.Click -= this.OnOverflowButtonElement_Click;
            this.showMoreButtons.Click -= this.OnShowMoreButtons_Click;
            this.showFewerButtons.Click -= this.OnShowFewerButtons_Click;
            this.overflowMenu.PopupOpened -= this.OnOverflowMenu_Shown;
            this.overflowMenu.PopupClosed -= this.overflowMenu_PopupClosed;
            RadPageViewLocalizationProvider.CurrentProviderChanged -= new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        #endregion

        #region Layout

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
            SizeF result = base.ArrangeOverride(finalSize);
            
            RectangleF clientRect = this.GetClientRectangle(finalSize);

            RectangleF buttonsRect = new RectangleF();
            float xCoord = this.RightToLeft ? 0 : clientRect.Width - (this.buttonsContainer.DesiredSize.Width + this.buttonsContainer.Margin.Right);
            buttonsRect.X = xCoord;
            buttonsRect.Y = clientRect.Y;
            buttonsRect.Size = new SizeF(this.buttonsContainer.DesiredSize.Width, clientRect.Height);
            this.buttonsContainer.Arrange(buttonsRect);
            return result;
        }

        #endregion

        #region Methods

        private RadPageViewOutlookAssociatedButton CreateCollapsedItemButton(RadPageViewOutlookItem item)
        {
            RadPageViewOutlookAssociatedButton buttonElement = new RadPageViewOutlookAssociatedButton();
            buttonElement.StretchVertically = true;
            buttonElement.ThemeRole = "ItemAssociatedButton";
            if (item.Image != null)
            {
                buttonElement.Image = item.Image;
            }
            else
            {
                buttonElement.Image = RadPageViewOutlookElement.AssociatedButtonDefaultImage;
            }
            buttonElement.Tag = item;
            return buttonElement;
        }

        public void RegisterCollapsedItem(RadPageViewOutlookItem stackItem)
        {
            Debug.Assert(stackItem.AssociatedOverflowButton == null, "Registering an item which already has an associated overflow button.");
            RadPageViewOutlookAssociatedButton buttonElement = this.CreateCollapsedItemButton(stackItem);
            buttonElement.Click += this.OnHiddenItemButton_Click;
            stackItem.AssociatedOverflowButton = buttonElement;
            buttonElement.ToolTipText = stackItem.Text;
            this.buttonsContainer.Children.Insert(0, buttonElement);
        }

        public void UnregisterCollapsedItem(RadPageViewOutlookItem stackItem)
        {
            Debug.Assert(stackItem.AssociatedOverflowButton != null, "Unregistering an item which does not have an associated overflow button.");
            RadPageViewButtonElement buttonElement = stackItem.AssociatedOverflowButton;
            buttonElement.Click -= this.OnHiddenItemButton_Click;
            this.buttonsContainer.Children.Remove(buttonElement);
            stackItem.AssociatedOverflowButton = null;
        }

        private int GetVisibleAssociatedButtonsCount()
        {
            int count = 0;
            foreach (RadPageViewButtonElement buttonElement in this.buttonsContainer.Children)
            {
                if (!(buttonElement is RadPageViewOutlookAssociatedButton))
                {
                    continue;
                }
                if (buttonElement.Visibility == ElementVisibility.Visible)
                {
                    count++;
                }
            }
            return count;
        }

        private void PrepareDropDownItems()
        {
            foreach (RadMenuItem item in this.addRemoveButtons.Items)
            {
                item.Click -= this.OnAvailableItem_Click;
            }

            this.addRemoveButtons.Items.Clear();

            foreach (RadPageViewOutlookItem item in this.owner.Items)
            {
                RadMenuItem availableItem = new RadMenuItem(item.Text, item);
                availableItem.Image = item.Image;
                availableItem.Layout.ImagePrimitive.ImageScaling = Telerik.WinControls.Enumerations.ImageScaling.SizeToFit;
                availableItem.IsChecked = !this.owner.UncheckedItems.Contains(item);
                availableItem.Click += this.OnAvailableItem_Click;
                this.addRemoveButtons.Items.Add(availableItem);
            }

            int visibleButtonsCount = this.GetVisibleAssociatedButtonsCount();
            this.showMoreButtons.Enabled = visibleButtonsCount > 0;
            this.showFewerButtons.Enabled = !(visibleButtonsCount == this.owner.Items.Count - this.owner.UncheckedItems.Count);
        }

        #endregion

        #region Event handling
         
        private void RadPageViewLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            RadPageViewLocalizationProvider localizationProvider = RadPageViewLocalizationProvider.CurrentProvider;
            this.showMoreButtons.Text = localizationProvider.GetLocalizedString(RadPageViewStringId.ShowMoreButtonsItemCaption);
            this.showFewerButtons.Text = localizationProvider.GetLocalizedString(RadPageViewStringId.ShowFewerButtonsItemCaption);
            this.addRemoveButtons.Text = localizationProvider.GetLocalizedString(RadPageViewStringId.AddRemoveButtonsItemCaption);
        }
         
        private void OnShowFewerButtons_Click(object sender, EventArgs e)
        {
            this.owner.DragGripDown();
        }

        private void OnShowMoreButtons_Click(object sender, EventArgs e)
        {
            this.owner.DragGripUp();
        }

        private void OnAvailableItem_Click(object sender, EventArgs e)
        {
            //TODO: take care for updating the visibility of the associated overflow button
            //when checking/unchecking outlookview items.
            RadMenuItem clickedItem = sender as RadMenuItem;
            RadPageViewOutlookItem item = clickedItem.Tag as RadPageViewOutlookItem;
            if (clickedItem.IsChecked)
            {
                this.owner.UncheckItem(item);

                if (item.AssociatedOverflowButton != null)
                {
                    item.AssociatedOverflowButton.Visibility = ElementVisibility.Collapsed;
                }
            }
            else
            {
                this.owner.CheckItem(item);

                if (item.AssociatedOverflowButton != null)
                {
                    item.AssociatedOverflowButton.Visibility = ElementVisibility.Visible;
                }
            }
            clickedItem.IsChecked = !clickedItem.IsChecked;
        }

        private void OnHiddenItemButton_Click(object sender, EventArgs e)
        {
            RadPageViewButtonElement buttonElement = sender as RadPageViewButtonElement;
            RadPageViewStackItem item = buttonElement.Tag as RadPageViewStackItem;
            this.owner.SelectItem(item);
            this.owner.OnItemAssociatedButtonClick(buttonElement, e);
        }

        private void OnOverflowButtonElement_Click(object sender, EventArgs e)
        {
            this.PrepareDropDownItems();

            if (!this.overflowMenu.IsLoaded)
            {
                this.overflowMenu.LoadElementTree();
            }

            if (this.overflowMenu.ThemeName != this.ElementTree.ThemeName)
            {
                this.overflowMenu.ThemeName = this.ElementTree.ThemeName;
            }

            Rectangle alignmentRectangle = this.ElementTree.Control.RectangleToScreen(this.overflowButtonElement.ControlBoundingRectangle);
            this.overflowMenu.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            this.overflowMenu.HorizontalPopupAlignment = (this.RightToLeft) ? HorizontalPopupAlignment.RightToRight : HorizontalPopupAlignment.LeftToLeft;
            this.overflowMenu.ShowPopup(alignmentRectangle);
        }

        private void overflowMenu_PopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            this.overflowButtonElement.SetValue(RadPageViewOutlookOverflowButton.OverflowMenuOpenedProperty, false);
        }

        private void OnOverflowMenu_Shown(object sender, EventArgs args)
        {
            this.overflowButtonElement.SetValue(RadPageViewOutlookOverflowButton.OverflowMenuOpenedProperty, true);
        }

        #endregion

        #region Cleanup

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();
            base.DisposeManagedResources();
        }

        #endregion
    }
}

