using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <summary>
    /// Exposes methods for drop targets
    /// </summary>
    public interface ISupportDrop
    {
        /// <summary>
        /// Determines whether the instance allows for drop operations.
        /// </summary>
        bool AllowDrop
        {
            get;
        }

        /// <summary>
        /// Completes drag-drop operation of instance of the IDraggable over the specified target.
        /// </summary>
        /// <param name="dropLocation">An instance of <see cref="System.Drawing.Point"/> which represents a drop location.</param>
        /// <param name="dragObject">An instance of the IDraggable which is dragged over the target.</param>
        void DragDrop(Point dropLocation, ISupportDrag dragObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentMouseLocation">The current position of the mouse cursor</param>
        /// <param name="dragObject">An instance of the IDraggable which is dragged over the specified target.</param>
        /// <returns>True if the operation finished successfully, otherwise false.</returns>
        bool DragOver(Point currentMouseLocation, ISupportDrag dragObject);

        /// <summary>
        /// Drop operations to occur in the drop target. Called when the cursor first enters the specified target.
        /// </summary>
        /// <param name="currentMouseLocation">The current position of the mouse cursor</param>
        /// <param name="dragObject">An instance of the IDraggable which is dragged over the target.</param>
        void DragEnter(Point currentMouseLocation, ISupportDrag dragObject);

        /// <summary>
        /// Special behavior when the drag operation leaves the specified target.
        /// </summary>
        /// <param name="oldMouseLocation">The old position of the mouse cursor</param>
        /// <param name="dragObject">An instance of the IDraggable which is dragged over the target.</param>
        void DragLeave(Point oldMouseLocation, ISupportDrag dragObject);
    }
}
