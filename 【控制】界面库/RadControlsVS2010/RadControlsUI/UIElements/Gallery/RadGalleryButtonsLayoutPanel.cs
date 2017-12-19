using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadGalleryButtonsLayoutPanel : LayoutPanel
    {
        public static RadProperty ChildrenWidthProperty = RadProperty.Register("ChildrenWidth", typeof(int), typeof(RadGalleryButtonsLayoutPanel),
            new RadElementPropertyMetadata(16, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public int ChildrenWidth
        {
            get
            {
                return (int)this.GetValue(ChildrenWidthProperty);
            }
            set
            {
                this.SetValue(ChildrenWidthProperty, value);
            }
        }

        public override void PerformLayoutCore(RadElement affectedElement)
        {
            if (this.Children.Count == 0)
                return;
            int childrenHeight = this.AvailableSize.Height / this.Children.Count;
            for (int i = 0; i <= this.Children.Count - 1; i++)
            {
                RadElement child = this.Children[i];
                Size childSize = child.GetPreferredSize(this.AvailableSize);
                child.Bounds = new Rectangle(0, i * childrenHeight, this.ChildrenWidth, childrenHeight);
            }
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (this.Children.Count == 0)
            {
                return finalSize;
            }

         //   float buttonWidthCoef = 1.8f;
            float childrenHeight = finalSize.Height / this.Children.Count;
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
               // SizeF childSize = child.DesiredSize;
                child.Arrange(new RectangleF(0,
                              i * childrenHeight,
                              child.DesiredSize.Width,
                              childrenHeight));
            }

            return finalSize;
        }
    }
}
