using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Data;
using System.Collections;
using System.Drawing.Design;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadPageViewBackstageElement : RadPageViewStripElement
    {
        #region Constructors

        static RadPageViewBackstageElement()
        {
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_RadPageView_Telerik_WinControls_UI_RadPageViewBackstageElement().DeserializeTheme();
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.StripAlignment = (this.RightToLeft) ? StripViewAlignment.Right : StripViewAlignment.Left;
            (this.ItemsParent as RadPageViewElementBase).SetBorderAndFillOrientation(
                (this.RightToLeft) ? PageViewContentOrientation.Horizontal180 : PageViewContentOrientation.Horizontal, false);

            OnPropertyChanged(new RadPropertyChangedEventArgs(StripAlignmentProperty, StripAlignmentProperty.GetMetadata(this), 
                StripViewAlignment.Top, StripViewAlignment.Left));
            this.ItemContentOrientation = PageViewContentOrientation.Horizontal;
            this.ContentArea.BackColor = Color.White;
            this.ContentArea.Shape = null;
            this.ContentArea.BorderBoxStyle = Telerik.WinControls.BorderBoxStyle.FourBorders;
            this.ContentArea.BorderTopWidth = 0;
            this.ContentArea.BorderRightWidth = 0;
            this.ContentArea.BorderBottomWidth = 0;
            this.ContentArea.BorderLeftWidth = 1;
            this.ContentArea.BorderLeftColor = Color.Black;
            this.ContentArea.DrawBorder = true;
            this.BackColor = Color.White;
            this.ItemContainer.ButtonsPanel.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            this.ItemContainer.MinSize = new Size(200, 0);
            this.ItemFitMode |= StripViewItemFitMode.FillHeight;
        }

        internal override void SelectItem(RadPageViewItem item)
        {
            if(item!=null && item.Page.Site == null && item.Page is RadPageViewItemPage)
            {
                return;
            }

            base.SelectItem(item);
        }
         
        protected internal override void CloseItem(RadPageViewItem item)
        {
            return;
        }

        public new StripViewNewItemVisibility NewItemVisibility
        {
            get
            {
                return StripViewNewItemVisibility.Hidden;
            }
            set
            {
                base.NewItemVisibility = value;
            }
        }

        protected override RadPageViewItem OnItemCreating(RadPageViewItemCreatingEventArgs args)
        {
            RadPageViewItemPage page = (args.Page as RadPageViewItemPage);
            if (page != null && page.ItemType == PageViewItemType.GroupHeaderItem)
            {
                args.Item = new RadPageViewStripGroupItem();
            }
            return base.OnItemCreating(args);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RightToLeftProperty)
            {
                this.SetDefaultValueOverride(StripAlignmentProperty, 
                 (this.RightToLeft) ? StripViewAlignment.Right : StripViewAlignment.Left);

                (this.ItemsParent as RadPageViewElementBase).SetBorderAndFillOrientation(
                (this.RightToLeft) ? PageViewContentOrientation.Horizontal180 : PageViewContentOrientation.Horizontal, false);
            }
        }

        #endregion
    }
}
