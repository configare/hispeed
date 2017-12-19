using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Paint;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Design;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
namespace Telerik.WinControls.Primitives
{
    /// <summary>Represents the internal part of the progress bar.</summary>
    public class ProgressBarPrimitive : FillPrimitive
    {
		private const int DefaultSeparatorWidth = 8;
        private int width;
        private int height;
        private int sweepAngle;
		private bool image;
		private Rectangle progressRect;

	    public static RadProperty DashProperty = RadProperty.Register("Dash", typeof(bool),
            typeof(ProgressBarPrimitive),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay ));

        public static RadProperty ValueProperty1 = RadProperty.Register("Value1", typeof(int),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ValueProperty2 = RadProperty.Register("Value2", typeof(int),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty MinimumProperty = RadProperty.Register("Minimum", typeof(int),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty MaximumProperty = RadProperty.Register("Maximum", typeof(int),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(100, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty StepProperty = RadProperty.Register("Step", typeof(int),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(10, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty StepWidthProperty = RadProperty.Register("StepWidth", typeof(int),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(8, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorWidthProperty = RadProperty.Register("SeparatorWidth", typeof(int),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(8, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HatchProperty = RadProperty.Register("Hatch", typeof(bool),
           typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SweepAngleProperty = RadProperty.Register("SweepAngle", typeof(int),
         typeof(ProgressBarPrimitive),
          new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorColorProperty1 = RadProperty.Register("SeparatorColor1", typeof(Color),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(Color.White, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty SeparatorColorProperty2 = RadProperty.Register("SeparatorColor2", typeof(Color),
         typeof(ProgressBarPrimitive),
         new RadElementPropertyMetadata(Color.White, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ProgressOrientationProperty = RadProperty.Register("Orientation", typeof(ProgressOrientation),
          typeof(ProgressBarPrimitive),
           new RadElementPropertyMetadata(ProgressOrientation.Left, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// indicates that Progress Bar has Image
        /// </summary>
		public bool HasImage
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
			}
		}
		/// <summary>
        /// Gets or sets progress bar orientation. Possible values are indicates in
        /// ProgressOrientaion enumeration: up, left, bottom, and right.
        /// </summary>
        public virtual ProgressOrientation Orientation
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
        /// 	<para>Indicates whether the progress bar style is dash. If both dash and hash are
        ///     true, hatch style is chosen.</para>
        /// </summary>
        public virtual bool Dash
        {
            get
            {
                return (bool)GetValue(DashProperty); 
            }
            set
            {
                SetValue(DashProperty, value);
            }
        }

        /// <summary>
        /// Indicates whether the progress bar style is hatch. When true, the style is Hatch.
        /// When both dash and hatch are true the style is hatch.
        /// </summary>
        public virtual bool Hatch
        {
            get
            {
                return (bool)GetValue(HatchProperty);
            }
            set
            {
                SetValue(HatchProperty, value);
            }
        }

        /// <summary>Gets or sets the angle in degrees of the progress bar dash or hatch parts.</summary>
        public virtual int SweepAngle
        {
            get
            {
                return (int)GetValue(SweepAngleProperty);
            }
            set
            {
                SetValue(SweepAngleProperty, value);
            }
        }

        /// <summary>Gets or sets the step width in pixels between separators.</summary>
        public virtual int StepWidth
        {
            get
            {
                return (int)GetValue(StepWidthProperty);
            }
            set
            {
                SetValue(StepWidthProperty, value);
            }
        }

        /// <summary>Gets or sets separators width in pixels.</summary>
        public virtual int SeparatorWidth
        {
            get
            {
                return (int)GetValue(SeparatorWidthProperty);
            }
            set
            {
                SetValue(SeparatorWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the first progress line. There could be two progress
        /// lines in the progress bar.
        /// </summary>
		public virtual int Value1
		{
			get
			{
				return (int)GetValue(ValueProperty1);
			}
			set
			{
				if (value < Minimum)
				{
					throw new ArgumentException("'"+value+"' is not a valid value for 'Value'.\n"+
						"'Value' must be between 'Minimum' and 'Maximum'.");
				}

				if (value > Maximum)
				{
					throw new ArgumentException("'"+value+"' is not a valid value for 'Value'.\n"+
						"'Value' must be between 'Minimum' and 'Maximum'.");
				}

				SetValue( ValueProperty1, value );			
				
			}
		}

        /// <summary>
        /// Gets or sets the value of the second progress line. There could be two progress
        /// lines in the progress bar.
        /// </summary>
        public virtual int Value2
        {
            get
            {
                return (int)GetValue(ValueProperty2);
            }
            set
            {
                if (value < Minimum)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                if (value > Maximum)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value'.\n" +
                        "'Value' must be between 'Minimum' and 'Maximum'.");
                }

                SetValue(ValueProperty2, value);

            }
        }

        /// <summary>Specifies minimum value for the progress.</summary>
		public virtual int Minimum
		{
			get
			{
                return (int)GetValue(MinimumProperty);
			}
			set
			{
                SetValue(MinimumProperty, value);
              
                if (Minimum > Maximum)
                    Maximum = Minimum;
                if (Minimum > Value1)
                    Value1 = Minimum;
                if (Minimum > Value2)
                    Value2 = Minimum;
			
            }
		}

        /// <summary>Gets or sets maximum value for the progress.</summary>
	    public virtual int Maximum
		{
			get
			{
                return (int)GetValue(MaximumProperty);
			}
			set
			{
                SetValue(MaximumProperty, value);
				if (Maximum < Value1)
					Value1 = Maximum;
                if (Maximum < Value2)
                    Value2 = Maximum;	
                if (Maximum < Minimum)
					Minimum = Maximum;
			}
		}

        /// <summary>
        /// indicates Stap value
        /// </summary>
        public virtual int Step
		{
			get
			{
                return (int)GetValue(StepProperty);
			}
			set
			{
                SetValue(StepProperty, value);
			}
		}

        /// <summary>Gets or sets the first color that is used in gradient effect.</summary>
        public virtual Color SeparatorColor1
        {
            get
            {
                return (Color)GetValue(SeparatorColorProperty1);    
            }
            set
            {
                SetValue(SeparatorColorProperty1, value);
            }
        }

        /// <summary>Gets or sets the second color that is used in gradient effect.</summary>
        public virtual Color SeparatorColor2
        {
            get
            {
                return (Color)GetValue(SeparatorColorProperty2);
            }
            set
            {
                SetValue(SeparatorColorProperty2, value);
            }
        }


        private void SetSize()
        {
            this.width = this.Parent.Size.Width;
            this.height = this.Parent.Size.Height;
        }

        /// <summary>Draws the primitive on the screen.</summary>
        public override void PaintPrimitive(Telerik.WinControls.Paint.IGraphics g, float angle, SizeF scale)
        {
            if (this.Parent == null)
            {
                return;
            }

            this.SetSize();
            if (this.width <= 0 || this.height <= 0)
            {
                return;
            }

            int colorsMaxValue = 4;
            
            Color[] colorStops = new Color[Math.Min(Math.Max(this.NumberOfColors, 1), colorsMaxValue)];
            float[] colorOffsets = new float[Math.Min(Math.Max(this.NumberOfColors, 1), colorsMaxValue)];

            if (this.NumberOfColors > 0)
            {
                colorStops[0] = BackColor;
                colorOffsets[0] = 0f;
            }

            if (this.NumberOfColors > 1)
            {
                colorStops[1] = BackColor2;
                colorOffsets[colorStops.Length - 1] = 1f;
            }

            if (this.NumberOfColors > 2)
            {
                colorStops[2] = BackColor3;
                colorOffsets[1] = this.GradientPercentage;
            }

            if (this.NumberOfColors > 3)
            {
                colorStops[3] = BackColor4;
                colorOffsets[2] = this.GradientPercentage2;
            }

            Rectangle rect = new Rectangle(Point.Empty, this.Size);
        
			int delta = Math.Max(1, this.Maximum - this.Minimum);
            int fillWidth = (int)Math.Round(((float)this.width * Value1) / delta);
            int fillHeight = (int)Math.Round(((float)this.height * Value1) / delta);
            int fillWidthValue2 = (int)Math.Round(((float)this.width * Value2) / delta);
            int fillHeightValue2 = (int)Math.Round(((float)this.height * Value2) / delta);
          
			int sepCount = (int)Math.Ceiling((double)fillWidth / ProgressBarPrimitive.DefaultSeparatorWidth);

            Rectangle chunkbar = new Rectangle(0, 0, fillWidth, height );
            Rectangle chunkBarValue2 = new Rectangle(0, 0, fillWidthValue2, height);
            // FILL the progress area
            switch (Orientation)
            {
                case ProgressOrientation.Top:
                    {
                        chunkbar = new Rectangle(0, 0, this.width, fillHeight);
                        chunkBarValue2 = new Rectangle(0, 0, this.width, fillHeightValue2);
                      
                        break;
                    }
                case ProgressOrientation.Right:
                    {
                        chunkbar = new Rectangle( this.width - fillWidth, 0, fillWidth, this.height);
                        chunkBarValue2 = new Rectangle(this.width - fillWidthValue2, 0, fillWidthValue2, this.height);      
                   
                        break;
                    }
                case ProgressOrientation.Bottom:
                    {
                        chunkbar = new Rectangle(0, height - fillHeight, this.width, fillHeight);
                        chunkBarValue2 = new Rectangle(0, height - fillHeightValue2, this.width, fillHeightValue2);      
                   
                        break;
                    }
            }

			this.progressRect = chunkbar;
            if (this.HasImage)
            {
                return;
            }

			switch (GradientStyle)
			{
				case GradientStyles.Solid:
					g.FillRectangle(chunkBarValue2, Color.FromArgb(25, BackColor));
                    g.FillRectangle(chunkbar, BackColor);
					break;
				case GradientStyles.Glass:
					g.FillRectangle(chunkBarValue2, Color.FromArgb(25, BackColor));
                    g.FillGlassRectangle(chunkbar, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2);
					break;
				case GradientStyles.OfficeGlass:
					g.FillRectangle(chunkBarValue2, Color.FromArgb(25, BackColor));
					g.FillOfficeGlassRectangle(chunkbar, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2,true );
					break;
                case GradientStyles.OfficeGlassRect:
                    g.FillRectangle(chunkBarValue2, Color.FromArgb(25, BackColor));
                    g.FillOfficeGlassRectangle(chunkbar, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2, false);
                    break;
                case GradientStyles.Vista:
					g.FillRectangle(chunkBarValue2, Color.FromArgb(25, BackColor));
                    g.FillVistaRectangle(chunkbar, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2);
					break;
				case GradientStyles.Gel:
					g.FillRectangle(chunkBarValue2, Color.FromArgb(25, BackColor));
                    g.FillGellRectangle(chunkbar, colorStops, this.GradientPercentage, this.GradientPercentage2);
					break;
				case GradientStyles.Radial:
				case GradientStyles.Linear:
					g.FillRectangle(chunkBarValue2, Color.FromArgb(25, BackColor));
                    g.FillGradientRectangle(chunkbar, colorStops, colorOffsets, base.GradientStyle, GradientAngle, this.GradientPercentage, this.GradientPercentage2);
                    break;
			}
			if (Dash)
			{
				DrawLines(g, fillWidth, fillHeight, fillWidthValue2, fillHeightValue2);
			}
        }

		public Rectangle GetProgressRectangle()
		{
			return progressRect;
		}

        private void DrawLines(IGraphics g, int fillWidth, int fillHeight, int fillWidthValue2, int fillHeightValue2)
        {
            int max = Math.Max(this.width, this.height);
            double x1, x2, y1, y2;
            Rectangle rec = new Rectangle(fillWidth, 0, this.width, this.height);
            if (width == 0 || height == 0)//found empty Rectangle
            {
                return;
            }
            LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(0, 0, width, height), SeparatorColor1, SeparatorColor2, LinearGradientMode.Vertical);
            g.ChangeSmoothingMode(SmoothingMode.AntiAlias);    
            x1 = x2 = y1 = y2 = 0;   
            sweepAngle = SweepAngle;
            int sepCount = max / StepWidth;
            max = sepCount * 2;
            switch (Orientation)
            {
                case ProgressOrientation.Right:
                    {
                        rec = new Rectangle(0, 0, this.width - fillWidth, this.height);
                        break;
                    }
                case ProgressOrientation.Bottom:
                    {
                        rec = new Rectangle(0, 0, this.width, this.height - fillHeight);
                        break;
                    }
                case ProgressOrientation.Top:
                    {
                        rec = new Rectangle(0, fillHeight, this.width, this.height - fillHeight);
                        break;
                    }
            }
			Region backup = (g.UnderlayGraphics as Graphics).Clip;
			g.ExcludeClip(rec);
            int k = sweepAngle / 180;
            if (sweepAngle < 90 + k * 180)
            {
                x1 = (double)(StepWidth / Math.Cos(sweepAngle * Math.PI / 180));
                x2 = x1 + (double)(this.height * Math.Tan(sweepAngle * Math.PI / 180));
                y1 = 0;
                y2 = this.height;
                for (int i = -max; i <= max; i++)
                {

                    x2 = x1 * i + (double)(this.height * Math.Tan(sweepAngle * Math.PI / 180));
                    double p1 = x1 * i + (double)(SeparatorWidth / Math.Cos(sweepAngle * Math.PI / 180));
                    double p2 = p1 + (double)(this.height * Math.Tan(sweepAngle * Math.PI / 180));

                    g.FillPolygon(lgb, new PointF[] { new PointF((float)( x1 * i), (float)y1), new PointF((float)( x2), 
                            (float)y2), new PointF((float)( p2), (float)y2), new PointF((float)( p1), (float)y1) });

                    if (Hatch)
                        g.FillPolygon(lgb, new PointF[] { new PointF((float)( x1 * i),
                            (float)y2), new PointF((float)( x2), (float)y1), new PointF((float)( p2), (float)y1), new PointF((float)( p1), (float)y2) });

                }
            }
            else
            {
                x1 = 0;
                x2 = (int)this.width;
                if ((Orientation == ProgressOrientation.Bottom) || (Orientation == ProgressOrientation.Top))
                    x2 = (int)this.width;
				
                y2 = y1 + (int)(this.width / Math.Tan(sweepAngle * Math.PI / 180));
               
                double p1 = 0;
                double p2 = 0;

                for (int i = -max; i <= max; i++)
                {
                    y1 = (int)(i * StepWidth / Math.Sin(sweepAngle * Math.PI / 180));
                    p1 = y1 + (int)(SeparatorWidth / Math.Sin(sweepAngle * Math.PI / 180));
                    p2 = p1 + (int)(this.width / Math.Tan(sweepAngle * Math.PI / 180));
                    y2 = y1 + (int)(this.width / Math.Tan(sweepAngle * Math.PI / 180));

                    g.FillPolygon(lgb, new PointF[] { new PointF((float)( x1), 
                            (float)y1), new PointF((float)( x2), (float)y2), new PointF((float)( x2), (float)p2), new PointF((float)( x1), (float)p1) });

                    if (Hatch)
                        g.FillPolygon(lgb, new PointF[] { new PointF((float)( x1),
                            (float)y2), new PointF((float)( x2), (float)y1), new PointF((float)( x2), (float)p1), new PointF((float)( x1), (float)p2) });

                }
            }
			lgb.Dispose();
			g.RestoreSmoothingMode();
			(g.UnderlayGraphics as Graphics).Clip = backup;	
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF final = base.ArrangeOverride(finalSize);

            if (this.Children.Count < 2)
                return final;

            BorderPrimitive borderPrimitive = this.Children[1] as BorderPrimitive;
            if (borderPrimitive == null)
                return final;


            int divider = 1;
            if (Minimum != 0)
            {
                divider = Maximum - Minimum;
            }

            float fillWidth = (finalSize.Width * Value1) / divider;
            float fillHeight = (finalSize.Height * Value1) / divider;

            if (Orientation == ProgressOrientation.Left)
            {
                borderPrimitive.Arrange(new RectangleF(1, 0, fillWidth + 1, final.Height - 1));
            }

            return final;
        }
    }
}
