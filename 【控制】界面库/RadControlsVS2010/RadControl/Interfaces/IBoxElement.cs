using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// Defines properties for the box-model; Elements are nodes of a tree, and a
    /// rectangular box is generated for each element.
    /// </summary>
	public interface IBoxElement
	{
        /// <summary>Gets or sets a value indicating the box width.</summary>
		float Width { get; set; }

        /// <summary>Gets or sets a value indicating the left width.</summary>
		float LeftWidth { get; set; }

        /// <summary>Gets or sets a value indicating the top width.</summary>
		float TopWidth { get; set; }

        /// <summary>Gets or sets a value indicating the right width.</summary>
		float RightWidth { get; set; }

        /// <summary>Gets or sets a value indicating the botton width.</summary>
		float BottomWidth { get; set; }

        /// <summary>Gets a value indicating the offset.</summary>
		SizeF Offset { get; }

        /// <summary>Gets a value indicating the border size.</summary>
		SizeF BorderSize { get; }

        /// <summary>Gets a value indicating the horizontal width.</summary>
		float HorizontalWidth { get; }

        /// <summary>Gets a value indicating the vertical width.</summary>
		float VerticalWidth { get; }
	}
}