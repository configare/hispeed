using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class RadDragDropEventArgs : RadDragEventArgs
    {
        #region Fields

        private ISupportDrop hitTarget;

        #endregion

        #region Constructor

        public RadDragDropEventArgs(ISupportDrag dragInstance, ISupportDrop hitTarget)
            : base(dragInstance)
        {
            this.hitTarget = hitTarget;
        }

        #endregion

        #region Properties

        public ISupportDrop HitTarget
        {
            get
            {
                return this.hitTarget;
            }
        }

        #endregion
    }
}
