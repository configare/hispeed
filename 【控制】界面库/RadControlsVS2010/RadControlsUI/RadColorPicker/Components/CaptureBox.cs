using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public partial class CaptureBox : UserControl
	{
        private Timer timer;

        private Color capturedColor = Color.Empty;
		/// <summary>
		/// Gets or sets the captured color
		/// </summary>
		public Color CapturedColor
		{
			get { return capturedColor; }
			set { capturedColor = value; }
		}

        /// <summary>
        /// Fires when the color is changed.
        /// </summary>
		public event ColorChangedEventHandler ColorChanged;

		public CaptureBox()
		{
			InitializeComponent();
			timer = new Timer();
			timer.Interval = 1;
			timer.Tick += new EventHandler(timer_Tick);
		}

        private Point mouse = Point.Empty;

        private void timer_Tick(object sender, EventArgs e)
		{
			if (mouse != Control.MousePosition)
			{
				mouse = Control.MousePosition;

				IntPtr hdc = NativeMethods.CreateDC("Display", null, null, IntPtr.Zero);
				int cr = NativeMethods.GetPixel(hdc, Control.MousePosition.X, Control.MousePosition.Y);
				NativeMethods.DeleteDC(new HandleRef(null, hdc));

				Color color = Color.FromArgb((cr & 0x000000FF),
							   (cr & 0x0000FF00) >> 8,
							   (cr & 0x00FF0000) >> 16);

				if (color != capturedColor)
				{
					capturedColor = color;
					if (this.ColorChanged != null)
						this.ColorChanged(this, new ColorChangedEventArgs(capturedColor));
				}
			}
		}

		public void Start()
		{
			timer.Start();
			this.Capture = true;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.Capture = false;
			timer.Stop();
		}

		public static Color GetScreenColor()
		{
			IntPtr hdc = NativeMethods.CreateDC("Display", null, null, IntPtr.Zero);
			int cr = NativeMethods.GetPixel(hdc, Control.MousePosition.X, Control.MousePosition.Y);
			NativeMethods.DeleteDC(new HandleRef(null, hdc));

			Color color = Color.FromArgb((cr & 0x000000FF),
						   (cr & 0x0000FF00) >> 8,
						   (cr & 0x00FF0000) >> 16);

			return color;
		}
	}
}
