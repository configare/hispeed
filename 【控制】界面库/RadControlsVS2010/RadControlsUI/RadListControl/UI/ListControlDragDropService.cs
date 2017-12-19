using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ListControlDragDropService : RadDragDropService
    {
        private RadListElement owner;
        private RadLayeredWindow dropHintWindow;

        public ListControlDragDropService(RadListElement owner)
        {
            this.owner = owner;            
        }

        protected virtual void PrepareDragHint(RadListElement listElement)
        {          
            Bitmap image = null;
            Size hintSize = Size.Empty;
            hintSize = new Size(this.GetDropHintWidth(listElement), 1);
            image = new Bitmap(hintSize.Width, hintSize.Height);

            using (Graphics temp = Graphics.FromImage(image))
            {
                using (SolidBrush solidBrush = new SolidBrush(Color.Red))
                {
                    temp.FillRectangle(solidBrush, new Rectangle(Point.Empty, hintSize));
                }
            }

          
            if (image != null)
            {
                this.dropHintWindow = new RadLayeredWindow();
                this.dropHintWindow.BackgroundImage = image;
            }
        }

        private int GetDropHintWidth(RadListElement listElement)
        {
            int width = listElement.ControlBoundingRectangle.Width - listElement.GetBorderThickness(true).Horizontal;

            if (listElement.VScrollBar.Visibility == ElementVisibility.Visible)
            {
                width -= listElement.VScrollBar.Size.Width;
            }

            return width;
        }

        protected override bool CanStart(object context)
        {
            return base.CanStart(context) && this.owner.AllowDrag;
        }

        protected override void OnPreviewDragHint(PreviewDragHintEventArgs e)
        {
            base.OnPreviewDragHint(e);
        }

        protected override void OnPreviewDragOver(RadDragOverEventArgs e)
        {
            RadListElement hitTarget = e.HitTarget as RadListElement;
            e.CanDrop = hitTarget != null && hitTarget.DataSource == null;
        }

        protected override void OnPreviewDragDrop(RadDropEventArgs e)
        {
            RadListVisualItem dragedItem = e.DragInstance as RadListVisualItem;
            RadListElement targetList = e.HitTarget as RadListElement;
            RadListVisualItem targetElement = targetList.ElementTree.GetElementAtPoint(e.DropLocation) as RadListVisualItem;
            this.OnPreviewDragDropCore(dragedItem, targetList, targetElement);

            base.OnPreviewDragDrop(e);
        }

        protected virtual void OnPreviewDragDropCore(RadListVisualItem dragedItem, RadListElement targetList, RadListVisualItem targetElement)
        {
            int index = targetList.Items.IndexOf(targetElement.Data);
            Debug.Assert(index != -1);

            RadListElement dragedListView = dragedItem.Data.Owner;

            IList<RadListDataItem> itemsToMove = new List<RadListDataItem>(dragedListView.SelectedItems.Count);

            foreach (RadListDataItem item in dragedListView.SelectedItems)
            {
                itemsToMove.Add(item);
            }

            dragedListView.BeginUpdate();
            foreach (RadListDataItem item in itemsToMove)
            {
                dragedListView.Items.Remove(item);
            }

            dragedListView.EndUpdate();
            targetList.BeginUpdate();            
            foreach (RadListDataItem item in itemsToMove)
            {
                targetList.Items.Insert(index++, item);
            }

            targetList.EndUpdate();
        }


        protected override void PerformStop()
        {
            base.PerformStop();
            this.DisposeHint();
        }

        protected override void HandleMouseMove(Point mousePosition)
        {
            base.HandleMouseMove(mousePosition);
            if (this.DropTarget == null)
            {
                return;
            }

            RadListElement nodeElement = this.DropTarget as RadListElement;

            if (nodeElement == null || !this.CanShowDropHint(mousePosition) || !this.CanCommit)
            {
                this.DisposeHint();
                return;
            }

            if (this.dropHintWindow==null)
            {
                this.PrepareDragHint(nodeElement);
            }

            if (this.dropHintWindow != null)
            {
                this.UpdateHintPosition(mousePosition);
            }
        }

        protected virtual void DisposeHint()
        {
            if (this.dropHintWindow != null)
            {
                this.dropHintWindow.Dispose();
                this.dropHintWindow = null;
            }
        }

        protected virtual void UpdateHintPosition(Point mousePosition)
        {
            RadListElement nodeElement = this.DropTarget as RadListElement;
            RadListVisualItem item = nodeElement.ElementTree.GetElementAtPoint(nodeElement.ElementTree.Control.PointToClient(mousePosition)) as RadListVisualItem;
            if (item == null)
            {
                return;
            }

            Rectangle itemBounds = nodeElement.ElementTree.Control.RectangleToScreen(item.ControlBoundingRectangle);
            Padding margins = Padding.Empty;
            int imageHeight = 1;

            //if (this.DropHintColor == Color.Empty)
            //{
            //    RadImageShape imageShape = this.owner.ItemDropHint;
            //    imageHeight = imageShape.Image.Size.Height;
            //    margins = imageShape.Margins;
            //}

            Point client = nodeElement.PointFromScreen(mousePosition);
            bool isDropAtTop = client.Y <= nodeElement.Size.Height / 2;
            int y = isDropAtTop ? itemBounds.Y : itemBounds.Bottom;
            Point hitLocation = new Point(itemBounds.X - margins.Left, y - imageHeight / 2);
            this.dropHintWindow.Width = itemBounds.Width;
            this.dropHintWindow.ShowWindow(hitLocation);
        }

        protected virtual bool CanShowDropHint(Point mousePosition)
        {
            RadListElement element = this.DropTarget as RadListElement;
            if (element == null)
            {
                return false;
            }
       
            Point client = element.PointFromScreen(mousePosition);
            int part = element.ItemHeight / 3;
            return client.Y < part || client.Y > part * 2;
        }
    }
}
