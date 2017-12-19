using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public partial class ProfessionalColors2DBox : UserControl
	{
		private bool mouseMoving;
		private Point markerPoint = Point.Empty;

		public ProfessionalColors2DBox()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
		}

        private ColorModes colorMode;
		/// <summary>
		/// Gets or sets the color mode
		/// </summary>
		public ColorModes ColorMode
		{
			get { return colorMode; }
			set 
			{ 
				colorMode = value;
				ResetMarker();
				Refresh(); 
			}
		}

		private HslColor colorHSL;
		/// <summary>
		/// Gets or sets the color in HSL format
		/// </summary>
		public HslColor ColorHSL
		{
			get { return colorHSL; }
			set 
			{ 
				colorHSL = value;
				colorRGB = colorHSL.RgbValue;
				ResetMarker();
				Refresh(); 
			}
		}

        private Color colorRGB = Color.Empty;
		/// <summary>
		/// Gets or sets the color in RgbValue format
		/// </summary>
		public Color ColorRGB
		{
			get { return colorRGB; }
			set 
			{ 
				colorRGB = value;
				colorHSL = HslColor.FromColor(colorRGB);
				ResetMarker();
				Refresh(); 
			}
		}
		/// <summary>
		/// Fires when the selected color has changed
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

            HslColor hslcolor1 = HslColor.FromAhsl(255);
            HslColor hslcolor2 = HslColor.FromAhsl(255);

			switch (this.ColorMode)
			{
				case ColorModes.Hue:
					hslcolor1.H = this.ColorHSL.H;
					hslcolor2.H = this.ColorHSL.H;
					hslcolor1.S = 0.0;
					hslcolor2.S = 1.0;
					break;

				case ColorModes.Saturation:
					hslcolor1.S = this.ColorHSL.S;
					hslcolor2.S = this.ColorHSL.S;
					hslcolor1.L = 1.0;
					hslcolor2.L = 0.0;
					break;

				case ColorModes.Luminance:
					hslcolor1.L = this.ColorHSL.L;
					hslcolor2.L = this.ColorHSL.L;
					hslcolor1.S = 1.0;
					hslcolor2.S = 0.0;
					break;
			}

			for (int i = 0; i < this.Height - 4; i++)
			{
				int value = ColorProvider.Round(255 - (255 * (double)i / (this.Height - 4)));
				Color color1 = Color.Empty;
				Color color2 = Color.Empty;

				switch (this.ColorMode)
				{
					case ColorModes.Red:
						color1 = Color.FromArgb(this.ColorRGB.R, value, 0);
						color2 = Color.FromArgb(this.ColorRGB.R, value, 255);
						break;
					
					case ColorModes.Green:
						color1 = Color.FromArgb(value, this.ColorRGB.G, 0);
						color2 = Color.FromArgb(value, this.ColorRGB.G, 255);
						break;
					
					case ColorModes.Blue:
						color1 = Color.FromArgb(0, value, this.ColorRGB.B);
						color2 = Color.FromArgb(255, value, this.ColorRGB.B);
						break;
					
					case ColorModes.Hue:
						hslcolor2.L = hslcolor1.L = 1.0 - (double)i / (this.Height - 4);
						color1 = hslcolor1.RgbValue;
						color2 = hslcolor2.RgbValue;
						break;

					case ColorModes.Saturation:
					case ColorModes.Luminance:
						hslcolor2.H = hslcolor1.H = (double)i / (this.Width - 4);
						color1 = hslcolor1.RgbValue;
						color2 = hslcolor2.RgbValue;
						break;
				}

				Rectangle bounds1 = new Rectangle(2, 2, this.Width - 4, 1);
				Rectangle bounds2 = new Rectangle(2, i + 2, this.Width - 4, 1);

				if (ColorMode == ColorModes.Saturation || ColorMode == ColorModes.Luminance)
				{
					bounds1 = new Rectangle(2, 2, 1, this.Height - 4);
					bounds2 = new Rectangle(i + 2, 2, 1, this.Height - 4);

					using (LinearGradientBrush brush = new LinearGradientBrush(bounds1, color1, color2, 90, false))
						e.Graphics.FillRectangle(brush, bounds2);
				}
				else
				{
					using (LinearGradientBrush brush = new LinearGradientBrush(bounds1, color1, color2, 0, false))
						e.Graphics.FillRectangle(brush, bounds2);
				}
			}

			Pen pen = Pens.White;
			if (colorHSL.L >= (double)200 / 255)
				if (colorHSL.H < (double)26 / 360 || colorHSL.H > (double)200 / 360)
				{
					if (colorHSL.S <= (double)70 / 255)
						pen = Pens.Black;
				}
				else
					pen = Pens.Black;

			e.Graphics.DrawEllipse(pen, markerPoint.X - 5, markerPoint.Y - 5, 10, 10);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			mouseMoving = true;
			SetMarker(e.X, e.Y);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (mouseMoving)
			{
				SetMarker(e.X, e.Y);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			mouseMoving = false;
		}

        private void SetMarker(int x, int y)
		{
			if (x < 0) x = 0;
			if (x > this.Width - 4) x = this.Width - 4;	//	Calculate marker position
			if (y < 0) y = 0;
			if (y > this.Height - 4) y = this.Height - 4;

			if (markerPoint.X == x && markerPoint.Y == y)
				return;

			markerPoint = new Point(x, y);
			colorHSL = GetColor(x, y);
			colorRGB = colorHSL.RgbValue;
			Refresh();

			if (ColorChanged != null)
				this.ColorChanged(this, new ColorChangedEventArgs(colorRGB));				
		}

        private void ResetMarker()
		{
			switch (colorMode)
			{
				case ColorModes.Hue:
					markerPoint.X = ColorProvider.Round((this.Width - 4) * colorHSL.S);
					markerPoint.Y = ColorProvider.Round((this.Height - 4) * (1.0 - colorHSL.L));
					break;

				case ColorModes.Saturation:
					markerPoint.X = ColorProvider.Round((this.Width - 4) * colorHSL.H);
					markerPoint.Y = ColorProvider.Round((this.Height - 4) * (1.0 - colorHSL.L));
					break;

				case ColorModes.Luminance:
					markerPoint.X = ColorProvider.Round((this.Width - 4) * colorHSL.H);
					markerPoint.Y = ColorProvider.Round((this.Height - 4) * (1.0 - colorHSL.S));
					break;

				case ColorModes.Red:
					markerPoint.X = ColorProvider.Round((this.Width - 4) * (double)colorRGB.B / 255);
					markerPoint.Y = ColorProvider.Round((this.Height - 4) * (1.0 - ((double)colorRGB.G / 255) ));
					break;

				case ColorModes.Green:
					markerPoint.X = ColorProvider.Round((this.Width - 4) * (double)colorRGB.B / 255);
					markerPoint.Y = ColorProvider.Round((this.Height - 4) * (1.0 - ((double)colorRGB.R / 255)) );
					break;

				case ColorModes.Blue:
					markerPoint.X = ColorProvider.Round((this.Width - 4) * (double)colorRGB.R / 255);
					markerPoint.Y = ColorProvider.Round((this.Height - 4) * (1.0 - ((double)colorRGB.G / 255)) );
					break;
			}
		}

        private HslColor GetColor(int x, int y)
		{
			HslColor hsl = HslColor.FromAhsl(255);
			int red, green, blue;

			switch (ColorMode)
			{
				case ColorModes.Hue:
					hsl.H = colorHSL.H;
					hsl.S = (double)x / (this.Width - 4);
					hsl.L = 1.0 - ( (double)y / (this.Height - 4) );
					break;

				case ColorModes.Saturation:
					hsl.S = colorHSL.S;
					hsl.H = (double)x / (this.Width - 4);
					hsl.L = 1.0 - ( (double)y / (this.Height - 4) );
					break;

				case ColorModes.Luminance:
					hsl.L = colorHSL.L;
					hsl.H = (double)x / (this.Width - 4);
					hsl.S = 1.0 - (double)y / (this.Height - 4);
					break;

				case ColorModes.Red:
					green = ColorProvider.Round(255 * (1.0 - (double)y / (this.Height - 4)));
					blue = ColorProvider.Round(255 * (double)x / (this.Width - 4));
                    hsl = HslColor.FromColor(Color.FromArgb(colorRGB.R, green, blue));
					break;

				case ColorModes.Green:
					red = ColorProvider.Round(255 * (1.0 - (double)y / (this.Height - 4)));
					blue = ColorProvider.Round(255 * (double)x / (this.Width - 4));
                    hsl = HslColor.FromColor(Color.FromArgb(red, colorRGB.G, blue));
					break;

				case ColorModes.Blue:
					red = ColorProvider.Round(255 * (double)x / (this.Width - 4));
					green = ColorProvider.Round(255 * (1.0 - (double)y / (this.Height - 4)));
                    hsl = HslColor.FromColor(Color.FromArgb(red, green, colorRGB.B));
					break;
			}

			return hsl;
		}
	}
}
