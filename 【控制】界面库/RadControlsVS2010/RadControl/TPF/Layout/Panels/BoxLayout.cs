using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.Layouts
{
	/// <summary>
	/// Represents the BoxLayout class
	/// </summary>
    public class BoxLayout : LayoutPanel
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

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
		/// Registers the Proportion dependancy property of BoxLayout
		/// </summary>
        public static RadProperty ProportionProperty = RadProperty.Register("Proportion", typeof(float), typeof(BoxLayout),
            new RadElementPropertyMetadata(0.0f, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

		/// <summary>
		/// Registers the Orientation dependancy proeprty of BoxLayout
		/// </summary>
        public static readonly RadProperty OrientationProperty = RadProperty.Register(
            "Orientation", typeof(Orientation), typeof(BoxLayout), new RadElementPropertyMetadata(
                Orientation.Horizontal, ElementPropertyOptions.CanInheritValue | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

		/// <summary>
		/// Registers the StripPosition dependancy property of BoxLayout
		/// </summary>
        public static readonly RadProperty StripPositionProperty = RadProperty.Register(
            "StripPosition", typeof(StripPosition), typeof(BoxLayout), new RadElementPropertyMetadata(
                StripPosition.First, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

  
		/// <summary>
		/// Gets the proportion based on a given element
		/// </summary>
        /// <param name="element">The element which proportion will be get.</param>
        /// <returns>The proportion value.</returns>
        public static float GetProportion(RadElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (float)element.GetValue(ProportionProperty);
        }

      

        /// <summary>
        /// Sets the proportion (attached property) of a given element.
        /// </summary>
        /// <param name="element">The element which proportion will be set.</param>
        /// <param name="proportion">The proportion value.</param>
        public static void SetProportion(RadElement element, float proportion)
        {
            if (element != null)
            {
                element.SetValue(ProportionProperty, proportion);
            }
        }

        Orientation? orientationCache = null;

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
                if (orientationCache == null || !orientationCache.HasValue)
                {
                    orientationCache = (Orientation)this.GetValue(OrientationProperty);
                }

                return orientationCache.Value;
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        private void InvalidateOrientation()
        {
            orientationCache = null;
        }

		/// <summary>
		/// Handles the properties values changes of BoxLayout 
		/// </summary>
		/// <param name="e"></param>
        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == OrientationProperty)
                InvalidateOrientation();

            base.OnPropertyChanged(e);
        }

		/// <summary>
		/// measures the size to layout the children
		/// </summary>
		/// <param name="availableSize"></param>
		/// <returns></returns>
        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF nonStretchableSize = SizeF.Empty;

            float proportionSum = 0;
            float availableWidth = availableSize.Width;
            float availableHeight = availableSize.Height;

            List<RadElement> stretchOnChildren = new List<RadElement>();
            List<RadElement> stretchOffChildren = new List<RadElement>();

           
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];

                if (child.AutoSize == false)
                    continue;

                float childProportion = GetProportion(child);

                if (childProportion == 0)
                {
                    child.Measure(availableSize);

                    if (Orientation == Orientation.Horizontal)
                    {
                        availableWidth -= child.DesiredSize.Width;

                        nonStretchableSize.Width += child.DesiredSize.Width;
                        nonStretchableSize.Height = Math.Max(nonStretchableSize.Height, child.DesiredSize.Height);

                    }
                    else
                    {
                        availableHeight -= child.DesiredSize.Height;
                        nonStretchableSize.Height += child.DesiredSize.Height;
                        nonStretchableSize.Width = Math.Max(nonStretchableSize.Width, child.DesiredSize.Width);

                    }
                    stretchOffChildren.Add(child);

                }
                else
                    stretchOnChildren.Add(child);

                proportionSum += childProportion;
            }

            if (proportionSum == 0)
            {
                availableWidth = availableSize.Width;
                availableHeight = availableSize.Height;
            }

            for (int i = 0; i < stretchOnChildren.Count; i++)
            {
                RadElement child = stretchOnChildren[i];
                if (child != null)
                {
                    if (child.AutoSize == false)
                        continue;

                    float childProportion = GetProportion(child);

                    if (this.Orientation == Orientation.Horizontal)
                    {
                        //       if (!child.StretchHorizontally)
                        {
                            SizeF childSize = new SizeF(((float)childProportion * availableWidth) / proportionSum, availableHeight);

                            child.Measure(childSize);

                            nonStretchableSize.Width += child.DesiredSize.Width;
                            //         if (!child.StretchVertically)
                            nonStretchableSize.Height = Math.Max(nonStretchableSize.Height, child.DesiredSize.Height);

                            continue;
                        }
                    }
                    else
                    {
                        //   if (!child.StretchVertically)
                        {
                            SizeF childSize = new SizeF(availableWidth, ((float)childProportion * availableHeight) / proportionSum);

                            child.Measure(childSize);

							nonStretchableSize.Height += child.DesiredSize.Height;
                            //     if (!child.StretchHorizontally)
                            nonStretchableSize.Width = Math.Max(nonStretchableSize.Width, child.DesiredSize.Width);
                            //					Console.WriteLine(nonStretchableSize);

                            continue;
                        }
                    }

                    //TODO: Possible BUG - the following code got in a dead code block due to removal of 2 if statemests in the blocks above
                    //child.Measure(availableSize);
                    //
                }
            }

            return nonStretchableSize;
        }

		/// <summary>
		/// arranges the children by a given criteria 
		/// </summary>
		/// <param name="finalSize"></param>
		/// <returns></returns>
        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            PointF firstPositions = PointF.Empty;

            PointF lastPositions = PointF.Empty;
            if (this.Orientation == Orientation.Horizontal)
                lastPositions.X = finalSize.Width;
            if (this.Orientation == Orientation.Vertical)
                lastPositions.Y = finalSize.Height;

            SizeF nonStretchableSize = SizeF.Empty;

            List<RadElement> stretchOnChildren = new List<RadElement>();
            List<RadElement> stretchOffChildren = new List<RadElement>();
            List<RadElement> proportionedChildren = new List<RadElement>();

            float availableWidth = finalSize.Width;
            float availableHeight = finalSize.Height;
            float proportionSum = 0;

            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];

                if (child.AutoSize == false)
                    continue;

                if (child != null)
                {
                    float childProportion = GetProportion(child);

                    if (this.Orientation == Orientation.Horizontal)
                    {
                        if (child.StretchHorizontally)
                            stretchOnChildren.Add(child);
                        else
                            stretchOffChildren.Add(child);
                    }
                    else
                    {
                        if (child.StretchVertically)
                            stretchOnChildren.Add(child);
                        else
                            stretchOffChildren.Add(child);
                    }

                    if (childProportion != 0)
                    {
                        proportionedChildren.Add(child);
                    }
                    else
                    {
                        if (Orientation == Orientation.Horizontal)
                            availableWidth -= child.DesiredSize.Width;
                        else
                            availableHeight -= child.DesiredSize.Height;
                    }

                    proportionSum += childProportion;
                }
            }

            if (proportionSum == 0)
            {
                availableWidth = finalSize.Width;
                availableHeight = finalSize.Height;
            }

            for (int i = 0; i < stretchOffChildren.Count; i++)
            {
                RadElement child = stretchOffChildren[i];

                if (child.AutoSize == false)
                    continue;

                SizeF childSize = SizeF.Empty;

                float childProportion = GetProportion(child);

                if (childProportion == 0)
                {
                    childSize = child.DesiredSize;
                }

                if (this.Orientation == Orientation.Horizontal)
                {

                    if (childProportion != 0)
                        childSize = new SizeF((availableWidth * childProportion) / proportionSum,
                            availableHeight);

                    nonStretchableSize.Width += childSize.Width;
                    //if (child.StretchVertically)
                    childSize.Height = availableHeight;
                     
                    StripPosition stripPosition = (StripPosition)child.GetValue(StripPositionProperty);
                    bool isFirst = ((stripPosition == StripPosition.First) ^ this.RightToLeft);
                    if(isFirst)
                    {
                            child.Arrange(new RectangleF(firstPositions, childSize));
                            firstPositions.X += childSize.Width;
                    }
                    else
                    {
                            
                            lastPositions.X -= childSize.Width;
                            child.Arrange(new RectangleF(lastPositions, childSize));
                    }
                    
                }
                else
                {
                    if (childProportion != 0)
                        childSize = new SizeF(availableWidth, (availableHeight * childProportion) / proportionSum);

                    nonStretchableSize.Height += child.DesiredSize.Height;
                    //if (child.StretchHorizontally)
                    childSize.Width = availableWidth;
                    StripPosition stripPosition = (StripPosition)child.GetValue(StripPositionProperty);
                    switch (stripPosition)
                    {
                        case StripPosition.First:
                            child.Arrange(new RectangleF(firstPositions, childSize));
                            firstPositions.Y += childSize.Height;
                            break;
                        case StripPosition.Last:
                            lastPositions.Y -= childSize.Height;
                            child.Arrange(new RectangleF(lastPositions, childSize));
                            break;
                    }
                }
            }

            for (int i = 0; i < stretchOnChildren.Count; i++)
            {
                float x = 0;
                float y = 0;

                RadElement child = stretchOnChildren[i];

                if (child.AutoSize == false)
                    continue;

                SizeF childSize = SizeF.Empty;

                float childProportion = GetProportion(child);

                if (childProportion == 0)
                {
                    childSize = child.DesiredSize;
                }

                if (this.Orientation == Orientation.Horizontal)
                {
                    if (childProportion != 0)
                    {
                        childSize = new SizeF((availableWidth * childProportion) / proportionSum,
                            availableHeight);
                        
                    }
                    else
                    {
                        childSize.Width = availableWidth - nonStretchableSize.Width;
                    }
                    x = childSize.Width;
                    //if (child.StretchVertically)
                    childSize.Height = availableHeight;

                    if (!this.RightToLeft)
                    {
                        child.Arrange(new RectangleF(firstPositions, childSize));

                        firstPositions.X += x;
                        firstPositions.Y += y;
                    }
                    else
                    {
                        lastPositions.X -= x;
                        lastPositions.Y -= y;

                        child.Arrange(new RectangleF(lastPositions, childSize));
                    }
                }
                else
                {
                    if (childProportion != 0)
                    {
                        childSize = new SizeF(availableWidth, (availableHeight * childProportion) / proportionSum);
                        y = childSize.Height;
                    }
                    else
                    {
                        childSize.Height = availableHeight - nonStretchableSize.Height;

                        //if (child.StretchHorizontally)
                        childSize.Width = availableWidth;
                    }

                    child.Arrange(new RectangleF(firstPositions, childSize));

                    firstPositions.X += x;
                    firstPositions.Y += y;
                }
            }

            return finalSize;
        }
    }
}
//add children to lay out to the custom layout panel