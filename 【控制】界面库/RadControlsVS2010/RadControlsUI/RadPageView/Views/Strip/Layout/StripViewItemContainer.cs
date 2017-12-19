using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class StripViewItemContainer : RadPageViewElementBase
    {
        #region Fields

        private StripViewItemLayout itemLayout;
        private StripViewButtonsPanel buttonsPanel;

        #endregion

        #region Constructor/Initializers

        static StripViewItemContainer()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new StripViewElementStateManager(), typeof(StripViewItemContainer));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = true;
            this.StretchVertically = false;
            this.ClipDrawing = false;
        }

        protected override void CreateChildElements()
        {
            this.itemLayout = new StripViewItemLayout();
            this.Children.Add(this.itemLayout);

            this.buttonsPanel = new StripViewButtonsPanel();
            this.Children.Add(this.buttonsPanel);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the element which hosts and arranges all the items within the strip.
        /// </summary>
        [Browsable(false)]
        public StripViewButtonsPanel ButtonsPanel
        {
            get
            {
                return this.buttonsPanel;
            }
        }

        /// <summary>
        /// Gets the element which hosts and arranges all the items within the strip.
        /// </summary>
        [Browsable(false)]
        public StripViewItemLayout ItemLayout
        {
            get
            {
                return this.itemLayout;
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            RectangleF client = this.GetClientRectangle(availableSize);

            SizeF measured = this.PerformMeasure(client.Size);
            measured = this.ApplyClientOffset(measured);

            if (this.StretchHorizontally)
            {
                measured.Width = availableSize.Width;
            }
            if (this.StretchVertically)
            {
                measured.Height = availableSize.Height;
            }

            return this.ApplyMinMaxSize(measured);
        }

        private SizeF PerformMeasure(SizeF availableSize)
        {
            this.buttonsPanel.Measure(availableSize);
            SizeF buttonsSize = this.buttonsPanel.DesiredSize;
            StripViewAlignment align = (StripViewAlignment)this.GetValue(RadPageViewStripElement.StripAlignmentProperty);

            switch (align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    availableSize.Width -= buttonsSize.Width + this.buttonsPanel.Margin.Horizontal;
                    this.buttonsPanel.Measure(availableSize);
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    availableSize.Height -= buttonsSize.Height + this.buttonsPanel.Margin.Vertical;
                    break;
            }

            this.itemLayout.Measure(availableSize);
            SizeF stripSize = this.itemLayout.DesiredSize;
            SizeF measured = SizeF.Empty;

            switch (align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    measured.Width = buttonsSize.Width + this.buttonsPanel.Margin.Horizontal +
                        stripSize.Width + this.itemLayout.Margin.Horizontal;
                    measured.Height = Math.Max(buttonsSize.Height + this.buttonsPanel.Margin.Vertical,
                        stripSize.Height + this.itemLayout.Margin.Vertical);
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    measured.Width = Math.Max(buttonsSize.Width + this.buttonsPanel.Margin.Horizontal,
                        stripSize.Width + this.itemLayout.Margin.Horizontal);
                    measured.Height = buttonsSize.Height + this.buttonsPanel.Margin.Vertical +
                        stripSize.Height + this.itemLayout.Margin.Vertical;
                    break;
            }

            return measured;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF available = this.ArrangeButtons(finalSize);
            this.itemLayout.Arrange(available);

            return finalSize;
        }

        private RectangleF ArrangeButtons(SizeF finalSize)
        {
            RectangleF client = this.GetClientRectangle(finalSize);
            SizeF buttonsSize = this.buttonsPanel.DesiredSize;

            StripViewAlignment align = (StripViewAlignment)this.GetValue(RadPageViewStripElement.StripAlignmentProperty);
            RectangleF buttonsRect = RectangleF.Empty;

            switch(align)
            {
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    float left = client.Right - buttonsSize.Width - this.buttonsPanel.Margin.Right;
                    buttonsRect = new RectangleF(left, client.Y, buttonsSize.Width, client.Height);
                    client.Width = buttonsRect.Left - client.Left - this.buttonsPanel.Margin.Left; 
                    if (this.RightToLeft)
                    {
                        client.X = client.X - this.Padding.Left + this.Padding.Right;
                        buttonsRect.X = client.X;
                        client.X += buttonsRect.Width;
                    }
                    break;
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    float top = client.Bottom - buttonsSize.Height - this.buttonsPanel.Margin.Bottom;
                    buttonsRect = new RectangleF(client.X, top, client.Width, buttonsSize.Height);
                    client.Height = buttonsRect.Top - client.Top - this.buttonsPanel.Margin.Top;
                    break;
            }
             
            this.buttonsPanel.Arrange(buttonsRect);

            return client;
        }

        internal void UpdateButtonsEnabledState()
        {
            if (this.itemLayout.LayoutInfo == null)
            {
                foreach (RadButtonElement button in this.buttonsPanel.Children)
                {
                    button.Enabled = false;
                }

                return;
            }

            int visibleItems = this.itemLayout.LayoutInfo.nonSystemItemCount;
            this.buttonsPanel.CloseButton.Enabled = visibleItems > 0;
            this.buttonsPanel.ItemListButton.Enabled = visibleItems > 0;

            this.buttonsPanel.ScrollLeftButton.Enabled = this.itemLayout.CanScroll(StripViewButtons.LeftScroll);
            this.buttonsPanel.ScrollRightButton.Enabled = this.itemLayout.CanScroll(StripViewButtons.RightScroll);
        }

        #endregion

        #region Buttons Notifications

        internal void OnStripButtonClicked(RadPageViewStripButtonElement button)
        {
            RadPageViewStripElement parent = this.FindAncestor<RadPageViewStripElement>();
            if (parent == null)
            {
                return;
            }

            switch ((StripViewButtons)button.Tag)
            {
                case StripViewButtons.Close:
                    parent.CloseItem(parent.SelectedItem);
                    break;
                case StripViewButtons.LeftScroll:
                    this.itemLayout.Scroll(StripViewButtons.LeftScroll);
                    break;
                case StripViewButtons.RightScroll:
                    this.itemLayout.Scroll(StripViewButtons.RightScroll);
                    break;
                case StripViewButtons.ItemList:
                    this.DisplayItemListMenu(parent);
                    break;
            }
        }

        private void DisplayItemListMenu(RadPageViewStripElement view)
        {
            RadPageViewElementBase menuOwner = this.buttonsPanel.ItemListButton;

            HorizontalPopupAlignment hAlign = (this.RightToLeft) ? HorizontalPopupAlignment.RightToRight : HorizontalPopupAlignment.LeftToLeft;
            VerticalPopupAlignment vAlign = VerticalPopupAlignment.TopToBottom;

            switch(view.StripAlignment)
            {
                case StripViewAlignment.Left:
                    hAlign = HorizontalPopupAlignment.LeftToRight;
                    vAlign = VerticalPopupAlignment.TopToTop;
                    break;
                case StripViewAlignment.Right:
                    hAlign = HorizontalPopupAlignment.RightToLeft;
                    vAlign = VerticalPopupAlignment.TopToTop;
                    break;
            }

            view.DisplayItemListMenu(menuOwner, hAlign, vAlign);
        }

        #endregion
    }
}
