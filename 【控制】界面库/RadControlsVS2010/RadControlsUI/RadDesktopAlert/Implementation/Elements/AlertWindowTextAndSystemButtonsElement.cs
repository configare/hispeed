using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This element represents the text and system buttons part of a <see cref="RadDesktopAlert"/>component.
    /// </summary>
    public class AlertWindowTextAndSystemButtonsElement : LightVisualElement
    {
        #region Fields

        private TextPrimitive text;
        private RadButtonElement closeButton;
        private RadToggleButtonElement pinButton;
        private RadDropDownButtonElement optionsButton;
        private StackLayoutPanel buttonsLayoutPanel;

        private DockLayoutPanel mainLayoutPanel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="StackLayoutPanel"/>that
        /// represents the layout panel which holds the alert window's
        /// text and system buttons elements.
        /// </summary>
        public DockLayoutPanel MainLayoutPanel
        {
            get
            {
                return this.mainLayoutPanel;
            }
        }


        /// <summary>
        /// Gets an instance of the <see cref="StackLayoutPanel"/>that
        /// represents the layout panel which holds the alert window's caption 
        /// buttons.
        /// </summary>
        public StackLayoutPanel ButtonsLayoutPanel
        {
            get
            {
                return this.buttonsLayoutPanel;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="TextPrimitive"/>class
        /// that represents the text of the<see cref="RadDesktopAlert"/> text 
        /// caption.
        /// </summary>
        public TextPrimitive TextElement
        {
            get
            {
                return this.text;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadButtonElement"/>class
        /// that represents the close button of a <see cref="RadDesktopAlert"/>component.
        /// </summary>
        public RadButtonElement CloseButton
        {
            get
            {
                return this.closeButton;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadToggleButtonElement"/>class
        /// that represents the pin button of a <see cref="RadDesktopAlert"/>component.
        /// </summary>
        public RadToggleButtonElement PinButton
        {
            get
            {
                return this.pinButton;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadImageButtonElement"/>class
        /// that represents the options button of a <see cref="RadDesktopAlert"/>component.
        /// </summary>
        public RadDropDownButtonElement OptionsButton
        {
            get
            {
                return this.optionsButton;
            }
        }

        #endregion

        #region Methods

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.text = new TextPrimitive();
            this.text.Class = "AlertWindowTextCaptionText";

            this.closeButton = new RadButtonElement();
            this.closeButton.SetDefaultValueOverride(RadButtonItem.DisplayStyleProperty, DisplayStyle.Image);
            this.closeButton.ThemeRole = "AlertCloseButton";

            this.pinButton = new RadToggleButtonElement();
            this.pinButton.SetDefaultValueOverride(RadButtonItem.DisplayStyleProperty, DisplayStyle.Image);
            this.pinButton.ThemeRole = "AlertWindowPinButton";

            this.optionsButton = new RadDropDownButtonElement();
            this.optionsButton.ArrowButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            this.optionsButton.ThemeRole = "AlertWindowOptionsButton";

            this.buttonsLayoutPanel = new StackLayoutPanel();
            this.buttonsLayoutPanel.Class = "AlertWindowButtonsLayoutPanel";
            this.mainLayoutPanel = new DockLayoutPanel();
            this.mainLayoutPanel.Class = "AlertWindowMainLayoutPanel";
            this.mainLayoutPanel.LastChildFill = true;

            this.MinSize = new System.Drawing.Size(0, 15);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.buttonsLayoutPanel.Children.Add(this.optionsButton);
            this.buttonsLayoutPanel.Children.Add(this.pinButton);
            this.buttonsLayoutPanel.Children.Add(this.closeButton);

            this.mainLayoutPanel.Children.Add(this.buttonsLayoutPanel);
            this.mainLayoutPanel.Children.Add(this.text);

            DockLayoutPanel.SetDock(this.text, (this.RightToLeft) ? Dock.Right : Dock.Left);
            DockLayoutPanel.SetDock(this.buttonsLayoutPanel, (this.RightToLeft) ? Dock.Left : Dock.Right);

            this.Children.Add(this.mainLayoutPanel);
             
         }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.RightToLeftProperty)
            {
                DockLayoutPanel.SetDock(this.text, (this.RightToLeft) ? Dock.Right : Dock.Left);
                DockLayoutPanel.SetDock(this.buttonsLayoutPanel, (this.RightToLeft) ? Dock.Left : Dock.Right);
            }
        }
        

        #endregion
    }
}
