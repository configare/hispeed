using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.Layouts
{   
    /// <summary>
    /// Defines basic methods for Telerik layout architecture. Since all layout panels 
    /// update their layout automatically through events, this functions are rarely used 
    /// directly.
    /// </summary>
	public interface IRadLayoutElement
	{
        /// <summary>
        /// Performs layout changes based on the element given as a paramater. 
        /// Sizes and places are determined by the concrete layout panel that is used. 
        /// For example if StackLayoutPanel is used, the element will be placed next to 
        /// the previously placed element. Since all layout panels update their layout 
        /// automatically through events, this function is rarely used directly.
        /// </summary>
        /// <param name="affectedElement"></param>
		void PerformLayout(RadElement affectedElement);

        /// <summary>
        /// Retrieves the preferred size of the layout panel. If the proposed size is 
        /// smaller than the minimal one, the minimal one is retrieved. Since all layout 
        /// panels update their layout automatically through events, this function is 
        /// rarely used directly.
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
		Size GetPreferredSize(Size proposedSize);
	}
}