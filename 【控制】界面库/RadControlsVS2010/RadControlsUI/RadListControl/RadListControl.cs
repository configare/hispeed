using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Data;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI.Data;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents data in a list layout similar to the ListBox control provided by Microsoft.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.DataControlsGroup)]
    [Designer(DesignerConsts.RadListControlDesignerString)]
    [DefaultEvent("SelectedIndexChanged")]
    [ComplexBindingProperties("DataSource", "ValueMember")]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
    [ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    [ToolboxItem(true)]
    public class RadListControl : RadControl
    {
        #region Event keys
        public static readonly object SelectedIndexChangedEventKey;
        public static readonly object SelectedIndexChangingEventKey;
        public static readonly object SelectedValueChangedEventKey;
        public static readonly object ListItemDataBindingEventKey;
        public static readonly object ListItemDataBoundEventKey;
        public static readonly object CreatingVisualListItemEventKey;
        public static readonly object SortStyleChangedEventKey;
        public static readonly object VisualItemFormattingEventKey;

        #endregion

        #region Fields

        private RadListElement element;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes all event key objects and performs other static initialization.
        /// </summary>
        static RadListControl()
        {
            SelectedIndexChangedEventKey = new object();
            SelectedIndexChangingEventKey = new object();
            SelectedValueChangedEventKey = new object();
            ListItemDataBindingEventKey = new object();
            ListItemDataBoundEventKey = new object();
            CreatingVisualListItemEventKey = new object();
            SortStyleChangedEventKey = new object();
            VisualItemFormattingEventKey = new object();
        }

        public RadListControl()
        {
            base.SetStyle(ControlStyles.UseTextForAccessibility, false);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            element = CreateListElement();
            parent.Children.Add(element);
            WireEvents();
        }

        protected virtual RadListElement CreateListElement()
        {
            return new RadListElement();
        }

        /// <summary>
        /// Subscribes to the relevant events of the underlaying RadListElement.
        /// </summary>
        protected virtual void WireEvents()
        {
            this.element.SelectedIndexChanged += element_SelectedIndexChanged;
            this.element.SelectedIndexChanging += element_SelectedIndexChanging;
            this.element.SelectedValueChanged += element_SelectedValueChanged;
            this.element.ItemDataBinding += element_ItemDataBinding;
            this.element.ItemDataBound += element_ItemDataBound;
            this.element.CreatingVisualItem += element_CreatingVisualItem;
            this.element.SortStyleChanged += element_SortStyleChanged;
            this.element.VisualItemFormatting += element_VisualItemFormatting;
            this.element.PropertyChanged += element_PropertyChanged;
        }

        /// <summary>
        /// Unsubscribes from the relevant events of the underlaying RadListElement.
        /// </summary>
        protected virtual void UnwireEvents()
        {
            if (this.element == null)
            {
                return;
            }

            this.element.SelectedIndexChanged -= element_SelectedIndexChanged;
            this.element.SelectedIndexChanging -= element_SelectedIndexChanging;
            this.element.SelectedValueChanged -= element_SelectedValueChanged;
            this.element.ItemDataBinding -= element_ItemDataBinding;
            this.element.ItemDataBound -= element_ItemDataBound;
            this.element.CreatingVisualItem -= element_CreatingVisualItem;
            this.element.SortStyleChanged -= element_SortStyleChanged;
            this.element.VisualItemFormatting -= element_VisualItemFormatting;
            this.element.PropertyChanged -= element_PropertyChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the items should be displayed in groups.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        [Description("Indicates whether the items should be displayed in groups.")]
        private bool ShowGroups
        {
            get
            {
                return this.ListElement.ShowGroups;
            }
            set
            {
                this.ListElement.ShowGroups = value;
            }
        }

        /// <summary>
        /// Gets the collection of groups that items are grouped into
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the collection of groups that items are grouped into.")]
        private ListGroupCollection Groups
        {
            get
            {
                return this.ListElement.Groups;
            }
        }

        [DefaultValue(true)]
        public bool FitItemsToSize
        {
            get
            {
                return this.ListElement.FitItemsToSize;
            }

            set
            {
                this.ListElement.FitItemsToSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether text case will be taken into account when sorting.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets a value that indicates whether text case will be taken into account when sorting.")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CaseSensitiveSort
        {
            get
            {
                return this.ListElement.CaseSensitiveSort;
            }

            set
            {
                this.ListElement.CaseSensitiveSort = value;
            }
        }

        [Description("Gets or sets the item height for the items. This property is disregarded when AutoSizeItems is set to true.")]
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(16)]
        public int ItemHeight
        {
            get
            {
                return this.ListElement.ItemHeight;
            }

            set
            {
                this.ListElement.ItemHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how long the user must wait before searching with the keyboard is reset.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets a value that specifies how long the user must wait before searching with the keyboard is reset.")]
        [DefaultValue(300)]
        [Category("Behavior")]
        public int KeyboardSearchResetInterval
        {
            get
            {
                return this.ListElement.KeyboardSearchResetInterval;
            }

            set
            {
                this.ListElement.KeyboardSearchResetInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the user can search for an item by typing characters when RadListControl is focused.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets a value that determines whether the user can search for an item by typing characters when RadListControl is focused.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool KeyboardSearchEnabled
        {
            get
            {
                return this.ListElement.KeyboardSearchEnabled;
            }

            set
            {
                this.ListElement.KeyboardSearchEnabled = value;
            }
        }

        /// <summary>
        /// The ListElement responsible for the majority of the control logic. The RadListControl is a wrapper of the RadListElement.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListElement ListElement
        {
            get
            {
                return this.element;
            }
            set
            {
                this.UnwireEvents();
                this.element = value;
                this.WireEvents();
                this.OnNotifyPropertyChanged("ListElement");
            }
        }

        /// <summary>
        /// Gets the Items collection. Items can not be modified in data bound mode, and a DataSource can not be assigned while there
        /// are items in this collection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.RadListControlDataItemCollectionDesignerString, typeof(UITypeEditor))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this RadListControl.")]
        [Browsable(true)]
        public RadListDataItemCollection Items
        {
            get
            {
                return (RadListDataItemCollection)element.Items;
            }
        }

        /// <summary>
        /// Provides a read only interface to the selected items. In order to select an item, use its Selected property.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyCollection<RadListDataItem> SelectedItems
        {
            get
            {
                return this.ListElement.SelectedItems;
            }
        }

        /// <summary>
        /// Gets or sets the SelectionMode of RadListControl. This property has a similar effect to the SelectionMode of the
        /// standard Microsoft ListBox control.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(SelectionMode.One)]
        [Category("Behavior")]
        [Description("Gets or sets the SelectionMode of RadListControl. This property has a similar effect to the SelectionMode of the standard Microsoft ListBox control.")]
        public SelectionMode SelectionMode
        {
            get
            {
                return this.ListElement.SelectionMode;
            }

            set
            {
                this.ListElement.SelectionMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the SelectedValue. A linear search is performed to find a data item that has the same value
        /// in its Value property and SelectedItem and SelectedIndex are updated to it and its index respectively.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(true)]
        public object SelectedValue
        {
            get
            {
                return this.ListElement.SelectedValue;
            }

            set
            {
                this.ListElement.SelectedValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the active item. The Active item is relevant only in MultiSimple SelectionMode or MultiExtended in combination with
        /// the control keyboard key.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListDataItem ActiveItem
        {
            get
            {
                return this.ListElement.ActiveItem;
            }

            set
            {
                this.ListElement.ActiveItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(true)]
        public RadListDataItem SelectedItem
        {
            get
            {
                return element.SelectedItem;
            }

            set
            {
                element.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected index.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                return element.SelectedIndex;
            }

            set
            {
                element.SelectedIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets an object which will provide the data to be visualized as a list.
        /// </summary>
        [DefaultValue(null)]
        [AttributeProvider(typeof(IListSource))]
        [Description("Gets or sets the object that is responsible for providing data objects for the RadListElement. Setting this property throws an InvalidOperationException if Items is not empty and the data source is null.")]
        [Category(RadDesignCategory.DataCategory)]
        [Browsable(true)]
        public object DataSource
        {
            get
            {
                return element.DataSource;
            }

            set
            {
                element.DataSource = value;
                this.OnDataBindingComplete(this, new ListBindingCompleteEventArgs(ListChangedType.Reset));
            }
        }

        /// <summary>
        /// Gets or sets a property name which will be used to extract a string value from the data items in order to provide
        /// a meaningful display value.
        /// </summary>
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Gets or sets a string which will be used to get a text string for each visual item. This property can not be set to null. Setting it to null will cause it to contain an empty string.")]
        [Category(RadDesignCategory.DataCategory)]
        [DefaultValue("")]
        [Browsable(true)]
        public string DisplayMember
        {
            get
            {
                return element.DisplayMember;
            }

            set
            {
                element.DisplayMember = value;
            }
        }

        /// <summary>
        /// Gets or sets a property name which will be used to extract a value from the data items. The value of the property with
        /// this name will be available via the Value property of every RadListDataItem in the Items collection.
        /// </summary>
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        [Description("Gets or sets the string through which the SelectedValue property will be determined. This property can not be set to null. Setting it to null will cause it to contain an empty string.")]
        [Category(RadDesignCategory.DataCategory)]
        [Browsable(true)]
        public string ValueMember
        {
            get
            {
                return element.ValueMember;
            }

            set
            {
                element.ValueMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort style.
        /// </summary>
        [DefaultValue(SortStyle.None)]
        [Category("Behavior")]
        [Description("Gets or sets the sort style.")]
        [Browsable(true)]
        public SortStyle SortStyle
        {
            get
            {
                return this.element.SortStyle;
            }

            set
            {
                this.element.SortStyle = value;
            }
        }

        /// <summary>
        /// Gets or set the scroll mode.
        /// </summary>
        [DefaultValue(ItemScrollerScrollModes.Smooth)]
        [Category("Behavior")]
        [Description("Gets or set the scroll mode.")]
        [Browsable(true)]
        public ItemScrollerScrollModes ScrollMode
        {
            get
            {
                return this.ListElement.ScrollMode;
            }

            set
            {
                this.ListElement.ScrollMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a format string which will be used for visual formatting of the items text.
        /// </summary>
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("Gets or sets a format string which will be used for visual formatting of the items text.")]
        [Browsable(true)]
        public string FormatString
        {
            get
            {
                return this.ListElement.FormatString;
            }

            set
            {
                this.ListElement.FormatString = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the FormatString and FormatInfo properties will be used to format
        /// the items text. Setting this property to false may improve performance.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Gets or sets a value that indicates whether the FormatString and FormatInfo properties will be used to format the items text. Setting this property to false may improve performance.")]
        [Browsable(true)]
        public bool FormattingEnabled
        {
            get
            {
                return this.ListElement.FormattingEnabled;
            }

            set
            {
                this.ListElement.FormattingEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether items will be sized according to
        /// their content. If this property is true the user can set the Height property of each
        /// individual RadListDataItem in the Items collection in order to override the automatic
        /// sizing.
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Description("Gets or sets a value that indicates whether items will be sized according to their content. If this property is true the user can set the Height property of each individual RadListDataItem in the Items collection in order to override the automatic sizing.")]
        [Browsable(true)]
        public bool AutoSizeItems
        {
            get
            {
                return this.ListElement.AutoSizeItems;
            }

            set
            {
                this.ListElement.AutoSizeItems = value;
                this.RootElement.InvalidateMeasure(true);
                this.RootElement.InvalidateArrange(true);
                this.RootElement.UpdateLayout();
                this.ListElement.ViewElement.UpdateItems();
                this.ListElement.ViewElement.PerformLayout();
                this.ListElement.Scroller.UpdateScrollRange();
                this.ListElement.HScrollBar.Maximum = this.ListElement.Scroller.MaxItemWidth;
            }
        }

        /// <summary>
        /// Gets or sets a predicate which filters which items can be visible.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Predicate<RadListDataItem> Filter
        {
            get
            {
                return this.ListElement.Filter;
            }

            set
            {
                this.ListElement.Filter = value;
            }
        }

        /// <summary>
        /// Gets or sets a filter expression which determines which items will be visible.
        /// </summary>
        [Category("Data")]
        [DefaultValue("")]
        [Description("Gets or sets a filter expression which determines which items will be visible.")]
        [Browsable(true)]
        public string FilterExpression
        {
            get
            {
                return this.ListElement.FilterExpression;
            }

            set
            {
                this.ListElement.FilterExpression = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is a Filter or FilterExpression set.
        /// </summary>
        [Browsable(false)]
        public bool IsFilterActive
        {
            get
            {
                return this.ListElement.IsFilterActive;
            }
        }

        /// <summary>
        /// Gets or sets an object that implements IFindStringComparer.
        /// The value of this property is used in the FindString() method when searching for an item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFindStringComparer FindStringComparer
        {
            get
            {
                return this.ListElement.FindStringComparer;
            }

            set
            {
                this.ListElement.FindStringComparer = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the FindString() method searches via the text property
        /// set by the user or by the text provided by the data binding logic, that is, by DisplayMember. 
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(ItemTextComparisonMode.UserText)]
        [Description("Gets or sets a value that determines whether the FindString() method searches via the text property set by the user or by the text provided by the data binding logic, that is, by DisplayMember.")]
        [Browsable(false)]
        public ItemTextComparisonMode ItemTextComparisonMode
        {
            get
            {
                return this.ListElement.ItemTextComparisonMode;
            }

            set
            {
                this.ListElement.ItemTextComparisonMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this RadListControl will stop firing the ItemsChanging and ItemsChanged events.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool SuspendItemsChangeEvents
        {
            get
            {
                return this.ListElement.SuspendItemsChangeEvents;
            }

            set
            {
                this.ListElement.SuspendItemsChangeEvents = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to stop the selection events from firing. These are SelectedIndexChanged,
        /// SelectedIndexChanging and SelectedValueChanged.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool SuspendSelectionEvents
        {
            get
            {
                return this.ListElement.SuspendSelectionEvents;
            }

            set
            {
                this.ListElement.SuspendSelectionEvents = value;
            }
        }

        #endregion

        #region Public Methods
        

        /// <commentsfrom cref="RadListElement.FindItemExact" filter=""/>
        public RadListDataItem FindItemExact(string text, bool caseSensitive)
        {
            return this.ListElement.FindItemExact(text, caseSensitive);
        }


        /// <summary>
        /// Forces re-evaluation of the current data source (if any).
        /// </summary>
        public void Rebind()
        {
            this.element.Rebind();
        }

        /// <summary>
        /// Suspends internal notifications and processing in order to improve performance.
        /// This method is cumulative, that is, if BeginUpdate is called N times, EndUpdate must also be called N times.
        /// Calling BeginUpdate will cause the ItemsChanged event to stop firing until EndUpdate is called.
        /// </summary>
        public void BeginUpdate()
        {
            this.ListElement.BeginUpdate();
        }

        /// <summary>
        /// Resumes the internal notifications and processing previously suspended by BeginUpdate.
        /// </summary>
        public void EndUpdate()
        {
            this.ListElement.EndUpdate();
        }

        /// <summary>
        /// Defers the refresh.
        /// </summary>
        /// <returns></returns>
        public virtual IDisposable DeferRefresh()
        {
            return this.ListElement.DeferRefresh();
        }

        /// <summary>
        /// Selects all items if the SelectionMode allows it.
        /// This method throws an InvalidOperationException if SelectionMode is One or None.
        /// </summary>
        public void SelectAll()
        {
            this.ListElement.SelectAll();
        }

        /// <summary>
        /// Clears the currently selected items and selects all items in the closed range [startIndex, endIndex].
        /// </summary>
        /// <param name="startIndex">The first index at which to start selecting items.</param>
        /// <param name="endIndex">The index of one item past the last one to be selected.</param>
        public void SelectRange(int startIndex, int endIndex)
        {
            this.ListElement.SelectRange(startIndex, endIndex);
        }

        /// <summary>
        /// Scrolls to the provided item so that the item will appear at the top of the view if it is before the currently visible items
        /// and at the bottom of the view if it is after the currently visible items.
        /// </summary>
        /// <param name="item">The item to scroll to.</param>
        public void ScrollToItem(RadListDataItem item)
        {
            this.ListElement.ScrollToItem(item);
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
            return this.ListElement.FindString(s);
        }

        /// <summary>
        /// Searches for an item related to the specified string. The relation is described by the object assigned to FindStringComparer property.
        /// By default this relation is the System.String.StartsWith().
        /// This method starts searching from the specified index. If the algorithm reaches the end of the Items collection it wraps to the beginning
        /// and continues until one before the provided index.
        /// </summary>
        /// <param name="s">The string with which every item will be compared.</param>
        /// <param name="startIndex">The index from which to start searching.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindString(string s, int startIndex)
        {
            return this.ListElement.FindString(s, startIndex);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public int FindStringExact(string s)
        {
            return this.ListElement.FindStringExact(s);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but matches an item only if its text is exactly equal to the provided string.
        /// </summary>
        public int FindStringExact(string s, int startIndex)
        {
            return this.ListElement.FindStringExact(s, startIndex);
        }

        /// <summary>
        /// Searches for an item in the same manner as FindString() but does not start from the beginning when the end of the Items collection
        /// is reached.
        /// </summary>
        /// <param name="s">The string that will be used to search for an item.</param>
        /// <returns>The index of the found item or -1 if no item is found.</returns>
        public int FindStringNonWrapping(string s)
        {
            return this.ListElement.FindStringNonWrapping(s);
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
            return this.ListElement.FindStringNonWrapping(s, startIndex);
        }

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
        /// This event fires when the selected index property changes.
        /// </summary>
        public event PositionChangedEventHandler SelectedIndexChanged
        {
            add
            {
                this.Events.AddHandler(SelectedIndexChangedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(SelectedIndexChangedEventKey, value);
            }
        }

        /// <summary>
        /// This event fires before SelectedIndex changes. This event allows the operation to be cancelled.
        /// </summary>
        public event PositionChangingEventHandler SelectedIndexChanging
        {
            add
            {
                this.Events.AddHandler(SelectedIndexChangingEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(SelectedIndexChangingEventKey, value);
            }
        }

        /// <summary>
        /// This event fires only if the SelectedValue has really changed. For example it will not fire if the previously selected item
        /// has the same value as the newly selected item.
        /// </summary>
        public event EventHandler SelectedValueChanged
        {
            add
            {
                this.Events.AddHandler(SelectedValueChangedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(SelectedValueChangedEventKey, value);
            }
        }

        /// <summary>
        /// This event fires before a RadListDataItem is data bound. This happens
        /// when the DataSource property is assigned and the event fires for every item provided by the data source.
        /// This event allows a custom RadListDataItem to be provided by the user.
        /// </summary>
        public event ListItemDataBindingEventHandler ItemDataBinding
        {
            add
            {
                this.Events.AddHandler(ListItemDataBindingEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(ListItemDataBindingEventKey, value);
            }
        }

        /// <summary>
        /// This event fires after a RadListDataItem is data bound. This happens
        /// when the DataSource property is assigned and the event is fired for every item provided by the data source.
        /// </summary>
        public event ListItemDataBoundEventHandler ItemDataBound
        {
            add
            {
                this.Events.AddHandler(ListItemDataBoundEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(ListItemDataBoundEventKey, value);
            }
        }

        /// <summary>
        /// This event allows the user to create custom visual items.
        /// It is fired initially for all the visible items and when the control is resized afterwards.
        /// </summary>
        public event CreatingVisualListItemEventHandler CreatingVisualListItem
        {
            add
            {
                this.Events.AddHandler(CreatingVisualListItemEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(CreatingVisualListItemEventKey, value);
            }
        }

        /// <summary>
        /// This event fires when the SortStyle property changes.
        /// </summary>
        public event SortStyleChangedEventHandler SortStyleChanged
        {
            add
            {
                this.Events.AddHandler(SortStyleChangedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(SortStyleChangedEventKey, value);
            }
        }

        /// <summary>
        /// The VisualItemFormatting event fires whenever a property of a visible data item changes
        /// and whenever a visual item is associated with a new data item. During scrolling for example.
        /// </summary>
        public event VisualListItemFormattingEventHandler VisualItemFormatting
        {
            add
            {
                this.Events.AddHandler(VisualItemFormattingEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(VisualItemFormattingEventKey, value);
            }
        }

        #endregion

        #region Event Handlers

        private void element_SelectedIndexChanging(object sender, PositionChangingCancelEventArgs e)
        {
            e.Cancel = this.OnSelectedIndexChanging(sender, e.Position);
        }

        private void element_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            this.OnSelectedIndexChanged(sender, e.Position);
        }

        private void element_SelectedValueChanged(object sender, EventArgs e)
        {
            ValueChangedEventArgs args = (ValueChangedEventArgs)e;
            this.OnSelectedValueChanged(sender, args.Position, args.NewValue, args.OldValue);
        }

        private void element_ItemDataBound(object sender, ListItemDataBoundEventArgs args)
        {
            this.OnItemDataBound(sender, args);
        }

        private void element_ItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            this.OnItemDataBinding(sender, args);
        }

        private void element_CreatingVisualItem(object sender, CreatingVisualListItemEventArgs args)
        {
            this.OnCreatingVisualItem(sender, args);
        }

        private void element_SortStyleChanged(object sender, SortStyleChangedEventArgs args)
        {
            this.OnSortStyleChanged(sender, args);
        }

        private void element_VisualItemFormatting(object sender, VisualItemFormattingEventArgs args)
        {
            this.OnVisualItemFormatting(sender, args);
        }

        void element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnNotifyPropertyChanged(e.PropertyName);
        }

        #endregion

        #region Events Management

        protected virtual void OnSelectedIndexChanged(object sender, int newIndex)
        {
            PositionChangedEventHandler handler = (PositionChangedEventHandler)this.Events[SelectedIndexChangedEventKey];
            if (handler != null)
            {
                handler(sender, new PositionChangedEventArgs(newIndex));
            }
        }

        protected virtual bool OnSelectedIndexChanging(object sender, int newIndex)
        {
            PositionChangingEventHandler handler = (PositionChangingEventHandler)this.Events[SelectedIndexChangingEventKey];
            if (handler != null)
            {
                PositionChangingCancelEventArgs args = new PositionChangingCancelEventArgs(newIndex);
                handler(this, args);
                return args.Cancel;
            }

            return false;
        }

        protected virtual void OnSelectedValueChanged(object sender, int newIndex, object newValue, object oldValue)
        {
            EventHandler handler = (EventHandler)this.Events[SelectedValueChangedEventKey];
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs(newIndex, newValue, oldValue));
            }
        }

        protected virtual void OnItemDataBinding(object sender, ListItemDataBindingEventArgs args)
        {
            ListItemDataBindingEventHandler handler = (ListItemDataBindingEventHandler)this.Events[ListItemDataBindingEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnItemDataBound(object sender, ListItemDataBoundEventArgs args)
        {
            ListItemDataBoundEventHandler handler = (ListItemDataBoundEventHandler)this.Events[ListItemDataBoundEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnCreatingVisualItem(object sender, CreatingVisualListItemEventArgs args)
        {
            CreatingVisualListItemEventHandler handler = (CreatingVisualListItemEventHandler)this.Events[CreatingVisualListItemEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnSortStyleChanged(object sender, SortStyleChangedEventArgs args)
        {
            SortStyleChangedEventHandler handler = (SortStyleChangedEventHandler)this.Events[SortStyleChangedEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnVisualItemFormatting(object sender, VisualItemFormattingEventArgs args)
        {
            VisualListItemFormattingEventHandler handler = (VisualListItemFormattingEventHandler)this.Events[VisualItemFormattingEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region Overrides

        protected override Size DefaultSize
        {
            get
            {
                return new Size(120, 95);
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (keyCode == Keys.Left || keyCode == Keys.Right || keyCode == Keys.Up || keyCode == Keys.Down)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.ListElement.Focus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UnwireEvents();
                this.element = null;
            }

            base.Dispose(disposing);
        }

        protected override bool CanEditElementAtDesignTime(RadElement element)
        {
            if (element is RadListVisualItem)
            {
                return false;
            }

            return base.CanEditElementAtDesignTime(element);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.ListElement.OnMouseWheel(e.Delta);
            if (e is HandledMouseEventArgs)
            {
                (e as HandledMouseEventArgs).Handled = true;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            RadElement elementUnderMouse = this.RootElement.ElementTree.GetElementAtPoint(
                this.RootElement, e.Location, null);
            if (elementUnderMouse != null && elementUnderMouse is RadItem)
            {
                //this.AccessibilityNotifyClients(AccessibleEvents.SelectionAdd, ((elementUnderMouse as RadItem).AccessibilityObject as RadItemAccessibleObject1).ID);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    this.HandlePageDownKey();
                    break;
                case Keys.PageUp:
                    this.HandlePageUpKey();
                    break;
                case Keys.Home:
                    this.HandleHomeKey();
                    break;
                case Keys.End:
                    this.HandleEndKey();
                    break;
            }
        }

        protected virtual void HandlePageDownKey()
        {
            this.ScrollByPage(1);
        }

        protected virtual void HandlePageUpKey()
        {
            this.ScrollByPage(-1);
        }

        protected virtual void HandleHomeKey()
        {
            this.ListElement.HomeEndSelect(this.Items.First);
        }

        protected virtual void HandleEndKey()
        {
            this.ListElement.HomeEndSelect(this.Items.Last);
        }

        #endregion

        public void ScrollByPage(int pageCount)
        {
            this.ListElement.ScrollByPage(pageCount);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadListControlAccessibleObject(this);
        }
    }
}
