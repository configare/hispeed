using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public partial class ColorWheel : UserControl
	{
        private const int COLOR_COUNT = 6 * 256;
        private const double DEGREES_PER_RADIAN = 180.0 / Math.PI;
        private bool mouseMoving;
        private Point markerPoint = Point.Empty;

		public ColorWheel()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

		}

        private Color colorRGB = Color.Empty;
		/// <summary>
		/// Gets or sets the RgbValue value
		/// </summary>
		public Color ColorRGB
		{
			get { return colorRGB; }
            set
            {
                colorRGB = value; 
                colorHSL = HslColor.FromColor(value); 
                Refresh(); 
                UpdateMarker();
            }
		}

        private HslColor colorHSL;
		/// <summary>
		/// Gets or sets the HSL value
		/// </summary>
		public HslColor ColorHSL
		{
			get { return colorHSL; }
			set { colorHSL = value; colorRGB = value.RgbValue; Refresh(); UpdateMarker();  }
		}

		/// <summary>
		/// Fires when the selected color changes
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			int radius = (int)Math.Min(this.Width, this.Height) / 2;
			Point centerPoint = new Point(this.Width / 2, this.Height / 2);
			
			Point[] points = new Point[COLOR_COUNT];
			Color[] colors = new Color[COLOR_COUNT];

			for (int i = 0; i <= COLOR_COUNT - 1; i++)
				points[i] = GetPoint((i * 360.0) / COLOR_COUNT, radius, centerPoint);

			for (int i = 0; i <= COLOR_COUNT - 1; i++)
				colors[i] = HSVtoRGB((int)((i * 255.0) / COLOR_COUNT), (int)255, (int)255);

			using (PathGradientBrush brush = new PathGradientBrush(points))
			{
				brush.CenterColor = Color.White;
				brush.CenterPoint = new PointF(radius, radius);
				brush.SurroundColors = colors;

				e.Graphics.FillEllipse(brush, 0, 0,	this.Width, this.Height);
			}

			e.Graphics.DrawEllipse(Pens.Black, markerPoint.X - 4, markerPoint.Y - 4, 8, 8);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			mouseMoving = true;

			Region colorRegion;
			using (GraphicsPath path = new GraphicsPath())
			{
				path.AddEllipse(new Rectangle(0, 0, this.Width, this.Height));
				colorRegion = new Region(path);
			}

			if (colorRegion.IsVisible(e.X, e.Y))
			{
				markerPoint = new Point(e.X, e.Y);
				UpdateColor();
				Refresh();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (mouseMoving)
			{
				Region colorRegion;
				using (GraphicsPath path = new GraphicsPath())
				{
					path.AddEllipse(new Rectangle(0, 0, this.Width, this.Height));
					colorRegion = new Region(path);
				}

				if (colorRegion.IsVisible(e.X, e.Y))
				{
					markerPoint = new Point(e.X, e.Y);
					UpdateColor();
					Refresh();
				}
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			mouseMoving = false;
		}

        private Point GetPoint(double degrees, double radius, Point centerPoint)
		{
			double radians = degrees / DEGREES_PER_RADIAN;

			return new Point((int)(centerPoint.X + Math.Floor(radius * Math.Cos(radians))),
				(int)(centerPoint.Y - Math.Floor(radius * Math.Sin(radians))));
		}
        private Color HSVtoRGB(int hue, int saturation, int value)
		{
			double h;
			double s;
			double v;

			double r = 0;
			double g = 0;
			double b = 0;

			h = ((double)hue / 255 * 360) % 360;
			s = (double)saturation / 255;
			v = (double)value / 255;

			if (s == 0)
			{
				r = v;
				g = v;
				b = v;
			}
			else
			{
				double p;
				double q;
				double t;

				double fractionalSector;
				int sectorNumber;
				double sectorPos;

				sectorPos = h / 60;
				sectorNumber = (int)Math.Floor(sectorPos);
				fractionalSector = sectorPos - sectorNumber;
				p = v * (1 - s);
				q = v * (1 - (s * fractionalSector));
				t = v * (1 - (s * (1 - fractionalSector)));

				switch (sectorNumber)
				{
					case 0:
						r = v;
						g = t;
						b = p;
						break;

					case 1:
						r = q;
						g = v;
						b = p;
						break;

					case 2:
						r = p;
						g = v;
						b = t;
						break;

					case 3:
						r = p;
						g = q;
						b = v;
						break;

					case 4:
						r = t;
						g = p;
						b = v;
						break;

					case 5:
						r = v;
						g = p;
						b = q;
						break;
				}
			}

			return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
		}

        private void UpdateColor()
		{
			int radius = Math.Min(this.Width, this.Height) / 2;
			Point centerPoint = new Point(this.Width / 2, this.Height / 2);
			Point delta = new Point(markerPoint.X - centerPoint.X, markerPoint.Y - centerPoint.Y);
			int degrees = CalcDegrees(delta);
			double distance = Math.Sqrt( ( delta.X * delta.X ) + ( delta.Y * delta.Y) ) / radius;

			int hue = degrees * 255 / 360;
			int saturation = (int)(distance * 255);
			int value = 255;
			
			colorRGB = HSVtoRGB(hue, saturation, value);
            colorHSL = HslColor.FromColor(colorRGB);

			if (ColorChanged != null)
				ColorChanged(this, new ColorChangedEventArgs(colorRGB));
		}

		private int CalcDegrees(Point pt)
		{
			int degrees;

			if (pt.X == 0)
			{
				if (pt.Y > 0) degrees = 270;
				else degrees = 90;
			}
			else
			{
				degrees = (int)(-Math.Atan((double)pt.Y / pt.X) * DEGREES_PER_RADIAN);
				if (pt.X < 0) degrees += 180;
				degrees = (degrees + 360) % 360;
			}

			return degrees;
		}

		private void UpdateMarker()
		{
		}
	}
}
