using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class RadDragOverEventArgs : RadDragDropEventArgs
    {
        #region Fields

        private bool canDrop;

        #endregion

        #region Constructor

        public RadDragOverEventArgs(ISupportDrag dragInstance, ISupportDrop dropTarget)
            : base(dragInstance, dropTarget)
        {
        }

        #endregion

        #region Properties

        public bool CanDrop
        {
            get
            {
                return this.canDrop;
            }
            set
            {
                this.canDrop = value;
            }
        }

        #endregion
    }
}
