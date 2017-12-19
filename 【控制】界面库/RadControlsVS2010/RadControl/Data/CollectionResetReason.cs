using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines possible reasons for a Reset notification from RadCollectionView.
    /// </summary>
    public enum CollectionResetReason
    {
        /// <summary>
        /// Entire data has changed.
        /// </summary>
        Refresh,
        /// <summary>
        /// Reset has been initiated by a change in collection's filtering logic.
        /// </summary>
        FilteringChanged,
        /// <summary>
        /// Reset has been initiated by a change in collection's grouping logic.
        /// </summary>
        GroupingChanged,
        /// <summary>
        /// Reset has been initiated by a change in collection's sorting logic.
        /// </summary>
        SortingChanged,
    }
}
