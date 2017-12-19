using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// 	<para>Content within a user interface is often larger than the visible area that
    ///     the user can see. Large Telerik elements can be put in scroll viewer in order to
    ///     scroll their content in small visible area.</para>
    /// 	<para>
    ///         Every element that support scrolling must implement this interface. Currently
    ///         only class RadScrollViewer implements this interface and all
    ///         Telerik elements that can be scrolled inherit that class.
    ///     </para>
    /// </summary>
    public interface IScrollViewer
    {
        /// <summary>
        /// Gets whether the scroll viewer uses a virtualized viewport
        /// </summary>
        bool Virtualized
        {
            get;
        }

        /// <summary>
        /// Scrolls down within viewport by one logical unit.
        /// </summary>
        void LineDown();
        /// <summary>
        /// Scrolls left within viewport by one logical unit.
        /// </summary>
        void LineLeft();
        /// <summary>
        /// Scrolls right within viewport by one logical unit.
        /// </summary>
        void LineRight();
        /// <summary>
        /// Scrolls up within viewport by one logical unit.
        /// </summary>
        void LineUp();

        /// <summary>
        /// Scrolls down within viewport by one page.
        /// </summary>
        void PageDown();
        /// <summary>
        /// Scrolls left within viewport by one page.
        /// </summary>
        void PageLeft();
        /// <summary>
        /// Scrolls right within viewport by one page.
        /// </summary>
        void PageRight();
        /// <summary>
        /// Scrolls up within viewport by one page.
        /// </summary>
        void PageUp();

        /// <summary>
        /// Scrolls vertically to the beginning of the content.
        /// </summary>
        void ScrollToTop();
        /// <summary>
        /// Scrolls vertically to the end of the content.
        /// </summary>
        void ScrollToBottom();
        /// <summary>
        /// Scrolls horizontally to the beginning of the content.
        /// </summary>
        void ScrollToLeftEnd();
        /// <summary>
        /// Scrolls horizontally to the end of the content.
        /// </summary>
        void ScrollToRightEnd();

        /// <summary>
        /// Scrolls both horizontally and vertically to the beginning of the content.
        /// </summary>
        void ScrollToHome();
        /// <summary>
        /// Scrolls both horizontally and vertically to the end of the content.
        /// </summary>
        void ScrollToEnd();

        void ScrollElementIntoView(RadElement element);
    }
}
