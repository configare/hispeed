using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Collections;
using Telerik.WinControls.Themes.ControlDefault;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
    public class RibbonBarCaptionLayoutPanel : LayoutPanel
    {
        public static RadProperty CaptionTextProperty = RadProperty.Register(
            "CaptionText", typeof(bool), typeof(RibbonBarCaptionLayoutPanel), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));


        private RadRibbonBarElement ribbonBarElement;
        private RadPageViewElement tabStripElement;
        private RadItemCollection contextualTabGroups;
        private RadElement captionTextElement = null;
        private const int RIGHTMOSTGROUP_VISIBILITY_TRESHOLD = 15;

        #region Constructor

        public RibbonBarCaptionLayoutPanel(RadRibbonBarElement ribbonBarElement)
        {
            this.tabStripElement = ribbonBarElement.TabStripElement;
            this.contextualTabGroups = ribbonBarElement.ContextualTabGroups;
            this.ribbonBarElement = ribbonBarElement;
        }

        #endregion

        #region Properties

       

        public RadElement CaptionTextElement
        {
            get
            {
                this.InitializeElements();
                return this.captionTextElement;
            }
        }

        #endregion

        #region Ribbon layout state variables

        private float rightEmptySpace;
        private bool showTabGroups = true;
        private bool isShrinking = false;
        private ContextualTabGroup rightMostGroup = null;

        #endregion

        private void InitializeElements()
        {
            if (this.captionTextElement != null)
            {
                return;
            }

            foreach (RadElement element in this.Children)
            {
                if ((bool)element.GetValue(RibbonBarCaptionLayoutPanel.CaptionTextProperty))
                {
                    captionTextElement = element;
                }
            }
        }

        #region New layouts

        #region Helper methods

        /// <summary>
        /// Transforms the given point's X coordinate from world coordinates to local coordinates.
        /// </summary>
        /// <param name="point">The point to transform</param>
        /// <returns>The transformed point</returns>
        private Point TransformXToClient(Point point)
        {
            float xCoord = 0;
            if (!this.ribbonBarElement.QuickAccessToolbarBelowRibbon)
            {
                if (!this.RightToLeft)
                {
                    xCoord = point.X - (
                       this.ribbonBarElement.dropDownButton.DesiredSize.Width +
                       this.ribbonBarElement.QuickAccessToolBar.DesiredSize.Width);
                }
                else
                {
                    xCoord = point.X
                        - (this.ribbonBarElement.RibbonCaption.SystemButtons.DesiredSize.Width);
                }
            }
            else
            {
                if (!this.RightToLeft)
                {
                    xCoord = point.X - (this.ribbonBarElement.dropDownButton.DesiredSize.Width);
                }
                else
                {
                    xCoord = xCoord = point.X
                        - (this.ribbonBarElement.RibbonCaption.SystemButtons.DesiredSize.Width);
                }
            }

            return Point.Round(new PointF(xCoord, point.Y));
        }

        /// <summary>
        /// This method calculates the available space for the
        /// ribbon caption text at the left side of the</summary>
        /// contextual tab groups
        /// <param name="availableSize">The total available size for the elements 
        /// managed by this layout panel.</param>
        /// <returns>The width available.</returns>
        private float GetLeftCaptionSpace(SizeF availableSize)
        {
            ContextualTabGroup leftMost = this.GetLeftMostGroup(this.IsDesignMode);

            if (!this.RightToLeft)
            {
                if (leftMost.TabItems.Count > 0)
                {
                    return this.TransformXToClient(leftMost.TabItems[0].ControlBoundingRectangle.Location).X;
                }
                else
                {
                    return leftMost.FullBoundingRectangle.X;
                }
            }
            else
            {
                if (leftMost.TabItems.Count > 0)
                {
                    int tabItemIndex = leftMost.TabItems.Count - 1;
                    float groupWidth = this.CalculateGroupWidth(leftMost);
                    return availableSize.Width - 
                        (this.TransformXToClient(
                        leftMost.TabItems[tabItemIndex].ControlBoundingRectangle.Location).X
                        + groupWidth);
                }
                else
                {
                    return availableSize.Width - (leftMost.FullBoundingRectangle.X + leftMost.DesiredSize.Width);
                }
            }
        }

        /// <summary>
        /// This method calculates the available space for the
        /// ribbon caption text at the right side of the</summary>
        /// contextual tab groups
        /// <param name="availableSize">The total available size for the elements 
        /// managed by this layout panel.</param>
        /// <returns>The width available.</returns>
        private float GetRightCaptionSpace(SizeF availableSize)
        {
            ContextualTabGroup rightMost = this.GetRightMostGroup(this.IsDesignMode);

            if (!this.RightToLeft)
            {
                if (rightMost.TabItems.Count > 0)
                {
                    float groupWidth = this.CalculateGroupWidth(rightMost);

                    return availableSize.Width - ((this.TransformXToClient(rightMost.TabItems[0].ControlBoundingRectangle.Location).X + groupWidth));
                }
                else
                {
                    return availableSize.Width - ((rightMost.BoundingRectangle.X + rightMost.BoundingRectangle.Width));
                }
            }
            else
            {
                if (rightMost.TabItems.Count > 0)
                {
                    int tabItemIndex = rightMost.TabItems.Count - 1;

                    return this.TransformXToClient(rightMost.TabItems[tabItemIndex].ControlBoundingRectangle.Location).X;
                }
                else
                {
                    return rightMost.BoundingRectangle.X;
                }
            }
        }

        /// <summary>
        /// Determines whether the tab strip items should be reordered so that they match
        /// the requirements for associated tab strip items.
        /// </summary>
        /// <returns>True if a reset is needed. Otherwise false.</returns>
        private bool ShouldResetContextTabs()
        {
            int tabIndexCounter = this.IsAddNewTabItemInTabStrip() ? this.tabStripElement.Items.Count - 2
                : this.tabStripElement.Items.Count - 1;

            for (int i = this.contextualTabGroups.Count - 1; i > -1 ; i--)
            {
                ContextualTabGroup tabGroup = this.contextualTabGroups[i] as ContextualTabGroup;

                if (tabGroup != null && tabGroup.TabItems.Count > 0)
                {
                    for (int k = tabGroup.TabItems.Count - 1; k > - 1 ; k--)
                    {
                        RadPageViewItem currentTab = tabGroup.TabItems[k] as RadPageViewItem;

                        int index = this.tabStripElement.Items.IndexOf(currentTab);

                        if (tabIndexCounter - index == 0)
                            tabIndexCounter--;
                        else
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the Add New Tab item is in the tab strip.
        /// </summary>
        /// <returns>True or false</returns>
        private bool IsAddNewTabItemInTabStrip()
        {
            for (int i = 0; i < this.ribbonBarElement.TabStripElement.Items.Count; i++)
            {
                RibbonTab currentTab = this.ribbonBarElement.TabStripElement.Items[i] as RibbonTab;

                if ((bool)currentTab.GetValue(RadItem.IsAddNewItemProperty))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the count of the empty contextual tab groups.
        /// </summary>
        /// <returns>The count of the empty groups.</returns>
        private int GetEmptyGroupsCount()
        {
            int result = 0;

            for (int i = 0; i < this.contextualTabGroups.Count; i++)
            {
                ContextualTabGroup tabGroup = this.contextualTabGroups[i] as ContextualTabGroup;

                if (tabGroup != null)
                {
                    if (tabGroup.TabItems.Count == 0)
                    {
                       result++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Resets the layout context variables which are used to determine the position
        /// of the caption text, the contextual tabs and the design-time
        /// contextual tab groups which are empty.
        /// </summary>
        private void ResetLayoutContext(SizeF availableSize)
        {
            //Check whether a new reordering of the contextual tab items
            //is needed. This may happen when new tabs are inserted in the tab strip,
            //tabs are moved between contextual tab groups or contextual tab groups
            //are removed.
            if (this.ShouldResetContextTabs())
            {
                //This method performs the reordering
                this.ResetAssociatedTabItems();
            }

            int tabCount = this.tabStripElement.Items.Count - 1;

            if (tabCount == -1)
                return;
            
            RadPageViewItem rightMostItem = this.tabStripElement.Items[tabCount] as RadPageViewItem;

            float rightMostItemRightEdge = this.TransformXToClient(rightMostItem.ControlBoundingRectangle.Location).X;
            this.rightEmptySpace = rightMostItemRightEdge;

            if (!this.RightToLeft)
            {
                rightMostItemRightEdge += rightMostItem.ControlBoundingRectangle.Width;
                this.rightEmptySpace = (availableSize.Width) - rightMostItemRightEdge;
            }
        }

        /// <summary>
        /// Gets the left most contextual tab group.
        /// </summary>
        /// <param name="considerEmpty">Determines whether empty contextual groups are considered when
        /// calculating the left most group</param>
        /// <returns>A reference to the left most group. Null if no groups are found.</returns>
        internal ContextualTabGroup GetLeftMostGroup(bool considerEmpty)
        {
            return this.GetEndLeftGroup(considerEmpty);
        }

        /// <summary>
        /// Gets the right most contextual tab group.
        /// </summary>
        /// <param name="considerEmpty">Determines whether empty contextual groups are considered when
        /// calculating the right most group</param>
        /// <returns>A reference to the right most contextual group. Null if no groups are found.</returns>
        internal ContextualTabGroup GetRightMostGroup(bool considerEmpty)
        {
            return this.GetEndRightGroup(considerEmpty);
        }


        #region New approach for acquiring the end/right most groups

//#warning This approach needs testing!!!

        internal ContextualTabGroup GetEndLeftGroup(bool considerEmpty)
        {
            ContextualTabGroup endLeftGroup = null;
            int leftMostIndex = -1;
            for (int i = 0; i < this.contextualTabGroups.Count; i++)
            {
                ContextualTabGroup currentGroup = this.contextualTabGroups[i] as ContextualTabGroup;

                if (currentGroup != null)
                {
                    if (currentGroup.TabItems.Count == 0 && considerEmpty)
                    {
                        if (endLeftGroup == null)
                        {
                            endLeftGroup = currentGroup;
                            leftMostIndex = this.contextualTabGroups.IndexOf(endLeftGroup);
                        }
                        else
                        {
                            if (i < leftMostIndex)
                            {
                                endLeftGroup = currentGroup;
                                leftMostIndex = this.contextualTabGroups.IndexOf(endLeftGroup);
                            }
                        }
                    }
                    else
                    {
                        if (currentGroup.TabItems.Count > 0 && currentGroup.Visibility == ElementVisibility.Visible)
                        {
                            if (endLeftGroup == null)
                            {
                                endLeftGroup = currentGroup;
                                leftMostIndex = this.contextualTabGroups.IndexOf(endLeftGroup);
                            }
                            else
                            {
                                if (endLeftGroup.TabItems.Count == 0)
                                {
                                    endLeftGroup = currentGroup;
                                    leftMostIndex = this.contextualTabGroups.IndexOf(endLeftGroup);
                                }

                                if (i < leftMostIndex && endLeftGroup.TabItems.Count > 0)
                                {
                                    endLeftGroup = currentGroup;
                                    leftMostIndex = this.contextualTabGroups.IndexOf(endLeftGroup);
                                }
                            }
                        }
                    }
                }
            }

            return endLeftGroup;
        }
        internal ContextualTabGroup GetEndRightGroup(bool considerEmpty)
        {
            ContextualTabGroup rightMostGroup = null;
            int rightMostIndex = -1;
            for (int i = 0; i < this.contextualTabGroups.Count; i++)
            {
                ContextualTabGroup currentGroup = this.contextualTabGroups[i] as ContextualTabGroup;

                if (currentGroup != null)
                {
                    if (currentGroup.TabItems.Count == 0 && considerEmpty)
                    {
                        if (rightMostGroup == null)
                        {
                            rightMostGroup = currentGroup;
                            rightMostIndex = this.contextualTabGroups.IndexOf(rightMostGroup);

                        }
                        else
                        {

                            if (i > rightMostIndex)
                            {
                                rightMostGroup = currentGroup;
                                rightMostIndex = this.contextualTabGroups.IndexOf(rightMostGroup);
                            }
                        }
                    }
                    else
                    {
                        if (currentGroup.TabItems.Count > 0 && currentGroup.Visibility == ElementVisibility.Visible)
                        {
                            if (rightMostGroup == null)
                            {
                                rightMostGroup = currentGroup;
                                rightMostIndex = this.contextualTabGroups.IndexOf(rightMostGroup);
                            }
                            else
                            {

                                if (i > rightMostIndex && rightMostGroup.TabItems.Count > 0)
                                {
                                    rightMostGroup = currentGroup;
                                    rightMostIndex = this.contextualTabGroups.IndexOf(rightMostGroup);
                                }
                            }
                        }
                    }
                }
            }

            return rightMostGroup;
        }

        #endregion

        

        /// <summary>
        /// This method reorders the TabStrip items so that they are positioned under the
        /// ContextualTabGroup they are associated with. All tab items that are
        /// associated with a tab groups should be positioned on the right side of the tab strip.
        /// This algorithm begins iterating from the first to the last contextual tab group as they
        /// appear in the collection of the ribbon bar. The associated tab items are always inserted
        /// at the end of the tab strip. In this way the effect of positioning the last associated
        /// tab item at the end of the corresponding contextual group is achieved.
        /// </summary>
        private void ResetAssociatedTabItems()
        {
            RibbonTab selectedTab = this.tabStripElement.SelectedItem as RibbonTab;

            List<RadItem> scheduledItemsForDeletion = new List<RadItem>();

            for (int i = 0; i < this.contextualTabGroups.Count; i++)
            {
                ContextualTabGroup currentGroup = this.contextualTabGroups[i] as ContextualTabGroup;

                for (int k = 0; k < currentGroup.TabItems.Count; k++)
                {
                    RadPageViewItem currentItem = currentGroup.TabItems[k] as RadPageViewItem;

                    for (int j = 0; j < this.tabStripElement.Items.Count; ++j)
                    {
                        if (((RibbonTab)this.tabStripElement.Items[j]).obsoleteTab == currentItem)
                        {
                            currentItem = (RibbonTab)this.tabStripElement.Items[j];
                            currentGroup.TabItems[k] = currentItem;
                        }
                    }

                    if (this.tabStripElement.Items.Contains(currentItem) &&
                        !(bool)currentItem.GetValue(RadItem.IsAddNewItemProperty)
                        && currentItem.Parent != null)
                    {
                        this.tabStripElement.RemoveItem(currentItem);

                        if (this.IsDesignMode)
                        {
                            if (this.tabStripElement.Items.Count == 0)
                            {
                                this.tabStripElement.InsertItem(this.tabStripElement.Items.Count, currentItem);
                            }
                            else
                            {
                                this.tabStripElement.InsertItem(this.tabStripElement.Items.Count - 1, currentItem);
                            }
                        }
                        else
                        {
                            this.tabStripElement.InsertItem(this.tabStripElement.Items.Count, currentItem);
                        }
                    }
                }
            }

            this.tabStripElement.SelectedItem = selectedTab;
        }

        /// <summary>
        /// This method calculates the size of a contextual group base on the associated tabs.
        /// </summary>
        /// <param name="tabGroup">The tab group which size is to be calculated</param>
        /// <returns>The calculated size of the group.</returns>
        private float CalculateGroupWidth(ContextualTabGroup tabGroup)
        {
            float result = 0;

            if (tabGroup.TabItems.Count > 0)
            {
                for (int i = 0; i < tabGroup.TabItems.Count; i++)
                {
                    RadPageViewItem currentItem = tabGroup.TabItems[i] as RadPageViewItem;

                    //backward compatabilitty 
                    //!TODO review 
                    for (int j = 0; j < this.tabStripElement.Items.Count; ++j)
                    {
                        if (((RibbonTab)this.tabStripElement.Items[j]) == currentItem)
                        {
                            currentItem = (RibbonTab)this.tabStripElement.Items[j];
                            tabGroup.TabItems[i] = currentItem;
                        }
                    }

                    if (currentItem != null)
                    {
                        result += currentItem.FullBoundingRectangle.Width;
                    }
                }

                TabLayoutPanel tabLayout = tabGroup.TabItems[0].Parent as TabLayoutPanel;

                int tabOverlapFactor = 0;

                if (tabLayout != null)
                {
                    tabOverlapFactor = tabLayout.ItemsOverlapFactor;
                }

                result -= (tabGroup.TabItems.Count - 1f) * tabOverlapFactor;
            }

            return result;
        }

        #endregion


        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            InitializeElements();

            this.ResetLayoutContext(availableSize);

            SizeF result = SizeF.Empty;

            SizeF groupsSize = this.ContextGroupsMeasure(availableSize);
            result.Width += groupsSize.Width;
            result.Height = groupsSize.Height;

            SizeF captionSize = this.CaptionMeasure(availableSize);
            result.Width += captionSize.Width;
            result.Height = Math.Max(groupsSize.Height, captionSize.Height);

            return result;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            InitializeElements();

            this.ContextGroupsArrange(finalSize);
            this.CaptionArrange(finalSize);

            return finalSize;
        }

        #region Caption layout

        private SizeF CaptionMeasure(SizeF availableSize)
        {
            RadItemCollection contextualTabGroups = this.contextualTabGroups;
            SizeF captionSize = Size.Empty;

            if (contextualTabGroups.Count > 0)
            {
                if (this.GetLeftMostGroup(this.IsDesignMode) == null || this.GetRightMostGroup(this.IsDesignMode) == null)
                {
                    this.captionTextElement.Measure(availableSize);
                }
                else if (!this.showTabGroups)
                {
                    this.captionTextElement.Measure(availableSize);
                }
                else
                {

                    float leftCaptionSpace = this.GetLeftCaptionSpace(availableSize);
                    float rightCaptionSpace = this.GetRightCaptionSpace(availableSize);

                    if (leftCaptionSpace > rightCaptionSpace)
                    {
                        captionSize.Width = leftCaptionSpace;
                        captionSize.Height = availableSize.Height;
                        this.captionTextElement.Measure(captionSize);
                    }
                    else
                    {
                        captionSize.Width = rightCaptionSpace;
                        captionSize.Height = availableSize.Height;
                        this.captionTextElement.Measure(captionSize);
                    }
                }
            }
            else
            {
                this.captionTextElement.Measure(availableSize);
            }

            return this.captionTextElement.DesiredSize;
        }

        private void CaptionArrange(SizeF finalSize)
        {
            RadItemCollection contextualTabGroups = this.contextualTabGroups;
            RectangleF boundingRect = new RectangleF();

            boundingRect.Width = this.captionTextElement.DesiredSize.Width;
            boundingRect.Height = this.captionTextElement.DesiredSize.Height;

            if (contextualTabGroups.Count > 0 && this.showTabGroups)
            {
                ContextualTabGroup rightMost;
                ContextualTabGroup leftMost;

                if ((leftMost = this.GetLeftMostGroup(this.IsDesignMode)) == null
                    || (rightMost = this.GetRightMostGroup(this.IsDesignMode)) == null)
                {
                    float xCoordinate = (finalSize.Width - this.captionTextElement.DesiredSize.Width) / 2;
                    float yCoordinate = (finalSize.Height - this.captionTextElement.DesiredSize.Height) / 2;
                    this.captionTextElement.Arrange(new RectangleF(
                        xCoordinate,
                        yCoordinate,
                        this.captionTextElement.DesiredSize.Width,
                        this.captionTextElement.DesiredSize.Height));

                    return;
                }

                float leftCaptionSpace = this.GetLeftCaptionSpace(finalSize);
                float rightCaptionSpace = this.GetRightCaptionSpace(finalSize);
                boundingRect.Y = (finalSize.Height - this.captionTextElement.DesiredSize.Height) / 2;

                if (leftCaptionSpace > rightCaptionSpace)
                {
                    if (!this.RightToLeft)
                    {
                        boundingRect.X = leftCaptionSpace / 2 - this.captionTextElement.DesiredSize.Width / 2;
                        
                        this.captionTextElement.Arrange(boundingRect);
                    }
                    else
                    {
                        boundingRect.X =
                            (leftMost.FullBoundingRectangle.X + leftMost.DesiredSize.Width)
                            + leftCaptionSpace / 2
                            - this.captionTextElement.DesiredSize.Width / 2;
                    }

                    this.captionTextElement.Arrange(boundingRect);
                }
                else
                {
                    if (!this.RightToLeft)
                    {
                        float rightMostEdge = rightMost.FullBoundingRectangle.X + rightMost.DesiredSize.Width;
                        boundingRect.X = rightMostEdge + rightCaptionSpace / 2 - this.captionTextElement.DesiredSize.Width / 2;
                    }
                    else
                    {
                        boundingRect.X = rightCaptionSpace / 2 - this.captionTextElement.DesiredSize.Width / 2;
                    }

                    this.captionTextElement.Arrange(boundingRect);
                }
            }
            else
            {
                boundingRect.X = (finalSize.Width - this.captionTextElement.DesiredSize.Width) / 2;
                boundingRect.Y = (finalSize.Height - this.captionTextElement.DesiredSize.Height) / 2;
                this.captionTextElement.Arrange(boundingRect);
            }
        }

        #endregion

        #region Contextual groups layout

        private float GetCalculatedGroupXCoord(ContextualTabGroup tabGroup)
        {
            float groupXCoord;

            if (!this.RightToLeft)
            {
                groupXCoord =
                    this.TransformXToClient(tabGroup.TabItems[0].ControlBoundingRectangle.Location).X;
            }
            else
            {
                int itemIndex = tabGroup.TabItems.Count - 1;
                groupXCoord =
                    this.TransformXToClient(tabGroup.TabItems[itemIndex].ControlBoundingRectangle.Location).X;
            }

            return groupXCoord;
        }

        private bool ShouldShrinkGroup(ContextualTabGroup tabGroup, float desiredGroupWidth, SizeF availableSize)
        {
            float groupXCoord = this.GetCalculatedGroupXCoord(tabGroup);

            bool shouldShrink;

            if (!this.RightToLeft)
            {
                shouldShrink = ((groupXCoord + desiredGroupWidth) -
                                       (availableSize.Width)) > 0;
            }
            else
            {
                shouldShrink = (groupXCoord <= 0);
            }

            return shouldShrink;
        }

        /// <summary>
        /// This method is responsible for measuring the rightmost visible contextual group with associated tabs.
        /// This is a private case method which is called only for the right most group,
        /// since it has to be shrinked when the system buttons panel has to 'step' over it while resizing.
        /// </summary>
        /// <param name="availableSize">The available size for measuring</param>
        /// <param name="tabGroup">The tab group which is to be shrinked</param>
        private SizeF PerformMeasureWithShrink(SizeF availableSize, ContextualTabGroup tabGroup)
        {
            bool shouldShrink = false;

            RadRibbonBarElement ribbonBar = this.ribbonBarElement;

            float groupXCoord = this.GetCalculatedGroupXCoord(tabGroup);
            
            float desiredGroupSize = this.CalculateGroupWidth(tabGroup);
            
            shouldShrink = this.ShouldShrinkGroup(tabGroup, desiredGroupSize, availableSize);

            if (shouldShrink && !this.isShrinking)
            {
                this.isShrinking = true;

                float newWidth;

                if (!this.RightToLeft)
                {
                    newWidth = (availableSize.Width) - groupXCoord;
                }
                else
                {
                    RadPageViewItem firstTabInGroup = tabGroup.TabItems[tabGroup.TabItems.Count - 1] as RadPageViewItem;

                    float firstTabAbsXCoord = firstTabInGroup.ControlBoundingRectangle.X;
                    float systemButtonsRightEdge = this.ribbonBarElement.RibbonCaption.SystemButtons.ControlBoundingRectangle.Right;

                    if (firstTabAbsXCoord - systemButtonsRightEdge < 0)
                    {
                        newWidth = desiredGroupSize - Math.Abs(firstTabAbsXCoord - systemButtonsRightEdge);
                    }
                    else
                    {
                        newWidth = desiredGroupSize;
                    }
                }

                if (newWidth < RIGHTMOSTGROUP_VISIBILITY_TRESHOLD)
                {
                    newWidth = 0;
                    this.showTabGroups = false;
                }
                else if (newWidth > RIGHTMOSTGROUP_VISIBILITY_TRESHOLD + 10)
                {
                    this.showTabGroups = true;
                }

                return new SizeF(newWidth, availableSize.Height);
            }
            else if (this.isShrinking)
            {
                float newWidth;

                if (!this.RightToLeft)
                {
                    newWidth = (availableSize.Width) - groupXCoord;
                }
                else
                {
                    RadPageViewItem firstTabInGroup = tabGroup.TabItems[tabGroup.TabItems.Count - 1] as RadPageViewItem;

                    float firstTabAbsXCoord = firstTabInGroup.ControlBoundingRectangle.X;
                    float systemButtonsRightEdge = this.ribbonBarElement.RibbonCaption.SystemButtons.ControlBoundingRectangle.Right;

                    if (firstTabAbsXCoord - systemButtonsRightEdge < 0)
                    {
                        newWidth = desiredGroupSize - Math.Abs(firstTabAbsXCoord - systemButtonsRightEdge);
                    }
                    else
                    {
                        newWidth = desiredGroupSize;
                    }
                }

                if (newWidth < RIGHTMOSTGROUP_VISIBILITY_TRESHOLD)
                {
                    newWidth = 0;

                    this.showTabGroups = false;
                }
                else if (newWidth > RIGHTMOSTGROUP_VISIBILITY_TRESHOLD + 10)
                {
                    this.showTabGroups = true;
                }

                if (newWidth >= desiredGroupSize)
                {
                    newWidth = desiredGroupSize;

                    this.showTabGroups = true;
                    this.isShrinking = false;
                }

                return new SizeF(newWidth, availableSize.Height);
            }
            else
            {
                if (!this.showTabGroups)
                {
                    this.showTabGroups = true;
                }

                return new SizeF(desiredGroupSize, availableSize.Height);
            }
        }

        /// <summary>
        /// This method is responsible for arranging the rightmost visible contextual group with associated tabs.
        /// This is a private case method which is called only for the right most group,
        /// since it has to be shrinked when the system buttons panel has to 'step' over it while resizing.
        /// </summary>
        /// <param name="finalSize">The final size for arranging</param>
        /// <param name="tabGroup">The tab group which is to be arranged</param>
        private RectangleF PerformArrangeWithShrink(SizeF finalSize, ContextualTabGroup tabGroup)
        {
            RadPageViewItem rightMost = tabGroup.TabItems[tabGroup.TabItems.Count - 1] as RadPageViewItem;

            RectangleF groupRectangle = new RectangleF();

            groupRectangle.Height = tabGroup.DesiredSize.Height;
            groupRectangle.Y = (finalSize.Height - tabGroup.DesiredSize.Height)/2;

            if (this.isShrinking)
            {
                groupRectangle.Width = tabGroup.DesiredSize.Width;
                groupRectangle.X = 0;
            }
            else
            {
                groupRectangle.Width = tabGroup.DesiredSize.Width;
                groupRectangle.X = this.TransformXToClient(rightMost.ControlBoundingRectangle.Location).X;
            }

            return groupRectangle;

        }

        private SizeF ContextGroupsMeasure(SizeF availableSize)
        {
            SizeF result = SizeF.Empty;
            SizeF measureSize = new SizeF();
            ContextualTabGroup rightMost = this.GetRightMostGroup(this.IsDesignMode);

            if (rightMost != this.rightMostGroup)
            {
                this.rightMostGroup = rightMost;
                this.showTabGroups = true;
            }

            int emptyGroupsCount = this.GetEmptyGroupsCount();

            for (int i = this.contextualTabGroups.Count - 1; i > -1; i--)
            {
                ContextualTabGroup tabGroup = this.contextualTabGroups[i] as ContextualTabGroup;

                if (tabGroup != null)
                {
                    if (tabGroup.TabItems.Count == 0)
                    {
                        if (this.IsDesignMode)
                        {
                            float groupWidth = 100;

                            if (emptyGroupsCount > 0 && ((this.rightEmptySpace / emptyGroupsCount) < 100))
                            {
                                groupWidth = ((this.rightEmptySpace) / emptyGroupsCount);
                            }

                            if (groupWidth < 20)
                            {
                                groupWidth = 0;
                            }

                            measureSize = new SizeF(groupWidth, availableSize.Height);
                            result.Width += tabGroup.DesiredSize.Width;
                            result.Height = tabGroup.DesiredSize.Height;
                        }
                        else
                        {
                            measureSize = Size.Empty;
                            result.Width += tabGroup.DesiredSize.Width;
                            result.Height = tabGroup.DesiredSize.Height;
                        }
                    }
                    else
                    {
                        if (tabGroup == rightMost)
                        {
                            SizeF groupSize = this.PerformMeasureWithShrink(availableSize, tabGroup);

                            if (!this.showTabGroups)
                            {
                                measureSize = Size.Empty;
                            }
                            else
                            {
                                measureSize = groupSize;
                            }
                            result.Width += tabGroup.DesiredSize.Width;
                            result.Height = tabGroup.DesiredSize.Height;
                        }
                        else
                        {
                            if (this.showTabGroups)
                            {
                                measureSize = new SizeF(this.CalculateGroupWidth(tabGroup), availableSize.Height);
                                result.Width += tabGroup.DesiredSize.Width;
                                result.Height = tabGroup.DesiredSize.Height;
                            }
                            else
                            {
                                measureSize = SizeF.Empty;
                                result.Width += tabGroup.DesiredSize.Width;
                            }
                        }
                    }
                }

                if (tabGroup.Visibility == ElementVisibility.Visible)
                {
                    tabGroup.Measure(measureSize);
                }
                else
                {
                    tabGroup.Measure(SizeF.Empty);
                }
            }
            
            return result;
        }

        private void ContextGroupsArrange(SizeF endSize)
        {
            float offset = 0;
            ContextualTabGroup rightMostGroup = this.GetRightMostGroup(this.IsDesignMode);
            
            for (int i = 0; i < this.contextualTabGroups.Count; i++)
            {
                ContextualTabGroup tabGroup = this.contextualTabGroups[i] as ContextualTabGroup;
                float yCoord = (endSize.Height - tabGroup.DesiredSize.Height) / 2;
                if (tabGroup != null)
                {
                    if (tabGroup.TabItems.Count == 0)
                    {
                        //Arrange the empty contextual tab groups only in design time
                        if (this.IsDesignMode)
                        {
                            float groupXCoord = 0;

                            if (this.tabStripElement.Items.Count > 0)
                            {
                                RadPageViewItem item = this.tabStripElement.Items[this.tabStripElement.Items.Count - 1] as RadPageViewItem;
                                Point relativeTabLocation = this.TransformXToClient(item.ControlBoundingRectangle.Location);

                                if (!this.RightToLeft)
                                {
                                    groupXCoord = offset + relativeTabLocation.X + item.FullBoundingRectangle.Width;

                                    tabGroup.Arrange(new RectangleF(groupXCoord, yCoord, tabGroup.DesiredSize.Width, tabGroup.DesiredSize.Height));
                                    offset += tabGroup.DesiredSize.Width;

                                }
                                else
                                {
                                    groupXCoord = relativeTabLocation.X;
                                    offset += tabGroup.DesiredSize.Width;
                                    groupXCoord -= offset;

                                    tabGroup.Arrange(new RectangleF(groupXCoord, yCoord, tabGroup.DesiredSize.Width, tabGroup.DesiredSize.Height));
                                }
                            }

                        }
                    }
                    else
                    {
                        RadPageViewItem leftMost = tabGroup.TabItems[0] as RadPageViewItem;
                        RadPageViewItem rightMost = tabGroup.TabItems[tabGroup.TabItems.Count - 1] as RadPageViewItem;

                        if (leftMost != null && rightMost != null)
                        {
                            if (tabGroup == rightMostGroup && this.RightToLeft)
                            {
                                tabGroup.Arrange(this.PerformArrangeWithShrink(endSize, tabGroup));
                            }
                            else
                            {
                                Point relativeTabLocation;

                                if (!this.RightToLeft)
                                {
                                    relativeTabLocation = this.TransformXToClient(leftMost.ControlBoundingRectangle.Location);
                                }
                                else
                                {
                                    relativeTabLocation = this.TransformXToClient(rightMost.ControlBoundingRectangle.Location);
                                }

                                tabGroup.Arrange(new RectangleF(relativeTabLocation.X, yCoord, tabGroup.DesiredSize.Width, tabGroup.DesiredSize.Height));
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
