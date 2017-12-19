using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class TabStripItem : RadPageViewStripItem,IGeoDoFree
    {
        public static RadProperty CloseButtonOffsetProperty =
            RadProperty.Register("CloseButtonOffset",
                                 typeof(int),
                                 typeof(TabStripItem),
                                 new RadElementPropertyMetadata(5, ElementPropertyOptions.AffectsMeasure |
                                                                    ElementPropertyOptions.AffectsArrange |
                                                                    ElementPropertyOptions.AffectsDisplay));

        #region Fields

        private TabPanel tabPanel;
        private RadImageButtonElement closeButton;
        private bool showCloseButton;
        private RadPageViewStripElement owner;

        #endregion

        #region Initialization

        public TabStripItem(TabPanel tabPanel)
            : base(tabPanel.Text)
        {
            this.Image = tabPanel.Image;
            this.tabPanel = tabPanel;
            this.showCloseButton = false;
            this.ToolTipText = tabPanel.ToolTipText;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.ClipDrawing = false;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.CreateCloseButton();
        }

        protected override void DisposeManagedResources()
        {
            if (this.closeButton != null)
            {
                this.closeButton.Click -= OnCloseButtonClick;
            }
            if (this.owner != null)
            {
                this.owner.PropertyChanged -= new PropertyChangedEventHandler(owner_PropertyChanged);
            }

            base.DisposeManagedResources();
        }

        #endregion

        #region Properties

        private TabStripPanel TabStrip
        {
            get
            {
                if (this.tabPanel != null)
                {
                    return this.tabPanel.Parent as TabStripPanel;
                }

                return null;
            }
        }

        public TabPanel TabPanel
        {
            get { return this.tabPanel; }
        }

        /// <summary>
        /// Gets the RadButtonElement that represents the CloseButton in this TabItem. May return null if ShowCloseButton is false.
        /// </summary>
        public RadImageButtonElement CloseButton
        {
            get
            {
                return this.closeButton;
            }
        }

        /// <summary>
        /// Determines whether the CloseButton of the item will be displayed or not.
        /// </summary>
        public bool ShowCloseButton
        {
            get
            {
                return this.showCloseButton;
            }
            set
            {
                if (this.showCloseButton == value)
                {
                    return;
                }

                this.showCloseButton = value;
                if (this.ElementState == ElementState.Loaded)
                {
                    this.UpdateCloseButton(this.TabStrip);
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset of the close button from the item's ImageAndTextLayout panel.
        /// </summary>
        public int CloseButtonOffset
        {
            get
            {
                return (int)this.GetValue(CloseButtonOffsetProperty);
            }
            set
            {
                this.SetValue(CloseButtonOffsetProperty, value);
            }
        }

        #endregion

        #region Methods

        private void CreateCloseButton()
        {
            this.closeButton = new RadImageButtonElement();
            this.closeButton.Class = "TabCloseButton";
            this.closeButton.BorderElement.Visibility = ElementVisibility.Collapsed;
            this.closeButton.ButtonFillElement.Visibility = ElementVisibility.Hidden;
            this.closeButton.StretchHorizontally = false;
            this.closeButton.StretchVertically = false;
            this.closeButton.Click += OnCloseButtonClick;
            this.closeButton.Visibility = ElementVisibility.Collapsed;

            this.closeButton.SetAllLocalValuesAsDefault(true);
            this.Children.Add(this.closeButton);
        }

        internal void UpdateCloseButton(TabStripPanel owner)
        {
            if (!this.showCloseButton)
            {
                this.closeButton.Visibility = ElementVisibility.Collapsed;
                return;
            }

            this.closeButton.AngleTransform = 360F - this.AngleTransform;
            this.closeButton.Visibility = ElementVisibility.Visible;

            if (owner == null)
            {
                return;
            }

            int offset = this.CloseButtonOffset;
            if (this.TextOrientation == Orientation.Vertical)
            {
                this.closeButton.Alignment = ContentAlignment.TopLeft;
            }
            else
            {
                this.closeButton.Alignment = ContentAlignment.TopRight;
            }
        }

        private void UpdateItemContentOrientation()
        {
            if (!(this.owner is RadPageViewTabStripElement) ||
                this.owner.TextOrientation != Orientation.Vertical)
            {
                return;
            }

            if (this.owner.StripAlignment == StripViewAlignment.Right || this.owner.StripAlignment == StripViewAlignment.Left)
            {
                this.owner.ItemContentOrientation = PageViewContentOrientation.Vertical270;
            }
            else if (this.owner.StripAlignment == StripViewAlignment.Bottom || this.owner.StripAlignment == StripViewAlignment.Top)
            {
                this.owner.ItemContentOrientation = PageViewContentOrientation.Horizontal180;
            }
            else
            {
                this.owner.ResetValue(RadPageViewStripElement.ItemContentOrientationProperty, ValueResetFlags.Local);
            }

            this.SetContentOrientation(this.owner.ItemContentOrientation, false);
        }

        #endregion

        #region Events

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.owner = base.Owner as RadPageViewStripElement;
            this.owner.PropertyChanged += new PropertyChangedEventHandler(owner_PropertyChanged);

            this.UpdateCloseButton(this.TabStrip);
            this.UpdateItemContentOrientation();
        }

        private void owner_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RadPageViewStripElement.StripAlignmentProperty.Name ||
                e.PropertyName == RadPageViewStripElement.TextOrientationProperty.Name)
            {
                this.UpdateItemContentOrientation();
            }
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            if (this.tabPanel == null)
            {
                return;
            }

            TabStripPanel strip = this.tabPanel.Parent as TabStripPanel;
            if (strip != null)
            {
                strip.OnTabCloseButtonClicked(this);
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == AngleTransformProperty ||
                e.Property == CloseButtonOffsetProperty ||
                e.Property == TextOrientationProperty)
            {
                this.UpdateCloseButton(this.TabStrip);
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF size = base.MeasureOverride(availableSize);
            if (!this.showCloseButton)
            {
                return size;
            }

            SizeF finalSize;
            if (this.owner.StripAlignment == StripViewAlignment.Top || this.owner.StripAlignment == StripViewAlignment.Bottom)
            {
                if (this.owner.TextOrientation == Orientation.Horizontal)
                {
                    finalSize = this.IncreaseFinalWidth(size);
                }
                else
                {
                    finalSize = this.IncreaseFinalHeight(size);
                }
            }
            else
            {
                if (this.owner.TextOrientation == Orientation.Horizontal)
                {
                    finalSize = this.IncreaseFinalHeight(size);
                }
                else
                {
                    finalSize = this.IncreaseFinalWidth(size);
                }
            }

            return finalSize;
        }

        private SizeF IncreaseFinalWidth(SizeF size)
        {
            float finalHeight = size.Height > this.closeButton.DesiredSize.Height ? size.Height : this.closeButton.DesiredSize.Height;
            SizeF finalSize = new SizeF(size.Width + this.closeButton.DesiredSize.Width + this.CloseButtonOffset, finalHeight);

            return finalSize;
        }

        private SizeF IncreaseFinalHeight(SizeF size)
        {
            float finalWidth = size.Width > this.closeButton.DesiredSize.Width ? size.Width : this.closeButton.DesiredSize.Width;
            SizeF finalSize = new SizeF(finalWidth, size.Height + this.closeButton.DesiredSize.Height + this.CloseButtonOffset);

            return finalSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            Padding padding = this.RotateTabItemPadding(this.Padding);
            int offsetTop = padding.Top + this.BorderThickness.Top;
            int offsetBottom = padding.Bottom + this.BorderThickness.Bottom;
            int offsetLeft = padding.Left + this.BorderThickness.Left;
            int offsetRight = padding.Right + this.BorderThickness.Right;

            if (this.owner.TextOrientation == Orientation.Horizontal)
            {
                this.ArrangeHorizontally(finalSize, offsetTop, offsetBottom, offsetLeft, offsetRight);
            }
            else
            {
                this.ArrangeVertically(finalSize, offsetTop, offsetBottom, offsetLeft, offsetRight);
            }

            return finalSize;
        }

        private void ArrangeHorizontally(SizeF finalSize, int offsetTop, int offsetBottom, int offsetLeft, int offsetRight)
        {
            RectangleF buttonRect = new RectangleF();
            RectangleF contentRect = new RectangleF();
            switch (this.owner.StripAlignment)
            {
                case StripViewAlignment.Top:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(finalSize.Width - this.closeButton.DesiredSize.Width - offsetRight, finalSize.Height / 2 - this.closeButton.DesiredSize.Height / 2,
                            this.closeButton.DesiredSize.Width, this.closeButton.DesiredSize.Height);
                        contentRect = new RectangleF(offsetLeft, offsetTop,
                            finalSize.Width - offsetLeft - offsetRight - buttonRect.Width - this.CloseButtonOffset, finalSize.Height - offsetTop - offsetBottom);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetLeft, offsetTop,
                            finalSize.Width - offsetLeft - offsetRight, finalSize.Height - offsetTop - offsetBottom);
                    }
                    break;
                case StripViewAlignment.Right:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(finalSize.Width / 2 - this.closeButton.DesiredSize.Width / 2, finalSize.Height - this.closeButton.DesiredSize.Height - offsetBottom,
                            this.closeButton.DesiredSize.Height, this.closeButton.DesiredSize.Width);
                        contentRect = new RectangleF(offsetTop, offsetRight,
                            finalSize.Height - offsetTop - offsetBottom - buttonRect.Height - this.CloseButtonOffset, finalSize.Width - offsetLeft - offsetRight);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetTop, offsetRight,
                            finalSize.Height - offsetTop - offsetBottom, finalSize.Width - offsetLeft - offsetRight);
                    }
                    break;
                case StripViewAlignment.Bottom:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(offsetLeft, finalSize.Height / 2 - this.closeButton.DesiredSize.Height / 2,
                            this.closeButton.DesiredSize.Width, this.closeButton.DesiredSize.Height);
                        contentRect = new RectangleF(offsetLeft + this.closeButton.DesiredSize.Width + this.CloseButtonOffset, offsetTop,
                            finalSize.Width - offsetLeft - offsetRight - buttonRect.Width - this.CloseButtonOffset, finalSize.Height - offsetTop - offsetBottom);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetLeft, offsetTop,
                            finalSize.Width - offsetLeft - offsetRight, finalSize.Height - offsetTop - offsetBottom);
                    }
                    break;
                case StripViewAlignment.Left:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(finalSize.Width / 2 - this.closeButton.DesiredSize.Width / 2, offsetTop,
                            this.closeButton.DesiredSize.Height, this.closeButton.DesiredSize.Width);
                        contentRect = new RectangleF(offsetBottom, offsetRight,
                            finalSize.Height - offsetTop - offsetBottom - buttonRect.Height - this.CloseButtonOffset, finalSize.Width - offsetLeft - offsetRight);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetBottom, offsetRight,
                            finalSize.Height - offsetTop - offsetBottom, finalSize.Width - offsetLeft - offsetRight);
                    }
                    break;
                default:
                    break;
            }

            if (this.showCloseButton)
            {
                this.closeButton.Arrange(buttonRect);
            }
            this.layoutManagerPart.Arrange(contentRect);
        }

        private void ArrangeVertically(SizeF finalSize, int offsetTop, int offsetBottom, int offsetLeft, int offsetRight)
        {
            RectangleF buttonRect = new RectangleF();
            RectangleF contentRect = new RectangleF();
            switch (this.owner.StripAlignment)
            {
                case StripViewAlignment.Top:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(finalSize.Width - this.closeButton.DesiredSize.Width - offsetRight, offsetTop,
                            this.closeButton.DesiredSize.Width, this.closeButton.DesiredSize.Height);
                        contentRect = new RectangleF(offsetLeft, offsetTop + this.closeButton.DesiredSize.Height + this.CloseButtonOffset,
                            finalSize.Width - offsetLeft - offsetRight, finalSize.Height - offsetTop - offsetBottom - buttonRect.Width - this.CloseButtonOffset);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetLeft, offsetTop,
                            finalSize.Width - offsetLeft - offsetRight, finalSize.Height - offsetTop - offsetBottom);
                    }
                    break;
                case StripViewAlignment.Right:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(finalSize.Width - this.closeButton.DesiredSize.Width - offsetRight, offsetTop,
                            this.closeButton.DesiredSize.Height, this.closeButton.DesiredSize.Width);
                        contentRect = new RectangleF(offsetBottom, offsetRight,
                            finalSize.Height - offsetTop - offsetBottom, finalSize.Width - offsetLeft - offsetRight - buttonRect.Height - this.CloseButtonOffset);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetBottom, offsetRight,
                            finalSize.Height - offsetTop - offsetBottom, finalSize.Width - offsetLeft - offsetRight);
                    }
                    break;
                case StripViewAlignment.Bottom:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(finalSize.Width - this.closeButton.DesiredSize.Width - offsetRight, finalSize.Height - this.closeButton.DesiredSize.Height - offsetBottom,
                            this.closeButton.DesiredSize.Width, this.closeButton.DesiredSize.Height);
                        contentRect = new RectangleF(offsetRight, offsetTop + this.closeButton.DesiredSize.Width + this.CloseButtonOffset,
                            finalSize.Width - offsetLeft - offsetRight, finalSize.Height - offsetTop - offsetBottom - buttonRect.Width - this.CloseButtonOffset);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetRight, offsetTop,
                            finalSize.Width - offsetLeft - offsetRight, finalSize.Height - offsetTop - offsetBottom);
                    }
                    break;
                case StripViewAlignment.Left:
                    if (this.showCloseButton)
                    {
                        buttonRect = new RectangleF(offsetLeft, offsetTop,
                            this.closeButton.DesiredSize.Height, this.closeButton.DesiredSize.Width);
                        contentRect = new RectangleF(offsetBottom, offsetRight + this.closeButton.DesiredSize.Width + this.CloseButtonOffset,
                            finalSize.Height - offsetTop - offsetBottom, finalSize.Width - offsetLeft - offsetRight - buttonRect.Height - this.CloseButtonOffset);
                    }
                    else
                    {
                        contentRect = new RectangleF(offsetBottom, offsetRight,
                            finalSize.Height - offsetTop - offsetBottom, finalSize.Width - offsetLeft - offsetRight);
                    }
                    break;
                default:
                    break;
            }

            if (this.showCloseButton)
            {
                this.closeButton.Arrange(buttonRect);
            }
            this.layoutManagerPart.Arrange(contentRect);
        }

        private Padding RotateTabItemPadding(Padding padding)
        {
            switch (this.owner.StripAlignment)
            {
                case StripViewAlignment.Right:
                    padding = new Padding(padding.Bottom, padding.Left, padding.Top, padding.Right);
                    break;
                case StripViewAlignment.Bottom:
                    padding = new Padding(padding.Right, padding.Bottom, padding.Left, padding.Top);
                    break;
                case StripViewAlignment.Left:
                    padding = new Padding(padding.Top, padding.Right, padding.Bottom, padding.Left);
                    break;
                default:
                    break;
            }

            return padding;
        }

        #endregion

        public void Free()
        {
            tabPanel = null;
        }
    }
}