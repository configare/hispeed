using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the main visual element of the <c ref="RadRibbonBarBackstageView"/>.
    /// </summary>
    public class BackstageViewElement : BackstageVisualElement
    {
        #region Fields

        //Visual elements
        private BackstageItemsPanelElement itemsElement;
        private BackstageContentPanelElement contentElement;
        private BackstageVisualElement headerItem;
        private BackstageTabItem selectedItem;
        
        //Selection variables
        private RadItem currentItem;
        private int currentIndex = -1;

        #endregion

        static BackstageViewElement()
        {
            new Themes.ControlDefault.RadRibbonBarBackstageView().DeserializeTheme();
        }

        #region Events

        /// <summary>
        /// Fires when an item from the items panel is clicked.
        /// </summary>
        public event EventHandler<BackstageItemEventArgs> ItemClicked;

        /// <summary>
        /// Fires when the selected tab is about to change.
        /// </summary>
        public event EventHandler<BackstageItemChangingEventArgs> SelectedItemChanging;

        /// <summary>
        /// Fires when the selected tab is changed.
        /// </summary>
        public event EventHandler<BackstageItemChangeEventArgs> SelectedItemChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <c ref="BackstageItemsPanelElement"/> on which the backstage items are arranged.
        /// </summary>
        public BackstageItemsPanelElement ItemsPanelElement
        {
            get
            {
                return this.itemsElement;
            }
        }

        /// <summary>
        /// Gets the <c ref="BackstageContentPanelElement"/> on which the backstage pages are arranged.
        /// </summary>
        public BackstageContentPanelElement ContentElement
        {
            get
            {
                return this.contentElement;
            }
        }

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary> 
        [Description("Gets or sets the selected tab.")]
        public BackstageTabItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (this.selectedItem != value)
                {
                    this.SelectItemCore(value);
                }
            }
        }

        /// <summary>
        /// Gets a collection representing the items contained in this backstage view.
        /// </summary>
        [Editor(DesignerConsts.RadRibbonBarBackstageItemsCollectionEditorString, typeof(UITypeEditor))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this backstage view.")]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.itemsElement.Items;
            }
        }

        #endregion

        #region Overrides
        
        protected override void CreateChildElements()
        {
            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.GradientStyle = GradientStyles.Solid;

            this.headerItem = new BackstageVisualElement();
            this.headerItem.DrawText = false;
            this.headerItem.DrawFill = true;
            this.headerItem.MinSize = new Size(0, 3); 
            this.headerItem.Class = "BackstageViewHeader";
            this.Children.Add(headerItem);

            this.itemsElement = new BackstageItemsPanelElement(this);
            this.itemsElement.DrawFill = true;
            this.Children.Add(itemsElement);

            this.contentElement = new BackstageContentPanelElement();
            this.contentElement.DrawFill = true;
            this.Children.Add(contentElement);

            base.CreateChildElements();
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);

            float headerDesiredHeight = this.headerItem.DesiredSize.Height;
            this.headerItem.Arrange(new RectangleF(clientRect.Location, new SizeF(clientRect.Width, headerDesiredHeight)));

            float itemsPanelDesiredWidth = this.itemsElement.DesiredSize.Width;
            float itemsPanelX = (this.RightToLeft) ? clientRect.Right - itemsPanelDesiredWidth : clientRect.Left;

            this.itemsElement.Arrange(new RectangleF(itemsPanelX, clientRect.Top + headerDesiredHeight, 
                itemsPanelDesiredWidth, clientRect.Height - headerDesiredHeight));
            
            float contentElementX = (this.RightToLeft) ? clientRect.Left : clientRect.Left + itemsPanelDesiredWidth;
            this.contentElement.Arrange(new RectangleF(contentElementX, clientRect.Top + headerDesiredHeight,
                clientRect.Width - itemsPanelDesiredWidth, clientRect.Height - headerDesiredHeight));

            if (this.selectedItem != null)
            {
                this.selectedItem.Page.Bounds = this.contentElement.ControlBoundingRectangle;
                this.selectedItem.Page.Visible = true;
            }

            return finalSize;
        }

        #endregion

        #region Methods
        
        internal void ProcessKeyboardSelection(Keys keyCode)
        {
            if (this.currentItem != null &&
                (keyCode == Keys.Space || keyCode == Keys.Enter))
            {
                this.currentItem.CallDoClick(EventArgs.Empty);
                return;
            }

            if ((keyCode == Keys.Right && !this.RightToLeft)
                || (keyCode == Keys.Left) && this.RightToLeft)
            {
                BackstageTabItem tab = this.currentItem as BackstageTabItem;
                if (tab != null)
                {
                    tab.Page.Focus();
                }
                return;
            }

            int dir = GetSelectionDirection(keyCode);
            if (dir == 0)
            {
                return;
            }

            this.HandleKeyboardNavigation(dir);
        }
        
        protected virtual void SelectItemCore(BackstageTabItem backstageTabItem)
        {
            if (this.OnSelectedItemChanging(backstageTabItem))
            {
                return;
            }

            BackstageTabItem oldItem = this.selectedItem;

            int itemsCount = this.Items.Count;

            for (int i = 0; i < itemsCount; i++)
            {
                if (this.Items[i] != backstageTabItem)
                {
                    BackstageTabItem tabItem = (this.Items[i] as BackstageTabItem);
                    if (tabItem != null)
                    {
                        tabItem.Selected = false;
                        tabItem.Page.Visible = false;
                    }
                }
            }
            
            this.selectedItem = backstageTabItem;
            if(!this.ElementTree.Control.Controls.Contains(this.selectedItem.Page))
            {
                this.ElementTree.Control.Controls.Add(this.selectedItem.Page);
            }

            this.selectedItem.Selected = true;

            this.InvalidateMeasure(true);

            this.OnSelectedItemChanged(this.selectedItem, oldItem);
        }

        protected virtual void OnSelectedItemChanged(BackstageTabItem newItem, BackstageTabItem oldItem)
        {
            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(this, new BackstageItemChangeEventArgs(newItem, oldItem));
            }
        }
    
        internal virtual void OnItemClicked(BackstageVisualElement backstageItem)
        {
            this.currentItem = backstageItem;
            this.currentIndex = this.Items.IndexOf(backstageItem);
            this.ResetIsCurrentProperties();

            if (this.ItemClicked != null)
            {
                this.ItemClicked(this, new BackstageItemEventArgs(backstageItem));
            }
        }
 
        protected virtual bool OnSelectedItemChanging(BackstageTabItem backstageTabItem)
        {
            if (this.SelectedItemChanging != null)
            {
                BackstageItemChangingEventArgs args = new BackstageItemChangingEventArgs(backstageTabItem, this.selectedItem);
                this.SelectedItemChanging(this, args);
                return args.Cancel;
            }

            return false;
        }

        #endregion

        #region Helpers

        private void HandleKeyboardNavigation(int dir)
        {
            if (this.currentIndex == -1 || this.currentItem == null)
            {
                this.currentIndex = 0;
                this.currentItem = (this.Items.Count > 0) ? this.Items[0] : null;
            }
            else
            {
                this.currentIndex += dir;
                if (this.currentIndex < 0) this.currentIndex = this.Items.Count - 1;
                if (this.currentIndex >= this.Items.Count) this.currentIndex = 0;
                this.currentItem = (this.currentIndex >= 0 && this.currentIndex < this.Items.Count) ? this.Items[this.currentIndex] : null;
            }

            this.ResetIsCurrentProperties();

            BackstageTabItem currentTab = this.currentItem as BackstageTabItem;
            if (currentTab != null)
            {
                this.SelectedItem = currentTab;
            }
            else if (currentItem != null)
            {
                currentItem.SetValue(BackstageButtonItem.IsCurrentProperty, true);
            }
        }

        private void ResetIsCurrentProperties()
        {
            foreach (RadItem item in this.Items)
            {
                item.SetValue(BackstageButtonItem.IsCurrentProperty, false);
            }
        }

        private int GetSelectionDirection(Keys keyCode)
        {
            if (keyCode == Keys.Up)
            {
                return -1;
            }
            else if (keyCode == Keys.Down)
            {
                return 1;
            }

            return 0;
        }

        #endregion
    }
}
