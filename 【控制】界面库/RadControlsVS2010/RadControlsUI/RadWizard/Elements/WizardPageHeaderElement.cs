using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a page header of RadWizard.
    /// </summary>
    public class WizardPageHeaderElement : BaseWizardElement
    {
        #region Dependency properties

        public static RadProperty IconProperty = RadProperty.Register(
            "Icon", typeof(Image), typeof(WizardPageHeaderElement), new RadElementPropertyMetadata());

        #endregion

        #region Fields

        private WizardTextElement titleElement;
        private WizardTextElement headerElement;
        private BaseWizardElement iconElement;

        private ElementVisibility defaultTitleVisibility;
        private bool setDefaultTitleVisibility;
        private ElementVisibility defaultHeaderVisibility;
        private bool setDefaultHeaderVisibility;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardPageHeaderElement instance.
        /// </summary>
        public WizardPageHeaderElement()
        {
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.titleElement = new WizardTextElement();
            this.titleElement.Class = "TitleElement";
            this.Children.Add(this.titleElement);

            this.headerElement = new WizardTextElement();
            this.headerElement.Class = "HeaderElement";
            this.Children.Add(this.headerElement);

            this.iconElement = new BaseWizardElement();
            this.iconElement.Class = "IconElement";
            this.Children.Add(this.iconElement);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the element containing the WizardPageHeader title text.
        /// </summary>
        public BaseWizardElement TitleElement
        {
            get { return this.titleElement; }
        }

        /// <summary>
        /// Gets or sets the text of TitleElement.
        /// </summary>
        public string Title
        {
            get { return this.titleElement.Text; }
            set { this.titleElement.Text = value; }
        }

        /// <summary>
        /// Gets or sets the TitleElement visibility.
        /// </summary>
        public ElementVisibility TitleVisibility
        {
            get { return this.titleElement.Visibility; }
            set
            {
                this.titleElement.Visibility = value;
                this.defaultTitleVisibility = value;
                this.setDefaultTitleVisibility = true;
            }
        }

        internal ElementVisibility DefaultTitleVisibility
        {
            get { return this.defaultTitleVisibility; }
            set { this.defaultTitleVisibility = value; }
        }

        internal bool SetDefaultTitleVisibility
        {
            get { return this.setDefaultTitleVisibility; }
            set { this.setDefaultTitleVisibility = value; }
        }


        /// <summary>
        /// Gets the element containing the WizardPageHeader header text.
        /// </summary>
        public BaseWizardElement HeaderElement
        {
            get { return this.headerElement; }
        }

        /// <summary>
        /// Gets or sets the text of HeaderElement.
        /// </summary>
        public string Header
        {
            get { return this.headerElement.Text; }
            set { this.headerElement.Text = value; }
        }

        /// <summary>
        /// Gets or sets the HeaderElement visibility.
        /// </summary>
        public ElementVisibility HeaderVisibility
        {
            get { return this.headerElement.Visibility; }
            set
            {
                this.headerElement.Visibility = value;
                this.defaultHeaderVisibility = value;
                this.setDefaultHeaderVisibility = true;
            }
        }

        internal ElementVisibility DefaultHeaderVisibility
        {
            get { return this.defaultHeaderVisibility; }
            set { this.defaultHeaderVisibility = value; }
        }

        internal bool SetDefaultHeaderVisibility
        {
            get { return this.setDefaultHeaderVisibility; }
            set { this.setDefaultHeaderVisibility = value; }
        }


        /// <summary>
        /// Gets the element containing the WizardPageHeader icon image.
        /// </summary>
        public BaseWizardElement IconElement
        {
            get { return this.iconElement; }
        }

        /// <summary>
        /// Gets or sets the WizardPageHeader icon image.
        /// </summary>
        [Description("Gets or sets the WizardPageHeader icon image.")]
        [TypeConverter(typeof(ImageTypeConverter))]
        public Image Icon
        {
            get { return (Image)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or set the alignment of the WizardPageHeader icon image.
        /// </summary>
        public ContentAlignment IconAlignment
        {
            get { return this.iconElement.Alignment; }
            set
            {
                this.iconElement.Alignment = value;
                this.InvalidateMeasure(true);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Updates the current state of the element.
        /// </summary>
        /// <param name="page"></param>
        public override void UpdateInfo(WizardPage page)
        {
            base.UpdateInfo(page);

            this.titleElement.UpdateInfo(page);
            this.headerElement.UpdateInfo(page);
            this.iconElement.UpdateInfo(page);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BoundsProperty && ((Rectangle)e.NewValue).Height != ((Rectangle)e.OldValue).Height &&
                DWMAPI.IsCompositionEnabled && !this.IsDesignMode && this.Owner != null && this.Owner.Mode == WizardMode.Aero && this.Owner.EnableAeroStyle)
            {
                this.UnapplyThemeStyles();
                this.Owner.ApplyAeroStyle();
            }
        }

        #endregion

        #region Methods

        internal void UnapplyThemeStyles()
        {
            if (this.IsDesignMode)
            {
                return;
            }

            this.DrawFill = false;
            this.BackgroundShape = null;
            WizardAeroTopElement topElement = (this.Owner.View as WizardAeroView).TopElement;
            topElement.DrawFill = false;
            topElement.BackgroundShape = null;
        }

        internal void ApplyThemeStyles()
        {
            if (this.IsDesignMode)
            {
                return;
            }

            this.ResetValue(DrawFillProperty, ValueResetFlags.Local);
            this.ResetValue(BackgroundShapeProperty, ValueResetFlags.Local);
            WizardAeroTopElement topElement = (this.Owner.View as WizardAeroView).TopElement;
            topElement.ResetValue(DrawFillProperty, ValueResetFlags.Local);
            topElement.ResetValue(BackgroundShapeProperty, ValueResetFlags.Local);
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Owner.PageHeaderHeight > -1)
            {
                return base.MeasureOverride(new SizeF(availableSize.Width, this.Owner.PageHeaderHeight));
            }

            this.titleElement.Measure(availableSize);
            this.headerElement.Measure(availableSize);
            this.iconElement.Measure(availableSize);

            float height = this.titleElement.DesiredSize.Height + this.headerElement.DesiredSize.Height;
            height += this.Padding.Top + this.Padding.Bottom;

            SizeF desiredSize = new SizeF(availableSize.Width, height);
            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            bool allignRight;
            switch (this.iconElement.Alignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    allignRight = false;
                    break;
                default:
                    allignRight = true;
                    break;
            }
            if (this.RightToLeft)
            {
                allignRight = !allignRight;
            }

            RectangleF iconRect;
            float titleX;
            if (allignRight)
            {
                iconRect = new RectangleF(finalSize.Width - this.iconElement.DesiredSize.Width, this.iconElement.Margin.Top, this.iconElement.DesiredSize.Width, this.iconElement.DesiredSize.Height);
                if (this.RightToLeft)
                {
                    titleX = finalSize.Width - this.titleElement.DesiredSize.Width - this.iconElement.DesiredSize.Width - this.Padding.Right;
                }
                else
                {
                    titleX = this.Padding.Left;
                }
            }
            else
            {
                iconRect = new RectangleF(0, this.iconElement.Margin.Top, this.iconElement.DesiredSize.Width, this.iconElement.DesiredSize.Height);
                if (this.RightToLeft)
                {
                    titleX = finalSize.Width - this.titleElement.DesiredSize.Width;
                }
                else
                {
                    titleX = this.iconElement.DesiredSize.Width;
                }
            }
            iconRect.Height -= this.iconElement.Margin.Top + this.iconElement.Margin.Bottom;
            RectangleF titleRect = new RectangleF(titleX, this.Padding.Top, this.titleElement.DesiredSize.Width, this.titleElement.DesiredSize.Height);
            RectangleF headerRect = new RectangleF(titleX, titleRect.Height, this.headerElement.DesiredSize.Width, this.headerElement.DesiredSize.Height);

            this.titleElement.Arrange(titleRect);
            this.headerElement.Arrange(headerRect);
            this.iconElement.Arrange(iconRect);

            return finalSize;
        }

        #endregion
    }
}