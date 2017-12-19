using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents event data of the Scroll event defined in all controls providing 
    /// scrolling functionality(e.g. RadScrollBar).
    /// </summary>
    public class ScrollPanelEventArgs : EventArgs
    {
        private Point oldValue;
        private Point newValue;

        /// <summary>Gets the old thumb position (point).</summary>
        public Point OldValue
        {
            get { return this.oldValue; }
        }

        /// <summary>Gets the new thumb position (point).</summary>
        public Point NewValue
        {
            get { return this.newValue; }
        }
        /// <summary>
        /// Initializes a new instance of the ScrollPanelEventArgs class.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public ScrollPanelEventArgs(Point oldValue, Point newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }

    /// <summary>
    ///     Represents event data for the
    ///     <see cref="RadScrollViewer">ScrollParametersChanged</see> event.
    /// </summary>
    public class RadPanelScrollParametersEventArgs : EventArgs
    {
        private bool isHorizontalScrollBar;
        private ScrollBarParameters scrollBarParameters;

        /// <summary>
        /// Indicates whether the scroll parameters are for the horizontal or for the vertical scroll bar.
        /// </summary>
        public bool IsHorizontalScrollBar
        {
            get { return this.isHorizontalScrollBar; }
        }
        
        /// <summary>
        /// Scroll bar parameters taken from the scroll bar that caused the event.
        /// All parameters are filled correctly, not only the chagned one.
        /// </summary>
        public ScrollBarParameters ScrollBarParameters
        {
            get { return this.scrollBarParameters; }
        }

        public RadPanelScrollParametersEventArgs(bool isHorizontalScrollBar, ScrollBarParameters scrollBarParameters)
        {
            this.isHorizontalScrollBar = isHorizontalScrollBar;
            this.scrollBarParameters = scrollBarParameters;
        }
    }

    /// <summary>
    /// Indicates whether the need for horizontal or vertical srcolling has changed
    /// </summary>
    public class ScrollNeedsEventArgs : EventArgs
    {
        private bool oldHorizontalScrollNeed;
        private bool newHorizontalScrollNeed;
        private bool oldVerticalScrollNeed;
        private bool newVerticalScrollNeed;

        /// <summary>
        /// Indicates whether horizontal scrolling was necessary
        /// </summary>
        public bool OldHorizontalScrollNeed
        {
            get { return this.oldHorizontalScrollNeed; }
        }
        
        /// <summary>
        /// Indicates whether horizontal scrolling is necessary
        /// </summary>
        public bool NewHorizontalScrollNeed
        {
            get { return this.newHorizontalScrollNeed; }
        }

        public bool OldVerticalScrollNeed
        {
            get { return this.oldVerticalScrollNeed; }
        }

        public bool NewVerticalScrollNeed
        {
            get { return this.newVerticalScrollNeed; }
        }

        public ScrollNeedsEventArgs(bool oldHorizontalScrollNeed, bool newHorizontalScrollNeed,
            bool oldVerticalScrollNeed, bool newVerticalScrollNeed)
        {
            this.oldHorizontalScrollNeed = oldHorizontalScrollNeed;
            this.newHorizontalScrollNeed = newHorizontalScrollNeed;
            this.oldVerticalScrollNeed = oldVerticalScrollNeed;
            this.newVerticalScrollNeed = newVerticalScrollNeed;
        }
    }

    public class ScrollViewportSet : EventArgs
    {
        private RadElement oldViewport;
        private RadElement newViewport;

        public RadElement OldViewport
        {
            get { return this.oldViewport; }
        }

        public RadElement NewViewport
        {
            get { return this.newViewport; }
        }

        public ScrollViewportSet(RadElement oldViewport, RadElement newViewport)
        {
            this.oldViewport = oldViewport;
            this.newViewport = newViewport;
        }
    }

    /// <summary>
    ///     Represents the method that will handle the
    ///     <see cref="RadScrollBarElement.Scroll">Scroll</see> event.
    ///     <param name="sender">Represents the event sender.</param>
    ///     <param name="args">Represents the event arguments.</param>
    /// </summary>
    public delegate void RadScrollPanelHandler(object sender, ScrollPanelEventArgs args);

    /// <summary>
    ///     Represents the method that will handle the
    ///     <see cref="RadScrollBarElement.Scroll">Scroll</see> event.
    /// </summary>
    /// <param name="sender">Represents the event sender.</param>
    /// <param name="args">Represents the <see cref="RadPanelScrollParametersEventArgs">event arguments</see>.</param>
    public delegate void RadPanelScrollParametersHandler(object sender, RadPanelScrollParametersEventArgs args);

    /// <summary>
    ///     Represents the method that will handle the
    ///     <see cref="RadScrollBarElement.Scroll">ScrollNeedsChanged</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ScrollNeedsHandler(object sender, ScrollNeedsEventArgs args);

    public delegate void ScrollViewportSetHandler(object sender, ScrollViewportSet args);
}
