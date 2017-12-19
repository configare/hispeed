using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines possible modes to be used when rendering an image.
    /// </summary>
    public enum ImagePaintMode
    {
        /// <summary>
        /// Image is painted without any modification.
        /// </summary>
        None,
        /// <summary>
        /// Image is stretched within the paint rectangle.
        /// </summary>
        Stretch,
        /// <summary>
        /// Image is stretched by the X axis and tiled by the Y one.
        /// </summary>
        StretchXTileY,
        /// <summary>
        /// Image is stretched by the Y axis and tiled by the X one.
        /// </summary>
        StretchYTileX,
        /// <summary>
        /// Inner image segment is tiled while all others are stretched.
        /// </summary>
        StretchXYTileInner,
        /// <summary>
        /// Image is centered within the paint rectangle.
        /// </summary>
        Center,
        /// <summary>
        /// Image is centered by the X axis and stretched by the Y one.
        /// </summary>
        CenterXStretchY,
        /// <summary>
        /// Image is centered by the Y axis and stretched by the X one.
        /// </summary>
        CenterYStretchX,
        /// <summary>
        /// Image is centered by the X axis and tiled by the Y one.
        /// </summary>
        CenterXTileY,
        /// <summary>
        /// Image is centered by the Y axis and tiled by the X one.
        /// </summary>
        CenterYTileX,
        /// <summary>
        /// Image is tiled within the paint rectangle.
        /// </summary>
        Tile,
        /// <summary>
        /// Image is flipped by the X axis and tiled within the paint rectangle.
        /// </summary>
        TileFlipX,
        /// <summary>
        /// Image is flipped by the X and Y axis and tiled within the paint rectangle.
        /// </summary>
        TileFlipXY,
        /// <summary>
        /// Image is flipped by the Y axis and tiled within the paint rectangle.
        /// </summary>
        TileFlipY,
    }
}
