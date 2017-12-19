using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class RadDragEventArgs : EventArgs
    {
        #region Fields

        private ISupportDrag dragInstance;

        #endregion

        #region Constructor

        public RadDragEventArgs(ISupportDrag dragInstance)
        {
            this.dragInstance = dragInstance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently dragged instance.
        /// </summary>
        public ISupportDrag DragInstance
        {
            get
            {
                return this.dragInstance;
            }
        }

        #endregion
    }
}

