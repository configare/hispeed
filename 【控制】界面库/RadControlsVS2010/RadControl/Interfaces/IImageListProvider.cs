using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <summary>
    /// Exposes the ImageList property. All classes that implement this interface 
    /// provide an image list.
    /// </summary>
    public interface IImageListProvider
    {
        /// <summary>
        /// Gets the image list.
        /// </summary>
        System.Windows.Forms.ImageList ImageList { get;}
    }
}
