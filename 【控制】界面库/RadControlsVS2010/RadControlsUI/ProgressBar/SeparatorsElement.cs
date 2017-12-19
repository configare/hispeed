using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Primitives;
using System;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.UI
{
    public class SeparatorsElement : BasePrimitive
    {
        #region Fields

        bool dash;
        bool hatch;

        #endregion

        #region RadProperties

        public static RadProperty SeparatorWidthProperty = RadProperty.Register("SeparatorWidth", typeof(int),
         typeof(SeparatorsElement), new RadElementPropertyMetadata(
             3, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty StepWidthProperty = RadProperty.Register("StepWidth", typeof(int),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            14, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorColor1Property = RadProperty.Register("SeparatorColor1", typeof(Color),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            Color.White, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorColor2Property = RadProperty.Register("SeparatorColor2", typeof(Color),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            Color.White, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorColor3Property = RadProperty.Register("SeparatorColor3", typeof(Color),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            Color.White, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorColor4Property = RadProperty.Register("SeparatorColor4", typeof(Color),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            Color.White, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorGradientAngleProperty = RadProperty.Register("SeparatorGeadientAngle", typeof(int),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            0, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorGradientPercentage1Property = RadProperty.Register("SeparatorGradientPercentage1", typeof(float),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            0.4f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorGradientPercentage2Property = RadProperty.Register("SeparatorGradientPercentage2", typeof(float),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            0.6f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ProgressOrientationProperty = RadProperty.Register("ProgressOrientation", typeof(ProgressOrientation),
         typeof(SeparatorsElement), new RadElementPropertyMetadata(
             ProgressOrientation.Left, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SweepAngleProperty = RadProperty.Register("SweepAngle", typeof(int),
         typeof(SeparatorsElement), new RadElementPropertyMetadata(
             90, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty NumberOfColorsProperty = RadProperty.Register("NumberOfColors", typeof(int),
        typeof(SeparatorsElement), new RadElementPropertyMetadata(
            2, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Properties

        public int SeparatorWidth
        {
            get
            {                
                return (int)this.GetValue(SeparatorWidthProperty);
            }
            set
            {
                this.SetValue(SeparatorWidthProperty, value);
            }
        }

        public int StepWidth
        {
            get
            {
                return (int)this.GetValue(StepWidthProperty);
            }
            set
            {
                this.SetValue(StepWidthProperty, value);
            }
        }

        public Color SeparatorColor1
        {
            get
            {
                return (Color)this.GetValue(SeparatorColor1Property);
            }
            set
            {
                this.SetValue(SeparatorColor1Property, value);
            }
        }

        public Color SeparatorColor2
        {
            get
            {
                return (Color)this.GetValue(SeparatorColor2Property);
            }
            set
            {
                this.SetValue(SeparatorColor2Property, value);
            }
        }

        public Color SeparatorColor3
        {
            get
            {
                return (Color)this.GetValue(SeparatorColor3Property);
            }
            set
            {
                this.SetValue(SeparatorColor3Property, value);
            }
        }

        public Color SeparatorColor4
        {
            get
            {
                return (Color)this.GetValue(SeparatorColor4Property);
            }
            set
            {
                this.SetValue(SeparatorColor4Property, value);
            }
        }

        public int SeparatorGradientAngle
        {
            get
            {
                return (int)this.GetValue(SeparatorGradientAngleProperty);
            }
            set
            {
                this.SetValue(SeparatorGradientAngleProperty, value);
            }
        }

        public float SeparatorGradientPercentage1
        {
            get
            {
                return (float)this.GetValue(SeparatorGradientPercentage1Property);
            }
            set
            {
                this.SetValue(SeparatorGradientPercentage1Property, value);
            }
        }

        public float SeparatorGradientPercentage2
        {
            get
            {
                return (float)this.GetValue(SeparatorGradientPercentage2Property);
            }
            set
            {
                this.SetValue(SeparatorGradientPercentage2Property, value);
            }
        }

        public int NumberOfColors
        {
            get
            {
                return (int)this.GetValue(NumberOfColorsProperty);
            }
            set
            {
                this.SetValue(NumberOfColorsProperty, value);
            }
        }

        public ProgressOrientation ProgressOrientation
        {
            get
            {
                return (ProgressOrientation)this.GetValue(ProgressOrientationProperty);
            }
            set
            {
                this.SetValue(ProgressOrientationProperty, value);
            }
        }

        public int SweepAngle
        {
            get
            {
                return (int)this.GetValue(SweepAngleProperty);
            }
            set
            {
                this.SetValue(SweepAngleProperty, value);
            }
        }

        public bool Dash
        {
            get
            {
                return this.dash;
            }
            set
            {
                this.dash = value;
            }
        }

        public bool Hatch
        {
            get
            {
                return this.hatch;
            }
            set
            {
                this.hatch = value;
            }
        }

        #endregion

        #region Painting

        public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
            graphics.ChangeSmoothingMode(SmoothingMode.AntiAlias);
            if (this.Size.Width <= 2 || this.Size.Height <= 2)
            {
                return;
            }

            int step = this.StepWidth + this.SeparatorWidth;
            float beginPoint = this.StepWidth;
            
            (graphics.UnderlayGraphics as Graphics).SetClip(new RectangleF(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height));
            Region clipRegion = (graphics.UnderlayGraphics as Graphics).Clip;
                        
            LinearGradientBrush brush = this.CreateBrush();

            if (this.ProgressOrientation == ProgressOrientation.Left || this.ProgressOrientation == ProgressOrientation.Right)
            {
                if (this.ProgressOrientation == ProgressOrientation.Right)
                {
                    beginPoint = this.Size.Width % step;
                    beginPoint += this.StepWidth;

                }

                float dx = (float)Math.Tan(DegreeToRadian(90 - this.SweepAngle)) * this.Size.Height;


                if (this.dash && !this.hatch)
                {
                    DrawHorizontalDashLines(graphics, beginPoint, step, dx, brush);
                }

                if (this.hatch)
                {
                    DrawHorizontalDashLines(graphics, beginPoint, step, dx, brush);
                    DrawHorizontalDashLines(graphics, beginPoint, step, -dx, brush);
                }
            }
            else
            {
                if (this.ProgressOrientation == ProgressOrientation.Bottom)
                {
                    beginPoint = this.Size.Height % step;
                    beginPoint += this.StepWidth;
                }

                float dy = (float)Math.Tan(DegreeToRadian(90 - this.SweepAngle)) * this.Size.Width; 

                if (this.dash && !this.hatch)
                {
                    DrawVerticalDashLines(graphics, beginPoint, step, dy, brush);
                }

                if (this.hatch)
                {
                    DrawVerticalDashLines(graphics, beginPoint, step, dy, brush);
                    DrawVerticalDashLines(graphics, beginPoint, step, -dy, brush);
                }
            }
            (graphics.UnderlayGraphics as Graphics).Clip = clipRegion;
            graphics.RestoreSmoothingMode();
        }

        private void DrawHorizontalDashLines(IGraphics graphics, float beginPoint, int step, float dx, LinearGradientBrush brush)
        {
            PointF[] points = new PointF[4];
            points[0] = new PointF(beginPoint, 0);
            points[1] = new PointF(points[0].X + SeparatorWidth, 0);
            points[2] = new PointF(points[1].X - dx, this.Size.Height);
            points[3] = new PointF(points[0].X - dx, this.Size.Height);

            int leftEnd = (int)(-Math.Abs(dx));
            int rightEnd = (int)(Size.Width + Math.Abs(dx));

            if (this.ProgressOrientation == ProgressOrientation.Left)
            {
                for (int i = leftEnd; i < rightEnd; i = i + step)
                {
                    points[0].X = i;
                    points[1].X = points[0].X + SeparatorWidth;
                    points[2].X = points[1].X - dx;
                    points[3].X = points[0].X - dx;
                    graphics.FillPolygon(brush, points);
                }
            }
            else
            {
                for (int i = rightEnd; i > leftEnd; i = i - step)
                {
                    points[0].X = i;
                    points[1].X = points[0].X + SeparatorWidth;
                    points[2].X = points[1].X - dx;
                    points[3].X = points[0].X - dx;
                    graphics.FillPolygon(brush, points);
                }
            }
        }

        private void DrawVerticalDashLines(IGraphics graphics, float beginPoint, int step, float dy, LinearGradientBrush brush)
        {
            PointF[] points = new PointF[4];
            points[0] = new PointF(0, beginPoint);
            points[1] = new PointF(0, points[0].Y + SeparatorWidth);
            points[2] = new PointF(this.Size.Width, points[1].Y - dy);
            points[3] = new PointF(this.Size.Width, points[0].Y - dy);

            int topEnd = (int)(-Math.Abs(dy));
            int bottomEnd = (int)(this.Size.Height + Math.Abs(dy));

            if (this.ProgressOrientation == ProgressOrientation.Top)
            {
                for (int i = topEnd; i < bottomEnd; i = i + step)
                {
                    points[0].Y = i;
                    points[1].Y = points[0].Y + SeparatorWidth;
                    points[2].Y = points[1].Y - dy;
                    points[3].Y = points[0].Y - dy;
                    graphics.FillPolygon(brush, points);
                }
            }
            else
            {
                for (int i = bottomEnd; i > topEnd; i = i - step)
                {
                    points[0].Y = i;
                    points[1].Y = points[0].Y + SeparatorWidth;
                    points[2].Y = points[1].Y + dy;
                    points[3].Y = points[0].Y + dy;
                    graphics.FillPolygon(brush, points);
                }
            }
        }

        private LinearGradientBrush CreateBrush()
        {
            RectangleF brushRect = new RectangleF(0, 0, this.Size.Width, this.Size.Height);
            LinearGradientBrush lgBrush;
            
            if (this.NumberOfColors < 2)
            {
                lgBrush = new LinearGradientBrush(brushRect, this.SeparatorColor1, this.SeparatorColor1, this.SeparatorGradientAngle);
                return lgBrush;
            }

            if (this.NumberOfColors < 3)
            {
                lgBrush = new LinearGradientBrush(brushRect, this.SeparatorColor1, this.SeparatorColor2, this.SeparatorGradientAngle);
                return lgBrush;
            }

            Color[] colors = new Color[4];
            float[] colorPositions = new float[4];

            colors[0] = this.SeparatorColor1;
            colors[1] = this.SeparatorColor2;
            colors[2] = this.SeparatorColor2;
            colors[3] = this.SeparatorColor3;

            colorPositions[0] = 0f;
            colorPositions[1] = this.SeparatorGradientPercentage1;
            colorPositions[2] = this.SeparatorGradientPercentage2;
            colorPositions[3] = 1f;

            if (this.NumberOfColors > 3)
            {
                colors[2] = this.SeparatorColor3;
                colors[3] = this.SeparatorColor4;
            }
            
            lgBrush = new LinearGradientBrush(brushRect, Color.White, Color.White, this.SeparatorGradientAngle);
            ColorBlend colorBlend = new ColorBlend();
            colorBlend.Colors = colors;
            colorBlend.Positions = colorPositions;
            lgBrush.InterpolationColors = colorBlend;

            return lgBrush;
        }
      
        #endregion

        #region Methods

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        #endregion
    }
}
