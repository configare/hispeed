using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadPageViewItemButtonsPanel : RadPageViewButtonsPanel
    {
        #region Fields

        private RadPageViewButtonElement closeButton;
        private RadPageViewItem owner;

        #endregion

        #region Constructor/Initializer

        public RadPageViewItemButtonsPanel(RadPageViewItem owner)
        {
            this.owner = owner;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.closeButton = new RadPageViewButtonElement();
            this.closeButton.ThemeRole = "PageViewItemCloseButton";
            this.closeButton.Click += OnCloseButtonClick;
            this.closeButton.ToolTipText = RadPageViewLocalizationProvider.CurrentProvider.GetLocalizedString(RadPageViewStringId.ItemCloseButtonTooltip);

            this.Children.Add(this.closeButton);
            RadPageViewLocalizationProvider.CurrentProviderChanged += new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.UpdateCloseButton();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="RadPageViewButtonElement">RadPageViewButtonElement</see> instance which represents the CloseButton for the owning item.
        /// </summary>
        [Browsable(false)]
        public RadPageViewButtonElement CloseButton
        {
            get
            {
                return this.closeButton;
            }
        }

        #endregion

        #region Event Handlers

        void RadPageViewLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            this.closeButton.ToolTipText = RadPageViewLocalizationProvider.CurrentProvider.GetLocalizedString(RadPageViewStringId.ItemCloseButtonTooltip);
        }

        protected virtual void OnCloseButtonClick(object sender, EventArgs e)
        {
            if (this.owner.Owner != null)
            {
                this.owner.Owner.CloseItem(this.owner);
            }
        }

        #endregion

        #region Overrides
         
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            RadPageViewLocalizationProvider.CurrentProviderChanged -= new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadPageViewElement.ShowItemCloseButtonProperty)
            {
                this.UpdateCloseButton();
            }
        }

        private void UpdateCloseButton()
        {
            if ((bool)this.GetValue(RadPageViewElement.ShowItemCloseButtonProperty))
            {
                this.closeButton.Visibility = ElementVisibility.Visible;
            }
            else
            {
                this.closeButton.Visibility = ElementVisibility.Collapsed;
            }
        }

        #endregion
    }
}
