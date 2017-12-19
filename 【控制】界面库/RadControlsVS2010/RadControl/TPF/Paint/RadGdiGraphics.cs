using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.Paint
{
    /// <summary>Implements functionality for drawing GDI+ graphics.</summary>
    public class RadGdiGraphics : IGraphics, IDisposable
    {       
        private Graphics graphics = null;
        private SmoothingMode oldSmoothingMode = SmoothingMode.None;
        private double opacity = 1;
        private double oldOpacity = 1;

        private Rectangle clipRectangle;

        private Stack<GraphicsPath> clippingPathStack;

        [ThreadStatic]
        private static Graphics measurementGraphics;

        public static Graphics MeasurementGraphics
        {
            get
            {
                if (measurementGraphics == null)
                {
                    measurementGraphics = Telerik.WinControls.MeasurementGraphics.CreateMeasurementGraphics().Graphics;
                }
                return measurementGraphics;
            }           
        }

        public Rectangle ClipRectangle
        {
            get
            {
                return this.clipRectangle;
            }
            set
            {
                this.clipRectangle = value;
            }
        }

        public object UnderlayGraphics
        {
            get { return graphics; }
        }

        public double Opacity
        {
            get
            {
                return opacity;
            }
        }

        /// <summary>
        /// Initializes a new instance of RadGdiGraphics class using GDI graphics context. 
        /// </summary>
        /// <param name="graphics"></param>
        public RadGdiGraphics(Graphics graphics)
        {
            this.graphics = graphics;
        }

        /// <summary>
        /// Gets or sets current GDI+ graphics context.
        /// </summary>
        public Graphics Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        public void ChangeOpacity(double opacity)
        {
            this.oldOpacity = this.opacity;
            this.opacity = opacity;
        }

        public void RestoreOpacity()
        {
            this.opacity = oldOpacity;
        }

        private Color GetColor(Color original)
        {
            if (this.opacity == 1)
                return original;

            return Color.FromArgb(Math.Min(255, Math.Max(0, (int)(original.A * this.opacity))), original);
        }

        private Color[] GetColors(Color[] original)
        {
            Color[] res = new Color[original.Length];
            for (int i = 0; i < original.Length; i++)
            {
                res[i] = GetColor(original[i]);
            }

            return res;
        }

        public virtual void ChangeSmoothingMode(SmoothingMode smoothingMode)
        {
            this.oldSmoothingMode = graphics.SmoothingMode;
            graphics.SmoothingMode = smoothingMode;
        }

        public virtual void ExcludeClip(Rectangle rectangle)
        {
            graphics.ExcludeClip(rectangle);
        }

        public virtual void RestoreSmoothingMode()
        {
            graphics.SmoothingMode = this.oldSmoothingMode;
        }

        public void RotateTransform(float angleInDegrees)
        {
            this.graphics.RotateTransform(angleInDegrees);
        }

        public void TranslateTransform(float offsetX, float offsetY)
        {
            this.graphics.TranslateTransform(offsetX, offsetY);
        }

        public void TranslateTransform(int offsetX, int offsetY)
        {
            this.graphics.TranslateTransform(offsetX, offsetY);
        }

        public void ScaleTransform(SizeF scale)
        {
            this.graphics.ScaleTransform(scale.Width, scale.Height);
        }

        public void ResetTransform()
        {
            this.graphics.ResetTransform();
        }

        public object SaveState()
        {
            return this.graphics.Save();
        }

        public void RestoreState(object state)
        {
            GraphicsState gdiState = state as GraphicsState;
            if (gdiState != null)
            {
                this.graphics.Restore(gdiState);
            }
        }

        private Stack<GraphicsPath> ClippingPathStack
        {
            get
            {
                if (this.clippingPathStack == null)
                {
                    this.clippingPathStack = new Stack<GraphicsPath>();
                }

                return this.clippingPathStack;
            }
        }

        public void PushCurrentClippingPath(GraphicsPath path)
        {
            this.ClippingPathStack.Push(path);
        }

        public GraphicsPath PopCurrentClippingPath()
        {
            return this.ClippingPathStack.Pop();
        }

        #region Drawing Shapes

        public virtual void DrawEllipse(Rectangle rectangle, Color color)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            using (Pen pen = new Pen(color))
            {
                graphics.DrawEllipse(pen, rectangle);
            }
        }

        public virtual void DrawLine(Color color, int x1, int y1, int x2, int y2)
        {
            using (Pen pen = new Pen(color))
            {
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        public virtual void DrawLine(Color color, float x1, float y1, float x2, float y2)
        {
            using (Pen pen = new Pen(color))
            {
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        public virtual void DrawLine(Color color, DashStyle dashStyle, int x1, int y1, int x2, int y2)
        {
            using (Pen pen = new Pen(color))
            {
                pen.DashStyle = dashStyle;
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        public virtual void DrawRectangle(Rectangle rectangle, Color color)
        {
            DrawRectangle(rectangle, color, PenAlignment.Center, 1f, null);
        }

        public virtual void DrawRectangle(Rectangle rectangle, Color color, PenAlignment penAlignment, float penWidth)
        {
            DrawRectangle((RectangleF)rectangle, color, penAlignment, penWidth);
        }

        public virtual void DrawRectangle(RectangleF rectangle, Color color, PenAlignment penAlignment, float penWidth)
        {
            DrawRectangle(rectangle, color, penAlignment, penWidth, null);
        }

        public virtual void DrawLinearGradientRectangle(RectangleF rectangle, Color[] gradientColors, PenAlignment penAlignment, float penWidth, float angle)
        {
            if (rectangle.Width == 0 || rectangle.Height == 0)
                return;

            using (LinearGradientBrush brush = new LinearGradientBrush(rectangle, gradientColors[0], gradientColors[1], angle))
            {
                SetGradientBrush(gradientColors, brush);
                DrawRectangle(rectangle, gradientColors[0], penAlignment, penWidth, brush);
            }
        }

        private void SetGradientBrush(Color[] gradientColors, LinearGradientBrush brush)
        {
            if (gradientColors.Length <= 2)
                return;

            ColorBlend blend = new ColorBlend();
            brush.WrapMode = WrapMode.TileFlipXY;
            blend.Colors = GetColors(gradientColors);
            blend.Positions = new float[] { 0f, 0.333f, 0.666f, 1f };
            brush.InterpolationColors = blend;
        }

        public virtual void DrawRadialGradientRectangle(RectangleF rectangle, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth)
        {
            using (GraphicsPath gradientPath = new GraphicsPath())
            {
                gradientPath.AddEllipse(rectangle);
                DrawCustomGradientRectangle(rectangle, gradientPath, color, gradientColors, penAlignment, penWidth);
            }
        }

        public virtual void DrawRadialGradientRectangle(Rectangle rectangle, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth)
        {
            DrawRadialGradientRectangle((RectangleF)rectangle, color, gradientColors, penAlignment, penWidth);
        }

        public virtual void DrawCustomGradientRectangle(RectangleF rectangle, GraphicsPath path, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth)
        {
            using (PathGradientBrush brush = new PathGradientBrush(path))
            {
                brush.CenterColor = GetColor(color);
                brush.SurroundColors = GetColors(gradientColors);
                DrawRectangle(rectangle, color, penAlignment, penWidth, brush);
            }
        }

        public virtual void DrawCustomGradientRectangle(Rectangle rectangle, GraphicsPath path, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth)
        {
            DrawCustomGradientRectangle((RectangleF)rectangle, path, color, gradientColors, penAlignment, penWidth);
        }

        private void DrawRectangle(RectangleF rectangle, Color color, PenAlignment penAlignment, float penWidth, Brush brush)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            using (Pen pen = new Pen(GetColor(color)))
            {
                pen.Width = penWidth;
                pen.Alignment = penAlignment;

                if (brush != null)
                    pen.Brush = brush;

                graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        /// <summary>
        /// Draws a border specified by rectangle structure, IBorderElement.
        /// </summary>
        public virtual void DrawBorder(RectangleF rectangle, IBorderElement borderElement)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            this.ChangeSmoothingMode(SmoothingMode.HighSpeed);

            float topWidth = (float)Math.Ceiling(borderElement.TopWidth == 1 ? 1 : (borderElement.TopWidth / 2));
            float bottomWidth = (float)Math.Ceiling(borderElement.BottomWidth == 1 ? 1 : (borderElement.BottomWidth / 2));
            float leftWidth = (float)Math.Ceiling(borderElement.LeftWidth == 1 ? 1 : (borderElement.LeftWidth / 2));
            float rightWidth = (float)Math.Ceiling(borderElement.RightWidth == 1 ? 1 : (borderElement.RightWidth / 2));

            float topInnerWidth = (float)Math.Floor(borderElement.TopWidth == 1 ? 1 : (borderElement.TopWidth / 2));
            float bottomInnerWidth = (float)Math.Floor(borderElement.BottomWidth == 1 ? 1 : (borderElement.BottomWidth / 2));
            float leftInnerWidth = (float)Math.Floor(borderElement.LeftWidth == 1 ? 1 : (borderElement.LeftWidth / 2));
            float rightInnerWidth = (float)Math.Floor(borderElement.RightWidth == 1 ? 1 : (borderElement.RightWidth / 2));

            RectangleF innerRect = new RectangleF(
                rectangle.Left + leftWidth,
                rectangle.Top + topWidth,
                rectangle.Width - (leftWidth + rightWidth),
                rectangle.Height - (topWidth + bottomWidth));

            //Fill rectangle paints inside the given rectangle and does not paint the exacly the pixeols of the bototm/right line
            //therefore we should add 1 pxiel to bottom/right lines

            // top line
            if (borderElement.TopWidth > 0)
            {
                using (Brush br1 = new SolidBrush(GetColor(borderElement.TopColor)))
                using (Brush br2 = new SolidBrush(GetColor(borderElement.TopShadowColor)))
                {
                    graphics.FillRectangle(br1, new RectangleF(
                        rectangle.Left,
                        rectangle.Top,
                        rectangle.Width + 1,
                        topWidth));
                    graphics.FillRectangle(br2, new RectangleF(
                        innerRect.Left,
                        innerRect.Top,
                        innerRect.Width + 1,
                        topInnerWidth));
                }
            }

            // bottom line
            if (borderElement.BottomWidth > 0)
            {
                using (Brush br1 = new SolidBrush(GetColor(borderElement.BottomColor)))
                using (Brush br2 = new SolidBrush(GetColor(borderElement.BottomShadowColor)))
                {
                    graphics.FillRectangle(br1, new RectangleF(
                        rectangle.Left,
                        rectangle.Bottom - bottomWidth + 1,
                        rectangle.Width + 1,
                        bottomWidth));
                    graphics.FillRectangle(br2, new RectangleF(
                        innerRect.Left,
                        innerRect.Bottom - bottomInnerWidth + 1,
                        innerRect.Width + 1,
                        bottomInnerWidth));
                }
            }

            // left line
            if (borderElement.LeftWidth > 0)
            {
                using (Brush br1 = new SolidBrush(GetColor(borderElement.LeftColor)))
                using (Brush br2 = new SolidBrush(GetColor(borderElement.LeftShadowColor)))
                {
                    float height = rectangle.Height - (topWidth + bottomWidth) + 1;
                    float yPosition = rectangle.Top + topWidth;
                    float innerHeight = innerRect.Height - (topInnerWidth + bottomInnerWidth) + 1;
                    float innerYPosition = innerRect.Top + topInnerWidth;
                    if ((int)(borderElement.BorderDrawMode & BorderDrawModes.LeftOverTop) != 0)
                    {
                        height += topWidth;
                        yPosition -= topWidth;

                        innerHeight += topInnerWidth;
                        innerYPosition -= topInnerWidth;
                    }

                    if ((int)(borderElement.BorderDrawMode & BorderDrawModes.LeftOverBottom) != 0)
                    {
                        height += bottomWidth;
                        innerHeight += bottomInnerWidth;
                    }

                    graphics.FillRectangle(br1, new RectangleF(
                        rectangle.Left,
                        yPosition,
                        leftWidth,
                        height));
                    graphics.FillRectangle(br2, new RectangleF(
                        innerRect.Left,
                        innerYPosition,
                        leftInnerWidth,
                        innerHeight));
                }
            }

            // right line
            if (borderElement.RightWidth > 0)
            {
                using (Brush br1 = new SolidBrush(GetColor(borderElement.RightColor)))
                using (Brush br2 = new SolidBrush(GetColor(borderElement.RightShadowColor)))
                {
                    //Fill rectangle paints inside the given rectangle and does not paint the exacly the pixeols of the bototm/right line
                    //therefore we should add 1 pxiel to bottom/right lines

                    float height = rectangle.Height - (topWidth + bottomWidth) + 1;
                    float yPosition = rectangle.Top + topWidth;
                    float innerHeight = innerRect.Height - (topInnerWidth + bottomInnerWidth) + 1;
                    float innerYPosition = innerRect.Top + topInnerWidth;
                    if ((int)(borderElement.BorderDrawMode & BorderDrawModes.RightOverTop) != 0)
                    {
                        height += topWidth;
                        yPosition -= topWidth;

                        innerHeight += topInnerWidth;
                        innerYPosition -= topInnerWidth;
                    }

                    if ((int)(borderElement.BorderDrawMode & BorderDrawModes.RightOverBottom) != 0)
                    {
                        height += bottomWidth;
                        innerHeight += bottomInnerWidth;
                    }

                    graphics.FillRectangle(br1, new RectangleF(
                        rectangle.Right - rightWidth + 1,
                        yPosition,
                        rightWidth,
                        height));
                    graphics.FillRectangle(br2, new RectangleF(
                        innerRect.Right - rightInnerWidth + 1,
                        innerYPosition,
                        rightInnerWidth,
                        innerHeight));
                }
            }

            this.RestoreSmoothingMode();
        }

        public void FillTextureRectangle(Rectangle rectangle, Image texture)
        {
            this.FillTextureRectangle(rectangle, texture, WrapMode.Tile);
        }

        public void FillTextureRectangle(Rectangle rectangle, Image texture, WrapMode wrapMode)
        {
            this.FillTextureRectangle((RectangleF)rectangle, texture, wrapMode);
        }

        public void FillTextureRectangle(RectangleF rectangle, Image texture)
        {
            this.FillTextureRectangle(rectangle, texture, WrapMode.Tile);
        }

        public void FillTextureRectangle(RectangleF rectangle, Image texture, WrapMode wrapMode)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            using (TextureBrush brush = new TextureBrush(texture, wrapMode))
            {
                Matrix brushOffset = new Matrix();
                brushOffset.Translate(rectangle.X, rectangle.Y);
                brush.Transform = brushOffset;
                graphics.FillRectangle(brush, rectangle);
            }
        }

        public virtual void FillRectangle(Rectangle rectangle, Color color)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            using (Brush brush = new SolidBrush(GetColor(color)))
            {
                graphics.FillRectangle(brush, rectangle);
            }
        }

        public virtual void FillRectangle(RectangleF rectangle, Color color)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            using (Brush brush = new SolidBrush(GetColor(color)))
            {
                GraphicsPath fillPath = this.UseFillPath(rectangle);
                if (fillPath != null)
                {
                    graphics.FillPath(brush, fillPath);
                }
                else
                {
                    graphics.FillRectangle(brush, rectangle);
                }
            }
        }

        public virtual void FillGradientRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, GradientStyles style, float angle)
        {
            Color[] colors = new Color[] { GetColor(color1), GetColor(color2), GetColor(color3), GetColor(color4) };
            float[] positions = new float[] { 0f, 1f };
            FillGradientRectangle(rectangle, colors, positions, style, angle, 0f, 0f);
        }

        public virtual void FillGradientRectangle(Rectangle rectangle, Color[] colorStops, float[] colorOffsets, GradientStyles style, float angle,
            float gradientPercentage, float gradientPercentage2)
        {
            this.FillGradientRectangle((RectangleF)rectangle, colorStops, colorOffsets, style, angle,
                                       gradientPercentage, gradientPercentage2);
        }

        public virtual void FillGradientRectangle(RectangleF rectangle, Color[] colorStops, float[] colorOffsets, GradientStyles style, float angle,
            float gradientPercentage, float gradientPercentage2)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            if (style == GradientStyles.Radial)
            {

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(rectangle);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        ColorBlend blend = new ColorBlend();
                        brush.CenterColor = GetColor(colorStops[0]);
                        blend.Colors = GetColors(colorStops);

                        if (colorStops.Length == 4)
                        {
                            blend.Positions = new float[] { 0f, gradientPercentage, gradientPercentage2, 1.0f };
                        }
                        else if (colorStops.Length == 3)
                        {
                            blend.Positions = new float[] { 0f, gradientPercentage, 1.0f };
                        }
                        else
                        {
                            blend.Positions = colorOffsets;
                        }

                        brush.InterpolationColors = blend;
                        graphics.FillPath(brush, path);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rectangle, Color.Black, Color.Black, angle))
                {
                    brush.WrapMode = WrapMode.TileFlipXY;
                    ColorBlend blend = new ColorBlend();
                    blend.Colors = GetColors(colorStops);
                    blend.Positions = colorOffsets;
                    brush.InterpolationColors = blend;

                    GraphicsPath fillPath = this.UseFillPath(rectangle);

                    if (fillPath != null)
                    {
                        graphics.FillPath(brush, fillPath);
                    }
                    else
                    {
                        graphics.FillRectangle(brush, rectangle);
                    }
                }
            }
        }

        private GraphicsPath UseFillPath(RectangleF rectangle)
        {
            if (this.ClippingPathStack.Count == 0)
            {
                return null;
            }

            GraphicsPath currentPath = this.ClippingPathStack.Peek();
            return currentPath;
        }

        public virtual void FillGradientRectangle(Rectangle rectangle, Color color1, Color color2, float angle)
        {
            Color[] colors = new Color[] { color1, color2 };
            float[] positions = new float[] { 0f, 1f };
            FillGradientRectangle(rectangle, colors, positions, GradientStyles.Linear, angle, 0f, 0f);
        }

        public virtual void FillGlassRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, float gradientPercentage, float gradientPercentage2)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            RectangleF clippingRect = rectangle;
            clippingRect.Inflate(-0.5f, -0.5f);

            SetClippingByPath(rectangle);

            this.ChangeSmoothingMode(SmoothingMode.None);

            Rectangle rectangle1 = rectangle;

            int topHeight = (int)Math.Round(rectangle.Height * (1 - gradientPercentage));
            rectangle1.Height = topHeight + 1;

            if (color1.A > 3) //only draw non-transparent stuff..
            {
                using (LinearGradientBrush upperGlassyEffect = new LinearGradientBrush(rectangle1, GetColor(color1), GetColor(color2), LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(upperGlassyEffect, rectangle1);
                }
            }

            rectangle1 = rectangle;
            rectangle1.Y = rectangle.Top + topHeight;
            rectangle1.Height = rectangle.Height - topHeight;

            using (SolidBrush sb = new SolidBrush(GetColor(color3)))
            {
                graphics.FillRectangle(sb, rectangle1);
            }

            Rectangle innerGradient = rectangle1;
            innerGradient.Y = rectangle.Top + (int)(topHeight * (gradientPercentage2));// (innerGradient.Width - rectangle1.Width) / 2;
            innerGradient.Height = (int)((rectangle.Height - (innerGradient.Y - rectangle.Top)) * 2.0);
            innerGradient.Width = (int)(innerGradient.Width * 1.6);
            innerGradient.X = (int)(rectangle.X - (innerGradient.Width - rectangle.Width) / 2.0);

            //graphics.DrawRectangle(Pens.Red, innerGradient);

            if (!CheckValidRectangle(innerGradient))
                return;

            if (color1.A > 3) //only draw non-transparent stuff..
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(innerGradient, 180, 180);
                    //path.AddRectangle(innerGradient);
                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.SurroundColors = new Color[] { Color.Transparent };
                        pgb.CenterColor = this.GetColor(color4);
                        pgb.CenterPoint = new PointF(rectangle.Width / 2.0f, (float)innerGradient.Y + innerGradient.Height / 2f);
                        pgb.FocusScales = new PointF(0.5f, 0.5f);

                        graphics.FillRectangle(pgb, rectangle);
                    }
                }
            }

            RestoreClippingByPath();

            this.RestoreSmoothingMode();
        }

        Region oldClip;
        Region graphicsClip;

        private GraphicsPath SetClippingByPath(Rectangle rectangle)
        {
            this.oldClip = null;
            this.graphicsClip = null;
            GraphicsPath clippingPath = this.UseFillPath(rectangle);
            if (clippingPath != null)
            {
                this.oldClip = this.graphics.Clip;
                graphicsClip = new Region(clippingPath);
                this.graphics.Clip = graphicsClip;
            }

            return clippingPath;
        }

        private void RestoreClippingByPath()
        {
            if (oldClip != null)
            {
                this.graphics.Clip = oldClip;
            }
            if (graphicsClip != null)
            {
                graphicsClip.Dispose();
            }
        }

        public virtual void FillGlassRectangleNew(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, float gradientPercentage, float gradientPercentage2)
        {
            if (!CheckValidRectangle(rectangle))
                return;

            this.ChangeSmoothingMode(SmoothingMode.None);

            Rectangle rectangle1 = rectangle;

            int topHeight = (int)Math.Round(rectangle.Height * (1 - gradientPercentage));
            rectangle1.Height = topHeight + 1;

            Graphics drawingGraphics = this.graphics;
            Bitmap tmp = null;

            //GraphicsPath clippingPath = null;
            GraphicsPath clippingPath = this.UseFillPath(rectangle);

            if (clippingPath != null)
            {
                tmp = new Bitmap(rectangle.Width, rectangle.Height);
                drawingGraphics = Graphics.FromImage(tmp);

                drawingGraphics.TranslateTransform(-rectangle.X, -rectangle.Y);
            }

            if (color1.A > 3) //only draw non-transparent stuff..
            {
                using (LinearGradientBrush upperGlassyEffect = new LinearGradientBrush(rectangle1, GetColor(color1), GetColor(color2), LinearGradientMode.Vertical))
                {
                    drawingGraphics.FillRectangle(upperGlassyEffect, rectangle1);
                }
            }

            rectangle1 = rectangle;
            rectangle1.Y = rectangle.Top + topHeight;
            rectangle1.Height = rectangle.Height - topHeight;

            using (SolidBrush brush = new SolidBrush(GetColor(color3)))
            {
                drawingGraphics.FillRectangle(brush, rectangle1);
            }

            Rectangle innerGradient = rectangle1;
            innerGradient.Y = rectangle.Top + (int)(topHeight * (gradientPercentage2));// (innerGradient.Width - rectangle1.Width) / 2;
            innerGradient.Height = (int)((rectangle.Height - (innerGradient.Y - rectangle.Top)) * 2.0);
            innerGradient.Width = (int)(innerGradient.Width * 1.6);
            innerGradient.X = (int)(rectangle.X - (innerGradient.Width - rectangle.Width) / 2.0);

            if (!CheckValidRectangle(innerGradient))
                return;

            if (color1.A > 3) //only draw non-transparent stuff..
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(innerGradient, 180, 180);
                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.SurroundColors = new Color[] { Color.Transparent };
                        pgb.CenterColor = this.GetColor(color4);
                        pgb.CenterPoint = new PointF(pgb.CenterPoint.X, (float)innerGradient.Y + innerGradient.Height / 2f);
                        pgb.FocusScales = new PointF(0.5f, 0.5f);

                        drawingGraphics.FillRectangle(pgb, rectangle);
                    }
                }
            }

            if (clippingPath != null)
            {
                drawingGraphics.TranslateTransform(rectangle.X, rectangle.Y);
                using (TextureBrush tBrush = new TextureBrush(tmp))
                {
                    tBrush.TranslateTransform(rectangle1.X, rectangle1.Y);
                    tBrush.WrapMode = WrapMode.Clamp;
                    graphics.FillPath(tBrush, clippingPath);
                }

                tmp.Dispose();
            }

            this.RestoreSmoothingMode();
        }

        public void FillVistaRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, float gradientPercentage, float gradientPercentage2)
        {
            Rectangle buttonRect = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
            DrawOfficeGlassRectangle(this.graphics, buttonRect, GetColor(color1), GetColor(color2), GetColor(color3), GetColor(color4), gradientPercentage, gradientPercentage2);
        }

        #region FillVista

        private static GraphicsPath OfficeGlassRoundedRectangleTop(Rectangle boundingRect, int cornerRadius, int margin)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(boundingRect.X + margin, boundingRect.Y + margin, cornerRadius * 2,
                    cornerRadius * 2, 180, 90);
            roundedRect.AddArc(boundingRect.X + boundingRect.Width - margin - cornerRadius * 2,
                    boundingRect.Y + margin, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.CloseFigure();
            Rectangle rec = boundingRect;
            rec.Y = (int)roundedRect.GetBounds().Bottom;
            roundedRect.AddRectangle(rec);
            return roundedRect;
        }
        private static GraphicsPath OfficeGlassRoundedRectangleBottom(Rectangle boundingRect, int cornerRadius, int margin)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(boundingRect.X + boundingRect.Width - margin - cornerRadius * 2,
                   boundingRect.Y + boundingRect.Height - margin - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddArc(boundingRect.X + margin,
                   boundingRect.Y + boundingRect.Height - margin - cornerRadius * 2,
                   cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.CloseFigure();
            Rectangle rec = boundingRect;
            rec.Height = boundingRect.Height - (int)roundedRect.GetBounds().Height;
            roundedRect.AddRectangle(rec);
            return roundedRect;
        }

        private void DrawOfficeGlassInnerArea(Graphics g, GraphicsPath innerPath, Rectangle rec, Color surroundColor)
        {
            innerPath.AddRectangle(rec);
            if (IsInvalidRectangle(rec)) return;
            using (PathGradientBrush pgb = new PathGradientBrush(innerPath))
            {
                pgb.SurroundColors = new Color[] { ReduceAlphaBasedOnOriginal(20, GetColor(surroundColor)) };
                pgb.CenterColor = GetColor(Color.White);
                g.FillPath(pgb, innerPath);
            }
        }

        private void DrawOfficeGlassRectangle(Graphics g, Rectangle buttonRect, Color color1, Color color2, Color color3, Color color4,
            float gradientPercentage1, float gradientPercentage2)
        {
            if (IsInvalidRectangle(buttonRect)) return;
            int scalingDividend = Math.Min(buttonRect.Width, buttonRect.Height);
            int rectCornerRadius = Math.Max(1, scalingDividend / 10);
            if (gradientPercentage2 == 0) gradientPercentage2 = 0.1f;
            Rectangle lgradientRec1 =
                new Rectangle(buttonRect.X, buttonRect.Y, buttonRect.Width, (int)(gradientPercentage1 * buttonRect.Height / 3) + 1);
            Rectangle lgradientRec2 =
                new Rectangle(buttonRect.X, buttonRect.Y + (int)(gradientPercentage1 * buttonRect.Height / 3), buttonRect.Width,
                              (int)(gradientPercentage2 * buttonRect.Height / 3));
            Rectangle lgradientRec3 =
                new Rectangle(buttonRect.X,
                              buttonRect.Y + (int)(gradientPercentage1 * buttonRect.Height / 3) +
                              (int)(gradientPercentage2 * buttonRect.Height / 3), buttonRect.Width,
                              buttonRect.Height - (int)(gradientPercentage1 * buttonRect.Height / 3) -
                              (int)(gradientPercentage2 * buttonRect.Height / 3));

            if (IsInvalidRectangle(lgradientRec1)) return;
            if (IsInvalidRectangle(lgradientRec2)) return;
            if (IsInvalidRectangle(lgradientRec3)) return;

            using (
                LinearGradientBrush lgb1 =
                    new LinearGradientBrush(lgradientRec1, ReduceAlphaBasedOnOriginal(50, color2), color2, LinearGradientMode.Vertical))
            using (
                LinearGradientBrush lgb2 = new LinearGradientBrush(lgradientRec2, color4, color1, LinearGradientMode.Vertical))
            using (
                LinearGradientBrush lgb3 = new LinearGradientBrush(lgradientRec3, color3, color4, LinearGradientMode.Vertical))
            using (GraphicsPath path1 = OfficeGlassRoundedRectangleTop(lgradientRec1, rectCornerRadius, 0))
            using (GraphicsPath path2 = new GraphicsPath())
            using (GraphicsPath path3 = OfficeGlassRoundedRectangleBottom(lgradientRec3, rectCornerRadius, 0))
            {
                path2.AddRectangle(lgradientRec2);

                g.FillPath(lgb1, path1);
                g.FillPath(lgb2, path2);
                g.FillPath(lgb3, path3);
            }

            using (GraphicsPath innerPath = new GraphicsPath())
                DrawOfficeGlassInnerArea(g, innerPath, buttonRect, color4);
        }

        private bool IsInvalidRectangle(Rectangle rec)
        {
            if ((rec.Width <= 0) || (rec.Height <= 0))
                return true;
            return false;
        }

        private bool IsInvalidRectangle(RectangleF rec)
        {
            if ((((int)rec.Width) <= 0) || (((int)rec.Height) <= 0))
                return true;
            return false;
        }

        #endregion

        public void FillOfficeGlassRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, float gradientPercentage, float gradientPercentage2, bool drawEllipse)
        {
            if (rectangle.IsEmpty)
                return;

            color1 = GetColor(color1);
            color2 = GetColor(color2);
            color3 = GetColor(color3);
            color4 = GetColor(color4);

            Rectangle originalRectangle = new Rectangle(rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2);

            this.ChangeSmoothingMode(SmoothingMode.AntiAlias);

            GraphicsPath clippingPath = UseFillPath(rectangle);

            int topHeight2 = (int)Math.Round(originalRectangle.Height - (originalRectangle.Height / 2) * gradientPercentage2);

            //main fill
            Rectangle innerRectangle = originalRectangle;
            if (drawEllipse)
            {
                using (SolidBrush brush = new SolidBrush(color3))
                    graphics.FillEllipse(brush, innerRectangle);
            }
            else if (clippingPath != null)
            {
                using (SolidBrush brush = new SolidBrush(color3))
                    graphics.FillPath(brush, clippingPath);
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(color3))
                    graphics.FillRectangle(brush, innerRectangle);
            }

            graphics.CompositingQuality = CompositingQuality.HighQuality;

            //bottom glow
            for (int i = 0; i < 2; i++)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    Rectangle rect = originalRectangle;

                    rect.Y += topHeight2 - (int)(rect.Height * 0.1);
                    rect.Width = (int)(rect.Width * 1.5);

                    rect.X = ((originalRectangle.Width / 2) - (rect.Width / 2)) + rect.X;

                    if (IsInvalidRectangle(rect))
                    {
                        this.RestoreSmoothingMode();
                        return;
                    }

                    path.AddEllipse(rect);

                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.SurroundColors = new Color[] { ReduceAlphaBasedOnOriginal(100, color3) };
                        pgb.CenterColor = ReduceAlphaBasedOnOriginal(220, color4);

                        if (drawEllipse)
                        {
                            graphics.FillEllipse(pgb, originalRectangle);
                        }
                        else if (clippingPath != null)
                        {
                            graphics.FillPath(pgb, clippingPath);
                        }
                        else
                        {
                            graphics.FillRectangle(pgb, originalRectangle);
                        }
                    }
                }
            }

            //top lighter glow
            using (GraphicsPath path = new GraphicsPath())
            {
                Rectangle rect = Rectangle.Inflate(originalRectangle, 0, 0);
                rect.Y = rect.Y - rect.Height / 2 - (int)(rect.Height * 0.02);
                rect.Width = (int)(rect.Width * (1.2 + gradientPercentage));
                rect.X = ((originalRectangle.Width / 2) - (rect.Width / 2)) + rect.X;

                path.AddEllipse(rect);

                using (PathGradientBrush pgb = new PathGradientBrush(path))
                {
                    pgb.SurroundColors = new Color[] { ReduceAlphaBasedOnOriginal(5, color2) };
                    pgb.CenterPoint = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height * 0.5f);
                    pgb.CenterColor = ReduceAlphaBasedOnOriginal(200, color1);

                    if (drawEllipse)
                    {
                        graphics.FillEllipse(pgb, originalRectangle);
                    }
                    else if (clippingPath != null)
                    {
                        graphics.FillPath(pgb, clippingPath);
                    }
                    else
                    {
                        graphics.FillRectangle(pgb, originalRectangle);
                    }
                }
            }

            //top inner glow
            for (int i = 0; i < 3; i++)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    Rectangle rect = Rectangle.Inflate(originalRectangle, 0, 0);

                    rect.Y = rect.Y + (int)(originalRectangle.Height * 0.08);
                    rect.Height = (int)(originalRectangle.Height * 0.77);
                    rect.Width = (int)(rect.Width * 0.8);
                    rect.X = ((originalRectangle.Width / 2) - (rect.Width / 2)) + rect.X;

                    if (IsInvalidRectangle(rect))
                    {
                        this.RestoreSmoothingMode();
                        return;
                    }

                    path.AddEllipse(rect);

                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.SurroundColors = new Color[] { GetColor(ReduceAlphaBasedOnOriginal(0, color2)) };
                        pgb.CenterColor = GetColor(ReduceAlphaBasedOnOriginal(130, color3));

                        if (drawEllipse)
                        {
                            graphics.FillEllipse(pgb, originalRectangle);
                        }
                        else if (clippingPath != null)
                        {
                            graphics.FillPath(pgb, clippingPath);
                        }
                        else
                        {
                            graphics.FillRectangle(pgb, originalRectangle);
                        }
                    }
                }
            }

            Color topBorder = GetColor(ReduceAlphaBasedOnOriginal(50, color1));
            Color bottomBorder = GetColor(ReduceAlphaBasedOnOriginal(50, color4));

            int borderThicness = Math.Max(1, (int)(originalRectangle.Width * 0.02));

            //inner border
            Rectangle innerBorderRect = Rectangle.Inflate(originalRectangle, -borderThicness, -borderThicness);
            if (!IsInvalidRectangle(innerBorderRect))
            {
                using (
                    LinearGradientBrush brush =
                        new LinearGradientBrush(innerBorderRect, topBorder, bottomBorder, LinearGradientMode.Vertical))
                {
                    brush.WrapMode = WrapMode.TileFlipXY;
                    using (Pen pen = new Pen(brush))
                    {
                        pen.Width = borderThicness;
                        if (drawEllipse)
                        {
                            graphics.DrawEllipse(pen, innerBorderRect);
                        }
                        else if (clippingPath != null)
                        {
                            graphics.DrawPath(pen, clippingPath);
                        }
                        else
                        {
                            graphics.DrawRectangle(pen, innerBorderRect);
                        }
                    }
                }
            }

            //outer border
            using (
                LinearGradientBrush brush =
                    new LinearGradientBrush(originalRectangle, this.ReduceAlphaBasedOnOriginal(100, color3), color3, LinearGradientMode.Vertical))
            {
                brush.WrapMode = WrapMode.TileFlipXY;
                using (Pen pen = new Pen(brush))
                {
                    pen.Width = borderThicness;
                    if (drawEllipse)
                    {
                        graphics.DrawEllipse(pen, originalRectangle);
                    }
                    else if (clippingPath != null)
                    {
                        graphics.DrawPath(pen, clippingPath);
                    }
                    else
                    {
                        graphics.DrawRectangle(pen, originalRectangle);
                    }
                }
            }

            this.RestoreSmoothingMode();
        }

        private Color ReduceAlphaBasedOnOriginal(int newA, Color color)
        {
            return Color.FromArgb((int)((double)newA * (color.A / 255d)), color);
        }

        public virtual void FillGellRectangle(Rectangle rectangle, Color[] colorStops, float gradientPercentage, float gradientPercentage2)
        {
            this.ChangeSmoothingMode(SmoothingMode.AntiAlias);

            float[] gradienStops = new float[] { 0 };

            if (colorStops.Length < 2)
            {
                this.FillRectangle(rectangle, colorStops[0]);
            }
            else
            {
                if (colorStops.Length > 3)
                {
                    gradienStops = new float[] { 0f, 0.333f, 0.666f, 1f };
                }
                else if (colorStops.Length > 2)
                {
                    gradienStops = new float[] { 0.0f, 0.50f, 1f };
                }
                else
                {
                    gradienStops = new float[] { 0.0f, 1f };
                }

                this.FillGradientRectangle(rectangle, colorStops, gradienStops, GradientStyles.Linear, 90, 0f, 1f);
            }

            float bubbleHeight = rectangle.Height / 2f;

            float bubbleFactor = (int)Math.Round((100f * gradientPercentage) * 0.2);
            float bubbleFactor2 = (int)Math.Round((bubbleHeight * gradientPercentage2) * 0.2);

            RectangleF innerRect = new RectangleF(
                rectangle.X + bubbleFactor,
                rectangle.Y,
                rectangle.Width - bubbleFactor * 2 - 1f,
                bubbleHeight - bubbleFactor2 * 2);

            if (!this.IsInvalidRectangle(innerRect))
            {
                using (GraphicsPath innerPath = GetRoundedRect(
                    innerRect, innerRect.Height))
                {
                    RectangleF innerRect1 = RectangleF.Inflate(innerRect, 1, 1);
                    using (LinearGradientBrush innerBrush = new LinearGradientBrush(
                        innerRect1,
                        ReduceAlphaBasedOnOriginal(253, GetColor(Color.White)),
                        ReduceAlphaBasedOnOriginal(42, GetColor(Color.White)),
                        LinearGradientMode.Vertical))
                    {
                        graphics.FillPath(innerBrush, innerPath);
                    }
                }
            }

            this.RestoreSmoothingMode();
        }

        public void FillPolygon(Color color, Point[] points)
        {
            using (Brush brush = new SolidBrush(color))
            {
                graphics.FillPolygon(brush, points);
            }
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            graphics.FillPolygon(brush, points);
        }

        public void FillPolygon(Color color, PointF[] points)
        {
            using (Brush brush = new SolidBrush(color))
            {
                graphics.FillPolygon(brush, points);
            }
        }

        public virtual void DrawRoundRect(Rectangle rectangle, Color color, float borderWidth, int radius)
        {
            using (GraphicsPath path = GetRoundedRect(rectangle, radius))
            {
                using (Pen pen = new Pen(GetColor(color), borderWidth))
                {
                    graphics.DrawPath(pen, path);
                }
            }
        }

        public virtual void DrawPath(GraphicsPath path, Color color, PenAlignment penAlignment, float penWidth)
        {
            DrawPath(path, color, penAlignment, penWidth, null);
        }

        public static RectangleF NormalizeRect(RectangleF actual)
        {
            float x = actual.X;
            float y = actual.Y;
            float width = actual.Width;
            float height = actual.Height;

            if (actual.Width < 0)
            {
                x = x - actual.Width;
                width = -actual.Width;
            }

            if (actual.Height < 0)
            {
                y = y - actual.Height;
                height = -actual.Height;
            }

            return new RectangleF(x, y, width, height);
        }

        public void DrawLinearGradientPath(GraphicsPath path, RectangleF bounds, Color[] gradientColors, PenAlignment penAlignment, float penWidth, float angle)
        {
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                return;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(NormalizeRect(bounds), gradientColors[0], gradientColors[1], angle))
            {
                if (gradientColors.Length > 2)
                {
                    ColorBlend blend = new ColorBlend();
                    blend.Colors = GetColors(gradientColors);
                    if (gradientColors.Length == 4)
                    {
                        blend.Positions = new float[] { 0f, 0.333f, 0.666f, 1f };
                    }
                    else if (gradientColors.Length == 3)
                    {
                        blend.Positions = new float[] { 0f, 0.5f, 1f };
                    }

                    brush.WrapMode = WrapMode.TileFlipXY;
                    brush.InterpolationColors = blend;
                }

                DrawPath(path, gradientColors[0], penAlignment, penWidth, brush);
            }
        }

        public void DrawRadialGradientPath(GraphicsPath path, Rectangle bounds, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth)
        {
            using (GraphicsPath gradientPath = new GraphicsPath())
            {
                gradientPath.AddEllipse(bounds);
                DrawCustomGradientPath(path, gradientPath, color, gradientColors, penAlignment, penWidth);
            }
        }

        public void DrawCustomGradientPath(GraphicsPath path, GraphicsPath gradientPath, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth)
        {
            using (PathGradientBrush brush = new PathGradientBrush(gradientPath))
            {
                brush.CenterColor = GetColor(color);
                brush.SurroundColors = GetColors(gradientColors);
                DrawPath(path, color, penAlignment, penWidth, brush);
            }
        }

        private void DrawPath(GraphicsPath path, Color color, PenAlignment penAlignment, float penWidth, Brush brush)
        {
            using (Pen pen = new Pen(GetColor(color)))
            {
                pen.Width = penWidth;
                pen.Alignment = penAlignment;
                if (brush != null)
                    pen.Brush = brush;

                graphics.DrawPath(pen, path);
            }
        }

        public void FillPath(Color color, GraphicsPath path)
        {
            this.ChangeSmoothingMode(SmoothingMode.AntiAlias);

            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.FillPath(brush, path);
            }
        }

        public void DrawBlurShadow(GraphicsPath path, Rectangle r, float offset, Color color)
        {
            float x = 1F - (offset * 2) / r.Width;

            float y = 1F - (offset * 2) / r.Height;

            using (PathGradientBrush pathGradientBrush = new PathGradientBrush(path))
            {
                pathGradientBrush.CenterPoint = PointF.Empty;

                pathGradientBrush.CenterColor = color;

                pathGradientBrush.FocusScales = new PointF(x, y);

                pathGradientBrush.SurroundColors = new Color[] { Color.FromArgb(100, color) };

                graphics.FillPath(pathGradientBrush, path);
            }
        }

        public void FillPath(Color[] colorStops, float[] colorOffsets, float angle,
            float gradientPercentage, float gradientPercentage2, Rectangle rectangle, GraphicsPath path)
        {
            if (this.IsInvalidRectangle(rectangle))
                return;

            this.ChangeSmoothingMode(SmoothingMode.AntiAlias);

            Brush pathFill;
            if (colorStops.Length == 1)
            {
                //use solid brush for 1 color
                pathFill = new SolidBrush(this.GetColor(colorStops[0]));
            }
            else
            {
                LinearGradientBrush brush = new LinearGradientBrush(rectangle, Color.Black, Color.Black, angle);
                ColorBlend blend = new ColorBlend();
                blend.Colors = this.GetColors(colorStops);
                blend.Positions = colorOffsets;
                brush.InterpolationColors = blend;

                pathFill = brush;
            }

            this.graphics.FillPath(pathFill, path);

            //clean-up
            pathFill.Dispose();
            this.RestoreSmoothingMode();
        }

        private const int blurAmount = 6;

        public Image ImageFromText(string strText, Font fnt, Color clrFore, Color clrBack)
        {
            Bitmap bmpOut = null; // bitmap we are creating and will return from this function.

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF sz = g.MeasureString(strText, fnt);
                using (Bitmap bmp = new Bitmap((int)sz.Width, (int)sz.Height))
                using (Graphics gBmp = Graphics.FromImage(bmp))
                using (SolidBrush brBack = new SolidBrush(ReduceAlphaBasedOnOriginal(16, GetColor(clrBack))))
                using (SolidBrush brFore = new SolidBrush(clrFore))
                {
                    gBmp.SmoothingMode = SmoothingMode.HighQuality;
                    gBmp.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    gBmp.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    gBmp.DrawString(strText, fnt, brBack, 0, 0);

                    // make bitmap we will return.
                    bmpOut = new Bitmap(bmp.Width + blurAmount, bmp.Height + blurAmount);
                    using (Graphics gBmpOut = Graphics.FromImage(bmpOut))
                    {
                        gBmpOut.SmoothingMode = SmoothingMode.HighQuality;
                        gBmpOut.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        gBmpOut.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        // smear image of background of text about to make blurred background "halo"
                        for (int x = 0; x <= blurAmount; x++)
                            for (int y = 0; y <= blurAmount; y++)
                                gBmpOut.DrawImageUnscaled(bmp, x, y);

                        // draw actual text
                        gBmpOut.DrawString(strText, fnt, brFore, blurAmount / 2, blurAmount / 2);
                    }
                }
            }

            return bmpOut;
        }

        #endregion

        #region Drawing Strings

        public virtual void DrawString(TextParams textParams, SizeF measuredSize)
        {
            if (textParams.paintingRectangle.Width <= 0 || textParams.paintingRectangle.Height <= 0)
            {
                return;
            }

            StringFormat stringFormat = textParams.CreateStringFormat();
            if (textParams.textOrientation == Orientation.Horizontal &&
                measuredSize.Height > textParams.paintingRectangle.Height)
            {
                stringFormat.LineAlignment = StringAlignment.Near;
            }

            this.DrawString(textParams.text,
                            textParams.paintingRectangle,
                            textParams.font,
                            textParams.foreColor,
                            textParams.alignment,
                            stringFormat,
                            textParams.shadow,
                            textParams.textRenderingHint,
                            textParams.textOrientation,
                            textParams.flipText);
            stringFormat.Dispose();
        }

        public virtual void DrawString(string drawString, Rectangle rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, Orientation orientation, bool flipText)
        {
            this.DrawString(drawString, rectangle, font, color, alignment, stringFormat, null, TextRenderingHint.SystemDefault, orientation, flipText);
        }

        public virtual void DrawString(string drawString, RectangleF rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, Orientation orientation, bool flipText)
        {
            this.DrawString(drawString, rectangle, font, color, alignment, stringFormat, null, TextRenderingHint.SystemDefault, orientation, flipText);
        }

        public virtual void DrawString(string drawString, Rectangle rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, ShadowSettings shadow, TextRenderingHint textRendering, Orientation orientation, bool flipText)
        {
            if (String.IsNullOrEmpty(drawString))
                return;

            if (orientation == Orientation.Vertical)
            {
                float angle = 90;

                if (flipText)
                    angle = 270;

                graphics.TranslateTransform(rectangle.X, rectangle.Y + rectangle.Height);
                graphics.RotateTransform(angle);

                Rectangle r = new Rectangle(0, 0, rectangle.Height, rectangle.Width);

                Size stringSize = Size.Ceiling(MeasureString(drawString, font, stringFormat));

                if (stringFormat.Trimming == StringTrimming.None)
                {
                    stringSize.Width = Math.Min(stringSize.Width, r.Width);
                    stringSize.Height = Math.Min(stringSize.Height, r.Height);
                }

                if (!flipText)
                {
                    r.X -= r.Width;
                    r.Y -= r.Height;
                }

                using (Brush textBrush = new SolidBrush(color))
                    graphics.DrawString(drawString, font, textBrush, r, stringFormat);

                graphics.RotateTransform(-angle);
                graphics.TranslateTransform(-rectangle.X, -(rectangle.Y + rectangle.Height));
            }
            else
            {
                if (!CheckValidString(drawString, font, rectangle))
                    return;

                //calculate the size of the string if rendered in the given rectangle
                Size stringSize = Size.Ceiling(MeasureString(drawString, font, rectangle.Width, stringFormat));
                if (stringFormat.Trimming == StringTrimming.None)
                {
                    stringSize.Width = Math.Min(stringSize.Width, rectangle.Width);
                    stringSize.Height = Math.Min(stringSize.Height, rectangle.Height);
                }

                if (stringSize.Width > rectangle.Width)
                {
                    stringSize.Width = rectangle.Width;
                }

                if (stringSize.Height > rectangle.Height)
                {
                    stringSize.Height = rectangle.Height;
                }

                RectangleF stringRectangle = LayoutUtils.Align(stringSize, rectangle, alignment);//new RectangleF(alignmentPoint, stringSize);

                if (flipText)
                {
                    graphics.TranslateTransform(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
                    graphics.RotateTransform(180);
                }

                if (shadow != null && !shadow.Depth.IsEmpty)
                {
                    using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb((int)(shadow.ShadowColor.A * opacity), graphics.GetNearestColor(shadow.ShadowColor))))
                    {
                        RectangleF fRect = stringRectangle;
                        try
                        {
                            graphics.DrawString(drawString, font, solidBrush,
                                                new RectangleF(fRect.X + shadow.Depth.X, fRect.Y + shadow.Depth.Y,
                                                               fRect.Width, fRect.Height), stringFormat);
                        }
                        catch
                        {
                            graphics.DrawString(drawString, Control.DefaultFont, solidBrush,
                                                new RectangleF(fRect.X + shadow.Depth.X, fRect.Y + shadow.Depth.Y,
                                                               fRect.Width, fRect.Height), stringFormat);
                        }
                    }
                }

                string s = null;
                if (drawString.Length > 65535)
                {
                    s = drawString.Substring(0, 65535);
                }
                else
                {
                    s = drawString;
                }

                using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb((int)(color.A * opacity), graphics.GetNearestColor(color))))
                {
                    try
                    {
                        graphics.DrawString(s, font, solidBrush, stringRectangle, stringFormat);
                    }
                    catch
                    {
                        graphics.DrawString(s, Control.DefaultFont, solidBrush, stringRectangle, stringFormat);
                    }
                }

                if (flipText)
                {
                    graphics.RotateTransform(-180);
                    graphics.TranslateTransform(-(rectangle.X + rectangle.Width), -(rectangle.Y + rectangle.Height));
                }
            }
        }

        public virtual void DrawString(string drawString, RectangleF rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, ShadowSettings shadow, TextRenderingHint textRendering, Orientation orientation, bool flipText)
        {
            if (orientation == Orientation.Vertical)
            {
                float angle = 90;

                if (flipText)
                    angle = 270;

                graphics.TranslateTransform(rectangle.X, rectangle.Y + rectangle.Height);
                graphics.RotateTransform(angle);

                RectangleF r = new RectangleF(0, 0, rectangle.Height, rectangle.Width);

                SizeF stringSize = MeasureString(drawString, font, stringFormat);

                if (stringFormat.Trimming == StringTrimming.None)
                {
                    stringSize.Width = Math.Min(stringSize.Width, r.Width);
                    stringSize.Height = Math.Min(stringSize.Height, r.Height);
                }

                if (!flipText)
                {
                    r.X -= r.Width;
                    r.Y -= r.Height;
                }

                using (Brush textBrush = new SolidBrush(color))
                    graphics.DrawString(drawString, font, textBrush, r, stringFormat);

                graphics.RotateTransform(-angle);
                graphics.TranslateTransform(-rectangle.X, -(rectangle.Y + rectangle.Height));
            }
            else
            {
                if (!CheckValidString(drawString, font, rectangle))
                    return;

                RectangleF stringRectangle = rectangle;

                if (flipText)
                {
                    graphics.TranslateTransform(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
                    graphics.RotateTransform(180);
                }

                if (shadow != null && !shadow.Depth.IsEmpty)
                {
                    using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb((int)(shadow.ShadowColor.A * opacity), graphics.GetNearestColor(shadow.ShadowColor))))
                    {
                        RectangleF fRect = stringRectangle;
                        try
                        {
                            graphics.DrawString(drawString, font, solidBrush,
                                                new RectangleF(fRect.X + shadow.Depth.X, fRect.Y + shadow.Depth.Y,
                                                               fRect.Width, fRect.Height), stringFormat);
                        }
                        catch//failed to paint with font and try to paint with Control.DefaultFont
                        {
                            graphics.DrawString(drawString, Control.DefaultFont, solidBrush,
                                                new RectangleF(fRect.X + shadow.Depth.X, fRect.Y + shadow.Depth.Y,
                                                               fRect.Width, fRect.Height), stringFormat);
                        }
                    }
                }

                using (SolidBrush solidBrush =
                        new SolidBrush(Color.FromArgb((int)(color.A * opacity), graphics.GetNearestColor(color))))
                {
                    try
                    {
                        graphics.DrawString(drawString, font, solidBrush, stringRectangle, stringFormat);
                    }
                    catch//failed to paint with font and try to paint with Control.DefaultFont
                    {
                        graphics.DrawString(drawString, Control.DefaultFont, solidBrush, stringRectangle, stringFormat);
                    }
                }

                if (flipText)
                {
                    graphics.RotateTransform(-180);
                    graphics.TranslateTransform(-(rectangle.X + rectangle.Width), -(rectangle.Y + rectangle.Height));
                }
            }
        }
        #endregion

        #region Drawing Images

        public virtual void DrawImage(Rectangle rectangle, Image image, ContentAlignment alignment, bool enabled)
        {
            if (image == null)
            {
                return;
            }

            Point alignmentPoint = CalculateCoordinates(alignment, rectangle, image.Size.Width, image.Size.Height);
            DrawImage(alignmentPoint, image, enabled);
        }

        public virtual void DrawImage(Point point, Image image, bool enabled)
        {
            if (enabled)
            {
                DrawBitmap(image, point.X, point.Y);
            }
            else
            {
                ControlPaint.DrawImageDisabled(graphics, image, point.X, point.Y, Color.Transparent);
            }
        }

        public virtual void DrawBitmap(Image image, int x, int y)
        {
            DrawBitmap(image, x, y, this.opacity);
        }

        public virtual void DrawBitmap(Image image, int x, int y, double opacity)
        {
            if (opacity == 1)
            {
                graphics.DrawImage(image, new Rectangle(x, y, image.Size.Width, image.Size.Height));
                return;
            }

            float[][] transformMatrix = BuildTransformMatrix(5, opacity);
            ColorMatrix colorMatrix = new ColorMatrix(transformMatrix);
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Rectangle rectangle = new Rectangle(x, y, image.Size.Width, image.Size.Height);
                try
                {
                    graphics.DrawImage(image, rectangle, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                catch (OutOfMemoryException)
                {
                    graphics.DrawImage(image, rectangle, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
                }
            }
        }

        public virtual void DrawBitmap(Image image, int x, int y, int width, int height)
        {
            DrawBitmap(image, x, y, width, height, this.opacity);
        }

        public virtual void DrawBitmap(Image image, int x, int y, int width, int height, double opacity)
        {
            if (opacity == 1)
            {
                graphics.DrawImage(image, new Rectangle(x, y, width, height));
                return;
            }

            float[][] transformMatrix = BuildTransformMatrix(5, opacity);
            ColorMatrix colorMatrix = new ColorMatrix(transformMatrix);
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Rectangle rectangle = new Rectangle(x, y, width, height);
                try
                {
                    graphics.DrawImage(image, rectangle, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                catch (OutOfMemoryException)
                {
                    graphics.DrawImage(image, rectangle, 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel);
                }
            }
        }

        public Bitmap CreateBitmapMask(Color maskColor, Bitmap bitmap)
        {
            Bitmap clonedBitmap = (Bitmap)bitmap.Clone();
            for (int i = 0; i < bitmap.Size.Width; i++)
            {
                for (int j = 0; j < bitmap.Size.Height; j++)
                {
                    Color color1 = bitmap.GetPixel(i, j);
                    if (color1.A > 0)
                    {
                        clonedBitmap.SetPixel(i, j, maskColor);
                    }
                }
            }
            return clonedBitmap;
        }

        #endregion

        #region Private Methods

        private bool CheckValidString(string drawString, Font font, Rectangle rectangle)
        {
            if (String.IsNullOrEmpty(drawString) || (font == null))
                return false;

            if ((rectangle.Height < 1) || (rectangle.Width < 1))
                return false;

            return true;
        }

        private bool CheckValidString(string drawString, Font font, RectangleF rectangle)
        {
            if (String.IsNullOrEmpty(drawString) || (font == null))
                return false;

            if ((rectangle.Height < 1) || (rectangle.Width < 1))
                return false;

            return true;
        }

        public SizeF MeasureString(string text, Font font, StringFormat stringFormat)
        {
            return MeasureString(text, font, 0, stringFormat);
        }

        public SizeF MeasureString(string text, Font font, int availableWidth, StringFormat stringFormat)
        {
            string s = "";
            if (text.Length > 65535)
                s = text.Substring(0, 65535);
            else
                s = text;
            SizeF sizeRectangle = graphics.MeasureString(s, font, availableWidth, stringFormat);

            if (font.Bold && (text.Length > 0))
            {
                SizeF outerSize = SizeF.Empty;
                SizeF innerSize = SizeF.Empty;
                using (StringFormat format1 = new StringFormat(stringFormat))
                {
                    RectangleF tempRectangle;
                    CharacterRange[] characterRanges = new CharacterRange[1] { new CharacterRange(0, text.Length) };
                    format1.SetMeasurableCharacterRanges(characterRanges);
                    Region[] oldRegions = graphics.MeasureCharacterRanges(text, font, new RectangleF(0f, 0f, float.MaxValue, float.MaxValue), format1);
                    if (oldRegions.Length > 0)
                    {
                        tempRectangle = oldRegions[0].GetBounds(graphics);
                        outerSize = tempRectangle.Size;
                    }
                    try
                    {
                        using (Font font1 = new Font(font, FontStyle.Regular))
                        {
                            Region[] newRegions = graphics.MeasureCharacterRanges(text, font1, new RectangleF(0f, 0f, float.MaxValue, float.MaxValue), format1);
                            if (newRegions.Length > 0)
                            {
                                tempRectangle = newRegions[0].GetBounds(graphics);
                                innerSize = tempRectangle.Size;
                            }
                        }
                    }
                    catch //Some fonts does not support regular style
                    {
                        Region[] newRegions = graphics.MeasureCharacterRanges(text, font, new RectangleF(0f, 0f, float.MaxValue, float.MaxValue), format1);
                        if (newRegions.Length > 0)
                        {
                            tempRectangle = newRegions[0].GetBounds(graphics);
                            innerSize = tempRectangle.Size;
                        }
                    }
                }

                float tmpXOffset = (outerSize.Width - innerSize.Width);
                float tmpYOffset = (outerSize.Height - innerSize.Height);

                if (tmpXOffset < 0)
                {
                    tmpXOffset = 0;
                }

                if (tmpYOffset < 0)
                {
                    tmpYOffset = 0;
                }

                sizeRectangle.Width += tmpXOffset;
                sizeRectangle.Height += tmpYOffset;
            }
            return sizeRectangle;
        }

        //private PointF CalculateCoordinates(ContentAlignment alignment, RectangleF rectangle, float width, float height)
        //{
        //    float left = rectangle.Left;
        //    float top = rectangle.Top;

        //    if (alignment <= ContentAlignment.MiddleCenter)
        //    {
        //        switch (alignment)
        //        {
        //            case ContentAlignment.TopLeft:
        //            case (ContentAlignment.TopCenter | ContentAlignment.TopLeft):
        //                {
        //                    break;
        //                }
        //            case ContentAlignment.TopCenter:
        //                {
        //                    left += ((rectangle.Width - width) / 2);
        //                    break;
        //                }
        //            case ContentAlignment.TopRight:
        //                {
        //                    left += (rectangle.Width - width);
        //                    break;
        //                }
        //            case ContentAlignment.MiddleLeft:
        //                {
        //                    top += ((rectangle.Height - height) / 2);
        //                    break;
        //                }
        //            case ContentAlignment.MiddleCenter:
        //                {
        //                    left += ((rectangle.Width - width) / 2);
        //                    top += ((rectangle.Height - height) / 2);
        //                    break;
        //                }
        //        }
        //    }
        //    else if (alignment <= ContentAlignment.BottomLeft)
        //    {
        //        if (alignment == ContentAlignment.MiddleRight)
        //        {
        //            left += (rectangle.Width - width);
        //            top += ((rectangle.Height - height) / 2);
        //        }
        //        else if (alignment == ContentAlignment.BottomLeft)
        //        {
        //            top += (rectangle.Height - height);
        //        }
        //    }
        //    else if (alignment != ContentAlignment.BottomCenter)
        //    {
        //        if (alignment == ContentAlignment.BottomRight)
        //        {
        //            left += (rectangle.Width - width);
        //            top += (rectangle.Height - height);
        //        }
        //    }
        //    else
        //    {
        //        left += ((rectangle.Width - width) / 2);
        //        top += (rectangle.Height - height);
        //    }
        //    return new PointF(left, top);
        //}

        private Point CalculateCoordinates(ContentAlignment alignment, Rectangle rectangle, int width, int height)
        {
            int left = rectangle.Left;
            int top = rectangle.Top;

            if (alignment <= ContentAlignment.MiddleCenter)
            {
                switch (alignment)
                {
                    case ContentAlignment.TopLeft:
                    case (ContentAlignment.TopCenter | ContentAlignment.TopLeft):
                        {
                            break;
                        }
                    case ContentAlignment.TopCenter:
                        {
                            left += ((rectangle.Width - width) / 2);
                            break;
                        }
                    case ContentAlignment.TopRight:
                        {
                            left += (rectangle.Width - width);
                            break;
                        }
                    case ContentAlignment.MiddleLeft:
                        {
                            top += ((rectangle.Height - height) / 2);
                            break;
                        }
                    case ContentAlignment.MiddleCenter:
                        {
                            left += ((rectangle.Width - width) / 2);
                            top += ((rectangle.Height - height) / 2);
                            break;
                        }
                }
            }
            else if (alignment <= ContentAlignment.BottomLeft)
            {
                if (alignment == ContentAlignment.MiddleRight)
                {
                    left += (rectangle.Width - width);
                    top += ((rectangle.Height - height) / 2);
                }
                else if (alignment == ContentAlignment.BottomLeft)
                {
                    top += (rectangle.Height - height);
                }
            }
            else if (alignment != ContentAlignment.BottomCenter)
            {
                if (alignment == ContentAlignment.BottomRight)
                {
                    left += (rectangle.Width - width);
                    top += (rectangle.Height - height);
                }
            }
            else
            {
                left += ((rectangle.Width - width) / 2);
                top += (rectangle.Height - height);
            }
            return new Point(left, top);
        }

        public static ImageAttributes GetOpacityAttributes(float opacity)
        {
            ImageAttributes attributes = new ImageAttributes();
            ColorMatrix matrix = new ColorMatrix();
            matrix.Matrix00 = 1F;
            matrix.Matrix11 = 1F;
            matrix.Matrix22 = 1F;
            matrix.Matrix33 = opacity;
            matrix.Matrix44 = 1F;
            attributes.SetColorMatrix(matrix);

            return attributes;
        }

        private float[][] BuildTransformMatrix(int length, double opacity)
        {
            float[][] transformMatrix = new float[length][];
            for (int i = 0; i < length; i++)
            {
                float[] matrixRow = new float[length];
                matrixRow[i] = 1f;
                transformMatrix[i] = matrixRow;
            }

            transformMatrix[3][3] = (float)opacity;

            return transformMatrix;
        }

        public GraphicsPath GetRoundedRect(RectangleF baseRect, float radius)
        {
            if (radius <= 0.0F)
            {
                GraphicsPath mPath = new GraphicsPath();
                mPath.AddRectangle(baseRect);
                mPath.CloseFigure();

                return mPath;
            }

            if (radius >= (Math.Min(baseRect.Width, baseRect.Height)) / 2.0)
                return GetCapsule(baseRect);

            float diameter = radius * 2.0F;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(baseRect.Location, sizeF);
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(arc, 180, 90);

            arc.X = baseRect.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = baseRect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = baseRect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();

            return path;
        }

        private GraphicsPath GetCapsule(RectangleF baseRect)
        {
            float diameter;
            RectangleF arc;
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            if (baseRect.Height <= 0 || baseRect.Width <= 0)
            {
                path.AddEllipse(baseRect);
                path.CloseFigure();
                return path;
            }

            try
            {
                if (baseRect.Width > baseRect.Height)
                {
                    diameter = baseRect.Height;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    path.AddArc(arc, 90, 180);
                    arc.X = baseRect.Right - diameter;
                    path.AddArc(arc, 270, 180);
                }
                else if (baseRect.Width < baseRect.Height)
                {
                    diameter = baseRect.Width;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    path.AddArc(arc, 180, 180);
                    arc.Y = baseRect.Bottom - diameter;
                    path.AddArc(arc, 0, 180);
                }
                else
                {
                    path.AddEllipse(baseRect);
                }
            }
            catch (Exception)
            {
                path.AddEllipse(baseRect);
            }
            finally
            {
                path.CloseFigure();
            }
            return path;
        }

        #endregion

        #region Dispose
        /// <summary>
        /// Disposes the object. 
        /// </summary>
        public void Destroy()
        {
            Dispose(true);
        }
        /// <summary>Disposes the GDI+ graphics context.</summary>
        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);

        }

        private void Dispose(bool finalizing)
        {
            if (!finalizing)
            {
                graphics.Dispose();
            }
        }

        ~RadGdiGraphics()
        {
            Destroy();
        }
        #endregion

        public static FontTextMetrics GetTextMetric(Font font)
        {
            //get the TEXTMETRIC for the font info
            NativeMethods.TEXTMETRIC textMetric = new NativeMethods.TEXTMETRIC();
            lock (Telerik.WinControls.MeasurementGraphics.SyncObject)
            {
                Graphics measurementGraphics = MeasurementGraphics;

                IntPtr hdc = measurementGraphics.GetHdc();
                IntPtr hFont = font.ToHfont();
                IntPtr hOldObject = NativeMethods.SelectObject(new HandleRef(null, hdc), new HandleRef(null, hFont));

                NativeMethods.GetTextMetrics(hdc, ref textMetric);

                //clean-up
                NativeMethods.SelectObject(new HandleRef(null, hdc), new HandleRef(null, hOldObject));
                NativeMethods.DeleteObject(new HandleRef(null, hFont));

                measurementGraphics.ReleaseHdc(hdc);
                //measurementGraphics.Dispose();
            }

                FontTextMetrics fontTextMetrics = new FontTextMetrics();
                fontTextMetrics.height = textMetric.tmHeight;
                fontTextMetrics.ascent = textMetric.tmAscent;
                fontTextMetrics.descent = textMetric.tmDescent;
                fontTextMetrics.internalLeading = textMetric.tmInternalLeading;
                fontTextMetrics.externalLeading = textMetric.tmExternalLeading;
                fontTextMetrics.aveCharWidth = textMetric.tmAveCharWidth;
                fontTextMetrics.maxCharWidth = textMetric.tmMaxCharWidth;
                fontTextMetrics.weight = textMetric.tmWeight;
                fontTextMetrics.overhang = textMetric.tmOverhang;
                fontTextMetrics.digitizedAspectX = textMetric.tmDigitizedAspectX;
                fontTextMetrics.digitizedAspectY = textMetric.tmDigitizedAspectY;

            return fontTextMetrics;
        }

        protected bool CheckValidRectangle(RectangleF rectangle)
        {
            if (rectangle.IsEmpty)
                return false;

            if ((rectangle.Width <= 0) || (rectangle.Height <= 0))
                return false;

            return true;
        }

        protected bool CheckValidRectangle(Rectangle rectangle)
        {
            if (rectangle.IsEmpty)
                return false;

            if ((rectangle.Width <= 0) || (rectangle.Height <= 0))
                return false;

            return true;
        }
    }
}
