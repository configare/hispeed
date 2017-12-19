using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using VisualStyles = System.Windows.Forms.VisualStyles;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents a scrollbar thumb in the scroll bar.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class ScrollBarThumb : RadItem
    {
        #region Fields

        private FillPrimitive fill;
        private BorderPrimitive borderPrimitive;
        private ImagePrimitive gripImage;
        private Point captureCursor, captureLocation;

        #endregion

        #region Constructor & Initialization

        static ScrollBarThumb()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ScrollBarThumbStateManager(), typeof(ScrollBarThumb));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            //at this point we already have an element tree
            //if (this.ElementTree.Control is RadHScrollBar)
            //{
            //    this.gripImage.RotateFlipType = RotateFlipType.Rotate90FlipNone;
            //}
        }

        protected override void CreateChildElements()
        {
            this.fill = new FillPrimitive();
            this.fill.Class = "ScrollBarThumbFill";
            this.Children.Add(this.fill);

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.borderPrimitive.Class = "ScrollBarThumbBorder";
            this.Children.Add(this.borderPrimitive);

            this.gripImage = new ImagePrimitive();
            this.gripImage.Alignment = ContentAlignment.MiddleCenter;
            this.gripImage.Image = this.GripImage;
            this.gripImage.Class = "ScrollBarThumbImagePrimitive";

            this.Children.Add(this.gripImage);

            this.fill.BindProperty(IsPressedProperty, this, IsPressedProperty, PropertyBindingOptions.OneWay);
        }

        #endregion
        
        #region Properties

        public static RadProperty IsPressedProperty = RadProperty.Register(
            "IsPressed",
            typeof(bool),
            typeof(ScrollBarThumb),
            new RadElementPropertyMetadata(
                false,
                ElementPropertyOptions.None));

        /// <summary>
        /// Gets a value indicating whether the thumb is in pressed state.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPressed
        {
            get
            {
                return (bool)this.GetValue(IsPressedProperty);
            }
        }

        /// <summary>
        /// Gets or sets the image associated with the thumb
        /// </summary>
        public Image GripImage
        {
            get
            {
                return this.gripImage.Image;
            }
            set
            {
                this.gripImage.Image = value;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="FillPrimitive"/> contained in the thumb.
        /// </summary>
        public FillPrimitive ThumbFill
        {
            get
            {
                return fill;
            }
        }

        /// <summary>
        /// Gets the <see cref="BorderPrimitive"/> contained in the thumb.
        /// </summary>
        public BorderPrimitive ThumbBorder
        {
            get
            {
                return borderPrimitive;
            }
        }

        #endregion

        #region Event handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if ( (e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                RadScrollBarElement scrollBar = this.Parent as RadScrollBarElement;
                if (scrollBar != null && scrollBar.Enabled)
                {
                    if (!this.UseNewLayoutSystem)
                        this.Parent.SuspendLayout();
                    
                    this.Capture = true;
                    this.SetValue(IsPressedProperty, true);

                    if (this.UseNewLayoutSystem)
                    {
                        this.captureCursor = e.Location;
                        this.captureLocation = this.BoundingRectangle.Location;
                        this.captureLocation.X -= this.Margin.Left;
                        this.captureLocation.Y -= this.Margin.Top;
                    }
                    else
                    {
                        if (this.ElementTree != null)
                            this.captureCursor = this.ElementTree.Control.PointToScreen(e.Location);
                        else
                            this.captureCursor = e.Location;

                        this.captureLocation = Parent.PointToScreen(this.Location);
                    }

                    scrollBar.FireThumbTrackMessage();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //Dido: The first part of the boolean expression is added due to the
            //usage of the scrollbar in the NC area RadForm control and 
            //the consequences that occur because of this.
            if ((e.Delta > 0 && e.Button == MouseButtons.Left
                && !this.Capture) || this.Capture)
            {
                SetPosition(e, true);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!this.Capture)
            {
                this.SetValue(IsPressedProperty, false);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.Capture || (e.Button == MouseButtons.Left && !this.Capture))
            {
                this.Capture = false;
                this.SetValue(IsPressedProperty, false);

                SetPosition(e, false);

                if (!this.UseNewLayoutSystem)
                    this.Parent.ResumeLayout(true);
            }
        }

        #endregion
        
        #region System skinning

        public override System.Windows.Forms.VisualStyles.VisualStyleElement GetXPVisualStyle()
        {
            if ((this.Parent as RadScrollBarElement).ScrollType == ScrollType.Horizontal)
            {
                if (!this.Enabled)
                {
                    return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Disabled;
                }
                else
                {
                    if (!this.IsPressed && !this.IsMouseOver)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Normal;
                    }
                    else if (this.IsPressed)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Pressed;
                    }
                    else
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Hot;
                    }

                }
            }
            else
            {
                if (!this.Enabled)
                {
                    return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonVertical.Disabled;
                }
                else
                {
                    if (!this.IsPressed && !this.IsMouseOver)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonVertical.Normal;
                    }
                    else if (this.IsPressed)
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonVertical.Pressed;
                    }
                    else
                    {
                        return VisualStyles.VisualStyleElement.ScrollBar.ThumbButtonVertical.Hot;
                    }

                }
            }

        }

        public override System.Windows.Forms.VisualStyles.VisualStyleElement GetVistaVisualStyle()
        {
            return this.GetXPVisualStyle();
        }

        protected override void PaintElementSkin(Telerik.WinControls.Paint.IGraphics graphics)
        {
            base.PaintElementSkin(graphics);
            if ((this.Parent as RadScrollBarElement).ScrollType == ScrollType.Horizontal)
            {
                VisualStyles.VisualStyleElement gripperGlyph = VisualStyles.VisualStyleElement.ScrollBar.GripperHorizontal.Normal;

                if (SystemSkinManager.Instance.SetCurrentElement(gripperGlyph))
                {
                    Graphics g = (Graphics)graphics.UnderlayGraphics;
                    Size sz = SystemSkinManager.Instance.GetPartPreferredSize(g, this.Bounds, System.Windows.Forms.VisualStyles.ThemeSizeType.True);
                    Point gripperLocation = new Point((int)(this.ControlBoundingRectangle.Width - sz.Width) / 2, (int)(this.ControlBoundingRectangle.Height - sz.Height) / 2);
                    Rectangle arrowRect = new Rectangle(gripperLocation, sz);
                    SystemSkinManager.Instance.PaintCurrentElement(g, arrowRect);
                }
            }
            else
            {
                VisualStyles.VisualStyleElement gripperGlyph = VisualStyles.VisualStyleElement.ScrollBar.GripperVertical.Normal;

                if (SystemSkinManager.Instance.SetCurrentElement(gripperGlyph))
                {
                    Graphics g = (Graphics)graphics.UnderlayGraphics;
                    Size sz = SystemSkinManager.Instance.GetPartPreferredSize(g, this.Bounds, System.Windows.Forms.VisualStyles.ThemeSizeType.True);
                    Point gripperLocation = new Point((int)(this.ControlBoundingRectangle.Width - sz.Width) / 2, (int)(this.ControlBoundingRectangle.Height - sz.Height) / 2);
                    Rectangle arrowRect = new Rectangle(gripperLocation, sz);
                    SystemSkinManager.Instance.PaintCurrentElement(g, arrowRect);
                }
            }
        }

        protected override bool ShouldPaintChild(RadElement element)
        {
            if (this.paintSystemSkin == true)
            {
                if (element is FillPrimitive || element is BorderPrimitive)
                {
                    return false;
                }
            }

            return base.ShouldPaintChild(element);
        }

        #endregion

        private Point GetNewLocation(MouseEventArgs e)
        {
            Point res = Point.Empty;

            if (this.UseNewLayoutSystem)
            {
                Point mouseDelta = Point.Subtract(e.Location, new Size(this.captureCursor));
                res = Point.Add(mouseDelta, new Size(this.captureLocation));
            }
            else
            {
                Point screenMousePos = e.Location;
                if (this.ElementTree != null)
                    screenMousePos = this.ElementTree.Control.PointToScreen(e.Location);

                Point newLocation = new Point(
                    captureLocation.X + screenMousePos.X - captureCursor.X,
                    captureLocation.Y + screenMousePos.Y - captureCursor.Y);

                res = this.Parent.PointFromScreen(newLocation);
            }

            return res;
        }

        private void SetPosition(MouseEventArgs e, bool dragging)
        {
            RadScrollBarElement scrollBar = this.Parent as RadScrollBarElement;
            if (scrollBar != null)
            {
                scrollBar.SetThumbPosition(GetNewLocation(e), dragging);
            }
        }
	}
}