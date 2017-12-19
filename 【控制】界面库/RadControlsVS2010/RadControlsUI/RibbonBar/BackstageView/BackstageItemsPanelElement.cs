using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the area where backstage items are arranged.
    /// </summary>
    public class BackstageItemsPanelElement : BackstageVisualElement
    {
        #region Fields

        private RadItemOwnerCollection items;
        private BackstageViewElement owner;

        #endregion

        #region Contructors

        public BackstageItemsPanelElement(BackstageViewElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <c ref="BackstageViewElement"/> that owns this element.
        /// </summary>
        public BackstageViewElement Owner
        {
            get
            {
                return owner;
            } 
        }

        /// <summary>
        /// Gets a collection representing the items contained in this BackstageView.
        /// </summary>
        [Editor(DesignerConsts.RadRibbonBarBackstageItemsCollectionEditorString, typeof(UITypeEditor))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this BackstageView.")]
        public RadItemOwnerCollection Items
        {
            get
            {
                return items;
            } 
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            this.items = new RadItemOwnerCollection();
            items.Owner = this;
            base.CreateChildElements();
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            int itemsCount = items.Count;
            float maxWidth = 132f;

            for (int i = 0; i < itemsCount; i++)
            {
                items[i].Measure(availableSize);
                maxWidth = Math.Max(maxWidth, items[i].DesiredSize.Width);
            }

            return new SizeF(maxWidth,availableSize.Height);
        }

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            System.Windows.Forms.Padding border = GetBorderThickness(true);

            int itemsCount = items.Count;
            float currentY = clientRect.Top;

            for (int i = 0; i < itemsCount; i++)
            {
                RadItem currentItem = items[i];
                BackstageTabItem tabItem = (currentItem as BackstageTabItem);
                if (tabItem != null && tabItem == this.owner.SelectedItem)
                { 
                    tabItem.Page.Visible = false;
                }

                currentItem.Arrange(new RectangleF(clientRect.X - border.Left, currentY, 
                    clientRect.Width + border.Horizontal, currentItem.DesiredSize.Height));
                
                currentY += currentItem.DesiredSize.Height;
            }

            return finalSize;
        }

        #endregion

    }
}
