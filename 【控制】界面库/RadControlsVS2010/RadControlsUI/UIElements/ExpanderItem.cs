using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents an expander that is drawn in expander cells
    /// </summary>
    public class ExpanderItem : LightVisualElement
    {
        #region Nested Types

        /// <summary>
        /// Defines the different sign styles
        /// </summary>
        public enum SignStyles
        {
            /// <summary>
            /// plus/minus sign
            /// </summary>
            PlusMinus,

            /// <summary>
            /// up/down arrow
            /// </summary>
            Arrow,

            /// <summary>
            /// image
            /// </summary>
            Image,

            /// <summary>
            /// Triangle
            /// </summary>
            Triangle
        }

        /// <summary>
        /// Defines a lines that will be render around the primitive
        /// </summary>
        [Flags]
        public enum LinkLineOrientation
        {
            None = 0,
            Bottom = 1,
            Top = 2,
            Horizontal = 4
        }

        #endregion

        #region Events

        /// <summary>
        /// This event fires when the expanded state is changed.
        /// </summary>
        public static RoutedEvent ExpandedChangedEvent = RadElement.RegisterRoutedEvent("ExpandedChangedEvent", typeof(ExpanderItem));

        #endregion

        #region Dependancy Properties

        public static readonly RadProperty SignPaddingProperty =
            RadProperty.Register("SignPadding", typeof(Padding), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Padding.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty SignWidthProperty =
            RadProperty.Register("SignWidth", typeof(float), typeof(ExpanderItem),
                new RadElementPropertyMetadata(1f, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty SignBorderWidthProperty =
            RadProperty.Register("SignBorderWidth", typeof(float), typeof(ExpanderItem),
                new RadElementPropertyMetadata(1f, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty SignBorderPaddingProperty =
            RadProperty.Register("SignBorderPadding", typeof(Padding), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Padding.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty DrawSignBorderProperty =
            RadProperty.Register("DrawSignBorder", typeof(bool), typeof(ExpanderItem),
                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty DrawSignFillProperty =
            RadProperty.Register("DrawSignFill", typeof(bool), typeof(ExpanderItem),
                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignBorderColorProperty = RadProperty.Register(
            "SignBorderColor", typeof(Color), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Color.FromKnownColor(KnownColor.Control), ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignBackColorProperty = RadProperty.Register(
            "SignBackColor", typeof(Color), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Color.FromKnownColor(KnownColor.Control), ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignBackColor2Property = RadProperty.Register(
            "SignBackColor2", typeof(Color), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Color.FromKnownColor(KnownColor.Control), ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignBackColor3Property = RadProperty.Register(
            "SignBackColor3", typeof(Color), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Color.FromKnownColor(KnownColor.ControlDark), ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignBackColor4Property = RadProperty.Register(
            "SignBackColor4", typeof(Color), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Color.FromKnownColor(KnownColor.ControlLightLight), ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignNumberOfColorsProperty = RadProperty.Register(
            "SignNumberOfColors", typeof(int), typeof(ExpanderItem),
                new RadElementPropertyMetadata(2, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignGradientStyleProperty = RadProperty.Register(
            "SignGradientStyle", typeof(GradientStyles), typeof(ExpanderItem),
                new RadElementPropertyMetadata(GradientStyles.Linear, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignGradientAngleProperty = RadProperty.Register(
            "SignGradientAngle", typeof(float), typeof(ExpanderItem),
                new RadElementPropertyMetadata(90f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignGradientPercentageProperty = RadProperty.Register(
            "SignGradientPercentage", typeof(float), typeof(ExpanderItem),
                new RadElementPropertyMetadata(0.5f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignGradientPercentage2Property = RadProperty.Register(
            "SignGradientPercentage2", typeof(float), typeof(ExpanderItem),
                new RadElementPropertyMetadata(0.666f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignStyleProperty = RadProperty.Register(
            "SignStyle", typeof(SignStyles), typeof(ExpanderItem),
                new RadElementPropertyMetadata(SignStyles.PlusMinus, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SquareSignSizeProperty = RadProperty.Register(
            "SquareSignSize", typeof(bool), typeof(ExpanderItem),
                new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignSizeProperty = RadProperty.Register(
            "SignSize", typeof(Size), typeof(ExpanderItem),
                new RadElementPropertyMetadata(new Size(9, 9), ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ExpandedProperty = RadProperty.Register(
            "Expanded", typeof(bool), typeof(ExpanderItem),
                new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SignImageProperty = RadProperty.Register(
            "SignImage", typeof(Image), typeof(ExpanderItem),
                new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty LinkLineStyleProperty = RadProperty.Register(
                   "LinkLineStyle", typeof(DashStyle), typeof(ExpanderItem),
                   new RadElementPropertyMetadata(DashStyle.Solid, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty LinkOrientationProperty = RadProperty.Register(
            "LinkOrientation", typeof(LinkLineOrientation), typeof(ExpanderItem),
            new RadElementPropertyMetadata(LinkLineOrientation.None, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty LinkLineColorProperty = RadProperty.Register(
            "LinkLineColor", typeof(Color), typeof(ExpanderItem),
                new RadElementPropertyMetadata(Color.Black, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private Image cachedSignImage = null;

        #endregion

        #region Initializtion and Dispose

        static ExpanderItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ExpanderItemStateManager(), typeof(ExpanderItem));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderItem"/> class.
        /// </summary>
        public ExpanderItem() { }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.ShouldHandleMouseInput = true;
            this.StretchHorizontally = false;
            this.StretchVertically = true;
            this.BypassLayoutPolicies = true;
            this.ShouldPaint = true;
        }

        protected override void DisposeManagedResources()
        {
            if (this.cachedSignImage != null)
            {
                this.cachedSignImage.Dispose();
                this.cachedSignImage = null;
            }

            base.DisposeManagedResources();
        }

        #endregion

        #region Properties

        /// <summary>
        ///  Gets or sets the padding sizes of the sign.
        /// </summary>
        public Padding SignPadding
        {
            get { return (Padding)this.GetValue(SignPaddingProperty); }
            set { this.SetValue(SignPaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the sign.
        /// </summary>
        public float SignWidth
        {
            get { return (float)this.GetValue(SignWidthProperty); }
            set { this.SetValue(SignWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border width of the sign.
        /// </summary>
        public float SignBorderWidth
        {
            get { return (float)this.GetValue(SignBorderWidthProperty); }
            set { this.SetValue(SignBorderWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the padding sizes of the border around the sign.
        /// </summary>
        public Padding SignBorderPadding
        {
            get { return (Padding)this.GetValue(SignBorderPaddingProperty); }
            set { this.SetValue(SignBorderPaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating that the sign's border must be drawn
        /// </summary>
        public bool DrawSignBorder
        {
            get { return (bool)this.GetValue(DrawSignBorderProperty); }
            set { this.SetValue(DrawSignBorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating that the sign's fill must be drawn
        /// </summary>
        public bool DrawSignFill
        {
            get { return (bool)this.GetValue(DrawSignFillProperty); }
            set { this.SetValue(DrawSignFillProperty, value); }
        }

        /// <summary>
        /// Gets or sets the sign's border color
        /// </summary>
        public virtual Color SignBorderColor
        {
            get { return (Color)GetValue(SignBorderColorProperty); }
            set { this.SetValue(SignBorderColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets sign's back color
        /// </summary>
        public virtual Color SignBackColor
        {
            get { return (Color)GetValue(SignBackColorProperty); }
            set { this.SetValue(SignBackColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets sign's second back color
        /// </summary>
        public virtual Color SignBackColor2
        {
            get { return (Color)GetValue(SignBackColor2Property); }
            set { this.SetValue(SignBackColor2Property, value); }
        }

        /// <summary>
        /// Gets or sets sign's third back color
        /// </summary>
        public virtual Color SignBackColor3
        {
            get { return (Color)GetValue(SignBackColor3Property); }
            set { this.SetValue(SignBackColor3Property, value); }
        }

        /// <summary>
        /// Gets or sets sign's fourth back color
        /// </summary>
        public virtual Color SignBackColor4
        {
            get { return (Color)GetValue(SignBackColor4Property); }
            set { this.SetValue(SignBackColor4Property, value); }
        }

        /// <summary>
        /// Gets or sets the number of colors used for drawing sign's background
        /// </summary>
        public virtual int SignNumberOfColors
        {
            get { return (int)GetValue(SignNumberOfColorsProperty); }
            set { this.SetValue(SignNumberOfColorsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the gradient style of sign's background
        /// </summary>
        public virtual GradientStyles SignGradientStyle
        {
            get { return (GradientStyles)GetValue(SignGradientStyleProperty); }
            set { this.SetValue(SignGradientStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the gradient angle of sign's background
        /// </summary>
        public virtual float SignGradientAngle
        {
            get { return (float)GetValue(SignGradientAngleProperty); }
            set { this.SetValue(SignGradientAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the gradient percentage of sign's background
        /// </summary>
        public virtual float SignGradientPercentage
        {
            get { return (float)GetValue(SignGradientPercentageProperty); }
            set { this.SetValue(SignGradientPercentageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the second gradient percentage of sign's background
        /// </summary>
        public virtual float SignGradientPercentage2
        {
            get { return (float)GetValue(SignGradientPercentage2Property); }
            set { this.SetValue(SignGradientPercentage2Property, value); }
        }

        /// <summary>
        /// Gets or sets the sign's style
        /// </summary>
        public virtual SignStyles SignStyle
        {
            get { return (SignStyles)GetValue(SignStyleProperty); }
            set { this.SetValue(SignStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating that the sign must maintain square size
        /// </summary>
        public virtual bool SquareSignSize
        {
            get { return (bool)GetValue(SquareSignSizeProperty); }
            set { this.SetValue(SquareSignSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the sign's size
        /// </summary>
        public virtual Size SignSize
        {
            get { return (Size)GetValue(SignSizeProperty); }
            set { this.SetValue(SignSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sign is in expanded or collapsed state
        /// </summary>
        public virtual bool Expanded
        {
            get { return (bool)GetValue(ExpandedProperty); }
            set { this.SetValue(ExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the sign image.
        /// </summary>
        [TypeConverter(typeof(ImageTypeConverter))]
        public virtual Image SignImage
        {
            get { return this.cachedSignImage; }
            set { this.SetValue(SignImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value detemining the link lines that be rendered around the expander sign
        /// </summary>
        public LinkLineOrientation LinkOrientation
        {
            get { return (LinkLineOrientation)this.GetValue(LinkOrientationProperty); }
            set { this.SetValue(LinkOrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value determining the style of the link lines
        /// </summary>
        public DashStyle LinkLineStyle
        {
            get { return (DashStyle)this.GetValue(LinkLineStyleProperty); }
            set { this.SetValue(LinkLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value determining the color of the link lines
        /// </summary>
        public Color LinkLineColor
        {
            get { return (Color)this.GetValue(LinkLineColorProperty); }
            set { this.SetValue(LinkLineColorProperty, value); }
        }

        #endregion

        #region Event Handlers

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == SignImageProperty)
            {
                if (e.NewValue != null)
                {
                    this.cachedSignImage = (Image)((Image)e.NewValue).Clone();
                }
                else
                {
                    this.cachedSignImage = null;
                }
            }
            else if (e.Property == ExpandedProperty)
            {
                this.RaiseBubbleEvent(this, new RoutedEventArgs(EventArgs.Empty, ExpandedChangedEvent));
            }

            base.OnPropertyChanged(e);
        }

        protected internal override bool IsPropertyCancelable(RadPropertyMetadata metadata)
        {
            return true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.ToggleExpanded();
            }
        }

        protected virtual void ToggleExpanded()
        {
            this.Expanded = !this.Expanded;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);

            SizeF desiredSize = this.SignSize;

            if (this.SignStyle == SignStyles.Image && this.cachedSignImage != null)
            {
                desiredSize = this.cachedSignImage.Size;
            }

            desiredSize.Width += this.Padding.Horizontal;
            desiredSize.Height += this.Padding.Vertical;

            return desiredSize;
        }

        #endregion

        #region Painting

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            base.PaintElement(graphics, angle, scale);

            int width = 0;
            int height = 0;
            int left = 0;
            int top = 0;

            if (SquareSignSize)
            {
                width = Math.Min(this.SignSize.Width, this.SignSize.Height);
                height = width;
            }
            else if (this.SignStyle == SignStyles.Image && this.cachedSignImage != null)
            {
                width = this.cachedSignImage.Size.Width;
                height = this.cachedSignImage.Size.Height;
            }
            else
            {
                width = this.SignSize.Width;
                height = this.SignSize.Height;
            }

            Rectangle clientRect = new Rectangle(this.Padding.Left, this.Padding.Top,
                this.Size.Width - this.Padding.Horizontal,
                this.Size.Height - this.Padding.Vertical);

            width = Math.Min(width, clientRect.Width);
            height = Math.Min(height, clientRect.Height);

            left = clientRect.Left + (clientRect.Width - width) / 2;
            top = clientRect.Top + (clientRect.Height - height) / 2;

            Rectangle signBorder = new Rectangle(left, top, width, height);

            if (this.DrawSignFill)
            {
                this.PaintFill(graphics, signBorder);
            }

            if (this.DrawSignBorder)
            {
                if (SignBorderWidth == 1)
                {
                    signBorder.Width--;
                    signBorder.Height--;
                }
                this.PaintBorder(graphics, signBorder);
            }

            left = signBorder.Left + SignPadding.Left;
            top = signBorder.Top + SignPadding.Top;
            width = signBorder.Width - SignPadding.Horizontal - 1;
            height = signBorder.Height - SignPadding.Vertical - 1;

            if (DrawSignBorder)
            {
                left += (int)SignBorderWidth;
                top += (int)SignBorderWidth;
                width -= (int)SignBorderWidth * 2;
                height -= (int)SignBorderWidth * 2;
                if (SignBorderWidth == 1)
                {
                    width++;
                    height++;
                }
            }

            Rectangle signRect = new Rectangle(left, top, width, height);

            this.PaintSign(graphics, signRect);

            this.PaintSignLines(graphics, signRect, signBorder);
        }

        /// <summary>
        /// Paints the sign's fill
        /// </summary>
        /// <param name="g">The IGraphics to use for painting the sign's fill</param>
        /// <param name="rect">Rectangle containing sign bounds</param>
        protected virtual void PaintFill(IGraphics g, Rectangle rect)
        {
            if (SignBackColor.A == 0)
            {
                if (SignNumberOfColors <= 1)
                    return;

                if (SignBackColor2.A == 0)
                {
                    if (SignNumberOfColors <= 2)
                        return;

                    if (SignBackColor3.A == 0)
                    {
                        if (SignNumberOfColors <= 3)
                            return;

                        if (SignBackColor4.A == 0)
                        {
                            return;
                        }
                    }
                }
            }

            if ((this.Size.Width <= 0) || (this.Size.Height <= 0))
                return;

            int colorsMaxValue = 4;
            Color[] colorStops = new Color[Math.Min(Math.Max(this.SignNumberOfColors, 1), colorsMaxValue)];
            float[] colorOffsets = new float[Math.Min(Math.Max(this.SignNumberOfColors, 1), colorsMaxValue)];

            if (this.SignNumberOfColors > 0)
            {
                colorStops[0] = SignBackColor;
                colorOffsets[0] = 0f;
            }

            if (this.SignNumberOfColors > 1)
            {
                colorStops[1] = SignBackColor2;
                colorOffsets[colorStops.Length - 1] = 1f;
            }

            if (this.SignNumberOfColors > 2)
            {
                colorStops[2] = SignBackColor3;
                colorOffsets[1] = this.SignGradientPercentage;
            }

            if (this.SignNumberOfColors > 3)
            {
                colorStops[3] = SignBackColor4;
                colorOffsets[2] = this.SignGradientPercentage2;
            }

            switch (SignGradientStyle)
            {
                case GradientStyles.Solid:
                    g.FillRectangle(rect, SignBackColor);
                    break;

                case GradientStyles.Glass:
                    g.FillGlassRectangle(rect, SignBackColor, SignBackColor2, SignBackColor3, SignBackColor4, this.SignGradientPercentage, this.SignGradientPercentage2);
                    break;

                case GradientStyles.OfficeGlass:
                    g.FillOfficeGlassRectangle(rect, SignBackColor, SignBackColor2, SignBackColor3, SignBackColor4, this.SignGradientPercentage, this.SignGradientPercentage2, true);
                    break;

                case GradientStyles.OfficeGlassRect:
                    g.FillOfficeGlassRectangle(rect, SignBackColor, SignBackColor2, SignBackColor3, SignBackColor4, this.SignGradientPercentage, this.SignGradientPercentage2, false);
                    break;

                case GradientStyles.Vista:
                    g.FillVistaRectangle(rect, SignBackColor, SignBackColor2, SignBackColor3, SignBackColor4, this.SignGradientPercentage, this.SignGradientPercentage2);
                    break;

                case GradientStyles.Gel:
                    g.FillGellRectangle(rect, colorStops, this.SignGradientPercentage, this.SignGradientPercentage2);
                    break;

                case GradientStyles.Radial:
                case GradientStyles.Linear:
                    if (SignNumberOfColors < 2)
                        g.FillRectangle(rect, SignBackColor);
                    else
                        g.FillGradientRectangle(rect, colorStops, colorOffsets, SignGradientStyle, SignGradientAngle, this.SignGradientPercentage, this.SignGradientPercentage2);
                    break;
            }
        }

        /// <summary>
        /// Paint the sign's border
        /// </summary>
        /// <param name="g">The IGraphics to use for painting the sign's border</param>
        /// <param name="signBorder">Rectangle containing sign bounds</param>
        protected virtual void PaintBorder(IGraphics g, Rectangle signBorder)
        {
            g.DrawRectangle(signBorder, this.SignBorderColor, PenAlignment.Inset, this.SignBorderWidth);
        }

        /// <summary>
        /// Paints the sign
        /// </summary>
        /// <param name="g">The IGraphics to use fo painting the sign</param>
        /// <param name="signRect">Rectangle containing sign bounds</param>
        protected virtual void PaintSign(IGraphics g, Rectangle signRect)
        {
            Rectangle signRealRectangle = Rectangle.Empty;

            if (this.SignStyle == SignStyles.Image)
            {
                if (cachedSignImage != null)
                {
                    int centerX = Math.Max(0, (this.Size.Width - cachedSignImage.Width) / 2);
                    int centerY = Math.Max(0, (this.Size.Height - cachedSignImage.Height) / 2);

                    Point pos = new Point(Math.Max(0, centerX), Math.Max(0, centerY));
                    pos.X = Math.Min(pos.X, this.Size.Width);
                    pos.Y = Math.Min(pos.Y, this.Size.Height);
                    Size sz = new Size(
                        Math.Min(this.Size.Width, cachedSignImage.Size.Width),
                        Math.Min(this.Size.Height, cachedSignImage.Size.Height));

                    Graphics rawGraphics = (Graphics)g.UnderlayGraphics;

                    Image imageToDraw = cachedSignImage;

                    if (this.Opacity != 1)
                    {
                        imageToDraw = new Bitmap(cachedSignImage);
                        ImageHelper.ApplyAlpha(imageToDraw as Bitmap, Convert.ToSingle(this.Opacity));
                    }

                    rawGraphics.DrawImageUnscaledAndClipped(imageToDraw, new Rectangle(pos, sz));
                    signRealRectangle = new Rectangle(pos, sz);
                }
            }
            else
            {
                if (signRect.Width <= SignWidth || signRect.Height <= SignWidth)
                {
                    return;
                }

                using (Pen pen = new Pen(this.ForeColor, SignWidth))
                {
                    pen.Alignment = PenAlignment.Inset;
                    Graphics gr = (Graphics)g.UnderlayGraphics;

                    if (this.SignStyle == SignStyles.PlusMinus)
                    {
                        if (this.Expanded)
                        {
                            gr.DrawLine(pen, signRect.Left, signRect.Top + signRect.Height / 2, signRect.Right, signRect.Top + signRect.Height / 2);
                        }
                        else
                        {
                            gr.DrawLine(pen, signRect.Left, signRect.Top + signRect.Height / 2, signRect.Right, signRect.Top + signRect.Height / 2);
                            gr.DrawLine(pen, signRect.Left + signRect.Width / 2, signRect.Top, signRect.Left + signRect.Width / 2, signRect.Bottom);
                        }
                    }
                    else if (this.SignStyle == SignStyles.Arrow)
                    {
                        if (this.Expanded)
                        {
                            gr.DrawLine(pen, signRect.Left, signRect.Bottom, signRect.Left + signRect.Width / 2, signRect.Top);
                            gr.DrawLine(pen, signRect.Left + signRect.Width / 2, signRect.Top, signRect.Right, signRect.Bottom);
                        }
                        else
                        {
                            gr.DrawLine(pen, signRect.Left, signRect.Top, signRect.Left + signRect.Width / 2, signRect.Bottom);
                            gr.DrawLine(pen, signRect.Left + signRect.Width / 2, signRect.Bottom, signRect.Right, signRect.Top);
                        }
                    }
                    else if (this.SignStyle == SignStyles.Triangle)
                    {
                        using (SolidBrush brush = new SolidBrush(this.ForeColor))
                        {
                            if (this.Expanded)
                            {
                                Point point1 = new Point(signRect.X, signRect.Bottom);
                                Point point2 = new Point(signRect.Right, signRect.Bottom);
                                Point point3 = new Point(signRect.X + signRect.Width / 2, signRect.Y);

                                Point[] trianglePoints = new Point[] { point1, point2, point3 };

                                gr.FillPolygon(brush, trianglePoints);
                            }
                            else
                            {
                                Point point1 = new Point(signRect.X, signRect.Y);
                                Point point2 = new Point(signRect.X + signRect.Width / 2, signRect.Bottom);
                                Point point3 = new Point(signRect.Right, signRect.Y);

                                Point[] trianglePoints = new Point[] { point1, point2, point3 };

                                gr.FillPolygon(brush, trianglePoints);
                            }
                        }
                    }
                }
            }

        }

        protected virtual void PaintSignLines(IGraphics g, Rectangle signRect, Rectangle signBorder)
        {
            if (this.LinkOrientation == LinkLineOrientation.None)
            {
                return;
            }

            int x = signRect.Left + signRect.Width / 2;

            if ((this.LinkOrientation & LinkLineOrientation.Top) != 0)
            {
                int y = this.Padding.Top;
                int bottom = signBorder.Top - 1;
                g.DrawLine(this.LinkLineColor, this.LinkLineStyle, x, y, x, bottom);
            }

            if ((this.LinkOrientation & LinkLineOrientation.Bottom) != 0)
            {
                int y = signBorder.Bottom;
                int bottom = this.Size.Height - this.Padding.Vertical;

                if (DrawSignBorder && SignBorderWidth == 1)
                {
                    y++;
                }
                g.DrawLine(this.LinkLineColor, this.LinkLineStyle, x, y, x, bottom);
            }

            if ((this.LinkOrientation & LinkLineOrientation.Horizontal) != 0)
            {
                int y = this.Size.Height / 2;
                if (RightToLeft)
                {
                    x = signRect.Left;
                    g.DrawLine(this.LinkLineColor, this.LinkLineStyle, 0, y, x, y);
                }
                else
                {
                    x = signRect.Right;
                    g.DrawLine(this.LinkLineColor, this.LinkLineStyle, x, y, this.Size.Width, y);
                }
            }
        }

        #endregion
    }
}
