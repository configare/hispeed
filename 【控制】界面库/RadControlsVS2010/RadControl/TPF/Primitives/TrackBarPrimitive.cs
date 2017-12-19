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
using Telerik.WinControls;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Enumerations;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.Primitives
{
	/// <summary>
	/// 	<para>Represents a track bar that is drawn on the screen.</para>
	/// 	<para>
	///         Extends
	///         <see cref="Telerik.WinControls.Primitives.BasePrimitive">BasePrimitive</see>
	/// 	</para>
	/// </summary>
	public class TrackBarPrimitive : FillPrimitive
	{

		public static RadProperty TickStyleProperty = RadProperty.Register("TickStyle",
				typeof(TickStyles), typeof(TrackBarPrimitive), new RadElementPropertyMetadata(TickStyles.Both,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ThumbWidthProperty = RadProperty.Register("ThumbWidth", typeof(int),
	typeof(TrackBarPrimitive),
	new RadElementPropertyMetadata(12, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty MinimumProperty = RadProperty.Register("Minimum", typeof(int),
	  typeof(TrackBarPrimitive),
	  new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty MaximumProperty = RadProperty.Register("Maximum", typeof(int),
		 typeof(TrackBarPrimitive),
		 new RadElementPropertyMetadata(10, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty SlideAreaWidthProperty = RadProperty.Register("SlideAreaWidth", typeof(int),
		 typeof(TrackBarPrimitive),
		 new RadElementPropertyMetadata(3, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty TrackBarOrientationProperty = RadProperty.Register("TrackBarOrientation", typeof(Orientation),
		typeof(TrackBarPrimitive),
		new RadElementPropertyMetadata(Orientation.Horizontal, ElementPropertyOptions.AffectsDisplay));


		public static RadProperty TickFrequencyProperty = RadProperty.Register("TickFrequency", typeof(int),
		 typeof(TrackBarPrimitive),
		new RadElementPropertyMetadata(1, ElementPropertyOptions.AffectsDisplay));

		[Description("The fifth color component when when gradient style is other than solid and number of colors property is greater than 2")]
		public static RadProperty BackColor5Property = RadProperty.Register(
			"BackColor5", typeof(Color), typeof(TrackBarPrimitive), new RadElementPropertyMetadata(
				Color.White, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ShowTicksProperty = RadProperty.Register("ShowTicks", typeof(bool),
		 typeof(TrackBarPrimitive),
		new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ShowSlideAreaProperty = RadProperty.Register("ShowSlideArea", typeof(bool),
		 typeof(TrackBarPrimitive),
		new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty FitToAvailableSizeProperty = RadProperty.Register("FitToAvailableSize", typeof(bool),
		typeof(TrackBarPrimitive),
		new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		[Description("The sixth color component when when gradient style is other than solid and number of colors property is greater than 2")]
		public static RadProperty BackColor6Property = RadProperty.Register(
			"BackColor6", typeof(Color), typeof(TrackBarPrimitive), new RadElementPropertyMetadata(
				Color.Blue, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty SliderAreaGradientAngleProperty = RadProperty.Register("SliderAreaGradientAngle", typeof(float),
		typeof(TrackBarPrimitive),
		new RadElementPropertyMetadata(0f, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty TickColorProperty = RadProperty.Register("TickColor", typeof(Color),
		typeof(TrackBarPrimitive),
		new RadElementPropertyMetadata(Color.White, ElementPropertyOptions.AffectsDisplay));


		/// <summary> Gets or Sets RadTrackBar's ticks color </summary>
		public virtual Color TickColor
		{
			get
			{
				return (Color)this.GetValue(TickColorProperty);
			}
			set
			{
				this.SetValue(TickColorProperty, value);
			}
		}

		/// <summary> Gets or Sets the gradient angle of the SliderArea </summary>
		public virtual float SliderAreaGradientAngle
		{
			get
			{
				return (float)this.GetValue(SliderAreaGradientAngleProperty);
			}
			set
			{
				this.SetValue(SliderAreaGradientAngleProperty, value);
			}
		}

		/// <summary> Gets or Sets whether the TrackBar should fit to available size </summary>
		public virtual bool FitToAvailableSize
		{
			get
			{
				return (bool)this.GetValue(FitToAvailableSizeProperty);
			}
			set
			{
				this.SetValue(FitToAvailableSizeProperty, value);
			}
		}

		/// <summary> Gets or Sets whether the SlideArea should be visible </summary>
		public virtual bool ShowSlideArea
		{
			get
			{
				return (bool)this.GetValue(ShowSlideAreaProperty);
			}
			set
			{
				this.SetValue(ShowSlideAreaProperty, value);
			}
		}

		/// <summary> Gets or Sets Ticks Visibility </summary>
		public virtual bool ShowTicks
		{
			get
			{
				return (bool)this.GetValue(ShowTicksProperty);
			}
			set
			{
				this.SetValue(ShowTicksProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets background color. This property is applicable to radial, glass,
		/// office glass, and vista gradients.
		/// </summary>
		[RadPropertyDefaultValue("BackColor5", typeof(TrackBarPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
		[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RadColorEditorConverter))]
		public virtual Color BackColor5
		{
			get
			{
				return (Color)this.GetValue(BackColor5Property);
			}
			set
			{
				this.SetValue(BackColor5Property, value);
			}
		}


		/// <summary>
		/// Gets or sets background color. This property is applicable to radial, glass,
		/// office glass, and vista gradients.
		/// </summary>
		[RadPropertyDefaultValue("BackColor6", typeof(TrackBarPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
		[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RadColorEditorConverter))]
		public virtual Color BackColor6
		{
			get
			{
				return (Color)this.GetValue(BackColor6Property);
			}
			set
			{
				this.SetValue(BackColor6Property, value);
			}
		}

		/// <summary>
		/// Gets or Sets TrackBar's thumbWidth
		/// </summary>
		public virtual int ThumbWidth
		{
			get
			{
				return (int)this.GetValue(ThumbWidthProperty);
			}
			set
			{
				this.SetValue(ThumbWidthProperty, value);
			}
		}

		/// <summary>
		/// Gets or Sets TrackBar's Orientation
		/// </summary>
		public virtual Orientation TrackBarOrientation
		{
			get
			{
				return (Orientation)this.GetValue(TrackBarOrientationProperty);
			}
			set
			{
				this.SetValue(TrackBarOrientationProperty, value);
			}
		}

		/// <summary>
		/// Indicates the tick style of the progress bar. Possible values are members of
		/// %TickStyles enumeration:Telerik.WinControls.Enumerations.TickStyles%: none, 
		/// topleft, BottomRight, and both.
		/// </summary>
		public virtual TickStyles TickStyle
		{
			get
			{
				return (TickStyles)this.GetValue(TickStyleProperty);
			}
			set
			{
				this.SetValue(TickStyleProperty, value);
			}
		}

		/// <summary>
		/// The number of positions between tick marks
		/// </summary>
		public virtual int TickFrequency
		{
			get
			{
				return (int)GetValue(TickFrequencyProperty);
			}
			set
			{
				if (value <= 0) value = 1;
				SetValue(TickFrequencyProperty, value);
			}
		}

		/// <summary>
		/// Gets or Sets the width of TrackBar's SlideArea
		/// </summary>
		public virtual int SlideAreaWidth
		{
			get
			{
				return (int)GetValue(SlideAreaWidthProperty);
			}
			set
			{
				SetValue(SlideAreaWidthProperty, value);
			}
		}

		/// <summary>Gets or sets a minimum int value for the trackbar position.</summary>
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

			}
		}

		/// <summary>Gets or sets a maximum int value for the trackbar position.</summary>
		public virtual int Maximum
		{
			get
			{
				return (int)GetValue(MaximumProperty);
			}
			set
			{
				SetValue(MaximumProperty, value);

				if (Maximum < Minimum)
					Minimum = Maximum;
			}
		}

		private void DrawHorizontalTrackFill(IGraphics graphics, float angle, SizeF scale)
		{
			int maxTickNumber = (this.Maximum - this.Minimum);
			if (maxTickNumber == 0) maxTickNumber = 1;
			int thumbHeight = this.Bounds.Height / 2;

			double x = this.ThumbWidth / 2;

			switch (this.TickStyle)
			{
				case TickStyles.Both:
				case TickStyles.None:
					{

						if (this.ShowSlideArea)
							graphics.FillGradientRectangle(new Rectangle(0, this.Bounds.Height / 2 - this.SlideAreaWidth / 2, this.Size.Width, this.SlideAreaWidth),
				new Color[] { this.BackColor5, this.BackColor6 }, new float[] { 0f, 1f }, GradientStyles.Linear, this.SliderAreaGradientAngle, 0, 0);

						if ((this.ShowTicks) && (!(this.FitToAvailableSize)))
						{
							// first Line
							graphics.DrawLine(this.TickColor, (float)x, 0, (float)x, thumbHeight / 2 - 1);
							graphics.DrawLine(this.TickColor, (float)x, this.Bounds.Bottom - thumbHeight / 2, (float)x, this.Bounds.Bottom);

							// last Line

							graphics.DrawLine(this.TickColor, this.Bounds.Width - (float)x, 0, this.Bounds.Width - (float)x, thumbHeight / 2 - 1);
							graphics.DrawLine(this.TickColor, this.Bounds.Width - (float)x, this.Bounds.Bottom - thumbHeight / 2, this.Bounds.Width - (float)x, this.Bounds.Bottom);

							for (int i = 0; i <= maxTickNumber; i++)
							{
								if (i % TickFrequency == 0)
								{
									int valueToDiv = this.Bounds.Width - this.ThumbWidth;
									x = (this.ThumbWidth / 2) + (i * valueToDiv / (maxTickNumber));

									// draw top ticks
									graphics.DrawLine(this.TickColor, (float)x, 0, (float)x, thumbHeight / 2 - 1);
									// draw bottom ticks
									graphics.DrawLine(this.TickColor, (float)x, this.Bounds.Bottom - thumbHeight / 2, (float)x, this.Bounds.Bottom);
								}
							}
						}
						break;
					}

				case TickStyles.BottomRight:
					{
						if (this.ShowSlideArea)
							graphics.FillGradientRectangle(new Rectangle(0, this.Bounds.Height / 4 - this.SlideAreaWidth / 2, this.Size.Width, this.SlideAreaWidth),
						new Color[] { this.BackColor5, this.BackColor6 }, new float[] { 0f, 1f }, GradientStyles.Linear, this.SliderAreaGradientAngle, 0, 0);

						if ((this.ShowTicks) && (!(this.FitToAvailableSize)))
						{
							// first Line
							graphics.DrawLine(this.TickColor, (float)x, this.Bounds.Bottom - thumbHeight + 1, (float)x, this.Bounds.Bottom);

							// last Line

							graphics.DrawLine(this.TickColor, this.Bounds.Width - (float)x, this.Bounds.Bottom - thumbHeight + 1, this.Bounds.Width - (float)x, this.Bounds.Bottom);

							for (int i = 0; i <= maxTickNumber; i++)
							{
								if (i % TickFrequency == 0)
								{
									int valueToDiv = this.Bounds.Width - this.ThumbWidth;
									x = (this.ThumbWidth / 2) + (i * valueToDiv / (maxTickNumber));

									graphics.DrawLine(this.TickColor, (float)x, this.Bounds.Bottom - thumbHeight + 1, (float)x, this.Bounds.Bottom);
								}
							}
						}
						break;
					}

				case TickStyles.TopLeft:
					{
						if (this.ShowSlideArea)
							graphics.FillGradientRectangle(new Rectangle(0, 3 * this.Bounds.Height / 4 - this.SlideAreaWidth / 2 - 1, this.Size.Width, this.SlideAreaWidth),
								new Color[] { this.BackColor5, this.BackColor6 }, new float[] { 0f, 1f }, GradientStyles.Linear, this.SliderAreaGradientAngle, 0, 0);

						if ((this.ShowTicks) && (!(this.FitToAvailableSize)))
						{
							// first Line
							graphics.DrawLine(this.TickColor, (float)x, 0, (float)x, thumbHeight - 1);

							// last Line

							graphics.DrawLine(this.TickColor, this.Bounds.Width - (float)x, 0, this.Bounds.Width - (float)x, thumbHeight - 1);

							for (int i = 0; i <= maxTickNumber; i++)
							{
								if (i % TickFrequency == 0)
								{
									int valueToDiv = this.Bounds.Width - this.ThumbWidth;
									x = (this.ThumbWidth / 2) + (i * valueToDiv / (maxTickNumber));

									// draw top ticks
									graphics.DrawLine(this.TickColor, (float)x, 0, (float)x, thumbHeight - 1);
								}
							}
						}
						break;
					}
			}
		}

		private void DrawVerticalTrackFill(IGraphics graphics, float angle, SizeF scale)
		{
			int maxTickNumber = (this.Maximum - this.Minimum);

			int thumbHeight = this.Bounds.Width / 2;

			double x = this.ThumbWidth / 2;

			switch (this.TickStyle)
			{
				case TickStyles.Both:
				case TickStyles.None:
					{

						if (this.ShowSlideArea)
							graphics.FillGradientRectangle(new Rectangle(this.Bounds.Width / 2 - this.SlideAreaWidth / 2, 0, this.SlideAreaWidth, this.Size.Height),
								new Color[] { this.BackColor5, this.BackColor6 }, new float[] { 0f, 1f }, GradientStyles.Linear, this.SliderAreaGradientAngle, 0, 0);

						if ((this.ShowTicks) && (!(this.FitToAvailableSize)))
						{
							// first Line
							graphics.DrawLine(this.TickColor, 0, (float)x, thumbHeight / 2 - 1, (float)x);
							graphics.DrawLine(this.TickColor, this.Bounds.Right - thumbHeight / 2, (float)x, this.Bounds.Right, (float)x);

							// last Line

							graphics.DrawLine(this.TickColor, 0, this.Bounds.Height - (float)x, thumbHeight / 2 - 1, this.Bounds.Height - (float)x);
							graphics.DrawLine(this.TickColor, this.Bounds.Right - thumbHeight / 2, this.Bounds.Height - (float)x, this.Bounds.Right - thumbHeight / 2, this.Bounds.Height - (float)x);

							for (int i = 0; i <= maxTickNumber; i++)
							{
								if (i % TickFrequency == 0)
								{
									int valueToDiv = this.Bounds.Height - this.ThumbWidth;
									x = (this.ThumbWidth / 2) + (i * valueToDiv / (maxTickNumber));

									// draw top ticks
									graphics.DrawLine(this.TickColor, 0, (float)x, thumbHeight / 2 - 1, (float)(x));
									// draw bottom ticks
									graphics.DrawLine(this.TickColor, this.Bounds.Right - thumbHeight / 2, (float)x, this.Bounds.Right, (float)x);
								}
							}
						}
						break;
					}

				case TickStyles.BottomRight:
					{
						if (this.ShowSlideArea)
							graphics.FillGradientRectangle(new Rectangle(this.Bounds.Width / 4 - this.SlideAreaWidth / 2, 0, this.SlideAreaWidth, this.Size.Height),
								new Color[] { this.BackColor5, this.BackColor6 }, new float[] { 0f, 1f }, GradientStyles.Linear, this.SliderAreaGradientAngle, 0, 0);

						if ((this.ShowTicks) && (!(this.FitToAvailableSize)))
						{
							// first Line
							graphics.DrawLine(this.TickColor, this.Bounds.Right - thumbHeight - 1, (float)x, this.Bounds.Right, (float)x);

							// last Line

							graphics.DrawLine(this.TickColor, this.Bounds.Right - thumbHeight - 1, this.Bounds.Height - (float)x, this.Bounds.Right, this.Bounds.Height - (float)x);

							for (int i = 0; i <= maxTickNumber; i++)
							{
								if (i % TickFrequency == 0)
								{
									int valueToDiv = this.Bounds.Height - this.ThumbWidth;
									x = (this.ThumbWidth / 2) + (i * valueToDiv / (maxTickNumber));

									// draw left ticks
									graphics.DrawLine(this.TickColor, this.Bounds.Right - thumbHeight - 1, (float)x, this.Bounds.Right, (float)x);
								}
							}
						}
						break;
					}

				case TickStyles.TopLeft:
					{
						if (this.ShowSlideArea)
							graphics.FillGradientRectangle(new Rectangle(3 * this.Bounds.Width / 4 - this.SlideAreaWidth / 2, 0, this.SlideAreaWidth, this.Size.Height),
								new Color[] { this.BackColor5, this.BackColor6 }, new float[] { 0f, 1f }, GradientStyles.Linear, this.SliderAreaGradientAngle, 0, 0);

						if ((this.ShowTicks) && (!(this.FitToAvailableSize)))
						{
							// first Line
							graphics.DrawLine(this.TickColor, 0, (float)x, thumbHeight - 1, (float)x);

							// last Line

							graphics.DrawLine(this.TickColor, 0, this.Bounds.Height - (float)x, thumbHeight - 1, this.Bounds.Height - (float)x);

							for (int i = 0; i <= maxTickNumber; i++)
							{
								if (i % TickFrequency == 0)
								{
									int valueToDiv = this.Bounds.Height - this.ThumbWidth;
									x = (this.ThumbWidth / 2) + (i * valueToDiv / (maxTickNumber));

									// draw left ticks
									graphics.DrawLine(this.TickColor, 0, (float)x, thumbHeight - 1, (float)(x));
								}
							}
						}
						break;
					}
			}
		}

		protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
		{
			base.PaintElement(graphics, angle, scale);
			if (this.TrackBarOrientation == Orientation.Horizontal)
			{
				this.DrawHorizontalTrackFill(graphics, angle, scale);
			}
			else
			{
				this.DrawVerticalTrackFill(graphics, angle, scale);
			}
		}
	}
}
