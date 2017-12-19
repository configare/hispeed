using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
    /// <summary>Represents office 12 like tab.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class TabOffice12Shape : ElementShape
	{

        /// <summary>
        /// Creates office 12 like tab. Overrides the method defined in its base class -
        /// ElementShape.
        /// </summary>
        public override GraphicsPath CreatePath(Rectangle bounds)
		{
            GraphicsPath path = new GraphicsPath();

            path.AddLines(
                new Point[]
                    {
                        new Point(bounds.X + 1, bounds.Bottom),
                        new Point(bounds.X, bounds.Bottom),
                        new Point(bounds.X + 2, bounds.Bottom - 1),
                        new Point(bounds.X + 3, bounds.Bottom - 2),
                        new Point(bounds.X + 3, bounds.Y + 3),
                        new Point(bounds.X + 4, bounds.Y + 2),
                        new Point(bounds.X + 5, bounds.Y + 1),
                        new Point(bounds.Right - 5, bounds.Y + 1),
                        new Point(bounds.Right - 4, bounds.Y + 2),
                        new Point(bounds.Right - 3, bounds.Y + 3),
                        new Point(bounds.Right - 3, bounds.Bottom - 2),
                        new Point(bounds.Right - 2, bounds.Bottom - 1),
                        new Point(bounds.Right, bounds.Bottom),
                        new Point(bounds.Right - 1, bounds.Bottom)
                    }
                );


            return path;
		}
	}
}