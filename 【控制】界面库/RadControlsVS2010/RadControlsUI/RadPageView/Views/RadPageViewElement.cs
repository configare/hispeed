using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Data;
using Telerik.WinControls.Themes;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Encapsulates the UI representation of a RadPageView instance. Different views will be described by different instances of this class.
    /// </summary>
    public abstract class RadPageViewElement : RadPageViewElementBase
    {
        #region Fields

        private RadContextMenu itemListMenu;
        private RadPageViewContentAreaElement contentArea;
        private RadPageViewLabelElement header;
        private RadPageViewLabelElement footer;
        private RadPageViewReadonlyCollection<RadPageViewItem> items;
        private MouseButtons actionMouseButton;
        private RadDragDropService itemDragService;

        //references
        private RadPageView owner;
        internal RadPageViewItem selectedItem;
        private bool updateSelectedItemContent;

        #endregion

        #region Events

        public event EventHandler ItemClicked;
        public event EventHandler<RadPageViewItemCreatingEventArgs> ItemCreating;
        public event EventHandler<RadPageViewItemSelectingEventArgs> ItemSelecting;
        public event EventHandler<RadPageViewItemSelectedEventArgs> ItemSelected;
        public event EventHandler<RadPageViewItemsChangedEventArgs> ItemsChanged;

        #endregion

        #region Contructor/Initializers

        public RadPageViewElement()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.itemDragService = new RadPageViewDragDropService(this);
            this.items = new RadPageViewReadonlyCollection<RadPageViewItem>();
            this.actionMouseButton = MouseButtons.Left;

            this.itemListMenu = new RadContextMenu();
            this.itemListMenu.DropDownClosed += OnItemListMenu_DropDownClosed;

            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.AllowDrop = true;
            this.UpdateSelectedItemContent = true;
        }

        protected override void CreateChildElements()
        {
            this.contentArea = new RadPageViewContentAreaElement();
            this.contentArea.Owner = this;
            this.Children.Add(this.contentArea);

            this.header = new RadPageViewLabelElement();
            this.header.Class = "PageViewHeader";
            this.header.Visibility = ElementVisibility.Collapsed;
            this.Children.Add(this.header);

            this.footer = new RadPageViewLabelElement();
            this.footer.Class = "PageViewFooter";
            this.footer.Visibility = ElementVisibility.Collapsed;
            this.Children.Add(this.footer);
        }

        #endregion

        #region Overrides

        protected override void DisposeManagedResources()
        {
            this.itemListMenu.DropDownClosed -= OnItemListMenu_DropDownClosed;
            this.itemListMenu.Dispose();
            this.items.Clear();
            this.selectedItem = null;
            this.owner = null;

            base.DisposeManagedResources();
        }

        #endregion

        #region Rad Properties

        public static RadProperty ShowItemCloseButtonProperty = RadProperty.Register(
            "ShowItemCloseButton",
            typeof(bool),
            typeof(RadPageViewElement),
            new RadElementPropertyMetadata(false,
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsDisplay |
                ElementPropertyOptions.CanInheritValue));

        public static RadProperty ItemDragModeProperty = RadProperty.Register(
           "ItemDragMode",
           typeof(PageViewItemDragMode),
           typeof(RadPageViewElement),
           new RadElementPropertyMetadata(PageViewItemDragMode.None, ElementPropertyOptions.None));

        public static RadProperty ItemDragHintProperty = RadProperty.Register(
           "ItemDragHint",
           typeof(RadImageShape),
           typeof(RadPageViewElement),
           new RadElementPropertyMetadata(null, ElementPropertyOptions.None));

        public static RadProperty ItemBorderAndFillOrientationProperty = RadProperty.Register(
           "ItemBorderAndFillOrientation",
           typeof(PageViewContentOrientation),
           typeof(RadPageViewElement),
           new RadElementPropertyMetadata(PageViewContentOrientation.Auto,
               ElementPropertyOptions.AffectsDisplay |
               ElementPropertyOptions.CanInheritValue));

        public static RadProperty EnsureSelectedItemVisibleProperty = RadProperty.Register(
           "EnsureSelectedItemVisible",
           typeof(bool),
           typeof(RadPageViewElement),
           new RadElementPropertyMetadata(true,
               ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ItemContentOrientationProperty = RadProperty.Register(
           "ItemContentOrientation",
           typeof(PageViewContentOrientation),
           typeof(RadPageViewElement),
           new RadElementPropertyMetadata(PageViewContentOrientation.Auto,
               ElementPropertyOptions.AffectsMeasure |
               ElementPropertyOptions.AffectsDisplay |
               ElementPropertyOptions.CanInheritValue));

        public static RadProperty ItemSizeModeProperty = RadProperty.Register(
           "ItemSizeMode",
           typeof(PageViewItemSizeMode),
           typeof(RadPageViewElement),
           new RadElementPropertyMetadata(PageViewItemSizeMode.EqualHeight,
               ElementPropertyOptions.AffectsMeasure |
               ElementPropertyOptions.AffectsDisplay |
               ElementPropertyOptions.CanInheritValue));

        public static RadProperty ItemSpacingProperty = RadProperty.Register(
           "ItemSpacing",
           typeof(int),
           typeof(RadPageViewElement),
           new RadElementPropertyMetadata(0,
               ElementPropertyOptions.AffectsMeasure |
               ElementPropertyOptions.AffectsDisplay |
               ElementPropertyOptions.CanInheritValue));

        #endregion

        #region CLR Properties

        internal abstract PageViewLayoutInfo ItemLayoutInfo
        {
            get;
        }

        /// <summary>
        /// Gets the RadElement instance that parents all the items.
        /// </summary>
        protected abstract RadElement ItemsParent
        {
            get;
        }

        /// <summary>
        /// Determines CloseButton will be displayed in each item, allowing that item to be closed.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines CloseButton will be displayed in each item, allowing that item to be closed.")]
        [RadPropertyDefaultValue("ShowItemCloseButton", typeof(RadPageViewStripElement))]
        public bool ShowItemCloseButton
        {
            get
            {
                return (bool)this.GetValue(ShowItemCloseButtonProperty);
            }
            set
            {
                this.SetValue(ShowItemCloseButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RadImageShape">RadImageShape</see> instance which describes the hint that indicates where an item will be dropped after a drag operation.
        /// </summary>
        [Browsable(false)]
        [VsbBrowsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadImageShape ItemDragHint
        {
            get
            {
                return (RadImageShape)this.GetValue(ItemDragHintProperty);
            }
            set
            {
                this.SetValue(ItemDragHintProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RadDragDropService">RadDragDropService</see> instance which handles item drag requests.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDragDropService ItemDragService
        {
            get
            {
                return this.itemDragService;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Cannot set null as item drag service.");
                }

                this.itemDragService = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode that controls item drag operation within the element.
        /// </summary>
        [DefaultValue(PageViewItemDragMode.None)]
        [Description("Gets or sets the mode that controls item drag operation within the element.")]
        public PageViewItemDragMode ItemDragMode
        {
            get
            {
                return (PageViewItemDragMode)this.GetValue(ItemDragModeProperty);
            }
            set
            {
                this.SetValue(ItemDragModeProperty, value);
            }
        }

        /// <summary>
        /// Determines whether the currently selected item will be automatically scrolled into view.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines whether the currently selected item will be automatically scrolled into view.")]
        [RadPropertyDefaultValue("EnsureSelectedItemVisible", typeof(RadPageViewElement))]
        public bool EnsureSelectedItemVisible
        {
            get
            {
                return (bool)this.GetValue(EnsureSelectedItemVisibleProperty);
            }
            set
            {
                this.SetValue(EnsureSelectedItemVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between two items within the element.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the spacing between two items within the element.")]
        [RadPropertyDefaultValue("ItemSpacing", typeof(RadPageViewElement))]
        public int ItemSpacing
        {
            get
            {
                return (int)this.GetValue(ItemSpacingProperty);
            }
            set
            {
                this.SetValue(ItemSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text orientation of the item within the owning RadPageViewElement instance.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the text orientation of the item within the owning RadPageViewElement instance.")]
        [RadPropertyDefaultValue("ItemSizeMode", typeof(RadPageViewElement))]
        public PageViewItemSizeMode ItemSizeMode
        {
            get
            {
                return (PageViewItemSizeMode)this.GetValue(ItemSizeModeProperty);
            }
            set
            {
                this.SetValue(ItemSizeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text orientation of the item within the owning RadPageViewElement instance.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the text orientation of the item within the owning RadPageViewElement instance.")]
        [RadPropertyDefaultValue("ItemContentOrientation", typeof(RadPageViewElement))]
        public PageViewContentOrientation ItemContentOrientation
        {
            get
            {
                return (PageViewContentOrientation)this.GetValue(ItemContentOrientationProperty);
            }
            set
            {
                this.SetValue(ItemContentOrientationProperty, value);
            }
        }

        /// <summary>
        /// Defines how each item's border and fill is oriented within this instance.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Defines how each item's border and fill is oriented within this instance.")]
        [RadPropertyDefaultValue("ItemBorderAndFillOrientation", typeof(RadPageViewElement))]
        public PageViewContentOrientation ItemBorderAndFillOrientation
        {
            get
            {
                return (PageViewContentOrientation)this.GetValue(ItemBorderAndFillOrientationProperty);
            }
            set
            {
                this.SetValue(ItemBorderAndFillOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets the RadPageView instance that owns this element. May be null if element is hosted on another RadControl instance.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageView Owner
        {
            get
            {
                return this.owner;
            }
            internal set
            {
                this.owner = value;
            }
        }

        /// <summary>
        /// Gets the element which represents the content area of the tab view.
        /// </summary>
        [Browsable(false)]
        public RadPageViewElementBase ContentArea
        {
            get
            {
                return this.contentArea;
            }
        }

        /// <summary>
        /// Gets the header element of the view.
        /// </summary>
        [Description("Gets the header element of the view.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadPageViewLabelElement Header
        {
            get
            {
                return this.header;
            }
        }

        /// <summary>
        /// Gets the footer element of the view.
        /// </summary>
        [Description("Gets the footer element of the view.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadPageViewLabelElement Footer
        {
            get
            {
                return this.footer;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item in the view.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageViewItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                this.VerifyUnboundMode();
                this.SetSelectedItem(value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse button that will be used to select items. Equals to MouseButtons.Left by default.
        /// </summary>
        [DefaultValue(MouseButtons.Left)]
        [Description("Gets or sets the mouse button that will be used to select items. Equals to MouseButtons.Left by default.")]
        public MouseButtons ActionMouseButton
        {
            get
            {
                return this.actionMouseButton;
            }
            set
            {
                this.actionMouseButton = value;
            }
        }

        /// <summary>
        /// Gets all the items currently present within this element.
        /// </summary>
        [Browsable(false)]
        public IReadOnlyCollection<RadPageViewItem> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Determines whether selecting an item will update the element's ContentArea.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether selecting an item will update the element's ContentArea.")]
        public bool UpdateSelectedItemContent
        {
            get
            {
                return this.updateSelectedItemContent;
            }
            set
            {
                this.updateSelectedItemContent = value;
            }
        }

        #endregion

        #region Overrides

        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (this.owner == null)
                return;

            RadPageViewPage selectedPage = this.owner.SelectedPage;

            if (selectedPage != null)
            {
                selectedPage.Visible = true;
            }
        }

        #endregion

        #region Virtual Methods

        protected abstract RadPageViewItem CreateItem();

        /// <summary>
        /// Gets an instance of the <see cref="RadPageViewContentAreaElement"/> class that
        /// represents the content area associated with the given item. By default, this method
        /// returns the main content area of the <see cref="RadPageView"/> control.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual RadPageViewContentAreaElement GetContentAreaForItem(RadPageViewItem item)
        {
            return this.contentArea;
        }

        /// <summary>
        /// Gets the area, where the currently active page may be displayed.
        /// </summary>
        public virtual Rectangle GetContentAreaRectangle()
        {
            return this.GetClientRectangleFromContentElement(this.GetContentAreaForItem(this.selectedItem));
        }

        public virtual Rectangle GetClientRectangleFromContentElement(RadPageViewContentAreaElement contentArea)
        {
            Rectangle contentBounds = contentArea.ControlBoundingRectangle;
            Padding padding = contentArea.Padding;
            Padding border = contentArea.GetBorderThickness(false);

            contentBounds.X += padding.Left + border.Left;
            contentBounds.Y += padding.Top + border.Top;
            contentBounds.Width -= padding.Horizontal + border.Horizontal;
            contentBounds.Height -= padding.Vertical + border.Vertical;

            return contentBounds;
        }

        /// <summary>
        /// Gets the rectangle where items reside.
        /// </summary>
        /// <returns></returns>
        public virtual RectangleF GetItemsRect()
        {
            return RectangleF.Empty;
        }

        #endregion

        #region ItemList Menu

        /// <summary>
        /// Displays the item list menu, using the provided element as menu's owner.
        /// </summary>
        /// <param name="menuOwner"></param>
        public void DisplayItemListMenu(RadPageViewElementBase menuOwner)
        {
            this.DisplayItemListMenu(menuOwner, HorizontalPopupAlignment.LeftToLeft, VerticalPopupAlignment.TopToBottom);
        }

        /// <summary>
        /// Displays the item list menu, using the provided element as menu's owner and the specified horizontal and vertical alignment.
        /// </summary>
        /// <param name="menuOwner"></param>
        /// <param name="hAlign"></param>
        /// <param name="vAlign"></param>
        public void DisplayItemListMenu(RadPageViewElementBase menuOwner, HorizontalPopupAlignment hAlign, VerticalPopupAlignment vAlign)
        {
            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }
            if (this.itemListMenu.DropDown.IsDisplayed)
            {
                this.itemListMenu.DropDown.ClosePopup(RadPopupCloseReason.CloseCalled);
            }

            List<RadMenuItemBase> menuItems = new List<RadMenuItemBase>();

            foreach (RadPageViewItem item in this.items)
            {
                if (item.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }

                RadMenuItem menuItem = new RadMenuItem(item.Text);
                menuItem.Image = item.Image;
                menuItem.Click += this.OnItemListMenuItem_Click;
                menuItem.Tag = item;
                if (item == this.selectedItem)
                {
                    menuItem.IsChecked = true;
                }

                menuItems.Add(menuItem);
            }

            Rectangle alignRect = this.ElementTree.Control.RectangleToScreen(menuOwner.ControlBoundingRectangle);
            RadPageViewMenuDisplayingEventArgs e = new RadPageViewMenuDisplayingEventArgs(menuItems, alignRect, hAlign, vAlign);
            this.DisplayItemListMenu(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void DisplayItemListMenu(RadPageViewMenuDisplayingEventArgs e)
        {
            if (this.owner != null)
            {
                this.owner.OnItemListMenuDisplaying(e);
            }

            if (e.Cancel)
            {
                return;
            }

            foreach (RadMenuItemBase menuItem in e.Items)
            {
                this.itemListMenu.Items.Add(menuItem);
            }

            this.itemListMenu.DropDown.HorizontalPopupAlignment = e.HAlign;
            this.itemListMenu.DropDown.VerticalPopupAlignment = e.VAlign;
            
            RadControl parent = (this.ElementTree.Control as RadControl);
            if (parent != null)
            {
                this.itemListMenu.ThemeName = parent.ThemeName;
            }

            if (!this.itemListMenu.DropDown.IsLoaded)
            {
                this.itemListMenu.DropDown.LoadElementTree();
            }
            else
            {
                this.itemListMenu.DropDown.RootElement.UpdateLayout();
            }
            this.itemListMenu.DropDown.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            this.itemListMenu.DropDown.ShowPopup(e.AlignRect);

            if (this.owner != null)
            {
                this.owner.OnItemListMenuDisplayed(EventArgs.Empty);
            }
        }

        private void OnItemListMenuItem_Click(object sender, EventArgs e)
        {
            RadMenuItemBase menuItem = sender as RadMenuItemBase;

            RadPageViewItem pageItem = menuItem.Tag as RadPageViewItem;
            if (this.selectedItem == pageItem)
            {
                this.EnsureItemVisible(pageItem);
            }
            else
            {
                this.SelectItem(pageItem);
            }

            this.ClearMenuItem(menuItem);
        }

        private void OnItemListMenu_DropDownClosed(object sender, EventArgs e)
        {
            for (int i = this.itemListMenu.Items.Count - 1; i >= 0; i--)
            {
                RadMenuItemBase menuItem = this.itemListMenu.Items[i] as RadMenuItemBase;
                if (this.itemListMenu.DropDown.ClickedItem != menuItem)
                {
                    this.ClearMenuItem(menuItem);
                }
            }
        }

        private void ClearMenuItem(RadMenuItemBase menuItem)
        {
            menuItem.Click -= OnItemListMenuItem_Click;
            menuItem.Dispose();
        }

        #endregion

        #region Items Collection

        public void AddItem(RadPageViewItem item)
        {
            this.VerifyUnboundMode();
            this.AddItemCore(item);
        }

        public void InsertItem(int index, RadPageViewItem item)
        {
            this.VerifyUnboundMode();
            this.InsertItemCore(index, item);
        }

        protected virtual void AddItemCore(RadPageViewItem item)
        {
            this.items.Add(item);
            item.Owner = this;
            this.SyncronizeItem(item);

            RadPageViewItemsChangedEventArgs itemsChangedEventArgs = new RadPageViewItemsChangedEventArgs(item, ItemsChangeOperation.Inserted);
            this.OnItemsChanged(this, itemsChangedEventArgs);

            RadElement parent = this.ItemsParent;
            if (parent != null)
            {
                parent.Children.Add(item);
            }

            if (this.selectedItem == null)
            {
                this.SelectItem(item);
            }
        }

        protected virtual void InsertItemCore(int index, RadPageViewItem item)
        {
            this.items.Insert(index, item);
            item.Owner = this;
            this.SyncronizeItem(item);

            RadPageViewItemsChangedEventArgs itemsChangedEventArgs = new RadPageViewItemsChangedEventArgs(item, ItemsChangeOperation.Inserted);
            this.OnItemsChanged(this, itemsChangedEventArgs);

            RadElement parent = this.ItemsParent;
            if (parent != null)
            {
                parent.Children.Insert(index, item);
            }

            if (this.selectedItem == null)
            {
                this.SelectItem(item);
            }
        }

        public void RemoveItem(RadPageViewItem item)
        {
            this.VerifyUnboundMode();
            this.RemoveItemCore(item);
        }

        protected virtual void RemoveItemCore(RadPageViewItem item)
        {
            int index = this.items.IndexOf(item);
            this.items.RemoveAt(index);
            item.Owner = null;

            RadPageViewItemsChangedEventArgs itemsChangedEventArgs = new RadPageViewItemsChangedEventArgs(item, ItemsChangeOperation.Removed);
            this.OnItemsChanged(this, itemsChangedEventArgs);

            RadElement parent = this.ItemsParent;
            if (parent != null)
            {
                parent.Children.Remove(item);
            }

            if (item != this.selectedItem)
            {
                return;
            }

            item.IsSelected = false;

            if (index >= this.items.Count)
            {
                index--;
            }

            RadPageViewItem newSelected = null;
            if (index >= 0 && index < this.items.Count)
            {
                newSelected = this.items[index];
            }

            this.SelectItem(newSelected);
        }

        public void SwapItems(RadPageViewItem item1, RadPageViewItem item2)
        {
            this.VerifyUnboundMode();

            int index1 = this.items.IndexOf(item1);
            int index2 = this.items.IndexOf(item2);

            if (index1 == -1 || index2 == -1)
            {
                throw new IndexOutOfRangeException();
            }

            this.SwapItemsCore(index1, index2);
        }

        public void SwapItems(int index1, int index2)
        {
            this.VerifyUnboundMode();

            if ((index1 < 0 || index1 >= this.items.Count) ||
                (index2 < 0 || index2 >= this.items.Count))
            {
                throw new IndexOutOfRangeException();
            }

            this.SwapItemsCore(index1, index2);
        }

        protected virtual void SwapItemsCore(int index1, int index2)
        {
            RadPageViewItem temp = this.items[index1];
            this.items[index1] = this.items[index2];
            this.items[index2] = temp;
        }

        protected virtual void SetItemIndex(int currentIndex, int newIndex)
        {
            int indexOffset = this.ItemsParent.Children.IndexOf(this.items[0]);

            RadPageViewItem item = this.items[currentIndex];
            this.items.RemoveAt(currentIndex);
            this.items.Insert(newIndex, item);

            this.ItemsParent.Children.Move(currentIndex + indexOffset, newIndex + indexOffset);
            this.ItemsParent.InvalidateMeasure();
        }

        public RadPageViewItem FindItem(RadElement content)
        {
            foreach (RadPageViewItem item in this.items)
            {
                if (item.Content == content)
                {
                    return item;
                }
            }

            return null;
        }

        public RadPageViewItem GetItemAt(int index)
        {
            Debug.Assert(index >= 0 && index < this.items.Count, "Invalid index.");
            return this.items[index];
        }

        /// <summary>
        /// Gets the item that contains the porvided point (in control's client coordinates).
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public RadPageViewItem ItemFromPoint(Point client)
        {
            foreach (RadPageViewItem item in this.items)
            {
                if (item.Visibility == ElementVisibility.Visible &&
                    item.ControlBoundingRectangle.Contains(client))
                {
                    return item;
                }
            }

            return null;
        }

        public bool EnsureItemVisible(RadPageViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Item");
            }

            if (item.Owner != this)
            {
                throw new InvalidOperationException("Could not EnsureVisible item that is not hosted on this instance.");
            }

            return this.EnsureItemVisibleCore(item);
        }

        protected internal virtual void SynchronizeItemsIndices()
        {
        }

        protected virtual bool EnsureItemVisibleCore(RadPageViewItem item)
        {
            return false;
        }

        protected internal virtual void SetSelectedItem(RadPageViewItem item)
        {
            //no need to change selection
            if (this.selectedItem == item)
            {
                return;
            }

            if (UpdateSelectedItemContent)
            {
                this.SetSelectedContent(item);
            }

            RadPageViewItemSelectingEventArgs selectingEventArgs = new RadPageViewItemSelectingEventArgs(this.selectedItem, item);
            this.OnItemSelecting(this, selectingEventArgs);
            if (selectingEventArgs.Cancel)
            {
                return;
            }

            if (this.selectedItem != null)
            {
                this.selectedItem.IsSelected = false;
            }

            RadPageViewItem previousItem = this.selectedItem;
            this.selectedItem = item;

            if (this.selectedItem != null)
            {
                this.selectedItem.IsSelected = true;
                this.header.Text = this.selectedItem.Title;
                this.footer.Text = this.selectedItem.Description;

                if (this.EnsureSelectedItemVisible)
                {
                    this.EnsureItemVisibleCore(item);
                }
            }
            else
            {
                this.header.ResetValue(RadItem.TextProperty);
                this.footer.ResetValue(RadItem.TextProperty);
            }

            RadPageViewItemSelectedEventArgs selectedEventArgs = new RadPageViewItemSelectedEventArgs(previousItem, this.selectedItem);
            this.OnItemSelected(this, selectedEventArgs);
        }

        protected internal virtual bool OnItemContentChanging(RadPageViewItem item, RadElement newContent)
        {
            return true;
        }

        protected internal virtual void OnItemContentChanged(RadPageViewItem item)
        {
        }

        protected internal virtual void CloseItem(RadPageViewItem item)
        {
            if (this.owner != null)
            {
                Debug.Assert(item.Page != null && item.Page.Owner == this.owner, "Invalid CloseItem request.");
                this.owner.Pages.Remove(item.Page);
            }
            else
            {
                this.RemoveItem(item);
            }
        }

        protected internal virtual void OnItemPropertyChanged(RadPageViewItem item, RadPropertyChangedEventArgs e)
        {
            if (item != this.selectedItem)
            {
                return;
            }

            if (e.Property == RadItem.TextProperty ||
                e.Property == RadPageViewItem.TitleProperty ||
                e.Property == RadPageViewItem.DescriptionProperty)
            {
                this.header.Text = item.Title;
                this.footer.Text = item.Description;
            }
        }

        protected virtual void SetSelectedContent(RadPageViewItem item)
        {
            if (this.owner != null)
            {
                if (item != null)
                {
                    Debug.Assert(item.Page != null, "Selecting item without a page.");
                }

                if (this.selectedItem != null && this.selectedItem.Page != null)
                {
                    this.selectedItem.Page.Visible = false;
                }

                if (item != null && item.Page != null)
                {
                    this.UpdatePageBounds(item.Page);
                    item.Page.Visible = true;
                }
            }
            else
            {
                if (item != null)
                {
                    Debug.Assert(item.Page == null, "Selecting item with valid page without being bound to RadPageView instance.");
                }

                //clear current content and add the item's one
                this.contentArea.Children.Clear();

                if (item != null && item.Content != null)
                {
                    item.Content.StretchHorizontally = true;
                    item.Content.StretchVertically = true;
                    this.contentArea.Children.Add(item.Content);
                }
            }
        }

        protected virtual void SyncronizeItem(RadPageViewItem item)
        {
            this.UpdateItemOrientation(new RadItem[] { item });
            this.InvalidateMeasure();
        }

        private void VerifyUnboundMode()
        {
            if (this.owner != null)
            {
                throw new InvalidOperationException("Cannot freely modify items of a bound RadPageViewElement. Use Pages collection of the owning RadPageView instead.");
            }
        }

        internal virtual void SelectItem(RadPageViewItem item)
        {
            if (this.owner != null)
            {
                if (item != null)
                {
                    Debug.Assert(item.Page != null, "Invalid Page in bound mode.");
                    this.owner.SelectedPage = item.Page;
                }
                else
                {
                    this.owner.SelectedPage = null;
                }
            }
            else
            {
                this.SetSelectedItem(item);
            }
        }

        #endregion

        #region Input Events

        public bool SelectNextItem()
        {
            if (this.selectedItem == null)
            {
                return false;
            }

            return this.SelectNextItemCore(this.selectedItem, true, true);
        }

        public bool SelectPreviousItem()
        {
            if (this.selectedItem == null)
            {
                return false;
            }

            return this.SelectNextItemCore(this.selectedItem, false, true);
        }

        protected virtual bool SelectNextItemCore(RadPageViewItem current, bool forward, bool wrap)
        {
            int itemCount = this.items.Count;
            if (itemCount <= 1)
            {
                return false;
            }

            int index = this.items.IndexOf(current);

            if (forward)
            {
                index++;
                if (index >= itemCount && wrap)
                {
                    index = 0;
                }
            }
            else
            {
                index--;
                if (index < 0 && wrap)
                {
                    index = itemCount - 1;
                }
            }

            if (index >= 0 && index < itemCount)
            {
                this.SelectItem(this.items[index]);
                return true;
            }

            return false;
        }

        protected internal virtual void OnItemMouseDown(RadPageViewItem sender, MouseEventArgs e)
        {
            if (e.Button == this.actionMouseButton && e.Clicks == 1)
            {
                if (!sender.IsSelected)
                {
                    this.SelectItem(sender);
                }
                else if (this.EnsureSelectedItemVisible)
                {
                    this.EnsureItemVisible(sender);
                }
            }
        }

        protected internal virtual void OnItemMouseUp(RadPageViewItem sender, MouseEventArgs e)
        {
        }

        protected internal virtual void OnItemDrag(RadPageViewItem sender, MouseEventArgs e)
        {
            if (this.IsDesignMode)
            {
                return;
            }

            if (this.ItemDragMode != PageViewItemDragMode.None)
            {
                this.StartItemDrag(sender);
            }
        }

        protected internal virtual void OnItemClick(RadPageViewItem sender, EventArgs e)
        {
            if (ItemClicked != null)
            {
                ItemClicked(sender, e);
            }
        }

        protected virtual RadPageViewItem OnItemCreating(RadPageViewItemCreatingEventArgs args)
        {
            if (this.ItemCreating != null)
            {
                this.ItemCreating(this, args);
            }

            return args.Item;
        }

        public virtual void OnItemSelected(object sender, RadPageViewItemSelectedEventArgs args)
        {
            if (this.ItemSelected != null)
            {
                this.ItemSelected(sender, args);
            }
        }

        public virtual void OnItemSelecting(object sender, RadPageViewItemSelectingEventArgs args)
        {
            if (this.ItemSelecting != null)
            {
                this.ItemSelecting(sender, args);
            }
        }

        protected virtual void OnItemsChanged(object sender, RadPageViewItemsChangedEventArgs args)
        {
            if (this.ItemsChanged != null)
            {
                this.ItemsChanged(sender, args);
            }
        }

        protected internal virtual void ProcessKeyDown(KeyEventArgs e)
        {
            if (this.IsNextKey(e.KeyCode))
            {
                this.SelectNextItem();
            }
            else if (this.IsPreviousKey(e.KeyCode))
            {
                this.SelectPreviousItem();
            }
        }

        protected internal virtual bool IsNextKey(Keys key)
        {
            return key == Keys.Right;
        }

        protected internal virtual bool IsPreviousKey(Keys key)
        {
            return key == Keys.Left;
        }

        #endregion

        #region Pages Callback

        protected internal virtual void OnPageAdded(RadPageViewEventArgs e)
        {
            RadPageViewItem item = this.OnItemCreating(new RadPageViewItemCreatingEventArgs(e.Page));

            if(item == null)
            {
                item = this.CreateItem();
            }

            item.Attach(e.Page);

            this.AddItemCore(item);
        }

        protected internal virtual void OnPageRemoved(RadPageViewEventArgs e)
        {
            RadPageViewItem item = e.Page.Item;
            Debug.Assert(item != null, "Must have an item associated with the removed page.");

            this.RemoveItemCore(item);
            item.Detach();
        }

        protected internal virtual void OnPageIndexChanged(RadPageViewIndexChangedEventArgs e)
        {
            Debug.Assert(this.owner != null, "No valid owner reference.");
            this.SetItemIndex(e.OldIndex, e.NewIndex);
        }

        protected internal virtual void OnSelectedPageChanged(RadPageViewEventArgs e)
        {
            RadPageViewItem item = null;
            if (e.Page != null)
            {
                item = e.Page.Item;
            }

            this.SetSelectedItem(item);
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            Padding borderThickness = GetBorderThickness(false);
            SizeF clientSize = this.GetClientRectangle(availableSize).Size;
            SizeF desiredSize = new SizeF(Padding.Horizontal + borderThickness.Horizontal, Padding.Vertical + borderThickness.Vertical);

            if (this.header.Visibility == ElementVisibility.Visible)
            {
                this.header.Measure(clientSize);
                clientSize.Height -= (this.header.DesiredSize.Height + this.header.Margin.Vertical);
                desiredSize.Height += this.header.DesiredSize.Height + this.header.Margin.Vertical;
            }
            if (this.footer.Visibility == ElementVisibility.Visible)
            {
                this.footer.Measure(clientSize);
                clientSize.Height -= (this.footer.DesiredSize.Height + this.footer.Margin.Vertical);
                desiredSize.Height += this.footer.DesiredSize.Height + this.footer.Margin.Vertical;
            }
            SizeF itemsDesiredSize = this.MeasureItems(clientSize);

            desiredSize.Width += itemsDesiredSize.Width;
            desiredSize.Height += itemsDesiredSize.Height;

            if (StretchHorizontally)
            {
                desiredSize.Width = availableSize.Width;
            }
            if (StretchVertically)
            {
                desiredSize.Height = availableSize.Height;
            }

            desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);

            return desiredSize;
        }

        protected virtual SizeF MeasureItems(SizeF availableSize)
        {
            return availableSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF client = this.GetClientRectangle(finalSize);
            Padding margin;

            if (this.header.Visibility != ElementVisibility.Collapsed)
            {
                margin = this.header.Margin;
                RectangleF headerRect = new RectangleF(client.X + margin.Left,
                    client.Y + margin.Top,
                    client.Width - margin.Horizontal,
                    this.header.DesiredSize.Height);

                this.header.Arrange(headerRect);

                client.Y += headerRect.Height + margin.Vertical;
                client.Height = Math.Max(0, client.Height - headerRect.Height - margin.Vertical);
            }

            if (this.footer.Visibility != ElementVisibility.Collapsed)
            {
                margin = this.footer.Margin;
                RectangleF footerRect = new RectangleF(client.X + margin.Left,
                    client.Bottom - this.footer.DesiredSize.Height - margin.Bottom,
                    client.Width - margin.Horizontal,
                    this.footer.DesiredSize.Height);

                this.footer.Arrange(footerRect);

                client.Height = Math.Max(0, client.Height - footerRect.Height - margin.Vertical);
            }

            this.PerformArrange(client);

            return finalSize;
        }

        protected virtual RectangleF PerformArrange(RectangleF clientRect)
        {
            RectangleF contentRect = this.ArrangeItems(clientRect);
            this.ArrangeContent(contentRect);
            return contentRect;
        }

        protected virtual RectangleF ArrangeContent(RectangleF clientRect)
        {
            if (this.contentArea.Visibility != ElementVisibility.Collapsed)
            {
                clientRect = LayoutUtils.DeflateRect(clientRect, this.contentArea.Margin);
                this.contentArea.Arrange(clientRect);
                if (this.selectedItem != null && this.owner != null)
                {
                    this.selectedItem.Page.Bounds = this.GetContentAreaRectangle();
                }
            }

            return clientRect;
        }

        /// <summary>
        /// Arranges the items and returns the available rectangle where the content area should be positioned.
        /// </summary>
        /// <param name="itemsRect"></param>
        /// <returns></returns>
        protected virtual RectangleF ArrangeItems(RectangleF itemsRect)
        {
            return RectangleF.Empty;
        }

        protected internal virtual void OnContentBoundsChanged()
        {
            if (this.owner == null || this.BitState[ArrangeInProgressStateKey])
            {
                return;
            }

            RadPageViewPage page = this.owner.SelectedPage;
            if (page != null)
            {
                this.UpdatePageBounds(page);
            }
        }

        protected virtual void UpdatePageBounds(RadPageViewPage page)
        {
            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            Rectangle bounds = this.GetContentAreaRectangle();
            if (page.Bounds != bounds)
            {
                page.Bounds = bounds;
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ItemContentOrientationProperty)
            {
                this.UpdateItemOrientation(this.items);
            }
            else if (e.Property == EnsureSelectedItemVisibleProperty)
            {
                if ((bool)e.NewValue && this.selectedItem != null)
                {
                    this.EnsureItemVisible(this.selectedItem);
                }
            }
        }

        /// <summary>
        /// Gets the default (automatic) item orientation, which may depend on some settings in inheritors such as RadPageViewStripElement.
        /// </summary>
        /// <param name="content">True to indicate that content orientation is to be retrieved, false to get orientation for border and fill.</param>
        /// <returns></returns>
        protected internal virtual PageViewContentOrientation GetAutomaticItemOrientation(bool content)
        {
            return PageViewContentOrientation.Horizontal;
        }

        protected virtual void UpdateItemOrientation(IEnumerable items)
        {
            PageViewContentOrientation contentOrientation = this.ItemContentOrientation;
            if (contentOrientation == PageViewContentOrientation.Auto)
            {
                contentOrientation = this.GetAutomaticItemOrientation(true);
            }

            PageViewContentOrientation borderOrientation = this.ItemBorderAndFillOrientation;
            if (borderOrientation == PageViewContentOrientation.Auto)
            {
                borderOrientation = this.GetAutomaticItemOrientation(false);
            }

            foreach (RadPageViewItem item in items)
            {
                item.SetContentOrientation(contentOrientation, false);
                item.SetBorderAndFillOrientation(borderOrientation, false);
            }
        }

        #endregion

        #region Item Drag

        protected internal virtual void StartItemDrag(RadPageViewItem item)
        {
            this.itemDragService.Start(item);
        }

        protected internal virtual void EndItemDrag(RadPageViewItem item)
        {
        }

        protected override bool ProcessDragOver(Point mousePosition, ISupportDrag dragObject)
        {
            RadPageViewItem dragItem = dragObject as RadPageViewItem;
            if (dragItem == null || dragItem.Owner != this)
            {
                return false;
            }

            RadPageViewItem hitItem = this.ItemFromPoint(mousePosition);
            if (!this.CanDropOverItem(dragItem, hitItem))
            {
                return false;
            }

            this.EnsureItemVisible(hitItem);
            this.ItemsParent.UpdateLayout();

            return hitItem.ControlBoundingRectangle.Contains(mousePosition);
        }

        protected virtual bool CanDropOverItem(RadPageViewItem dragItem, RadPageViewItem hitItem)
        {
            if (hitItem == null)
            {
                return false;
            }

            return hitItem != dragItem;
        }

        protected override void ProcessDragDrop(Point dropLocation, ISupportDrag dragObject)
        {
            RadPageViewItem dragItem = dragObject as RadPageViewItem;
            Debug.Assert(dragItem != null, "Invalid drop notification");
            if (dragItem == null)
            {
                return;
            }

            RadPageViewItem hitItem = this.ItemFromPoint(dropLocation);
            Debug.Assert(hitItem != null, "Invalid drop notification");
            if (hitItem != null)
            {
                this.PerformItemDrop(dragItem, hitItem);
            }
        }

        protected internal void PerformItemDrop(RadPageViewItem dragItem, RadPageViewItem hitItem)
        {
            int newIndex;
            if (this.owner != null)
            {
                newIndex = this.owner.Pages.IndexOf(hitItem.Page);
                this.owner.Pages.ChangeIndex(dragItem.Page, newIndex);
            }
            else
            {
                newIndex = this.items.IndexOf(hitItem);
                this.SetItemIndex(this.items.IndexOf(dragItem), newIndex);
            }
        }

        #endregion

        #region NC Handling

        protected internal virtual Padding GetNCMetrics()
        {
            return Padding.Empty;
        }

        protected internal virtual void OnNCPaint(Graphics g)
        {
        }

        protected internal virtual bool EnableNCPainting
        {
            get
            {
                return false;
            }
        }

        protected internal virtual bool EnableNCModification
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
