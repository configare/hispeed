using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Design;
namespace Telerik.WinControls.UI
{   //TODO
	/// <summary>Represents a track bar item.</summary>
	public class RadTrackBarItem : RadItem
	{
		public static RadProperty TickStyleProperty = RadProperty.Register(
			"TickStyle", typeof(TickStyles), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				TickStyles.Both, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty MinimumProperty = RadProperty.Register(
			"Minimum", typeof(int), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				0, ElementPropertyOptions.AffectsDisplay ));

		public static RadProperty MaximumProperty = RadProperty.Register(
			"Maximum", typeof(int), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				20, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

		public static RadProperty LargeChangeProperty = RadProperty.Register(
			"LargeChange", typeof(int), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				1, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty SmallChangeProperty = RadProperty.Register(
			"SmallChange", typeof(int), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				1, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ShowTicksProperty = RadProperty.Register(
			"ShowTicks", typeof(bool), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				true, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty FitToAvailableSizeProperty = RadProperty.Register(
			"FitToAvailableSize", typeof(bool), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

		public static RadProperty ShowSlideAreaProperty = RadProperty.Register(
			"ShowSlideArea", typeof(bool), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				true, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ValueProperty = RadProperty.Register(
			"Value", typeof(int), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				0, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty SlideAreaWidthProperty = RadProperty.Register(
			"SlideAreaWidth", typeof(int), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				3, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty TrackBarOrientationProperty = RadProperty.Register(
			"TrackBarOrientation", typeof(Orientation), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				Orientation.Horizontal, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty TickFrequencyProperty = RadProperty.Register(
			"TickFrequency", typeof(int), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				1, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty SliderAreaGradientColor1Property = RadProperty.Register(
			"SliderAreaGradientColor1", typeof(Color), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				Color.White, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty TickColorProperty = RadProperty.Register(
			"TickColor", typeof(Color), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				Color.White, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty SliderAreaGradientColor2Property = RadProperty.Register(
			"SliderAreaGradientColor2", typeof(Color), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				Color.Black, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty SliderAreaGradientAngleProperty = RadProperty.Register(
			"SliderAreaGradientAngle", typeof(float), typeof(RadTrackBarItem), new RadElementPropertyMetadata(
				0f, ElementPropertyOptions.AffectsDisplay));

		/// <summary>
		/// Gets or sets SliderArea's first background color.
		/// </summary>
		[Description("Gets or sets SliderArea's first background color.")]
        [RadPropertyDefaultValue("SliderAreaGradientColor1", typeof(RadTrackBarItem))]
        public virtual Color SliderAreaGradientColor1
		{
			get
			{
				return (Color)this.GetValue(SliderAreaGradientColor1Property);
			}
			set
			{
				this.SetValue(SliderAreaGradientColor1Property, value);
			}
		}

		/// <summary>
		/// Gets or sets SliderArea's second background color.
		/// </summary>
		[Description("Gets or sets SliderArea's second background color.")]
        [RadPropertyDefaultValue("SliderAreaGradientColor2", typeof(RadTrackBarItem))]
        public virtual Color SliderAreaGradientColor2
		{
			get
			{
				return (Color)this.GetValue(SliderAreaGradientColor2Property);
			}
			set
			{
				this.SetValue(SliderAreaGradientColor2Property, value);
			}
		}

		/// <summary>Gets or sets RadTrackBar's ticks color.</summary>
		[Description("Gets or sets RadTrackBar's ticks color")]
        [RadPropertyDefaultValue("TickColor", typeof(RadTrackBarItem))]
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

		/// <summary>Gets or sets the gradient angle of the SliderArea.</summary>
		[Description("Gets or sets the gradient angle of the SliderArea.")]
        [RadPropertyDefaultValue("SliderAreaGradientAngle", typeof(RadTrackBarItem))]
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

		/// <summary>Gets or sets whether the TrackBar should fit to available size.</summary>
		[Description("Gets or sets whether the TrackBar should fit to available size")]
        [RadPropertyDefaultValue("FitToAvailableSize", typeof(RadTrackBarItem))]
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

		/// <summary>Gets or sets whether the SlideArea should be visible.</summary>
		[Description("Gets or sets whether the SlideArea should be visible.")]
        [RadPropertyDefaultValue("ShowSlideArea", typeof(RadTrackBarItem))]
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

		/// <summary>Gets or sets Ticks Visibility.</summary>
		[Description("Gets or sets Ticks Visibility")]
        [RadPropertyDefaultValue("ShowTicks", typeof(RadTrackBarItem))]
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
		/// The number of positions the slider moves in response to mouse clicks.
		/// </summary>
		[Description("The number of positions the slider moves in response to mouse clicks.")]
        [RadPropertyDefaultValue("LargeChange", typeof(RadTrackBarItem))]
        public virtual int LargeChange
		{
			get
			{
				return (int)this.GetValue(LargeChangeProperty);
			}
			set
			{
				this.SetValue(LargeChangeProperty, value);
			}
		}

        /// <summary>
        /// The number of positions the slider moves in response to mouse clicks.
        /// </summary>
        [Description("The number of positions the slider moves in response to a mouse click.")]
        [RadPropertyDefaultValue("SmallChange", typeof(RadTrackBarItem))]
        public virtual int SmallChange
		{
			get
			{
				return (int)this.GetValue(SmallChangeProperty);
			}
			set
			{
				this.SetValue(SmallChangeProperty, value);
			}
		}

		/// <summary>
		/// The number of positions between tick marks.
		/// </summary>
		[Description("The number of positions between tick marks.")]
        [RadPropertyDefaultValue("TickFrequency", typeof(RadTrackBarItem))]
        public virtual int TickFrequency
		{
			get
			{
				return (int)this.GetValue(TickFrequencyProperty);
			}
			set
			{
				this.SetValue(TickFrequencyProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets TrackBar's orientation.
		/// </summary>

		[Description("Gets or sets TrackBar's orientation.")]
        [RadPropertyDefaultValue("TrackBarOrientation", typeof(RadTrackBarItem))]
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
		/// Gets or sets the width of TrackBar's SlideArea.
		/// </summary>
		[Description("Gets or sets the width of TrackBar's SlideArea.")]
        [RadPropertyDefaultValue("SlideAreaWidth", typeof(RadTrackBarItem))]
        public virtual int SlideAreaWidth
		{
			get
			{
				return (int)this.GetValue(SlideAreaWidthProperty);
			}
			set
			{
				this.SetValue(SlideAreaWidthProperty, value);
			}
		}
		/// <summary>
		/// Indicates the tick style of the progress bar. Possible values are members of
		/// %TickStyles enumeration:Telerik.WinControls.Enumerations.TickStyles%: none, 
		/// topleft, BottomRight, and both.
		/// </summary>
		[Description("Indicates the tick style of the trackBar")]
        [RadPropertyDefaultValue("TickStyle", typeof(RadTrackBarItem))]
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

		/// <summary>Gets or sets a minimum int value for the trackbar position.</summary>
		[Description("Gets or sets a minimum int value for the trackbar position.")]
        [RadPropertyDefaultValue("Minimum", typeof(RadTrackBarItem))]
        public virtual int Minimum
		{
			get
			{
				return (int)GetValue(MinimumProperty);
			}
			set
			{

				if (value < 0) value = 0;
				if (value > Maximum)
					value = Maximum;

				SetValue(MinimumProperty, value);
	
			}
		}

		/// <summary>Gets or sets a maximum int value for the trackbar position.</summary>
		[Description("Gets or sets a maximum int value for the trackbar position.")]
        [RadPropertyDefaultValue("Maximum", typeof(RadTrackBarItem))]
        public virtual int Maximum
		{
			get
			{
				return (int)GetValue(MaximumProperty);
			}
			set
			{
				SetValue(MaximumProperty, value);

				if (Maximum < 0) Maximum = 0;
				if (Maximum < Minimum)
					Minimum = Maximum;
			}
		}

		/// <summary>
		/// Gets or sets the position of the Slider.
		/// </summary>
		[Description("Gets or sets the position of the Slider")]
        [RadPropertyDefaultValue("Value", typeof(RadTrackBarItem))]
        public virtual int Value
		{
			get
			{
				return (int)GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);

				if (Value < Minimum)
					Value = Minimum;
				if (Value > Maximum)
					Value = Maximum;
			}
		}

	}
}