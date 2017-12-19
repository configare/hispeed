using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class OutlookViewOverflowGrip : RadPageViewElementBase
    {
        #region Events

        public event OverflowGripDragHandler Dragged;
        internal int DragDelta = 20;
        private Cursor oldCursor;

        #endregion

        #region Methods

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            
            if (oldCursor == null)
            {
                oldCursor = new Cursor(this.ElementTree.Control.Cursor.Handle);
            }
            this.ElementTree.Control.Cursor = Cursors.SizeNS;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.Capture)
            {
                this.ElementTree.Control.Cursor = this.oldCursor;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            this.Capture = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            this.Capture = false;

            if (this.ElementTree.Control.Cursor != this.oldCursor)
            {
                this.ElementTree.Control.Cursor = this.oldCursor;
            }

            base.OnMouseUp(e);
        }

        private Point? lastPoint;
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.Capture)
            {
                Point currentLocation = this.ControlBoundingRectangle.Location;
                bool up = false;
                Point cursorPosition = Control.MousePosition;
                if (this.lastPoint != null)
                {
                    up = this.lastPoint.Value.Y > cursorPosition.Y;
                }

                this.lastPoint = cursorPosition;

                Point mousePointerOnControl = this.ElementTree.Control.PointToClient(cursorPosition);
                OverflowEventArgs args = null;
                if (mousePointerOnControl.Y - currentLocation.Y > 20 && !up)
                {
                    args = new OverflowEventArgs();
                    args.Up = false;
                }
                else if (currentLocation.Y - mousePointerOnControl.Y > 20 && up)
                {
                    args = new OverflowEventArgs();
                    args.Up = true;
                }
                if (args != null && this.Dragged != null)
                {
                    this.Dragged(this, args);
                }
            }
        }


        #endregion
    }

    public delegate void OverflowGripDragHandler(object sender, OverflowEventArgs args);

    public class OverflowEventArgs : EventArgs
    {
        public bool Up = false;
    }
}
