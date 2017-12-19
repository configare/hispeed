using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
	public class RadLineItem : RadItem
	{
		public enum LineDrawingStyle
		{
			Flat,
			Bevel,
			Emboss
		};
		
		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
        }

		public static RadProperty SweepAngleProperty = RadProperty.Register(
			"SweepAngle", typeof(int), typeof(RadLineItem), new RadElementPropertyMetadata(
				0, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty LineWidthProperty = RadProperty.Register(
			"LineWidth", typeof(int), typeof(RadLineItem), new RadElementPropertyMetadata(
				2, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty OrientationProperty = RadProperty.Register(
			"Orientation", typeof(SepOrientation), typeof(RadLineItem), new RadElementPropertyMetadata(
				SepOrientation.Horizontal, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty LineColorProperty = RadProperty.Register(
			"LineColor", typeof(Color), typeof(RadLineItem), new RadElementPropertyMetadata(
				Color.Black, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty LineColor2Property = RadProperty.Register(
			"LineColor2", typeof(Color), typeof(RadLineItem), new RadElementPropertyMetadata(
				Color.White, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty LineStyleProperty = RadProperty.Register(
			"LineStyle", typeof(LineDrawingStyle), typeof(RadLineItem), new RadElementPropertyMetadata(
				LineDrawingStyle.Bevel, ElementPropertyOptions.AffectsDisplay));

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

		[RadPropertyDefaultValue("LineColor", typeof(RadLineItem))]
		public virtual Color LineColor
		{
			get
			{
				return (Color)GetValue(LineColorProperty);
			}
			set
			{
				SetValue(LineColorProperty, value);
			}
		}

		[RadPropertyDefaultValue("LineColor2", typeof(RadLineItem))]
		public virtual Color LineColor2
		{
			get
			{
				return (Color)GetValue(LineColor2Property);
			}
			set
			{
				SetValue(LineColor2Property, value);
			}
		}

		[RadPropertyDefaultValue("LineStyle", typeof(RadLineItem))]
		public virtual LineDrawingStyle LineStyle
		{
			get
			{
				return (LineDrawingStyle) GetValue(LineStyleProperty);
			}
			set
			{
				SetValue(LineStyleProperty, value);
			}
		}

		private LinePrimitive linePrimitive;

		protected override void CreateChildElements()
		{
			this.linePrimitive = new LinePrimitive();
			this.linePrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.linePrimitive.BackColor = Color.Black;
			this.linePrimitive.BackColor2 = Color.White;
			this.linePrimitive.BackColor3 = Color.White;
			this.linePrimitive.NumberOfColors = 3;
			this.linePrimitive.GradientStyle = GradientStyles.Linear;

			this.linePrimitive.BindProperty(LinePrimitive.BackColorProperty, this,
				RadLineItem.LineColorProperty, PropertyBindingOptions.OneWay);
			this.linePrimitive.BindProperty(LinePrimitive.BackColor2Property, this,
				RadLineItem.LineColor2Property, PropertyBindingOptions.OneWay);
			this.linePrimitive.BindProperty(LinePrimitive.BackColor3Property, this,
				RadLineItem.LineColor2Property, PropertyBindingOptions.OneWay);

			this.linePrimitive.BindProperty(LinePrimitive.LineWidthProperty, this,
				RadLineItem.LineWidthProperty, PropertyBindingOptions.OneWay);
			this.linePrimitive.BindProperty(LinePrimitive.SweepAngleProperty, this,
				RadLineItem.SweepAngleProperty, PropertyBindingOptions.OneWay);
			this.linePrimitive.BindProperty(LinePrimitive.OrientationProperty, this,
				RadLineItem.OrientationProperty, PropertyBindingOptions.OneWay);

			this.Children.Add(this.linePrimitive);
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if(e.Property == RadLineItem.LineStyleProperty)
			{
				LineDrawingStyle newStyle = (LineDrawingStyle) e.NewValue;
				
				switch(newStyle)
				{
					case LineDrawingStyle.Flat:
						this.linePrimitive.NumberOfColors = 1;
						this.UnbindBackColorProperties();

						this.linePrimitive.BindProperty(LinePrimitive.BackColorProperty, this,
							RadLineItem.LineColorProperty, PropertyBindingOptions.OneWay);
					break;
					case LineDrawingStyle.Bevel:
						this.linePrimitive.NumberOfColors = 3;
						
						this.UnbindBackColorProperties();

						this.linePrimitive.BindProperty(LinePrimitive.BackColorProperty, this,
							RadLineItem.LineColorProperty, PropertyBindingOptions.OneWay);
						this.linePrimitive.BindProperty(LinePrimitive.BackColor2Property, this,
							RadLineItem.LineColor2Property, PropertyBindingOptions.OneWay);
						this.linePrimitive.BindProperty(LinePrimitive.BackColor3Property, this,
							RadLineItem.LineColor2Property, PropertyBindingOptions.OneWay);
					break;
					case LineDrawingStyle.Emboss:
						this.linePrimitive.NumberOfColors = 3;
						this.UnbindBackColorProperties();

						this.linePrimitive.BindProperty(LinePrimitive.BackColorProperty, this,
							RadLineItem.LineColor2Property, PropertyBindingOptions.OneWay);
						this.linePrimitive.BindProperty(LinePrimitive.BackColor2Property, this,
							RadLineItem.LineColorProperty, PropertyBindingOptions.OneWay);
						this.linePrimitive.BindProperty(LinePrimitive.BackColor3Property, this,
							RadLineItem.LineColorProperty, PropertyBindingOptions.OneWay);
					break;
				}
			}
			
			base.OnPropertyChanged(e);
		}
		
		private void UnbindBackColorProperties()
		{
			this.linePrimitive.UnbindProperty(LinePrimitive.BackColorProperty);
			this.linePrimitive.UnbindProperty(LinePrimitive.BackColor2Property);
			this.linePrimitive.UnbindProperty(LinePrimitive.BackColor3Property);
		}
	}
}
