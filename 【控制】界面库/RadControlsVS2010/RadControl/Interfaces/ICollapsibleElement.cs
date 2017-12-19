using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// Defines methods and properties for a calapsible element. For example, 
    /// RadRibonBarChunk is a collapsible element.
    /// </summary>
	public interface ICollapsibleElement
	{
        /// <summary>
        /// Expands the element.
        /// </summary>
		bool ExpandElement();

		/// <summary>
        /// Collapses the element.
        /// </summary>
		bool CollapseElement(Size preferredSize);

        /// <summary>
        /// Gets or sets a value indicating the expanded size of the element.
        /// </summary>
		Size ExpandedSize { get; set; }

		/// <summary>
		/// Gets the max number of steps needed for collapsing the collapsible element.
		/// </summary>
		int CollapseMaxSteps { get; }

		/// <summary>
		/// Gets the current collapse step for the collapsible element.
		/// </summary>
        int CollapseStep { get; set; }
	}
}