using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ColumnResizingBehavior
    {
        #region Fields

        protected ListViewDetailColumn resizedColumn = null;
        protected bool isResizing = false;
        protected bool allowColumnResize = true;
        protected RadListViewElement owner;
        protected Point lastMousePosition = Point.Empty;

        #endregion

        #region Ctor

        public ColumnResizingBehavior(RadListViewElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties

        public RadListViewElement Owner
        {
            get
            {
                return owner;
            }
        }

        public bool AllowColumnResize
        {
            get
            {
                return allowColumnResize;
            }
            set
            {
                allowColumnResize = value;
            }
        }

        public ListViewDetailColumn ResizedColumn
        {
            get
            {
                return resizedColumn;
            }
        }

        public bool IsResizing
        {
            get
            {
                return isResizing;
            }
        }
        
        #endregion

        #region Public Methods

        public virtual bool BeginResize(ListViewDetailColumn column, Point mousePosition)
        {
            if (!allowColumnResize || this.isResizing || this.owner.ViewType != ListViewType.DetailsView)
            {
                return false;
            }

            this.owner.Capture = true;
            this.lastMousePosition = mousePosition;
            isResizing = true;
            resizedColumn = column;
            this.owner.ElementTree.Control.Cursor = Cursors.SizeWE;

            return true;
        }

        public virtual bool Resize(int offset)
        {
            if (this.isResizing && this.resizedColumn != null)
            {
                float oldWidth = this.resizedColumn.Width;
                this.resizedColumn.Width += offset;

                return oldWidth != this.resizedColumn.Width;
            }

            return false;
        }

        public virtual void HandleMouseMove(Point mousePosition)
        {
            if (!this.IsResizing)
            {
                return;
            }

            int offset = mousePosition.X - lastMousePosition.X;

            if (this.Resize(offset))
            {
                lastMousePosition.X = mousePosition.X;
            }
        }

        public virtual void EndResize()
        {
            this.owner.Capture = false;
            this.isResizing = false;
            this.resizedColumn = null;
            this.owner.ElementTree.Control.Cursor = Cursors.Default;
        }

        #endregion

    }
}
