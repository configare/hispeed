using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Sets the way opacity is applied to carousel items 
    /// </summary>
    public enum OpacityChangeConditions
    {
        /// <summary>
        /// Opacity is not modified
        /// </summary>
        None,
        /// <summary>
        /// Selected item is with opacity 1.0. Opacity decreases corresponding to the distance from the selected item.
        /// </summary>
        SelectedIndex,
        /// <summary>
        /// Opacity increases relatively to items' ZIndex. The Item with greatest ZIndex has opacity of 1.0
        /// </summary>
        ZIndex
    }
}
