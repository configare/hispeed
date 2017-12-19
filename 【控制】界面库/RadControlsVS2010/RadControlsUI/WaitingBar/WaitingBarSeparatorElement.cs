using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class WaitingBarSeparatorElement : LightVisualElement
    {
        #region Initialization

        static WaitingBarSeparatorElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new WaitingBarSeparatorStateManager(), typeof(WaitingBarSeparatorElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        #endregion

        #region RadProperties

        public static RadProperty DashProperty = RadProperty.Register("Dash", typeof(bool),
         typeof(WaitingBarSeparatorElement), new RadElementPropertyMetadata(
             false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsVerticalProperty = RadProperty.Register("IsVertical", typeof(bool),
        typeof(WaitingBarSeparatorElement), new RadElementPropertyMetadata(
           false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HatchProperty = RadProperty.Register("Hatch", typeof(bool),
         typeof(WaitingBarSeparatorElement), new RadElementPropertyMetadata(
             false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorWidthProperty = RadProperty.Register("SeparatorWidth", typeof(int),
         typeof(WaitingBarSeparatorElement), new RadElementPropertyMetadata(
             3, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty StepWidthProperty = RadProperty.Register("StepWidth", typeof(int),
        typeof(WaitingBarSeparatorElement), new RadElementPropertyMetadata(
            14, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ProgressOrientationProperty = RadProperty.Register("ProgressOrientation", typeof(ProgressOrientation),
         typeof(WaitingBarSeparatorElement), new RadElementPropertyMetadata(
             ProgressOrientation.Left, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SweepAngleProperty = RadProperty.Register("SweepAngle", typeof(int),
         typeof(WaitingBarSeparatorElement), new RadElementPropertyMetadata(
             45, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Properties

        /// <summary>
        /// Sets and gets the width of each separator line in pixels
        /// </summary>
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

        /// <summary>
        /// Sets and gets the distance between two adjacent separator lines
        /// </summary>
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

        /// <summary>
        /// Sets and gets the orientation of the separator element
        /// </summary>
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

        /// <summary>
        /// Sets and gets the angle of rotation of all separator lines
        /// </summary>
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

        /// <summary>
        /// Indicates whether separator lines should be drawn
        /// </summary>
        public bool Dash
        {
            get
            {
                return (bool)this.GetValue(DashProperty);
            }
            set
            {
                this.SetValue(DashProperty, value);
                InvalidateMeasure(true);
            }
        }

        /// <summary>
        /// Indicates whether a second set of separator lines should be drawn
        /// </summary>
        public bool Hatch
        {
            get
            {
                return (bool)this.GetValue(HatchProperty);
            }
            set
            {
                this.SetValue(HatchProperty, value);
                InvalidateMeasure(true);
            }
        }

        #endregion

        #region Painting

        protected override void  PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            graphics.ChangeSmoothingMode(SmoothingMode.AntiAlias);
            if (this.Size.Width <= 2 || this.Size.Height <= 2)
            {
                return;
            }

            int step = this.StepWidth + this.SeparatorWidth;
            float beginPoint = this.StepWidth;

            LinearGradientBrush brush = this.CreateBrush();

            if (this.ProgressOrientation == ProgressOrientation.Left || this.ProgressOrientation == ProgressOrientation.Right)
            {
                if (this.ProgressOrientation == ProgressOrientation.Left)
                {
                    beginPoint = this.Size.Width % step;
                    beginPoint += this.StepWidth;

                }

                float dx = (float)Math.Tan(DegreeToRadian(90 - this.SweepAngle)) * this.Size.Height;


                if (this.Dash && !this.Hatch)
                {
                    DrawHorizontalDashLines(graphics, beginPoint, step, dx, brush);
                }

                if (this.Hatch)
                {
                    DrawHorizontalDashLines(graphics, beginPoint, step, dx, brush);
                    DrawHorizontalDashLines(graphics, beginPoint, step, -dx, brush);
                }
            }
            else
            {
                if (this.ProgressOrientation == ProgressOrientation.Top)
                {
                    beginPoint = this.Size.Height % step;
                    beginPoint += this.StepWidth;
                }

                float dy = (float)Math.Tan(DegreeToRadian(90 - this.SweepAngle)) * this.Size.Width;

                if (this.Dash && !this.Hatch)
                {
                    DrawVerticalDashLines(graphics, beginPoint, step, dy, brush);
                }

                if (this.Hatch)
                {
                    DrawVerticalDashLines(graphics, beginPoint, step, dy, brush);
                    DrawVerticalDashLines(graphics, beginPoint, step, -dy, brush);
                }
            }

            graphics.RestoreSmoothingMode();
            base.PaintElement(graphics, angle, scale);
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

            if (this.ProgressOrientation == ProgressOrientation.Right)
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

            if (this.ProgressOrientation == ProgressOrientation.Bottom)
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
                lgBrush = new LinearGradientBrush(brushRect, this.BackColor, this.BackColor, this.GradientAngle);
                return lgBrush;
            }

            if (this.NumberOfColors < 3)
            {
                lgBrush = new LinearGradientBrush(brushRect, this.BackColor, this.BackColor2, this.GradientAngle);
                return lgBrush;
            }

            Color[] colors = new Color[4];
            float[] colorPositions = new float[4];
            
            colors[0] = this.BackColor;
            colors[1] = this.BackColor2;
            colors[2] = this.BackColor2;
            colors[3] = this.BackColor3;

            colorPositions[0] = 0f;
            colorPositions[1] = this.GradientPercentage;
            colorPositions[2] = this.GradientPercentage2;
            colorPositions[3] = 1f;

            if (this.NumberOfColors > 3)
            {
                colors[2] = this.BackColor3;
                colors[3] = this.BackColor4;
            }

            lgBrush = new LinearGradientBrush(brushRect, Color.White, Color.White, this.GradientAngle);
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
