using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
namespace Telerik.WinControls.UI
{
	/// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public class TrackBarUThumbShape : ElementShape
	{
		/// <summary>
		/// Creates IE like tab shape. Overrides CreatePath method in the base class
		/// ElementShape.
		/// </summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			GraphicsPath path = new GraphicsPath();

			path.AddPolygon(new Point[] {
                            new Point(0, bounds.Height/4),
                            new Point(bounds.Width / 2, 0),
                            new Point(bounds.Width, bounds.Height/4)
                        });
			path.AddRectangle(new Rectangle(0, bounds.Height / 4, bounds.Width, 3 * bounds.Height / 4));
			return path;
		}
	}
}
