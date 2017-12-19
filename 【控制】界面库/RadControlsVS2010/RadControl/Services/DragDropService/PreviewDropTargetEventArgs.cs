using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class PreviewDropTargetEventArgs : RadDragDropEventArgs
    {
        #region Fields

        private ISupportDrop dropTarget;

        #endregion

        #region Constructor

        public PreviewDropTargetEventArgs(ISupportDrag dragInstance, ISupportDrop hitTarget)
            : base(dragInstance, hitTarget)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the drop target for the operation.
        /// </summary>
        public ISupportDrop DropTarget
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
