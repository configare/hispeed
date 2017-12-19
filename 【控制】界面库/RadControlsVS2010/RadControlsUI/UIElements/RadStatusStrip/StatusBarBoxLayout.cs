using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the StatusBarBoxLayout class
    /// </summary>
    public class StatusBarBoxLayout : LayoutPanel
    {
        #region Nested types


        /// <summary>
        /// represents StripPosition enumeration
        /// </summary>
        public enum StripPosition
        {
            First,
            Last
        }
        #endregion

        /// <summary>
        /// Registers the Proportion dependancy property of StatusBarBoxLayout
        /// </summary>
        public static RadProperty ProportionProperty = RadProperty.Register("Proportion", typeof(int), typeof(StatusBarBoxLayout),
            new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Registers the Orientation dependancy proeprty of StatusBarBoxLayout
        /// </summary>
        public static readonly RadProperty OrientationProperty = RadProperty.Register(
            "Orientation", typeof(Orientation), typeof(StatusBarBoxLayout), new RadElementPropertyMetadata(
                Orientation.Horizontal, ElementPropertyOptions.CanInheritValue | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Registers the StripPosition dependancy property of StatusBarBoxLayout
        /// </summary>
        public static readonly RadProperty StripPositionProperty = RadProperty.Register(
            "StripPosition", typeof(StripPosition), typeof(StatusBarBoxLayout), new RadElementPropertyMetadata(
                StripPosition.First, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        
        [RefreshProperties(RefreshProperties.All)]
        public static RadProperty SpringProperty = RadProperty.RegisterAttached(
            "Spring", typeof(bool), typeof(StatusBarBoxLayout), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsParentArrange | ElementPropertyOptions.AffectsArrange));


        /// <summary>
        /// Gets the proportion based on a given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int GetProportion(RadElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (int)element.GetValue(ProportionProperty);
        }

        /// <summary>
        /// Gets or sets strip orientation - it could be horizontal or vertical.
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory)]
        [RadPropertyDefaultValue("Orientation", typeof(StripLayoutPanel))]
        [Description("Orientation of the strip - could be horizontal or vertical")]
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

        /// <summary>
        /// arranges the children by a given criteria 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override SizeF ArrangeOverride(SizeF finalSize)
        {            
            float strechOffWidth = 0f;
            float strechOffHeight = 0f;
            int strechOnCount = 0;
            RadElement item = null;
            for( int i = 0; i < this.Children.Count; ++i )
            {
                item = this.Children[i];


                SizeF defaultSize = ((VisualElement)item).DefaultSize;

                if ((bool)item.GetValue(StatusBarBoxLayout.SpringProperty) == false)
                {
                    strechOffWidth += Math.Max( item.DesiredSize.Width, defaultSize.Width );
                    strechOffHeight += Math.Max(item.DesiredSize.Height, defaultSize.Height); ;
                }
                else
                {
                    strechOnCount++;
                }
            }
            
            float spaceForAllStrechOn = 0f;
            if (this.Orientation == Orientation.Horizontal)
            {
                spaceForAllStrechOn = finalSize.Width - strechOffWidth;
            }
            else
            {
                spaceForAllStrechOn = finalSize.Height - strechOffHeight;
            }

            float spaceForSingleSpringElement = 0f;
            if (strechOnCount != 0)
            {
                spaceForSingleSpringElement = spaceForAllStrechOn / strechOnCount;
            }

            PointF firstPosition = PointF.Empty;
            SizeF incrementSize = SizeF.Empty;            

            bool springElementInMinSize = false;
            if (this.Orientation == Orientation.Horizontal)
            {
                incrementSize.Height = finalSize.Height;

                for (int i = 0; i < this.Children.Count; ++i)
                {
                    item = this.Children[i];
                    if ((bool)item.GetValue(StatusBarBoxLayout.SpringProperty))
                    {
                        if (spaceForSingleSpringElement <= item.DesiredSize.Width)
                        {
                            springElementInMinSize = true;
                            break;
                        }
                    }
                    
                }

                for (int i = 0; i < this.Children.Count; ++i)
                {
                    item = this.Children[i];
                    
                    SizeF defaultSize = ((VisualElement)item).DefaultSize;

                    if ((bool)item.GetValue(StatusBarBoxLayout.SpringProperty) == false)
                    {
                        incrementSize.Width = Math.Max( item.DesiredSize.Width, defaultSize.Width);
                    }
                    else
                    {
                        incrementSize.Width = spaceForSingleSpringElement - 1;
                    }
                    RectangleF arrangeRectangle = new RectangleF(firstPosition, incrementSize);
                    if (this.RightToLeft)
                    {
                        arrangeRectangle = LayoutUtils.RTLTranslateNonRelative(arrangeRectangle, new RectangleF(PointF.Empty, finalSize));
                    }
                    item.Arrange(arrangeRectangle);

                    if (
                        (
                        strechOnCount==0 &&
                        firstPosition.X + incrementSize.Width + strechOnCount >= finalSize.Width && 
                        finalSize.Width != 0)||
                        (
                        strechOnCount!=0 && 
                        springElementInMinSize == true &&
                        firstPosition.X + incrementSize.Width + strechOnCount >= finalSize.Width)||
                        (
                        incrementSize.Width<0)
                        )
                    {                       
                        item.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Hidden);
                    }
                    else
                    {
                        item.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
                    }

                    firstPosition.X += incrementSize.Width;
                }
            }
            else//vertical
            {
                incrementSize.Width = finalSize.Width;

                for (int i = 0; i < this.Children.Count; ++i)
                {
                    item = this.Children[i];
                    SizeF defaultSize = ((VisualElement)item).DefaultSize;

                    if ((bool)item.GetValue(StatusBarBoxLayout.SpringProperty) == false)
                    {
                        incrementSize.Height = Math.Max( item.DesiredSize.Height, defaultSize.Height );
                    }
                    else
                    {
                        incrementSize.Height = spaceForSingleSpringElement;
                    }

                    item.Arrange(new RectangleF(firstPosition, incrementSize));
                    firstPosition.Y += incrementSize.Height;
                }
            }
            return finalSize;            
        }

        protected override SizeF MeasureOverride(SizeF constraint)
        {
            float width;
            RadElementCollection internalChildren = this.Children;
            SizeF size = new SizeF();
            SizeF availableSize = constraint;
            bool flag = this.Orientation == Orientation.Horizontal;
            if (flag)
            {
                availableSize.Width = float.PositiveInfinity;
                width = constraint.Width;
            }
            else
            {
                availableSize.Height = float.PositiveInfinity;
                width = constraint.Height;
            }
            int num5 = 0;
            int count = internalChildren.Count;
            while (num5 < count)
            {
                RadElement element = internalChildren[num5];
                if (element != null)
                {
                    float height;
                    element.Measure(availableSize);
                    SizeF desiredSize = Size.Empty;
                    if (element.Visibility == ElementVisibility.Visible)
                        desiredSize = element.DesiredSize;
                    if (flag)
                    {
                        size.Width += desiredSize.Width;
                        size.Height = Math.Max(size.Height, desiredSize.Height);
                        height = desiredSize.Width;
                    }
                    else
                    {
                        size.Width = Math.Max(size.Width, desiredSize.Width);
                        size.Height += desiredSize.Height;
                        height = desiredSize.Height;
                    }
                }
                num5++;
            }
            return size;
        }
    }
}
