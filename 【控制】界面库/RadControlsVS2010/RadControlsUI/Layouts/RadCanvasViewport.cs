using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{

    /// <summary>Defines scrolling states.</summary>
    public enum ScrollState
    {
        AutoHide,
        AlwaysShow,
        AlwaysHide
    }
    ///<exclude/>
    public class RadCanvasViewport : RadElement, IRadScrollViewport
    {
        #region Properties
        private bool canvasSizeInvalidated;
        private Size canvasSize;
        public Size CanvasSize
        {
            get
            {
                if (this.canvasSizeInvalidated)
                {
                    this.canvasSizeInvalidated = false;
                    this.canvasSize = CalcCanvasSize();
                }
                return this.canvasSize;
            }
            set
            {
#if DEBUG
                this.canvasSize = value;
                this.canvasSizeInvalidated = false;
#endif
            }
        }

        protected virtual Size CalcCanvasSize()
        {
            Rectangle childrenRect = Rectangle.Empty;
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                if (child.AutoSize == false || child.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize)
                {
                    if (childrenRect.IsEmpty)
                        childrenRect = child.FullBoundingRectangle;
                    else
                        childrenRect = Rectangle.Union(childrenRect, child.FullBoundingRectangle);
                }
            }
            Size res = new Size(childrenRect.Left + childrenRect.Width,
                childrenRect.Top + childrenRect.Height);
            return res;
        }
        #endregion

        #region IScrollViewport implementation
        public Size GetExtentSize()
        {
            return this.CanvasSize;
        }

        public void InvalidateViewport()
        {
            this.canvasSizeInvalidated = true;
        }

        public Point ResetValue(Point currentValue, Size viewportSize, Size canvasSize)
        {
            Point res = Point.Empty;
            res.X = ValidatePosition(currentValue.X,
                canvasSize.Width - viewportSize.Width);
            res.Y = ValidatePosition(currentValue.Y,
                canvasSize.Height - viewportSize.Height);
            return res;
        }

        internal static int ValidatePosition(int currentPosition, int maxPosition)
        {
            int res = currentPosition;
            if (res > maxPosition)
                res = maxPosition;
            if (res < 0)
                res = 0;
            return res;
        }

        // Calculate the minimum offset necessary for scr to be moved in dest.
        // All you need is call src.Offset() with the result of this function
        internal static Size CalcMinOffset(Rectangle src, Rectangle dest)
        {
            Size res = Size.Empty;
            int lastOffset = 0;

            res.Width = dest.Left - src.Left;
            lastOffset = dest.Right - src.Right;
            if (Math.Sign(res.Width) * Math.Sign(lastOffset) == 1)
            {
                if (Math.Abs(res.Width) > Math.Abs(lastOffset))
                    res.Width = lastOffset;
            }
            else
            {
                res.Width = 0;
            }

            res.Height = dest.Top - src.Top;
            lastOffset = dest.Bottom - src.Bottom;
            if (Math.Sign(res.Height) * Math.Sign(lastOffset) == 1)
            {
                if (Math.Abs(res.Height) > Math.Abs(lastOffset))
                    res.Height = lastOffset;
            }
            else
            {
                res.Height = 0;
            }

            return res;
        }

        public virtual void DoScroll(Point oldValue, Point newValue)
        {
            this.PositionOffset = new SizeF(-newValue.X, -newValue.Y);
        }

        public virtual Size ScrollOffsetForChildVisible(RadElement childElement, Point currentScrollValue)
        {
            Rectangle clientRect = new Rectangle(Point.Empty, this.Size);
            Rectangle childRect = childElement.FullBoundingRectangle;
            childRect.Offset((int)Math.Round(this.PositionOffset.Width), (int)Math.Round(this.PositionOffset.Height));
            Size childOffset = CalcMinOffset(childRect, clientRect);
            Size viewportOffset = new Size(-childOffset.Width, -childOffset.Height);
            return viewportOffset;
        }

        public ScrollPanelParameters GetScrollParams(Size viewportSize, Size canvasSize)
        {
            return new ScrollPanelParameters(
                0, Math.Max(1, canvasSize.Width), 1, Math.Max(1, viewportSize.Width),
                0, Math.Max(1, canvasSize.Height), 1, Math.Max(1, viewportSize.Height)
            );
        }
        #endregion

        #region Construction

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.canvasSizeInvalidated = true;
        }

        #endregion

        #region Overrides
        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
            {
                return this.CanvasSize;
            }
            else
                return base.GetPreferredSizeCore(proposedSize);
        }

        protected override void CreateChildElements()
        {
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            canvasSizeInvalidated = true;
        }
        #endregion
    }
}
