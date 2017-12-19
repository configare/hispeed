using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Telerik.WinControls.UI.Data;
using System.ComponentModel;
using Telerik.WinControls.Enumerations;
using System.Collections;
using System.Globalization;

namespace Telerik.WinControls.UI
{
    #region Delegates
    public delegate void ListItemDataBindingEventHandler(object sender, ListItemDataBindingEventArgs args);
    public delegate void ListItemDataBoundEventHandler(object sender, ListItemDataBoundEventArgs args);
    public delegate void CreatingVisualListItemEventHandler(object sender, CreatingVisualListItemEventArgs args);
    public delegate void SortStyleChangedEventHandler(object sender, SortStyleChangedEventArgs args);
    public delegate void VisualListItemFormattingEventHandler(object sender, VisualItemFormattingEventArgs args);

    #endregion

    #region EventArgs Classes

    /// <summary>
    /// Contains the visual list item which is to be formatted in the VisualItemFormatting event of RadListControl.
    /// </summary>
    public class VisualItemFormattingEventArgs
    {
        private RadListVisualItem item = null;

        public VisualItemFormattingEventArgs(RadListVisualItem item)
        {
            Debug.Assert(item != null, "Provided item can not be null.");
            this.item = item;
        }

        /// <summary>
        /// Gets the visual list item which is to be formatted.
        /// </summary>
        public RadListVisualItem VisualItem
        {
            get
            {
                return this.item;
            }
        }
    }

    /// <summary>
    /// Allows setting custom instances of the visual list items in RadListControl.
    /// </summary>
    public class CreatingVisualListItemEventArgs : EventArgs
    {
        private RadListVisualItem listItemElement = null;

        /// <summary>
        /// Gets or sets the custom visual list item that will be used as visual representation
        /// of the data items.
        /// </summary>
        public RadListVisualItem VisualItem
        {
            get
            {
                return this.listItemElement;
            }

            set
            {
                this.listItemElement = value;
            }
        }
    }

    /// <summary>
    /// Allows setting custom instances of the data items in RadListControl.
    /// </summary>
    public class ListItemDataBindingEventArgs : EventArgs
    {
        private RadListDataItem newItem = null;

        public ListItemDataBindingEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets a data item that will be used to store logical information
        /// to represent data records.
        /// </summary>
        public RadListDataItem NewItem
        {
            get
            {
                return this.newItem;
            }

            set
            {
                this.newItem = value;
            }
        }
    }

    /// <summary>
    /// Provides a data item that was just bound during RadListControls data binding.
    /// </summary>
    public class ListItemDataBoundEventArgs : EventArgs
    {
        private RadListDataItem newItem = null;

        public ListItemDataBoundEventArgs(RadListDataItem newItem)
        {
            this.newItem = newItem;
        }

        /// <summary>
        /// Gets the data item that was just associated with a data record.
        /// The data record can be accessed through the DataBoundItem property.
        /// </summary>
        public RadListDataItem NewItem
        {
            get
            {
                return this.newItem;
            }
        }
    }

    /// <summary>
    /// Provides the new sort style after the same property of RadListControl changes.
    /// </summary>
    public class SortStyleChangedEventArgs : EventArgs
    {
        private SortStyle style;

        public SortStyleChangedEventArgs(SortStyle sortStyle)
        {
            this.style = sortStyle;
        }

        /// <summary>
        /// Gets the new sort style value.
        /// </summary>
        public SortStyle SortStyle
        {
            get
            {
                return this.style;
            }
        }
    }

    #endregion

    #region Misc Types

    /// <summary>
    /// This interface is used in the FindString method of RadListControl.
    /// Users can assign their custom comparer to the StringComparer property of RadListControl.
    /// </summary>
    public interface IFindStringComparer
    {
        bool Compare(string x, string y);
    }

    /// <summary>
    /// This class is used to create the initial instance of the IFindStringComparer in RadListControl.
    /// It uses the string StartsWith method.
    /// </summary>
    public class DefaultFindStringComparer : IFindStringComparer
    {
        #region IStringSearchComparer Members

