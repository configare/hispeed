using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class ExpandableStackLayout : StackLayoutPanel
    {
        public static RadProperty CollapsingEnabledProperty = RadProperty.Register(
            "CollapsingEnabled", typeof(bool), typeof(ExpandableStackLayout), new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout));

        private List<CollapsibleElement> collapsibleChildren = null;

        /// <summary>
        /// Gets or sets a boolean value determining whether the layout panel
        /// will collapse its content according to its size.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the layout panel will collapse its content according to its size.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool CollapsingEnabled
        {
            get
            {
                return (bool)this.GetValue(CollapsingEnabledProperty);
            }
            set
            {
                this.SetValue(CollapsingEnabledProperty, value);
            }
        }

        protected List<CollapsibleElement> CollapsibleChildren1
        {
            get { return collapsibleChildren; }
            set { collapsibleChildren = value; }
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            this.collapsibleChildren = null;
            base.OnChildrenChanged(child, changeOperation);
        }

        public class CollapsableOrderSorter : IComparer<CollapsibleElement>
        {
            public int Compare(CollapsibleElement x, CollapsibleElement y)
            {
                RadRibbonBarGroup left = x as RadRibbonBarGroup;
                RadRibbonBarGroup right = y as RadRibbonBarGroup;
                if (left == null || right == null)
                {
                    return 0;
                }

                return left.CollapsingPriority.CompareTo(right.CollapsingPriority);
            }
        } 

        private List<CollapsibleElement> CollapsibleChildren
        {
            get
            {
                if (this.collapsibleChildren == null)
                {
                    this.collapsibleChildren = new List<CollapsibleElement>();                    
                    RadElementCollection children = this.Children;
                    int itemsCount = children.Count;
                    bool shouldSortItems = false;

                    for (int i = 0; i < itemsCount ; ++i)
                    {
                        CollapsibleElement collapsibleElement = children[i] as CollapsibleElement;
                        if( collapsibleElement != null )
                        {
                            this.collapsibleChildren.Add(collapsibleElement);

                            RadRibbonBarGroup ribbonBarGroup = collapsibleElement as RadRibbonBarGroup;
                            if (ribbonBarGroup != null && ribbonBarGroup.CollapsingPriority > 0)
                            {
                                shouldSortItems = true;
                            }
                        }
                    }

                    if (shouldSortItems)
                    {
                        this.collapsibleChildren.Sort(new CollapsableOrderSorter());  
                    }
                }
                             
                return this.collapsibleChildren;
            }
        }

        private SizeF GetAllElementsSize(SizeF constraint)
        {
            SizeF allElementsSize = SizeF.Empty;
            foreach (RadElement element in this.Children)
            {
                element.Measure(constraint);
                allElementsSize.Width += element.DesiredSize.Width;
                allElementsSize.Height = Math.Max(allElementsSize.Height, element.DesiredSize.Height);   
            }

            return allElementsSize;
        }

        private int collapsedElementCount = 0;

        public static void InvalidateAll(RadElement baseElement)
        {
            baseElement.InvalidateMeasure();
            baseElement.InvalidateArrange();
            foreach (RadElement element in baseElement.Children)
            {
                InvalidateAll(element);
            }
        }

        public static void SetCollapsingEnabled(RadElement rootElement, bool enabled)
        {
            foreach (RadElement childElement in rootElement.EnumDescendants(TreeTraversalMode.BreadthFirst))
            {
                if (childElement is ExpandableStackLayout)
                {
                    childElement.SetValue(ExpandableStackLayout.CollapsingEnabledProperty, enabled);
                }
            }
        }

        protected override SizeF MeasureOverride(SizeF constraint)
        {
            if (!this.CollapsingEnabled || this.IsDesignMode)
            {
                return base.MeasureOverride(constraint);
            }

            InvalidateAll(this);
            SizeF allElementsSize = this.GetAllElementsSize(constraint);

            int count = this.Children.Count;
 
            if (allElementsSize.Width >= constraint.Width)
            {
                while (true)
                {
                    int possibleCollapseStep = -1;
                    CollapsibleElement collapsibleElement = this.GetElementToCollapse(out possibleCollapseStep);
                    if (collapsibleElement == null)
                    {
                        break;
                    }

                    collapsibleElement.InvalidateMeasure();
                    collapsibleElement.Measure(constraint);

                    SizeF sizeBeforeCollapsing = collapsibleElement.DesiredSize;
                    collapsibleElement.SizeBeforeCollapsing = sizeBeforeCollapsing;
                    if (collapsibleElement.CollapseElementToStep(possibleCollapseStep))
                    {
                        InvalidateAll(collapsibleElement);
                        collapsibleElement.Measure(constraint);

                        SizeF sizeAfterCollapsing = collapsibleElement.DesiredSize;

                        collapsibleElement.SizeAfterCollapsing = sizeAfterCollapsing;

                        this.collapsedElementCount++;
                        allElementsSize = this.GetAllElementsSize(constraint);
                        if (allElementsSize.Width <= constraint.Width)
                        {
                            break;
                        }
                    }
                }
            }
            else 
            {
                for (int i = 0; i < count ; ++i)
                {
                    int possibleExpandStep = -1;
                    CollapsibleElement expandableElement = this.GetElementToExpand(allElementsSize.Width, constraint.Width, out possibleExpandStep);
                    if (expandableElement != null)
                    {
                        if (allElementsSize.Width - expandableElement.DesiredSize.Width + expandableElement.SizeBeforeCollapsing.Width <= constraint.Width)
                        {
                            if (expandableElement.ExpandElementToStep(possibleExpandStep))
                            {
                                this.collapsedElementCount--;
                            }
                        }
                    }
                }
            }

            InvalidateAll(this);
            //allElementsSize = this.GetAllElementsSize(constraint);
            return allElementsSize;
        }

        protected CollapsibleElement GetElementToCollapse( out int possibleCollapseStep )
        {
            possibleCollapseStep = -1;

            IList<CollapsibleElement> children = this.CollapsibleChildren;
            int count = children.Count;
            if (count == 0)
            {
                return null;
            }

            int nextCollapseStep = children[ count - 1 ].CollapseStep + 1;
            for (int i = 0; i < children.Count; ++i)
            {
                nextCollapseStep = Math.Min(children[i].CollapseStep, nextCollapseStep);
            }

            nextCollapseStep++;

            int maxCollapseStep = children[ 0 ].CollapseMaxSteps;
            for (; nextCollapseStep < maxCollapseStep; ++nextCollapseStep)
            {
                for (int collapsibleIndex = count - 1; collapsibleIndex >= 0; --collapsibleIndex)
                {
                    CollapsibleElement collapsibleElement = children[collapsibleIndex];

                    if (collapsibleElement.CanCollapseToStep(nextCollapseStep))
                    {
                        possibleCollapseStep = nextCollapseStep;
                        return collapsibleElement;
                    }
                }
            }

            for (int collapsibleIndex = count - 1; collapsibleIndex >= 0; --collapsibleIndex)
            {
                CollapsibleElement collapsibleElement = children[collapsibleIndex];
                if (collapsibleElement.CanCollapseToStep(collapsibleElement.CollapseMaxSteps))
                {
                    possibleCollapseStep = collapsibleElement.CollapseMaxSteps;
                    return collapsibleElement;
                }
            }
                
            return null;
        }

        protected CollapsibleElement GetElementToExpand(float sumAllElementsWidth, float availableWidth, out int possibleExpandStep)
        {
            possibleExpandStep = -1;
            CollapsibleElement elementToExpand = null;

            IList<CollapsibleElement> children = this.CollapsibleChildren;
            int count = children.Count;
            if (count == 0)
            {
                return null;
            }

            for (int nextCollapseStep = (int)ChunkVisibilityState.Collapsed - 1; nextCollapseStep >= 0; nextCollapseStep--)
            {
                for (int collapsibleIndex = 0; collapsibleIndex < count; ++collapsibleIndex)
                {
                    CollapsibleElement collapsibleElement = children[collapsibleIndex];

                    if (collapsibleElement.CanExpandToStep(nextCollapseStep))
                    {
                        possibleExpandStep = nextCollapseStep;
                        return collapsibleElement;
                    }
                }
            }

            return elementToExpand;
        }
    }
}
