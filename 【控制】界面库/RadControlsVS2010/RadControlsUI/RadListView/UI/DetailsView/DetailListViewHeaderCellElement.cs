
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System;
namespace Telerik.WinControls.UI
{
    public class DetailListViewHeaderCellElement : DetailListViewCellElement
    {
        #region Fields

        protected ArrowPrimitive arrow;
        private const int resizePointerOffset = 4;

        #endregion

        #region Constructors

        public DetailListViewHeaderCellElement(ListViewDetailColumn column)
            : base(column)
        { }

        #endregion

        #region Virtual Methods

        protected virtual void BeginDragDrop()
        {
            if (!this.Data.Owner.AllowColumnReorder)
            {
                return;
            }

            (this.Data.Owner.ViewElement as DetailListViewElement).ColumnDragDropService.Start(this);
        }

        public virtual void BeginResize(Point mousePosition)
        {
            this.Data.Owner.ColumnResizingBehavior.BeginResize(this.Data, mousePosition);
        }

        public virtual bool IsInResizeLocation(Point point)
        {
            if (!this.Data.Owner.AllowColumnResize)
            {
                return false;
            }

            return (point.X >= this.ControlBoundingRectangle.X && point.X <= this.ControlBoundingRectangle.X + resizePointerOffset)
                   || (point.X >= this.ControlBoundingRectangle.Right - resizePointerOffset && point.X <= this.ControlBoundingRectangle.Right);
        }

        #endregion

        #region Overrides

        public override void Synchronize()
        {
            if (column.Owner != null)
            {
                int descriptorIndex = column.Owner.SortDescriptors.IndexOf(column.Name);

                if (descriptorIndex >= 0)
                {
                    this.arrow.Direction = column.Owner.SortDescriptors[descriptorIndex].Direction == ListSortDirection.Ascending ?
                    ArrowDirection.Up :
                    ArrowDirection.Down;
                }
            }

            base.Synchronize();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            arrow = new ArrowPrimitive();
            arrow.Alignment = ContentAlignment.TopCenter;
            arrow.Margin = new Padding(0, 1, 0, 0);
            this.Children.Add(arrow);
        }
         
        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e); 

            this.SetValue(IsMouseDownProperty, false);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Data.Owner.EndEdit(); 

            if (IsInResizeLocation(e.Location))
            {
                this.CallDoMouseUp(e);
                if (e.X <= this.ControlBoundingRectangle.X + resizePointerOffset)
                {
                    int index = this.Parent.Children.IndexOf(this);
                    if (index > 0)
                    {
                        DetailListViewHeaderCellElement previousCell = this.Parent.Children[index - 1] as DetailListViewHeaderCellElement;
                        if (previousCell != null)
                        {
                            previousCell.BeginResize(e.Location);
                        }
                    }
                }
                else
                {
                    this.BeginResize(e.Location);
                }
            }
            else if(e.Button == MouseButtons.Left)
            {
                BeginDragDrop();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.Data.Owner.EnableSorting && this.Data.Owner.EnableColumnSort && 
                !this.IsInResizeLocation(e.Location) && e.Button == MouseButtons.Left)
            {
                int descriptorIndex = column.Owner.SortDescriptors.IndexOf(column.Name);
                ListSortDirection sortDirection = ListSortDirection.Ascending;
                bool shouldSort = true;

                if (descriptorIndex >= 0)
                {
                    sortDirection = column.Owner.SortDescriptors[descriptorIndex].Direction;
                    if (sortDirection == ListSortDirection.Ascending)
                    {
                        sortDirection = ListSortDirection.Descending;
                    }
                    else
                    {
                        shouldSort = false;
                    }
                }
                this.Data.Owner.SortDescriptors.BeginUpdate();
                this.Data.Owner.SortDescriptors.Clear();

                if (shouldSort)
                {
                    this.Data.Owner.SortDescriptors.Add(this.Data.Name, sortDirection);
                }

                this.Data.Owner.SortDescriptors.EndUpdate();
                this.Synchronize();
            }
        }

        protected override bool ProcessDragOver(Point mousePosition, ISupportDrag dragObject)
        {
            return this.Data.Owner.AllowColumnReorder;
        }
         
        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);
            return new SizeF(this.Data.Width, Math.Max(desiredSize.Height, this.Data.Owner.HeaderHeight));
        }

        #endregion
    }
}
