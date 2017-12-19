using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <summary>
    /// Exposes methods and properties for draggable elements.
    /// </summary>
    public interface ISupportDrag
    {
        /// <summary>
        /// Determines that the element is availble for dragging.
        /// </summary>
        /// <param name="dragStartPoint">An instance of <see cref="System.Drawing.Point"/> which represents a dragging start location.</param>
        /// <returns>True if the object can be dragged, otherwise false.</returns>
        bool CanDrag(Point dragStartPoint);

        /// <summary>
        /// Gets the assosiated with dragged element data context.
        /// </summary>
        /// <returns></returns>
        object GetDataContext();

        /// <summary>
        /// Gets the image used by the DragDropService to indicate that the element is being dragged.
        /// Usually this is a snapshot of the element itself.
        /// </summary>
        /// <returns></returns>
        Image GetDragHint();

        /// <summary>
        /// Determines whether this instance may enter drag operation.
        /// </summary>
        bool AllowDrag
        {
            get;
            set;
        }
    }
}
