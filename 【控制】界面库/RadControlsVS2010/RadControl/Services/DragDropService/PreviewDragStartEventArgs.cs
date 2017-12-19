using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class PreviewDragStartEventArgs : RadDragEventArgs
    {
        #region Fields

        private bool canStart;

        #endregion

        #region Constructor

        public PreviewDragStartEventArgs(ISupportDrag instance)
            : base(instance)
        {
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Determines whether a drag operation may start.
        /// </summary>
        public bool CanStart
        {
            get
            {
                return this.canStart;
            }
            set
            {
                this.canStart = value;
            }
        }

        #endregion
    }
}
