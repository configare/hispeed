using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines method signature to handle a <see cref="DragDropService.PreviewHitTest">PreviewHitTest</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DragDropHitTestEventHandler(object sender, DragDropHitTestEventArgs e);
    /// <summary>
    /// Represents the arguments, associated with a <see cref="DragDropService.PreviewDropTarget">PreviewDropTarget</see> event.
    /// </summary>
    public class DragDropHitTestEventArgs : EventArgs
    {
        #region Fields

        private DockingGuideHitTest hitTest;
        private SplitPanel dropTarget;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropHitTestEventArgs">DragDropHitTestEventArgs</see> class.
        /// </summary>
        /// <param name="dropTarget"></param>
        /// <param name="hitTest"></param>
        public DragDropHitTestEventArgs(SplitPanel dropTarget, DockingGuideHitTest hitTest) 
        {
            this.hitTest = hitTest;
            this.dropTarget = dropTarget;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently generated hit-test result.
        /// </summary>
        public DockingGuideHitTest HitTest
        {
            get
            {
                return this.hitTest;
            }
            set
            {
                this.hitTest = value;
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

        #endregion
    }
}
