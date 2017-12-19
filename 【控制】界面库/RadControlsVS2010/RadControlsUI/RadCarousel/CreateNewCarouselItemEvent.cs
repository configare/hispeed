using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// CreateNewCarouselItem delegate usined by RadCarousel control
    /// </summary>
    public delegate void NewCarouselItemCreatingEventHandler(object sender, NewCarouselItemCreatingEventArgs e);

    /// <summary>
    /// Arguments of CreateNewCarouselItem event
    /// </summary>
    public class NewCarouselItemCreatingEventArgs: EventArgs
    {
        private RadItem newCarouselItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewCarouselItemCreatingEventArgs"/> class.
        /// </summary>
        /// <param name="newCarouselItem">The new carousel item.</param>
        public NewCarouselItemCreatingEventArgs(RadItem newCarouselItem)
        {
            this.newCarouselItem = newCarouselItem;
        }

        /// <summary>
        /// Gets or sets the newly created item that will be added in RadCarousel
        /// </summary>
        public RadItem NewCarouselItem
        {
            get { return newCarouselItem; }
            set { newCarouselItem = value; }
        }
    }
}
