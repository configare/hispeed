using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public partial class RadarColors : UserControl
	{
		public RadarColors()
		{
			InitializeComponent();
            this.adobeColorsSlider1.ColorHSL = HslColor.FromColor(Color.Green);
			this.adobeColorsSlider1.ColorMode = ColorModes.Luminance;

			this.adobeColorsSlider1.ColorChanged += new ColorChangedEventHandler(adobeColorsSlider1_ColorChanged);
			this.colorWheel1.ColorChanged += new ColorChangedEventHandler(colorWheel1_ColorChanged);
		}

		private Color colorRGB = Color.Empty;
		/// <summary>
		/// Gets or sets the color in RgbValue format
		/// </summary>
		public Color ColorRGB
		{
			get 
			{
				return colorRGB; 
			}
			set
			{ 
				colorRGB = value;
                HslColor color = HslColor.FromColor(value);
				this.adobeColorsSlider1.ColorHSL = color;
				this.colorWheel1.ColorRGB = value;
			}
		}
		/// <summary>
		/// Fires when the selected color changes
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

		private void colorWheel1_ColorChanged(object sender, ColorChangedEventArgs args)
		{
			HslColor oldColor = this.adobeColorsSlider1.ColorHSL;
            HslColor newColor = HslColor.FromColor(args.SelectedColor);
			newColor.L = oldColor.L;
			this.adobeColorsSlider1.ColorHSL = newColor;

			this.colorRGB = newColor.RgbValue;

			if (ColorChanged != null)
				ColorChanged(this, new ColorChangedEventArgs(this.colorRGB));
		}

		private void adobeColorsSlider1_ColorChanged(object sender, ColorChangedEventArgs args)
		{
			if (ColorChanged != null)
				ColorChanged(this, args);
		}			
	}
}
