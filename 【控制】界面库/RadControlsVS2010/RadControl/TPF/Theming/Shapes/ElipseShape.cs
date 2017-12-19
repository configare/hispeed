using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    /// <summary>Represents ellipse shape.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class EllipseShape : ElementShape
	{
        /// <summary>
        /// Creates ellipse shape. Overrides the method defined in its base class -
        /// ElementShape.
        /// </summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse(bounds);
			return path;
		}
	}
}
