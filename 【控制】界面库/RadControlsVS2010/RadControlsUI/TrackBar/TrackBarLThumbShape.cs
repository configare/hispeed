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
	public class TrackBarLThumbShape : ElementShape
	{
		/// <summary>
		/// Creates IE like tab shape. Overrides CreatePath method in the base class
		/// ElementShape.
		/// </summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			GraphicsPath path = new GraphicsPath();

			path.AddPolygon(
					   new Point[] {
                            new Point(bounds.Width/4, 0),
                            new Point(0, bounds.Height / 2),
                            new Point(bounds.Width/4, bounds.Height)
                        });
			path.AddRectangle(new Rectangle(bounds.Width / 4, 0, 3 * bounds.Width / 4, bounds.Height));

			return path;
		}
	}
}
