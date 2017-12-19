using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.RibbonBar;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RibbonTabStripElement : RadPageViewStripElement
    {
        #region Fields

        private bool paintTabShadows;
        private RadRibbonBarCommandTabCollection tabItems;

        #endregion

        #region Initialization

        public RibbonTabStripElement()
        {
            this.StretchVertically = false;
            this.UpdateSelectedItemContent = false;
            this.StripButtons = StripViewButtons.None;

            this.SetDefaultValueOverride(RadElement.PaddingProperty, new Padding(1, 0, 1, 0));
            this.ContentArea.SetDefaultValueOverride(RadElement.PaddingProperty, new Padding(2));
            this.ContentArea.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(0, -1, 0, 0));
            this.ItemContainer.SetDefaultValueOverride(RadElement.ZIndexProperty, 1);
            this.ItemContainer.SetDefaultValueOverride(RadElement.MarginProperty, new Padding(4, 0, 4, 0));
            this.ItemContainer.SetDefaultValueOverride(RadElement.PaddingProperty, new Padding(39, 0, 20, 0));

            this.tabItems = new RadRibbonBarCommandTabCollection(this);
            this.tabItems.ItemTypes = new Type[] { typeof(RibbonTab) };
            #pragma warning disable 0618
            this.tabItems.ExcludedTypes = new Type[] { typeof(RadRibbonBarCommandTab) };
            #pragma warning restore 0618
        }

        #endregion

        #region Properties

        public bool PaintTabShadows
        {
            get { return this.paintTabShadows; }
            set { this.paintTabShadows = value; }
        }

        public RadRibbonBarCommandTabCollection TabItems
        {
            get { return this.tabItems; }
            set { this.tabItems = value; }
        }

        #endregion
    }
}