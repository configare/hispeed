using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.Primitives
{
    internal class FillPrimitiveImpl
    {
        private readonly IFillElement fillElement;
        private readonly IPrimitiveElement primitiveElement;
        private SizeF lastScale;
        
        public FillPrimitiveImpl(IFillElement fillElement, IPrimitiveElement radElement)
        {
            this.fillElement = fillElement;
            this.primitiveElement = radElement;        
        }


        private FillElementPaintBuffer fillElementPaintBuffer;

        private FillElementPaintBuffer FillElementPaintBuffer
        {
            get
            {
                //TODO: set fillElementPaintBuffer to null when the control is changed
                if (this.fillElementPaintBuffer == null && this.primitiveElement.ElementTree != null)
                {
                    this.fillElementPaintBuffer = new FillElementPaintBuffer(this.fillElement,
                        this.primitiveElement.ElementTree.ComponentTreeHandler.Behavior.BitmapRepository);
                }

                return this.fillElementPaintBuffer;
            }
        }

        private bool IsTransparent()
        {
            if (this.fillElement.BackColor.A == 0)
            {
                int numberOfColors = this.fillElement.NumberOfColors;
                if (numberOfColors <= 1)
                {
                    return true;
                }

                if (this.fillElement.BackColor2.A == 0)
                {
                    if (numberOfColors <= 2)
                    {
                        return true;
                    }

                    if (this.fillElement.BackColor3.A == 0)
                    {
                        if (numberOfColors <= 3)
                        {
                            return true;
                        }

                        if (this.fillElement.BackColor4.A == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void PaintFill(IGraphics graphics, float angle, SizeF scale)
        {
            RectangleF prefferedRectangle = this.primitiveElement.GetPaintRectangle(this.primitiveElement.BorderThickness, angle, scale);

            PaintFill(graphics, angle, scale, prefferedRectangle);
        }

        public void PaintFill(IGraphics graphics, float angle, SizeF scale, RectangleF paintRect)
        {
            if (this.IsTransparent())
            {
                return;
            }

            Size size = Size.Round(paintRect.Size);
            if ((size.Width <= 0) || (size.Height <= 0))
            {
                return;
            }

            this.lastScale = scale;

            ElementShape currentShape = this.primitiveElement.GetCurrentShape();

            //Check if we have already painded in PaintBuffer with this shape
            FillElementPaintBuffer paintBuffer = this.FillElementPaintBuffer;
            if (currentShape != null && paintBuffer != null)
            {
                string shapeProps = currentShape.SerializeProperties();
                int shapeHash;
                if (!string.IsNullOrEmpty(shapeProps))
                {
                    shapeHash = shapeProps.GetHashCode();
                }
                else
                {
                    shapeHash = currentShape.GetHashCode();
                }

                paintBuffer.SetShapeHash(shapeHash);
            }

            //Graphics graphics = null;
            bool usePaintBuffer = paintBuffer != null && paintBuffer.ShouldUsePaintBuffer() && this.primitiveElement.ShouldUsePaintBuffer();
            try
            {
                if (usePaintBuffer)
                {
                    if (!this.primitiveElement.IsDesignMode && paintBuffer.PaintFromBuffer(graphics, scale, size))
                    {
                        return;
                    }
                    graphics.ChangeOpacity(1d);
                    if (!this.primitiveElement.IsDesignMode)
                    {
                        paintBuffer.SetGraphics(graphics, scale);
                    }
                }
            }
            catch
            {
                usePaintBuffer = false;
            }

            GraphicsPath clippingPath = null;
            if (currentShape != null)
            {
                clippingPath = currentShape.CreatePath(paintRect);
                graphics.PushCurrentClippingPath(clippingPath);
            }

            this.FillRectangle(graphics, paintRect);

            if (clippingPath != null)
            {
                graphics.PopCurrentClippingPath();
                clippingPath.Dispose();
            }

            if (usePaintBuffer)
            {
                graphics.RestoreOpacity();
                if (!this.primitiveElement.IsDesignMode)
                {
                    paintBuffer.ResetGraphics(graphics, scale);
                }
            }
        }

        private void FillRectangle(IGraphics g, RectangleF rect)
        {
            int colorsMaxValue = 4;
            int numberOfColors = this.fillElement.NumberOfColors;
            Color[] colorStops = new Color[Math.Min(Math.Max(numberOfColors, 1), colorsMaxValue)];
            float[] colorOffsets = new float[Math.Min(Math.Max(numberOfColors, 1), colorsMaxValue)];
            this.FillColorStopsAndOffsets(colorStops, colorOffsets, numberOfColors);

            switch (this.fillElement.GradientStyle)
            {
                case GradientStyles.Solid:
                    g.FillRectangle(rect, this.fillElement.BackColor);
                    break;

                case GradientStyles.Glass:
                    //g.FillOfficeGlassRectangle(rect, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2);
                    g.FillGlassRectangle(Rectangle.Round(rect), this.fillElement.BackColor, this.fillElement.BackColor2, this.fillElement.BackColor3, this.fillElement.BackColor4, this.fillElement.GradientPercentage, this.fillElement.GradientPercentage2);
                    break;

                case GradientStyles.OfficeGlass:
                    g.FillOfficeGlassRectangle(Rectangle.Round(rect), this.fillElement.BackColor, this.fillElement.BackColor2, this.fillElement.BackColor3, this.fillElement.BackColor4, this.fillElement.GradientPercentage, this.fillElement.GradientPercentage2, true);
                    break;

                case GradientStyles.OfficeGlassRect:
                    g.FillOfficeGlassRectangle(Rectangle.Round(rect), this.fillElement.BackColor, this.fillElement.BackColor2, this.fillElement.BackColor3, this.fillElement.BackColor4, this.fillElement.GradientPercentage, this.fillElement.GradientPercentage2, false);
                    break;

                case GradientStyles.Vista:
                    g.FillVistaRectangle(Rectangle.Round(rect), this.fillElement.BackColor, this.fillElement.BackColor2, this.fillElement.BackColor3, this.fillElement.BackColor4, this.fillElement.GradientPercentage, this.fillElement.GradientPercentage2);
                    break;

                case GradientStyles.Gel:
                    g.FillGellRectangle(Rectangle.Round(rect), colorStops, this.fillElement.GradientPercentage, this.fillElement.GradientPercentage2);
                    break;

                case GradientStyles.Radial:
                case GradientStyles.Linear:
                    if (numberOfColors < 2 || (numberOfColors == 2 && this.fillElement.BackColor == this.fillElement.BackColor2))
                    {
                        g.FillRectangle(rect, this.fillElement.BackColor);
                    }
                    else
                    {
                        g.FillGradientRectangle(rect, colorStops, colorOffsets, this.fillElement.GradientStyle, this.fillElement.GradientAngle, this.fillElement.GradientPercentage, this.fillElement.GradientPercentage2);
                    }
                    break;
            }
        }

        private void FillColorStopsAndOffsets(Color[] colorStops, float[] colorOffsets, int numberOfColors)
        {
            if (numberOfColors > 0)
            {
                colorStops[0] = this.fillElement.BackColor;
                colorOffsets[0] = 0f;
            }

            if (numberOfColors > 1)
            {
                colorStops[1] = this.fillElement.BackColor2;
                colorOffsets[colorStops.Length - 1] = 1f;
            }
            else
            {
                return;
            }

            if (numberOfColors > 2)
            {
                colorStops[2] = this.fillElement.BackColor3;
                colorOffsets[1] = this.fillElement.GradientPercentage;
            }
            else
            {
                return;
            }

            if (numberOfColors > 3)
            {
                colorStops[3] = this.fillElement.BackColor4;
                colorOffsets[2] = this.fillElement.GradientPercentage2;
            }
        }

        public void OnBoundsChanged(Rectangle oldBounds)
        {
            //Rectangle oldBounds = (Rectangle)e.OldValue;
            FillElementPaintBuffer paintBuffer = this.FillElementPaintBuffer;
            if (paintBuffer != null)
            {
                paintBuffer.RemoveBitmapsBySize(oldBounds.Size, lastScale);
            }
        }

        public void InvalidateFillCache(RadProperty property)
        {
            FillElementPaintBuffer paintBuffer = this.FillElementPaintBuffer;
            if (paintBuffer != null)
            {
                paintBuffer.InvalidateCachedPrimitiveHash(property);
            }
        }        
    }
}
