using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents IE like tab shape. Shapes are series of connected lines and curves.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class TabIEShape : ElementShape
	{
        /// <summary>
        /// Creates IE like tab shape. Overrides CreatePath method in the base class
        /// ElementShape.
        /// </summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			GraphicsPath path = new GraphicsPath();

			path.AddLine(new Point(bounds.X, bounds.Y + bounds.Height + 1), new Point(bounds.X, bounds.Y + 2));
			path.AddLine(new Point(bounds.X, bounds.Y + 2), new Point(bounds.X + 2, bounds.Y));
			path.AddLine(new Point(bounds.X + 2, bounds.Y), new Point(bounds.X + bounds.Width - 2, bounds.Y));
			path.AddLine(new Point(bounds.X + bounds.Width - 2, bounds.Y), new Point(bounds.X + bounds.Width, bounds.Y + 2));
			path.AddLine(new Point(bounds.X + bounds.Width, bounds.Y + 2), new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height + 1));

			return path;
		}
	}

}
