using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI.Properties;

namespace Telerik.WinControls.UI
{
    public class RadPageViewDragDropService : RadDragDropService
    {
        #region Fields

        private RadPageViewElement owner;
        private RadLayeredWindow insertHint;
        private RadPageViewItem dragItem;
        private RadImageShape insertHintImage;
        private static Cursor DefaultValidCursor;

        #endregion

        #region Constructor

        public RadPageViewDragDropService(RadPageViewElement owner)
        {
            this.owner = owner;
            this.ValidCursor = DefaultValidCursor;
        }

        static RadPageViewDragDropService()
        {
            DefaultValidCursor = CursorHelper.CursorFromBitmap(Resources.cursor_move, new Point(1, 1));
        }

        #endregion

        #region Overrides

        protected override void PerformStart()
        {
            this.dragItem = this.Context as RadPageViewItem;
            this.insertHintImage = this.owner.ItemDragHint;

            if (this.owner.ItemDragMode == PageViewItemDragMode.Preview)
            {
                this.PrepareInsertHint();
            }

            base.PerformStart();
        }

        protected override void HandleMouseMove(Point mousePos)
        {
            if (this.owner.ItemDragMode == PageViewItemDragMode.Immediate)
            {
                this.DoImmediateDrag(mousePos);
            }
            else
            {
                base.HandleMouseMove(mousePos);

                if (this.insertHint != null)
                {
                    this.UpdateHintImage(mousePos);
                }
            }
        }

        protected override void PerformStop()
        {
            base.PerformStop();

            this.owner.EndItemDrag(this.dragItem);

            this.dragItem = null;
            this.insertHintImage = null;

            if (this.insertHint != null)
            {
                this.insertHint.Dispose();
                this.insertHint = null;
            }
        }

        #endregion

        #region Implementation

        private void DoImmediateDrag(Point mousePos)
        {
            PageViewLayoutInfo layoutInfo = this.owner.ItemLayoutInfo;
            if(layoutInfo == null)
            {
                return;
            }

            Point client = this.owner.ElementTree.Control.PointToClient(mousePos);
            RadPageViewItem hitItem = this.owner.ItemFromPoint(client);

            if (hitItem == null || hitItem == this.dragItem)
            {
                return;
            }

            RectangleF dragBounds = this.dragItem.ControlBoundingRectangle;
            RectangleF hitBounds = hitItem.ControlBoundingRectangle;
            bool swapItems;

            if (layoutInfo.vertical)
            {
                if (hitBounds.Y > dragBounds.Y)
                {
                    swapItems = client.Y > hitBounds.Bottom - dragBounds.Height;
                }
                else
                {
                    swapItems = client.Y < hitBounds.Y + dragBounds.Height;
                }
            }
            else
            {
                if (hitBounds.X > dragBounds.X)
                {
                    swapItems = client.X > hitBounds.Right - dragBounds.Width;
                }
                else
                {
                    swapItems = client.X < hitBounds.X + dragBounds.Width;
                }
            }

            if (swapItems)
            {
                this.owner.PerformItemDrop(dragItem, hitItem);
                this.owner.UpdateLayout();
            }
        }

        private void PrepareInsertHint()
        {
            if (this.insertHintImage == null || this.insertHintImage.Image == null)
            {
                return;
            }

            PageViewLayoutInfo info = this.owner.ItemLayoutInfo;
            RectangleF itemsRectangle = this.owner.GetItemsRect();
            int length = info.vertical ? (int)itemsRectangle.Size.Width : (int)itemsRectangle.Size.Height;
            if (length <= 0)
            {
                return;
            }

            Size imageSize = this.insertHintImage.Image.Size;
            Size hintSize;
            if (info.vertical)
            {
                hintSize = new Size(length + this.insertHintImage.Margins.Horizontal, imageSize.Height);
            }
            else
            {
                hintSize = new Size(imageSize.Width, length + this.insertHintImage.Margins.Vertical);
            }

            //generate hint image
            Bitmap image = new Bitmap(hintSize.Width, hintSize.Height);
            Graphics temp = Graphics.FromImage(image);
            this.insertHintImage.Paint(temp, new RectangleF(PointF.Empty, hintSize));
            temp.Dispose();

            this.insertHint = new RadLayeredWindow();
            this.insertHint.BackgroundImage = image;
        }

        private void UpdateHintImage(Point mousePos)
        {
            if (!this.CanCommit)
            {
                this.insertHint.Visible = false;
                return;
            }

            Point client = this.owner.ElementTree.Control.PointToClient(mousePos);
            RadPageViewItem item = this.owner.ItemFromPoint(client);
            Rectangle itemBounds = this.owner.ElementTree.Control.RectangleToScreen(item.ControlBoundingRectangle);

            Point hintLocation;
            Size imageSize = this.insertHintImage.Image.Size;
            Padding margins = this.insertHintImage.Margins;

            if (this.owner.ItemLayoutInfo.vertical)
            {
                hintLocation = new Point(itemBounds.X - margins.Left, itemBounds.Y - imageSize.Height / 2);
            }
            else
            {
                hintLocation = new Point(itemBounds.X - imageSize.Width / 2, itemBounds.Y - margins.Top);
            }

            this.insertHint.ShowWindow(hintLocation);
        }

        #endregion
    }
}
