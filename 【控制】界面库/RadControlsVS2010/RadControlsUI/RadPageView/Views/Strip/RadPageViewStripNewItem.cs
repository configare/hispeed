using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    internal class RadPageViewStripNewItem : RadPageViewStripItem
    {
        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.MinSize = new Size(14, 14);
            this.IsSystemItem = true;
            this.ToolTipText = RadPageViewLocalizationProvider.CurrentProvider.GetLocalizedString(RadPageViewStringId.NewItemTooltipText);
            RadPageViewLocalizationProvider.CurrentProviderChanged += new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        void RadPageViewLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            this.ToolTipText = RadPageViewLocalizationProvider.CurrentProvider.GetLocalizedString(RadPageViewStringId.NewItemTooltipText);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            RadPageViewLocalizationProvider.CurrentProviderChanged -= new EventHandler(RadPageViewLocalizationProvider_CurrentProviderChanged);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.ButtonsPanel.Visibility = ElementVisibility.Collapsed;
        }

        #endregion
    }
}
