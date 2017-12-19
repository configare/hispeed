using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    public class StackLayoutElement : LightVisualElement
    {
        #region Fields

        private bool fitInAvailableSize;

        #endregion

        #region Nested

        public enum RightToLeftModes
        {
            None,
            ReverseItems,
            ReverseOffset
        }

        #endregion

        #region Constructor & Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.StretchHorizontally = false;
            this.StretchVertically = false;
        }

        #endregion

        #region Properties

        public static RadProperty OrientationProperty = RadProperty.Register(
            "Orientation", typeof(Orientation), typeof(StackLayoutElement),
                new RadElementPropertyMetadata(Orientation.Horizontal,
                    ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        [RadPropertyDefaultValue("Orientation", typeof(StackLayoutElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        public static RadProperty ElementSpacingProperty = RadProperty.Register(
            "ElementSpacing", typeof(int), typeof(StackLayoutElement),
                new RadElementPropertyMetadata(0,
                    ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        [RadPropertyDefaultValue("ElementSpacing", typeof(StackLayoutElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public int ElementSpacing
        {
            get
            {
                return (int)this.GetValue(ElementSpacingProperty);
            }
            set
            {
                this.SetValue(ElementSpacingProperty, value);
            }
        }

        public static RadProperty RightToLeftModeProperty = RadProperty.Register(
                 "RightToLeftMode", typeof(RightToLeftModes), typeof(StackLayoutElement),
                     new RadElementPropertyMetadata(RightToLeftModes.ReverseItems,
                         ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        [RadPropertyDefaultValue("ElementSpacing", typeof(StackLayoutElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public RightToLeftModes RightToLeftMode
        {
            get
            {
                return (RightToLeftModes)this.GetValue(RightToLeftModeProperty);
            }
            set
            {
                this.SetValue(RightToLeftModeProperty, value);
            }
        }

        public bool FitInAvailableSize
        {
            get { return fitInAvailableSize; }
            set
            {
                if (fitInAvailableSize != value)
                {
                    fitInAvailableSize = value;
                    InvalidateMeasure();
                }
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            Padding clientOffset = this.GetClientOffset(true);
            SizeF desiredSize = new SizeF(clientOffset.Horizontal, clientOffset.Vertical);
            availableSize.Width -= clientOffset.Horizontal;
            availableSize.Height -= clientOffset.Vertical;

            lock (this.Children)
            {
                this.layoutManagerPart.Measure(availableSize);
                int spacing = this.ElementSpacing;


                foreach (RadElement element in this.Children)
                {
                    element.Measure(availableSize);
                    if (Orientation == Orientation.Vertical)
                    {
                        desiredSize.Height += element.DesiredSize.Height;
                        desiredSize.Width = Math.Max(desiredSize.Width, element.DesiredSize.Width);
                        if (FitInAvailableSize)
                        {
                            availableSize.Height -= element.DesiredSize.Height + spacing;
                        }
                    }
                    else
                    {
                        desiredSize.Width += element.DesiredSize.Width;
                        desiredSize.Height = Math.Max(desiredSize.Height, element.DesiredSize.Height);
                        if (FitInAvailableSize)
                        {
                            availableSize.Width -= element.DesiredSize.Width + spacing;
                        }
                    }
                }

                if (this.Children.Count > 0)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        desiredSize.Height += spacing * (this.Children.Count - 1) + 1;
                    }
                    else
                    {
                        desiredSize.Width += spacing * (this.Children.Count - 1) + 1;
                    }
                }

                desiredSize.Width += clientOffset.Horizontal;
                desiredSize.Height += clientOffset.Vertical;
            }

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
                        
            this.layoutManagerPart.Arrange(clientRect);

            if (Orientation == Orientation.Vertical)
                ArrangeVertically(finalSize);
            else
                ArrangeHorizontally(finalSize);

            return finalSize;
        }

        protected virtual void ArrangeHorizontally(SizeF finalSize)
        {
            RectangleF clientRect = this.GetClientRectangle(finalSize);
            float stretchableWidth = 0;
            int stretchableCount = 0;

            int nonStretchableItems = 0;
            foreach (RadElement element in this.Children)
            {
                if (element.StretchHorizontally)
                {
                    stretchableCount++;
                }
                else
                {
                    stretchableWidth += element.DesiredSize.Width;
                    nonStretchableItems++;
                }
            }

            if (nonStretchableItems > 0)
            {
                stretchableWidth += ElementSpacing * (nonStretchableItems - 1);
            }

            int spacing = this.ElementSpacing;

            if (stretchableCount > 0)
            {
                stretchableWidth = (clientRect.Width - stretchableWidth) / stretchableCount;
                stretchableWidth -= spacing * stretchableCount;
            }

            ArrangeItemsHorizontaly(clientRect, finalSize, stretchableWidth, spacing);
        }

        protected virtual void ArrangeItemsHorizontaly(RectangleF clientRect, SizeF finalSize, float stretchableWidth, float spacing)
        {
            float x = clientRect.X;
            float y = clientRect.Y;
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement element = null;

                if (this.RightToLeft && this.RightToLeftMode == RightToLeftModes.ReverseItems)
                {
                    element = this.Children[this.Children.Count - i - 1];
                }
                else
                {
                    element = this.Children[i];
                }

                float width = element.StretchHorizontally ? stretchableWidth : element.DesiredSize.Width;
                float height = element.StretchVertically ? clientRect.Height : element.DesiredSize.Height;
                float xWithOffset = x;

                if (RightToLeft && this.RightToLeftMode == RightToLeftModes.ReverseOffset)
                {
                    xWithOffset = clientRect.Right - x - width;
                }

                this.ArrangeElement(element, clientRect, new RectangleF(xWithOffset, y, width, height), finalSize);

                x += width + spacing;
            }
        }

        protected virtual void ArrangeVertically(SizeF finalSize)
        {
            RectangleF clientRect = this.GetClientRectangle(finalSize);
            float x = clientRect.X;
            float y = clientRect.Y;
            float stretchableHeight = 0;
            int stretchableCount = 0;
            int nonStretchableItems = 0;

            foreach (RadElement element in this.Children)
            {
                if (element.StretchVertically)
                {
                    stretchableCount++;
                }
                else
                {
                    stretchableHeight += element.DesiredSize.Height;
                    nonStretchableItems++;
                }
            }

            if (nonStretchableItems > 0)
            {
                stretchableHeight += ElementSpacing * (nonStretchableItems - 1);
            }

            int spacing = this.ElementSpacing;

            if (stretchableCount > 0)
            {
                stretchableHeight = (clientRect.Height - stretchableHeight) / stretchableCount;
                stretchableHeight -= spacing * stretchableCount;
            }

            foreach (RadElement element in this.Children)
            {
                float width = element.StretchHorizontally ? clientRect.Width : element.DesiredSize.Width;
                float height = element.StretchVertically ? stretchableHeight : element.DesiredSize.Height;
                if (y + height > clientRect.Bottom)
                {
                    height = clientRect.Bottom - y - 1;
                }
                this.ArrangeElement(element, clientRect, new RectangleF(x, y, width, height), finalSize);
                y += height + spacing;
            }
        }

        protected virtual void ArrangeElement(RadElement element, RectangleF clientRect, RectangleF finalRect, SizeF finalSize)
        {
            if (element.FitToSizeMode == RadFitToSizeMode.FitToParentBounds && Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                finalRect.X -= this.Padding.Left + this.GetBorderThickness(true).Left;
                finalRect.Height = finalSize.Height;
                finalRect.Y = 0;
            }
            element.Arrange(finalRect);
        }

        #endregion
    }
}
