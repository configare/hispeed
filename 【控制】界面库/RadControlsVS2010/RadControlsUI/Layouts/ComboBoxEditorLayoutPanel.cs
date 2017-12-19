using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class ComboBoxEditorLayoutPanel : LayoutPanel
    {
        private RadElement content;
        public RadElement Content
        {
            get
            {
                return this.content;
            }
            set
            {
                if (!Object.ReferenceEquals(value, this.content))
                {
                    if (this.content != null)
                    {
                        this.Children.Remove(this.content);
                    }
                    this.Children.Add(value);
                    this.content = value;
                }
            }
        }

        private RadElement arrowButton;
        public RadElement ArrowButton
        {
            get
            {
                return this.arrowButton;
            }
            set
            {
                if (!Object.ReferenceEquals(value, this.arrowButton))
                {
                    if (this.arrowButton != null)
                    {
                        this.Children.Remove(this.arrowButton);
                    }
                    this.Children.Add(value);
                    this.arrowButton = value;
                }
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF result = SizeF.Empty;
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                child.Measure(availableSize);
                result.Width += child.DesiredSize.Width;
                result.Height = Math.Max(result.Height, child.DesiredSize.Height);
            }
            return result;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF arrowButtonDesiredSize = this.arrowButton != null ? this.arrowButton.DesiredSize : SizeF.Empty;

            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                if (this.arrowButton != null && object.ReferenceEquals(child, this.arrowButton))
                {
                    float arrowLeftPos = this.RightToLeft ? 0 : finalSize.Width - arrowButtonDesiredSize.Width;
                    RectangleF arrowArea = new RectangleF(arrowLeftPos, 0, arrowButtonDesiredSize.Width, finalSize.Height);
                    child.Arrange(arrowArea);
                }
                else if (this.content != null && object.ReferenceEquals(child, this.content))
                {
                    float contentLeftPos = this.RightToLeft ? arrowButtonDesiredSize.Width : 0;
                    RectangleF arrowArea = new RectangleF(contentLeftPos, 0, finalSize.Width - arrowButtonDesiredSize.Width, finalSize.Height);
                    child.Arrange(arrowArea);
                }
                else
                {
                    child.Arrange(new RectangleF(PointF.Empty, finalSize));
                }
            }
            return finalSize;
        }

        public override void PerformLayoutCore(RadElement affectedElement)
        {
            //base.PerformLayoutCore(affectedElement);

            Size proposedSize = this.AvailableSize;// this.Size;
            Size arrowBtnSize = Size.Empty;
            Size contentSize = Size.Empty;

            if (this.arrowButton != null)
            {
                arrowBtnSize = this.arrowButton.GetPreferredSize(proposedSize);
                arrowBtnSize = this.arrowButton.LayoutEngine.CheckSize(arrowBtnSize);
            }
            if (this.content != null)
            {
                Size tempSize = new Size(proposedSize.Width - arrowBtnSize.Width, proposedSize.Height);
                contentSize = this.content.GetPreferredSize(tempSize);
            }

            int height = Math.Max(arrowBtnSize.Height, contentSize.Height + this.content.Margin.Vertical);
            arrowBtnSize.Height = height;
            //contentSize.Height = height;

            if (this.arrowButton != null)
            {
                if (base.RightToLeft == false)
                {
                    this.arrowButton.Bounds = new Rectangle(new Point(content.Size.Width, 0), arrowBtnSize);
                }
                else
                {
                    this.arrowButton.Bounds = new Rectangle(Point.Empty, arrowBtnSize);
                }
            }
            if (this.content != null)
            {
                if (base.RightToLeft == false)
                {
                    this.content.Bounds = new Rectangle(Point.Empty, contentSize);
                }
                else
                {
                    this.content.Bounds = new Rectangle(new Point(arrowBtnSize.Width, 0 ), contentSize );
                }
            }
        }

        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
            {
                return this.AvailableSize;
            }

            int maxHeight = 0, width = 0;
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                Size childSize = child.FullSize;
                width += childSize.Width;
                maxHeight = Math.Max(maxHeight, childSize.Height);
            }
            return new Size(width, maxHeight);
        }        
    }
}
