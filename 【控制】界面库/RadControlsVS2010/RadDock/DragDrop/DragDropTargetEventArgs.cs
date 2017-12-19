using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines method signature to handle a <see cref="DragDropService.PreviewDropTarget">PreviewDropTarget</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DragDropTargetEventHandler(object sender, DragDropTargetEventArgs e);
    /// <summary>
    /// Represents the arguments, associated with a <see cref="DragDropService.PreviewDropTarget">PreviewDropTarget</see> event.
    /// </summary>
    public class DragDropTargetEventArgs : EventArgs
    {
        #region Fields

        private SplitPanel dropTarget;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropTargetEventArgs">DragDropTargetEventArgs</see> class.
        /// </summary>
        /// <param name="target"></param>
        public DragDropTargetEventArgs(SplitPanel target)
        {
            this.dropTarget = target;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently hit-tested drop target.
        /// </summary>
        public SplitPanel DropTarget
        {
            get
            {
                return this.dropTarget;
            }
            set
            {
                this.dropTarget = value;
            }
        }

        #endregion
    }
}
