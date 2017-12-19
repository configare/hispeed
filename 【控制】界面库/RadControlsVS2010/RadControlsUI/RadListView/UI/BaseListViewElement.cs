using System;
using System.Collections.Generic;
using Telerik.WinControls.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a base class for <see cref="RadListView"/> view elements.
    /// </summary>
    public abstract class BaseListViewElement : VirtualizedScrollPanel<ListViewDataItem, BaseListViewVisualItem>
    {
        #region RadProperties

        public static RadProperty ItemSizeProperty = RadProperty.Register(
            "ItemSize", typeof(Size), typeof(BaseListViewElement), new RadElementPropertyMetadata(
               new Size(200, 20), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty GroupItemSizeProperty = RadProperty.Register(
            "GroupItemSize", typeof(Size), typeof(BaseListViewElement), new RadElementPropertyMetadata(
               new Size(200, 20), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty GroupIndentProperty = RadProperty.Register(
            "GroupIndent", typeof(int), typeof(BaseListViewElement), new RadElementPropertyMetadata(
                25, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsArrange));

        public static RadProperty AllowArbitraryItemHeightProperty = RadProperty.Register(
            "AllowArbitraryItemHeight", typeof(bool), typeof(BaseListViewElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty AllowArbitraryItemWidthProperty = RadProperty.Register(
            "AllowArbitraryItemWidth", typeof(bool), typeof(BaseListViewElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty FullRowSelectProperty = RadProperty.Register(
            "FullRowSelect", typeof(bool), typeof(BaseListViewElement), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay));
        #endregion
         
        #region Fields

        protected RadListViewElement owner;
        protected ListViewDataItem anchor = null;
        protected ScrollServiceBehavior scrollBehavior;

        protected Timer beginEditTimer = new Timer();
        protected ListViewDataItem lastClickedItem = null;
        protected bool disableEditOnMouseUp = false;

        protected Timer groupSelectionTimer = new Timer();
        protected Keys lastModifierKeys = Keys.None;
        protected bool disableGroupSelectOnMouseUp;
        
        #endregion

        #region Constructor

        public BaseListViewElement(RadListViewElement owner)
        {
            this.owner = owner;
            this.ViewElement.FitElementsToSize = this.FullRowSelect;
            this.Items = owner.Items;
            this.Scroller.ScrollMode = ItemScrollerScrollModes.Smooth;
            this.Scroller.ScrollerUpdated += new EventHandler(Scroller_ScrollerUpdated);

            this.scrollBehavior = new ScrollServiceBehavior();
            this.scrollBehavior.ScrollServices.Add(new ScrollService(this.ViewElement, this.Scroller.Scrollbar));

            this.beginEditTimer = new Timer();
            this.beginEditTimer.Interval = SystemInformation.DoubleClickTime + 10;
            this.beginEditTimer.Tick += new EventHandler(beginEditTimer_Tick);

            this.groupSelectionTimer = new Timer();
            this.groupSelectionTimer.Interval = SystemInformation.DoubleClickTime + 10;
            this.groupSelectionTimer.Tick += new EventHandler(groupSelectionTimer_Tick);
        }

        #endregion

        #region Properties
         
        /// <summary>
        /// Gets the <see cref="ScrollServiceBehavior"/> that is responsible for the kinetic scrolling option.
        /// </summary>
        public ScrollServiceBehavior ScrollBehavior
        {
            get
            {
                return this.scrollBehavior;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the view element.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                if (this.Orientation != value)
                {
                    base.Orientation = value;
                    this.OnOrientationChanged();
                }
            }

        }

        /// <summary>
        /// Gets the <see cref="RadListViewElement"/> that owns the view.
        /// </summary>
        public RadListViewElement Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items can have different height.
        /// </summary>
        public bool AllowArbitraryItemHeight
        {
            get
            {
                return (bool)this.GetValue(AllowArbitraryItemHeightProperty);
            }
            set
            {
                this.SetValue(AllowArbitraryItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items can have different width.
        /// </summary>
        public bool AllowArbitraryItemWidth
        {
            get
            {
                return (bool)this.GetValue(AllowArbitraryItemWidthProperty);
            }
            set
            {
                this.SetValue(AllowArbitraryItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the full row should be selected.
        /// </summary>
        public virtual bool FullRowSelect
        {
            get
            {
                return (bool)this.GetValue(FullRowSelectProperty);
            }
            set
            {
                this.SetValue(FullRowSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the default item size.
        /// </summary>
        public Size ItemSize
        {
            get
            {
                return (Size)this.GetValue(ItemSizeProperty);
            }
            set
            {
                this.SetValue(ItemSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the default group item size.
        /// </summary>
        public Size GroupItemSize
        {
            get
            {
                return (Size)this.GetValue(GroupItemSizeProperty);
            }
            set
            {
                this.SetValue(GroupItemSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indent of the items when they are displayed in a group.
        /// </summary>
        public int GroupIndent
        {
            get
            {
                return (int)this.GetValue(GroupIndentProperty);
            }
            set
            {
                this.SetValue(GroupIndentProperty, value);
            }
        }
        
        #endregion

        #region Overrides
        
        protected override VirtualizedStackContainer<ListViewDataItem> CreateViewElement()
        {
            return new BaseListViewContainer(this);
        }

        protected override ITraverser<ListViewDataItem> CreateItemTraverser(IList<ListViewDataItem> items)
        {
            return new ListViewTraverser(this.owner);
        }

        protected override IVirtualizedElementProvider<ListViewDataItem> CreateElementProvider()
        {
            return new ListViewVirtualizedElementProvider(this);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FullRowSelectProperty)
            {
                this.ViewElement.FitElementsToSize = this.FullRowSelect;
            }

            if (e.Property == FullRowSelectProperty ||
                e.Property == AllowArbitraryItemHeightProperty ||
                e.Property == AllowArbitraryItemWidthProperty ||
                e.Property == ItemSizeProperty ||
                e.Property == GroupIndentProperty ||
                e.Property == GroupItemSizeProperty)
            {
                this.owner.Update(RadListViewElement.UpdateModes.RefreshAll);
            }
        }

        protected override bool UpdateOnMeasure(SizeF availableSize)
        {
            bool result = base.UpdateOnMeasure(availableSize);
            
            ElementVisibility oldVisibility = this.HScrollBar.Visibility;

            RectangleF clientRect = GetClientRectangle(availableSize);
            HScrollBar.LargeChange = (int)(clientRect.Width - VScrollBar.DesiredSize.Width - this.ViewElement.Margin.Horizontal);
            HScrollBar.SmallChange = HScrollBar.LargeChange / 10;

            this.UpdateHScrollbarMaximum();

            SizeF clientSize = clientRect.Size;
            if (this.HScrollBar.Visibility != ElementVisibility.Collapsed)
            {
                clientSize.Height -= HScrollBar.DesiredSize.Height;
            }
            this.Scroller.ClientSize = clientSize;

            if (this.HScrollBar.Visibility != oldVisibility)
            {
                return true;
            }

            return result;
        }

        protected override void DisposeManagedResources()
        {
            this.beginEditTimer.Stop();
            this.beginEditTimer.Tick -= new EventHandler(beginEditTimer_Tick);
            base.DisposeManagedResources();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the <see cref="ListViewDataItem"/> at a specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The <see cref="ListViewDataItem"/>.</returns>
        public ListViewDataItem GetItemAt(Point location)
        {
            BaseListViewVisualItem itemAtPoint = this.GetVisualItemAt(location);

            return (itemAtPoint != null) ? itemAtPoint.Data : null;
        }

        /// <summary>
        /// Gets the <see cref="BaseListViewVisualItem"/> at a specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The <see cref="BaseListViewVisualItem"/>.</returns>
        public virtual BaseListViewVisualItem GetVisualItemAt(Point location)
        {
            return this.ElementTree.GetElementAtPoint(location) as BaseListViewVisualItem;
        }

        /// <summary>
        /// Scrolls the view with a given amount.
        /// </summary>
        /// <param name="delta">The amount to scroll with.</param>
        public void ScrollTo(int delta)
        {
            RadScrollBarElement scrollBar = this.Scroller.Scrollbar;

            int result = scrollBar.Value - delta * scrollBar.SmallChange;
            if (result > scrollBar.Maximum - scrollBar.LargeChange + 1)
            {
                result = scrollBar.Maximum - scrollBar.LargeChange + 1;
            }

            if (result < scrollBar.Minimum)
            {
                result = 0;
            }
            else if (result > scrollBar.Maximum)
            {
                result = scrollBar.Maximum;
            }

            scrollBar.Value = result;
        }

        /// <summary>
        /// Ensures that a given <see cref="ListViewDataItem"/> is visible on the client area.
        /// </summary>
        /// <param name="item">The <see cref="ListViewDataItem"/> to ensure visibility of.</param>
        public virtual void EnsureItemVisible(ListViewDataItem item)
        {
            this.EnsureItemVisible(item, false);
        }

        /// <summary>
        /// Ensures that a given <see cref="ListViewDataItem"/> is visible on the client area.
        /// </summary>
        /// <param name="item">The <see cref="ListViewDataItem"/> to ensure visibility of.</param>
        /// <param name="ensureHorizontally">Indicates if the view should be scrolled horizontally.</param>
        public virtual void EnsureItemVisible(ListViewDataItem item, bool ensureHorizontally)
        {
            this.EnsureItemVisibleVertical(item);
            
            if (ensureHorizontally)
            {
                this.EnsureItemVisibleHorizontal(item);
            }

            this.UpdateLayout();
        }

        /// <summary>
        /// Clears the selection.
        /// </summary>
        public void ClearSelection()
        {
            this.owner.SelectedItems.Clear();
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Ensures that a given <see cref="ListViewDataItem"/> is visible by scrolling the view horizontally.
        /// </summary>
        /// <param name="item">The item to ensure visibility of.</param>
        protected virtual void EnsureItemVisibleHorizontal(ListViewDataItem item)
        {

        }


        /// <summary>
        /// Ensures that a given <see cref="ListViewDataItem"/> is visible by scrolling the view vertically.
        /// </summary>
        /// <param name="item">The item to ensure visibility of.</param>
        protected virtual void EnsureItemVisibleVertical(ListViewDataItem item)
        {
            if (item == null)
            {
                return;
            }

            BaseListViewVisualItem visualItem = this.GetElement(item);

            if (visualItem == null)
            {
                this.UpdateLayout();

                if (this.ViewElement.Children.Count > 0)
                {

                    int itemIndex = GetItemIndex(item);
                    int firstVisibleIndex = GetItemIndex(((BaseListViewVisualItem)this.ViewElement.Children[0]).Data);

                    if (itemIndex <= firstVisibleIndex)
                    {
                        this.Scroller.ScrollToItem(item);
                    }
                    else
                    {
                        EnsureItemVisibleVerticalCore(item);
                    }
                }
            }
            else
            {
                if (visualItem.ControlBoundingRectangle.Bottom > this.ViewElement.ControlBoundingRectangle.Bottom)
                {
                    int offset = visualItem.ControlBoundingRectangle.Bottom - this.ViewElement.ControlBoundingRectangle.Bottom;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + offset);
                }
                else if (visualItem.ControlBoundingRectangle.Top < this.ViewElement.ControlBoundingRectangle.Top)
                {
                    int offset = this.ViewElement.ControlBoundingRectangle.Top - visualItem.ControlBoundingRectangle.Top;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value - offset);
                }
            }
        }

        /// <summary>
        /// Ensures that a given <see cref="ListViewDataItem"/> is visible when it is below the last visible item in the view.
        /// </summary>
        /// <param name="item">The item to ensure visibility of.</param>
        protected virtual void EnsureItemVisibleVerticalCore(ListViewDataItem item)
        {
            if (item == null)
            {
                return;
            }

            bool start = false;
            int offset = 0;
            ListViewDataItem lastVisible = ((BaseListViewVisualItem)this.ViewElement.Children[this.ViewElement.Children.Count - 1]).Data;
            ListViewTraverser traverser = (ListViewTraverser)this.Scroller.Traverser.GetEnumerator();
            BaseListViewVisualItem visualItem = null;

            while (traverser.MoveNext())
            {
                if (traverser.Current == item)
                {
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + offset);
                    this.UpdateLayout();
                    visualItem = this.GetElement(item);

                    if (visualItem != null &&
                        visualItem.ControlBoundingRectangle.Bottom > this.ViewElement.ControlBoundingRectangle.Bottom)
                    {
                        this.EnsureItemVisible(item);
                    }
                    break;
                }
                if (traverser.Current == lastVisible)
                {
                    start = true;
                }
                if (start)
                {
                    offset += (int)ViewElement.ElementProvider.GetElementSize(traverser.Current).Height + this.ItemSpacing;
                }
            }
        }

        /// <summary>
        /// Called when the orientation of the view has changed.
        /// </summary>
        protected virtual void OnOrientationChanged()
        {

        }

        /// <summary>
        /// Updates the horizontal scrollbar.
        /// </summary>
        internal virtual void UpdateHScrollbarMaximum()
        {

        }

        /// <summary>
        /// Updates the visibility of the horizontal scrollbar.
        /// </summary>
        protected virtual void UpdateHScrollbarVisibility()
        {

        }
        
        /// <summary>
        /// Processes the MouseUp event.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <returns>true if the processing of the event should be stopped, false otherwise.</returns>
        internal virtual bool ProcessMouseUp(MouseEventArgs e)
        {
            if (this.owner.EnableKineticScrolling && this.ViewElement.ContainsMouse)
            {
                this.scrollBehavior.MouseUp(e.Location);
            }
            else
            {
                this.scrollBehavior.Stop();
            }

            if (e.Button == MouseButtons.Left)
            {
                ListViewDataItem itemUnderMouse = this.GetItemAt(e.Location);
                if (itemUnderMouse == null || !itemUnderMouse.Enabled)
                {
                    this.lastModifierKeys = Keys.None;
                    this.groupSelectionTimer.Stop();
                    this.beginEditTimer.Stop();
                    this.lastClickedItem = null;
                    return false;
                }

                if (itemUnderMouse is ListViewDataItemGroup)
                {
                    if (!this.disableGroupSelectOnMouseUp && this.owner.MultiSelect)
                    {
                        this.lastClickedItem = itemUnderMouse;
                        this.lastModifierKeys = Control.ModifierKeys;
                        this.groupSelectionTimer.Start();
                    }
                    else if (!this.owner.MultiSelect)
                    {
                        this.ProcessSelection(itemUnderMouse, Control.ModifierKeys, true);
                    }
                    else
                    {
                        this.lastClickedItem = null;
                        this.lastModifierKeys = Keys.None;
                        this.groupSelectionTimer.Stop();
                    }
                    
                    return false;
                }

                this.lastClickedItem = null;
                this.lastModifierKeys = Keys.None;
                this.groupSelectionTimer.Stop();

                if (itemUnderMouse != null && !disableEditOnMouseUp &&
                    itemUnderMouse == this.owner.SelectedItem &&
                    Control.ModifierKeys == Keys.None &&
                    this.lastClickedItem == null)
                {
                    this.lastClickedItem = itemUnderMouse;
                    this.beginEditTimer.Start();
                }
                else
                {
                    this.beginEditTimer.Stop();
                    this.lastClickedItem = null;
                    this.ProcessSelection(itemUnderMouse, Control.ModifierKeys, true);
                }
            }
            else
            {
                this.lastModifierKeys = Keys.None;
                this.groupSelectionTimer.Stop();
                this.beginEditTimer.Stop();
                this.lastClickedItem = null;
            }

            return false;
        }

        /// <summary>
        /// Processes the MouseMove event.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <returns>true if the processing of the event should be stopped, false otherwise.</returns>
        internal virtual bool ProcessMouseMove(MouseEventArgs e)
        {
            if (this.owner.EnableKineticScrolling && this.ViewElement.ContainsMouse)
            {
                this.scrollBehavior.MouseMove(e.Location);
            }

            return false;
        }

        /// <summary>
        /// Processes the MouseDown event.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <returns>true if the processing of the event should be stopped, false otherwise.</returns>
        internal virtual bool ProcessMouseDown(MouseEventArgs e)
        {
            this.beginEditTimer.Stop();
            this.lastClickedItem = null;
            this.lastModifierKeys = Keys.None;
            this.groupSelectionTimer.Stop();

            disableGroupSelectOnMouseUp = (e.Clicks != 1);
            disableEditOnMouseUp = (e.Clicks != 1);

            if (this.owner.EnableKineticScrolling && this.ViewElement.ContainsMouse)
            {
                this.scrollBehavior.MouseDown(e.Location);
            }
            else
            {
                this.scrollBehavior.Stop();
            }

            return false;
        }

        /// <summary>
        /// Processes the KeyDown event.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <returns>true if the processing of the event should be stopped, false otherwise.</returns>
        internal virtual bool ProcessKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    HandleDownKey(e);
                    break;
                case Keys.Up:
                    HandleUpKey(e);
                    break;
                case Keys.Left:
                    HandleLeftKey(e);
                    break;
                case Keys.Right:
                    HandleRightKey(e);
                    break;
                case Keys.Home:
                    HandleHomeKey(e);
                    break;
                case Keys.End:
                    HandleEndKey(e);
                    break;
                case Keys.PageDown:
                    HandlePageDownKey(e);
                    break;
                case Keys.PageUp:
                    HandlePageUpKey(e);
                    break;
                case Keys.Space:
                    HandleSpaceKey(e);
                    break;
                case Keys.F2:
                    HandleF2Key(e);
                    break;
                case Keys.Escape:
                    HandleEscapeKey(e);
                    break;
                case Keys.Delete:
                    HandleDeleteKey(e);
                    break;
            }

            return false;
        }

        /// <summary>
        /// Handles a press of the PageUp key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandlePageUpKey(KeyEventArgs e)
        {
            int delta = this.ControlBoundingRectangle.Height;
            this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value - delta);
        }

        /// <summary>
        /// Handles a press of the PageDown key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandlePageDownKey(KeyEventArgs e)
        {
            int delta = this.ControlBoundingRectangle.Height;
            this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + delta);
        }

        /// <summary>
        /// Handles a press of the Delete key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleDeleteKey(KeyEventArgs e)
        {
            if (this.owner.IsEditing || !this.owner.AllowRemove)
            {
                return;
            }

            this.owner.Items.BeginUpdate();

            List<ListViewDataItem> itemsToRemove = new List<ListViewDataItem>(this.owner.SelectedItems);

            while (itemsToRemove.Count > 0)
            {
                int lastIndex = itemsToRemove.Count - 1;
                
                if (!this.owner.OnItemRemoving(new ListViewItemCancelEventArgs(itemsToRemove[lastIndex])))
                {
                    this.owner.Items.Remove(itemsToRemove[lastIndex]);
                    this.owner.OnItemRemoved(new ListViewItemEventArgs(itemsToRemove[lastIndex]));
                }

                itemsToRemove.RemoveAt(lastIndex);
            }

            this.owner.Items.EndUpdate();
        }

        /// <summary>
        /// Handles a press of the End key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleEndKey(KeyEventArgs e)
        {
            ITraverser<ListViewDataItem> enumerator = this.Scroller.Traverser.GetEnumerator() as ITraverser<ListViewDataItem>;

            while (enumerator.MoveNext()) ;

            if (enumerator.Current != null)
            {
                this.ProcessSelection(enumerator.Current, Control.ModifierKeys, false);
            }
        }

        /// <summary>
        /// Handles a press of the Home key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleHomeKey(KeyEventArgs e)
        {
            ITraverser<ListViewDataItem> enumerator = this.Scroller.Traverser.GetEnumerator() as ITraverser<ListViewDataItem>;

            enumerator.Reset();

            if (enumerator.MoveNext() && enumerator.Current != null)
            {
                this.ProcessSelection(enumerator.Current, Control.ModifierKeys, false);
            }
        }

        /// <summary>
        /// Handles a press of the Escape key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleEscapeKey(KeyEventArgs e)
        {
            this.owner.CancelEdit();
        }

        /// <summary>
        /// Handles a press of the F2 key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleF2Key(KeyEventArgs e)
        {
            this.owner.BeginEdit();
        }

        /// <summary>
        /// Handles a press of the Left key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleLeftKey(KeyEventArgs e)
        {
            ListViewDataItemGroup group = this.owner.CurrentItem as ListViewDataItemGroup;
            if (group != null)
            {
                group.Expanded = false;
            }
        }

        /// <summary>
        /// Handles a press of the Right key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleRightKey(KeyEventArgs e)
        {
            ListViewDataItemGroup group = this.owner.CurrentItem as ListViewDataItemGroup;
            if (group != null)
            {
                group.Expanded = true;
            }
        }

        /// <summary>
        /// Handles a press of the Down key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleDownKey(KeyEventArgs e)
        {
            ListViewDataItem nextItem = GetNextItem(this.owner.CurrentItem);
            if (nextItem != null)
            {
                this.ProcessSelection(nextItem, Control.ModifierKeys, false);
            }
        }

        /// <summary>
        /// Handles a press of the Up key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleUpKey(KeyEventArgs e)
        {
            ListViewDataItem previousItem = GetPreviousItem(this.owner.CurrentItem);
            if(previousItem != null)
            {
                this.ProcessSelection(previousItem, Control.ModifierKeys, false);
            }
        }

        /// <summary>
        /// Handles a press of the Space key.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void HandleSpaceKey(KeyEventArgs e)
        {
            if (this.owner.CurrentItem == null)
            {
                return;
            }

            if (!this.owner.MultiSelect)
            {
                this.ClearSelection();
            }

            this.owner.CurrentItem.Selected = !this.owner.CurrentItem.Selected;
            if (this.owner.CurrentItem.Selected)
            {
                if (this.owner.CurrentItem.Selected)
                {
                    this.owner.SetSelectedItem(this.owner.CurrentItem);
                }
                else
                {
                    this.owner.SetSelectedItem(this.owner.SelectedItems.Count > 0 ?
                                              this.owner.SelectedItems[this.owner.SelectedItems.Count - 1] : null);
                }

                this.anchor = this.owner.SelectedItem;
            }
        }

        /// <summary>
        /// Gets the previous visible item of a given <see cref="ListViewDataItem"/>.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <returns>The previous item.</returns>
        protected virtual ListViewDataItem GetPreviousItem(ListViewDataItem currentItem)
        {
            ListViewTraverser enumerator = this.Scroller.Traverser.GetEnumerator() as ListViewTraverser;
            enumerator.Position = currentItem;

            if (enumerator.MovePrevious())
            {
                return enumerator.Current;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the next visible item of a given <see cref="ListViewDataItem"/>.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <returns>The next item.</returns>
        protected virtual ListViewDataItem GetNextItem(ListViewDataItem currentItem)
        {
            ListViewTraverser enumerator = this.Scroller.Traverser.GetEnumerator() as ListViewTraverser;
            enumerator.Position = currentItem;

            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Processes the MouseWheel event.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <returns>true if the processing of the event should be stopped, false otherwise.</returns>
        internal virtual bool ProcessMouseWheel(MouseEventArgs e)
        {
            this.beginEditTimer.Stop();
            this.lastClickedItem = null;
            this.lastModifierKeys = Keys.None;
            this.groupSelectionTimer.Stop();

            int step = Math.Max(1, e.Delta / SystemInformation.MouseWheelScrollDelta);
            int delta = Math.Sign(e.Delta) * step * SystemInformation.MouseWheelScrollLines;

            this.ScrollTo(delta);

            return false;
        }

        /// <summary>
        /// Processes the selection of a specified item.
        /// </summary>
        /// <param name="item">The <see cref="ListViewDataItem"/> which is being processed.</param>
        /// <param name="modifierKeys">The modifier keys which are pressed during selection.</param>
        /// <param name="isMouseSelection">[true] if the selection is triggered by mouse input, [false] otherwise.</param>
        internal virtual void ProcessSelection(ListViewDataItem item, Keys modifierKeys, bool isMouseSelection)
        {
            if (item == null)
            {
                this.ClearSelection();
                return;
            }

            bool isShiftPressed = (modifierKeys & Keys.Shift) == Keys.Shift;
            bool isControlPressed = (modifierKeys & Keys.Control) == Keys.Control;
            bool clearSelection = this.owner.MultiSelect && ((isShiftPressed && !isControlPressed) || !isControlPressed ||
                                                             (!isMouseSelection && !isShiftPressed && !isControlPressed));

            if (clearSelection)
            {
                this.ClearSelection();
            }

            if (this.owner.MultiSelect)
            {
                if (isShiftPressed)
                {
                    ListViewTraverser enumerator = this.Scroller.Traverser.GetEnumerator() as ListViewTraverser;
                    if (enumerator == null)
                    {
                        return;
                    }

                    ListViewDataItemGroup group = item as ListViewDataItemGroup;

                    if (group != null)
                    {
                        if (group.Items.Count == 0)
                        {
                            this.owner.CurrentItem = item;
                            return;
                        }
                        else
                        {
                            item = group.Items[group.Items.Count - 1];
                        }
                    }

                    enumerator.Position = null;
                    bool shouldSelectItem = false;

                    while (enumerator.MoveNext())
                    {
                        if (!shouldSelectItem && (enumerator.Current == item || enumerator.Current == anchor))
                        {
                            if (!(enumerator.Current is ListViewDataItemGroup))
                            {
                                enumerator.Current.Selected = true;
                            }

                            shouldSelectItem = item != anchor;
                            continue;
                        }

                        if (shouldSelectItem && !(enumerator.Current is ListViewDataItemGroup))
                            enumerator.Current.Selected = true;

                        if ((enumerator.Current == item || enumerator.Current == anchor))
                            break;
                    }

                    this.owner.SetSelectedItem(item);
                }
                else if (isControlPressed)
                {
                    if (isMouseSelection)
                    {
                        item.Selected = !item.Selected;
                        if (item.Selected)
                        {
                            this.owner.SetSelectedItem(item);
                        }
                        else
                        {
                            this.owner.SetSelectedItem(this.owner.SelectedItems.Count > 0 ?
                                this.owner.SelectedItems[this.owner.SelectedItems.Count - 1] : null);
                        }
                        this.anchor = item;
                    }
                }
                else
                {
                    item.Selected = true;
                    this.owner.SetSelectedItem(item);
                    this.anchor = item;
                }
            }
            else
            {
                if (!isControlPressed && !(item is ListViewDataItemGroup))
                {
                    this.ClearSelection();
                    item.Selected = true;
                    this.owner.SetSelectedItem(item);
                    this.anchor = item;
                }
            }

            this.owner.CurrentItem = item;
        }

        #endregion

        #region Event Handlers

        void groupSelectionTimer_Tick(object sender, EventArgs e)
        {
            this.groupSelectionTimer.Stop();

            if (this.lastClickedItem != null && this.lastClickedItem is ListViewDataItemGroup)
            {
                this.ProcessSelection(this.lastClickedItem, this.lastModifierKeys, true);
            }

            this.lastModifierKeys = Keys.None;
        }

        void beginEditTimer_Tick(object sender, EventArgs e)
        {
            this.beginEditTimer.Stop();

            if (this.lastClickedItem != null && this.lastClickedItem.Selected)
            {
                this.owner.BeginEdit();
            }

            this.lastClickedItem = null;
        }

        protected virtual void Scroller_ScrollerUpdated(object sender, EventArgs e)
        {
            this.owner.EndEdit();
        }

        #endregion

        #region Helpers

        private int GetItemIndex(ListViewDataItem item)
        {
            ListViewTraverser enumerator = (ListViewTraverser)this.Scroller.Traverser.GetEnumerator();
            enumerator.Position = null;
            int index = 0;

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == item)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        protected void SetScrollValue(RadScrollBarElement scrollbar, int newValue)
        {
            if (newValue > scrollbar.Maximum - scrollbar.LargeChange + 1)
            {
                newValue = scrollbar.Maximum - scrollbar.LargeChange + 1;
            }
            if (newValue < scrollbar.Minimum)
            {
                newValue = scrollbar.Minimum;
            }
            scrollbar.Value = newValue;
        }

        #endregion

    }
}