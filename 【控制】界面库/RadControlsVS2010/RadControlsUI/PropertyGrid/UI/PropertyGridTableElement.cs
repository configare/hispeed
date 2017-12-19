using System;
using Telerik.WinControls.Data;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    public class PropertyGridTableElement : VirtualizedScrollPanel<PropertyGridItemBase, PropertyGridItemElementBase>, IDataItemSource
    {
        #region Nested
        
        public enum UpdateActions
        {
            Reset,
            Resume,
            ExpandedChanged,
            StateChanged,
            SortChanged,
            ValueChanged
        }

        #endregion

        #region RadProperties

        public static RadProperty ItemHeightProperty = RadProperty.Register(
            "ItemHeight", typeof(int), typeof(PropertyGridTableElement), new RadElementPropertyMetadata(
                24, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty ItemIndentProperty = RadProperty.Register(
            "ItemIndent", typeof(int), typeof(PropertyGridTableElement), new RadElementPropertyMetadata(
                20, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty ExpandImageProperty = RadProperty.Register(
            "ExpandImage", typeof(Image), typeof(PropertyGridTableElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty CollapseImageProperty = RadProperty.Register(
            "CollapseImage", typeof(Image), typeof(PropertyGridTableElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty HoveredExpandImageProperty = RadProperty.Register(
            "HoveredExpandImage", typeof(Image), typeof(PropertyGridTableElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty HoveredCollapseImageProperty = RadProperty.Register(
            "HoveredCollapseImage", typeof(Image), typeof(PropertyGridTableElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        #endregion

        #region Fields
        
        private int updateSuspendedCount = 0;

        private RadListSource<PropertyGridItem> listSource; 
        private FilterDescriptorCollection filterDescriptors;
        private UpdateActions resumeAction = UpdateActions.Resume;
        private int minimumColumnWidth;
        private PropertyGridTraverser traverser;
        private object currentObject;
        private PropertyGridItemBase selectedItem;
        private Dictionary<Type, IInputEditor> cachedEditors = new Dictionary<Type, IInputEditor>();
        private bool autoExpandGroups = true;
        private bool pendingScrollerUpdates;
        private int valueColumnWidth;
        private float ratio;
        private float currentAvailableWidth;
        private bool readOnly;
        private IInputEditor activeEditor;
        private object cachedOldValue;
        private PropertyGridRootItemsCollection items;
        private PropertyGridGroupItemCollection groupItems;
        private PropertySort propertySort;
        private RadPropertyGridBeginEditModes beginEditMode = RadPropertyGridBeginEditModes.BeginEditOnClick;
        private Timer clickTimer;
        private bool doubleClick;
        private Point mouseDownLocation;
        private RadContextMenu contextMenu;
        private bool wasInEditModeOnMouseDown;
        private bool wasSelectedOnMouseDown;
        private bool selectionWasCanceled;

        #endregion

        #region Initialization & Disposal

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ratio = 0.5f;
            this.minimumColumnWidth = 30;
            this.pendingScrollerUpdates = true;
            this.NotifyParentOnMouseInput = true;
            this.propertySort = PropertySort.NoSort;
        }

        protected override void CreateChildElements()
        {
            this.items = new PropertyGridRootItemsCollection(this);
            this.groupItems = new PropertyGridGroupItemCollection(this);
            this.listSource = new RadListSource<PropertyGridItem>(this);
            this.listSource.CollectionView.GroupFactory = new PropertyGridGroupFactory(this);
            this.filterDescriptors = new PropertyGridFilterDescriptorCollection();
            this.traverser = new PropertyGridTraverser(this);
            this.clickTimer = new Timer();

            base.CreateChildElements();

            this.ViewElement.NotifyParentOnMouseInput = true;
            this.Scroller.ScrollMode = ItemScrollerScrollModes.Smooth;
            this.Scroller.Traverser = this.traverser;
            this.ItemSpacing = -1;
        }

        protected override IVirtualizedElementProvider<PropertyGridItemBase> CreateElementProvider()
        {
            return new PropertyGridItemElementProvider(this);
        }

        protected override void WireEvents()
        {
            base.WireEvents();

            this.listSource.CollectionChanged += new NotifyCollectionChangedEventHandler(listSource_CollectionChanged);
            this.listSource.CollectionView.SortDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(SortDescriptors_CollectionChanged);
            this.filterDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(filterDescriptors_CollectionChanged);
            this.listSource.CollectionView.GroupDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupDescriptors_CollectionChanged);
            this.clickTimer.Tick += new EventHandler(clickTimer_Tick);
        }
        
        protected override void UnwireEvents()
        {
            base.UnwireEvents();

            this.listSource.CollectionChanged -= listSource_CollectionChanged;
            this.listSource.CollectionView.SortDescriptors.CollectionChanged -= SortDescriptors_CollectionChanged;
            this.filterDescriptors.CollectionChanged -= filterDescriptors_CollectionChanged;
            this.listSource.CollectionView.GroupDescriptors.CollectionChanged -= GroupDescriptors_CollectionChanged;
            this.clickTimer.Tick -= clickTimer_Tick;
        }
        
        protected override void DisposeManagedResources()
        {
            foreach (KeyValuePair<Type, IInputEditor> keyValuePairEditor in this.cachedEditors)
            {
                IInputEditor editor = keyValuePairEditor.Value;
                IDisposable disposable = null;
                BaseInputEditor baseInputEditor = editor as BaseInputEditor;
                if (baseInputEditor != null)
                {
                    disposable = baseInputEditor.EditorElement;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                disposable = editor as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            this.cachedEditors.Clear();

            UnwireEvents();

            base.DisposeManagedResources();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="PropertyGridElement"/> that is a parent to this element.
        /// </summary>
        public PropertyGridElement PropertyGridElement
        {
            get
            {
                return this.FindAncestor<PropertyGridElement>();
            }
        }

        /// <summary>
        /// Gets the active editor.
        /// </summary>
        public IValueEditor ActiveEditor
        {
            get 
            {
                return this.activeEditor; 
            }
        }

        /// <summary>
        /// Gets or sets the mode in which the properties will be displayed in the <see cref="PropertyGridTableElement"/>.
        /// </summary>
        [Description("Gets or sets the mode in which the properties will be displayed in the PropertyGridTableElement."),
        Category(RadDesignCategory.AppearanceCategory),
        Browsable(true), DefaultValue(PropertySort.NoSort)]
        public PropertySort PropertySort
        {
            get
            {
                return this.propertySort;
            }
            set
            {
                this.propertySort = value;
                this.PerformPropertySort(value);
                this.OnNotifyPropertyChanged("PropertySort");
            }
        }

        /// <summary>
        /// Gets or sets the minimum width columns can have.
        /// </summary>
        [Description("Gets or sets the minimum width columns can have."),
        Category(RadDesignCategory.AppearanceCategory),
        Browsable(true), DefaultValue(30)]
        public int MinimumColumnWidth
        {
            get
            {
                return this.minimumColumnWidth;
            }
            set
            {
                this.minimumColumnWidth = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are currently open editors.
        /// </summary>
        [Description("Gets a value indicating whether there are currently open editors."),
        Browsable(false),
        DefaultValue(false)]
        public bool IsEditing
        {
            get
            {
                return this.activeEditor != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to edit the values of the properties.
        /// </summary>
        [Description("Gets or sets a value indicating whether the user is allowed to edit the values of the properties."),
        Category(RadDesignCategory.BehaviorCategory),
        Browsable(true), DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                return this.readOnly;
            }
            set
            {
                this.readOnly = value;

                this.OnNotifyPropertyChanged("ReadOnly");
            }
        }

        /// <summary>
        /// Gets or sets the width of the "column" that holds the values.
        /// </summary>
        [Description("Gets or sets the width of the \"column\" that holds the values."),
        Category(RadDesignCategory.LayoutCategory),
        Browsable(true), DefaultValue(-1)]
        public int ValueColumnWidth
        {
            get
            {
                return this.valueColumnWidth;
            }
            set
            {
                this.SetValueColumnWidth(value, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the groups will be expanded or collapse upon creation.
        /// </summary>
        [Description("Gets or sets a value indicating whether the groups will be expanded or collapse upon creation."),
        Category(RadDesignCategory.BehaviorCategory),
        Browsable(true), DefaultValue(true)]
        public bool AutoExpandGroups
        {
            get
            {
                return this.autoExpandGroups;
            }
            set
            {
                this.autoExpandGroups = value;
                foreach (PropertyGridGroup group in this.CollectionView.Groups)
                {
                    group.GroupItem.Expanded = value;
                }

                this.OnNotifyPropertyChanged("AutoExpandGroups");
            }
        }

        /// <summary>
        /// Gets the group descriptors.
        /// </summary>
        /// <value>The group descriptors.</value>
        [Browsable(false)]
        public GroupDescriptorCollection GroupDescriptors
        {
            get
            {
                return this.CollectionView.GroupDescriptors;
            }
        }

        /// <summary>
        /// Gets the filter descriptors.
        /// </summary>
        /// <value>The filter descriptors.</value>
        [Browsable(false)]
        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.filterDescriptors;
            }
        }

        /// <summary>
        /// Gets the sort descriptors.
        /// </summary>
        /// <value>The sort descriptors.</value>
        [Browsable(false)]
        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.listSource.CollectionView.SortDescriptors;
            }
        }

        /// <summary>
        /// Gets or sets the sort order of Nodes.
        /// </summary>
        /// <value>The sort order.</value>
        [Browsable(false)]
        [DefaultValue(SortOrder.None)]
        public SortOrder SortOrder
        {
            get
            {
                if (this.SortDescriptors.Count > 0)
                {
                    ListSortDirection direction = this.SortDescriptors[0].Direction;
                    switch (direction)
                    {
                        case ListSortDirection.Ascending:
                            return SortOrder.Ascending;
                        case ListSortDirection.Descending:
                            return SortOrder.Descending;
                    }
                }

                return SortOrder.None;
            }
            set
            {
                if (value == SortOrder.None)
                {
                    if (this.SortDescriptors.Count > 0)
                    {
                        this.SortDescriptors.RemoveAt(0);
                    }

                    return;
                }

                ListSortDirection direction = ListSortDirection.Ascending;
                if (value == SortOrder.Descending)
                {
                    direction = ListSortDirection.Descending;
                }

                if (this.SortDescriptors.Count == 0)
                {
                    this.SortDescriptors.Add("Name", direction);
                    return;
                }

                this.SortDescriptors[0].Direction = direction;
            }
        }

        /// <summary>
        /// Gets or sets the height of the items.
        /// </summary>
        /// <value>The height of the item.</value>
        [Browsable(true), DefaultValue(24)]
        [Description("Gets or sets a value indicating the height of the RadPropertyGrid items.")]
        public int ItemHeight
        {
            get
            {
                return (int)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
                this.Update(UpdateActions.StateChanged);
            }
        }

        /// <summary>
        /// Gets or sets the width of the indentation of subitems.
        /// </summary>
        [Description("Gets or sets the width of the indentation of subitems."),
        Category(RadDesignCategory.LayoutCategory),
        Browsable(true), DefaultValue(20)]
        public int ItemIndent
        {
            get
            {
                return (int)GetValue(ItemIndentProperty);
            }
            set
            {
                SetValue(ItemIndentProperty, value);
                this.InvalidateMeasure(true);
            }
        }        

        /// <summary>
        /// Gets or sets the object which properties the RadPropertyGrid is displaying.
        /// </summary>
        [Description("Gets or sets the object which properties the RadPropertyGrid is displaying."),
        Browsable(false), DefaultValue(null)]
        public Object SelectedObject
        {
            get
            {
                if (this.currentObject != null)
                {
                    return this.currentObject;
                }
                
                return null;
            }
            set
            {
                PropertyGridSelectedObjectChangingEventArgs args = new PropertyGridSelectedObjectChangingEventArgs(value);
                this.OnSelectedObjectChnging(args);

                if (args.Cancel)
                {
                    return;
                }

                this.currentObject = value;
                BuildSingleObjectDataSource();
                this.OnSelectedObjectChanged(new PropertyGridSelectedObjectChangedEventArgs(value));
            }
        }

        /// <summary>
        /// Gets the collection to which the RadPropertyGrid is bound to.
        /// </summary>
        [Description("Gets the collection of items the RadPropertyGrid is bound to."),
        Browsable(false)]
        public RadCollectionView<PropertyGridItem> CollectionView
        {
            get
            {
                return this.listSource.CollectionView;
            }
        }
        
        public RadListSource<PropertyGridItem> ListSource
        {
            get
            {
                return this.listSource;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridTableElement"/> selected item.
        /// </summary>
        public PropertyGridItemBase SelectedGridItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (this.selectedItem != value)
                {
                    ProcessSelection(value, false);
                }
            }
        }     

        public virtual PropertyGridGroupItemCollection Groups
        {
            get
            {
                return groupItems;
            }
        }

        public virtual PropertyGridRootItemsCollection PropertyItems
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets or sets the context menu.
        /// </summary>
        /// <value>The context menu.</value>
        public virtual RadContextMenu ContextMenu
        {
            get { return this.contextMenu; }
            set
            {
                if (this.contextMenu != value)
                {
                    this.contextMenu = value;
                    this.OnNotifyPropertyChanged("ContextMenu");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how user begins editing a cell.
        /// </summary>
        [Description("Gets or sets a value indicating how user begins editing a cell.")]
        [Browsable(true), DefaultValue(RadPropertyGridBeginEditModes.BeginEditOnClick)]
        public RadPropertyGridBeginEditModes BeginEditMode
        {
            get
            {
                return this.beginEditMode; 
            }
            set
            {
                if (this.beginEditMode != value)
                {
                    this.beginEditMode = value;
                    OnNotifyPropertyChanged("BeginEditMode");
                }
            }
        }

        public override int ItemSpacing
        {
            get
            {
                return base.ItemSpacing;
            }
            set
            {
                if (ItemSpacing != value)
                {
                    base.ItemSpacing = value;
                    this.ViewElement.Invalidate();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs before the selected object is changed.
        /// </summary>
        public event PropertyGridSelectedObjectChangingEventHandler SelectedObjectChanging;

        protected virtual void OnSelectedObjectChnging(PropertyGridSelectedObjectChangingEventArgs e)
        {
            PropertyGridSelectedObjectChangingEventHandler handler = this.SelectedObjectChanging;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs after the property grid selected object has been changed.
        /// </summary>
        public event PropertyGridSelectedObjectChangedEventHandler SelectedObjectChanged;

        protected virtual void OnSelectedObjectChanged(PropertyGridSelectedObjectChangedEventArgs e)
        {
            PropertyGridSelectedObjectChangedEventHandler handler = this.SelectedObjectChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }     

        /// <summary>
        /// Occurs when <see cref="PropertyGridItemElement"/> is formatting
        /// </summary>
        public event PropertyGridItemFormattingEventHandler ItemFormatting;

        protected internal virtual void OnItemFormatting(PropertyGridItemFormattingEventArgs e)
        {
            PropertyGridItemFormattingEventHandler handler = ItemFormatting;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        /// <summary>
        /// Occurs when a mouse button is pressed on the <see cref="PropertyGridItemElement"/>.
        /// </summary>
        public event PropertyGridMouseEventHandler ItemMouseDown;

        protected internal virtual void OnItemMouseDown(PropertyGridMouseEventArgs e)
        {
            PropertyGridMouseEventHandler handler = this.ItemMouseDown;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a mouse button is clicked inside a <see cref="PropertyGridItemElementBase"/>
        /// </summary>
        public event RadPropertyGridEventHandler ItemMouseClick;

        protected internal virtual void OnItemMouseClick(RadPropertyGridEventArgs e)
        {
            RadPropertyGridEventHandler handler = this.ItemMouseClick;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a mouse button is double clicked inside a <see cref="PropertyGridItemElementBase"/>
        /// </summary>
        public event RadPropertyGridEventHandler ItemMouseDoubleClick;

        protected internal virtual void OnItemMouseDoubleClick(RadPropertyGridEventArgs e)
        {
            RadPropertyGridEventHandler handler = this.ItemMouseDoubleClick;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when mouse moves over a <see cref="PropertyGridItemElement"/>.
        /// </summary>
        public event PropertyGridMouseEventHandler ItemMouseMove;

        protected internal virtual void OnItemMouseMove(PropertyGridMouseEventArgs e)
        {
            PropertyGridMouseEventHandler handler = this.ItemMouseMove;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        /// <summary>
        /// Occurs when item is expanding.
        /// </summary>
        public event RadPropertyGridCancelEventHandler ItemExpandedChanging;

        protected internal virtual void OnItemExpandedChanging(RadPropertyGridCancelEventArgs e)
        {
            RadPropertyGridCancelEventHandler handler = this.ItemExpandedChanging;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when item has been expanded.
        /// </summary>
        public event RadPropertyGridEventHandler ItemExpandedChanged;

        protected internal virtual void OnItemExpandedChanged(RadPropertyGridEventArgs e)
        {
            RadPropertyGridEventHandler handler = this.ItemExpandedChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the selected item is changing
        /// </summary>
        public event RadPropertyGridCancelEventHandler SelectedGridItemChanging;

        protected virtual void OnSelectedGridItemChanging(RadPropertyGridCancelEventArgs args)
        {
            RadPropertyGridCancelEventHandler handler = SelectedGridItemChanging;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Occurs when selected item has been changed.
        /// </summary>
        public event RadPropertyGridEventHandler SelectedGridItemChanged;

        protected virtual void OnSelectedGridItemChanged(RadPropertyGridEventArgs args)
        {
            RadPropertyGridEventHandler handler = SelectedGridItemChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Occurs when editor is required.
        /// </summary>
        public event PropertyGridEditorRequiredEventHandler EditorRequired;

        protected virtual void OnEditorRequired(PropertyGridEditorRequiredEventArgs e)
        {
            PropertyGridEditorRequiredEventHandler handler = EditorRequired;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when editing is started.
        /// </summary>
        public event PropertyGridItemEditingEventHandler Editing;

        protected virtual void OnEditing(PropertyGridItemEditingEventArgs e)
        {
            PropertyGridItemEditingEventHandler handler = Editing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when editor is initialized.
        /// </summary>
        public event PropertyGridItemEditorInitializedEventHandler EditorInitialized;

        protected virtual void OnEditorInitialized(PropertyGridItemEditorInitializedEventArgs e)
        {
            PropertyGridItemEditorInitializedEventHandler handler = EditorInitialized;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when editing has been finished.
        /// </summary>
        public event PropertyGridItemEditedEventHandler Edited;

        protected virtual void OnEdited(PropertyGridItemEditedEventArgs e)
        {
            PropertyGridItemEditedEventHandler handler = Edited;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when item's value is changing.
        /// </summary>
        public event PropertyGridItemValueChangingEventHandler PropertyValueChanging;

        protected internal virtual void OnPropertyValueChanging(PropertyGridItemValueChangingEventArgs e)
        {
            PropertyGridItemValueChangingEventHandler handler = PropertyValueChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when item's value has been changed.
        /// </summary>
        public event PropertyGridItemValueChangedEventHandler PropertyValueChanged;

        protected internal virtual void OnPropertyValueChanged(PropertyGridItemValueChangedEventArgs e)
        {
            PropertyGridItemValueChangedEventHandler handler = PropertyValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event PropertyGridContextMenuOpeningEventHandler ContextMenuOpening;

        protected virtual void OnContextMenuOpening(PropertyGridContextMenuOpeningEventArgs e)
        {
            PropertyGridContextMenuOpeningEventHandler handler = ContextMenuOpening;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public CreatePropertyGridItemEventHandler CreateItem;

        protected virtual void OnCreateItem(CreatePropertyGridItemEventArgs e)
        {
            CreatePropertyGridItemEventHandler handler = CreateItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public CreatePropertyGridItemElementEventHandler CreateItemElement;

        protected internal virtual void OnCreateItemElement(CreatePropertyGridItemElementEventArgs e)
        {
            CreatePropertyGridItemElementEventHandler handler = CreateItemElement;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires when a property value is validating.
        /// </summary>
        public event PropertyValidatingEventHandler PropertyValidating;

        protected internal virtual void OnPropertyValidating(PropertyValidatingEventArgs e)
        {
            PropertyValidatingEventHandler handler = PropertyValidating;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires when a peoperty has finished validating.
        /// </summary>
        public event PropertyValidatedEventHandler PropertyValidated;

        protected internal virtual void OnPropertyValidated(PropertyValidatedEventArgs e)
        {
            PropertyValidatedEventHandler handler = PropertyValidated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Begins the update.
        /// </summary>
        public void BeginUpdate()
        {
            this.updateSuspendedCount++;
            this.resumeAction = UpdateActions.Resume;
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        public void EndUpdate()
        {
            EndUpdate(true, resumeAction);
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        /// <param name="performUpdate">Tells the view whether an update is required or not.</param>
        /// <param name="action">Indicates the update action</param>
        public void EndUpdate(bool performUpdate, UpdateActions action)
        {
            if (this.updateSuspendedCount == 0)
            {
                return;
            }

            this.updateSuspendedCount--;
            if (this.updateSuspendedCount == 0 && performUpdate)
            {
                Update(action);
                return;
            }
        }

        /// <summary>
        /// Updates the visual items in the property grid
        /// </summary>
        /// <param name="updateAction">Indicated the update action</param>
        public void Update(UpdateActions updateAction)
        {
            if (this.updateSuspendedCount > 0)
            {
                if (updateAction == UpdateActions.Reset)
                {
                    this.resumeAction = UpdateActions.Reset;
                }
                return;
            }

            if (updateAction == UpdateActions.ExpandedChanged)
            {
                this.ViewElement.UpdateItems();
                this.Scroller.UpdateScrollRange();
                this.SynchronizeVisualItems();

                if (this.VScrollBar.Visibility == ElementVisibility.Visible && this.VScrollBar.Value > this.VScrollBar.Maximum - this.VScrollBar.LargeChange + 1)
                {
                    SetScrollValue(this.VScrollBar, this.VScrollBar.Maximum - this.VScrollBar.LargeChange + 1);
                }

                return;
            }

            if (updateAction == UpdateActions.Reset)
            {
                this.VScrollBar.Value = this.VScrollBar.Minimum;
                this.ViewElement.ElementProvider.ClearCache();
                this.ViewElement.DisposeChildren();
                this.ViewElement.ElementProvider.ClearCache();
            }

            if (updateAction == UpdateActions.Reset ||
                updateAction == UpdateActions.Resume ||
                updateAction == UpdateActions.SortChanged)
            {
                this.UpdateScrollers(updateAction);
                this.Scroller.UpdateScrollRange();
            }

            this.ViewElement.UpdateItems();

            if (updateAction == UpdateActions.StateChanged ||
                updateAction == UpdateActions.SortChanged ||
                updateAction == UpdateActions.Resume ||
                updateAction == UpdateActions.ValueChanged)
            {
                this.SynchronizeVisualItems();
            }
        }

        /// <summary>
        /// Gets the element at specified coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>An instance of <see cref="PropertyGridItemBase"/> if successfull.</returns>
        public PropertyGridItemElementBase GetElementAt(int x, int y)
        {
            if (!this.IsInValidState(true))
            {
                return null;
            }
            
            RadElement element = this.ElementTree.GetElementAtPoint(new Point(x, y));

            if (element == null)
            {
                return null;
            }

            if (element is PropertyGridItemElementBase)
            {
                return element as PropertyGridItemElementBase;
            }            
            return element.FindAncestor<PropertyGridItemElementBase>();
        }

        /// <summary>
        /// Ensures the item is visible within the RadPropertygridElement and scrolls the element if needed.
        /// </summary>
        /// <param name="item">The item to visualize.</param>
        public void EnsureVisible(PropertyGridItemBase item)
        {
            PropertyGridItemElementBase element = this.GetElement(item);

            if (element == null)
            {
                if (this.ViewElement.Children.Count > 0)
                {
                    PropertyGridItemBase firstVisibleItem = ((PropertyGridItemElementBase)this.ViewElement.Children[0]).Data;
                    PropertyGridItemBase lastVisibleItem = ((PropertyGridItemElementBase)this.ViewElement.Children[this.ViewElement.Children.Count - 1]).Data;

                    int firstItemIndex = traverser.GetIndex(firstVisibleItem);
                    int itemIndex = traverser.GetIndex(item);

                    if (itemIndex <= firstItemIndex)
                    {
                        this.Scroller.ScrollToItem(item);
                    }
                    else
                    {
                        EnsureVisibleCore(item);
                    }
                }
            }
            else
            {
                if (element.ControlBoundingRectangle.Bottom > this.ViewElement.ControlBoundingRectangle.Bottom)
                {
                    int offset = element.ControlBoundingRectangle.Bottom - this.ViewElement.ControlBoundingRectangle.Bottom;
                    offset = Math.Max(1, offset);
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + offset);
                }
                else if (element.ControlBoundingRectangle.Top <= this.ViewElement.ControlBoundingRectangle.Top)
                {
                    int offset = this.ViewElement.ControlBoundingRectangle.Top - element.ControlBoundingRectangle.Top;
                    offset = Math.Max(1, offset);
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value - offset);
                }
            }
            this.UpdateLayout();
        }

        protected virtual void EnsureVisibleCore(PropertyGridItemBase item)
        {
            bool start = false;
            int offset = 0;
            PropertyGridItemBase lastVisible = ((PropertyGridItemElementBase)this.ViewElement.Children[this.ViewElement.Children.Count - 1]).Data;
            PropertyGridTraverser traverser = (PropertyGridTraverser)this.Scroller.Traverser.GetEnumerator();

            while (traverser.MoveNext())
            {
                if (traverser.Current == item)
                {
                    int oldMaximum = this.VScrollBar.Maximum;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + offset);
                    this.UpdateLayout();
                    PropertyGridItemElementBase itemElement = this.GetElement(item);

                    if (itemElement != null &&
                        itemElement.ControlBoundingRectangle.Bottom > this.ViewElement.ControlBoundingRectangle.Bottom)
                    {
                        this.EnsureVisible(item);
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
        /// Scrolls the scrollbar to bring the specified <see cref="PropertyGridItemBase"/> into view.
        /// </summary>
        /// <param name="item">The item to visualize.</param>
        public void ScrollToItem(PropertyGridItemBase item)
        {
            Scroller.ScrollToItem(item);
        }

        /// <summary>
        /// Initializes and returns the context menu associated with the specified <see cref="PropertyGridItemElementBase"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>An instance of <see cref="RadContextMenu"/> if successfull.</returns>
        public RadContextMenu GetElementContextMenu(PropertyGridItemElementBase element)
        {
            RadContextMenu menu = this.ContextMenu;

            if (element != null && element.Data.ContextMenu != null)
            {
                menu = element.Data.ContextMenu;
            }

            if (element == null && menu is PropertyGridDefaultContextMenu)
            {
                menu = null;
            }

            if (menu != null)
            {
                PropertyGridDefaultContextMenu defaultMenu = menu as PropertyGridDefaultContextMenu;

                if (defaultMenu != null)
                {
                    if (element.Data.Expanded && element.Data.GridItems.Count > 0)
                    {
                        defaultMenu.ExpandCollapseMenuItem.Text = PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuCollapse);
                    }
                    else
                    {
                        defaultMenu.ExpandCollapseMenuItem.Text = PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuExpand);
                    }
                    defaultMenu.ExpandCollapseMenuItem.Enabled = element.Data.GridItems.Count > 0;
                    defaultMenu.EditMenuItem.Enabled = !ReadOnly;
                }

                if (element != null)
                {
                    this.SelectedGridItem = element.Data;
                }

                PropertyGridContextMenuOpeningEventArgs args = new PropertyGridContextMenuOpeningEventArgs(element.Data, menu);
                OnContextMenuOpening(args);

                if (!args.Cancel)
                {
                    return menu;
                }
            }

            return null;
        }

        /// <summary>
        /// Makes the property grid columns even.
        /// </summary>
        public void ResetColumnWidths()
        {
            this.ratio = 0.5f;
            this.ValueColumnWidth = (int)(this.currentAvailableWidth * this.ratio);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Performs the needed operations on the data layer when the <see cref="PropertySort"/> mode is changed.
        /// </summary>
        /// <param name="propertySort"></param>
        protected virtual void PerformPropertySort(PropertySort propertySort)
        {
            switch (propertySort)
            {
                case PropertySort.Alphabetical:
                    this.SortDescriptors.Clear();
                    this.GroupDescriptors.Clear();
                    this.SortDescriptors.Add("Name", ListSortDirection.Ascending);
                    break;
                case PropertySort.Categorized:
                    this.SortDescriptors.Clear();
                    this.GroupDescriptors.Clear();
                    this.GroupDescriptors.Add("Category", ListSortDirection.Ascending);
                    break;
                case PropertySort.CategorizedAlphabetical:
                    this.SortDescriptors.Clear();
                    this.SortDescriptors.Add("Name", ListSortDirection.Ascending);
                    this.GroupDescriptors.Clear();
                    this.GroupDescriptors.Add("Category", ListSortDirection.Ascending);
                    break;
                case PropertySort.NoSort:
                    this.GroupDescriptors.Clear();
                    this.SortDescriptors.Clear();
                    break;
            }

            PropertyGridElement propertyGridElement = this.PropertyGridElement;

            if (propertyGridElement != null)
            {
                propertyGridElement.ToolbarElement.SyncronizeToggleButtons();
            }

            this.Update(UpdateActions.Reset);
        }

        /// <summary>
        /// Gets the default property for the selected object
        /// </summary>
        /// <returns>The <see cref="PropertyGridItem"/> that is the default property.</returns>
        protected virtual PropertyGridItem GetSelectedObjectDefaultProperty()
        {
            Object[] defaultProperty = this.SelectedObject.GetType().GetCustomAttributes(typeof(DefaultPropertyAttribute), false);
            string defaultPropertyName = String.Empty;
            if (defaultProperty.Length > 0)
            {
                defaultPropertyName = ((DefaultPropertyAttribute)defaultProperty[0]).Name;
            }

            PropertyGridItem defaultItem = null;
            if (defaultPropertyName != String.Empty)
            {
                if (this.CollectionView.Count > 0)
                {
                    foreach (PropertyGridItem item in this.CollectionView)
                    {
                        if (item.Name == defaultPropertyName)
                        {
                            defaultItem = item;
                        }
                    }
                }
            }

            return defaultItem;
        }

        protected virtual void UpdateScrollers(UpdateActions updateAction)
        {
            if (this.listSource.CollectionView.Count > 0)
            {
            }
            else
            {
                this.VScrollBar.Value = this.VScrollBar.Minimum;
                this.VScrollBar.Maximum = this.VScrollBar.Minimum;
            }
        }

        /// <summary>
        /// Syncronizes all visual elements.
        /// </summary>
        protected virtual void SynchronizeVisualItems()
        {
            foreach (PropertyGridItemElementBase item in this.ViewElement.Children)
            {
                item.Synchronize();
            }
            this.Invalidate();
        }

        private void BuildSingleObjectDataSource()
        {
            if (currentObject != null)
            {
                PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(currentObject, new Attribute[] { new BrowsableAttribute(true) }, false);
                this.listSource.DataSource = descriptors;
            }
            else
            {
                this.listSource.DataSource = null;
            }
        }

        private bool IsScrollBar(Point point)
        {
            if (this.ElementTree == null)
            {
                return false;
            }

            RadElement scrollBar = this.ElementTree.GetElementAtPoint(point);

            if (scrollBar != null)
            {
                if (scrollBar is RadScrollBarElement)
                {
                    return true;
                }
                else
                {
                    return scrollBar.FindAncestor<RadScrollBarElement>() != null;
                }
            }

            return false;
        }

        private bool IsExpander(Point point)
        {
            if (this.ElementTree == null)
            {
                return false;
            }

            return this.ElementTree.GetElementAtPoint(point) is ExpanderItem;
        }

        private bool IsEditor(Point point)
        {
            if (this.ElementTree == null || this.activeEditor == null)
            {
                return false;
            }
            
            RadElement editor = ((BaseInputEditor)this.activeEditor).EditorElement;

            if (editor != null && editor.ControlBoundingRectangle.Contains(point))
            {
                return true;
            }

            return false;
        }
               
        private void SetScrollValue(RadScrollBarElement scrollbar, int newValue)
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

        private bool IsNumericType(Type itemType)
        {
            if ((itemType.IsArray))
                return false;

            switch (Type.GetTypeCode(itemType))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
            }
            
            return false;
        }

        private void SetValueColumnWidth(int value, bool resetRatio)
        {
            if (value < this.minimumColumnWidth)
            {
                this.valueColumnWidth = this.minimumColumnWidth;
            }
            else if (value > this.currentAvailableWidth - this.minimumColumnWidth)
            {
                this.valueColumnWidth = (int)this.currentAvailableWidth - this.minimumColumnWidth;
            }
            else
            {
                this.valueColumnWidth = value;
            }

            if (resetRatio)
            {
                this.ratio = this.valueColumnWidth / this.currentAvailableWidth;
            }

            this.ViewElement.InvalidateMeasure(true);
        }
        
        #endregion

        #region Event handlers
        
        private void SortDescriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Update(UpdateActions.SortChanged);
        }

        private void filterDescriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.listSource.CollectionView.FilterExpression = this.filterDescriptors.Expression;

            if (this.filterDescriptors.Count > 0)
            {
                this.PropertyGridElement.ToolbarElement.SearchTextBoxElement.Text = this.filterDescriptors[0].Value.ToString();
            }

            this.Update(UpdateActions.Resume);
        }

        private void GroupDescriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.traverser.Reset();
            this.Update(UpdateActions.Resume);
            this.SelectedGridItem = this.selectedItem;
        }

        private void listSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Update(UpdateActions.Reset);
            }
            else
            {
                this.Scroller.UpdateScrollRange();
            }
        }

        protected override void OnAutoSizeChanged()
        {
            if (!AutoSizeItems && Items.Count > 0)
            {
                foreach (PropertyGridItem item in this.Items)
                {
                    item.ItemHeight = -1;
                }
            }

            base.OnAutoSizeChanged();
        }

        protected internal virtual bool ProcessMouseDown(MouseEventArgs e)
        {
            if (IsEditor(e.Location))
            {
                return false;
            }

            PropertyGridItemElementBase itemElement = this.GetElementAt(e.Location.X, e.Location.Y);
            wasSelectedOnMouseDown = itemElement != null && itemElement.Data.Selected;
            wasInEditModeOnMouseDown = IsEditing;

            if (IsEditing)
            {
                if (!this.EndEdit())
                {
                    return false;
                }
                if (itemElement != null && this.selectedItem == itemElement.Data)
                {
                    this.selectionWasCanceled = true;
                    return true;
                }
            }

            if (itemElement != null)
            {
                selectionWasCanceled = !ProcessSelection(itemElement.Data, true);
            }
            else
            {
                return false;
            }
            this.mouseDownLocation = e.Location;
            return selectionWasCanceled;
        }

        protected internal virtual bool ProcessMouseUp(MouseEventArgs e)
        {
            if (selectionWasCanceled)
            {
                return true;
            }

            PropertyGridItemElementBase itemElement = this.GetElementAt(e.Location.X, e.Location.Y);

            if (this.doubleClick || this.IsEditor(e.Location) || itemElement == null || !itemElement.ControlBoundingRectangle.Contains(this.mouseDownLocation))
            {
                this.clickTimer.Stop();
                this.doubleClick = false;
                return false;
            }

            PropertyGridItemElement propertyItemElement = itemElement as PropertyGridItemElement;
            if (this.IsScrollBar(e.Location) || (propertyItemElement != null && propertyItemElement.IsInResizeLocation(e.Location)))
            {
                if (this.IsEditing)
                {
                    this.EndEdit();
                }

                return false;
            }
            
            if (e.Button == MouseButtons.Left)
            {
                PropertyGridItemElement element = itemElement as PropertyGridItemElement;
                if (element != null && element.TextElement.PropertyValueButton.ControlBoundingRectangle.Contains(e.Location))
                {
                    return false;
                } 
                
                if ((this.beginEditMode == RadPropertyGridBeginEditModes.BeginEditOnClick || wasInEditModeOnMouseDown) && !IsExpander(e.Location))
                {
                    BeginEdit();
                }
                else if (this.beginEditMode == RadPropertyGridBeginEditModes.BeginEditOnSecondClick && 
                    (this.wasSelectedOnMouseDown || this.wasInEditModeOnMouseDown) && !IsExpander(e.Location))
                {
                    this.clickTimer.Interval = SystemInformation.DoubleClickTime;
                    this.clickTimer.Start();
                }
            }

            return false;
        }

        protected internal virtual bool ProcessMouseMove(MouseEventArgs e)
        {
            if (IsScrollBar(e.Location))
            {
                this.ElementTree.Control.Cursor = Cursors.Default;
            }
            return false;
        }

        protected internal virtual bool ProecessMouseEnter(EventArgs e)
        {
            return false;
        }

        protected internal virtual bool ProecessMouseLeave(EventArgs e)
        {
            return false;
        }

        protected internal virtual bool ProcessMouseClick(MouseEventArgs e)
        {
            return false;
        }

        protected internal virtual bool ProcessMouseDoubleClick(MouseEventArgs e)
        {
            PropertyGridItemElementBase itemElement = this.GetElementAt(e.Location.X, e.Location.Y);

            PropertyGridItemElement resizeElement = itemElement as PropertyGridItemElement;
            bool resized = false;
            if (resizeElement != null && resizeElement.IsInResizeLocation(e.Location))
            {
                this.ResetColumnWidths();
                resized = true;
            }

            if (itemElement != null && itemElement.Data.GridItems.Count > 0)
            {
                if (this.activeEditor != null)
                {
                    if (!this.EndEdit())
                    {
                        this.doubleClick = true;
                        return false;
                    }
                }
                if (!this.IsExpander(e.Location) && !resized && beginEditMode == RadPropertyGridBeginEditModes.BeginEditOnSecondClick)
                {
                    itemElement.Data.Expanded = !itemElement.Data.Expanded;
                }
            }
            else if (beginEditMode == RadPropertyGridBeginEditModes.BeginEditOnSecondClick)
            {
                PropertyGridItemElement element = itemElement as PropertyGridItemElement;
                if (element != null)
                {
                    BeginEdit();
                }
            }

            this.doubleClick = true;
            return false;
        }

        protected internal virtual bool ProcessMouseWheel(MouseEventArgs e)
        {
            if (!this.IsEditing)
            {
                int step = Math.Max(1, e.Delta / SystemInformation.MouseWheelScrollDelta);
                int delta = Math.Sign(e.Delta) * step * SystemInformation.MouseWheelScrollLines;
                int value = this.VScrollBar.Value - delta * this.VScrollBar.SmallChange;
                SetScrollValue(this.VScrollBar, value);
            }

            return false;
        }

        protected internal virtual bool ProcessKeyDown(KeyEventArgs e)
        {
            if (this.IsEditing)
            {
                return false;
            }

            switch (e.KeyCode)
            {
                case Keys.Left:
                    HandleLeftKey(e);
                    break;
                case Keys.Right:
                    HandleRightKey(e);
                    break;
                case Keys.Up:
                    HandleUpKey(e);
                    break;
                case Keys.Down:
                    HandleDownKey(e);
                    break;
                case Keys.End:
                    HandleEndKey(e);
                    break;
                case Keys.Home:
                    HandleHomeKey(e);
                    break;
                case Keys.Add:
                    HandleAddKey(e);
                    break;
                case Keys.Subtract:
                    HandleSubtractKey(e);
                    break;
                case Keys.Escape:
                    HandleEscapeKey(e);
                    break;
                case Keys.F2:
                case Keys.Enter:
                case Keys.Space:
                    HandleF2Key(e);
                    return true;
                case Keys.PageUp:
                    HandlePageUpKey(e);
                    break;
                case Keys.PageDown:
                    HandlePageDownKey(e);
                    break;
            }

            return false;
        }

        protected internal virtual bool ProcessContextMenu(Point location)
        {
            if (IsScrollBar(location) || IsExpander(location))
            {
                return false;
            }

            if (this.activeEditor != null)
            {
                if (!this.EndEdit())
                {
                    return false;
                }
            }

            PropertyGridItemElementBase element = this.GetElementAt(location.X, location.Y);
            RadContextMenu menu = GetElementContextMenu(element);
            if (menu != null)
            {
                menu.Show(this.ElementTree.Control, location);
                return true;
            }

            return false;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (this.pendingScrollerUpdates)
            {
                this.pendingScrollerUpdates = false;
                this.Update(UpdateActions.Resume);
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.BindingContextProperty)
            {
                EventHandler handler = BindingContextChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
        
        private void clickTimer_Tick(object sender, EventArgs e)
        {
            this.clickTimer.Stop();
            this.BeginEdit();
        }
          
        #endregion

        #region Selection

        protected virtual bool ProcessSelection(PropertyGridItemBase item, bool isMouseSelection)
        {
            if (this.selectedItem == item)
            {
                if (this.IsEditing)
                {
                    return this.EndEdit();   
                }

                return true;
            }

            RadPropertyGridCancelEventArgs args = new RadPropertyGridCancelEventArgs(item);
            this.OnSelectedGridItemChanging(args);

            if (args.Cancel)
            {
                return false;
            }

            if (this.selectedItem != null && IsEditing)
            {
                if (!this.EndEdit())
                {
                    return false;
                }
            }

            this.selectedItem = item;

            this.Update(UpdateActions.StateChanged);
            if (!isMouseSelection)
            {
                this.EnsureVisible(item);
            }

            this.OnNotifyPropertyChanged("SelectedItem");
            this.OnSelectedGridItemChanged(new RadPropertyGridEventArgs(item));

            return true;
        }

        #endregion

        #region Keyboard handling

        private void HandleEscapeKey(KeyEventArgs e)
        {
            this.CancelEdit();
        }

        private void HandleSubtractKey(KeyEventArgs e)
        {
            if (this.selectedItem != null)
            {
                this.selectedItem.Collapse();
            }
        }

        private void HandleAddKey(KeyEventArgs e)
        {
            if (this.selectedItem != null)
            {
                this.selectedItem.Expand();
            }
        }

        private void HandleHomeKey(KeyEventArgs e)
        {
            PropertyGridTraverser traverser = new PropertyGridTraverser(this);
            if (traverser.MoveToFirst())
            {
                this.ProcessSelection(traverser.Current, false);
            }
        }

        private void HandleEndKey(KeyEventArgs e)
        {
            PropertyGridTraverser traverser = new PropertyGridTraverser(this);
            if (traverser.MoveToEnd())
            {
                this.ProcessSelection(traverser.Current, false);
            }
        }

        private void HandleLeftKey(KeyEventArgs e)
        {
            if (this.selectedItem == null)
            {
                return;
            }

            if (this.selectedItem.Expandable && this.selectedItem.Expanded)
            {
                this.selectedItem.Collapse();
                return;
            }

            HandleUpKey(e);
        }

        private void HandleRightKey(KeyEventArgs e)
        {
            if (this.selectedItem == null)
            {
                return;
            }

            if (this.selectedItem.Expandable && !this.selectedItem.Expanded)
            {
                this.selectedItem.Expand();
                return;
            }

            HandleDownKey(e);
        }

        private void HandleUpKey(KeyEventArgs e)
        {
            if (this.selectedItem == null)
            {
                PropertyGridTraverser traverser = new PropertyGridTraverser(this);
                if (traverser.MoveNext())
                {
                    this.ProcessSelection(traverser.Current, false);
                }
            }
            else
            {
                PropertyGridTraverser traverser = new PropertyGridTraverser(this);
                traverser.MoveTo(this.selectedItem);
                if (traverser.MovePrevious())
                {
                    PropertyGridItemBase item = traverser.Current;
                    if (traverser.MovePrevious())
                    {
                        this.ProcessSelection(item, false);
                    }
                }
            }
        }

        private void HandleDownKey(KeyEventArgs e)
        {
            if (this.selectedItem == null)
            {
                PropertyGridTraverser traverser = new PropertyGridTraverser(this);
                if (traverser.MoveNext())
                {
                    this.ProcessSelection(traverser.Current, false);
                }
            }
            else
            {
                PropertyGridTraverser traverser = new PropertyGridTraverser(this);
                traverser.MoveTo(this.selectedItem);
                if (traverser.MoveNext())
                {
                    this.ProcessSelection(traverser.Current, false);
                }
            }
        }

        private void HandleF2Key(KeyEventArgs e)
        {
            this.BeginEdit();
        }

        private void HandlePageUpKey(KeyEventArgs e)
        {
            PropertyGridItemElementBase treeNodeElement = GetFirstFullVisibleElement();

            if (treeNodeElement != null)
            {
                if (!treeNodeElement.Data.Selected)
                {
                    this.SelectedGridItem = treeNodeElement.Data;
                }
                else
                {
                    int delta = ViewElement.ControlBoundingRectangle.Height - treeNodeElement.ControlBoundingRectangle.Bottom;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value - delta);
                    this.ViewElement.UpdateItems();
                    this.UpdateLayout();
                    treeNodeElement = GetFirstFullVisibleElement();

                    if (treeNodeElement != null)
                    {
                        this.SelectedGridItem = treeNodeElement.Data;
                    }
                }
            }
        }

        private void HandlePageDownKey(KeyEventArgs e)
        {
            PropertyGridItemElementBase itemElement = GetLastFullVisibleElement();

            if (itemElement != null)
            {
                if (!itemElement.Data.Selected)
                {
                    this.SelectedGridItem = itemElement.Data;
                }
                else
                {
                    int delta = itemElement.ControlBoundingRectangle.Top;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + delta);
                    this.ViewElement.UpdateItems();
                    this.UpdateLayout();
                    itemElement = GetLastFullVisibleElement();
                    if (itemElement != null)
                    {
                        this.SelectedGridItem = itemElement.Data;
                    }
                }
            }
        }

        private PropertyGridItemElementBase GetLastFullVisibleElement()
        {
            for (int i = ViewElement.Children.Count - 1; i >= 0; i--)
            {
                PropertyGridItemElementBase temp = (PropertyGridItemElementBase)ViewElement.Children[i];
                if (temp.ControlBoundingRectangle.Bottom <= ViewElement.ControlBoundingRectangle.Bottom)
                {
                    return temp;
                }
            }
            return null;
        }

        private PropertyGridItemElementBase GetFirstFullVisibleElement()
        {
            for (int i = 0; i < ViewElement.Children.Count - 1; i++)
            {
                PropertyGridItemElementBase temp = (PropertyGridItemElementBase)ViewElement.Children[i];
                if (temp.ControlBoundingRectangle.Top >= ViewElement.ControlBoundingRectangle.Top)
                {
                    return temp;
                }
            }
            return null;
        }

        #endregion

        #region IDataItemSource Members

        /// <summary>
        /// Occurs when [binding context changed].
        /// </summary>
        public event EventHandler BindingContextChanged;  

        public IDataItem NewItem()
        {
            CreatePropertyGridItemEventArgs args = new CreatePropertyGridItemEventArgs(typeof(PropertyGridItem));
            OnCreateItem(args);

            PropertyGridItem item = null;
            if (args.Item != null)
            {
                item = args.Item as PropertyGridItem;
            }

            if (item == null)
            {
                item = new PropertyGridItem(this);
            }

            return item;
        }

        public void Initialize()
        {
            PropertyGridItemComparer comparer = this.listSource.CollectionView.Comparer as PropertyGridItemComparer;
            if (comparer != null)
            {
                comparer.Update();
            }
        }

        public void BindingComplete()
        {
        }
        
        #endregion

        #region Editing

        /// <summary>
        /// Gets the type of editor used for a editing the given item.
        /// </summary>
        /// <param name="item">The item to get editor type for.</param>
        /// <returns>The type of the editor</returns>
        protected virtual Type GetEditorTypeForItem(PropertyGridItem item)
        {
            Type itemType = item.PropertyType;

            if (itemType.IsEnum || itemType == typeof(bool))
            {
                return typeof(PropertyGridDropDownListEditor);
            }
            else if (this.IsNumericType(itemType))
            {
                return typeof(PropertyGridSpinEditor);
            }
            else if (itemType == typeof(DateTime))
            {
                return typeof(PropertyGridDateTimeEditor);
            }
            else if (itemType == typeof(Color))
            {
                return typeof(PropertyGridColorEditor);
            }
            else if (itemType == typeof(Image))
            {
                return typeof(PropertyGridBrowseEditor);
            }
            else
            {
                return typeof(PropertyGridTextBoxEditor);
            }
        }

        /// <summary>
        /// Puts the current item in edit mode.
        /// </summary>
        /// <returns></returns>
        public virtual bool BeginEdit()
        {
            if (this.readOnly || this.activeEditor != null || this.SelectedGridItem == null || this.selectedItem is PropertyGridGroupItem)
            {
                return false;
            }

            PropertyGridItem item = this.selectedItem as PropertyGridItem;
            if (item != null && !item.Enabled)
            {
                return false;
            }

            EnsureVisible(SelectedGridItem);

            Type editorType = this.GetEditorTypeForItem((PropertyGridItem)this.selectedItem);

            PropertyGridEditorRequiredEventArgs editorRequiredEventArgs = new PropertyGridEditorRequiredEventArgs(this.SelectedGridItem, editorType);
            OnEditorRequired(editorRequiredEventArgs);

            IInputEditor editor = editorRequiredEventArgs.Editor as IInputEditor;
            if (editor == null)
            {
                editor = GetEditor(editorRequiredEventArgs.EditorType);
            }

            if (editor == null)
            {
                return false;
            }

            this.activeEditor = editor;

            PropertyGridItemEditingEventArgs editingEventArgs = new PropertyGridItemEditingEventArgs(this.selectedItem, editor);
            this.OnEditing(editingEventArgs);

            if (editingEventArgs.Cancel)
            {
                this.activeEditor = null;
                return false;
            }

            if (this.GetElement(this.selectedItem) == null)
            {
                this.SelectedGridItem.EnsureVisible();
            }

            PropertyGridItemElement visualItem = GetElement(this.SelectedGridItem) as PropertyGridItemElement;

            if (visualItem == null)
            {
                this.activeEditor = null;
                return false;
            }

            visualItem.AddEditor(editor);

            ISupportInitialize initializable = this.activeEditor as ISupportInitialize;
            if (initializable != null)
            {
                initializable.BeginInit();
            }

            PropertyGridItem itemToEdit = this.selectedItem as PropertyGridItem;

            if (itemToEdit == null)
            {
                this.activeEditor = null;
                return false;
            }

            activeEditor.Initialize(visualItem, itemToEdit.FormattedValue);

            if (initializable != null)
            {
                initializable.EndInit();
            }

            OnEditorInitialized(new PropertyGridItemEditorInitializedEventArgs(visualItem, editor));

            this.activeEditor.BeginEdit();

            this.cachedOldValue = itemToEdit.FormattedValue;

            return true;
        }

        /// <summary>
        ///  Commits any changes and ends the edit operation on the current item.
        /// </summary>
        /// <returns></returns>
        public bool EndEdit()
        {
            return EndEditCore(true);
        }

        /// <summary>
        /// Close the currently active editor and discard changes.
        /// </summary>
        /// <returns></returns>
        public void CancelEdit()
        {
            EndEditCore(false);
        }

        /// <summary>
        /// Ends the editing of an item and commits or discards the changes.
        /// </summary>
        /// <param name="commitChanges">Determines if the changes are commited [true] or discarded [false].</param>
        /// <returns></returns>
        protected virtual bool EndEditCore(bool commitChanges)
        {
            this.clickTimer.Stop();
            if (!IsEditing)
            {
                return false;
            }

            PropertyGridItemElement visualItem = GetElement(this.SelectedGridItem) as PropertyGridItemElement;
            if (visualItem == null)
            {
                return false;
            }

            if (commitChanges && this.activeEditor.IsModified)
            {                
                PropertyGridItem item = this.selectedItem as PropertyGridItem;

                PropertyValidatingEventArgs args = new PropertyValidatingEventArgs(visualItem.Data, this.activeEditor.Value, item.Value);
                OnPropertyValidating(args);
                if (args.Cancel)
                {
                    return false;
                }
                OnPropertyValidated(new PropertyValidatedEventArgs(visualItem.Data));

                if (item != null)
                {
                    if (item.PropertyType == typeof(Image))
                    {
                        item.Value = Image.FromFile(this.activeEditor.Value.ToString());
                    }
                    else
                    {
                        item.Value = this.activeEditor.Value;
                    }
                }
            }

            this.activeEditor.EndEdit();
            visualItem.RemoveEditor(this.activeEditor);

            this.InvalidateMeasure();
            UpdateLayout();

            OnEdited(new PropertyGridItemEditedEventArgs(visualItem, activeEditor, !commitChanges));

            this.activeEditor = null;

            return true;
        }

        /// <summary>
        /// Gets an editor depending on the type of the value to be edited.
        /// </summary>
        /// <param name="editorType">The type of the value.</param>
        /// <returns></returns>
        protected virtual IInputEditor GetEditor(Type editorType)
        {
            IInputEditor editor = null;

            if (!this.cachedEditors.TryGetValue(editorType, out editor))
            {
                editor = Activator.CreateInstance(editorType) as IInputEditor;
            }

            if (editor != null && !this.cachedEditors.ContainsValue(editor))
            {
                this.cachedEditors.Add(editorType, editor);
            }

            return editor;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);

            float availableWidth = availableSize.Width - this.ItemIndent;

            if (this.VScrollBar.Visibility == ElementVisibility.Visible)
            {
                availableWidth -= this.VScrollBar.DesiredSize.Width;
            }

            this.currentAvailableWidth = availableWidth;
            int newColumnWidth = (int)(availableWidth * this.ratio);
            this.SetValueColumnWidth(newColumnWidth, false);
           
            return desiredSize;
        }

        #endregion
    }
}
