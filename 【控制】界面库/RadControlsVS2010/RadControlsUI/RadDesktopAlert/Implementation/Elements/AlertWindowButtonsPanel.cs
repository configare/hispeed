using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layout;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the element which holds the buttons
    /// that can be added in a <see cref="RadDesktopAlert"/>window.
    /// </summary>
    public class AlertWindowButtonsPanel : LightVisualElement
    {
        #region Fields

        private WrapLayoutPanel buttonsLayout;
        private RadItemOwnerCollection items;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="RadItemOwnerCollection"/>that
        /// represents the buttons collection of the <see cref="RadDesktopAlert"/>window.
        /// </summary>
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="WrapLayoutPanel"/>
        /// that represents the layout panel which holds the added buttons.
        /// </summary>
        public WrapLayoutPanel ButtonsLayoutPanel
        {
            get
            {
                return this.buttonsLayout;
            }
        }

        #endregion

        #region Methods

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.buttonsLayout = new WrapLayoutPanel();
            this.buttonsLayout.Class = "ButtonsLayoutPanel";
            this.items = new RadItemOwnerCollection();
            this.items.Owner = this.buttonsLayout;
            this.items.ItemTypes = new Type[] { typeof(RadButtonElement)};
            this.items.SealedTypes = new Type []{typeof(RadButtonElement)};
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.Children.Add(this.buttonsLayout);
        }

        #endregion
    }
}
