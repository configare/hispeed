using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
namespace Telerik.WinControls.UI
{
	/// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public class TrackBarDThumbShape : ElementShape
	{
		/// <summary>
		/// Creates IE like tab shape. Overrides CreatePath method in the base class
		/// ElementShape.
		/// </summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			GraphicsPath path = new GraphicsPath();

			path.AddRectangle(new Rectangle(bounds.X, bounds.Y, bounds.Width, 3 * bounds.Height / 4));
			path.CloseFigure();

			path.AddPolygon(new Point[] {
                            new Point(0, 3 * bounds.Height / 4),
                            new Point(bounds.Width / 2, bounds.Height),
                            new Point(bounds.Width, 3 *bounds.Height / 4) });

			return path;

		}
	}
}
