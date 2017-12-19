using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// A class that represents the Border Primitive used in the new RadRibbonForm.
    /// </summary>
    public class RibbonFormBorderPrimitive : BasePrimitive
    {
        #region RadProperties

        public static RadProperty BorderColorProperty = RadProperty.Register(
            "BorderColor",
            typeof(Color),
            typeof(RibbonFormBorderPrimitive),
            new RadElementPropertyMetadata(Color.DarkBlue,
                ElementPropertyOptions.AffectsDisplay));

        public static RadProperty BorderColorProperty1 = RadProperty.Register(
            "BorderColor1",
            typeof(Color),
            typeof(RibbonFormBorderPrimitive),
            new RadElementPropertyMetadata(Color.LightBlue,
                ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ShadowColorProperty = RadProperty.Register(
            "ShadowColor",
            typeof(Color),
            typeof(RibbonFormBorderPrimitive),
            new RadElementPropertyMetadata(Color.FromArgb(30, Color.Black),
                ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private const int OUTER_BORDER_WIDTH = 1;
        private const int INNER_BORDER_WIDTH = 3;



        private RadRibbonBar ribbonBar = null;
        private Bitmap ribbonCaptionFillBitmap = null;

        #endregion

        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the color of the form's first broder.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Determines the color of the form's first border")]
        public Color BorderColor
        {
            get
            {
                return  (Color)this.GetValue(RibbonFormBorderPrimitive.BorderColorProperty);
            }
            set
            {
                this.SetValue(RibbonFormBorderPrimitive.BorderColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the form's second broder.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Determines the color of the form's second border")]
        public Color BorderColor1
        {
            get
            {
                return (Color)this.GetValue(RibbonFormBorderPrimitive.BorderColorProperty1);
            }
            set
            {
                this.SetValue(RibbonFormBorderPrimitive.BorderColorProperty1, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the form's client area shadow.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Determines the color of the form's client area shadow")]
        public Color ShadowColor
        {
            get
            {
                return (Color)this.GetValue(RibbonFormBorderPrimitive.ShadowColorProperty);
            }
            set
            {
                this.SetValue(RibbonFormBorderPrimitive.ShadowColorProperty, value);
            }
        }

        #endregion

        #region Methods

        private RadRibbonBar GetRibbonBar()
        {
            if (this.ribbonBar != null)
            {
                return this.ribbonBar;
            }

            foreach (Control control in this.ElementTree.Control.Controls)
            {
                if (control is RadRibbonBar)
                {
                    RadRibbonBar ribbon = control as RadRibbonBar;
                    ribbon.RibbonBarElement.CaptionFill.PropertyChanged += this.CaptionFill_PropertyChanged;
                    return this.ribbonBar = ribbon;
                }
            }

            return null;
        }

        protected override void OnParentChanged(RadElement previousParent)
        {
            base.OnParentChanged(previousParent);

            if (this.Parent != null)
            {
                this.Parent.BorderThickness =
                    new System.Windows.Forms.Padding(
                        INNER_BORDER_WIDTH + OUTER_BORDER_WIDTH,
                        OUTER_BORDER_WIDTH,
                        INNER_BORDER_WIDTH + OUTER_BORDER_WIDTH,
                        INNER_BORDER_WIDTH + OUTER_BORDER_WIDTH);
            }
        }

        public override void PaintPrimitive(Telerik.WinControls.Paint.IGraphics graphics, float angle, SizeF scale)
        {
            base.PaintPrimitive(graphics, angle, scale);

            if (this.Size.Height == 0 || this.Size.Width == 0)
            {
                return;
            }

            Rectangle innerBorderRect = new Rectangle(
                new Point(OUTER_BORDER_WIDTH, OUTER_BORDER_WIDTH),
                Size.Subtract(this.Size, new Size(OUTER_BORDER_WIDTH * 2, OUTER_BORDER_WIDTH * 2)));


            this.PaintInnerBorder(graphics, innerBorderRect, this.BorderColor1);
            this.PaintTitleBarExtensions(graphics);

            Rectangle outerBorderRect = new Rectangle(Point.Empty, Size.Subtract(this.Size, new Size(1, 1)));
            this.PaintOuterBorder(graphics, outerBorderRect, this.BorderColor);

            this.PaintClientAreaShadow(graphics, this.ShadowColor);


        }

        private void PaintTitleBarExtensions(IGraphics graphics)
        {
            RadRibbonBar ribbonBar = this.GetRibbonBar();

            if (ribbonBar == null)
            {
                return;
            }

            if (ribbonBar.RibbonBarElement.Children.Count > 5)
            {
                FillPrimitive titleBarFill = ribbonBar.RibbonBarElement.CaptionFill as FillPrimitive;
                BorderPrimitive fillPrimitiveBorder = ribbonBar.RibbonBarElement.CaptionBorder as BorderPrimitive;

                if (titleBarFill != null)
                {
                    this.PaintExtensionsFill(graphics, titleBarFill);
                    
                }

                if (fillPrimitiveBorder != null &&
                    fillPrimitiveBorder.Visibility == ElementVisibility.Visible)
                {
                    this.PaintExtensionsBorders(graphics, titleBarFill, fillPrimitiveBorder);
                }
            }
        }

        private void PaintExtensionsFill(IGraphics graphics, FillPrimitive titleBarFill)
        {
            this.ribbonCaptionFillBitmap = titleBarFill.GetAsBitmap(Brushes.Transparent, 0, new SizeF(1, 1));
            if (this.ribbonCaptionFillBitmap == null)
            {
                return;
            }

            Point leftExtFillLocation = titleBarFill.ControlBoundingRectangle.Location;
            leftExtFillLocation.Offset(new Point(0, OUTER_BORDER_WIDTH));

            graphics.DrawBitmap(
                this.ribbonCaptionFillBitmap,
                leftExtFillLocation.X,
                leftExtFillLocation.Y,
                this.Size.Width,
                titleBarFill.ControlBoundingRectangle.Height);

            this.ribbonCaptionFillBitmap.Dispose();
        }

        private void PaintExtensionsBorders(IGraphics graphics, FillPrimitive titleBarFill, BorderPrimitive fillPrimitiveBorder)
        {
            Rectangle leftExtensionRect = new Rectangle(
                            new Point(OUTER_BORDER_WIDTH, titleBarFill.ControlBoundingRectangle.Y + OUTER_BORDER_WIDTH),
                            new Size(INNER_BORDER_WIDTH, titleBarFill.ControlBoundingRectangle.Height));

            int leftExtShadowYCoord = fillPrimitiveBorder.BottomWidth > 1 ? (int)(leftExtensionRect.Bottom - fillPrimitiveBorder.BottomWidth)
                : leftExtensionRect.Bottom - 2;

            int leftExtSBorderYCoord = fillPrimitiveBorder.BottomWidth > 1 ? (int)(leftExtensionRect.Bottom - ( fillPrimitiveBorder.BottomWidth / 2) )
                : leftExtensionRect.Bottom - 1;

            Point leftExtShadowBorderStart = new Point(leftExtensionRect.X, leftExtShadowYCoord);
            Point leftExtShadowBorderEnd = new Point(leftExtensionRect.Right, leftExtShadowYCoord);

            Point leftExtBorderStart = new Point(leftExtensionRect.X, leftExtSBorderYCoord);
            Point leftExtBorderEnd = new Point(leftExtensionRect.Right, leftExtSBorderYCoord);

            graphics.DrawLine(fillPrimitiveBorder.BottomShadowColor, leftExtShadowBorderStart.X, leftExtShadowBorderStart.Y, leftExtShadowBorderEnd.X, leftExtShadowBorderEnd.Y);
            graphics.DrawLine(fillPrimitiveBorder.BottomColor, leftExtBorderStart.X, leftExtBorderStart.Y, leftExtBorderEnd.X, leftExtBorderEnd.Y);


            Rectangle rightExtensionRect = new Rectangle(
                new Point(this.Size.Width - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH), titleBarFill.ControlBoundingRectangle.Y + OUTER_BORDER_WIDTH),
                new Size(INNER_BORDER_WIDTH, titleBarFill.ControlBoundingRectangle.Height));


            int rightExtShadowYCoord = fillPrimitiveBorder.BottomWidth > 1 ? (int)(rightExtensionRect.Bottom - fillPrimitiveBorder.BottomWidth)
             : rightExtensionRect.Bottom - 2;

            int rightExtSBorderYCoord = fillPrimitiveBorder.BottomWidth > 1 ? (int)(rightExtensionRect.Bottom - (fillPrimitiveBorder.BottomWidth / 2) )
                : rightExtensionRect.Bottom - 1;

            Point rightExtShadowBorderStart = new Point(rightExtensionRect.X, rightExtShadowYCoord);
            Point rightExtShadowBorderEnd = new Point(rightExtensionRect.Right, rightExtShadowYCoord);

            Point rightExtBorderStart = new Point(rightExtensionRect.X, rightExtSBorderYCoord);
            Point rightExtBorderEnd = new Point(rightExtensionRect.Right, rightExtSBorderYCoord);

            graphics.DrawLine(fillPrimitiveBorder.BottomShadowColor, rightExtShadowBorderStart.X, rightExtShadowBorderStart.Y, rightExtShadowBorderEnd.X, rightExtShadowBorderEnd.Y);
            graphics.DrawLine(fillPrimitiveBorder.BottomColor, rightExtBorderStart.X, rightExtBorderStart.Y, rightExtBorderEnd.X, rightExtBorderEnd.Y);
        }

        private void PaintOuterBorder(IGraphics graphics, Rectangle rect, Color color)
        {
            OfficeShape shape = new OfficeShape(true);
            GraphicsPath path = shape.GetContourPath(rect);

            graphics.DrawPath(path, color, PenAlignment.Inset, OUTER_BORDER_WIDTH);
            path.Dispose();
        }

        private void PaintInnerBorder(IGraphics graphics, Rectangle rect, Color color)
        {
            graphics.DrawRectangle(rect, color, PenAlignment.Inset, INNER_BORDER_WIDTH);
        }

        private void PaintClientAreaShadow(IGraphics graphics, Color color)
        {
            RadRibbonBar ribbonBar = this.GetRibbonBar();

            if (ribbonBar == null)
            {
                return;
            }

            int captionHeight = ribbonBar.RibbonBarElement.RibbonCaption.ControlBoundingRectangle.Height;

            graphics.DrawLine(color,
                (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH) - 1,
                captionHeight + OUTER_BORDER_WIDTH,
                (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH) - 1,
                this.ControlBoundingRectangle.Height - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH));

            graphics.DrawLine(color,
                (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH),
                this.ControlBoundingRectangle.Height - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH),
                this.ControlBoundingRectangle.Width - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH + 1),
                this.ControlBoundingRectangle.Height - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH));

            graphics.DrawLine(color,
                this.ControlBoundingRectangle.Width - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH),
                captionHeight + OUTER_BORDER_WIDTH,
                this.ControlBoundingRectangle.Width - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH),
                this.ControlBoundingRectangle.Height - (OUTER_BORDER_WIDTH + INNER_BORDER_WIDTH));
        }

        #endregion

        #region Event handling

        private void CaptionFill_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BackColor" ||
                e.PropertyName == "BackColor2" ||
                e.PropertyName == "BackColor3" ||
                e.PropertyName == "BackColor4")
            {
                this.Invalidate();
            }
        }

        #endregion

        #region Cleanup

        protected override void DisposeManagedResources()
        {
            if (this.ribbonBar != null && !(this.ribbonBar.Disposing || this.ribbonBar.IsDisposed))
            {

                this.ribbonBar.RibbonBarElement.CaptionFill.PropertyChanged -= this.CaptionFill_PropertyChanged;
            }
            base.DisposeManagedResources();
        }

        #endregion
    }
}

