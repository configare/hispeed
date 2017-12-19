using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.Primitives
{
    internal class BorderPrimitiveImpl
    {
        private readonly IBorderElement borderElement;
        private readonly IPrimitiveElement primitiveElement;

        public BorderPrimitiveImpl(IBorderElement borderElement, IPrimitiveElement radElement)
        {
            this.borderElement = borderElement;
            this.primitiveElement = radElement;        
        }

        /// <summary>
        /// Virtual method that paints the primitive on the screen. It may be overriden by
        /// the derived types.
        /// </summary>
        public virtual void PaintBorder(IGraphics graphics, float angle, SizeF scale)
        {
            RectangleF paintRectangle = this.primitiveElement.GetPaintRectangle(this.primitiveElement.BorderThickness, angle, scale);
            this.PaintBorder(graphics, angle, scale, paintRectangle);
        }

        private bool IsTransparent()
        {
            if (this.borderElement.BoxStyle == BorderBoxStyle.SingleBorder)
            {
                return IsOuterBorderTransparent();
            }
            else if (this.borderElement.BoxStyle == BorderBoxStyle.OuterInnerBorders)
            {
                return IsOuterBorderTransparent() &&
                       IsInnerBorderTransparent();
            }

            return false;
        }

        private bool IsOuterBorderTransparent()
        {
            if (this.borderElement.ForeColor.A == 0)
            {
                if (this.borderElement.GradientStyle == GradientStyles.Solid)
                {
                    return true;
                }

                if (this.borderElement.ForeColor2.A == 0 &&
                    this.borderElement.ForeColor3.A == 0 &&
                    this.borderElement.ForeColor4.A == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInnerBorderTransparent()
        {
            if (this.borderElement.InnerColor.A == 0)
            {
                if (this.borderElement.GradientStyle == GradientStyles.Solid)
                {
                    return true;
                }

                if (this.borderElement.InnerColor2.A == 0 &&
                    this.borderElement.InnerColor3.A == 0 &&
                    this.borderElement.InnerColor4.A == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Virtual method that paints the primitive on the screen. It may be overriden by
        /// the derived types.
        /// </summary>
        public virtual void PaintBorder(IGraphics graphics, float angle, SizeF scale, RectangleF preferedRectangle)
        {
            Size size = this.primitiveElement.Size;
            if ((size.Width <= 0) || (size.Height <= 0))
            {
                return;
            }

            if (IsTransparent())
            {
                return;
            }

            if (this.borderElement.Width <= 0 &&
                (this.borderElement.BoxStyle != BorderBoxStyle.FourBorders ||
                    (this.borderElement.LeftWidth <= 0 &&
                    this.borderElement.RightWidth <= 0 &&
                    this.borderElement.TopWidth <= 0 &&
                    this.borderElement.BottomWidth <= 0)))
            {
                return;
            }

            ElementShape shape = this.primitiveElement.GetCurrentShape();
            Color[] gradientColors = new Color[]
                                         {
                                             this.borderElement.ForeColor, this.borderElement.ForeColor2,
                                             this.borderElement.ForeColor3, this.borderElement.ForeColor4
                                         };

            GraphicsPath clippingPath = null;

            if (shape != null)
            {
                clippingPath = shape.CreatePath(preferedRectangle);
            }

            if (clippingPath != null)
            {
                if (this.borderElement.BoxStyle == BorderBoxStyle.OuterInnerBorders)
                {
                    float halfWidth = Math.Max(1f, this.borderElement.Width/2f);
                    this.DrawPath(graphics, clippingPath, preferedRectangle, gradientColors, halfWidth);

                    float innerBorderWidth = -(float) Math.Floor(Math.Max(1, halfWidth));
                    RectangleF innerRectangle = RectangleF.Inflate(preferedRectangle, innerBorderWidth, innerBorderWidth);

                    GraphicsPath innerPath = shape.CreatePath(innerRectangle);
                    if (innerPath != null)
                    {
                        Color[] innerColors = new Color[]
                                                  {
                                                      this.borderElement.InnerColor, this.borderElement.InnerColor2,
                                                      this.borderElement.InnerColor3, this.borderElement.InnerColor4
                                                  };
                        this.DrawPath(graphics, innerPath, innerRectangle, innerColors, this.borderElement.Width/2);
                        innerPath.Dispose();
                    }
                }
                else
                {
                    this.DrawPath(graphics, clippingPath, preferedRectangle, gradientColors, this.borderElement.Width);
                }

                clippingPath.Dispose();
            }
            else
            {
                if (this.borderElement.BoxStyle == BorderBoxStyle.OuterInnerBorders)
                {
                    //Rectangle rectangle = new Rectangle(Point.Empty, this.Size);
                    DrawRectangle(graphics, preferedRectangle, gradientColors, this.borderElement.Width);

                    float halfWidth = Math.Max(1f, this.borderElement.Width/2);

                    Color[] innerColors = new Color[]
                                              {
                                                  this.borderElement.InnerColor, this.borderElement.InnerColor2,
                                                  this.borderElement.InnerColor3, this.borderElement.InnerColor4
                                              };
                    float innerBorderWidth = -(float) Math.Floor(Math.Max(1, halfWidth));
                    RectangleF newRectangle = RectangleF.Inflate(preferedRectangle, innerBorderWidth, innerBorderWidth);

                    DrawRectangle(graphics, newRectangle, innerColors, this.borderElement.Width);
                }
                else
                {
                    Rectangle rectangle = Rectangle.Round(preferedRectangle);
                    if (this.borderElement.BoxStyle == BorderBoxStyle.FourBorders)
                    {
                        //Note: Contrary to all other border types, four borders uses FillRectangle and paints exactly 
                        //in the specified rectanle, so we need to recalculate
                        rectangle = Rectangle.Round(this.primitiveElement.GetExactPaintingRectangle(angle, scale));
                    }
                    if ((this.borderElement.Width == 1f) && (this.borderElement.BoxStyle == BorderBoxStyle.SingleBorder))
                    {
                        //we do not need this any longer since GetPaintRectangle does this job already
                        //rectangle = GetPatchedRect(graphics, this.Size, angle, scale);
                        if ((rectangle.Width <= 0) || (rectangle.Height <= 0))
                        {
                            return;
                        }
                    }

                    DrawRectangle(graphics, rectangle, gradientColors, this.borderElement.Width);
                }
            }
        }

        private void DrawRectangle(IGraphics graphics, RectangleF rectangle, Color[] gradientColors, float width)
        {
            if (this.borderElement.BoxStyle == BorderBoxStyle.FourBorders)
            {
                graphics.DrawBorder(rectangle, this.borderElement);
                return;
            }

            if (this.borderElement.GradientStyle == GradientStyles.Solid)
            {
                graphics.DrawRectangle(rectangle, gradientColors[0], PenAlignment.Center, width);
            }
            else if (this.borderElement.GradientStyle == GradientStyles.Linear)
            {
                graphics.DrawLinearGradientRectangle(rectangle, gradientColors, PenAlignment.Center, width, this.borderElement.GradientAngle);
            }
            else
            {
                if (this.borderElement.GradientStyle == GradientStyles.Radial)
                    graphics.DrawRadialGradientRectangle(rectangle, gradientColors[0], gradientColors, PenAlignment.Inset, width);
            }
        }

        private void DrawPath(IGraphics graphics, GraphicsPath path, RectangleF rectangle, Color[] gradientColors, float width)
        {
            PenAlignment penAlignment = (width <= 1) ? PenAlignment.Inset : PenAlignment.Center;

            if (this.borderElement.GradientStyle == GradientStyles.Solid)
            {
                graphics.DrawPath(path, gradientColors[0], penAlignment, width);
            }
            else if (this.borderElement.GradientStyle == GradientStyles.Linear)
            {
                graphics.DrawLinearGradientPath(path, rectangle, gradientColors, penAlignment, width, this.borderElement.GradientAngle);
            }
            else
            {
                if (this.borderElement.GradientStyle == GradientStyles.Radial)
                    graphics.DrawRadialGradientPath(path, Rectangle.Round(rectangle), gradientColors[0],
                                                    new Color[]
					                                	{
					                                		gradientColors[1],
					                                		gradientColors[2],
					                                		gradientColors[3]
					                                	}
                                                    , penAlignment, width);
            }
        }

    }
}
