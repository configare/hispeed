using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using Telerik.WinControls.UI.UIElements.ListBox.Data;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Stack viewport that support UI virtualization. By default the virtualization is on but can be switched
    /// via the Virtualized property.
    /// Generally Virtualized should not be set directly to the viewport but via the RadScrollViewer's Virtualized
    /// property.
    /// </summary>
    public class RadVirtualizedStackViewport : RadStackViewport, IVisualElementProvider, IVirtualViewport
    {
        #region BitState Keys

        internal const ulong UpdateSuspendedStateKey = RadStackViewportLastStateKey << 1;
        internal const ulong IsInResetVisualListStateKey = UpdateSuspendedStateKey << 1;
        internal const ulong VirtualizedStateKey = IsInResetVisualListStateKey << 1;
        internal const ulong RadVirtualizedStackViewportLastStateKey = VirtualizedStateKey;

        #endregion

        #region Fields

        private Point currentScrollPosition = Point.Empty;
        private IVisualElementProvider visualElementProvider;
        private IVirtualizationCollection virtualItems;
        private int indexOfChildToSet;

        #endregion

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            // Maybe the scroll panel should have the default implementation of IVisualElementProvider
            visualElementProvider = this;
            this.BitState[VirtualizedStateKey] = true;
        }

        public override void SuspendLayout(bool recursive)
        {
            base.SuspendLayout(recursive);

            this.BeginUpdate();
        }

        public override void ResumeLayout(bool recursive, bool performLayout)
        {
            base.ResumeLayout(recursive, performLayout);

            if (!base.IsLayoutSuspended)
            {
                this.EndUpdate(performLayout);
            }
        }

        /// <summary>
        /// Gets or sets the UI virtualization mode of the viewport. There is a virtual items collection that is
        /// linked with the scroll viewer that holds this viewport.
        /// When Virtualized is false all items in the virtual items collection will be children of the viewport.
        /// When Virtualized is true the viewport will have children only to fill its visible part on the screen.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool Virtualized
        {
            get
            {
                return this.GetBitState(VirtualizedStateKey);
            }
            set
            {
                this.SetBitState(VirtualizedStateKey, value);
            }
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key != VirtualizedStateKey)
            {
                return;
            }

            if (newValue)
            {
                this.ResetVisualList(0);

                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    this.currentScrollPosition.Y = 0;
                else
                    this.currentScrollPosition.X = 0;

                RadScrollLayoutPanel scrollPanel = (RadScrollLayoutPanel)this.Parent;
                scrollPanel.SetScrollValueInternal(this.currentScrollPosition);
                scrollPanel.ResetScrollParamsInternal();
            }
            else
            {
                this.ClearChildren();
                for (int i = 0; i < this.virtualItems.Count; i++)
                {
                    AddChild((VisualElement)this.virtualItems.GetVirtualData(i));
                }
            }
        }

        private bool IsIntegralSize
        {
            get
            {
                return this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    this.EqualChildrenHeight : this.EqualChildrenWidth;
            }
        }

        /*protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            if (changeOperation == ItemsChangeOperation.Inserted ||
                changeOperation == ItemsChangeOperation.Removed ||
                changeOperation == ItemsChangeOperation.Cleared ||
                changeOperation == ItemsChangeOperation.Sorted)
            {
                // Invalidate the scroll position so the optimization will not omit calls for scrolling
                //this.currentScrollPosition = Point.Empty;
            }
        }*/

        protected override void OnChildDesiredSizeChanged(RadElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            ResetMaxExtentLength(child);
        }


        #region Overrides

        private int maxExtentLength;

        private void ResetMaxExtentLength(RadElement child)
        {
            if (child == null)
            {
                this.maxExtentLength = 0;
            }
            else
            {
                int childLenght = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    (int)Math.Round(child.DesiredSize.Width) : (int)Math.Round(child.DesiredSize.Height);

                if (childLenght >= this.maxExtentLength)
                    this.maxExtentLength = 0;
            }
        }

        // For vertical strip the vertical extent should not be the height of all visual children
        // but should be determined by the logical elements in the ItemsCollection.
        // Don't forget that the visible children are the minimum necessary to fill the visible area.
        protected override Size CalcExtentSize()
        {
            Size res = base.CalcExtentSize();
            if (this.Virtualized)
            {
                if (this.GetBitState(UpdateSuspendedStateKey))
                {
                    ResetMaxExtentLength(null);
                    return res;
                }

                this.maxExtentLength = Math.Max(this.maxExtentLength,
                    this.Orientation == System.Windows.Forms.Orientation.Vertical ? res.Width : res.Height);

                int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    this.currentScrollPosition.Y : this.currentScrollPosition.X;

                if (this.IsIntegralSize)
                {
                    int childrenCount = GetNonCollapsedChildrenNum();
                    if (childrenCount > 0)
                    {
                        VisualElement child = this.virtualItems.GetVirtualData(startingItemIndex) as VisualElement;
                        Size desiredChildSize = GetElementDesiredSize(child);
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        {
                            res.Width = this.maxExtentLength;
                            res.Height = childrenCount * desiredChildSize.Height;
                        }
                        else
                        {
                            res.Width = childrenCount * desiredChildSize.Width;
                            res.Height = this.maxExtentLength;
                        }
                    }
                }
                else
                {
                    // TODO: replace the following code with code for non-integral size
                    int childrenCount = GetNonCollapsedChildrenNum();
                    if (childrenCount > 0)
                    {
                        VisualElement child = this.virtualItems.GetVirtualData(startingItemIndex) as VisualElement;
                        Size desiredChildSize = GetElementDesiredSize(child);
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        {
                            res.Width = this.maxExtentLength;
                            res.Height = childrenCount * desiredChildSize.Height;
                        }
                        else
                        {
                            res.Width = childrenCount * desiredChildSize.Width;
                            res.Height = this.maxExtentLength;
                        }
                    }
                }
            }
            return res;
        }

        protected override int GetNonCollapsedChildrenNum()
        {
            if (this.Virtualized)
            {
                int nonCollapsedVirtualItems = 0;
                for (int i = 0; i < this.virtualItems.Count; i++)
                {
                    RadItem item = this.virtualItems.GetVirtualData(i) as RadItem;
                    if (item != null && item.Visibility != ElementVisibility.Collapsed)
                        nonCollapsedVirtualItems++;
                }
                return nonCollapsedVirtualItems;
            }
            else
            {
                return base.GetNonCollapsedChildrenNum();
            }
        }

        protected override int GetLastFullVisibleItemsNum()
        {
            if (this.Virtualized)
            {
                int itemsCount = this.virtualItems.Count;
                if (itemsCount <= 0)
                    return 0;

                // 1 is the minimum number if there are items
                if (this.GetBitState(UpdateSuspendedStateKey))
                    return 1;

                // !!! If there are items the result must be at least 1 !!!

                if (this.IsIntegralSize)
                {
                    int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                        this.currentScrollPosition.Y : this.currentScrollPosition.X;
                    return Math.Max(1, this.GetNecessaryVisibleItemsCount(startingItemIndex, true));
                }
                else
                {
                    int totalSize = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                        this.Size.Height : this.Size.Width;
                    int sumSize = 0;
                    for (int i = itemsCount - 1; i >= 0; i--)
                    {
                        VisualElement child = this.virtualItems.GetVirtualData(i) as VisualElement;
                        if (child.Visibility != ElementVisibility.Collapsed)
                        {
                            Size childDesiredSize = GetElementDesiredSize(child);
                            sumSize += this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                                childDesiredSize.Height : childDesiredSize.Width;
                            if (sumSize > totalSize)
                            {
                                return Math.Max(1, itemsCount - i - 1);
                            }
                        }
                    }
                    return itemsCount;
                }
            }
            return base.GetLastFullVisibleItemsNum();
        }

        public override void DoScroll(Point oldValue, Point newValue)
        {
            if (this.Virtualized)
            {
                if (this.GetBitState(UpdateSuspendedStateKey))
                {
                    this.currentScrollPosition = newValue;
                    return;
                }

                if (oldValue != this.currentScrollPosition)
                    oldValue = this.currentScrollPosition;
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    if (newValue.Y > oldValue.Y)
                        ScrollDownRight(newValue.Y - oldValue.Y);
                    else if (newValue.Y < oldValue.Y)
                        ScrollUpLeft(oldValue.Y - newValue.Y);

                    this.PositionOffset = new SizeF(-newValue.X, 0);
                }
                else
                {
                    if (newValue.X > oldValue.X)
                        ScrollDownRight(newValue.X - oldValue.X);
                    else if (newValue.X < oldValue.X)
                        ScrollUpLeft(oldValue.X - newValue.X);

                    this.PositionOffset = new SizeF(0, -newValue.Y);
                }
            }
            else
            {
                //ResetVisualList(newValue, true);
                base.DoScroll(oldValue, newValue);
            }

            this.currentScrollPosition = newValue;
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            switch (changeOperation)
            {
                case ItemsChangeOperation.Removed:
                    RadItem item = child as RadItem;
                    if (item == null || !child.IsDisposing)
                    {
                        break;
                    }
                    RadItemVirtualizationCollection collection = this.virtualItems as RadItemVirtualizationCollection;
                    Debug.Assert(collection != null, "Unknown Virtualization collection");

                    int index = collection.IndexOf(item);
                    if (index >= 0)
                    {
                        collection.RemoveAt(index);
                    }
                    break;
            }

            base.OnChildrenChanged(child, changeOperation);
        }

        public override Size ScrollOffsetForChildVisible(RadElement childElement, Point currentScrollValue)
        {
            Size viewportOffset = Size.Empty;

            if (this.Virtualized)
            {
                int childIndex = this.virtualItems.IndexOf(childElement);
                if (childIndex < 0 || childIndex >= this.virtualItems.Count)
                    return Size.Empty;

                int indexOffset = 0;
                int firstVisibleItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    this.currentScrollPosition.Y : this.currentScrollPosition.X;

                int visibleItemsCount = GetNecessaryVisibleItemsCount(firstVisibleItemIndex, true);
                int lastVisibleItemIndex = Math.Max(0, firstVisibleItemIndex + visibleItemsCount - 1);
                
                if (childIndex < firstVisibleItemIndex)
                {
                    indexOffset = childIndex - firstVisibleItemIndex;
                }
                else if (childIndex > lastVisibleItemIndex)
                {
                    indexOffset = childIndex - lastVisibleItemIndex;
                }

                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    viewportOffset.Height = indexOffset;
                }
                else
                {
                    viewportOffset.Width = indexOffset;
                }
            }
            else
            {
                viewportOffset = base.ScrollOffsetForChildVisible(childElement, currentScrollValue);
            }

            return viewportOffset;
        }

        public override Point ResetValue(Point currentValue, Size viewportSize, Size extentSize)
        {
            Point res = base.ResetValue(currentValue, viewportSize, extentSize);
            
            //Console.WriteLine("VirtStrip.ResetValue(): currentValue = {0}; viewportSize = {1}; extentSize = {2}; base = {3}",
            //    currentValue, viewportSize, extentSize, res);

            if (this.Virtualized)
            {
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    res.Y = currentValue.Y;
                else
                    res.X = currentValue.X;

                // TODO: Try to workarround the layout cycling with caching the result
                // this.Children.Clear();
                res = ResetVisualList(res, false);
            }
            else
            {
                //res = ResetVisualList(res, false);
            }

            return res;
        }

        #endregion



        #region ItemsData collection changed

        public void OnItemDataInserted(int index, object itemData)
        {
            if (this.Virtualized)
            {
                if (this.GetBitState(UpdateSuspendedStateKey))
                    return;

                this.SuspendThemes();

                RadElementCollection children = this.Children;

                RadScrollLayoutPanel scrollPanel = (RadScrollLayoutPanel)this.Parent;
                int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    this.currentScrollPosition.Y : this.currentScrollPosition.X;

                if (index < startingItemIndex)
                {
                    // TODO: optimize the code below - ScrollUpLeft() should not be called in that situation
                    Point newValue = scrollPanel.Value;
                    if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    {
                        newValue.Y = Math.Min(newValue.Y + 1, scrollPanel.VerticalScrollBar.Maximum);
                    }
                    else
                    {
                        newValue.X = Math.Min(newValue.X + 1, scrollPanel.HorizontalScrollBar.Maximum);
                    }
                    scrollPanel.SetScrollValueInternal(newValue);
                    this.currentScrollPosition = newValue;

                    //this.ScrollUpLeft(1);
                }
                else
                {
                    //int necessaryItemsCount = GetNecessaryVisibleItemsCount(this.currentScrollPosition, false);
                    //int currentChildrenCount = this.Children.Count;
                    bool addVisualChild = this.CanAddVisualElement();
                    if (!addVisualChild)
                    {
                        addVisualChild = index >= startingItemIndex && index < startingItemIndex + children.Count;
                    }
                    if (addVisualChild)
                    //if (currentChildrenCount < necessaryItemsCount)
                    {
                        VisualElement ve = this.VisualElementProvider.CreateElement(itemData);
                        if (ve != null)
                        {
                            int childIndex = index - startingItemIndex;
                            if (childIndex < 0 || childIndex > children.Count)
                                this.AddChild(ve);
                            else
                                this.InsertChild(childIndex, ve);

                            // Remove visual children that are outside the viewport
                            this.CleanupInvisibleChildren(startingItemIndex);
                        }
                    }
                }

                this.ResumeThemes(true);

                scrollPanel.ResetScrollParamsInternal();
            }
            else
            {
                Debug.Assert(this.Children.Count == (this.virtualItems.Count - 1));
                VisualElement ve = this.VisualElementProvider.CreateElement(itemData);
                InsertChild(index, ve);
                Debug.Assert(this.Children.Count == this.virtualItems.Count);
            }
        }

        public void OnItemDataRemoved(int index, object itemData)
        {
            if (this.Virtualized)
            {
                if (this.GetBitState(UpdateSuspendedStateKey))
                    return;

                this.SuspendThemes();

                RadScrollLayoutPanel scrollPanel = (RadScrollLayoutPanel)this.Parent;
                int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    this.currentScrollPosition.Y : this.currentScrollPosition.X;

                if (index < startingItemIndex)
                {
                    Point newValue = scrollPanel.Value;
                    if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    {
                        newValue.Y = Math.Max(newValue.Y - 1, scrollPanel.VerticalScrollBar.Minimum);
                    }
                    else
                    {
                        newValue.X = Math.Max(newValue.X - 1, scrollPanel.HorizontalScrollBar.Minimum);
                    }
                    scrollPanel.SetScrollValueInternal(newValue);
                    this.currentScrollPosition = newValue;
                }
                else
                {
                    VisualElement ve = this.GetVisibleElement(itemData);
                    if (ve != null)
                    {
                        ResetMaxExtentLength(ve);
                        this.RemoveChild(ve);
                        this.VisualElementProvider.CleanupElement(ve);
                    }

                    // Add visual children if necessary
                    this.FillWithVisibleChildren(startingItemIndex);
                }

                this.ResumeThemes(false);

                scrollPanel.ResetScrollParamsInternal();
            }
            else
            {
                VisualElement ve = (VisualElement)itemData;
                if (this.Children.Contains(ve))
                {
                    Debug.Assert((this.Children.Count - 1) == this.virtualItems.Count);
                    RemoveChild(ve);
                    Debug.Assert(this.Children.Count == this.virtualItems.Count);
                }
            }
        }

        public void OnItemDataSet(int index, object oldItemData, object newItemData)
        {
            if (this.Virtualized)
            {
                if (this.GetBitState(UpdateSuspendedStateKey))
                    return;

                this.SuspendThemes();

                VisualElement ve = this.GetVisibleElement(oldItemData);
                if (ve != null)
                {
                    ResetMaxExtentLength(ve);
                    if (this.VisualElementProvider.IsElementCompatible(ve, newItemData))
                    {
                        this.VisualElementProvider.SetElementData(ve, newItemData);
                    }
                    else
                    {
                        VisualElement newElement = this.VisualElementProvider.CreateElement(newItemData);
                        if (newElement != null)
                        {
                            int indexToInsertNewElement = this.Children.IndexOf(ve);
                            if (indexToInsertNewElement >= 0)
                            {
                                this.RemoveChild(ve);
                                this.VisualElementProvider.CleanupElement(ve);
                                this.InsertChild(indexToInsertNewElement, newElement);
                            }
                        }
                    }
                }
                else
                {
                    if (this.CanAddVisualElement())
                    {
                        VisualElement ve1 = this.VisualElementProvider.CreateElement(newItemData);
                        if (ve1 != null)
                        {
                            this.AddChild(ve1);
                            // Remove visual children that are outside the viewport
                            int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                                this.currentScrollPosition.Y : this.currentScrollPosition.X;
                            this.CleanupInvisibleChildren(startingItemIndex);
                        }
                    }
                }

                this.ResumeThemes(true);
            }
            else
            {
                Debug.Assert(this.Children.Count == this.virtualItems.Count);
                // TODO: Use the VisualElementProvider
                int childIndex = this.Children.IndexOf((VisualElement)oldItemData);
                if (childIndex >= 0)
                {
                    this.Children[childIndex] = (VisualElement)newItemData;
                }
                else
                {
                    AddChild((VisualElement)newItemData);
                }
            }
        }

        public void OnItemsDataClear()
        {
            if (this.Virtualized)
            {
                ResetMaxExtentLength(null);
            }
        }

        public void OnItemsDataClearComplete()
        {
            if (this.Virtualized)
            {
                if (this.GetBitState(UpdateSuspendedStateKey))
                    return;

                this.SuspendThemes();

                RadElement[] removedChildren = new RadElement[this.Children.Count];
                this.Children.CopyTo(removedChildren, 0);

                ClearChildren();

                for (int i = 0; i < removedChildren.Length; i++)
                {
                    this.VisualElementProvider.CleanupElement((VisualElement)removedChildren[i]);
                }

                this.ResumeThemes(false);
            }
            else
            {
                ClearChildren();
                Debug.Assert(this.Children.Count == this.virtualItems.Count);
            }

            this.currentScrollPosition = Point.Empty;
        }

        public void OnItemsDataSort()
        {
        }

        public void OnItemsDataSortComplete()
        {
            if (this.Virtualized)
            {
                ResetMaxExtentLength(null);
                int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    this.currentScrollPosition.Y : this.currentScrollPosition.X;
                this.ResetVisualList(startingItemIndex);
            }
            else
            {
                Debug.Assert(this.Children.Count == this.virtualItems.Count);

                this.BeginUpdate();
                this.Children.Clear();
                RadListBoxItemOwnerCollection items = (RadListBoxItemOwnerCollection)this.virtualItems;
                foreach (RadItem item in items)
                {
                    this.Children.Add(item);
                }
                this.EndUpdate(true);
            }
        }

        public void BeginUpdate()
        {
            this.BitState[UpdateSuspendedStateKey] = true;
        }

        public void EndUpdate()
        {
            EndUpdate(true);
        }

        private void EndUpdate(bool forceUpdate)
        {
            this.BitState[UpdateSuspendedStateKey] = false;

            if (forceUpdate)
            {
                if (this.Virtualized)
                {
                    int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                        this.currentScrollPosition.Y : this.currentScrollPosition.X;
                    this.ResetVisualList(startingItemIndex);

                    this.UpdateLayout();

                    RadScrollLayoutPanel scrollPanel = (RadScrollLayoutPanel)this.Parent;
                    this.currentScrollPosition = scrollPanel.Value;
                    scrollPanel.ResetScrollParamsInternal();
                }
                else
                {
                    Debug.Assert(this.Children.Count == this.virtualItems.Count);
                    this.UpdateLayout();
                }
            }
        }

        #endregion


        #region IVirtualViewport implementation

        /// <summary>
        /// Provider for the visual elements that will be handled by this virtualizing viewport.
        /// VisualElementProvider must never be null - by default it has non-null value.
        /// The default visula element provider assumes identity between the visual element and its data.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVisualElementProvider VisualElementProvider
        {
            get { return visualElementProvider; }
            set { visualElementProvider = value; }
        }

        public void SetVirtualItemsCollection(IVirtualizationCollection virtualItemsCollection)
        {
            this.virtualItems = virtualItemsCollection;
        }


        /// <summary>
        /// Get visible element from its data
        /// </summary>
        /// <param name="data">logical object in the virtualized collection</param>
        /// <returns>visual element that corresponds to the given data or null if the data has not GUI representation</returns>
        public VisualElement GetVisibleElement(object data)
        {
            VisualElement res = data as VisualElement;
            if (res != null && this.Children.Contains(res))
                return res;
            return null;
        }

        #endregion


        #region Default IVisualElementProvider Members

        public VisualElement CreateElement(object data)
        {
            VisualElement result = data as VisualElement;
            if (this.Virtualized)
            {
                SetMinMaxChildSize(result);
            }
            return result;
        }

        public void CleanupElement(VisualElement element)
        {
            
        }

        public bool SetElementData(VisualElement element, object data)
        {
            if (IsElementCompatible(element, data))
            {
                // Set the data to the element
                if (this.Virtualized)
                {
                    if (!Object.ReferenceEquals(element, data))
                    {
                        VisualElement child = (VisualElement)data;
                        SetMinMaxChildSize(child);

                        this.RemoveChildAt(this.indexOfChildToSet);
                        this.InsertChild(this.indexOfChildToSet, child);
                    }
                }
                return true;
            }
            return false;
        }

        public bool IsElementCompatible(VisualElement element, object data)
        {
            if (this.Virtualized)
                return data is VisualElement;
            return Object.ReferenceEquals(element, data);
        }

        #endregion

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < this.Children.Count;
        }

        #region Operations

        private void AddChild(VisualElement child)
        {
            Debug.Assert(this.IsInValidState(true), "RadVirtualizedStackViewPort.AddChild(): Attempting to modify children while in invalid state.");
            Debug.Assert(!this.Children.Contains(child), "RadVirtualizedStackViewPort.AddChild(): Child should not be added when it is already in the Children collection.");

            this.Children.Add(child);
        }

        private void InsertChild(int index, VisualElement child)
        {
            Debug.Assert(this.IsInValidState(true), "RadVirtualizedStackViewPort.InsertChild(): Attempting to modify children while in invalid state.");
            Debug.Assert(!this.Children.Contains(child), "RadVirtualizedStackViewPort.InsertChild(): Child should not be added when it is already in the Children collection.");
            //for re-examine !!!  
            if (this.Children.Contains(child))
            {
                return;
            }

            this.Children.Insert(index, child);
        }

        private void RemoveChild(VisualElement child)
        {
            Debug.Assert(this.IsInValidState(true), "RadVirtualizedStackViewPort.RemoveChild(): Attempting to modify children while in invalid state.");
            Debug.Assert(this.Children.Contains(child), "RadVirtualizedStackViewPort.RemoveChild(): Child should not be removed when it is no in the Children collection.");

            this.Children.Remove(child);
        }

        private void RemoveChildAt(int index)
        {
            Debug.Assert(this.IsInValidState(true), "RadVirtualizedStackViewPort.RemoveChildAt(): Attempting to modify children while in invalid state.");
            Debug.Assert(IsValidIndex(index), "RadVirtualizedStackViewPort.RemoveChildAt(): Invalid index.");

            this.Children.RemoveAt(index);
        }

        private void ClearChildren()
        {
            Debug.Assert(this.IsInValidState(true), "RadVirtualizedStackViewPort.ClearChildren(): Attempting to clear Children while in invalid state.");

            this.Children.Clear();
        }

        private void SuspendThemes()
        {
        }

        private void ResumeThemes(bool forceApply)
        {
        }

        private void ScrollUpLeft(int numItems)
        {
            if (numItems >= this.Children.Count)
            {
                Point newScrollPos = this.currentScrollPosition;
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    newScrollPos.Y -= numItems;
                    if (newScrollPos.Y < 0)
                        newScrollPos.Y = 0;
                    this.currentScrollPosition = newScrollPos;
                    this.ResetVisualList(newScrollPos.Y);
                }
                else
                {
                    newScrollPos.X -= numItems;
                    if (newScrollPos.X < 0)
                        newScrollPos.X = 0;
                    this.currentScrollPosition = newScrollPos;
                    this.ResetVisualList(newScrollPos.X);
                }

                return;
            }

            this.SuspendThemes();

            int itemIndexToStartAdding = 0;
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                itemIndexToStartAdding = this.currentScrollPosition.Y;
            else
                itemIndexToStartAdding = this.currentScrollPosition.X;

            int indexToRemove = this.Children.Count - numItems;
            int i;
            for (i = 0; i < numItems; i++)
                this.RemoveChildAt(indexToRemove);

            int virtualIndex = itemIndexToStartAdding - 1;
            for (i = 0; i < numItems && virtualIndex >= 0; i++, virtualIndex--)
                this.InsertChild(0, (VisualElement)this.virtualItems.GetVirtualData(virtualIndex));

            int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                this.currentScrollPosition.Y : this.currentScrollPosition.X;
            startingItemIndex -= numItems;
            this.FillWithVisibleChildren(Math.Max(0, startingItemIndex));

            this.ResumeThemes(true);
        }

        private void ScrollDownRight(int numItems)
        {
            if (numItems >= this.Children.Count)
            {
                Point newScrollPos = this.currentScrollPosition;
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    newScrollPos.Y += numItems;
                    this.currentScrollPosition = newScrollPos;
                    this.ResetVisualList(newScrollPos.Y);
                }
                else
                {
                    newScrollPos.X += numItems;
                    this.currentScrollPosition = newScrollPos;
                    this.ResetVisualList(newScrollPos.X);
                }

                return;
            }

            this.SuspendThemes();

            int itemIndexToStartAdding = 0;
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                itemIndexToStartAdding = this.currentScrollPosition.Y + this.Children.Count;
            else
                itemIndexToStartAdding = this.currentScrollPosition.X + this.Children.Count;

            int i;
            for (i = 0; i < numItems; i++)
                this.RemoveChildAt(0);

            int virtualIndex = itemIndexToStartAdding;
            for (i = 0; i < numItems && virtualIndex < this.virtualItems.Count; i++, virtualIndex++)
                this.AddChild((VisualElement)this.virtualItems.GetVirtualData(virtualIndex));

            this.ResumeThemes(true);
        }

        private void ResetVisualList(int startingItemIndex)
        {
            Debug.Assert(this.Virtualized);

            if (this.GetBitState(UpdateSuspendedStateKey))
                return;

            this.SuspendThemes();

            int necessaryItemsCount = GetNecessaryVisibleItemsCount(startingItemIndex, false);
            this.ClearChildren();

            for (int i = 0; i < necessaryItemsCount; i++)
            {
                int virtualIndex = startingItemIndex + i;
                if (virtualIndex >= this.virtualItems.Count)
                    break;

                object itemData = this.virtualItems.GetVirtualData(virtualIndex);
                VisualElement child = this.VisualElementProvider.CreateElement(itemData);

                this.AddChild(child);
            }

            this.ResumeThemes(true);
        }

        private Point ResetVisualList(Point scrollValue, bool isScrolling)
        {
            Debug.Assert(this.Virtualized);

            Point res = scrollValue;

            if (this.GetBitState(UpdateSuspendedStateKey))
                return res;

            if (!this.GetBitState(IsInResetVisualListStateKey))
            {
                this.SuspendThemes();

                this.BitState[IsInResetVisualListStateKey] = true;
                //TraceChildren(scrollValue);

                int leftTopScrollValue = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                    this.currentScrollPosition.Y : this.currentScrollPosition.X;
                for (int i = leftTopScrollValue; i < this.virtualItems.Count; i++)
                {
                    object data = this.virtualItems.GetVirtualData(i);
                    this.indexOfChildToSet = i - leftTopScrollValue;

                    // Remove all children that are not visible (if there are such)
                    // This code leaves sometimes one totaly invisible element in the children collection
                    // but prevents cycling when switching Virtualized property.
                    if (!this.CanAddVisualElement(this.indexOfChildToSet))
                    {
                        RemoveChildrenAfter(this.indexOfChildToSet + 1);
                        break;
                    }

                    if (this.indexOfChildToSet < this.Children.Count)
                    {
                        if (!isScrolling && this.currentScrollPosition != scrollValue)
                        {
                            // replace visual child
                            VisualElement ve = this.Children[this.indexOfChildToSet] as VisualElement;
                            if (this.VisualElementProvider.IsElementCompatible(ve, data))
                            {
                                this.VisualElementProvider.SetElementData(ve, data);
                            }
                            else
                            {
                                this.RemoveChild(ve);
                                VisualElement newChild = this.VisualElementProvider.CreateElement(data);
                                if (newChild != null)
                                    this.InsertChild(this.indexOfChildToSet, newChild);
                            }
                        }
                    }
                    else
                    {
                        // add visual child
                        VisualElement ve = this.VisualElementProvider.CreateElement(data);
                        if (ve != null)
                            this.AddChild(ve);
                    }
                }

                //if (!isScrolling)
                //if (leftTopScrollValue + this.Children.Count < this.virtualItems.Count)
                {
                    // While resizing and the scroll is at the end items must be insreted as first children
                    int necessaryVisibleItems = GetNecessaryVisibleItemsCount(leftTopScrollValue, true);
                    int itemsToInsert = necessaryVisibleItems - this.Children.Count;
                    itemsToInsert = Math.Min(itemsToInsert, leftTopScrollValue);
                    if (itemsToInsert > 0)
                    {
						for (int i = 0; i < itemsToInsert; i++)
						{
                            if (this.virtualItems.Count > leftTopScrollValue - i - 1)
                                this.InsertChild(0, (VisualElement)this.virtualItems.GetVirtualData(leftTopScrollValue - i - 1));
						}

                        int newScrollPos = leftTopScrollValue - itemsToInsert;
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        {
                            res.Y = newScrollPos;
                            // Stop DoScroll from calling ScrollUpLeft
                            this.currentScrollPosition.Y = newScrollPos;
                        }
                        else
                        {
                            res.X = newScrollPos;
                            // Stop DoScroll from calling ScrollUpLeft
                            this.currentScrollPosition.X = newScrollPos;
                        }
                    }
                }

                //this.LayoutEngine.PerformLayout(this, true);
                //this.LayoutEngine.PerformParentLayout();

                this.ResumeThemes(true);

                this.BitState[IsInResetVisualListStateKey] = false;
            }
            
            return res;

#if false
            // The orientation is considered in PixelOffsetToIndexOffset
            int leftTopScrollValue = PixelOffsetToIndexOffset(scrollValue);
            for (int i = leftTopScrollValue; i < virtualItems.Count; i++)
            {
                object data = virtualItems[i];
                if (i - leftTopScrollValue < this.Children.Count)
                {
                    /// replace visual child
                    VisualElement ve = this.Children[i - leftTopScrollValue] as VisualElement;
                    if (this.VisualElementProvider.IsElementCompatible(ve, data))
                    {
                        this.VisualElementProvider.SetElementData(ve, data);
                    }
                    else
                    {
                        this.Children.Remove(ve);
                        VisualElement newChild = this.VisualElementProvider.CreateElement(data);
                        if (newChild != null)
                            this.Children.Insert(i - leftTopScrollValue, newChild);
                    }
                }
                else
                {
                    // add visual child
                    VisualElement ve = this.VisualElementProvider.CreateElement(data);
                    if (ve != null)
                        this.Children.Add(ve);
                }
                // Put this check at the end to ensure at least one visual child in the Children collection
                // Remove all children that are not visible (if there are such)
                if (!this.CanAddVisualElement())
                {
                    this.Children.RemoveAfter(i - leftTopScrollValue);
                    break;
                }
            }
#endif
        }

        #endregion

        #region Helpers

        // Get the number of items that can fill the viewport
        // The last one could be partially visible - use the argument fullyVisible to determine
        // whether to include or not that last item.
        // This method is used only in virtual mode
        private int GetNecessaryVisibleItemsCount(int startingItemIndex, bool fullyVisible)
        {
            Debug.Assert(this.Virtualized);

            if (this.Orientation == System.Windows.Forms.Orientation.Vertical && this.Size.Height == 0)
                return 0;
            else if (this.Orientation == System.Windows.Forms.Orientation.Horizontal && this.Size.Width == 0)
                return 0;

            int visibleItemsCount = 0;

            int itemsCount = this.virtualItems.Count;
            if (itemsCount > 0)
            {
                if (this.IsIntegralSize)
                {
                    VisualElement child = this.virtualItems.GetVirtualData(startingItemIndex) as VisualElement;
                    if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    {
                        int childHeight = Math.Max(1, GetElementDesiredSize(child).Height);
                        visibleItemsCount = this.Size.Height / childHeight;
                        if (!fullyVisible && (this.Size.Height % childHeight) > 0)
                            visibleItemsCount++;
                    }
                    else
                    {
                        int childWidth = Math.Max(1, GetElementDesiredSize(child).Width);
                        visibleItemsCount = this.Size.Width / childWidth;
                        if (!fullyVisible && (this.Size.Width % childWidth) > 0)
                            visibleItemsCount++;
                    }
                }
                else
                {
                    int totalSize = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                        this.Size.Height : this.Size.Width;
                    int sumSize = 0;
                    for (int i = startingItemIndex; i < itemsCount; i++)
                    {
                        VisualElement child = this.virtualItems.GetVirtualData(i) as VisualElement;
                        if (child.Visibility != ElementVisibility.Collapsed)
                        {
                            Size childDesiredSize = GetElementDesiredSize(child);
                            sumSize += this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                                childDesiredSize.Height : childDesiredSize.Width;
                            if (sumSize == totalSize)
                            {
                                return i - startingItemIndex + 1;
                            }
                            else if (sumSize > totalSize)
                            {
                                int fullyVisibleItems = i - startingItemIndex;
                                return fullyVisible ? fullyVisibleItems : fullyVisibleItems + 1;
                            }
                        }
                    }
                    return itemsCount;
                }
            }

            return Math.Max(0, Math.Min(itemsCount, visibleItemsCount));
        }

        private Size GetElementDesiredSize(RadElement child)
        {
            if (child == null)
                return Size.Empty;

            if (child.GetBitState(NeverMeasuredStateKey))
            {
                return new Size(16, 16);
            }

            return Size.Round(child.DesiredSize);
        }

        /// <summary>
        /// Check whether there is non occupied by visual elements space in the viewport.
        /// </summary>
        /// <returns>true if a newly added visual element will be at least partially visible</returns>
        private bool CanAddVisualElement()
        {
            return this.CanAddVisualElement(this.Children.Count);
        }

        private bool CanAddVisualElement(int visibleElementsNum)
        {
            Debug.Assert(this.Virtualized);

            if (this.Orientation == System.Windows.Forms.Orientation.Vertical && this.Size.Height == 0)
                return false;
            else if (this.Orientation == System.Windows.Forms.Orientation.Horizontal && this.Size.Width == 0)
                return false;

            bool res = true;
            if (this.IsIntegralSize)
            {
                if (this.virtualItems.Count > 0)
                {
                    int startingItemIndex = this.Orientation == System.Windows.Forms.Orientation.Vertical ?
                        this.currentScrollPosition.Y : this.currentScrollPosition.X;
                    VisualElement child = this.virtualItems.GetVirtualData(startingItemIndex) as VisualElement;
                    if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        res = this.Size.Height > visibleElementsNum * GetElementDesiredSize(child).Height;
                    else
                        res = this.Size.Width > visibleElementsNum * GetElementDesiredSize(child).Width;
                }
                else
                {
                    res = false;
                }
            }
            return res;
        }

        private void RemoveChildrenAfter(int index)
        {
            if (index >= 0 && index < this.Children.Count)
            {
                while (this.Children.Count > index)
                    this.RemoveChildAt(index);
            }
        }

        private void FillWithVisibleChildren(int startingItemIndex)
        {
            Debug.Assert(this.Virtualized);

            int necessaryItemsCount = GetNecessaryVisibleItemsCount(startingItemIndex, false);
            int currentChildrenCount = this.Children.Count;
            int itemsToAdd = necessaryItemsCount - currentChildrenCount;
            for (int i = 0; i < itemsToAdd; i++)
            {
                int virtualIndex = startingItemIndex + currentChildrenCount + i;
                if (virtualIndex >= this.virtualItems.Count)
                    break;

                object itemData = this.virtualItems.GetVirtualData(virtualIndex);
                VisualElement child = this.VisualElementProvider.CreateElement(itemData);
                this.AddChild(child);
            }
        }

        private void CleanupInvisibleChildren(int startingItemIndex)
        {
            Debug.Assert(this.Virtualized);

            int necessaryItemsCount = GetNecessaryVisibleItemsCount(startingItemIndex, false);
            int currentChildrenCount = this.Children.Count;
            int itemsToRemove = currentChildrenCount - necessaryItemsCount;
            for (int i = 0; i < itemsToRemove; i++)
            {
                this.RemoveChildAt(necessaryItemsCount);
            }
        }

        private void SetMinMaxChildSize(VisualElement child)
        {
            /*if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                //child.MinSize = new Size(ElementWidth, child.MinSize.Height);
                child.MaxSize = new Size(ElementWidth, child.MaxSize.Height);
            }
            else
            {
                //child.MinSize = new Size(child.MinSize.Width, ElementHeight);
                child.MaxSize = new Size(child.MaxSize.Width, ElementHeight);
            }*/
        }

        private void TraceChildren(Point scrollValue)
        {
            Size sz = this.Size;
            for (int i = 0; i < this.Children.Count; i++)
            {
                /*RadItem child = this.Children[i] as RadItem;
                if (child != null)
                {
                    Console.WriteLine("VirtStrip.ResetVisualList(): scrollValue = {0}; child[{1}]({2}) = {3} / {4}",
                        scrollValue, i, child.Text, child.Size, child.GetPreferredSize(sz));
                }
                else
                {
                    Console.WriteLine("VirtStrip.ResetVisualList(): scrollValue = {0}; child[{1}] = null",
                        scrollValue, i);
                }*/
                //this.Children[i].PerformLayoutCore(this);
            }
        }

        #endregion
    }
}
