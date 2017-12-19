using System;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class DetailListViewDragDropService : RadDragDropService
    {
        #region Fields

        private ListViewDetailColumn draggedColumn;
        private DetailListViewElement owner;
        private RadLayeredWindow dragHintWindow;

        #endregion

        #region Constructor

        public DetailListViewDragDropService(DetailListViewElement owner)
        {
            this.owner = owner; 
        }

        #endregion

        #region Overrides

        protected override void HandleMouseMove(Point mousePos)
        {
            ISupportDrop dropTarget = this.DropTarget;

            base.HandleMouseMove(mousePos);

            if (dropTarget != this.DropTarget && this.dragHintWindow == null && this.DropTarget.AllowDrop)
            {
                this.PrepareDragHint();
            }
             
            this.UpdateDragHintLocation(mousePos);
        }

        protected override void PerformStart()
        {
            base.PerformStart();

            DetailListViewCellElement draggedColumn = this.Context as DetailListViewCellElement;
            if (draggedColumn == null)
            {
                this.Stop(false);
                return;
            }

            this.draggedColumn = draggedColumn.Data;
            this.PrepareDragHint();
        }

        protected override void PerformStop()
        {
            base.PerformStop();
            this.DisposeDragHint();
        }

        protected override void OnPreviewDragOver(RadDragOverEventArgs e)
        {
            base.OnPreviewDragOver(e);
            if (!e.CanDrop)
            {
                this.DisposeDragHint();
            }
        }

        protected override void OnPreviewDragDrop(RadDropEventArgs e)
        { 
            DetailListViewCellElement targetElement = e.HitTarget as DetailListViewCellElement;

            ListViewDetailColumn targetColumn = targetElement.Data;

            int draggedIndex = owner.Owner.Columns.IndexOf(draggedColumn);
            
            int targetIndex = e.DropLocation.X <= targetElement.Size.Width / 2 ? 
                owner.Owner.Columns.IndexOf(targetColumn):
                owner.Owner.Columns.IndexOf(targetColumn) + 1;

            owner.Owner.Columns.Remove(draggedColumn);
            if (draggedIndex < targetIndex)
            {
                targetIndex--;
            }

            owner.Owner.Columns.Insert(targetIndex, draggedColumn);            

            owner.InvalidateMeasure(true);

            base.OnPreviewDragDrop(e);
        }

        protected override bool IsDropTargetValid(ISupportDrop dropTarget)
        {
            DetailListViewCellElement targetCellElement = dropTarget as DetailListViewCellElement;

            if (targetCellElement != null)
            {
                return targetCellElement.Data != this.draggedColumn;
            }

            return base.IsDropTargetValid(dropTarget);
        }

        #endregion

        #region Virtual Methods

        protected virtual void UpdateDragHintLocation(Point mousePosition)
        {
            if (!this.CanCommit)
            {
                if (this.dragHintWindow != null)
                {
                    this.dragHintWindow.Visible = false;
                }

                return;
            }

            RadElement dropTargetElement = this.DropTarget as RadElement;
            Rectangle itemBounds = this.owner.ElementTree.Control.RectangleToScreen(dropTargetElement.ControlBoundingRectangle);
            Rectangle rowBounds = this.owner.ElementTree.Control.RectangleToScreen(owner.ColumnContainer.ControlBoundingRectangle);

            Size imageSize = this.owner.ColumnDragHint.Image.Size;
            Padding margins = this.owner.ColumnDragHint.Margins;

            Point client = dropTargetElement.PointFromScreen(mousePosition);
            int x = (client.X < dropTargetElement.Size.Width / 2) ? itemBounds.X : itemBounds.Right;

            Point hintLocation = new Point(x - imageSize.Width / 2, rowBounds.Y - margins.Top);

            this.dragHintWindow.ShowWindow(hintLocation);
        }

        protected virtual void PrepareDragHint()
        {
            this.dragHintWindow = new RadLayeredWindow();
            if(this.owner.ColumnDragHint != null)
            {
                Size hintSize = this.GetDragHintSize(this.DropTarget);
                Bitmap image = new Bitmap(hintSize.Width, hintSize.Height);
                Graphics temp = Graphics.FromImage(image);
                this.owner.ColumnDragHint.Paint(temp, new RectangleF(PointF.Empty, hintSize));
                temp.Dispose();
   
                this.dragHintWindow.BackgroundImage = image;
            }
        }

        protected virtual Size GetDragHintSize(ISupportDrop iSupportDrop)
        {
            int width = this.owner.ColumnDragHint.Image.Size.Width;
            int height = this.owner.ColumnContainer.Size.Height;

            Padding margins = this.owner.ColumnDragHint.Margins;
            height += margins.Vertical;
            width += margins.Horizontal;

            return new Size(width, height);
        }

        protected virtual void DisposeDragHint()
        {
            if (this.dragHintWindow != null)
            {
                this.dragHintWindow.Dispose();
                this.dragHintWindow = null;
            }
        }

        #endregion
    }
}
