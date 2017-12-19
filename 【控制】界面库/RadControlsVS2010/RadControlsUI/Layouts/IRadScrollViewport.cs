using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     This interface defines all necessary methods for custom scrolling. Performing each
    ///     scroll operation via the method <see cref="DoScroll"/> (thus allowing custom
    ///     logic to be used) is called logical scrolling. The only way to enable logical
    ///     scrolling in <see cref="RadScrollViewer"/> is via implementation of this
    ///     interface.
    /// </summary>
    public interface IRadScrollViewport
    {
        /// <summary>
        /// Gets the real size of the content that the viewport must visualize.
        /// </summary>
        Size GetExtentSize();
        
        /// <summary>
        /// Invalidates the viewport.
        /// </summary>
        void InvalidateViewport();

        /// <summary>
        /// Calculate scroll value. This method is used while resizing the scroll panel.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="viewportSize"></param>
        /// <param name="extentSize"></param>
        /// <returns></returns>
        Point ResetValue(Point currentValue, Size viewportSize, Size extentSize);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void DoScroll(Point oldValue, Point newValue);

        /// <summary>Calculates the necessary offset in order to make the given child visible.</summary>
        Size ScrollOffsetForChildVisible(RadElement childElement, Point currentScrollValue);

        /// <summary>
        /// Retrieves the scroll parameters.
        /// </summary>
        /// <returns></returns>
        ScrollPanelParameters GetScrollParams(Size viewportSize, Size extentSize);
    }
}
