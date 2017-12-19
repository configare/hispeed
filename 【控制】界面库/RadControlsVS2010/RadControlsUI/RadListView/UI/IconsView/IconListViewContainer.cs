using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class IconListViewContainer : BaseListViewContainer
    {
        #region Fields
        private float currentY;
        private float currentX;
        private RectangleF clientRect;
        private float maxItemHeight = 0;
        private float maxItemWidth = 0;
        #endregion

        public IconListViewContainer(BaseListViewElement owner) : base(owner)
        {
        }

        #region Layout

        protected override bool IsItemVisible(ListViewDataItem data)
        {
            return data.Visible;
        }

        protected override bool BeginMeasure(SizeF availableSize)
        {
            this.clientRect = new RectangleF(Padding.Left, Padding.Top,
                                             availableSize.Width - Padding.Horizontal, availableSize.Height - Padding.Vertical);


            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                this.currentX = this.clientRect.X;
                this.currentY = this.clientRect.Y + this.ScrollOffset.Height;
            }
            else
            {
                this.currentX = this.clientRect.X + this.ScrollOffset.Width;
                this.currentY = this.clientRect.Y;
            }

            this.maxItemHeight = 0;
            this.maxItemWidth = 0;
            return base.BeginMeasure(availableSize);
        }

        protected override bool MeasureElement(IVirtualizedElement<ListViewDataItem> element)
        {
            RadElement radElement = element as RadElement;
            if (radElement == null)
            {
                return false;
            }

            SizeF elementDesiredSize = MeasureElementCore(radElement, clientRect.Size);
            
            bool continueMeasure = true;

            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                if (this.currentX + elementDesiredSize.Width > clientRect.Right)
                {
                    this.currentX = this.clientRect.X;
                    this.currentY += this.maxItemHeight;
                    this.maxItemHeight = 0;
                    if (currentY >= clientRect.Bottom)
                    {
                        continueMeasure = false;
                    }
                }

                this.currentX += elementDesiredSize.Width;
                this.maxItemHeight = Math.Max(maxItemHeight, elementDesiredSize.Height);
            }
            else
            {
                if (this.currentY + elementDesiredSize.Height > clientRect.Bottom)
                {
                    this.currentY = this.clientRect.Y;
                    this.currentX += this.maxItemWidth;
                    this.maxItemWidth = 0;
                    if (currentX >= clientRect.Right)
                    {
                        continueMeasure = false;
                    }
                }

                this.currentY += elementDesiredSize.Height;
                this.maxItemWidth = Math.Max(maxItemWidth, elementDesiredSize.Width);
            }
            return continueMeasure;
            
        }

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
            RectangleF clientRect = new RectangleF(Padding.Left, Padding.Top,
                                                   finalSize.Width - Padding.Horizontal, finalSize.Height - Padding.Vertical);

            float x = clientRect.X, y = clientRect.Y;

            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                y += this.ScrollOffset.Height;
            }
            else
            {
                x += this.ScrollOffset.Width;
            }

            float maxHeight = 0;
            float maxWidth = 0;

            foreach (RadElement element in this.Children)
            {
                BaseListViewVisualItem visualItem = element as BaseListViewVisualItem;
                
                if (visualItem == null || visualItem.Data == null)
                    continue;

                Size iconSize = this.ElementProvider.GetElementSize(visualItem.Data).ToSize();
                
                maxHeight = Math.Max(maxHeight, iconSize.Height);
                maxWidth = Math.Max(maxWidth, iconSize.Width);

                if (element is BaseListViewGroupVisualItem)
                {
                    if (x != clientRect.X)
                    {
                        x = clientRect.X;
                        y += maxHeight;
                        maxHeight = iconSize.Height;
                    }

                    element.Arrange(new RectangleF(new PointF(x, y), iconSize));

                    x = clientRect.X;
                    y += iconSize.Height + this.ItemSpacing;
                }
                else
                { 
                    if (x + iconSize.Width > clientRect.Right && this.Orientation == Orientation.Vertical)
                    {
                        x = clientRect.X;
                        y += maxHeight + this.ItemSpacing;
                        maxHeight = iconSize.Height;
                    }
                    else if (y + iconSize.Height > clientRect.Bottom && this.Orientation == Orientation.Horizontal)
                    {
                        y = clientRect.Y;
                        x += maxWidth + this.ItemSpacing;
                        maxWidth = iconSize.Width;
                    }


                    if (x == clientRect.X &&
                        this.owner.Owner.ShowGroups &&
                        (this.owner.Owner.EnableCustomGrouping || this.owner.Owner.EnableGrouping) &&
                        (this.owner.Owner.Groups.Count > 0) &&
                        !this.owner.Owner.FullRowSelect)
                    {
                        x += this.owner.Owner.GroupIndent;
                    }

                    element.Arrange(new RectangleF(new PointF(x, y), iconSize));

                    if (this.Orientation == Orientation.Vertical)
                    {
                        x += iconSize.Width + this.ItemSpacing;
                    }
                    else
                    {
                        y += iconSize.Height + this.ItemSpacing;
                    }
                }
            }

            return finalSize;
        }
        #endregion

        
    }
}