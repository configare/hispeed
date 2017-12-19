using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class PreviewDragContextEventArgs : RadDragEventArgs
    {
        #region Fields

        private object dragContext;

        #endregion

        #region Constructor

        public PreviewDragContextEventArgs(ISupportDrag instance)
            : base(instance)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the context associated with a drag operation.
        /// </summary>
        public object Context
        {
            get
            {
                return this.dragContext;
            }
            set
            {
                this.dragContext = value;
            }
        }

        #endregion
    }
}
