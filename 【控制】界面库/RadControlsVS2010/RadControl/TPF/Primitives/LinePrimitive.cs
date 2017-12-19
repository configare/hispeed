using System;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls;

namespace Telerik.WinControls.Primitives
{
	/// <summary>Represents a line that is drawn on the screen.</summary>
	public class LinePrimitive : FillPrimitive
	{
		public static RadProperty SweepAngleProperty = RadProperty.Register(
			"SweepAngle", typeof(int), typeof(LinePrimitive), new RadElementPropertyMetadata(
				0, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty LineWidthProperty = RadProperty.Register(
			"LineWidth", typeof(int), typeof(LinePrimitive), new RadElementPropertyMetadata(
				1, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty OrientationProperty = RadProperty.Register(
			"SeparatorOrientation", typeof(SepOrientation), typeof(LinePrimitive), new RadElementPropertyMetadata(
				SepOrientation.Horizontal, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets the line width in pixels.</summary>
		public virtual int LineWidth
		{
			get
			{
				return (int)GetValue(LineWidthProperty);
			}
			set
			{
				SetValue(LineWidthProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the line orientation. Possible values are defined in the SepOrientation
		/// enumeration.
		/// </summary>
		public virtual SepOrientation SeparatorOrientation
		{
			get
			{
				return (SepOrientation)GetValue(OrientationProperty);
			}
			set
			{
				SetValue(OrientationProperty, value);
			}
		}

		/// <summary>Gets or sets the line angle in degrees.</summary>
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

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.FitToSizeMode = RadFitToSizeMode.FitToParentPadding;
        }

		/// <summary>Draws the primitive on the screen.</summary>
        public override void PaintPrimitive(IGraphics g, float angle, SizeF scale)
		{
			switch (SeparatorOrientation)
			{
				case SepOrientation.Vertical:
					{
						SweepAngle = 90;
						break;
					}
				case SepOrientation.Horizontal:
					{
						SweepAngle = 0;
						break;
					}
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
				colorStops[colorStops.Length - 1] = BackColor2;
				colorOffsets[colorStops.Length - 1] = 1f;
			}

			if (this.NumberOfColors > 2)
			{
                colorStops[1] = BackColor2;
                colorOffsets[1] = this.GradientPercentage;

                colorStops[colorStops.Length - 1] = BackColor3;
                colorOffsets[colorStops.Length - 1] = 1f;
			}

			if (this.NumberOfColors > 3)
			{
                colorStops[2] = BackColor3;
                colorOffsets[2] = this.GradientPercentage2;

                colorStops[colorStops.Length - 1] = BackColor4;
                colorOffsets[colorStops.Length - 1] = 1f;
			}

            Rectangle fillRect = new Rectangle(Point.Empty, this.Size);
            if (this.SeparatorOrientation == SepOrientation.Vertical)
            {
                fillRect.Width = this.LineWidth;
            }
            else
            {
                fillRect.Height = this.LineWidth;
            }

			switch (GradientStyle)
			{
				case GradientStyles.Solid:

					g.FillRectangle(fillRect, BackColor);
	            
					break;

				case GradientStyles.Glass:
                    g.FillGlassRectangle(fillRect, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2);
					break;

				case GradientStyles.OfficeGlass:
					g.FillOfficeGlassRectangle(fillRect, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2, true);
					break;

                case GradientStyles.OfficeGlassRect:
                    g.FillOfficeGlassRectangle(fillRect, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2, false);
                    break;
                
                case GradientStyles.Vista:
					g.FillVistaRectangle(fillRect, BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage, this.GradientPercentage2);
					break;

				case GradientStyles.Gel:
                    g.FillGellRectangle(fillRect, colorStops, this.GradientPercentage, this.GradientPercentage2);
					break;

				case GradientStyles.Radial:
				case GradientStyles.Linear:
					{ 
						if (NumberOfColors < 2)
						{
							g.FillRectangle(fillRect, BackColor);
						}
						else
						{
							g.FillGradientRectangle(fillRect, colorStops, colorOffsets, GradientStyle, GradientAngle, this.GradientPercentage, this.GradientPercentage2);
	                    
						}
	                                      
						break;
					}
			}
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);
            if (SeparatorOrientation == SepOrientation.Horizontal)
                return new SizeF(0f, 1f);
            else
                return new SizeF(1f, 0f);
        }
	}
}