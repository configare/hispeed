using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public class RadDropEventArgs : RadDragDropEventArgs
    {
        #region Fields

        private bool handled;
        private Point dropLocation;

        #endregion

        #region Constructor

        public RadDropEventArgs(ISupportDrag dragInstance, ISupportDrop dropTarget, Point dropLocation)
            : base(dragInstance, dropTarget)
        {
            this.dropLocation = dropLocation;
        }

        #endregion

        #region Properties

        public Point DropLocation
        {
            get
            {
                return this.dropLocation;
            }
        }

        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                this.handled = value;
            }
        }

        #endregion
    }
}
