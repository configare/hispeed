using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines method signature to handle a <see cref="DragDropService.PreviewDockPosition">PreviewDockPosition</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DragDropDockPositionEventHandler(object sender, DragDropDockPositionEventArgs e);
    /// <summary>
    /// Represents the arguments, associated with a <see cref="DragDropService.PreviewDockPosition">PreviewDockPosition</see> event.
    /// </summary>
    public class DragDropDockPositionEventArgs : EventArgs
    {
        #region Fields

        private AllowedDockPosition allowedDockPosition;
        private SplitPanel dropTarget;
        private DockingGuidesPosition guidePosition;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropDockPositionEventArgs">DragDropDockPositionEventArgs</see> class.
        /// </summary>
        /// <param name="dropTarget"></param>
        /// <param name="position"></param>
        /// <param name="guidePosition"></param>
        public DragDropDockPositionEventArgs(SplitPanel dropTarget, AllowedDockPosition position, DockingGuidesPosition guidePosition)
        {
            this.dropTarget = dropTarget;
            this.allowedDockPosition = position;
            this.guidePosition = guidePosition;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the allowed dock position for the hit-tested drop target.
        /// </summary>
        public AllowedDockPosition AllowedDockPosition
        {
            get
            {
                return this.allowedDockPosition;
            }
            set
            {
                this.allowedDockPosition = value;
            }
        }

        /// <summary>
        /// Gets the current drop target of the drag-drop operation.
        /// </summary>
        public SplitPanel DropTarget
        {
            get
            {
                return this.dropTarget;
            }
        }

        /// <summary>
        /// Gets the position of the docking guide that is currently hit-tested.
        /// </summary>
        public DockingGuidesPosition GuidePosition
        {
            get
            {
                return this.guidePosition;
            }
        }

        #endregion
    }
}
