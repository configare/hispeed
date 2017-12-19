using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.UI.ControlDefault;
using System.Windows.Forms;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the main element of a <see cref="RadDesktopAlert"/>window.
    /// </summary>
    public class RadDesktopAlertElement : LightVisualElement
    {
        #region Fields

        private AlertWindowCaptionElement alertWindowCaptionElement;
        private AlertWindowContentElement alertWindowContentElement;
        private AlertWindowButtonsPanel alertWindowButtonsPanel;
        private bool showCloseButton = true;
        private bool showPinButton = true;
        private bool showOptionsButton = true;

        #endregion

        #region Ctor

        static RadDesktopAlertElement()
        {
            new ControlDefault_Telerik_WinControls_UI_DesktopAlertPopup().DeserializeTheme();
        }

        #endregion

        #region RadProperties

        public static RadProperty CaptionTextProperty = RadProperty.Register(
            "CaptionText",
            typeof(string),
            typeof(RadDesktopAlertElement),
            new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty ContentTextProperty = RadProperty.Register(
            "ContentText",
            typeof(string),
            typeof(RadDesktopAlertElement),
            new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty ContentImageProperty = RadProperty.Register(
            "ContentImage",
            typeof(Image),
            typeof(RadDesktopAlertElement),
            new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsDisplay)
            );


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a boolean value determining whether the options button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the options button is shown.")]
        [DefaultValue(true)]
        public bool ShowOptionsButton
        {
            get
            {
                return this.showOptionsButton;
            }
            set
            {
                if (this.showOptionsButton != value)
                {
                    this.showOptionsButton = value;
                    this.OnNotifyPropertyChanged("ShowOptionsButton");
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the pin button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the pin button is shown.")]
        [DefaultValue(true)]
        public bool ShowPinButton
        {
            get
            {
                return this.showPinButton;
            }
            set
            {
                if (this.showPinButton != value)
                {
                    this.showPinButton = value;
                    this.OnNotifyPropertyChanged("ShowPinButton");
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the close button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the close button is shown.")]
        [DefaultValue(true)]
        public bool ShowCloseButton
        {
            get
            {
                return this.showCloseButton;
            }
            set
            {
                if (this.showCloseButton != value)
                {
                    this.showCloseButton = value;
                    this.OnNotifyPropertyChanged("ShowCloseButton");
                }
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="Image"/>class
        /// that represents the alert's content image.
        /// </summary>
        [Description("Gets or sets the alert's content image.")]
        public Image ContentImage
        {
            get
            {
                return this.GetValue(ContentImageProperty) as Image;
            }
            set
            {
                this.SetValue(ContentImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text of the <see cref="RadDesktopAlertElement"/>caption.
        /// </summary>
        [Description("Gets or sets the text of the alert's caption.")]
        public string CaptionText
        {
            get
            {
                return (string)this.GetValue(CaptionTextProperty);
            }
            set
            {
                this.SetValue(CaptionTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content text of the <see cref="RadDesktopAlertElement"/>.
        /// This is the actual text displayed in a <see cref="RadDesktopAlert"/>.
        /// </summary>
        [Description("Gets or sets the content text of the desktop alert. This text "
            +"is displayed in the content area of the alert's popup.")]
        public string ContentText
        {
            get
            {
                return (string)this.GetValue(ContentTextProperty);
            }
            set
            {
                this.SetValue(ContentTextProperty, value);
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="AlertWindowCaptionElement"/>class
        /// that represents the caption of a <see cref="RadDesktopAlert"/>component.
        /// The caption contains moving grip and system buttons.
        /// </summary>
        [Browsable(false)]
        public AlertWindowCaptionElement CaptionElement
        {
            get
            {
                return this.alertWindowCaptionElement;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="AlertWindowContentElement"/>class
        /// that represents the main content element of a <see cref="RadDesktopAlert"/>component.
        /// This element contains an image and a text element.
        /// </summary>
        [Browsable(false)]
        public AlertWindowContentElement ContentElement
        {
            get
            {
                return this.alertWindowContentElement;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="AlertWindowButtonsPanel"/>class
        /// that represents the panel which holds the buttons added to the 
        /// <see cref="RadDesktopAlert"/>component.
        /// </summary>
        [Browsable(false)]
        public AlertWindowButtonsPanel ButtonsPanel
        {
            get
            {
                return this.alertWindowButtonsPanel;
            }
        }

        #endregion

        #region Methods

        #region Layouts

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF result = base.MeasureOverride(availableSize);

            int reservedVerticalSpace = this.GetVerticalReservedSpace();
            int reservedHorizontalSpace = this.GetHorizontalReservedSpace();

            int availableHorizontalSize = (int)availableSize.Width - reservedHorizontalSpace;
            int availableVerticalSize = (int)availableSize.Height - reservedVerticalSpace;

            float availableHeight = availableVerticalSize - 
                (this.alertWindowCaptionElement.DesiredSize.Height
                + this.alertWindowButtonsPanel.DesiredSize.Height);

            SizeF contentSize = new SizeF(availableHorizontalSize, availableHeight);

            this.alertWindowContentElement.Measure(contentSize);

            result = new SizeF(result.Width,
                this.alertWindowCaptionElement.DesiredSize.Height + this.alertWindowButtonsPanel.DesiredSize.Height + availableHeight);

            return result;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF result = base.ArrangeOverride(finalSize);

            System.Windows.Forms.Padding borderThickness = this.GetBorderThickness();
            System.Windows.Forms.Padding padding = this.Padding;

            int reservedHorizontalSpace = this.GetHorizontalReservedSpace();
            int reservedVerticalSpace = this.GetVerticalReservedSpace();

            int availableHorizontalSize = (int)finalSize.Width - reservedHorizontalSpace;
            int availableVerticalSize = (int)finalSize.Height - reservedVerticalSpace;

            int captionXCoord = borderThickness.Left + padding.Left;
            int captionYCoord = borderThickness.Top + padding.Top;

            RectangleF captionRect = new RectangleF(
                captionXCoord,
                captionYCoord,
                availableHorizontalSize,
                this.alertWindowCaptionElement.DesiredSize.Height);

            this.alertWindowCaptionElement.Arrange(captionRect);

            int footerXCoord = captionXCoord;
            int footerYCoord = (int)finalSize.Height - 
                (borderThickness.Bottom + 
                padding.Bottom + 
                (int)this.alertWindowButtonsPanel.DesiredSize.Height);


            RectangleF footerRectangle = new RectangleF(
                footerXCoord,
                footerYCoord,
                availableHorizontalSize,
                this.alertWindowButtonsPanel.DesiredSize.Height);

            this.alertWindowButtonsPanel.Arrange(footerRectangle);

            float availableContentHeight = availableVerticalSize - (this.alertWindowCaptionElement.DesiredSize.Height + this.alertWindowButtonsPanel.DesiredSize.Height);

            int contentXCoord = this.alertWindowCaptionElement.ControlBoundingRectangle.Left;
            int contextYCoord = this.alertWindowCaptionElement.ControlBoundingRectangle.Bottom;

            RectangleF contentRectangle = new RectangleF(
                contentXCoord,
                contextYCoord,
                availableHorizontalSize,
                availableContentHeight);

            this.alertWindowContentElement.Arrange(contentRectangle);

            return result;
        }

        private int GetHorizontalReservedSpace()
        {
            Padding borderThickness = this.GetBorderThickness();
            int horizontalUsedSpace = this.Padding.Horizontal + borderThickness.Horizontal;
            return horizontalUsedSpace;
        }

        private int GetVerticalReservedSpace()
        {
            Padding borderThickness = this.GetBorderThickness();
            int verticalUsedSpace = this.Padding.Vertical + borderThickness.Vertical;
            return verticalUsedSpace;
        }

        private Padding GetBorderThickness()
        {
            switch(this.BorderBoxStyle)
            {
                case BorderBoxStyle.SingleBorder:
                    {
                        return new Padding((int)this.BorderWidth);
                    }
                case BorderBoxStyle.OuterInnerBorders:
                    {
                        return new Padding((int)this.BorderWidth*2);
                    }
                case BorderBoxStyle.FourBorders:
                    {
                        return new Padding(
                            (int)this.BorderLeftWidth,
                            (int)this.BorderTopWidth,
                            (int)this.BorderRightWidth,
                            (int)this.BorderBottomWidth);
                    }
            }

            return Padding.Empty;
        }

        #endregion

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.alertWindowContentElement = new AlertWindowContentElement();
            this.alertWindowContentElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.alertWindowButtonsPanel = new AlertWindowButtonsPanel();
            this.alertWindowCaptionElement = new AlertWindowCaptionElement();

            this.alertWindowCaptionElement.TextAndButtonsElement.TextElement.BindProperty(
                TextPrimitive.TextProperty, this, RadDesktopAlertElement.CaptionTextProperty, PropertyBindingOptions.OneWay);
            this.alertWindowContentElement.BindProperty(
                LightVisualElement.ImageProperty, this, RadDesktopAlertElement.ContentImageProperty, PropertyBindingOptions.OneWay);
            this.alertWindowContentElement.BindProperty(
                LightVisualElement.TextProperty, this, RadDesktopAlertElement.ContentTextProperty, PropertyBindingOptions.OneWay);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.Children.Add(this.alertWindowCaptionElement);
            this.Children.Add(this.alertWindowContentElement);
            this.Children.Add(this.alertWindowButtonsPanel);
        }

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            switch(propertyName)
            {
                case "ShowCloseButton":
                    {
                        ElementVisibility visibility = this.showCloseButton ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                        this.alertWindowCaptionElement.TextAndButtonsElement.CloseButton.Visibility = visibility;
                        break;
                    }
                case "ShowPinButton":
                    {
                        ElementVisibility visibility = this.showPinButton ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                        this.alertWindowCaptionElement.TextAndButtonsElement.PinButton.Visibility = visibility;
                        break;
                    }
                case "ShowOptionsButton":
                    {
                        ElementVisibility visibility = this.showOptionsButton ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                        this.alertWindowCaptionElement.TextAndButtonsElement.OptionsButton.Visibility = visibility;
                        break;
                    }
            }

            base.OnNotifyPropertyChanged(propertyName);
        }

        #endregion
    }
}
