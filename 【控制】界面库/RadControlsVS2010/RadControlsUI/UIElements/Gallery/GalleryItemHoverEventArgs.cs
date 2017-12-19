using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class GalleryItemHoverEventArgs : EventArgs
    {
        private RadGalleryItem item;

        public RadGalleryItem GalleryItem
        {
            get
            {
                return this.item;
            }
        }

        public GalleryItemHoverEventArgs(RadGalleryItem galleryItem)
        {
            this.item = galleryItem;
        }
    }
}
