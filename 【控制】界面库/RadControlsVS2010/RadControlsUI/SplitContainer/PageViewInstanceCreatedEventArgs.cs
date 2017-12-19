using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle the PageViewInstanceCreated events of RadDock.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Instance of PageViewInstanceCreatedEventArgs.</param>
    public delegate void PageViewInstanceCreatedEventHandler(object sender, PageViewInstanceCreatedEventArgs e);

    /// <summary>
    /// Provides data for the PageViewInstanceCreated event.
    /// </summary>
    public class PageViewInstanceCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the created RadPageViewElement.
        /// </summary>
        public readonly RadPageViewElement PageViewElement;

        /// <summary>
        /// Initializes a new instance of the PageViewInstanceCreatedEventArgs class.
        /// </summary>
        /// <param name="pageViewElement">The created RadPageViewElement.</param>
        public PageViewInstanceCreatedEventArgs(RadPageViewElement pageViewElement)
        {
            this.PageViewElement = pageViewElement;
        }
    }
}