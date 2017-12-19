using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadPageViewTabStripElement : RadPageViewStripElement
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();
            base.ItemDragService = new RadPageViewInDockDragDropService(this);
            base.ItemDragMode = PageViewItemDragMode.Preview;
        }

        protected internal override void OnItemMouseDown(RadPageViewItem sender, MouseEventArgs e)
        {
            if ((e.Button == base.ActionMouseButton || e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) && e.Clicks == 1)
            {
                if (!sender.IsSelected)
                {
                    base.SelectItem(sender);
                }
                else if (base.EnsureSelectedItemVisible)
                {
                    base.EnsureItemVisible(sender);
                }
            }
        }

        protected override bool ProcessDragOver(Point mousePosition, ISupportDrag dragObject)
        {
            RadPageViewItem dragItem = dragObject as RadPageViewItem;
            if (dragItem == null || dragItem.Owner != this)
            {
                return false;
            }

            Point mousePt = this.GetMousePosition(mousePosition);
            RadPageViewItem hitItem = this.ItemFromPoint(mousePt);
            if (!this.CanDropOverItem(dragItem, hitItem))
            {
                return false;
            }

            this.EnsureItemVisible(hitItem);
            this.ItemsParent.UpdateLayout();

            return hitItem.ControlBoundingRectangle.Contains(mousePt);
        }

        protected override void ProcessDragDrop(Point dropLocation, ISupportDrag dragObject)
        {
            RadPageViewItem dragItem = dragObject as RadPageViewItem;
            if (dragItem == null)
            {
                return;
            }

            Point mousePt = this.GetMousePosition(dropLocation);
            RadPageViewItem hitItem = this.ItemFromPoint(mousePt);
            if (hitItem != null)
            {
                this.PerformItemDrop(dragItem, hitItem);
            }
        }

        private Point GetMousePosition(Point mousePosition)
        {
            Point mousePt = Control.MousePosition;
            if (this.IsInValidState(true))
            {
                mousePt = this.ElementTree.Control.PointToClient(mousePt);
            }
            else
            {
                mousePt = mousePosition;
            }

            return mousePt;
        }
    }
}