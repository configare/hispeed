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
	public partial class ProfessionalColorsSlider : UserControl
	{
		private bool mouseMoving;
		private bool setHueSilently = false;
		public ProfessionalColorsSlider()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
		}

        private ColorModes colorMode;
		/// <summary>
		/// Gets or sets the color mode of the slider
		/// </summary>
		public ColorModes ColorMode
		{
			get { return colorMode; }
			set 
			{ 
				colorMode = value; 
				ResetSlider();
				Refresh(); 
			}
		}

        private HslColor colorHSL = HslColor.FromAhsl(255);
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
				ResetSlider();
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
				if(!setHueSilently)
					colorHSL = HslColor.FromColor(colorRGB);
				ResetSlider();
				Refresh(); 
			}
		}

		private int position;
		/// <summary>
		/// Gets or sets the position of the slider arrow
		/// </summary>
		public int Position
		{
			get { return position; }
			set 
			{
				int y = value;

				if (y < 0) y = 0;
				if (y > this.Height - 9) y = this.Height - 9;

				if (y != position)
				{
					position = y;
					ResetHSLRGB();
					Refresh();
					if (ColorChanged != null)
						ColorChanged(this, new ColorChangedEventArgs(colorRGB));
				}
			}
		}
		/// <summary>
		/// Fires when the selected color has changed
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

		    HslColor hsl = HslColor.FromAhsl(255);

			switch (this.ColorMode)
			{
				case ColorModes.Hue:
					hsl.L = hsl.S = 1.0;
					break;

				case ColorModes.Saturation:
					hsl.H = this.ColorHSL.H;
					hsl.L = this.ColorHSL.L;
					break;

				case ColorModes.Luminance:
					hsl.H = this.ColorHSL.H;
					hsl.S = this.ColorHSL.S;
					break;
			}

			for (int i = 0; i < this.Height - 8; i++)
			{
				double step = 0;
				if (this.ColorMode < ColorModes.Hue)
					step = 255.0 - ColorProvider.Round(255.0 * (double)i / (this.Height - 8.0));
				else
					step = 1.0 - ( (double)i / (this.Height - 8) );

				Color color = Color.Empty;
				switch (this.ColorMode)
				{
					case ColorModes.Hue: hsl.H = step; color = hsl.RgbValue; break;
					case ColorModes.Saturation: hsl.S = step; color = hsl.RgbValue; break;
					case ColorModes.Luminance: hsl.L = step; color = hsl.RgbValue; break;
					case ColorModes.Red: color = Color.FromArgb((int)step, this.ColorRGB.G, this.ColorRGB.B); break;
					case ColorModes.Green: color = Color.FromArgb(this.ColorRGB.R, (int)step, this.ColorRGB.B); break;
					case ColorModes.Blue: color = Color.FromArgb(this.ColorRGB.R, this.ColorRGB.G, (int)step); break;
				}

				using (Pen pen = new Pen(color))
					e.Graphics.DrawLine(pen, 11, i + 4, this.Width - 11, i + 4);
			}

			DrawSlider(e.Graphics);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			mouseMoving = true;
			Position = e.Y - 4;
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (mouseMoving)
			{
				Position = e.Y - 4;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			mouseMoving = false;
		}
		
		private void DrawSlider(Graphics g)
		{
			using (Pen pencil = new Pen(Color.FromArgb(116, 114, 106)))
			{
				Brush brush = Brushes.White;

				Point[] arrow = new Point[7];				//	 GGG
				arrow[0] = new Point(1, position);			//	G   G
				arrow[1] = new Point(3, position);			//	G    G
				arrow[2] = new Point(7, position + 4);		//	G     G
				arrow[3] = new Point(3, position + 8);		//	G      G
				arrow[4] = new Point(1, position + 8);		//	G     G
				arrow[5] = new Point(0, position + 7);		//	G    G
				arrow[6] = new Point(0, position + 1);		//	G   G

				g.FillPolygon(brush, arrow);
				g.DrawPolygon(pencil, arrow);

				arrow[0] = new Point(this.Width - 2, position);		//	   G   G
				arrow[1] = new Point(this.Width - 4, position);		//	  G    G
				arrow[2] = new Point(this.Width - 8, position + 4);	//	 G     G
				arrow[3] = new Point(this.Width - 4, position + 8);	//	G      G
				arrow[4] = new Point(this.Width - 2, position + 8);	//	 G     G
				arrow[5] = new Point(this.Width - 1, position + 7);	//	  G    G
				arrow[6] = new Point(this.Width - 1, position + 1);	//	   G   G

				g.FillPolygon(brush, arrow);
				g.DrawPolygon(pencil, arrow);
			}
		}
		private void ResetHSLRGB()
		{
			setHueSilently = true;
			switch (ColorMode)
			{
				case ColorModes.Hue:
					colorHSL.H = 1.0 - ( (double)position / (this.Height - 9) );
					ColorRGB = ColorHSL.RgbValue;
					break;
				case ColorModes.Saturation:
					colorHSL.S = 1.0 - ( (double)position / (this.Height - 9) );
					ColorRGB = ColorHSL.RgbValue;
					break;
				case ColorModes.Luminance:
					colorHSL.L = 1.0 - ( (double)position / (this.Height - 9) );
					ColorRGB = ColorHSL.RgbValue;
					break;
				case ColorModes.Red:
					ColorRGB = Color.FromArgb(255 - ColorProvider.Round(255 * (double)position / (this.Height - 9)), ColorRGB.G, ColorRGB.B);
					ColorHSL = HslColor.FromColor(ColorRGB);
					break;
				case ColorModes.Green:
					ColorRGB = Color.FromArgb(ColorRGB.R, 255 - ColorProvider.Round(255 * (double)position / (this.Height - 9)), ColorRGB.B);
                    ColorHSL = HslColor.FromColor(ColorRGB);
					break;
				case ColorModes.Blue:
					ColorRGB = Color.FromArgb(ColorRGB.R, ColorRGB.G, 255 - ColorProvider.Round(255 * (double)position / (this.Height - 9)));
                    ColorHSL = HslColor.FromColor(ColorRGB);
					break;
			}
			setHueSilently = false;
		}
		private void ResetSlider()
		{
			double step = 0;

			switch (ColorMode)
			{
				case ColorModes.Hue: step = colorHSL.H;	break;
				case ColorModes.Saturation: step = colorHSL.S; break;
				case ColorModes.Luminance: step = colorHSL.L; break;
				case ColorModes.Red: step = colorRGB.R / 255.0; break;
				case ColorModes.Green: step = colorRGB.G / 255.0; break;
				case ColorModes.Blue: step = colorRGB.B / 255.0; break;
			}

			position = this.Height - 8 - ColorProvider.Round((this.Height - 8) * step);
		}
	}
}
