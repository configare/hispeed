using System;
using Telerik.WinControls.Data;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the main element of <see cref="RadListView"/>.
    /// </summary>
    public class RadListViewElement : LightVisualElement, IDataItemSource
    {
        #region Nested

        public enum UpdateModes
        {
            InvalidateItems = 1,
            InvalidateMeasure = InvalidateItems << 1,
            UpdateLayout = InvalidateMeasure << 1,
            Invalidate = UpdateLayout << 1,
            UpdateScroll = Invalidate << 1,
            RefreshLayout = InvalidateMeasure | UpdateLayout | Invalidate,
            RefreshAll = InvalidateItems | InvalidateMeasure | UpdateLayout | Invalidate | UpdateScroll
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the content of the SelectedItems collection has changed.
        /// </summary>
        public event EventHandler SelectedItemsChanged;

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        public event ListViewItemEventHandler SelectedItemChanged;

        /// <summary>
        /// Occurs when the ViewType of <see cref="RadListView">RadListView</see> is changed.
        /// </summary>
        public event EventHandler ViewTypeChanged;

        /// <summary>
        /// Occurs when the ViewType of <see cref="RadListView">RadListView</see> is about to change. Cancelable.
        /// </summary>
        public event ViewTypeChangingEventHandler ViewTypeChanging;

        /// <summary>
        /// Occurs when the user presses a mouse button over a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemMouseEventHandler ItemMouseDown;

        /// <summary>
        /// Occurs when the user presses a mouse button over a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemMouseEventHandler ItemMouseUp;

        /// <summary>
        /// Occurs when the user moves the mouse over a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemMouseEventHandler ItemMouseMove;

        /// <summary>
        /// Occurs when the user hovers a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseHover;

        /// <summary>
        /// Occurs when the mouse pointer enters a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseEnter;

        /// <summary>
        /// Occurs when the mouse pointer leaves a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseLeave;

        /// <summary>
        /// Occurs when the user clicks a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseClick;

        /// <summary>
        /// Occurs when the user double-clicks a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseDoubleClick;

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> is about to be checked. Cancelable.
        /// </summary>
        public event ListViewItemCancelEventHandler ItemCheckedChanging;

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> is checked.
        /// </summary>
        public event ListViewItemEventHandler ItemCheckedChanged;

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> changes its state and needs to be formatted.
        /// </summary>
        public event ListViewVisualItemEventHandler VisualItemFormatting;

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> needs to be created.
        /// </summary>
        public event ListViewItemCreatingEventHandler ItemCreating;

        /// <summary>
        /// Occurs when a <see cref="BaseListViewVisualItem">BaseListViewVisualItem</see> needs to be created;
        /// </summary>
        public event ListViewVisualItemCreatingEventHandler VisualItemCreating;

        /// <summary>
        /// Occurs when a DetailsView cell needs to be formatted.
        /// </summary>
        public event ListViewCellFormattingEventHandler CellFormatting;

        /// <summary>
        /// Occurs when a data-bound item is being attached to a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemDataBound;

        /// <summary>
        /// Occurs when the CurrentItem property is changed.
        /// </summary>
        public event ListViewItemEventHandler CurrentItemChanged;

        /// <summary>
        /// Occurs when the CurrentItem property is about to change. Cancelable.
        /// </summary>
        public event ListViewItemChangingEventHandler CurrentItemChanging;

        /// <summary>
        /// Occurs when an editor is required.
        /// </summary>
        public event ListViewItemEditorRequiredEventHandler EditorRequired;

        /// <summary>
        /// Occurs when an edit operation is about to begin. Cancelable.
        /// </summary>
        public event ListViewItemEditingEventHandler ItemEditing;

        /// <summary>
        /// Occurs when an editor is initialized.
        /// </summary>
        public event ListViewItemEditorInitializedEventHandler EditorInitialized;

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> is edited.
        /// </summary>
        public event ListViewItemEditedEventHandler ItemEdited;

        /// <summary>
        /// Fires when a validation error occurs.
        /// </summary>
        public event EventHandler ValidationError;

        /// <summary>
        /// Occurs when an edit operation needs to be validated.
        /// </summary>
        public event ListViewItemValidatingEventHandler ItemValidating;

        /// <summary>
        /// Occurs when the value of a <see cref="ListViewDataItem">ListViewDataItem</see> is changed.
        /// </summary>
        public event ListViewItemValueChangedEventHandler ItemValueChanged;

        /// <summary>
        /// Occurs when the value of a <see cref="ListViewDataItem">ListViewDataItem</see> is about to change. Cancelable.
        /// </summary>
        public event ListViewItemValueChangingEventHandler ItemValueChanging;

        /// <summary>
        /// Occurs when a <see cref="ListViewDetailColumn"/> needs to be created.
        /// </summary>
        public event ListViewColumnCreatingEventHandler ColumnCreating;

        /// <summary>
        /// Occurs when a <see cref="DetailListViewCellElement"/> needs to be created.
        /// </summary>
        public event ListViewCellElementCreatingEventHandler CellCreating;

        /// <summary>
        /// Occurs when an item is about to be removed using the Delete key. Cancelable.
        /// </summary>
        public event ListViewItemCancelEventHandler ItemRemoving;

        /// <summary>
        /// Occurs when an item is removed using the Delete key.
        /// </summary>
        public event ListViewItemEventHandler ItemRemoved;

        #endregion

        #region Event Management

        protected virtual void OnSelectedItemChanged(ListViewDataItem item)
        {
            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(this, new ListViewItemEventArgs(item));
            }
        }

        protected internal virtual void OnSelectedItemsChanged()
        {
            if (this.SelectedItemsChanged != null)
            {
                this.SelectedItemsChanged(this, EventArgs.Empty);
            }
        }

        protected virtual bool OnViewTypeChanging(ViewTypeChangingEventArgs args)
        {
            if (this.ViewTypeChanging != null)
            {
                this.ViewTypeChanging(this, args);
            }

            return args.Cancel;
        }

        protected virtual void OnViewTypeChanged()
        {
            if (this.ViewTypeChanged != null)
            {
                this.ViewTypeChanged(this, EventArgs.Empty);
            }
        }

        protected internal virtual void OnCellFormatting(ListViewCellFormattingEventArgs args)
        {
            if (this.CellFormatting != null)
            {
                this.CellFormatting(this, args);
            }
        }

        protected internal virtual bool OnItemCheckedChanging(ListViewItemCancelEventArgs args)
        {
            if (this.ItemCheckedChanging != null)
            {
                this.ItemCheckedChanging(this, args);
            }

            return args.Cancel;
        }

        protected internal virtual void OnItemCheckedChanged(ListViewItemEventArgs args)
        {
            if (this.ItemCheckedChanged != null)
            {
                this.ItemCheckedChanged(this, args);
            }
        }

        protected internal virtual void OnItemMouseEnter(ListViewItemEventArgs args)
        {
            if (this.ItemMouseEnter != null)
            {
                this.ItemMouseEnter(this, args);
            }
        }

        protected internal virtual void OnItemMouseLeave(ListViewItemEventArgs args)
        {
            if (this.ItemMouseLeave != null)
            {
                this.ItemMouseLeave(this, args);
            }
        }

        protected internal virtual void OnItemMouseDown(ListViewItemMouseEventArgs args)
        {
            if (this.ItemMouseDown != null)
            {
                this.ItemMouseDown(this, args);
            }
        }

        protected internal virtual void OnItemMouseUp(ListViewItemMouseEventArgs args)
        {
            if (this.ItemMouseUp != null)
            {
                this.ItemMouseUp(this, args);
            }
        }

        protected internal virtual void OnItemMouseMove(ListViewItemMouseEventArgs args)
        {
            if (this.ItemMouseMove != null)
            {
                this.ItemMouseMove(this, args);
            }
        }

        protected internal virtual void OnItemMouseHover(ListViewItemEventArgs args)
        {
            if (this.ItemMouseHover != null)
            {
                this.ItemMouseHover(this, args);
            }
        }

        protected internal virtual void OnItemMouseClick(ListViewItemEventArgs args)
        {
            if (this.ItemMouseClick != null)
            {
                this.ItemMouseClick(this, args);
            }
        }

        protected internal virtual void OnItemMouseDoubleClick(ListViewItemEventArgs args)
        {
            if (this.ItemMouseDoubleClick != null)
            {
                this.ItemMouseDoubleClick(this, args);
            }
        }

        protected virtual void OnCurrecntItemChanged(ListViewItemEventArgs args)
        {
            if (this.CurrentItemChanged != null)
            {
                this.CurrentItemChanged(this, args);
            }
        }

        protected virtual bool OnCurrentItemChanging(ListViewItemChangingEventArgs args)
        {
            if (this.CurrentItemChanging != null)
            {
                this.CurrentItemChanging(this, args);
            }

            return args.Cancel;
        }

        protected virtual void OnValueChanged(ListViewItemValueChangedEventArgs args)
        {
            if (this.ItemValueChanged != null)
            {
                this.ItemValueChanged(this, args);
            }
        }

        protected virtual bool OnValueChanging(ListViewItemValueChangingEventArgs args)
        {
            if (this.ItemValueChanging != null)
            {
                this.ItemValueChanging(this, args);
            }

            return args.Cancel;
        }

        protected virtual void OnValidationError(EventArgs args)
        {
            if (this.ValidationError != null)
            {
                this.ValidationError(this, args);
            }
        }

        protected virtual void OnValueValidating(ListViewItemValidatingEventArgs args)
        {
            if (this.ItemValidating != null)
            {
                this.ItemValidating(this, args);
            }
        }

        protected virtual void OnEdited(ListViewItemEditedEventArgs args)
        {
            if (this.ItemEdited != null)
            {
                this.ItemEdited(this, args);
            }
        }

        protected internal virtual void OnItemDataBound(ListViewDataItem item)
        {
            if (this.ItemDataBound != null)
            {
                this.ItemDataBound(this, new ListViewItemEventArgs(item));
            }
        }

        protected internal virtual void OnVisualItemFormatting(BaseListViewVisualItem item)
        {
            if (this.VisualItemFormatting != null)
            {
                this.VisualItemFormatting(this, new ListViewVisualItemEventArgs(item));
            }
        }

        protected virtual void OnDataItemCreating(ListViewItemCreatingEventArgs args)
        {
            if (this.ItemCreating != null)
            {
                this.ItemCreating(this, args);
            }
        }

        protected internal virtual void OnVisualItemCreating(ListViewVisualItemCreatingEventArgs args)
        {
            if (this.VisualItemCreating != null)
            {
                this.VisualItemCreating(this, args);
            }
        }

        protected virtual void OnColumnCreating(ListViewColumnCreatingEventArgs args)
        {
            if (this.ColumnCreating != null)
            {
                this.ColumnCreating(this, args);
            }
        }

        protected internal virtual void OnCellCreating(ListViewCellElementCreatingEventArgs args)
        {
            if (this.CellCreating != null)
            {
                this.CellCreating(this, args);
            }
        }

        protected internal virtual bool OnItemRemoving(ListViewItemCancelEventArgs args)
        {
            if (this.ItemRemoving != null)
            {
                this.ItemRemoving(this, args);

                return args.Cancel;
            }

            return false;
        }

        protected internal virtual void OnItemRemoved(ListViewItemEventArgs args)
        {
            if (this.ItemRemoved != null)
            {
                this.ItemRemoved(this, args);
            }
        }

        #endregion

        #region Fields

        private ListViewType viewType = ListViewType.ListView;
        private BaseListViewElement viewElement;
        private ListViewDataItemCollection items;
        private ListViewListSource listSource;
        private BindingContext bindingContext;
        private string displayMember = "";
        private string valueMember = "";
        private ListViewDataItemGroupCollection groups;
        private ListViewFilterDescriptorCollection filterDescriptors;
        private ListViewColumnCollection columns;
        private bool enableCustomGrouping;
        private Dictionary<Type, IInputEditor> cachedEditors = new Dictionary<Type, IInputEditor>();
        private ColumnResizingBehavior resizingBehavior;

        private object cachedOldValue;
        private bool allowEdit = true;
        private bool allowRemove = true;
        private bool allowColumnReorder = true;
        private bool showCheckBoxes = false;
        private bool showGroups = false;
        private bool showColumnHeaders = true;
        private bool enableColumnSort = false;
        private bool enableKineticScrolling = false;
        private bool hotTracking = true;
        private bool isEndingEdit = false;
        private bool isEditorInitializing = false;

        private bool enableSorting;
        private bool enableFiltering = false;
        private bool enableGrouping = false;

        private IInputEditor activeEditor;
        private bool multiSelect = false;
        private ListViewSelectedItemCollection selectedItems;
        private ListViewCheckedItemCollection checkedItems;
        private ListViewDataItem currentItem;
        private ListViewDataItem selectedItem;
        private ListViewDetailColumn currentColumn;
        private float headerHeight = 35;
        private bool itemsMeasureValid;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether items should react on mouse hover.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether items should react on mouse hover.")]
        public bool HotTracking
        {
            get { return this.hotTracking; }
            set
            {
                if (this.hotTracking != value)
                {
                    this.hotTracking = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the kinetic scrolling function is enabled.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the kinetic scrolling function is enabled.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool EnableKineticScrolling
        {
            get
            {
                return this.enableKineticScrolling;
            }
            set
            {
                if (this.enableKineticScrolling != value)
                {
                    this.enableKineticScrolling = value;
                    if (!value)
                    {
                        this.viewElement.ScrollBehavior.Stop();
                    }
                }

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items should be sorted when clicking on header cells.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the items should be sorted when clicking on header cells.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool EnableColumnSort
        {
            get
            {
                return this.enableColumnSort;
            }
            set
            {
                this.enableColumnSort = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column headers should be drawn.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the column headers should be drawn.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ShowColumnHeaders
        {
            get
            {
                return this.showColumnHeaders;
            }
            set
            {
                if (this.showColumnHeaders != value)
                {
                    this.showColumnHeaders = value;
                    this.Update(UpdateModes.InvalidateMeasure);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items should be shown in groups.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(true)]
        [Description("Gets or sets a value indicating whether the items should be shown in groups.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ShowGroups
        {
            get
            {
                return this.showGroups;
            }
            set
            {
                if (this.showGroups != value)
                {
                    this.showGroups = value;
                    this.Update(UpdateModes.RefreshLayout | UpdateModes.UpdateScroll);
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether checkboxes should be shown.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets value indicating whether checkboxes should be shown.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ShowCheckBoxes
        {
            get
            {
                return this.showCheckBoxes;
            }
            set
            {
                if (this.showCheckBoxes != value)
                {
                    this.showCheckBoxes = value;

                    if (!this.FullRowSelect && this.AllowArbitraryItemWidth)
                    {
                        this.IsItemsMeasureValid = false;
                    }

                    this.Update(UpdateModes.RefreshLayout);
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating if the user can reorder columns via drag and drop.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets or sets value indicating if the user can reorder columns via drag and drop.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool AllowColumnReorder
        {
            get
            {
                return this.allowColumnReorder;
            }
            set
            {
                this.allowColumnReorder = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating if the user can resize the columns.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets or sets value indicating if the user can resize the columns.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool AllowColumnResize
        {
            get
            {
                return this.resizingBehavior.AllowColumnResize;
            }
            set
            {
                this.resizingBehavior.AllowColumnResize = value;
            }
        }

        /// <summary>
        /// Gets or sets the current column in Details View.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the current column in Details View.")]
        public ListViewDetailColumn CurrentColumn
        {
            get
            {
                return currentColumn;
            }
            set
            {
                if (this.currentColumn != value)
                {
                    if (this.currentColumn != null)
                        this.currentColumn.SetValue(ListViewDetailColumn.CurrentProperty, false);

                    this.EndEdit();
                    this.currentColumn = value;
                    this.EnsureColumnVisible(this.currentColumn);

                    if (this.currentColumn != null)
                        this.currentColumn.SetValue(ListViewDetailColumn.CurrentProperty, true);
                }
            }
        }

        /// <summary>
        /// Indicates whether there is an active editor.
        /// </summary>
        [Browsable(false)]
        [Description("Indicates whether there is an active editor.")]
        public bool IsEditing
        {
            get
            {
                return this.ActiveEditor != null;
            }
        }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the current item.")]
        public ListViewDataItem CurrentItem
        {
            get
            {
                return this.currentItem;
            }
            set
            {
                if (this.currentItem != value && !this.OnCurrentItemChanging(new ListViewItemChangingEventArgs(this.currentItem, value)))
                {
                    if (this.currentItem != null)
                    {
                        this.currentItem.Current = false;
                    }

                    this.currentItem = value;

                    if (this.currentItem != null)
                    {
                        if (this.Items.Contains(value))
                        {
                            this.listSource.CollectionView.CurrentChanged -= CollectionView_CurrentChanged;
                            this.listSource.CollectionView.MoveCurrentTo(this.currentItem);
                            this.listSource.CollectionView.CurrentChanged += CollectionView_CurrentChanged;
                        }

                        this.currentItem.Current = true;
                        this.EnsureItemVisible(this.currentItem);
                    }
                    this.OnCurrecntItemChanged(new ListViewItemEventArgs(this.currentItem));
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the index of the selected item.")]
        public int SelectedIndex
        {
            get
            {
                if (this.selectedItem == null)
                {
                    return -1;
                }

                return this.items.IndexOf(this.selectedItem);
            }
            set
            {
                if (this.items.Count > value && value >= 0)
                {
                    this.SelectedItem = this.items[value];
                }
                else
                {
                    this.SelectedItem = null;
                }
            }
        }


        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the selected item.")]
        public ListViewDataItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (this.selectedItem != value)
                {
                    this.ViewElement.ProcessSelection(value, Keys.None, false);
                }
            }
        }

        /// <summary>
        /// Gets a collection containing the selected items.
        /// </summary>
        [Browsable(false)]
        [Description("Gets a collection containing the selected items.")]
        public ListViewSelectedItemCollection SelectedItems
        {
            get
            {
                return selectedItems;
            }
        }

        /// <summary>
        /// Gets a collection containing the checked items.
        /// </summary>
        [Browsable(false)]
        [Description("Gets a collection containing the checked items.")]
        public ListViewCheckedItemCollection CheckedItems
        {
            get
            {
                return checkedItems;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether multi selection is enabled.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets value indicating whether multi selection is enabled.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool MultiSelect
        {
            get
            {
                return multiSelect;
            }
            set
            {
                multiSelect = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether editing is enabled.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets or sets value indicating whether editing is enabled.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool AllowEdit
        {
            get
            {
                return allowEdit;
            }
            set
            {
                allowEdit = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether the user can remove items with the Delete key.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets or sets value indicating whether the user can remove items with the Delete key.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool AllowRemove
        {
            get
            {
                return allowRemove;
            }
            set
            {
                allowRemove = value;
            }
        }

        /// <summary>
        /// Gets the currently active editor.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the currently active editor.")]
        public IInputEditor ActiveEditor
        {
            get
            {
                return this.activeEditor;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items can have different height.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the items can have different height.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool AllowArbitraryItemHeight
        {
            get
            {
                return this.ViewElement.AllowArbitraryItemHeight;
            }
            set
            {
                this.ViewElement.AllowArbitraryItemHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items can have different width.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the items can have different width.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool AllowArbitraryItemWidth
        {
            get
            {
                return this.ViewElement.AllowArbitraryItemWidth;
            }
            set
            {
                this.ViewElement.AllowArbitraryItemWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the full row should be selected.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the full row should be selected.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool FullRowSelect
        {
            get
            {
                return this.ViewElement.FullRowSelect;
            }
            set
            {
                this.ViewElement.FullRowSelect = value;
            }
        }

        /// <summary>
        /// Gets or sets the default item size.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "0,0")]
        [Description("Gets or sets the default item size.")]
        [Category(RadDesignCategory.LayoutCategory)]
        public Size ItemSize
        {
            get
            {
                return this.ViewElement.ItemSize;
            }
            set
            {
                this.ViewElement.ItemSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the default group item size.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "0,0")]
        [Description("Gets or sets the default group item size.")]
        [Category(RadDesignCategory.LayoutCategory)]
        public Size GroupItemSize
        {
            get
            {
                return this.ViewElement.GroupItemSize;
            }
            set
            {
                this.ViewElement.GroupItemSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the indent of the items when they are displayed in a group.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(25)]
        [Description("Gets or sets the indent of the items when they are displayed in a group.")]
        [Category(RadDesignCategory.LayoutCategory)]
        public int GroupIndent
        {
            get
            {
                return this.ViewElement.GroupIndent;
            }
            set
            {
                this.ViewElement.GroupIndent = value;
            }
        }

        /// <summary>
        /// Gets or sets the space between the items.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Description("Gets or sets the space between the items.")]
        [Category(RadDesignCategory.LayoutCategory)]
        public int ItemSpacing
        {
            get
            {
                return this.ViewElement.ItemSpacing;
            }
            set
            {
                this.ViewElement.ItemSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="ListViewDetailColumn">ListViewDetailColumn</see> object which represent the columns in DetailsView.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.ListViewColumnCollectionDesignerString, typeof(UITypeEditor))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets a collection of ListViewDetailColumn object which represent the columns in DetailsView.")]
        public ListViewColumnCollection Columns
        {
            get
            {
                return this.columns;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in bound mode.
        /// </summary>
        [Browsable(false)]
        [Description("Gets a value indicating whether the control is in bound mode.")]
        public bool IsDataBound
        {
            get
            {
                return this.DataSource != null;
            }
        }

        /// <summary>
        /// Gets a collection containg the groups of the RadListViewElement.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.ListViewGroupCollectionDesignerString, typeof(UITypeEditor))]
        [Description("Gets a collection containg the groups of the RadListViewElement.")]
        [Category(RadDesignCategory.DataCategory)]
        public ListViewDataItemGroupCollection Groups
        {
            get
            {
                return this.groups;
            }
        }

        /// <summary>
        /// Gets or sets the value member.
        /// </summary>
        [Browsable(true)]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets the value member.")]
        public string ValueMember
        {
            get
            {
                return this.valueMember;
            }
            set
            {
                if (this.valueMember != value)
                {
                    this.valueMember = value;
                    this.SynchronizeVisualItems();
                    this.ViewElement.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the display member.
        /// </summary>
        [Browsable(true)]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets the display member.")]
        public string DisplayMember
        {
            get
            {
                return this.displayMember;
            }
            set
            {
                if (this.displayMember != value)
                {
                    this.displayMember = value;
                    this.SynchronizeVisualItems();
                    this.ViewElement.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets the DataView collection.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the DataView collection")]
        public RadCollectionView<ListViewDataItem> DataView
        {
            get
            {
                return this.listSource.CollectionView;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sorting is enabled.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets a value indicating whether sorting is enabled.")]
        public bool EnableSorting
        {
            get
            {
                return (this.IsDesignMode) ? this.enableSorting : this.listSource.CollectionView.CanSort;
            }
            set
            {
                if (!this.IsDesignMode && this.listSource.CollectionView.CanSort != value)
                {
                    this.listSource.CollectionView.CanSort = value;
                }
                else
                {
                    enableSorting = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets a value indicating whether filtering is enabled.")]
        public bool EnableFiltering
        {
            get
            {
                return (this.IsDesignMode) ? this.enableFiltering : this.listSource.CollectionView.CanFilter;
            }
            set
            {
                if (!this.IsDesignMode && this.listSource.CollectionView.CanFilter != value)
                {
                    this.listSource.CollectionView.CanFilter = value;
                }
                else
                {
                    enableFiltering = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets a value indicating whether filtering is enabled.")]
        public bool EnableGrouping
        {
            get
            {
                return (this.IsDesignMode) ? this.enableGrouping : this.listSource.CollectionView.CanGroup;
            }
            set
            {
                if (!this.IsDesignMode && this.listSource.CollectionView.CanGroup != value)
                {
                    this.listSource.CollectionView.CanGroup = value;

                    this.Update(UpdateModes.RefreshLayout | UpdateModes.UpdateScroll);
                }
                else
                {
                    enableGrouping = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether custom grouping is enabled.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets a value indicating whether custom grouping is enabled.")]
        public bool EnableCustomGrouping
        {
            get
            {
                return this.enableCustomGrouping;
            }
            set
            {
                if (value != this.enableCustomGrouping)
                {
                    this.enableCustomGrouping = value;
                    this.Update(UpdateModes.UpdateScroll | UpdateModes.RefreshLayout);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="FilterDescriptor">filter descriptors</see> by which you can apply filter rules to the items.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets a collection of filter descriptors by which you can apply filter rules to the items.")]
        [Category(RadDesignCategory.DataCategory)]
        public ListViewFilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return filterDescriptors;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="SortDescriptor">SortDescriptor</see> which are used to define sorting rules over the 
        /// <see cref="ListViewDataItemCollection">ListViewDataItemCollection</see>.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection of SortDescriptor which are used to define sorting rules over the ListViewDataItemCollection.")]
        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.listSource.CollectionView.SortDescriptors;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="GroupDescriptor">GroupDescriptor</see> which are used to define grouping rules over the 
        /// <see cref="ListViewDataItemCollection">ListViewDataItemCollection</see>.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection of GroupDescriptor which are used to define grouping rules over the ListViewDataItemCollection.")]
        public GroupDescriptorCollection GroupDescriptors
        {
            get
            {
                return this.listSource.CollectionView.GroupDescriptors;
            }
        }

        /// <summary>
        /// Gets the source of the items.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the source of the items.")]
        public RadListSource<ListViewDataItem> ListSource
        {
            get
            {
                return listSource;
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="ListViewDataItem">ListViewDataItem</see> object which represent the items in RadListViewElement.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.ListViewItemCollectionDesignerString, typeof(UITypeEditor))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets a collection of ListViewDataItem object which represent the items in RadListViewElement.")]
        public ListViewDataItemCollection Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets the element that represents the active view.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the element that represents the active view.")]
        public BaseListViewElement ViewElement
        {
            get
            {
                return viewElement;
            }
        }

        /// <summary>
        /// Gets or sets the type of the view.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(ListViewType.ListView)]
        [Description("Gets or sets the type of the view.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public ListViewType ViewType
        {
            get
            {
                return viewType;
            }
            set
            {
                if (this.viewType != value && !this.OnViewTypeChanging(new ViewTypeChangingEventArgs(this.viewType, value)))
                {
                    this.viewElement.ScrollBehavior.Stop();
                    this.Children.Remove(this.viewElement);
                    this.viewElement.Dispose();

                    viewType = value;
                    this.viewElement = this.CreateViewElement();
                    this.Children.Add(this.viewElement);

                    this.InvalidateMeasure(true);
                    this.Update(UpdateModes.RefreshAll);

                    this.OnViewTypeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the data source of a RadListViewElement.
        /// </summary>
        [Browsable(true)]
        [AttributeProvider(typeof(IListSource))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets the data source of a RadListViewElementElement.")]
        public object DataSource
        {
            get
            {
                return this.listSource.DataSource;
            }
            set
            {
                if (value == this.DataSource)
                {
                    return;
                }

                if (value == null)
                {
                    this.DisplayMember = "";
                    this.ValueMember = "";
                }

                this.listSource.DataSource = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the <see cref="RadListViewElement"/> is displaying data. 
        /// </summary>
        public string DataMember
        {
            get { return this.ListSource.DataMember; }
            set
            {
                if (this.ListSource.DataMember != value)
                {
                    this.ListSource.DataMember = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the header in Details View.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(35.0f)]
        [Description("Gets or sets the height of the header in Details View.")]
        [Category(RadDesignCategory.LayoutCategory)]
        public float HeaderHeight
        {
            get { return this.headerHeight; }
            set
            {
                if (this.headerHeight != value)
                {
                    this.headerHeight = value;
                    this.Update(UpdateModes.RefreshLayout | UpdateModes.UpdateScroll);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ColumnResizingBehavior"/> that is responsible for resizing the columns.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the ColumnResizingBehavior that is responsible for resizing the columns.")]
        public ColumnResizingBehavior ColumnResizingBehavior
        {
            get
            {
                return resizingBehavior;
            }
            set
            {
                resizingBehavior = value;
            }
        }


        internal bool IsItemsMeasureValid
        {
            get
            {
                return this.itemsMeasureValid;
            }
            set
            {
                this.itemsMeasureValid = value;
            }
        }

        #endregion

        #region Selection

        internal void SetSelectedItem(ListViewDataItem item)
        {
            if (this.selectedItem != item && !(item is ListViewDataItemGroup))
            {
                this.EndEdit();
                this.selectedItem = item;
                this.OnSelectedItemChanged(item);
            }
        }

        #endregion

        #region Editing

        /// <summary>
        /// Begins an edit operation over the currently sellected item.
        /// </summary>
        /// <returns>[true] if success, [false] otherwise</returns>
        public virtual bool BeginEdit()
        {
            if (!this.allowEdit || this.activeEditor != null ||
                this.SelectedItems.Count > 1 || this.SelectedItem == null ||
                (this.viewType == ListViewType.DetailsView && this.currentColumn == null))
            {
                return false;
            }

            this.EnsureItemVisible(this.selectedItem, true);

            if (this.viewType == ListViewType.DetailsView)
            {
                this.EnsureColumnVisible(this.currentColumn);
            }

            BaseListViewVisualItem visualItem = this.viewElement.GetElement(this.selectedItem);

            if (visualItem == null)
            {
                this.activeEditor = null;
                return false;
            }

            ListViewItemEditorRequiredEventArgs editorRequiredEventArgs = new ListViewItemEditorRequiredEventArgs(this.SelectedItem, typeof(ListViewTextBoxEditor));
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

            ListViewItemEditingEventArgs editingEventArgs = new ListViewItemEditingEventArgs(this.SelectedItem, editor);
            this.OnEditing(editingEventArgs);

            if (editingEventArgs.Cancel)
            {
                this.activeEditor = null;
                return false;
            }


            this.isEditorInitializing = true;


            visualItem.AddEditor(editor);

            ISupportInitialize initializable = this.activeEditor as ISupportInitialize;
            if (initializable != null)
            {
                initializable.BeginInit();
            }

            if (visualItem is DetailListViewVisualItem)
            {
                activeEditor.Initialize(visualItem, this.SelectedItem[this.currentColumn]);
            }
            else
            {
                activeEditor.Initialize(visualItem, this.SelectedItem.Value);
            }

            if (initializable != null)
            {
                initializable.EndInit();
            }

            OnEditorInitialized(new ListViewItemEditorInitializedEventArgs(visualItem, editor));

            this.activeEditor.BeginEdit();

            this.cachedOldValue = this.SelectedItem.Value;

            this.isEditorInitializing = false;

            return true;
        }

        /// <summary>
        /// Ends the current edit operations if such. Saves the changes.
        /// </summary>
        /// <returns>[true] if success, [false] otherwise</returns>
        public bool EndEdit()
        {
            return this.EndEditCore(true);
        }

        /// <summary>
        /// Ends the current edit operations if such. Discards the changes.
        /// </summary>
        /// <returns>[true] if success, [false] otherwise</returns>
        public bool CancelEdit()
        {
            return this.EndEditCore(false);
        }

        protected virtual void OnEditorInitialized(ListViewItemEditorInitializedEventArgs e)
        {
            if (this.EditorInitialized != null)
            {
                this.EditorInitialized(this, e);
            }
        }

        protected virtual void OnEditing(ListViewItemEditingEventArgs e)
        {
            if (this.ItemEditing != null)
            {
                this.ItemEditing(this, e);
            }
        }

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

        protected virtual void OnEditorRequired(ListViewItemEditorRequiredEventArgs e)
        {
            if (this.EditorRequired != null)
            {
                this.EditorRequired(this, e);
            }
        }

        protected virtual bool EndEditCore(bool commitChanges)
        {
            if (!IsEditing || this.isEndingEdit || this.isEditorInitializing)
            {
                return false;
            }
            
            this.isEndingEdit = true;

            BaseListViewVisualItem visualItem = this.viewElement.GetElement(this.SelectedItem);
            if (visualItem == null)
            {
                this.isEndingEdit = false;
                return false;
            }

            ListViewDataItem editedItem = visualItem.Data;

            if (commitChanges && this.ActiveEditor.IsModified)
            {
                SaveEditorValue(visualItem, this.ActiveEditor.Value);
            }

            this.activeEditor.EndEdit();
            visualItem.RemoveEditor(this.activeEditor);
            this.activeEditor = null;

            this.Update(UpdateModes.RefreshLayout);
            this.EnsureItemVisible(editedItem);

            OnEdited(new ListViewItemEditedEventArgs(visualItem, activeEditor, !commitChanges));
             
            this.ElementTree.Control.Focus();

            this.isEndingEdit = false;
            return true;
        }

        protected virtual void SaveEditorValue(BaseListViewVisualItem visualItem, object newValue)
        {  
            if (Object.Equals(newValue, String.Empty))
            {
                newValue = null;
            }

            ListViewItemValidatingEventArgs validatingArgs = new ListViewItemValidatingEventArgs(visualItem, cachedOldValue, newValue);
            OnValueValidating(validatingArgs);
            if (validatingArgs.Cancel)
            {
                this.OnValidationError(EventArgs.Empty);
                return;
            }

            newValue = validatingArgs.NewValue;

            ListViewItemValueChangingEventArgs valueChangingArgs = new ListViewItemValueChangingEventArgs(this.SelectedItem, newValue, cachedOldValue);
            OnValueChanging(valueChangingArgs);

            if (!valueChangingArgs.Cancel)
            {
                if (this.ViewType == ListViewType.DetailsView)
                {
                    this.SelectedItem[this.CurrentColumn] = newValue;
                }
                else
                {
                    this.SelectedItem.Value = newValue;
                }

                OnValueChanged(new ListViewItemValueChangedEventArgs(this.SelectedItem));
            }
             
        }

        #endregion

        #region IDataItemSource Members

        /// <summary>
        /// Occurs when the BindingContext has changed.
        /// </summary>
        public event EventHandler BindingContextChanged;

        /// <summary>
        /// Occurs when the procces of binding <see cref="RadListViewElement"/> to a data source has finished
        /// </summary>
        public event EventHandler BindingCompleted;

        /// <summary>
        /// Gets or sets the BindingContext.
        /// </summary>
        public override BindingContext BindingContext
        {
            get
            {
                return this.bindingContext;
            }
            set
            {
                if (this.bindingContext != value)
                {
                    this.bindingContext = value;
                    this.OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        public IDataItem NewItem()
        {
            ListViewDataItem newItem = new ListViewDataItem();
            newItem.Owner = this;
            ListViewItemCreatingEventArgs args = new ListViewItemCreatingEventArgs(newItem);
            this.OnDataItemCreating(args);
            if (args.Item != newItem)
            {
                args.Item.Owner = this;
            }

            return args.Item;
        }

        public void Initialize()
        {
            if (this.IsDataBound)
            {
                InitializeBoundColumns();
                RebuildColumnAccessors();
            }
        }

        public void BindingComplete()
        {
            if (this.BindingCompleted != null)
            {
                this.BindingCompleted(this, EventArgs.Empty);
            }
        }

        protected virtual BindingContext CreateBindingContext()
        {
            return new BindingContext();
        }

        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            EventHandler bindingContextChanged = this.BindingContextChanged;

            if (bindingContextChanged != null)
            {
                bindingContextChanged(this, e);
            }
        }

        #endregion

        #region Initialization and Disposal

        static RadListViewElement()
        {
            new Themes.ControlDefault.ListView().DeserializeTheme();
        }

        /// <summary>
        /// Creates a view element corresponding to the current ViewType.
        /// </summary>
        /// <returns>The view element.</returns>
        protected virtual BaseListViewElement CreateViewElement()
        {
            BaseListViewElement newElement = null;

            switch (this.viewType)
            {
                case ListViewType.ListView:
                    newElement = new SimpleListViewElement(this);
                    break;
                case ListViewType.IconsView:
                    newElement = new IconListViewElement(this);
                    break;
                case ListViewType.DetailsView:
                    newElement = new DetailListViewElement(this);
                    break;
                default:
                    newElement = new SimpleListViewElement(this);
                    break;
            }

            return newElement;
        }

        protected virtual void WireEvents()
        {
            this.groups.CollectionChanged += new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(groups_CollectionChanged);
            this.columns.CollectionChanged += new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(columns_CollectionChanged);
            this.filterDescriptors.CollectionChanged += new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(filterDescriptors_CollectionChanged);
            this.ListSource.CollectionView.SortDescriptors.CollectionChanged += new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(SortDescriptors_CollectionChanged);
            this.listSource.CollectionView.CollectionChanged += new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(CollectionView_CollectionChanged);
            this.listSource.CollectionView.CurrentChanged += new EventHandler(CollectionView_CurrentChanged);
        }

        protected virtual void UnwireEvents()
        {
            this.groups.CollectionChanged -= new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(groups_CollectionChanged);
            this.columns.CollectionChanged -= new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(columns_CollectionChanged);
            this.filterDescriptors.CollectionChanged -= new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(filterDescriptors_CollectionChanged);
            this.ListSource.CollectionView.SortDescriptors.CollectionChanged -= new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(SortDescriptors_CollectionChanged);
            this.listSource.CollectionView.CollectionChanged -= new Telerik.WinControls.Data.NotifyCollectionChangedEventHandler(CollectionView_CollectionChanged);
            this.listSource.CollectionView.CurrentChanged -= new EventHandler(CollectionView_CurrentChanged);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.bindingContext = this.CreateBindingContext();
            this.listSource = new ListViewListSource(this);
            this.listSource.CollectionView.CanGroup = false;
            this.listSource.CollectionView.CanSort = false;
            this.listSource.CollectionView.CanFilter = false;
            this.listSource.CollectionView.GroupFactory = new ListViewGroupFactory(this);
            this.filterDescriptors = new ListViewFilterDescriptorCollection();
            this.items = new ListViewDataItemCollection(this);
            this.groups = new ListViewDataItemGroupCollection(this);
            this.columns = new ListViewColumnCollection(this);

            this.selectedItems = new ListViewSelectedItemCollection(this);
            this.checkedItems = new ListViewCheckedItemCollection(this);

            WireEvents();

            this.viewElement = this.CreateViewElement();
            this.Children.Add(this.viewElement);

            this.resizingBehavior = new ColumnResizingBehavior(this);
            this.resizingBehavior.AllowColumnResize = true;
        }

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();

            foreach (KeyValuePair<Type, IInputEditor> keyValuePairEditor in this.cachedEditors)
            {
                IInputEditor editor = keyValuePairEditor.Value;
                IDisposable disposable = null;
                BaseInputEditor baseInputEditor = editor as BaseInputEditor;
                if (baseInputEditor != null)
                {
                    disposable = baseInputEditor.EditorElement as IDisposable;
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

            base.DisposeManagedResources();
        }

        #endregion

        #region Public Methods

        public virtual void Update(UpdateModes updateMode)
        {
            if ((updateMode & UpdateModes.InvalidateItems) == UpdateModes.InvalidateItems)
            {
                this.IsItemsMeasureValid = false;
            }

            if ((updateMode & UpdateModes.InvalidateMeasure) == UpdateModes.InvalidateMeasure)
            {
                this.ViewElement.InvalidateMeasure(true);
            }

            if ((updateMode & UpdateModes.UpdateLayout) == UpdateModes.UpdateLayout)
            {
                this.UpdateLayout();
            }

            if ((updateMode & UpdateModes.Invalidate) == UpdateModes.Invalidate)
            {
                this.ViewElement.ViewElement.Invalidate();
            }

            if ((updateMode & UpdateModes.UpdateScroll) == UpdateModes.UpdateScroll)
            {
                this.ViewElement.Scroller.UpdateScrollRange();
            }
        }

        /// <summary>
        /// Causes synchronization of the visual items with the logical ones.
        /// </summary>
        public virtual void SynchronizeVisualItems()
        {
            foreach (BaseListViewVisualItem visualItem in this.ViewElement.ViewElement.Children)
            {
                visualItem.Synchronize();
            }

            this.Invalidate();
        }

        /// <summary>
        /// Ensures that a given item is visible on the client area.
        /// </summary>
        /// <param name="item">The item to ensure visibility of.</param>
        public virtual void EnsureItemVisible(ListViewDataItem item)
        {
            this.ViewElement.EnsureItemVisible(item);
        }

        /// <summary>
        /// Ensures that a given item is visible on the client area.
        /// </summary>
        /// <param name="item">The item to ensure visibility of.</param>
        /// <param name="ensureHorizontally">Indicates whether the view should be scrolled horizontally.</param>
        public virtual void EnsureItemVisible(ListViewDataItem item, bool ensureHorizontally)
        {
            this.ViewElement.EnsureItemVisible(item, ensureHorizontally);
        }

        /// <summary>
        /// Ensures that a given column is visible on the client area.
        /// </summary>
        /// <param name="listViewDetailColumn">The column to ensure visibility of.</param>
        public virtual void EnsureColumnVisible(ListViewDetailColumn listViewDetailColumn)
        {
            DetailListViewElement detailsElement = this.ViewElement as DetailListViewElement;

            if (detailsElement != null)
            {
                detailsElement.EnsureColumnVisible(listViewDetailColumn);
            }
        }

        /// <summary>
        /// Selects a range of items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Select(ListViewDataItem[] items)
        {
            if (!this.MultiSelect)
            {
                if (items.GetLength(0) > 0)
                {
                    this.viewElement.ProcessSelection(items[items.GetLength(0)], Keys.None, true);
                }

                return;
            }

            foreach (ListViewDataItem item in items)
            {
                if (item.Owner != this)
                {
                    continue;
                }

                item.Selected = true;
            }

            if (items.GetLength(0) > 0)
            {
                this.SetSelectedItem(items[items.GetLength(0)]);
            }
        }

        #endregion

        #region Event Handlers

        void CollectionView_CurrentChanged(object sender, EventArgs e)
        {
            this.ViewElement.ProcessSelection(this.listSource.CollectionView.CurrentItem, Keys.None, true);
        }

        void SortDescriptors_CollectionChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            DetailListViewElement detailsView = (this.viewElement as DetailListViewElement);
            if (detailsView != null)
            {
                foreach (DetailListViewCellElement cell in detailsView.ColumnContainer.Children)
                {
                    cell.Synchronize();
                }
            }

            int offset = this.viewElement.Scroller.Scrollbar.Value;
            this.viewElement.Scroller.Scrollbar.Value = 0;
            this.viewElement.Scroller.Traverser.Reset();
            this.viewElement.Scroller.Scrollbar.Value = offset;
        }

        void groups_CollectionChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            this.SynchronizeVisualItems();
            this.Update(UpdateModes.RefreshLayout | UpdateModes.UpdateScroll);
        }

        void columns_CollectionChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            RebuildColumnAccessors();
            this.Update(UpdateModes.RefreshLayout);
        }

        void filterDescriptors_CollectionChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            this.listSource.CollectionView.FilterExpression = this.filterDescriptors.Expression;
        }

        void CollectionView_CollectionChanged(object sender, Telerik.WinControls.Data.NotifyCollectionChangedEventArgs e)
        {
            this.SynchronizeVisualItems();
            this.Update(UpdateModes.RefreshLayout | UpdateModes.UpdateScroll);
        }

        protected override void OnStyleChanged(RadPropertyChangedEventArgs e)
        {
            base.OnStyleChanged(e);
            if (e.NewValue != null)
            {
                this.ViewElement.ViewElement.DisposeChildren();
                this.ViewElement.ViewElement.ElementProvider.ClearCache();
                this.ViewElement.ViewElement.InvalidateMeasure();
            }
        }

        #endregion

        #region Private Methods

        private void InitializeBoundColumns()
        {
            if (this.IsDesignMode)
            {
                return;
            }

            this.columns.BeginUpdate();
            this.columns.Clear();
            PropertyDescriptorCollection boundProperties = this.ListSource.BoundProperties;
            for (int i = 0; i < boundProperties.Count; i++)
            {
                if (boundProperties[i].PropertyType == typeof(IBindingList))
                {
                    continue;
                }

                ListViewDetailColumn column = new ListViewDetailColumn(boundProperties[i].Name, boundProperties[i].Name);

                column.FieldName = boundProperties[i].DisplayName;

                column.Owner = this;

                ListViewColumnCreatingEventArgs args = new ListViewColumnCreatingEventArgs(column);

                this.OnColumnCreating(args);

                this.columns.Add(args.Column);
            }

            this.columns.EndUpdate(false);
        }

        private void RebuildColumnAccessors()
        {
            for (int i = 0; i < this.columns.Count; i++)
            {
                ListViewDetailColumn column = this.columns[i];

                column.Accessor = (this.IsDataBound) ?
                                  new ListViewBoundAccessor(column) :
                                  new ListViewAccessor(column);
            }
        }

        #endregion

        #region Input

        internal virtual bool ProcessMouseUp(MouseEventArgs e)
        {
            return this.viewElement.ProcessMouseUp(e);
        }

        internal virtual bool ProcessMouseMove(MouseEventArgs e)
        {
            return this.viewElement.ProcessMouseMove(e);
        }

        internal virtual bool ProcessMouseDown(MouseEventArgs e)
        {
            if (this.viewElement.ProcessMouseDown(e))
            {
                return true;
            }

            return false;
        }

        internal virtual bool ProcessKeyDown(KeyEventArgs e)
        {
            if (this.viewElement.ProcessKeyDown(e))
            {
                return true;
            }

            return false;
        }

        internal virtual bool ProcessMouseWheel(MouseEventArgs e)
        {
            if (this.viewElement.ProcessMouseWheel(e))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Scrolls the view with a given amount.
        /// </summary>
        /// <param name="delta">The amount to scroll the view with.</param>
        public void ScrollTo(int delta)
        {
            this.ViewElement.ScrollTo(delta);
        }

        #endregion
    }
}