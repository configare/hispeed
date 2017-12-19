using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using Telerik.WinControls.UI.Properties;

namespace Telerik.WinControls.UI
{
    public class StripViewButtonsPanel : RadPageViewButtonsPanel
    {
        #region Fields

        private RadPageViewStripButtonElement itemListButton;
        private RadPageViewStripButtonElement scrollLeftButton;
        private RadPageViewStripButtonElement scrollRightButton;
        private RadPageViewStripButtonElement closeButton;

        #endregion

        #region Constructor/Initializers

        static StripViewButtonsPanel()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new StripViewElementStateManager(), typeof(StripViewButtonsPanel));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Padding = new Padding(2);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            RadPageViewLocalizationProvider localizationProvider = RadPageViewLocalizationProvider.CurrentProvider;

            this.scrollLeftButton = new RadPageViewStripButtonElement();
            this.scrollLeftButton.ThemeRole = "StripViewLeftScrollButton";
            this.scrollLeftButton.Image = Resources.Back;
            this.scrollLeftButton.Tag = StripViewButtons.LeftScroll;
            this.scrollLeftButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.LeftScrollButtonTooltip);
            this.scrollLeftButton.Click += OnButtonClick;
            

            this.scrollRightButton = new RadPageViewStripButtonElement();
            this.scrollRightButton.ThemeRole = "StripViewRightScrollButton";
            this.scrollRightButton.Image = Resources.Next;
            this.scrollRightButton.Tag = StripViewButtons.RightScroll;
            this.scrollRightButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.RightScrollButtonTooltip);
            this.scrollRightButton.Click += OnButtonClick;
            

            this.itemListButton = new RadPageViewStripButtonElement();
            this.itemListButton.ThemeRole = "StripViewItemListButton";
            this.itemListButton.Image = Resources.DropDown2;
            this.itemListButton.Tag = StripViewButtons.ItemList;
            this.itemListButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.ItemListButtonTooltip);
            this.itemListButton.Click += OnButtonClick;
            

            this.closeButton = new RadPageViewStripButtonElement();
            this.closeButton.ThemeRole = "StripViewCloseButton";
            this.closeButton.Image = Resources.Close;
            this.closeButton.Tag = StripViewButtons.Close;
            this.closeButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.CloseButtonTooltip);
            this.closeButton.Click += OnButtonClick;

            if (this.RightToLeft && this.ContentOrientation != PageViewContentOrientation.Vertical270
                    && this.ContentOrientation != PageViewContentOrientation.Vertical90)
            {
                this.Children.Add(this.closeButton);
                this.Children.Add(this.itemListButton);
                this.Children.Add(this.scrollLeftButton);
                this.Children.Add(this.scrollRightButton);
            }
            else
            {
                this.Children.Add(this.scrollLeftButton);
                this.Children.Add(this.scrollRightButton);
                this.Children.Add(this.itemListButton);
                this.Children.Add(this.closeButton);
            }
             
            RadPageViewLocalizationProvider.CurrentProviderChanged += new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        void RadPageViewLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            RadPageViewLocalizationProvider localizationProvider = RadPageViewLocalizationProvider.CurrentProvider;
            this.scrollLeftButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.LeftScrollButtonTooltip);
            this.scrollRightButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.RightScrollButtonTooltip);
            this.itemListButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.ItemListButtonTooltip);
            this.closeButton.ToolTipText = localizationProvider.GetLocalizedString(RadPageViewStringId.CloseButtonTooltip);
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public RadPageViewStripButtonElement ScrollLeftButton
        {
            get
            {
                return this.scrollLeftButton;
            }
        }

        [Browsable(false)]
        public RadPageViewStripButtonElement ScrollRightButton
        {
            get
            {
                return this.scrollRightButton;
            }
        }

        [Browsable(false)]
        public RadPageViewStripButtonElement ItemListButton
        {
            get
            {
                return this.itemListButton;
            }
        }

        [Browsable(false)]
        public RadPageViewStripButtonElement CloseButton
        {
            get
            {
                return this.closeButton;
            }
        }

        #endregion

        #region Implementation

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            RadPageViewLocalizationProvider.CurrentProviderChanged -= new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.UpdateButtonsVisibility();
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadPageViewStripElement.StripButtonsProperty ||
                e.Property == RadPageViewStripElement.ItemFitModeProperty)
            {
                this.UpdateButtonsVisibility();
            }

            if (e.Property == RadElement.RightToLeftProperty)
            {
                this.SuspendLayout();
                this.Children.Clear();
                if (this.RightToLeft && this.ContentOrientation != PageViewContentOrientation.Vertical270 
                    && this.ContentOrientation!= PageViewContentOrientation.Vertical90)
                {
                    this.Children.Add(this.closeButton);
                    this.Children.Add(this.itemListButton);
                    this.Children.Add(this.scrollLeftButton);
                    this.Children.Add(this.scrollRightButton);
                }
                else
                {
                    this.Children.Add(this.scrollLeftButton);
                    this.Children.Add(this.scrollRightButton);
                    this.Children.Add(this.itemListButton);
                    this.Children.Add(this.closeButton);
                }

                this.ResumeLayout(true);
            }
        }

        private void UpdateButtonsVisibility()
        {
            StripViewButtons buttons = (StripViewButtons)this.GetValue(RadPageViewStripElement.StripButtonsProperty);
            if (buttons == StripViewButtons.Auto)
            {
                buttons = this.GetAutoButtons();
            }

            if ((buttons & StripViewButtons.LeftScroll) == StripViewButtons.LeftScroll)
            {
                this.scrollLeftButton.Visibility = ElementVisibility.Visible;
            }
            else
            {
                this.scrollLeftButton.Visibility = ElementVisibility.Collapsed;
            }

            if ((buttons & StripViewButtons.RightScroll) == StripViewButtons.RightScroll)
            {
                this.scrollRightButton.Visibility = ElementVisibility.Visible;
            }
            else
            {
                this.scrollRightButton.Visibility = ElementVisibility.Collapsed;
            }

            if ((buttons & StripViewButtons.ItemList) == StripViewButtons.ItemList)
            {
                this.itemListButton.Visibility = ElementVisibility.Visible;
            }
            else
            {
                this.itemListButton.Visibility = ElementVisibility.Collapsed;
            }

            if ((buttons & StripViewButtons.Close) == StripViewButtons.Close)
            {
                this.closeButton.Visibility = ElementVisibility.Visible;
            }
            else
            {
                this.closeButton.Visibility = ElementVisibility.Collapsed;
            }
        }

        private StripViewButtons GetAutoButtons()
        {
            StripViewItemFitMode fitMode = (StripViewItemFitMode)this.GetValue(RadPageViewStripElement.ItemFitModeProperty);
            if (fitMode == StripViewItemFitMode.None || 
                fitMode == StripViewItemFitMode.Fill)
            {
                return StripViewButtons.VS2005Style;
            }

            return StripViewButtons.VS2008Style;
        }

        #endregion

        #region Event Handlers

        private void OnButtonClick(object sender, EventArgs e)
        {
            StripViewItemContainer parent = this.FindAncestor<StripViewItemContainer>();
            if (parent != null)
            {
                parent.OnStripButtonClicked(sender as RadPageViewStripButtonElement);
            }
        }

        #endregion
    }
}