        public bool Compare(string x, string y)
        {
            return x.StartsWith(y, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }

    /// <summary>
    /// This class is used in the FindStringExact method. It searches for an item whose text is exactly equal to the provided string.
    /// </summary>
    public class ExactFindStringComparer : IFindStringComparer
    {
        #region IFindStringComparer Members

        public bool Compare(string x, string y)
        {
            return x.CompareTo(y) == 0;
        }

        #endregion
    }

    /// <summary>
    /// This enum is used in RadListControl.FindString() to determine whether an item is searched via the text property
    /// set by the user or the text provided by the data binding logic.
    /// </summary>
    public enum ItemTextComparisonMode
    {
        DataText,
        UserText
    }

    #endregion

    /// <summary>
    /// This class is used to represent data in a list similar to the ListBox control provided by Microsoft.
    /// </summary>
    public class RadListElement : VirtualizedScrollPanel<RadListDataItem, RadListVisualItem> 
    {
        #region RadProperties

        public static readonly RadProperty CaseSensitiveSortProperty = RadProperty.Register("CaseSensitiveSort", typeof(bool), typeof(RadListElement), new RadElementPropertyMetadata(true));

        private static readonly RadProperty CollapsibleGroupItemsOffsetProperty =
            RadProperty.Register("CollapsibleGroupItemsOffset", typeof(float), typeof(RadListElement),
            new RadElementPropertyMetadata(15, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange));

        private static readonly RadProperty NonCollapsibleGroupItemsOffsetProperty =
                    RadProperty.Register("NonCollapsibleGroupItemsOffset", typeof(float), typeof(RadListElement),
                    new RadElementPropertyMetadata(4, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange));

        #endregion

        #region Fields

        private DefaultListControlStackContainer viewElement = null; // The view element is a LightVisualElement and is responsible for the layout of the list control items.
        private int beginUpdateCount = 0;
        private int subscriptionCounter = 0; // Keeps the number of subscriptions to current position changed of the data layer. This must never be greater than 1.
        // Cached RadProperties
        private float cachedCollapsibleGroupItemsOffset = 15;
        private float cachedNonCollapsibleGroupItemsOffset = 4;

        // Fields related to selection.
        private SelectionMode selectionMode = SelectionMode.One;
        private int oldSelectedIndex = -1;
        private bool suspendSelectionEvents = false;  // Boolean flag that allows the user to stop the selection events from firing if needed.
        private bool suspendItemsChangeEvents = false;
        private RadListDataItem oldSelectedItem = null;

        // Fields related to the data layer.
        private ListDataLayer dataLayer;
        private SortDescriptorCollection sortDescriptors; 

        // Fields related to filtering
        private RadListDataItem preFilterItem = null; // The selected item right before filtering is applied. After the filtering if the item still exists it is selected again.
        private bool filtering = false; // This flag is set to true right before applying a filter and is reset on collection changed. Used with preFilterItem in order to restore the selection.

        //Fields related to grouping
        internal ListGroupFactory groupFactory;
        int suspendGroupRefresh = 0;
        private bool showGroups = false;

        // The active item is used by the scrolling logic so that it is always into view.
        // An item can be only in active state in MultiSimple or MultiExtended + CTRL selection mode.
        private RadListDataItem activeListItem = null;
        private RadListDataItemSelectedCollection selectedItems = null;

        // These two fields will be non-null only in data-bound mode or if the user explicitly sets the value property in unbound mode.
        private object newValue = null; // the new selected value
        private object oldValue = null; // the old selected value

        // Fields related to item text formatting
        private string formatString = "";
        private IFormatProvider formatInfo = CultureInfo.CurrentCulture;
        private bool formattingEnabled = true;

        // Fields related to the FindString methods.
        private IFindStringComparer findStringComparer = null;
        private ItemTextComparisonMode itemTextComparisonMode = ItemTextComparisonMode.UserText;

        // Fields related to keyboard search
        Timer searchTimer = null;
        StringBuilder searchBuffer = null;
        private bool keyboardSearchEnabled = true;
        private int searchStartIndex = 0;

        // These two fields are written when the binding context changes and before the new context
        // is assigned to the data layer. When the current position changes we check these two
        // fields whether they are the same and do nothing if they are because we do not want to
        // fire any unnecessary events.
        private int bindingContextPosition = -1;
        private object bindingContextDataSource = null;

        private int indexBeforeItemsChange = -1;
        private bool selectionInProgress = false;
        private int shiftSelectStartIndex = 0;

        private ListControlDragDropService dragDropService;
      
        #endregion

        #region Initialization

        static RadListElement()
        {
            new Themes.ControlDefault.ListControl().DeserializeTheme();
        }

        /// <summary>
        /// Creates a new instance of the RadListElement class.
        /// </summary>
        public RadListElement()
        {
            this.viewElement = (DefaultListControlStackContainer)this.ViewElement;

            this.Items = this.dataLayer.Items;
            this.NotifyParentOnMouseInput = true;

            this.ItemHeight = this.GetDefaultItemHeight();
            this.AllowDrag = true;
            this.AllowDrop = true;
            this.dragDropService = new ListControlDragDropService(this);
        }

        protected override void WireEvents()
        {
            base.WireEvents();
            this.WireCurrentPosition();

            this.dataLayer.DataView.CollectionChanged += new NotifyCollectionChangedEventHandler(DataView_CollectionChanged);
        }

        protected override void UnwireEvents()
        {
            base.UnwireEvents();
            this.UnwireCurrentPosition();

            this.dataLayer.DataView.CollectionChanged -= new NotifyCollectionChangedEventHandler(DataView_CollectionChanged);

            if (this.DataSource is Component)
            {
                (this.DataSource as Component).Disposed -= this.ListElementDataSource_Disposed;
            }
        }

        protected virtual IFindStringComparer CreateStringComparer()
        {
            return new DefaultFindStringComparer();
        }

        protected virtual int GetDefaultItemHeight()
        {
            return 18;
        }

        Point dragStart = Point.Empty;

        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    base.OnMouseDown(e);

        //    this.dragStart = e.Location;
        //    if (this.SelectedItems.Count > 0)
        //    {
        //        RadElement elementUnderMouse = this.ElementTree.GetElementAtPoint(e.Location);
        //        dragDropService.Start(elementUnderMouse);
        //    }
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);            

        //    if (this.Capture && e.Button == MouseButtons.Left &&
        //        this.ControlBoundingRectangle.Contains(e.Location) &&
        //        RadDragDropService.ShouldBeginDrag(e.Location, this.dragStart))
        //    {
               
        //    }
        //}

        /// <summary>
        /// Creates an instance of the data layer object responsibe for items management in bound or unbound mode.
        /// </summary>
        /// <returns></returns>
        protected virtual ListDataLayer CreateDataLayer()
        {
            return new ListDataLayer(this);
        }

        /// <summary>
        /// Creates an instance of the element provider object which is responsible for mapping logical and visual items and determining
        /// when a visual item must be updated to reflect the state of its corresponding logical item.
        /// </summary>
        /// <returns></returns>
        protected override IVirtualizedElementProvider<RadListDataItem> CreateElementProvider()
        {
            return new ListElementProvider(this);
        }

        /// <summary>
        /// Creates an instance of the visual element responsible for displaying the visual items in a particular layout.
        /// </summary>
        /// <returns></returns>
        protected override VirtualizedStackContainer<RadListDataItem> CreateViewElement()
        {
            return new DefaultListControlStackContainer();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.groupFactory = new ListGroupFactory(this);
            this.showGroups = false;
            

            this.DrawBorder = true;
           
            //Special case here - we need an entry in the property store so that we receive PropertyChanged for BindingContext.
            //TODO: We need a better solution here.
            if (this.BindingContext != null)
            {
            }

            this.selectedItems = new RadListDataItemSelectedCollection(this);
            this.dataLayer = this.CreateDataLayer();
            this.dataLayer.DataView.GroupFactory = this.groupFactory;
            this.dataLayer.DataView.CanGroup = false;
            this.dataLayer.DisplayMember = "";
            this.dataLayer.ValueMember = "";
            this.dataLayer.ChangeCurrentOnAdd = false;
            this.dataLayer.DataView.PropertyChanged += new PropertyChangedEventHandler(DataView_PropertyChanged);
            this.dataLayer.DataView.Comparer = new ListItemAscendingComparer();
            this.sortDescriptors = this.dataLayer.DataView.SortDescriptors;

            this.FindStringComparer = this.CreateStringComparer();

            this.searchTimer = new Timer();
            this.searchTimer.Interval = 300;
            this.searchTimer.Tick += this.SearchTimer_Tick;
            this.searchBuffer = new StringBuilder();
        }

        protected override void InitializeItemScroller(ItemScroller<RadListDataItem> scroller)
        {
            base.InitializeItemScroller(scroller);
            scroller.ScrollMode = ItemScrollerScrollModes.Smooth;
        }

        #endregion

        #region Properties

        #region Internal & private 

        internal ListDataLayer DataLayer
        {
            get
            {
                return this.dataLayer;
            }
        }

        internal ListGroupCollection Groups
        {
            get
            {
                return this.groupFactory.Groups;   
            }
        }

        internal bool ShowGroups
        {
            get
            {
                return showGroups;
            }
            set
            {
                if (showGroups != value)
                {
                    showGroups = value;
                    this.dataLayer.DataView.CanGroup = value;
                    this.UpdateItemTraverser();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the oldSelectedIndex is reset to initial state.
        /// The old selected index is in initial state only when the list control is newly
        /// constructed and has not yet had any selected items, or when the data layer sends
        /// a reset notification. This happens when the data source is changed.
        /// </summary>
        private bool IsOldSelectedIndexInInitialState
        {
            get
            {
                return this.oldSelectedIndex == -2;
            }
        }  

        #endregion

        #region Protected

        /// <summary>
        /// Gets a value indicating whether the SelectedValue property is different after the selection last changed.
        /// </summary>
        protected bool HasSelectedValueChanged
        {
            get
            {
                bool hasSelectedValueChanged = false;

                if (this.newValue != null && !this.newValue.Equals(this.oldValue))
                {
                    hasSelectedValueChanged = true;
                }

                if (!hasSelectedValueChanged && this.oldValue != null && !this.oldValue.Equals(this.newValue))
                {
                    hasSelectedValueChanged = true;
                }

                return hasSelectedValueChanged;
            }
        }

        #endregion

        #region Public


        /// <summary>
        /// Finds the first item in the RadList control that matches the specified string.
        /// </summary>
        /// <param name="text">The string to search for.</param>
        /// <returns>The zero-based index of the first item found; returns null if no match is found.</returns>
        public RadListDataItem FindItemExact(string text, bool caseSensitive)
        {
            foreach (RadListDataItem item in this.Items)
            {
                if (item.Text.Equals(text, caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the offset of the items when they are displayed in a collapsible group.
        /// </summary>
        internal float CollapsibleGroupItemsOffset
        {
            get
            {
                return cachedCollapsibleGroupItemsOffset;
            }
            set
            {
                if (value != cachedCollapsibleGroupItemsOffset)
                {
                    cachedCollapsibleGroupItemsOffset = value;
                    this.SetValue(RadListElement.CollapsibleGroupItemsOffsetProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset of the items when they are displayed in a non-collapsible group.
        /// </summary>
        internal float NonCollapsibleGroupItemsOffset
        {
            get
            {
                return cachedNonCollapsibleGroupItemsOffset;
            }
            set
            {
                if (value != cachedNonCollapsibleGroupItemsOffset)
                {
                    cachedNonCollapsibleGroupItemsOffset = value;
                    this.SetValue(RadListElement.NonCollapsibleGroupItemsOffsetProperty, value);
                }
            }
        }

        public virtual bool IsUpdating
        {
            get
            {
                return beginUpdateCount > 0;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this RadListElement will stop firing the ItemsChanging and ItemsChanged events.
        /// </summary>
        public bool SuspendItemsChangeEvents
        {
            get
            {
                return this.suspendItemsChangeEvents;
            }

            set
            {

                this.suspendItemsChangeEvents = value;
                this.OnNotifyPropertyChanged("SuspendItemsChangeEvents");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether text case will be taken into account when sorting.
        /// </summary>
        public bool CaseSensitiveSort
        {
            get
            {
                return (bool)this.GetValue(RadListElement.CaseSensitiveSortProperty);
            }

            set
            {
                this.SetValue(RadListElement.CaseSensitiveSortProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how long the user must wait before searching with the keyboard is reset.
        /// The default value of this property is 300 ms.
        /// </summary>
        public int KeyboardSearchResetInterval
        {
            get
            {
                return this.searchTimer.Interval;
            }

            set
            {
                if (this.searchTimer.Interval == value)
                {
                    return;
                }

                this.searchTimer.Interval = value;
                this.OnNotifyPropertyChanged("KeyboardSearchResetInterval");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the user can search for an item by typing characters when RadListElement is focused.
        /// </summary>
        public bool KeyboardSearchEnabled
        {
            get
            {
                return this.keyboardSearchEnabled;
            }

            set
            {
                if (this.keyboardSearchEnabled == value)
                {
                    return;
                }

                this.keyboardSearchEnabled = value;
                this.OnNotifyPropertyChanged("KeyboardSearchEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the FindString() method searches via the text property
        /// set by the user or by the text provided by the data binding logic, that is, by DisplayMember. 
        /// </summary>
        public ItemTextComparisonMode ItemTextComparisonMode
        {
            get
            {
                return this.itemTextComparisonMode;
            }

            set
            {
                if (this.itemTextComparisonMode == value)
                {
                    return;
                }

                this.itemTextComparisonMode = value;
                this.OnNotifyPropertyChanged("ItemTextComparisonMode");
            }
        }

        /// <summary>
        /// Gets or sets a Predicate that will be called for every data item in order to determine
        /// if the item will be visible.
        /// </summary>
        public Predicate<RadListDataItem> Filter
        {
            get
            {
                return this.dataLayer.DataView.Filter;
            }

            set
            {
                if (this.dataLayer.DataView.Filter == value)
                {
                    return;
                }

                this.preFilterItem = this.SelectedItem;
                this.filtering = true;
                this.dataLayer.DataView.Filter = value;
                this.OnNotifyPropertyChanged("Filter");

                this.EnsureSelectedIndexOnFilterChanged();
            }
        }

        private void EnsureSelectedIndexOnFilterChanged()
        {
            if (this.Items.Count > 0)
            {
                //by defaylt select first item
                this.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gets or sets a filter expression that determines which items will be visible.
        /// </summary>
        public string FilterExpression
        {
            get
            {
                return this.dataLayer.DataView.FilterExpression;
            }

            set
            {
                if (this.dataLayer.DataView.FilterExpression == value)
                {
                    return;
                }

                this.dataLayer.DataView.FilterExpression = value;
                this.OnNotifyPropertyChanged("FilterExpression");
            }
        }

        /// <summary>
        /// Gets or sets an object that implements IFindStringComparer.
        /// The value of this property is used in the FindString() method when searching for an item.
        /// </summary>
        public IFindStringComparer FindStringComparer
        {
            get
            {
                return this.findStringComparer;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("The StringSearchComparer can not be set to null.");
                }

                this.findStringComparer = value;
                this.OnNotifyPropertyChanged("FindStringComparer");
            }
        }

        /// <summary>
        /// Gets or sets an object that implements IComparer which is used when sorting items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IComparer<RadListDataItem> ItemsSortComparer
        {
            get
            {
                return this.dataLayer.DataView.Comparer;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("ItemsSortComparer can not be null.");
                }

                this.dataLayer.DataView.Comparer = value;
                this.OnNotifyPropertyChanged("ItemsSortComparer");
            }
        }

        /// <summary>
        /// Gets or sets the active item. This property is meaningful only when SelectionMode is MultiSimple or MultiExtended with the Control key pressed.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListDataItem ActiveItem
        {
            get
            {
                return this.activeListItem;
            }

            set
            {
                if (value != null && value.Owner != this)
                {
                    throw new ArgumentException("Provided item does not exist in the Items collection.");
                }

                this.UpdateActiveItem(value, true);
            }
        }

        /// <summary>
        /// Provides a readonly interface to the currently selected items.
        /// </summary>
        public IReadOnlyCollection<RadListDataItem> SelectedItems
        {
            get
            {
                return this.selectedItems;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to stop the selection events from firing. These are SelectedIndexChanged,
        /// SelectedIndexChanging and SelectedValueChanged.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SuspendSelectionEvents
        {
            get
            {
                return this.suspendSelectionEvents;
            }

            set
            {
                if (this.suspendSelectionEvents == value)
                {
                    return;
                }

                this.suspendSelectionEvents = value;
                this.OnNotifyPropertyChanged("SuspendSelectionEvents");
            }
        }

        /// <summary>
        /// Gets or sets the SelectionMode which determines selection behavior of RadListElement.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectionMode SelectionMode
        {
            get
            {
                return this.selectionMode;
            }

            set
            {
                this.SetSelectionMode(value);
            }
        }

        /// <summary>
        /// Gets or sets the object that is responsible for providing data objects for the RadListElement.
        /// Setting this property throws an InvalidOperationException if Items is not empty and the data source is null.
        /// </summary>
        public object DataSource
        {
            get
            {
                return this.dataLayer.DataSource;
            }
            set
            {
                if (this.DataSource == value)
                {
                    return;
                }

                if (this.DataSource is Component)
                {
                    (this.DataSource as Component).Disposed -= ListElementDataSource_Disposed;
                }

                // Reset display and value member when the data source is set to null.
                if (value == null)
                {
                    this.dataLayer.DisplayMember = "";
                    this.dataLayer.ValueMember = "";
                    this.DisposeItems();
                    this.dataLayer.ChangeCurrentOnAdd = false;
                }
                else
                {
                    this.CheckReadyForBinding();

                    if (value is Component)
                    {
                        (value as Component).Disposed += ListElementDataSource_Disposed;
                    }

                    this.dataLayer.ChangeCurrentOnAdd = true;
                }

                this.BeginUpdate();
                this.oldValue = this.SelectedValue;
                this.ViewElement.Children.Clear();
                this.ViewElement.ElementProvider.ClearCache();
                this.activeListItem = null;
                this.selectedItems.Clear();
                this.dataLayer.DataSource = value;                
                this.EndUpdate();
                
                this.OnNotifyPropertyChanged("DataSource");
                this.OnDataBindingComplete(this, new ListBindingCompleteEventArgs(ListChangedType.Reset));

            }
        }

        /// <summary>
        /// Gets or sets the position of the selection.
        /// Setting this property will cause the SelectedIndexChanging and SelectedIndexChanged events to fire.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                if (this.selectedItems.Count == 0)
                {
                    return -1;
                }

                // When items are 0 the data layer sometimes returns a wrong current position.
                if (this.Items.Count == 0)
                {
                    return -1;
                }

                return this.dataLayer.CurrentPosition;
            }
            set
            {
                this.SetSelectedIndex(value);
            }
        }

        /// <summary>
        /// Gets or sets the selected logical list item.
        /// Setting this property will cause the selection events to fire.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListDataItem SelectedItem
        {
            get
            {
                if (this.selectedItems.Count == 0)
                {
                    return null;
                }

                if (this.dataLayer.CurrentPosition == -1)
                {
                    return null;
                }

                // When items are 0 the data layer returns a wrong current position
                // and the CurrentItem property throws index out of range.
                if (this.Items.Count == 0)
                {
                    return null;
                }

                RadListDataItem item = null;

                //TODO: remove this try catch once the CurrentItem stops throwing exceptions after filtering.
                try
                {
                    item = this.dataLayer.CurrentItem;
                }
                catch(ArgumentOutOfRangeException)
                {
                    return null;
                }

                return item;
            }

            set
            {
                this.SetSelectedItem(value);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected value. Setting the SelectedValue to a value that is shared between many items causes the first item to be selected.
        /// This property triggers the selection events.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedValue
        {
            get
            {
                if (this.selectedItems.Count == 0)
                {
                    return null;
                }

                if (this.dataLayer.CurrentPosition == -1)
                {
                    return null;
                }

                // When items are 0 the data layer returns a wrong current position
                // and the CurrentItem property throws index out of range.
                if (this.Items.Count == 0)
                {
                    return null;
                }

                RadListDataItem selectedItem = this.dataLayer.CurrentItem;
                return selectedItem != null ? selectedItem.Value : null;
            }

            set
            {
                this.SetSelectedValue(value);
            }
        }

        /// <summary>
        /// Gets or sets a string which will be used to get a text string for each visual item. This property can not be set to null. Setting
        /// it to null will cause it to contain an empty string.
        /// </summary>
        public string DisplayMember
        {
            get
            {
                return dataLayer.DisplayMember;
            }
            set
            {
                if (this.dataLayer.DisplayMember == value)
                {
                    return;
                }

                dataLayer.DisplayMember = value + ""; // Add empty string in case a null value is provided. DisplayMember must never be null.

                viewElement.ForceVisualStateUpdate();

                this.OnNotifyPropertyChanged("DisplayMember");
            }
        }

        /// <summary>
        /// Gets or sets the string through which the SelectedValue property will be determined. This property can not be set to null.
        /// Setting it to null will cause it to contain an empty string.
        /// </summary>
        public string ValueMember
        {
            get
            {
                return dataLayer.ValueMember;
            }

            set
            {
                if (this.dataLayer.ValueMember == value)
                {
                    return;
                }

                dataLayer.ValueMember = value + ""; // Value member must also never be null.

                this.newValue = this.SelectedValue;
                this.OnSelectedValueChanged(this.SelectedIndex);
                this.OnNotifyPropertyChanged("ValueMember");

                if (this.DisplayMember == "") // If there is no display member, set it the same as ValueMember. This is the behavior of the Microsoft list box.
                {
                    this.DisplayMember = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the item height for the items. This property is disregarded when AutoSizeItems is set to true.
        /// </summary>
        public int ItemHeight
        {
            get
            {
                return this.Scroller.ItemHeight;
            }
            set
            {                
                SizeF defaultSize = this.ViewElement.ElementProvider.DefaultElementSize;
                if (defaultSize.Height != value)
                {
                    this.Scroller.ItemHeight = value;
                    this.ViewElement.ElementProvider.DefaultElementSize = new SizeF(defaultSize.Width, value);
                    this.Scroller.UpdateScrollRange();
                    this.ViewElement.UpdateItems();
                    this.OnNotifyPropertyChanged("ItemHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the sort style. It can be Ascending, Descending or None. Sorting is performed according to the property specified by DisplayMember.
        /// </summary>
        public SortStyle SortStyle
        {
            get
            {
                if (this.sortDescriptors.Count > 0)
                {
                    return this.GetSortStyle(this.sortDescriptors[0].Direction);
                }

                return SortStyle.None;
            }
            set
            {
                this.SetSortStyle(value);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether text formatting is enabled for the visual items.
        /// </summary>
        public bool FormattingEnabled
        {
            get
            {
                return this.formattingEnabled;
            }

            set
            {
                this.formattingEnabled = value;
                this.OnNotifyPropertyChanged("FormattingEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a format string that will be used for visual item formatting if FormattingEnabled is set to true.
        /// </summary>
        public string FormatString
        {
            get
            {
                return this.formatString;
            }

            set
            {
                if (this.formatString == value)
                {
                    return;
                }

                this.formatString = value;
                this.OnNotifyPropertyChanged("FormatString");
            }
        }

        /// <summary>
        /// Gets or sets an object that implements the IFormatProvider interface. This object is used when formatting items. The default object is
        /// CultureInfo.CurrentCulture.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFormatProvider FormatInfo
        {
            get
            {
                return this.formatInfo;
            }

            set
            {
                if (this.formatInfo == value)
                {
                    return;
                }

                this.formatInfo = value;
                this.OnNotifyPropertyChanged("FormatInfo");
            }
        }

        /// <summary>
        /// Gets or sets the scrolling mode.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemScrollerScrollModes ScrollMode
        {
            get
            {
                return this.Scroller.ScrollMode;
            }

            set
            {
                ItemScrollerScrollModes prevMode = this.Scroller.ScrollMode;

                this.Scroller.ScrollMode = value;

                if (value != prevMode)
                {
                    this.OnNotifyPropertyChanged("ScrollMode");
                }
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the is a filter currently set either with the Filter or FilterExpression properties.
        /// </summary>
        public bool IsFilterActive
        {
            get
            {
                return this.Filter != null || !string.IsNullOrEmpty(this.FilterExpression);
            }
        }

        #endregion

        #endregion

        #region Events

        #region DataBindingComplete

        /// <summary>
        /// Fires after data binding operation has finished.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        /// <seealso cref="DataSource"/>
        /// <seealso cref="DataMember"/>
        /// <seealso cref="ListBindingCompleteEventHandler"/>
        /// <seealso cref="OnDataBindingComplete(ListBindingCompleteEventArgs)"/>
        /// <seealso cref="DataError"/>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Fires after data binding operation has finished.")]
        public event ListBindingCompleteEventHandler DataBindingComplete;

        /// <summary>
        /// Raises the <see cref="DataBindingComplete" /> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">An <see cref="ListBindingCompleteEventArgs" /> instance that contains the event data.</param>
        /// <seealso cref="DataBindingComplete"/>
        /// <seealso cref="ListBindingCompleteEventArgs"/>
        protected virtual void OnDataBindingComplete(object sender, ListBindingCompleteEventArgs e)
        {
            if (DataBindingComplete != null)
            {
                DataBindingComplete(this, e);
            }
        }

        #endregion

        /// <summary>
        /// This event fires when the SelectedValue changes. This is will not always fire when the SelectedItem or SelectedIndex changes because the new item may have the same value.
        /// </summary>
        public event EventHandler SelectedValueChanged;

        /// <summary>
        /// This event fires when selected index changes. This always happens when the SelectedItem changes.
        /// </summary>
        public event PositionChangedEventHandler SelectedIndexChanged;

        /// <summary>
        /// This event fires before SelectedIndexChanged and provides a means for cancelling the whole selection operation.
        /// Someties this event will not fire since cancelling the change is not possible, for example when the DataSource is set to null.
        /// </summary>
        public event PositionChangingEventHandler SelectedIndexChanging;

        /// <summary>
        /// This item fires for data item that is being created during data binding and fires before the ItemDataBound event. The event provides a means for changing the instance of the data item
        /// to a custom data item.
        /// </summary>
        public event ListItemDataBindingEventHandler ItemDataBinding;

        /// <summary>
        /// This event fires after a data item has been created and bound.
        /// </summary>
        public event ListItemDataBoundEventHandler ItemDataBound;

        /// <summary>
        /// This event fires while creating visual items. This happens on during initial layout and during resizing if the new size is larger and thus allowing more items to be visualized.
        /// The event provides a means to create a custom visual item.
        /// </summary>
        public event CreatingVisualListItemEventHandler CreatingVisualItem;

        /// <summary>
        /// This event fires after the sorting style changes.
        /// </summary>
        public event SortStyleChangedEventHandler SortStyleChanged;

        /// <summary>
        /// The visual item formatting fires whenever the state of a visible logical item changes and when scrolling.
        /// </summary>
        public event VisualListItemFormattingEventHandler VisualItemFormatting;

        /// <summary>
        /// This event fires whenever an item is added, removed, set or if the whole items collection was modified.
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsChanged;

        /// <summary>
        /// This event fires right before adding, removing or setting an item. This event will not fire if an item is added to a data source directly
        /// because there is no way for RadListElement to be notified before the change.
        /// </summary>
        public event NotifyCollectionChangingEventHandler ItemsChanging;

        /// <summary>
        /// This event fires whenever a RadProperty of a data item changes. This event is most often used to listen changes in Selected and Active properties of the data items.
        /// </summary>
        public event RadPropertyChangedEventHandler DataItemPropertyChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Suspends notifications of changing groups.
        /// This method is cumulative, that is, if SuspendGroupRefresh is called N times, ResumeGroupRefresh must also be called N times.
        /// </summary>
        internal void SuspendGroupRefresh()
        {
            suspendGroupRefresh++;
        }


        /// <summary>
        /// Resumes refreshing of groups.
        /// </summary>
        /// <param name="performGroupRefresh">Indicates whether refreshing of groups should be performed.</param>
        internal void ResumeGroupRefresh(bool performGroupRefresh)
        {
            if (suspendGroupRefresh > 0)
            {
                suspendGroupRefresh--;
            }

            if (performGroupRefresh)
            {
                this.UpdateItemTraverser();
            }
        }

        /// <summary>
        /// Refreshes the groups.
        /// </summary>
        internal void RefreshGroups()
        {
            this.UpdateItemTraverser();
        }


        /// <summary>
        /// Scrolls to the active item if it is not null and if it is not fully visible.
        /// </summary>
        public void ScrollToActiveItem()
        {
            this.ScrollToItem(this.activeListItem);
        }

        /// <summary>
        /// Forces re-evaluation of the current data source (if any).
        /// </summary>
        public void Rebind()
        {
            this.dataLayer.ListSource.Reset();
        }

        /// <summary>
        /// Suspends internal notifications and processing in order to improve performance.
        /// This method is cumulative, that is, if BeginUpdate is called N times, EndUpdate must also be called N times.
        /// </summary>
        public void BeginUpdate()
        {
            this.beginUpdateCount++;
            this.dataLayer.ListSource.BeginUpdate();
        }

        /// <summary>
        /// Resumes the internal notifications and processing previously suspended by BeginUpdate.
        /// </summary>
        public void EndUpdate()
        {
            if (this.beginUpdateCount == 0)
            {
                return;
            }

            this.beginUpdateCount--;
            this.dataLayer.ListSource.EndUpdate();

            if (this.beginUpdateCount > 0)
            {
                return;
            }

            this.ViewElement.UpdateItems();
            this.Scroller.UpdateScrollRange();

            int currentPos = this.dataLayer.CurrentPosition;
            if (this.SelectedIndex != currentPos)
            {
                this.ProcessSelection(currentPos, false, InputType.Code);
            }

            if (this.oldSelectedIndex != currentPos)
            {
                this.OnSelectedIndexChanged(currentPos);
            }

            // We need to update the layout because UpdateItems only schedules one. We also need to invalidate this element
            // because UpdateItems removes the visual items from the element tree and nothing can invalidate their old bounds.
            this.UpdateLayout();
            this.Invalidate();
        }

        /// <summary>
        /// Defers the refresh.
        /// </summary>
        /// <returns></returns>
        public virtual IDisposable DeferRefresh()
        {
            this.BeginUpdate();
            return new DeferHelper(this);
        }

        private class DeferHelper : IDisposable
        {
            private RadListElement listElement;

            public DeferHelper(RadListElement listElement)
            {
                this.listElement = listElement;
            }

            public void Dispose()
            {
                if (this.listElement != null)
                {
                    this.listElement.EndUpdate();
                    this.listElement = null;
                }
            }
        }


        /// <summary>
        /// Selects all items if the SelectionMode allows it.
        /// This method throws an InvalidOperationException if SelectionMode is One or None.
        /// </summary>
        public void SelectAll()
        {
            if (this.selectionMode == SelectionMode.One || this.selectionMode == SelectionMode.None)
            {
                throw new InvalidOperationException("Selecting all items is not a valid operation in the current selection mode. SelectionMode = " + this.selectionMode.ToString() + ".");
            }

            this.SelectRange(0, this.Items.Count - 1);
        }

        /// <summary>
        /// Clears the currently selected items and selects all items in the closed range [startIndex, endIndex].
        /// </summary>
        /// <param name="startIndex">The first index at which to start selecting items.</param>
        /// <param name="endIndex">The index of one item past the last one to be selected.</param>
        public void SelectRange(int startIndex, int endIndex)
        {
            this.selectedItems.Clear();

            // If either argument is -1 we return and leave the selection cleared.
            if (startIndex == -1 || endIndex == -1)
            {
                return;
            }

            if (!this.IsIndexValid(startIndex))
            {
                throw new ArgumentException("Start index is out of range.");
            }

            if (!this.IsIndexValid(endIndex))
            {
                throw new ArgumentException("End index is out of range.");
            }

            this.UnwireCurrentPosition();

            this.HandleMultiSelectRange(startIndex, endIndex);

            this.WireCurrentPosition();
        }

        public void ScrollByPage(int pageCount)
        {
            if (pageCount == 0)
            {
                return;
            }

            RadScrollBarElement scrollBar = this.Scroller.Scrollbar;
            int value = pageCount * scrollBar.LargeChange + scrollBar.Value;
            this.ClampValue(scrollBar.Minimum, scrollBar.Maximum - scrollBar.LargeChange, ref value);
            scrollBar.Value = value;
        }

        /// <summary>
        /// Scrolls to the provided item so that the item will appear at the top of the view if it is before the currently visible items
        /// and at the bottom of the view if it is after the currently visible items.
        /// </summary>
        /// <param name="item">The item to scroll to.</param>
        public void ScrollToItem(RadListDataItem item)
        {
            if (item == null || ViewElement.Children.Count == 0)
            {
                return;
            }

            RadListVisualItem visualItem = item.VisualItem;
            bool discrete = ScrollMode == ItemScrollerScrollModes.Discrete;

            if (visualItem != null)
            {
                if (visualItem.ControlBoundingRectangle.Y < ViewElement.ControlBoundingRectangle.Y)
                {
                    int newValue = VScrollBar.Value - (discrete ? 1 : (ViewElement.ControlBoundingRectangle.Y - visualItem.ControlBoundingRectangle.Y));
                    ClampValue(VScrollBar.Minimum, VScrollBar.Maximum - VScrollBar.LargeChange + 1, ref newValue);
                    VScrollBar.Value = newValue;
                }
                else if (visualItem.ControlBoundingRectangle.Bottom > ViewElement.ControlBoundingRectangle.Bottom)
                {
                    int newValue = VScrollBar.Value + (discrete ? 1 : (visualItem.ControlBoundingRectangle.Bottom - ViewElement.ControlBoundingRectangle.Bottom));
                    ClampValue(VScrollBar.Minimum, VScrollBar.Maximum - VScrollBar.LargeChange + 1, ref newValue);
                    VScrollBar.Value = newValue;
                }
                else
                {
                    return;
                }
            }
            else
            {                
                int lastVisibleIndex = GetLastVisibleItemIndex();
                if (item.RowIndex == lastVisibleIndex + 1)
                {
                    RadListVisualItem lastItem = (RadListVisualItem)this.ViewElement.Children[this.ViewElement.Children.Count - 1];
                    int newValue = VScrollBar.Value;
                    if (discrete)
                    {
                        newValue++;
                    }
                    else
                    {
                        int delta = lastItem.ControlBoundingRectangle.Bottom - ViewElement.ControlBoundingRectangle.Bottom;
                        if (delta < 0)
                        {
                            return;
                        }
                        newValue += delta + Scroller.GetScrollHeight(item) + this.ItemSpacing;
                    }
                    ClampValue(VScrollBar.Minimum, VScrollBar.Maximum - VScrollBar.LargeChange + 1, ref newValue);
                    VScrollBar.Value = newValue;
                }
                else
                {
                    if (discrete)
                    { 
                        int firstVisibleIndex = GetFirstVisibleItemIndex();
                        if (item.RowIndex == firstVisibleIndex - 1)
                        {
                            int newValue = VScrollBar.Value - 1;
                            ClampValue(VScrollBar.Minimum, VScrollBar.Maximum - VScrollBar.LargeChange + 1, ref newValue);
                            VScrollBar.Value = newValue;
                            return;
                        }
                    }
                    Scroller.ScrollToItem(item);
                }
            }
        }

        /// <summary>
        /// Searches for an item related to the specified string. The relation is described by the object assigned to FindStringComparer property.
        /// By default this relation is the System.String.StartsWith().
        /// This method starts searching from the beginning of the items.
        /// </summary>
        /// <param name="s">The string with which every item will be compared.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindString(string s)
        {
            if (this.Items.Count == 0)
            {
                return -1;
            }

            return this.FindString(s, 0);
        }

        /// <summary>
        /// Searches for an item related to the specified string. The relation is described by the object assigned to FindStringComparer property.
        /// By default FindStringComparer uses the System.String.StartsWith() method.
        /// This method starts searching from the specified index. If the algorithm reaches the end of the Items collection it wraps to the beginning
        /// and continues until one before the provided index.
        /// </summary>
        /// <param name="s">The string with which every item will be compared.</param>
        /// <param name="startIndex">The index from which to start searching.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindString(string s, int startIndex)
        {
            if (!this.IsIndexValid(startIndex))
            {
                return -1;
            }

            int i = startIndex;
            do
            {
                i++;
                if (i == this.Items.Count)
                {
                    i = 0;
                }

                RadListDataItem item = this.Items[i];
                Debug.Assert(this.FindStringComparer != null, "The FindStringComparer can never be null.");
                if (this.FindStringComparer.Compare(this.GetItemText(item), s))
                {
                    return item.RowIndex;
                }

            } while (i != startIndex);

            return -1;
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public int FindStringExact(string s)
        {
            return this.FindStringExact(s, 0);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public int FindStringExact(string s, int startIndex)
        {
            IFindStringComparer prevComparer = this.FindStringComparer;
            this.FindStringComparer = new ExactFindStringComparer();

            int index = this.FindString(s, startIndex);

            this.FindStringComparer = prevComparer;

            return index;
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but does not start from the beginning when the end of the Items collection
        /// is reached.
        /// </summary>
        /// <param name="s">The string that will be used to search for an item.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindStringNonWrapping(string s)
        {
            return this.FindStringNonWrapping(s, 0);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but does not start from the beginning when the end of the Items collection
        /// is reached.
        /// </summary>
        /// <param name="s">The string that will be used to search for an item.</param>
        /// <param name="startIndex">The index from which to start searching.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindStringNonWrapping(string s, int startIndex)
        {
            for (int i = startIndex; i < this.Items.Count; ++i)
            {
                RadListDataItem item = this.Items[i];
                if (this.FindStringComparer.Compare(this.GetItemText(item), s))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a dictionary of RadListDataGroupItems with keys - their corresponding group.
        /// </summary>
        /// <returns>The dictionary.</returns>
        internal Dictionary<Group<RadListDataItem>, RadListDataGroupItem> CreateGroupHeaderItems()
        {
            Dictionary<Group<RadListDataItem>, RadListDataGroupItem> headerItems = new Dictionary<Group<RadListDataItem>, RadListDataGroupItem>();
            foreach (ListGroup group in this.dataLayer.DataView.Groups)
            {
                headerItems.Add(group, this.dataLayer.NewHeaderItem(group));
            }

            return headerItems;
        }

        /// <summary>
        /// Creates a new item traverser and updates the current.
        /// If group refresh is suspended this method has no effect.
        /// </summary>
        protected internal void UpdateItemTraverser()
        {
            if (suspendGroupRefresh == 0)
            { 
                this.dataLayer.DataView.LazyRefresh();
                this.Scroller.Traverser = this.CreateItemTraverser(this.Items);
            }
        }
         
        // The minor scroll offset is the offset that should be added to the major offset if the last/first visible item is partially visible.
        // This is required in order to avoid scrolling incorrectly and not seeing the whole item that we need to scroll to.
        protected int GetMinorScrollOffset(int direction)
        {
            Rectangle client = Rectangle.Round(this.GetClientRectangle(this.PreviousConstraint));
            if (direction == 1)
            {
                int lastItemIndex = this.viewElement.Children.Count;
                int lastItemBottom = this.viewElement.Children[lastItemIndex - 1].BoundingRectangle.Bottom;
                return lastItemBottom - client.Height;
            }
            else if (direction == -1)
            {

                int lastItemTop = this.viewElement.Children[0].BoundingRectangle.Top;
                return client.Top - lastItemTop;
            }

            return 0;
        }

        /// <summary>
        /// This method returns true if the ActiveItem is fully visible.
        /// </summary>
        protected bool ItemFullyVisible(RadListDataItem item)
        {
            if (item == null)
            {
                return false;
            }

            RadListVisualItem visualItem = item.VisualItem;
            return visualItem != null && !IsItemPartiallyVisible(visualItem);
        }

        /// <summary>
        /// Gets the index of the last visible item.
        /// </summary>
        protected int GetLastVisibleItemIndex()
        {
            if (this.viewElement.Children.Count == 0)
            {
                return -1;
            }

            return ((RadListVisualItem)this.viewElement.Children[this.viewElement.Children.Count - 1]).Data.RowIndex;
        }

        /// <summary>
        /// Gets the index of the first visible item.
        /// </summary>
        protected int GetFirstVisibleItemIndex()
        {
            if (this.viewElement.Children.Count == 0)
            {
                return -1;
            }

            return ((RadListVisualItem)this.viewElement.Children[0]).Data.RowIndex;
        }

        /// <summary>
        /// Gets the index of the middle visible item.
        /// </summary>
        protected int GetMiddleVisibleItemIndex()
        {
            RadElementCollection children = this.viewElement.Children;
            if (children.Count == 0)
            {
                return -1;
            }

            return ((RadListVisualItem)children[children.Count / 2]).Data.RowIndex;
        }

        /// <summary>
        /// Determines if the provided visual item intersects the view but is not contained in it.
        /// </summary>
        protected bool IsItemPartiallyVisible(RadListVisualItem item)
        {
            Rectangle viewBounds = this.viewElement.Bounds;

            bool itemContained = viewBounds.Contains(item.BoundingRectangle);

            // The intersect method modifies the rectangle on which it is called, so after the call below
            // viewBounds will actually contain the intersection.
            viewBounds.Intersect(item.BoundingRectangle);

            if (item.ControlBoundingRectangle.Bottom > viewElement.ControlBoundingRectangle.Bottom)
            {
                return true;
            }        

            return !itemContained && !viewBounds.IsEmpty;
        }

        #endregion

        #region Event Handlers

        protected void DataView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Comparer")
            {
                this.dataLayer.DataView.GroupPredicate = DataViewGroupPredicate;
            }
        }

        protected object DataViewGroupPredicate(RadListDataItem item, int level)
        {
            return (item.Group != null) ? item.Group.Key : 0;
        }

        /// <summary>
        /// If the object assigned to the DataSource property is of type Component, this callback will be invoked if
        /// the data source is disposed which cause all data items to become disposed.
        /// </summary>
        private void ListElementDataSource_Disposed(object sender, EventArgs e)
        {
            this.DisposeItems();
        }

        /// <summary>
        /// Handles changes in the data layer.
        /// Nothing will done if we the RadListElement is in a BeginUpdate state.
        /// </summary>
        private void DataView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (this.beginUpdateCount > 0)
            {
                return;
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.HandleItemsAdded(args);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.HandleItemsRemoved(args);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.HandleItemsReset(args);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.HandleItemsReplaced(args);
                    break;
            }

            if (this.activeListItem != null && !this.IsIndexValid(this.activeListItem.RowIndex))
            {
                this.UpdateActiveItem(this.activeListItem, false);
            }

            this.Scroller.UpdateScrollRange();
            this.ViewElement.UpdateItems();
            
            this.PerformLayout();
            this.Invalidate();

            this.ScrollToActiveItem();
        }

        protected virtual void HandleItemsReplaced(NotifyCollectionChangedEventArgs args)
        {
            for(int i = 0; i < args.NewItems.Count; ++i)
            {
                RadListDataItem current = (RadListDataItem)args.NewItems[i];
                current.Owner = this;
                current.DataLayer = this.dataLayer;
                if (((RadListDataItem)args.OldItems[i]).Active)
                {
                    current.Active = true;
                }
            }
        }

        protected virtual void HandleItemsReset(NotifyCollectionChangedEventArgs args)
        {
            this.activeListItem = null;
            this.selectedItems  = new RadListDataItemSelectedCollection(this);
            this.ViewElement.Children.Clear();
            this.ViewElement.ElementProvider.ClearCache();
            this.ViewElement.UpdateItems();
            UpdateFitToSizeMode();
            Scroller.UpdateScrollRange();

           //// this.UpdateScrollbar(this.HScrollBar);
           // if (!this.filtering)
           // {
           //     this.oldSelectedIndex = -2;
           // }

           // if (this.dataLayer.DataView.Count == 0)
           // {
           //     this.UpdateActiveItem(null, false);
           //     this.selectedItems.Clear();
           //     this.UpdateSelectedIndexOnItemsChanged();
           // }
           // else
           // {
           //     //TODO: Refactor this else clause once the data layer stops sending wrong current position after filtering.
           //     if (preFilterItem != null)
           //     {
           //         if (this.IsIndexValid(this.GetIndex(preFilterItem)))
           //         {
           //             this.SelectedItem = preFilterItem;
           //             preFilterItem = null;
           //             this.filtering = false;
           //         }
           //         else
           //         {
           //             this.selectedItems.Clear();
           //             this.UpdateActiveItem(this.activeListItem, false);
           //             this.OnSelectedIndexChanged(-1);
           //         }
           //     }
           //     else
           //     {                  
           //         this.ProcessSelection(this.dataLayer.CurrentPosition, false, InputType.Code);                    
           //     }
           // }

           // UpdateFitToSizeMode();
           // Scroller.UpdateScrollRange();
        }

        protected virtual void OnSelectedItemAdded(RadListDataItem newItem)
        {
            switch(this.SelectionMode)
            {
                case SelectionMode.None:
                    newItem.Selected = false;
                    newItem.Active = false;
                    break;
                case SelectionMode.One:
                    if(this.SelectedIndex == -1 || newItem.Selected)
                    {
                        this.SelectedItem = newItem;
                    }
                    //else
                    //{
                    //    newItem.Selected = false;
                    //    newItem.Active = false;
                    //}
                    break;
                default:
                    if(this.SelectedIndex == -1)
                    {
                        this.SelectedItem = newItem;
                    }
                    else
                    {
                        this.selectedItems.Add(newItem);
                        newItem.Active = false;
                    }
                    break;
            }
        }

        protected virtual void OnActiveItemAdded(RadListDataItem newItem)
        {
            if (this.ActiveItem != null)
            {
                newItem.Active = false;
            }
            else
            {
                switch(this.SelectionMode)
                {
                    case SelectionMode.None:
                    newItem.Active = false;
                        break;
                    case SelectionMode.MultiSimple:
                        this.ActiveItem = newItem;
                        break;
                    default:
                        this.OnSelectedItemAdded(newItem);
                        break;
                }
            }
        }

        protected virtual void HandleItemsAdded(NotifyCollectionChangedEventArgs args)
        {
            foreach (RadListDataItem current in args.NewItems)
            {
                if (current.Selected)
                {
                    this.OnSelectedItemAdded(current);
                }
                else if (current.Active)
                {
                    this.OnActiveItemAdded(current);
                }
            }
        }

        internal void UpdateSelectedIndexOnItemsChanged()
        {
            int newIndex = this.GetIndex(this.ActiveItem);
            if (/*!this.IsOldSelectedIndexInInitialState &&*/ newIndex != this.oldSelectedIndex)
            {
                this.SuspendSelectionEvents = true;
                this.SelectedItem = this.ActiveItem;
                this.SuspendSelectionEvents = false;
                this.OnSelectedIndexChanged(newIndex);
            }
        }

        protected virtual void HandleItemsRemoved(NotifyCollectionChangedEventArgs args)
        {
            if (args.Action != NotifyCollectionChangedAction.Remove)
            {
                return;
            }

            foreach (RadListDataItem toRemove in args.NewItems)
            {
                if (toRemove == this.activeListItem)
                {
                    this.UpdateActiveItem(toRemove, false);
                }

                if (!toRemove.Selected)
                {
                    continue;
                }

                toRemove.Owner = null;
                this.selectedItems.Remove(toRemove);

                int index = -1;
                if (this.selectedItems.Count == 0 && (this.selectionMode == SelectionMode.One || this.selectionMode == SelectionMode.MultiExtended))
                {
                    index = this.oldSelectedIndex;
                }

                if (this.IsIndexValid(index))
                {
                    this.ProcessSelection(index, false, InputType.Mouse);
                    return;
                }
            }
        }

        /// <summary>
        /// When the data layer changes the current position, this callback triggers the selection logic with the new position.
        /// </summary>
        private void dataLayer_CurrentPositionChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (e.Position == this.oldSelectedIndex)
            {
                return;
            }

            // When the binding context changes we need to know on the next current position changed
            // whether the data source and the position have changed. If nothing is different
            // we ignore the current change caused by the new binding context. And return the position of the data layer
            // to the previous position.
            if (this.bindingContextPosition != -1 && this.bindingContextDataSource == this.DataSource)
            {
                this.UnwireCurrentPosition();
                this.dataLayer.CurrentPosition = this.bindingContextPosition;
                this.bindingContextPosition = -1;
                this.bindingContextDataSource = null;
                this.WireCurrentPosition();
                return;
            }

            this.ProcessSelection(e.Position, false, InputType.Code);
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            this.searchTimer.Stop();
        }

        #endregion

        #region Events Management

        /// <summary>
        /// Fires the SelectedIndexChanged event.
        /// </summary>
        protected internal virtual void OnSelectedIndexChanged(int newIndex)
        {
            if (this.SuspendSelectionEvents)// || this.selectedItems.Inserting)
            {
                return;
            }

            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, new PositionChangedEventArgs(newIndex));
            }

            this.newValue = this.SelectedValue;
            this.oldSelectedIndex = newIndex;
            this.OnNotifyPropertyChanged("SelectedValue"); 
            OnSelectedValueChanged(newIndex);
        }

        /// <summary>
        /// Fires the SelectedIndexChanging event.
        /// </summary>
        protected virtual bool OnSelectedIndexChanging(int newIndex)
        {
            if (this.SuspendSelectionEvents)
            {
                return false;
            }
            
            this.oldValue = this.GetValueByIndex(this.oldSelectedIndex);

            if (newIndex != this.oldSelectedIndex && this.SelectedIndexChanging != null)
            {
                PositionChangingCancelEventArgs args = new PositionChangingCancelEventArgs(newIndex);
                this.SelectedIndexChanging(this, args);
                return args.Cancel;
            }

            return false;
        }

        /// <summary>
        /// Fires the SelectedValueChanged event if SelectedValue has actually changed since many items can have the same value.
        /// </summary>
        protected virtual void OnSelectedValueChanged(int newIndex)
        {
            if (this.SuspendSelectionEvents)
            {
                return;
            }

            if (!this.HasSelectedValueChanged)
            {
                return;
            }

            if (this.SelectedValueChanged != null)
            {
                this.SelectedValueChanged(this, new ValueChangedEventArgs(newIndex, this.newValue, this.oldValue));
            }
        }

        /// <summary>
        /// Fires the ItemDataBinding event.
        /// </summary>
        protected internal virtual RadListDataItem OnListItemDataBinding()
        {
            if (this.ItemDataBinding != null)
            {
                ListItemDataBindingEventArgs args = new ListItemDataBindingEventArgs();
                this.ItemDataBinding(this, args);

                return args.NewItem;
            }

            return null;
        }

        /// <summary>
        /// Fires the ItemDataBound event.
        /// </summary>
        protected internal virtual void OnListItemDataBound(RadListDataItem newItem)
        {
            if (this.ItemDataBound != null)
            {
                this.ItemDataBound(this, new ListItemDataBoundEventArgs(newItem));
            }
        }

        /// <summary>
        /// Fires the CreatingVisualItem event.
        /// </summary>
        protected internal virtual RadListVisualItem OnCreatingVisualListItem()
        {
            if (this.CreatingVisualItem != null)
            {
                CreatingVisualListItemEventArgs args = new CreatingVisualListItemEventArgs();
                this.CreatingVisualItem(this, args);
                return args.VisualItem;
            }

            return null;
        }

        /// <summary>
        /// Fires the SortStyleChanged event.
        /// </summary>
        protected virtual void OnSortStyleChanged(SortStyle sortStyle)
        {
            if (this.SortStyleChanged != null)
            {
                this.SortStyleChanged(this, new SortStyleChangedEventArgs(sortStyle));
            }
        }

        /// <summary>
        /// Fires the VisualItemFormattingeEvent with the provided visual item.
        /// </summary>
        protected internal virtual void OnVisualItemFormatting(RadListVisualItem item)
        {
            if (this.VisualItemFormatting != null)
            {
                this.VisualItemFormatting(this, new VisualItemFormattingEventArgs(item));
            }
        }

        /// <summary>
        /// Performs scrolling logic depending on the delta from the mouse wheel.
        /// </summary>
        protected internal virtual void OnMouseWheel(int delta)
        {
            RadScrollBarElement scrollbar = this.Scroller.Scrollbar;
            int step = Math.Max(1, delta / SystemInformation.MouseWheelScrollDelta);
            int elementDelta = Math.Sign(delta) * step * SystemInformation.MouseWheelScrollLines;
            int newValue = scrollbar.Value - elementDelta * scrollbar.SmallChange;
            int max = scrollbar.Maximum - scrollbar.LargeChange + 1;
            int min = scrollbar.Minimum;

            this.ClampValue(min, max, ref newValue);

            this.Scroller.Scrollbar.Value = newValue;
        }

        /// <summary>
        /// Raises the ItemsChanged event with the provided arguments.
        /// </summary>
        /// <param name="args">The arguments that contain the data relevant to the items change.</param>
        protected internal virtual void OnItemsChanged(NotifyCollectionChangedEventArgs args)
        {
            object oldPosition = this.Scroller.Traverser.Position;
            this.UpdateItemTraverser();
            this.Scroller.Traverser.Position = oldPosition;

            if (this.beginUpdateCount > 0 || this.SuspendItemsChangeEvents)
            {
                return;
            }

            int newIndex = this.GetIndex(this.oldSelectedItem);
            if (this.indexBeforeItemsChange != newIndex)
            {
                if (this.IsIndexValid(newIndex))
                {
                    this.UnwireCurrentPosition();
                    this.dataLayer.CurrentPosition = newIndex;
                    this.WireCurrentPosition();
                    this.OnSelectedIndexChanged(newIndex);
                }
            }

            if (this.ItemsChanged != null)
            {
                this.ItemsChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the ItemsChanging event with the provided arguments.
        /// </summary>
        /// <param name="args">The arguments that contain the data relevant to the pending items change.</param>
        protected internal virtual void OnItemsChanging(NotifyCollectionChangingEventArgs args)
        {
            if (this.beginUpdateCount > 0 || this.SuspendItemsChangeEvents)
            {
                return;
            }

            this.oldSelectedItem = this.SelectedItem;
            this.indexBeforeItemsChange = this.GetIndex(this.SelectedItem);

            if (this.ItemsChanging != null)
            {
                this.ItemsChanging(this, args);
            }
        }

        private void DataItemSelectedPropertyChanged(RadListDataItem item, bool newVal)
        {
            if (this.selectionInProgress)
            {
                return;
            }

            this.selectionInProgress = true;

            switch (this.SelectionMode)
            {
                case SelectionMode.None:
                    if (newVal)
                    {
                        throw new InvalidOperationException("An item can not be selected when SelectionMode is None.");
                    }
                    break;

                case SelectionMode.One:
                    if (item != this.SelectedItem && newVal)
                    {
                        this.HandleSelectOne(this.GetIndex(item));
                    }
                    break;

                default:
                    if (newVal)
                    {
                        if (!this.selectedItems.Contains(item))
                        {
                            this.selectedItems.Add(item);
                        }
                    }
                    else
                    {
                        this.MultiSimpleRemove(this.GetIndex(item), true, item);
                    }
                    break;
            }

            this.selectionInProgress = false;
        }

        /// <summary>
        /// Raises the DataItemPropertyChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected internal virtual void OnDataItemPropertyChanged(object sender, RadPropertyChangedEventArgs args)
        {
            if(args.Property == RadListDataItem.SelectedProperty)
            {
                RadListDataItem item = (RadListDataItem)sender;
                bool newVal = (bool)args.NewValue;

                this.DataItemSelectedPropertyChanged(item, newVal);
            }

            if (args.Property == RadListDataItem.ActiveProperty && (bool)args.NewValue)
            {
                this.UpdateActiveItem((RadListDataItem)sender, (bool)args.NewValue);
            }

            if (this.DataItemPropertyChanged != null)
            {
                this.DataItemPropertyChanged(sender, args);
            }
        }

        #endregion

        #region Overrides

        protected override ITraverser<RadListDataItem> CreateItemTraverser(IList<RadListDataItem> items)
        {
            if (this.ShowGroups)
            {
                return new ListGroupedItemsTraverser(this.dataLayer.DataView.Groups, this.CreateGroupHeaderItems());
            }
            else
            {
                return base.CreateItemTraverser(items);
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadObject.BindingContextProperty)
            {
                this.bindingContextDataSource = this.DataSource;
                this.bindingContextPosition = this.dataLayer.CurrentPosition;
                this.dataLayer.BindingContext = (BindingContext)e.NewValue;
            }

            if (e.Property == RadListElement.CaseSensitiveSortProperty)
            {
                if (this.SortStyle != SortStyle.None)
                {
                    this.dataLayer.Refresh();
                }
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.dataLayer.BindingContext = this.ElementTree.Control.BindingContext;
            this.ViewElement.UpdateItems();
            this.ViewElement.PerformLayout();
            this.Scroller.UpdateScrollRange();
            this.HScrollBar.Maximum = this.Scroller.MaxItemWidth;
        }

        

        protected override void DisposeManagedResources()
        {
            this.searchTimer.Stop();
            this.searchTimer.Tick -= this.SearchTimer_Tick;
            this.searchTimer.Dispose();
            this.DisposeItems();
            base.DisposeManagedResources();
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);

            this.HandleKeyboard(sender, args);
            this.HandleMouse(sender, args);
        }

        protected override SizeF GetItemDesiredSize(RadListDataItem item)
        {
            if (item.MeasuredSize != SizeF.Empty)
            {
                return item.MeasuredSize;
            }
            return base.GetItemDesiredSize(item);
        }

        protected override void OnBoundsChanged(RadPropertyChangedEventArgs e)
        {
            base.OnBoundsChanged(e);
            if (this.AutoSizeItems)
            {
                this.ViewElement.UpdateItems();
                this.ViewElement.PerformLayout();
                this.Scroller.UpdateScrollRange();
                this.HScrollBar.Maximum = this.Scroller.MaxItemWidth;

                //this.Scroller.UpdateScrollRange();
                //this.ViewElement.UpdateItems();
            }
        }

        #endregion

        #region Selection

        //#region SelectionTree

        //private void ProcessSelection(RadTreeNode node, Keys modifierKeys)
        //{
        //    if (node == null || this.updateSelectionChanged > 0)
        //    {
        //        return;
        //    }

        //    this.updateSelectionChanged++;
        //    this.BeginUpdate();

        //    bool isSelectedNode = node.Selected;
        //    bool isShiftPressed = (modifierKeys & Keys.Shift) == Keys.Shift;
        //    bool isControlPressed = (modifierKeys & Keys.Control) == Keys.Control;
        //    bool clearSelection = this.MultiSelect && (isShiftPressed || !isControlPressed);

        //    if (!node.Current)
        //    {
        //        if (!this.ProcessCurrentNode(node, clearSelection))
        //        {
        //            this.EndUpdate(false, UpdateActions.StateChanged);
        //            this.updateSelectionChanged--;
        //            return;
        //        }
        //    }
        //    else if (clearSelection)
        //    {
        //        this.ClearSelection();
        //        node.Selected = true;
        //        anchorPosition = node;
        //        anchorIndex = -1;
        //    }

        //    if (this.MultiSelect)
        //    {
        //        if (!isShiftPressed)
        //        {
        //            this.anchorPosition = node;
        //            this.anchorIndex = -1;

        //            if (isControlPressed && isSelectedNode)
        //            {
        //                node.Selected = !node.Selected;
        //            }
        //        }
        //        else
        //        {
        //            if (this.anchorIndex == -1)
        //            {
        //                this.anchorIndex = this.GetNodeIndex(this.anchorPosition);
        //            }

        //            bool forward = anchorIndex < GetNodeIndex(node);
        //            RadTreeNode temp = anchorPosition;

        //            while (temp != node)
        //            {
        //                temp.Selected = true;
        //                temp = forward ? temp.NextVisibleNode : temp.PrevVisibleNode;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        this.anchorPosition = node;
        //        this.anchorIndex = -1;
        //    }

        //    this.EndUpdate(true, UpdateActions.StateChanged);
        //    this.updateSelectionChanged--;
        //}

        //internal bool ProcessCurrentNode(RadTreeNode node, bool clearSelection)
        //{
        //    if (this.updateCurrentNodeChanged > 0)
        //    {
        //        return true;
        //    }

        //    this.updateCurrentNodeChanged++;
        //    RadTreeViewCancelEventArgs args = new RadTreeViewCancelEventArgs(node);
        //    this.OnSelectedNodeChanging(args);

        //    if (args.Cancel)
        //    {
        //        this.updateCurrentNodeChanged--;
        //        return false;
        //    }

        //    if (clearSelection)
        //    {
        //        this.ClearSelection();
        //    }

        //    if (this.selected != null)
        //    {
        //        this.selected.Current = false;

        //        if (!this.MultiSelect)
        //        {
        //            this.selected.Selected = false;
        //        }
        //    }

        //    this.selected = node;

        //    if (this.updateSelectionChanged == 0)
        //    {
        //        this.anchorPosition = this.selected;
        //        this.anchorIndex = -1;
        //    }

        //    if (this.TreeNodeProvider != null)
        //    {
        //        this.TreeNodeProvider.SetCurrent(this.selected);
        //    }

        //    if (this.selected != null)
        //    {
        //        this.selected.Current = true;
        //        this.selected.Selected = true;
        //        this.OnNotifyPropertyChanged("SelectedNode");
        //    }

        //    this.OnSelectedNodeChanged(new RadTreeViewEventArgs(node));
        //    this.updateCurrentNodeChanged--;
        //    this.Update(UpdateActions.StateChanged);
        //    return true;
        //}

        //#endregion

        #region Mouse

        /// <summary>
        /// Handles the mouse input by finding the RadElement involved with the mouse and sending the element and event information to the appropriate
        /// subsystem of RadListElement.
        /// </summary>
        private void HandleMouse(object sender, RoutedEventArgs args)
        {
            RoutedEvent routed = args.RoutedEvent;
            if (routed == RadElement.MouseUpEvent || routed == RadElement.MouseDownEvent || routed == RadElement.MouseClickedEvent)
            {
                MouseEventArgs mouseArgs = args.OriginalEventArgs as MouseEventArgs;
                if (mouseArgs == null || mouseArgs.Button != MouseButtons.Left)
                {
                    return;
                }

                RadListVisualItem clickedListVisualItem = this.FindParentListVisualItem(this.ElementTree.GetElementAtPoint(mouseArgs.Location));

                if (clickedListVisualItem != null)
                {
                    this.HandleMouseCore(routed, clickedListVisualItem);
                }
            }
        }

        protected RadListVisualItem FindParentListVisualItem(RadElement child)
        {
            while (child != null)
            {
                RadListVisualItem listVisualItem = child as RadListVisualItem;
                if (listVisualItem != null)
                {
                    return listVisualItem;
                }

                child = child.Parent;
            }

            return null;
        }

        /// <summary>
        /// Performs logical branching depending on the type of the routed event.
        /// </summary>
        private void HandleMouseCore(RoutedEvent routed, RadListVisualItem clickedListVisualItem)
        {
            MouseNotification notification = MouseNotification.Click;
            if (routed == RadElement.MouseUpEvent)
            {
                notification = MouseNotification.MouseUp;
            }

            this.ProcessMouseSelection(clickedListVisualItem.Data, notification);
        }

        /// <summary>
        /// Performs logical branching of the selection logic depending on the notification reason.
        /// </summary>
        private void ProcessMouseSelection(RadListDataItem item, MouseNotification reason)
        {
            switch (reason)
            {
                // TODO: Implement other cases.
                case MouseNotification.MouseUp:
                    ProcessSelection(GetIndex(item), false, InputType.Mouse);
                    break;
            }
        }

        #endregion

        #region Keyboard

        /// <summary>
        /// Handles the keyboard input by delegating the information of the event to the appropriate RadListElement subsystem.
        /// </summary>
        private void HandleKeyboard(object sender, RoutedEventArgs args)
        {
            if (args.RoutedEvent == RadItem.KeyDownEvent)
            {
                KeyEventArgs keyArgs = (KeyEventArgs)args.OriginalEventArgs;
                keyArgs.Handled = this.ProcessKeyboardSelection(keyArgs.KeyCode);
            }

            if (args.RoutedEvent == RadItem.KeyPressEvent)
            {
                KeyPressEventArgs keyArgs = (KeyPressEventArgs)args.OriginalEventArgs;
                this.ProcessKeyboardSearch(keyArgs.KeyChar);
                keyArgs.Handled = true;
            }
        }

        /// <summary>
        /// Finds an item with the text provided by an internal search buffer after the character argument is appended to the buffer.
        /// The search buffer is reset after a user defined time since the last character was typed. By default this is 300 ms.
        /// Users can set the KeyboardSearchResetInterval property to a custom interval.
        /// </summary>
        /// <param name="character">A character that will be appended to the search buffer.</param>
        protected internal virtual void ProcessKeyboardSearch(char character)
        {
            if (!this.KeyboardSearchEnabled)
            {
                return;
            }

            if (searchTimer.Enabled)
            {
                searchTimer.Stop();
                searchTimer.Start();
            }
            else
            {
                searchBuffer = new StringBuilder();
                this.searchStartIndex = 0;
                searchTimer.Start();
            }

            searchBuffer.Append(character);

            this.searchStartIndex = this.FindStringNonWrapping(searchBuffer.ToString(), this.searchStartIndex);
            if (this.IsIndexValid(this.searchStartIndex))
            {
                if (this.SelectionMode == SelectionMode.MultiSimple)
                {
                    this.ActiveItem = this.Items[this.searchStartIndex];
                }
                else if(this.SelectionMode != SelectionMode.None)
                {
                    this.HandleSelectOne(this.searchStartIndex);
                }

                this.ScrollToActiveItem();
            }
            else
            {
                this.searchStartIndex = 0;
            }
        }

        /// <summary>
        /// Handles the space key press depending on the SelectionMode and the state of the control key.
        /// </summary>
        private void OnSpace()
        {
            // MultiExtended SelectionMode is equal to MultiSimple when the Control key is pressed.
            if (this.selectionMode == SelectionMode.MultiSimple || (this.selectionMode == SelectionMode.MultiExtended && Control.ModifierKeys == Keys.Control))
            {
                if (this.activeListItem != null)
                {
                    // Pressing the space bar is the same as clicking an item. This is why the InputType is Mouse :)
                    // If the type is Keyboard only the active item will be updated.
                    this.ProcessSelection(GetIndex(this.activeListItem), false, InputType.Mouse);
                }
            }
        }

        /// <summary>
        /// This method is the entry point for the selection logic if initiated by the keyboard.
        /// </summary>
        internal bool ProcessKeyboardSelection(Keys keyCode)
        {
            if (keyCode == Keys.Space)
            {
                OnSpace();
                return true;
            }

            int dir = GetSelectionDirection(keyCode);
            // dir will be 0 if we are not interested in the key provided.
            if (dir == 0)
            {
                return false;
            }

            // The index of the activeItem + the direction may get our of range if the active item is the last item.
            int newIndex = GetIndex(this.activeListItem) + dir;
            if (!IsIndexValid(newIndex))
            {
                return false;
            }

            ProcessSelection(newIndex, false, InputType.Keyboard);

            return true;
        }

        /// <summary>
        /// Determines whether the selection logic should select the next or the previous item depending on the which arrow key is pressed.
        /// </summary>
        private int GetSelectionDirection(Keys keyCode)
        {
            if (keyCode == Keys.Up || keyCode == Keys.Left)
            {
                return -1;
            }
            else if (keyCode == Keys.Down || keyCode == Keys.Right)
            {
                return 1;
            }

            return 0;
        }

        internal void HomeEndSelect(RadListDataItem item)
        {
            if (this.SelectionMode == SelectionMode.MultiExtended)
            {
                if(Control.ModifierKeys == Keys.Shift)
                {
                    int index = this.SelectedIndex;
                    if (!this.IsIndexValid(index))
                    {
                        return;
                    }

                    this.selectionInProgress = true;
                    this.SuspendSelectionEvents = true;
                    this.selectedItems.Clear();
                    this.SelectRange(index, item.RowIndex);
                    this.shiftSelectStartIndex = index;
                    this.SuspendSelectionEvents = false;
                    this.selectionInProgress = false;
                }
                else
                {
                    this.selectionInProgress = true;
                    this.HandleSelectOne(item.RowIndex);
                    this.selectionInProgress = false;
                }
            }

            if(this.SelectionMode == SelectionMode.One)
            {
                this.selectionInProgress = true;
                this.HandleSelectOne(item.RowIndex);
                this.selectionInProgress = false;
            }

            this.ScrollToItem(item);
        }

        #endregion

        #region Core Logic

        private void ClearSelection()
        {
            if (this.OnSelectedIndexChanging(-1))
            {
                return;
            }

            this.selectedItems.Clear();
            this.UpdateActiveItem(this.activeListItem, false);
            this.activeListItem = null;

            // If an invalid index comes from the data layer, we do not want to raise any events
            // if the old selected index is in initial state.
            if (!this.IsOldSelectedIndexInInitialState && (this.oldSelectedIndex != -1))
            {
                this.OnSelectedIndexChanged(-1);
            }
        }
        
        /// <summary>
        /// This method is the entry point in RadListElements selection logic.
        /// </summary>
        private void ProcessSelection(int newIndex, bool onMouseDrag, InputType inputType)
        {
            if (this.selectionInProgress)
            {
                return;
            }

            this.selectionInProgress = true;

            if (!this.IsIndexValid(newIndex))
            {
                this.ClearSelection();
                this.selectionInProgress = false;
                return;
            }

            // We do not care about data layer CurrentPositionChanged since we are causing it with the selection behavior.
            this.UnwireCurrentPosition();

            switch (this.selectionMode)
            {
                case SelectionMode.MultiExtended:
                    HandleMultiExtended(newIndex, onMouseDrag, inputType, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);
                    break;

                case SelectionMode.MultiSimple:
                    HandleMultiSimple(newIndex, inputType);
                    break;

                case SelectionMode.One:
                    HandleSelectOne(newIndex);
                    break;
            }

            this.WireCurrentPosition();

            this.selectionInProgress = false;

            this.ScrollToActiveItem();

            if (this.selectionMode != SelectionMode.None)
            {
                this.viewElement.ForceVisualStateUpdate();
            }
        }

        /// <summary>
        /// Performs logical branching of the MultiExtended selection logic depending on the parameters. 
        /// </summary>
        private void HandleMultiExtended(int newIndex, bool onMouseDrag, InputType inputType, bool shiftKeyPressed, bool controlKeyPressed)
        {
            if (shiftKeyPressed || onMouseDrag)
            {
                int startIndex = this.shiftSelectStartIndex;

                if (!this.IsIndexValid(startIndex))
                {
                    return;
                }

                this.HandleMultiSelectRange(startIndex, newIndex);
            }
            else
            {
                if (controlKeyPressed)
                {
                    HandleMultiSimple(newIndex, inputType);
                }
                else
                {
                    HandleSelectOne(newIndex);
                }
            }
        }

        /// <summary>
        /// This method performs only logical branching of the selection logic depending on the input type parameter.
        /// </summary>
        private void HandleMultiSimple(int newIndex, InputType inputType)
        {
            this.shiftSelectStartIndex = newIndex;
            switch (inputType)
            {
                case InputType.Code:               
                    this.HandleCodeMultiSimple(newIndex);
                    break;
                case InputType.Mouse:                
                    this.HandleMouseMultiSimple(newIndex, true);
                    break;
                case InputType.Keyboard:
                    this.selectedItems.Clear();
                    this.HandleMouseMultiSimple(newIndex, true);
                    break;
            }

            this.UpdateActiveItem(this.dataLayer.GetItemAtIndex(newIndex), true);
        }

        /// <summary>
        /// This method is for clarity. CodeMultiSimple is the same as MouseMultiSimple but does not change the current position of the data layer.
        /// </summary>
        private void HandleCodeMultiSimple(int newIndex)
        {
            this.HandleMouseMultiSimple(newIndex, false);
        }

        /// <summary>
        /// Toggles the Selected state of the item at the specified index and fires selection events depending on the second argument.
        /// </summary>
        /// <param name="newIndex">The index of the item which will selected or deselected.</param>
        /// <param name="changeCurrentPosition">Indicates whether to change the current positio of the data layer and therefore fire selecton events.</param>
        private void HandleMouseMultiSimple(int newIndex, bool changeCurrentPosition)
        {
            RadListDataItem item = this.dataLayer.GetItemAtIndex(newIndex);//this.Items[newIndex];
            if (item.Selected)
            {
                this.MultiSimpleRemove(newIndex, changeCurrentPosition, item);
            }
            else
            {
                this.MultiSimpleAdd(newIndex, changeCurrentPosition, item);
            }
        }

        /// <summary>
        /// Handles the MultiSimple selection logic for adding items.
        /// </summary>
        private void MultiSimpleAdd(int newIndex, bool changeCurrentPosition, RadListDataItem itemToAdd)
        {
            if (this.OnSelectedIndexChanging(newIndex))
            {
                return;
            }

            this.selectedItems.Add(itemToAdd);

            if (changeCurrentPosition)
            {
                this.dataLayer.CurrentPosition = newIndex;
            }

            OnSelectedIndexChanged(this.SelectedIndex);
        }

        /// <summary>
        /// Handles the MultiSimple selection logic for removing items.
        /// </summary>
        private void MultiSimpleRemove(int newIndex, bool changeCurrentPosition, RadListDataItem itemToRemove)
        {
            bool changeSelectedIndex = newIndex == this.SelectedIndex;

            if (changeSelectedIndex)
            {
                int indexToRemove = this.selectedItems.IndexOf(itemToRemove);
                int prevSelectedItemIndex =  indexToRemove - 1;
                RadListDataItem prevSelectedItem = null;

                if (prevSelectedItemIndex > -1)
                {
                    prevSelectedItem = this.selectedItems[prevSelectedItemIndex];
                }
                else
                {
                    prevSelectedItemIndex = indexToRemove + 1;
                    if (prevSelectedItemIndex < this.selectedItems.Count)
                    {
                        prevSelectedItem = this.selectedItems[prevSelectedItemIndex];
                    }
                }

                int nextIndex = this.GetIndex(prevSelectedItem);

                if (this.OnSelectedIndexChanging(nextIndex))
                {
                    return;
                }

                this.selectedItems.Remove(itemToRemove);

                if (changeCurrentPosition && nextIndex > -1)
                {
                    this.dataLayer.CurrentPosition = nextIndex;
                }

                this.OnSelectedIndexChanged(nextIndex);
            }
            else
            {
                this.selectedItems.Remove(itemToRemove);
            }
        }

        bool handleSelectionOneInProgress = false;
        /// <summary>
        /// Selects the item at the specified index and clears all other selected items and updates the active item.
        /// This method triggers selection events.
        /// </summary>
        /// <param name="newIndex">The index of the item which will be selected.</param>
        private void HandleSelectOne(int newIndex)
        {
            if (this.handleSelectionOneInProgress)
            {
                return;
            }

            this.shiftSelectStartIndex = newIndex;
            if (this.OnSelectedIndexChanging(newIndex))
            {
                return;
            }

            this.handleSelectionOneInProgress = true;
            bool fireSelectedIndexChanged = false;
            if (newIndex != this.oldSelectedIndex)
            {
                fireSelectedIndexChanged = true;
            }

            this.selectedItems.Clear();
            RadListDataItem newItem = this.dataLayer.GetItemAtIndex(newIndex);
            
            this.selectedItems.Add(newItem);            
            
            this.SelectedIndex = newIndex;            

            this.UpdateActiveItem(newItem, true);

            if (fireSelectedIndexChanged)// && !this.selectedItems.Inserting)
            {
                this.OnSelectedIndexChanged(newIndex);
            }

            this.handleSelectionOneInProgress = false;
        }

        /// <summary>
        /// Selects all items in the range [startIndex, endIndex] and clears all other selected items.
        /// This method triggers selection events.
        /// </summary>
        /// <param name="startIndex">The beginning of the selection range.</param>
        /// <param name="endIndex">The end of the selected range.</param>
        private void HandleMultiSelectRange(int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
            {
                this.SwapIntegers(ref startIndex, ref endIndex);
            }

            for (int i = this.selectedItems.Count - 1; i > -1; i--)
            {
                int index = this.GetIndex(this.selectedItems[i]);
                if (index < startIndex || index > endIndex)
                {
                    this.selectedItems.RemoveAt(i);
                    int prevIndex = i - 1;
                    if (prevIndex > -1 && prevIndex < this.selectedItems.Count)
                    {
                        this.UpdateActiveItem(this.selectedItems[prevIndex], true);
                    }
                }
            }

            List<RadListDataItem> itemRange = this.dataLayer.GetItemRange(startIndex, endIndex);
            
            for (int i = itemRange.Count -1 ; i >= 0; i--)
            {
                RadListDataItem current = itemRange[i];
            
                if (!current.Selected)
                {
                    this.selectedItems.Add(current);
                    this.UpdateActiveItem(current, true);
                }
            }

            //this.OnSelectedIndexChanged(newPos);
        }

        /// <summary>
        /// This method sets the provided item as active and the previous one to inactive. There can be only active item at a time.
        /// </summary>
        /// <param name="item">The item to set to an active state.</param>
        /// <param name="active">The value to which the Active property of item will be set.</param>
        protected virtual void UpdateActiveItem(RadListDataItem item, bool active)
        {
            if (this.activeListItem == item && active)
            {
                return;
            }

            if (this.activeListItem != null)
            {
                this.activeListItem.SetValue(RadListDataItem.ActiveProperty, false);
            }

            if (active)
            {
                this.activeListItem = item;
            }
            else
            {
                this.activeListItem = null;
            }

            if (item != null)
            {
                item.SetValue(RadListDataItem.ActiveProperty, active);
            }

            this.OnNotifyPropertyChanged("ActiveItem");
        }

        /// <summary>
        /// Sets the SelectedItem and thus SelectedIndex to the logical item with the specified value. If there are many items with the same value the first item found will be selected.
        /// This method triggers selection events.
        /// </summary>
        /// <param name="value">The value for which to find an item.</param>
        protected virtual void SetSelectedValue(object value)
        {
            if (value == this.SelectedValue)
            {
                return;
            }

            if (value == null ||  Convert.IsDBNull(value))
            {
                this.ProcessSelection(-1, false, InputType.Code);
                this.OnNotifyPropertyChanged("SelectedValue");
                return;
            }

            foreach (RadListDataItem current in this.dataLayer.ListSource)
            {
                object currentVal = current.Value;
                if (currentVal == value || ( currentVal !=null && currentVal.Equals(value)))
                {
                    this.ProcessSelection(this.GetIndex(current), false, InputType.Mouse);
                   // this.SelectedItem = current;
                    this.OnNotifyPropertyChanged("SelectedValue");
                    return;
                }

                if (currentVal == null && current.Text != null && value is string && current.Text == value.ToString())
                {
                    this.ProcessSelection(this.GetIndex(current), false, InputType.Mouse);
                    // this.SelectedItem = current;
                    this.OnNotifyPropertyChanged("SelectedValue");
                    return;
                }
            }
        }

        /// <summary>
        /// Sets the the selected data item to the specified item. If the item is different than the current one the selection events will be fired.
        /// This method triggers selection events.
        /// </summary>
        /// <param name="value"></param>
        protected void SetSelectedItem(RadListDataItem value)
        {
            if (value != null && value.Owner != this)
            {
                throw new InvalidOperationException("SelectedItem can not be set to an item that does not exist in this RadListElement instance.");
            }

            if (this.SelectedItem == value)
            {
                return;
            }

            this.SetSelectedItemCore(value);

            this.OnNotifyPropertyChanged("SelectedItem");
        }

        protected virtual void SetSelectedItemCore(RadListDataItem item)
        {
            if (item == null)
            {
                this.ClearSelection();
                return;
            }

            switch(this.SelectionMode)
            {
                case SelectionMode.None:
                    return;
                case SelectionMode.One:
                case SelectionMode.MultiExtended:
                    this.ProcessSelection(this.GetIndex(item), false, InputType.Mouse);
                    break;
                case SelectionMode.MultiSimple:
                    int index = this.GetIndex(item);
                    if (this.OnSelectedIndexChanging(index))
                    {
                        return;
                    }

                    // We don't care about the current position, we are causing it.
                    this.UnwireCurrentPosition();
                    this.dataLayer.CurrentItem = item;
                    if (!this.selectedItems.Contains(item))
                    {
                        this.selectedItems.Add(item);
                    }
                    this.WireCurrentPosition();

                    this.OnSelectedIndexChanged(index);
                    break;
            }
        }

        /// <summary>
        /// Sets the selected index to the specified value if it is different than the current value and fires the selection events.
        /// This method triggers selection events.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void SetSelectedIndex(int value)
        {
            if (value == this.SelectedIndex)
            {
                return;
            }

            if (value == -1)
            {
                if (this.OnSelectedIndexChanging(value))
                {
                    return;
                }

                this.selectedItems.Clear();
                this.OnSelectedIndexChanged(value);

                this.UpdateActiveItem(this.activeListItem, false);
                this.OnNotifyPropertyChanged("SelectedIndex");
                return;
            }

            if (this.dataLayer.CurrentPosition == value)
            {
                this.ProcessSelection(value, false, InputType.Code);
            }
            else
            {
                //if (value < 0 || value >= this.Items.Count)
                //{
                //    throw new ArgumentOutOfRangeException("Selected Index is out of range");
                //}

                this.dataLayer.DataView.MoveCurrentToPosition(value);
            }

            this.OnNotifyPropertyChanged("SelectedIndex");
        }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines if RadListElement is ready for data binding. This is true only when Items is empty or DataSource is different from null.
        /// If RadListElement is not ready for binding an InvalidOperationException is thrown.
        /// </summary>
        private void CheckReadyForBinding()
        {
            if (this.Items.Count != 0 && this.DataSource == null)
            {
                //throw new InvalidOperationException("RadListControl can not transition directly from unbound to bound mode. The Items collection must be explicitly cleared first.");
            }
        }

        /// <summary>
        /// Determines if this list element is ready for unbound mode.
        /// If it is not an invalid operation exception is thrown.
        /// RadListElement is ready for unbound mode if it has not data source set.
        /// </summary>
        internal void CheckReadyForUnboundMode()
        {
            if (this.DataSource != null)
            {
                //throw new InvalidOperationException("RadListControl can not transition directly from bound to unbound mode. The data source must be removed first.");
            }
        }

        /// <summary>
        /// Returns the value of the Value property of the RadListDataItem at the specified index.
        /// </summary>
        /// <param name="index">The index of the item from which to get the Value property.</param>
        /// <returns></returns>
        private object GetValueByIndex(int index)
        {
            if (IsIndexValid(index))
            {
                return this.dataLayer.DataView[index].Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the index of the provided list data item. This index determines the items position in the data view.
        /// </summary>
        /// <param name="item">The index for which to return an index.</param>
        /// <returns>Returns the index of the provided item.</returns>
        protected int GetIndex(RadListDataItem item)
        {
            if (item == null)
            {
                return -1;
            }

            return item.RowIndex;
        }

        /// <summary>
        /// Gets the text of the data item provided in the argument depending on the ItemTextComparisonMode property.
        /// </summary>
        /// <param name="dataItem">The data item for which to get the Text value.</param>
        /// <returns>The text value of the provided data item.</returns>
        internal string GetItemText(RadListDataItem dataItem)
        {
            if (this.itemTextComparisonMode == ItemTextComparisonMode.DataText)
            {
                return dataItem.CachedText;
            }
            else
            {
                return dataItem.Text;
            }
        }

        /// <summary>
        /// Determines whether the provided index is in the range [0, Items.Count)
        /// </summary>
        /// <param name="value">The index to validate.</param>
        /// <returns>Returns true if the index is inside [0, Items.Count) and false otherwise.</returns>
        protected bool IsIndexValid(int value)
        {
            return value >= 0 && value < this.dataLayer.GetVisibleItemsCount();
        }

        /// <summary>
        /// Swaps two integers.
        /// </summary>
        private void SwapIntegers(ref int a, ref int b)
        {
            int t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// Disposes every item in the Items collection.
        /// </summary>
        private void DisposeItems()
        {
            foreach (RadListDataItem itemToDispose in this.Items)
            {
                itemToDispose.Dispose();
            }
        }

        /// <summary>
        /// Converts the provided ListSortDirection to SortStyle.
        /// </summary>
        /// <param name="direction">The ListSortDirection to be converted to SortStyle.</param>
        /// <returns>The converted SortStyle value.</returns>
        private SortStyle GetSortStyle(ListSortDirection direction)
        {
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    return SortStyle.Ascending;
                case ListSortDirection.Descending:
                    return SortStyle.Descending;
            }

            return SortStyle.None;
        }

        /// <summary>
        /// Sets the sort style to the specified value and fires the SortStyle changed event if the new value is different than the previous value.
        /// </summary>
        /// <param name="value"></param>
        private void SetSortStyle(SortStyle value)
        {
            if (value == this.SortStyle)
            {
                return;
            }

            this.SuspendSelectionEvents = true;
            RadListDataItem currentItem = this.SelectedItem;
            int prevIndex = this.GetIndex(currentItem);
            this.sortDescriptors.Clear();

            bool clearSort = false;
            ListSortDirection direction = ListSortDirection.Ascending;
            this.dataLayer.DataView.Comparer = new ListItemAscendingComparer();

            switch (value)
            {
                case SortStyle.Descending:
                    direction = ListSortDirection.Descending;
                    this.dataLayer.DataView.Comparer = new ListItemDescendingComparer();
                    break;
                case SortStyle.None:
                    clearSort = true; 
                    break;
            }

            if (!clearSort)
            {
                this.sortDescriptors.Add(this.GetSortPropertyName(), direction);
            }

            if (this.SelectedItem != currentItem)
            {
                this.SelectedItem = currentItem;
            }

            this.OnSortStyleChanged(clearSort ? SortStyle.None : value);
            this.OnNotifyPropertyChanged("SortStyle");
            this.SuspendSelectionEvents = false;

            int curIndex = this.GetIndex(currentItem);

            if (curIndex != prevIndex)
            {
                this.OnSelectedIndexChanged(curIndex);
            }

            return;
        }

        /// <summary>
        /// Sets the selection mode of this RadListElement to the provided value.
        /// </summary>
        /// <param name="mode">The new selection mode.</param>
        private void SetSelectionMode(SelectionMode mode)
        {
            if (mode == this.selectionMode)
            {
                return;
            }

            this.selectionMode = mode;
            int curPos = this.dataLayer.CurrentPosition;
            if(!this.IsIndexValid(curPos))
            {
                return;
            }

            RadListDataItem currentItem = this.Items[curPos];

            switch (this.selectionMode)
            {
                case SelectionMode.None:
                    this.SetSelectedIndex(-1);
                    break;

                case SelectionMode.MultiSimple:
                case SelectionMode.MultiExtended:
                    if (curPos > -1 && !currentItem.Selected)
                    {
                        this.ProcessSelection(curPos, false, InputType.Code);
                    }
                    break;

                case SelectionMode.One:
                    if (curPos > -1)
                    {
                        this.ProcessSelection(curPos, false, InputType.Code);
                    }
                    break;
            }

            this.OnNotifyPropertyChanged("SelectionMode");
        }

        /// <summary>
        /// Gets property name by which items will be sorted when SortStyle is set.
        /// If DisplayMember is an empty string, items will be sorted by their text, otherwise
        /// they will be sorted according to the DisplayMember.
        /// </summary>
        /// <returns>Returns the property by which items will be sorted.</returns>
        private string GetSortPropertyName()
        {
            if (this.DataSource == null)
            {
                return this.DisplayMember == "" ? "Text" : this.DisplayMember;
            }
            else
            {
                return this.DisplayMember;
            }
        }

        /// <summary>
        /// Clamps the provided value parameter to be be in the closed range [min, max].
        /// </summary>
        /// <param name="min">The left bound of the range.</param>
        /// <param name="max">The right bound of the range.</param>
        /// <param name="value">The value to be clamped to the range specified by the previous two parameters.</param>
        private void ClampValue(int min, int max, ref int value)
        {
            if (value > max)
            {
                value = max;
            }

            if (value < min)
            {
                value = min;
            }
        }

        /// <summary>
        /// This is a helper method which keeps track of the number of subscriptions to the CurrentPositionChanged event of the data layer.
        /// </summary>
        private void WireCurrentPosition()
        {
            Debug.Assert(subscriptionCounter == 0);
            subscriptionCounter++;
            this.dataLayer.CurrentPositionChanged += this.dataLayer_CurrentPositionChanged;
        }

        /// <summary>
        /// This is a helper method which keeps track of the number of unsubscriptions from the CurrentPositionChanged event of the data layer.
        /// </summary>
        private void UnwireCurrentPosition()
        {
            subscriptionCounter--;
            this.dataLayer.CurrentPositionChanged -= this.dataLayer_CurrentPositionChanged;
        }

        #endregion

        #region Private Types

        /// <summary>
        /// This class is used to compare data items when sorting in ascending order.
        /// </summary>
        private class ListItemAscendingComparer : IComparer<RadListDataItem>
        {
            #region IComparer<ListItem> Members
            public virtual int Compare(RadListDataItem x, RadListDataItem y)
            {
                bool ignoreCase = false;
                if (x.Owner != null)
                {
                    ignoreCase = !x.Owner.CaseSensitiveSort;
                }

                return string.Compare(x.CachedText, y.CachedText, ignoreCase);
            }
            #endregion
        }

        /// <summary>
        /// This class is used to compare data items when sorting in descending order.
        /// </summary>
        private class ListItemDescendingComparer : IComparer<RadListDataItem>
        {
            #region IComparer<ListItem> Members
            public virtual int Compare(RadListDataItem x, RadListDataItem y)
            {
                bool ignoreCase = false;
                if (x.Owner != null)
                {
                    ignoreCase = !x.Owner.CaseSensitiveSort;
                }

                return string.Compare(y.CachedText, x.CachedText, ignoreCase);
            }
            #endregion
        }

        #endregion

        #region Testing Methods

        /// <summary>
        /// This method is for testing purposes. It invokes the MultiExtended selection logic with the supplied parameters.
        /// </summary>
        /// <param name="index">The index to which the selection will span starting from SelectedIndex.</param>
        /// <param name="input">An enumeration indicating whether the input comes from the keyboard, the mouse or from code.</param>
        /// <param name="shift">If this flag is true the selection logic will invoke MultiExtended as if the shift key was pressed.</param>
        /// <param name="control">If this flag is true the selection logic will invoke MultiExtended as if the control key was pressed.</param>
        internal void InvokeMultiExtendedSelection(int index, InputType input, bool shift, bool control)
        {
            this.UnwireCurrentPosition();
            this.HandleMultiExtended(index, false, input, shift, control);
            this.WireCurrentPosition();
        }

        /// <summary>
        /// Returns the logical item associated with the top visible item if the layout is vertical and the left most item if the layout is horizontal.
        /// </summary>
        /// <returns></returns>
        internal RadListDataItem GetTopVisibleItem()
        {
            return ((RadListVisualItem)this.viewElement.Children[0]).Data;
        }
      
        #endregion

        #region Drag&Drop        
        
        
        //protected override void ProcessDragDrop(Point dropLocation, ISupportDrag dragObject)
        //{
        //    if (dragObject is RadListElement)
        //    {
        //        RadElement element = this.ElementTree.GetElementAtPoint(dropLocation);
        //        IReadOnlyCollection<RadListDataItem> draggedItems= dragObject.GetDataContext() as IReadOnlyCollection<RadListDataItem>;
        //        RadListVisualItem visualItem = element as RadListVisualItem;
        //        int index = 0;
        //        if (visualItem != null)
        //        {
        //            index = this.Items.IndexOf(visualItem.Data);
        //        }
                
        //        for (int i = index; i < draggedItems.Count; ++i)
        //        {
        //            this.Items.Insert(i, draggedItems[i]);
        //        }                
        //    }
        //    else
        //    {
        //        base.ProcessDragDrop(dropLocation, dragObject);
        //    }
        //}

        #endregion
    }   
}
