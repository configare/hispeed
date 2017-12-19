using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    internal class PageViewItemSizeInfo
    {
        #region Fields

        public RadPageViewItem item;
        public bool verticalOrientation;
        public bool verticalAlign;
        public SizeF desiredSize;
        public SizeF minSize;
        public SizeF maxSize;
        public SizeF layoutSize;
        public SizeF currentSize;
        public RectangleF itemRectangle;

        public float desiredLength;
        public float minLength;
        public float maxLength;
        public float layoutLength;
        public float currentLength;
        public int marginLength;

        public int itemIndex;

        #endregion

        #region Ctor

        public PageViewItemSizeInfo(RadPageViewItem item, bool vertical)
        {
            this.item = item;
            this.verticalAlign = vertical;
            this.verticalOrientation = item.ContentOrientation == PageViewContentOrientation.Vertical90 || 
                item.ContentOrientation == PageViewContentOrientation.Vertical270;

            this.desiredSize = item.DesiredSize;
            this.layoutSize = this.desiredSize;
            this.minSize = item.MinSize;
            this.maxSize = item.MaxSize;
            this.currentSize = item.CurrentSize;
            if (this.currentSize == Size.Empty)
            {
                this.currentSize = this.desiredSize;
            }

            this.desiredLength = this.GetLength(this.desiredSize);
            this.currentLength = this.GetLength(this.currentSize);
            this.layoutLength = this.desiredLength;

            this.minLength = this.verticalOrientation ? this.minSize.Height : this.minSize.Width;
            this.maxLength = this.verticalOrientation ? this.maxSize.Height : this.maxSize.Width;
            this.marginLength = this.GetItemMarginLength(item);
        }

        #endregion

        #region Methods

        protected virtual int GetItemMarginLength(RadPageViewItem item)
        {
            if (this.verticalAlign)
            {
                return item.AutoFlipMargin ? item.Margin.Horizontal : item.Margin.Vertical;
            }
            else
            {
                return item.Margin.Horizontal;
            }
        }

        public void SetLayoutSize(SizeF newSize)
        {
            this.layoutSize = newSize;
            this.layoutLength = this.GetLength(newSize);
        }

        public void SetCurrentSize(SizeF newSize)
        {
            this.currentSize = newSize;
            this.currentLength = this.GetLength(newSize);
        }

        public void ChangeLayoutLength(int newLength)
        {
            this.layoutLength = newLength;

            if (this.verticalAlign)
            {
                this.layoutSize.Height = newLength;
            }
            else
            {
                this.layoutSize.Width = newLength;
            }
        }

        private float GetLength(SizeF size)
        {
            return this.verticalAlign ? size.Height : size.Width;
        }

        #endregion
    }
}
