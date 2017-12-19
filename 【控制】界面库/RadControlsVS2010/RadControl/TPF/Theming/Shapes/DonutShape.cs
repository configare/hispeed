using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.Tests
{
    /// <summary>Represents donut like shape.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class DonutShape : ElementShape
	{
        private int thickness = 10;

        [DefaultValue(10)]
        public int Thickness
        {
            get { return thickness; }
            set { thickness = value; }
        }

        /// <summary>
        /// Creates donut-like path. Overrides the method defined in its base class -
        /// ElementShape.
        /// </summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse(bounds);
            bounds.Inflate(-thickness, -thickness);
			path.AddEllipse(bounds);
			return path;
		}
	}

    [ToolboxItem(false), ComVisible(false)]
	public class MediaShape : ElementShape
	{
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			
			Size size = new Size(50, 50);
			if (bounds.Height < 50)
				size = new Size(bounds.Height / 2, bounds.Height / 2);

			Rectangle rc = bounds;
			GraphicsPath path = new GraphicsPath();

			int height = rc.Height - 3 * size.Height / 4;
			if (height <= 0) height = 1;

			// left down arc
			path.AddArc(rc.Left, rc.Top + size.Height / 4,
				size.Width, height, 90, 180);

			// top lines

			path.AddLine(rc.Left + size.Width / 2, rc.Top + size.Height / 4,
				rc.Left + (float)((float)(2 * rc.Width) / 5) + rc.Width / 20, rc.Top + size.Height / 4);

			path.AddArc(rc.Left + (float)((float)(2 * rc.Width) / 5) + rc.Width / 20, rc.Top,
				(float)((float)(rc.Width) / 10), size.Height / 2, 180, 180);

			path.AddLine(rc.Left + (float)((float)(3 * rc.Width) / 5), rc.Top + size.Height / 4,
			   rc.Right - size.Width / 2, rc.Top + size.Height / 4);

			path.AddArc(rc.Right - size.Width, rc.Top + size.Height / 4,
				size.Width, height, 270, 180);

			// bottom lines

			path.AddLine(rc.Right - size.Width / 2, rc.Bottom - size.Height / 2, rc.Left + (float)((float)(3 * rc.Width) / 5),
				rc.Bottom - size.Height / 2);

			path.AddArc(rc.Left + (float)((float)(2 * rc.Width) / 5) + rc.Width / 20, rc.Bottom - 3 * size.Height / 4,
				(float)((float)(rc.Width) / 10), size.Height / 2, 0, 180);

			path.AddLine(rc.Left + size.Width / 2, rc.Bottom - size.Height / 2,
	rc.Left + (float)((float)(2 * rc.Width) / 5) + rc.Width / 20, rc.Bottom - size.Height / 2);



			path.CloseAllFigures();

			return path;

		}
	}
}