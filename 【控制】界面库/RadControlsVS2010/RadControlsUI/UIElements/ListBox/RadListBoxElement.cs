using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Diagnostics;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Design;
using System.Collections;
using System.Globalization;
using System.Drawing.Design;
using Telerik.WinControls.UI.Design;
using System.Drawing;
using Telerik.WinControls.UI.Data;
using Telerik.WinControls.UI.UIElements.ListBox.Data;
using Telerik.WinControls.UI.UIElements.ListBox;

namespace Telerik.WinControls.UI
{
    public delegate void ItemDataBoundEventHandler(object sender, ItemDataBoundEventArgs e);
    public delegate void ItemDataBindingEventHandler(object sender, ItemDataBindingEventArgs e);
    public delegate void RadListBoxSelectionChangeEventHandler(object sender, RadListBoxSelectionChangeEventArgs e);
    
    /// <summary>
    ///     Represents a list box element. The <see cref="RadListBox">RadListBox</see> is a
    ///     simple wrapper for the RadListBoxElement class. The latter implements all logical
    ///     and UI functionality. <see cref="RadListBox">RadListBox</see> act to transfer
    ///     events to and from its RadListBoxElement instance.
    /// </summary>
    //[ComplexBindingProperties("DataSource", "ValueMember")]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
    [Obsolete("This control is obsolete, Use RadListElement instead.")]
    public class RadListBoxElement : RadScrollViewer
    {
        #region Constructors
        /// <summary>Initializes a new instance of the RadListBoxElement class.</summary>
        static RadListBoxElement()
        {
            new Themes.ControlDefault.ListBox().DeserializeTheme();

            SelectedIndexChangedEventKey = new object();
            SelectedValueChangedEventKey = new object();
            SortItemsChangedEventKey = new object();
            SelectedItemChangedEventKey = new object();
        }

        public RadListBoxElement()
        {
            this.CreateDataProvider();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.selectedItems = new RadListBoxItemCollection();

            this.items = new RadListBoxItemOwnerCollection(this);
            this.items.ItemsChanged += this.ItemsChanged;
            this.items.ItemTypes = new Type[] { typeof(RadListBoxItem) };
            this.items.ExcludedTypes = new Type[] { typeof(RadComboBoxItem) };

            this.CanFocus = true;
            //Special case here - we need an entry in the property store so that we receive PropertyChanged for BindingContext
            //TODO: We need more elegant solution here
            if (this.BindingContext != null)
            {
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.stackViewport = new RadVirtualizedStackViewport();
            this.stackViewport.Orientation = System.Windows.Forms.Orientation.Vertical;

            this.stackViewport.EqualChildrenWidth = true;
            //TODO: this is temporary as the RadVirtualizedStackViewport should work when EqualChildrenHeight is false, too.
            this.stackViewport.EqualChildrenHeight = true;
            this.Viewport = stackViewport;
        }

        #endregion

        #region BitState Keys

        internal const ulong IsInitializedStateKey = RadScrollViewerLastStateKey << 1;
        internal const ulong IntegralHeightStateKey = IsInitializedStateKey << 1;
        internal const ulong SelectedCollectionChangeSilentlyStateKey = IntegralHeightStateKey << 1;
        internal const ulong RefreshSelectedIndicesStateKey = SelectedCollectionChangeSilentlyStateKey << 1;
        internal const ulong ClearItemsSilentlyStateKey = RefreshSelectedIndicesStateKey << 1;
        internal const ulong FormattingEnabledStateKey = ClearItemsSilentlyStateKey << 1;
        internal const ulong IsDataSourceSetStateKey = FormattingEnabledStateKey << 1;

        internal const ulong RadListBoxElementLastStateKey = IsDataSourceSetStateKey;

        #endregion

        #region Event Keys
        private static readonly object SelectedIndexChangedEventKey;
        private static readonly object SelectedValueChangedEventKey;
        private static readonly object SortItemsChangedEventKey;
        private static readonly object SelectedItemChangedEventKey;
        #endregion

        #region Fields

        private RadListBoxItemOwnerCollection items;
        private RadListBoxItemCollection selectedItems;
        private RadItemCollection originalItems = new RadItemCollection();

        private RadStackViewport stackViewport;
        private SelectionMode selectionMode = SelectionMode.One;
        
        private RadItem activeItem;
        private int shiftSelectionStart = -1;

        private bool readOnly;

        //Binding		
        private IFormatProvider formatInfo;
        private string formatString = "";
        private int updateCount;
        private int oldIndex = -1;
        private bool suspendEvents = false;

        protected RadListBoxBindingProvider dataProvider = null;

        private object dataSource = null;
        private string valueMember = "";
        private string displayMember = "";

        public static readonly RadProperty CaseSensitiveProperty = RadProperty.Register("CaseSensitive", typeof(bool), typeof(RadListBoxElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        public static readonly RadProperty SortItemsProperty = RadProperty.Register("SortItems", typeof(SortStyle), typeof(RadListBoxElement),
            new RadElementPropertyMetadata(SortStyle.None, ElementPropertyOptions.None));
        #endregion

        #region Properties
        /// <summary>
        ///     Gets a list of items that represents the children of the viewport. It is not a
        ///     problem to manipulate the content of this property even if
        ///     <see cref="Viewport"/> is null. But in order to see the items in this
        ///     collection a viewport must be set (when it is set the content of Items will be set
        ///     as its children).
        /// </summary>
        /// <value>Instance of <see cref="RadItemCollection"/>. It is never null.</value>
        [Browsable(true)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this listbox element.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItemCollection Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// This property must be used only for test purposes. The original items are required for removing sorting and returning to the previous unsorted state.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal RadItemCollection OriginalItems
        {
            get
            {
                return this.originalItems;
            }
        }

        /// <summary>
        /// Enables or disables the ReadOnly mode of RadListBox. The default value is false.
        /// </summary>
        [Obsolete("This property should not be used anymore. To disable the selection, the SelectionMode property must be set to None.")] // Marked obsolete for the Q3 2009 release.
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool ReadOnly
        {
            get
            {
                return this.readOnly;
            }
            set
            {
                if (this.readOnly != value)
                {
                    this.readOnly = value;
                    this.OnNotifyPropertyChanged("ReadOnly");
                }
            }
        }

        public override RadElement Viewport
        {
            get
            {
                return base.Viewport;
            }
            set
            {
                base.Viewport = value;
                ((RadItemVirtualizationCollection)this.items).OwnerViewport = value as IVirtualViewport;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether RadScrollViewer uses UI virtualization.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether RadScrollViewer uses UI virtualization.")]
        public override bool Virtualized
        {
            set
            {
                SetVirtualized(value);
            }
        }

        private void SetVirtualized(bool value)
        {
            if (this.Virtualized != value)
            {
                RadVirtualizedStackViewport viewport = this.stackViewport as RadVirtualizedStackViewport;
                if (viewport != null)
                {
                    if (value)
                    {
                        this.stackViewport.EqualChildrenHeight = true;
                    }
                    else
                    {
                        this.stackViewport.EqualChildrenHeight = this.IntegralHeight;
                    }

                    viewport.Virtualized = value;
                }
                this.OnNotifyPropertyChanged("Virtualized");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list enables selection of list items. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Description("Gets a value indicating whether the list enables selection of list items."),
        DefaultValue(false)]
        protected virtual bool AllowSelection
        {
            get
            {
                return (this.selectionMode != System.Windows.Forms.SelectionMode.None);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sorting is case-sensitive.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether sorting is case-sensitive."),
        DefaultValue(false)]
        public bool CaseSensitive
        {
            get { return (bool)this.GetValue(CaseSensitiveProperty); }
            set { this.SetValue(CaseSensitiveProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control should show or not partial items.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets a value indicating whether the control should show or not partial items."),
        DefaultValue(false)]
        public bool IntegralHeight
        {
            get
            {
                return this.GetBitState(IntegralHeightStateKey);
            }
            set
            {
                this.SetBitState(IntegralHeightStateKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the index specifying the currently selected item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets the index specifying the currently selected item.")]
        public virtual int SelectedIndex
        {
            get
            {
                return GetSelectedIndex();
            }
            set
            {
                SetSelectedIndex(value, true);
            }
        }

        private int GetSelectedIndex()
        {
            int index = -1;
            if (this.SelectedItem is RadListBoxItem)
            {
                index = (this.SelectedItem as RadListBoxItem).Index;
            }

            return this.selectedItems.Count == 0 ? -1 : index;
        }

        internal void SetSelectedIndex(int value, bool fireEvents)
        {
            if ((this.items.Count == 0 && value != -1) || this.SelectedIndex == value)
            {
                return;
            }

            object oldValue = this.SelectedValue;
            this.oldIndex = this.SelectedIndex;

            this.ClearSelectedCollectionsSilently();

            if (IsIndexValid(value))
            {
                this.selectedItems.Add(this.items[value]);
                SetActiveItem(this.selectedItems[0]);
            }

            if (fireEvents)
            {
                DispatchSelectionEvents(true, true, oldValue);
            }

            this.ScrollElementIntoView((RadItem)this.SelectedItem);
        }

        /// <summary>
        /// Gets the text of the currently selected item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets the text of the currently selected item."),
        Browsable(false),
        Bindable(true)]
        public string SelectedText
        {
            get
            {
                RadItem selected = (RadItem)this.SelectedItem;
                if (selected != null)
                {
                    return selected.Text;
                }

                return "";
            }
        }

        /// <summary>
        /// Gets or sets value specifying the currently selected item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets value specifying the currently selected item."),
        Browsable(false),
        Bindable(true)]
        public Object SelectedValue
        {
            get
            {
                return GetSelectedValue();
            }
            set
            {
                SetSelectedValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the currently selected item."),
        Browsable(false),
        Bindable(true)]
        public virtual Object SelectedItem
        {
            get
            {
                return GetSelectedItem();
            }
            set
            {
                SetSelectedItem(value, true);
            }
        }

        object GetSelectedItem()
        {
            return this.selectedItems.Count == 0 ? null : this.selectedItems.First;
        }

        private void SetSelectedItem(object value, bool fireEventsParam)
        {
            bool fireEvents = false;
            object oldValue = this.SelectedValue == null ? null : this.SelectedValue;
            this.oldIndex = this.SelectedIndex;

            if ((value is RadItem))
            {
                RadItem item = (RadItem)value;
                if (this.Items.Contains(item))
                {
                    this.ClearSelectedCollectionsSilently();
                    this.selectedItems.Add(item);
                    this.SetActiveItem(item);
                    fireEvents = true;
                }
            }
            else if (value is string)
            {
                RadItem item = this.FindItemExact((string)value);
                if (item != null)
                {
                    this.ClearSelectedCollectionsSilently();
                    this.selectedItems.Add(item);
                    this.SetActiveItem(item);

                    fireEvents = true;
                }
            }
            else if (value == null)
            {
                this.ClearSelectedCollectionsSilently();
                this.SetActiveItem(null);
                fireEvents = true;
            }

            if (fireEvents && fireEventsParam)
            {
                DispatchSelectionEvents(oldIndex != this.SelectedIndex, true, oldValue);
            }
        }

        private object GetSelectedValue()
        {
            object item = this.SelectedItem;
            if (item is RadListBoxItem)
            {
                return (item as RadListBoxItem).Value;
            }

            return null;
        }

        private void SetSelectedValue(object value)
        {
            object oldValue = this.SelectedValue;
            if (value == null && oldValue == null)
            {
                return;
            }

            if (oldValue != null && oldValue.Equals(value))
            {
                return;
            }

            bool found = false;
            for (int i = 0; i < this.Items.Count; i++)
            {
                RadListBoxItem listBoxItem = this.Items[i] as RadListBoxItem;
                if (listBoxItem.Value != null && listBoxItem.Value.Equals(value))
                {
                    this.SelectedItem = listBoxItem;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                this.SelectedItem = null;
            }
        }

        /// <summary>
        /// Gets a list that contains all currently selected items in the list box.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets a list that contains all currently selected items in the list box."),
        Browsable(false)]
        public RadListBoxItemCollection SelectedItems
        {
            get
            {
                return this.selectedItems;
            }
        }

        /// <summary>
        /// Gets or sets the method in which items are selected in the ListBox.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets the method in which items are selected in the ListBox."),
        DefaultValue(SelectionMode.One)]
        public SelectionMode SelectionMode
        {
            get
            {
                return this.selectionMode;
            }
            set
            {
                this.selectionMode = value;
                if (this.selectionMode == SelectionMode.None)
                {
                    this.SelectedItems.Clear();
                    if (this.activeItem != null)
                    {
                        this.SetActiveItem(null);
                    }
                }
            }
        }

        /// <summary>Gets or sets a value indicating the sort style the of items in the list box.</summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        RadPropertyDefaultValue("SortItems", typeof(RadListBoxElement)),
        Description("Gets or sets a value indicating the sort style the of items in the list box.")]
        public SortStyle SortItems
        {
            get { return (SortStyle)this.GetValue(SortItemsProperty); }
            set { this.SetValue(SortItemsProperty, value); }
        }

        /// <summary>
        /// Gets or searches for the text of the currently selected item in the list box.
        /// </summary>
        [Description("Gets or searches for the text of the currently selected item in the list box.")]
        public override string Text
        {
            get
            {
                if (this.SelectedItem != null)
                    return ((RadItem)this.SelectedItem).Text;
                return base.Text;
            }
            set
            {
                if (this.Text != value)
                {
                    RadItem item = this.FindItemExact(value);
                    if (item != null)
                        this.SelectedItem = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the format-specifier characters that indicate how a value is to be displayed. 
        /// </summary>
        [DefaultValue(""),
        Editor(typeof(FormatStringEditor), typeof(UITypeEditor)),
        MergableProperty(false),
        Description("Gets or sets the format-specifier characters that indicate how a value is to be displayed. ")]
        public string FormatString
        {
            get
            {
                return this.formatString;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (!value.Equals(this.formatString))
                {
                    this.formatString = value;
                    this.OnNotifyPropertyChanged("FormatString");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether formatting is applied to the DisplayMember property.
        /// </summary>
        [Description("Gets or sets a value indicating whether formatting is applied to the DisplayMember property."),
        DefaultValue(false)]
        public bool FormattingEnabled
        {
            get
            {
                return this.GetBitState(FormattingEnabledStateKey);
            }
            set
            {
                this.SetBitState(FormattingEnabledStateKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the IFormatProvider that provides custom formatting behavior.
        /// </summary>
        [Browsable(false),
        Description("Gets or sets the IFormatProvider that provides custom formatting behavior."),
        EditorBrowsable(EditorBrowsableState.Advanced),
        DefaultValue((string)null)]
        public IFormatProvider FormatInfo
        {
            get
            {
                return this.formatInfo;
            }
            set
            {
                if (value != this.formatInfo)
                {
                    this.formatInfo = value;
                    this.OnNotifyPropertyChanged("FormatInfo");
                }
            }
        }
        #endregion

        #region Data Binding

        /// <summary>
        /// Gets or sets the property to display. 
        /// </summary>
        [DefaultValue(""),
        TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
        Description("Gets or sets the property to display."),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        public string DisplayMember
        {
            get
            {
                return this.displayMember;
            }
            set
            {
                SetDisplayMember(value);
            }
        }

        /// <summary>
        /// Gets or sets t he property to use as the actual value for the items. 
        /// </summary>
        [Description("Gets or sets the property to use as the actual value for the items."),
        Category(RadDesignCategory.DataCategory),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember
        {
            get
            {
                return this.valueMember;
            }
            set
            {
                SetValueMember(value);
            }
        }

        /// <summary>
        /// Used internally in the items collection. The collection throws exceptions if it is being modified while this property
        /// is returning true.
        /// </summary> // The reason for this is that manual items can only be inserted before or after the data bound items in order
                       // to avoid currency desynchronization.
        internal bool IsDataSourceSet
        {
            get
            {
                if (this.IsDesignMode)
                {
                    return false; // The items collection must be modifiable during design-time regardless of the DataSource being set or not.
                }

                return this.GetBitState(IsDataSourceSetStateKey);
            }
        }

        /// <summary>
        /// Gets or sets the data source. 
        /// </summary>
        [DefaultValue((string)null),
        AttributeProvider(typeof(IListSource)),
        Description("Gets or sets the data source."),
        Category(RadDesignCategory.DataCategory),
        RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                Bind(value);
            }
        }

        protected virtual void SetValueMember(string value)
        {
            if (this.valueMember == value)
            {
                return;
            }

            this.valueMember = value == null ? "" : value;

            if (string.IsNullOrEmpty(this.displayMember))
            {
                this.DisplayMember = this.valueMember; // The standard listbox/combobox behaves this way. Sad but true :)
            }

            if (this.dataProvider == null || this.dataSource == null)
            {
                return;
            }

            UpdatePropertyMappings();
            this.dataProvider.UpdateItems();
            OnSelectedValueChanged(EventArgs.Empty);
        }

        protected virtual void SetDisplayMember(string value)
        {
            if (this.displayMember == value)
            {
                return;
            }

            this.displayMember = value == null ? "" : value;

            if (this.dataProvider == null)
            {
                return;
            }

            UpdatePropertyMappings();
            this.dataProvider.UpdateItems();
        }

        private void UpdatePropertyMappings()
        {
            BindingMemberInfo displayInfo = new BindingMemberInfo(this.displayMember);
            BindingMemberInfo valueInfo = new BindingMemberInfo(this.valueMember);
            this.dataProvider.UpdatePropertyMappings(ConvertDataValueToString, displayInfo.BindingField, valueInfo.BindingField);
        }

        /// <summary>
        /// Binds RadListBoxElement to the specified data source.
        /// </summary>
        /// <param name="value">The new data source to bind to.</param>
        private void Bind(object value)
        {
            if (value == this.dataSource)
            {
                if (this.dataSource == null)
                {
                    this.BitState[IsDataSourceSetStateKey] = false;
                }
                return;
            }

            this.dataSource = value;

            if (this.dataSource == null)
            {
                this.SelectedIndex = -1;
            }

            this.BitState[IsDataSourceSetStateKey] = this.dataSource != null;

            UpdatePropertyMappings();

            object oldValue = this.SelectedValue;
            this.BeginUpdate();
            this.dataProvider.SetDataSource(value, new BindingMemberInfo(this.displayMember).BindingPath);
            this.EndUpdate();

            if (!this.IsInValidState(true))
            {
                return;
            }

            if (this.dataSource == null)
            {
                this.BitState[IsDataSourceSetStateKey] = false;
                this.DisplayMember = "";
            }
        }

        /// <summary>
        /// Removes all databound items, disposes them, requests new items from the data provider and adds them to the items collection.
        /// </summary>
        public void Rebind()
        {
            if (this.dataProvider == null || !this.IsInValidState(true) || this.dataSource == null)
            {
                return;
            }

            bool oldState = this.GetBitState(IsDataSourceSetStateKey);
            this.BitState[IsDataSourceSetStateKey] = false;

            UpdatePropertyMappings();
            
            this.BeginUpdate();
            this.dataProvider.SetDataSource(null, "");
            this.dataProvider.SetDataSource(this.dataSource, new BindingMemberInfo(this.displayMember).BindingPath);
            this.EndUpdate();

            this.SelectedIndex = GetCorrectPosition();

            this.BitState[IsDataSourceSetStateKey] = oldState;
        }

        bool refreshingItems = false;
        private void RefreshItems()
        {
            if (this.refreshingItems)
            {
                return;
            }

            this.refreshingItems = true;
            this.BeginUpdate();

            this.RemoveDataBoundItems();
            this.CreateDataBoundItems();
            this.refreshingItems = false;
            this.EndUpdate();
        }

        /// <summary>
        /// Gets the correct index to be selected taking into account the position of the currency manager and the offsset of the
        /// data bound items in case there are manual items inserted before them.
        /// </summary>
        /// <returns>The correct index corresponding to the position of the currency manager.</returns>
        protected int GetCorrectPosition()
        {
            if (this.dataProvider == null)
            {
                return this.SelectedIndex;
            }

            int boundItemsBeginning = FindBoundItemsBeginning();
            if (boundItemsBeginning == -1)
            {
                return this.SelectedIndex;
            }

            return this.dataProvider.Position + (boundItemsBeginning == -1 ? 0 : boundItemsBeginning);
        }

        /// <summary>
        /// This method is used to convert data items to string in case there is no DisplayMember set.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        private object ConvertDataValueToString(object value, TypeConverter converter)
        {
            string valueToString = "";
            if (this.FormattingEnabled)
            {
                valueToString = (string)Formatter.FormatObject(value, typeof(string), converter, TypeDescriptor.GetConverter(typeof(string)), this.formatString, this.formatInfo, null, DBNull.Value);
            }
            else
            {
                if (converter != null)
                {
                    if (converter.CanConvertTo(typeof(string)))
                    {
                        valueToString = converter.ConvertToString(value);
                    }
                    else if(value != null)
                    {
                        valueToString = value.ToString();
                    }
                }
                else if(value != null)
                {
                    valueToString = value.ToString();
                }
            }

            return valueToString;
        }

        /// <summary>
        /// Creates an instance the data provider.
        /// </summary>
        protected virtual void InstantiateDataProvider()
        {
            this.dataProvider = new RadListBoxBindingProvider(this);
        }

        /// <summary>
        /// Creates the data provider, subscribes to its events and updates its property mappings.
        /// </summary>
        private void CreateDataProvider()
        {
            if (!this.IsInValidState(false))
            {
                return;
            }
            
            InstantiateDataProvider();
            this.dataProvider.ItemsChanged += dataProvider_ItemsChanged;
            this.dataProvider.PositionChanged += dataProvider_PositionChanged;
            UpdatePropertyMappings();
            
        }

        /// <summary>
        /// Updates the selected index in order to synchronize with the currency manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void dataProvider_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            this.SelectedIndex = GetCorrectPosition();
        }

        /// <summary>
        /// Finds the first non-data bound item.
        /// </summary>
        /// <returns>Returns -1 if there are no data bound items otherwise returns the index of the first non-data bound item.</returns>
        internal int FindBoundItemsEnd()
        {
            int i = FindBoundItemsBeginning();
            if (i == -1)
            {
                return -1;
            }

            while(i < this.items.Count)
            {
                RadListBoxItem item = this.items[i] as RadListBoxItem;
                if (item != null && !item.IsDataBound)
                {
                    return i;
                }
                ++i;
            }

            return this.items.Count == 0 ? 0 : this.items.Count;
        }

        /// <summary>
        /// Finds the beginning of the data bound items since there can be manually inserted items before the data items.
        /// </summary>
        /// <returns>Returns the index of the first data bound item found or -1 if there are none.</returns>
        internal int FindBoundItemsBeginning()
        {
            for (int i = 0; i < this.items.Count; ++i)
            {
                RadListBoxItem item = this.items[i] as RadListBoxItem;
                if (item != null && item.IsDataBound)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes all data bound items and disposes them.
        /// </summary>
        private void RemoveDataBoundItems()
        {
            this.BitState[IsDataSourceSetStateKey] = false;
            List<RadListBoxItem> itemsToRemove = new List<RadListBoxItem>();

            foreach (RadItem i in this.items)
            {
                if (i is RadListBoxItem && (i as RadListBoxItem).IsDataBound)
                {
                    itemsToRemove.Add(i as RadListBoxItem);
                }
            }

            this.suspendEvents = true;
            foreach (RadListBoxItem i in itemsToRemove)
            {
                i.Dispose();
                this.items.Remove(i);
            }
            this.suspendEvents = false;
        }

        /// <summary>
        /// Requests new data items from the data provider and adds them to the items collection.
        /// </summary>
        private void CreateDataBoundItems()
        {
            IEnumerable<RadListBoxItem> items = this.dataProvider.GetItems(null);

            IEnumerator<RadListBoxItem> enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RadListBoxItem newItem = enumerator.Current;

                OnItemDataBinding(new ItemDataBindingEventArgs(newItem, newItem.DataItem));
                this.items.Add(newItem);
                OnItemDataBound(new ItemDataBoundEventArgs(newItem, newItem.DataItem));
            }
        }

        protected virtual void dataProvider_ItemsChanged(object sender, ListChangedEventArgs<RadListBoxItem> e)
        {
            if (this.dataProvider == null || !this.IsInValidState(false) || this.IsDesignMode)
            {
                return;
            }

            bool oldState = this.GetBitState(IsDataSourceSetStateKey);
            this.BitState[IsDataSourceSetStateKey] = false;
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    int startIndex = FindBoundItemsBeginning();
                    this.items.Insert(startIndex == -1 ? 0 : startIndex + e.NewIndex, e.NewItems[0]);
                    break;
                case ListChangedType.ItemDeleted:
                    this.items.Remove(e.NewItems[0]);
                    e.NewItems[0].Dispose();
                    break;
                case ListChangedType.Reset:
                    RefreshItems();
                    break;
            }

            this.SelectedIndex = GetCorrectPosition();

            this.BitState[IsDataSourceSetStateKey] = oldState;
            this.ShiftSelectionStart = this.SelectedIndex;
        }

        /// <summary>
        /// Determines if an integer is inside the bounds of the items collection.
        /// </summary>
        /// <param name="value">The index to check.</param>
        /// <returns>true if the index is not out of bounds, false otherwise.</returns>
        protected bool IsIndexValid(int value)
        {
            return value >= 0 && value < this.items.Count;
        }

        [Category("Data")]
        [Description("Fires after an item from the data source is inserted in the RadListBox")]
        public event ItemDataBoundEventHandler ItemDataBound;

        [Category("Data")]
        [Description("Fires before an item from the data source is inserted in the RadListBox")]
        public event ItemDataBindingEventHandler ItemDataBinding;

        private void RaiseItemDataBound(RadItem item, object dataItem)
        {
            ItemDataBoundEventArgs args = new ItemDataBoundEventArgs(item, dataItem);
            this.OnItemDataBound(args);
        }

        private void RaiseItemDataBinding(RadItem item, object dataItem)
        {
            ItemDataBindingEventArgs args = new ItemDataBindingEventArgs(item, dataItem);
            this.OnItemDataBinding(args);
        }

        protected virtual void OnItemDataBinding(ItemDataBindingEventArgs args)
        {
            if (this.ItemDataBinding != null)
            {
                this.ItemDataBinding(this, args);
            }
        }

        protected virtual void OnItemDataBound(ItemDataBoundEventArgs e)
        {
            if (this.ItemDataBound != null)
            {
                this.ItemDataBound(this, e);
            }
        }

        #endregion

        #region ItemsChanged Handling
        private void ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (this.updateCount > 0)
            {
                return;
            }

            switch (operation)
            {
                case ItemsChangeOperation.Set:
                    OnSet(target);
                    break;
                case ItemsChangeOperation.Inserted:
                    OnItemInserted(target);
                    break;
                case ItemsChangeOperation.Removing:
                    OnItemRemoving(target);
                    break;
                case ItemsChangeOperation.Removed:
                    OnItemRemoved(target);
                    break;
                case ItemsChangeOperation.Clearing:
                    OnItemsClearing();
                    break;
                case ItemsChangeOperation.Cleared:
                    OnItemsCleared();
                    break;
                case ItemsChangeOperation.Sorted:
                    ResetIndices();
                    break;
            }
        }

        private void OnSet(RadItem item)
        {
            if (item is RadListBoxItem)
            {
                RadListBoxItem listBoxItem = item as RadListBoxItem;
                listBoxItem.OwnerElement = this;
                this.originalItems[listBoxItem.Index] = listBoxItem;
            }
        }

        int removedIndex;
        bool lastItemSelected = false;
        bool removedItemWasActive = false;
        private void OnItemRemoving(RadItem item)
        {
            if (item is RadListBoxItem)
            {
                removedIndex = (item as RadListBoxItem).Index;
            }
            else
            {
                removedIndex = this.items.IndexOf(item);
            }
            
            if (item == this.SelectedItem && this.SelectedItem == this.items.Last)
            {
                lastItemSelected = true;
            }

            if (item == this.activeItem)
            {
                removedItemWasActive = true;
            }
        }

        private void OnItemRemoved(RadItem item)
        {
            this.originalItems.Remove(item);

            RadListBoxItem listBoxItem = item as RadListBoxItem;
            if (listBoxItem == null)
            {
                return;
            }
            
            listBoxItem.OwnerElement = null;

            if (this.selectedItems.Contains(item))
            {
                this.selectedItems.Remove(item);
            }
            RadItem[] selectedItemsArray = this.SaveSelectedItems();

            if (IsIndexValid(removedIndex))
            {
                UpdateIndices(removedIndex, -1);
                if (removedItemWasActive)
                {
                    this.SelectedItem = this.items[removedIndex];
                    removedItemWasActive = false;
                }
            }

            SelectNextLastItem();

            this.RestoreSelectedItems(selectedItemsArray);
        }
        
        private void OnItemInserted(RadItem item)
        {
            RadListBoxItem listBoxItem = item as RadListBoxItem;
            if (listBoxItem == null)
            {
                if (this.updateCount == 0)
                {
                    this.originalItems.Insert(this.items.IndexOf(item), item);
                }
                return;
            }

            listBoxItem.Selected = false;
            listBoxItem.Active = false;
            listBoxItem.OwnerElement = this;

            UpdateIndices(listBoxItem.Index + 1, 1);

            this.originalItems.Insert(listBoxItem.Index, listBoxItem);
        }

        private void OnItemsCleared()
        {
            this.SelectedIndex = -1;
            this.originalItems.Clear();
            SetActiveItem(null);
        }

        private void OnItemsClearing()
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i] is RadListBoxItem)
                {
                    ((RadListBoxItem)this.items[i]).OwnerElement = null;
                }
            }
        }

        private void SelectNextLastItem()
        {
            if (this.lastItemSelected)
            {
                this.lastItemSelected = false;
                this.SelectedItem = this.items.Last;
            }
        }

        private void UpdateIndices(int startIndex, int updateValue)
        {
            Debug.Assert(Math.Abs(updateValue) == 1, "Incrementing/Decrementing indices with more than 1 ?!");
            this.oldIndex = this.SelectedIndex;
            for (int i = startIndex; i < this.items.Count; ++i)
            {
                if (this.items[i] is RadListBoxItem)
                {
                    (this.items[i] as RadListBoxItem).Index += updateValue;
                }
            }

            if (this.oldIndex != this.SelectedIndex)
            {
                this.DispatchSelectionEvents(true, false, null);
            }
        }

        private void ResetIndices()
        {
            for (int i = 0; i < this.items.Count; ++i)
            {
                if (this.items[i] is RadListBoxItem)
                {
                    (this.items[i] as RadListBoxItem).Index = i;
                }
            }
        }

        private RadItem[] SaveSelectedItems()
        {
            RadItem[] selectedItemsArray = null;
            if (this.updateCount == 0)
            {
                selectedItemsArray = new RadItem[this.selectedItems.Count];
                this.selectedItems.CopyTo(selectedItemsArray, 0);
            }

            return selectedItemsArray;
        }

        private void RestoreSelectedItems(RadItem[] savedItems)
        {
            if (savedItems == null)
            {
                return;
            }

            foreach (RadItem i in savedItems)
            {
                if (!this.selectedItems.Contains(i))
                {
                    this.selectedItems.Add(i);
                }
            }
        }

        #endregion

        #region Selection Handling

        internal void ProcessMouseSelection(RadItem item, MouseNotification reason)
        {
            int newIndex = GetIndex(item);
            if (!IsIndexValid(newIndex))
            {
                return;
            }

            switch(reason)
            {
                case MouseNotification.MouseDown:
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        return;
                    }
                    this.ShiftSelectionStart = GetIndex(item);
                    break;
                case MouseNotification.MouseDrag:
                    ProcessSelection(newIndex, true, InputType.Mouse);
                    break;
                case MouseNotification.MouseUp:
                    ProcessSelection(newIndex, false, InputType.Mouse);
                    break;
                case MouseNotification.Click:
                    // TODO: Implement once RadItem's Click is fixed. Currently it fires on mouse up.
                    // Handling this case is required for mouse multi-select to work.
                    break;
            }
        }

        private void ProcessSelection(int newIndex, bool onMouseDrag, InputType device)
        {
            switch(this.SelectionMode)
            {
                case SelectionMode.MultiExtended:
                    HandleMultiExtended(newIndex, onMouseDrag, device);
                    break;

                case SelectionMode.MultiSimple:
                    HandleMultiSimple(newIndex, device);
                    break;

                case SelectionMode.One:
                    HandleSelectOne(newIndex);
                    break;
            }
        }

        private void HandleMultiExtended(int newIndex, bool onMouseDrag, InputType device)
        {
            if (Control.ModifierKeys == Keys.Shift || onMouseDrag)
            {
                this.HandleMultiSelectShift(this.shiftSelectionStart, newIndex);
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                HandleMultiSimple(newIndex, device);
            }
            else
            {
                HandleSelectOne(newIndex);
            }
        }
        
        private void HandleMultiSimple(int newIndex, InputType device)
        {
            if (device == InputType.Keyboard)
            {
                SetActiveItem(this.items[newIndex]);
            }
            else
            {
                HandleMouseMultiSimple(newIndex);
            }
        }

        private void HandleMouseMultiSimple(int newIndex)
        {
            if (this.selectedItems.Contains(this.items[newIndex]))
            {
                this.selectedItems.Remove(this.items[newIndex]);
            }
            else
            {
                this.selectedItems.Add(this.items[newIndex]);
                object oldVal = this.SelectedValue;
                DispatchSelectionEvents(true, true, oldVal);
            }

            SetActiveItem(this.items[newIndex]);

            this.ShiftSelectionStart = newIndex;
        }

        private void HandleSelectOne(int newIndex)
        {
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control)
            {
                OnItemClicked();
                return;
            }

            this.SelectedIndex = newIndex;
            this.ShiftSelectionStart = newIndex;
            OnItemClicked();
        }

        private void HandleMultiSelectShift(int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
            {
                Swap(ref startIndex, ref endIndex);
            }

            for (int i = this.selectedItems.Count - 1; i > -1; i--)
            {
                int index = GetIndex(this.selectedItems[i]);
                if (index < startIndex || index > endIndex)
                {
                    this.selectedItems.RemoveAt(i);
                }
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (!this.selectedItems.Contains(this.items[i]))
                {
                    this.selectedItems.Add(this.items[i]);
                }
            }
        }

        private int ShiftSelectionStart
        {
            get
            {
                return this.shiftSelectionStart;
            }

            set
            {
                this.shiftSelectionStart = value;
            }
        }

        public void SetActiveItem(RadItem item)
        {
            if (item == this.activeItem)
                return;

            if (this.activeItem != null)
            {
                this.activeItem.SetValue(RadListBoxItem.ActiveProperty, false);
            }

            this.activeItem = item;

            if (this.activeItem != null)
            {
                this.activeItem.SetValue(RadListBoxItem.ActiveProperty, true);
                this.ScrollElementIntoView(this.activeItem);
            }
        }

        private int FindNextVisibleItem(RadItem item, int direction)
        {
            Debug.Assert(Math.Abs(direction) == 1);

            if (item == null)
            {
                return -1;
            }

            int nextVisibleIndex;
            if (item is RadListBoxItem)
            {
                nextVisibleIndex = (item as RadListBoxItem).Index;
            }
            else
            {
                nextVisibleIndex = this.items.IndexOf(item);
            }

            while (IsIndexValid(nextVisibleIndex) && this.items[nextVisibleIndex].Visibility != ElementVisibility.Visible)
            {
                nextVisibleIndex += direction;
            }

            return nextVisibleIndex;
        }

        private void DispatchSelectionEvents(bool selectedIndex, bool selectedItem, object oldValue)
        {
            if(this.suspendEvents)
            {
                return;
            }

            if (selectedIndex)
            {
                OnSelectedIndexChanged(new SelectedIndexChangedEventArgs(this.oldIndex, this.SelectedIndex));
            }

            if (selectedItem)
            {
                RadItem item = (RadItem)this.SelectedItem;
                OnSelectedItemChanged(new RadListBoxSelectionChangeEventArgs(GetIndex(item), item));
            }

            bool selectedValueFired = false;
            object selectedValue = this.SelectedValue;
            if (selectedValue != null && !selectedValue.Equals(oldValue))
            {
                OnSelectedValueChanged(EventArgs.Empty);
                selectedValueFired = true;
            }

            if (oldValue != null && !oldValue.Equals(selectedValue) && !selectedValueFired)
            {
                OnSelectedValueChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Unselects all items.
        /// </summary>
        public void ClearSelected()
        {
            this.SelectedItems.Clear();
        }

        #region Keyboard selection
        private void ProcessKeyboardSelection(Keys keyCode)
        {
            int dir = GetSelectionDirection(keyCode);
            if (dir == 0)
            {
                return;
            }

            int newIndex = GetIndex(this.activeItem) + dir;
            if (!IsIndexValid(newIndex))
            {
                return;
            }

            newIndex = FindNextVisibleItem(this.items[newIndex], dir);
            if (IsIndexValid(newIndex))
            {
                SetActiveItem(this.items[newIndex]);
                ProcessSelection(newIndex, false, InputType.Keyboard);
            }
        }

        /// <summary>
        /// Selects all items.
        /// </summary>
        public void SelectAll()
        {
            if (this.selectionMode == SelectionMode.One || this.selectionMode == SelectionMode.None)
            {
                return;
            }

            this.selectedItems.BeginUpdate();
            this.selectedItems.Clear();
            foreach (RadItem i in this.items)
            {
                this.selectedItems.Add(i);
            }
            this.selectedItems.EndUpdate();
        }

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
        #endregion

        #endregion

        #region Sorting

        protected void Sort(SortStyle sortStyle)
        {
            this.oldIndex = this.SelectedIndex;

            IComparer comparer = CreateComparer(sortStyle);

            this.stackViewport.Children.Sort(comparer);
            this.Items.Sort(comparer);
            this.selectedItems.Sort(comparer);

            this.BitState[RefreshSelectedIndicesStateKey] = true;
            this.selectedItems.Clear();
            foreach (RadItem item in this.selectedItems)
            {
                this.selectedItems.Add(item);
            }

            for (int i = 0; i < this.items.Count; ++i)
            {
                RadListBoxItem item = this.items[i] as RadListBoxItem;
                if (item != null)
                {
                    item.Index = i;
                }
            }

            this.BitState[RefreshSelectedIndicesStateKey] = false;
            if (this.selectedItems.Count > 0 && oldIndex != this.SelectedIndex)
            {
                DispatchSelectionEvents(true, false, false);
            }

            ScrollToActiveItem();
        }

        protected virtual IComparer CreateComparer(SortStyle sortStyle)
        {
            IComparer comparer = null;
            switch(sortStyle)
            {
                case SortStyle.Ascending:
                    comparer = this.IsDesignMode ? new RadListBoxItemDesignTimeTextComparer(this.CaseSensitive, false) : new RadListBoxItemTextComparer(this.CaseSensitive, false);
                    break;
                case SortStyle.Descending:
                    comparer = this.IsDesignMode ? new RadListBoxItemDesignTimeTextComparer(this.CaseSensitive, true) : new RadListBoxItemTextComparer(this.CaseSensitive, true);
                    break;
                case SortStyle.None:
                    comparer = this.IsDesignMode ? new RadListBoxItemDesignTimeIndexComparer(this.originalItems) : new RadListBoxItemIndexComparer(this.originalItems);
                    break;
            }

            return comparer;
        }

        private void ScrollToActiveItem()
        {
            if (this.activeItem != null)
            {
                this.ScrollElementIntoView(this.activeItem);
            }
            else
            {
                this.ScrollToTop();
            }
        }

        private void SortSelectedItems()
        {
            switch (this.SortItems)
            {
                case SortStyle.Ascending:
                    this.selectedItems.Sort(new RadListBoxItemTextComparer(this.CaseSensitive, false));
                    break;
                case SortStyle.Descending:
                    this.selectedItems.Sort(new RadListBoxItemTextComparer(this.CaseSensitive, true));
                    break;
                case SortStyle.None:
                    this.selectedItems.Sort(new RadListBoxItemIndexComparer(this.originalItems));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Item Visibility
        /// <summary>
        /// Gets the top-most visible item in the list box. When <see cref="Virtualized"/> is false and <see cref="RadScrollLayoutPanel.UsePhysicalScrolling"/>
        /// is true, there is a posibility for the top-most item to be partialy visible. To specify which element is required,
        /// use the <paramref name="fullyVisible"/> parameter.
        /// </summary>
        /// <param name="fullyVisible">when true, the first fully visible item will be returned.</param>
        /// <returns>The top-most visible item in the list box.</returns>
        public RadItem GetTopVisibleItem(bool fullyVisible)
        {
            RadElementCollection children = this.stackViewport.Children;
            if (this.Virtualized)
            {
                if (children.Count > 0)
                    return (RadItem)children[0];

                //this is need for the case when there are items in Items collection but they are not transferred to the Children colection
                if (this.Items.Count > 0)
                    return this.Items[0];
            }
            else
            {
                if (this.UsePhysicalScrolling)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        int topPixelPosition = -(int)Math.Round(this.stackViewport.PositionOffset.Height);
                        if (fullyVisible)
                        {
                            if (children[i].BoundingRectangle.Top > topPixelPosition)
                                return (RadItem)children[i];
                        }
                        else
                        {
                            if (children[i].BoundingRectangle.Bottom > topPixelPosition)
                                return (RadItem)children[i];
                        }
                    }
                }
                else
                {
                    int itemIndex = Math.Min(this.Value.Y, children.Count - 1);
                    if (children.Count > 0)
                        return (RadItem)children[itemIndex];
                }
            }
            return null;
        }

        internal RadListBoxItem GetNextVisibleItem(int index)
        {
            RadListBoxItem item = (RadListBoxItem)this.Items[index];

            while (index < this.Items.Count)
            {
                index++;

                if (index < this.Items.Count)
                {
                    item = (RadListBoxItem)this.Items[index];
                    if (item.Visibility == ElementVisibility.Visible)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        internal RadListBoxItem GetPrevVisibleItem(int index)
        {
            RadListBoxItem item = (RadListBoxItem)this.Items[index];

            while (index > 0)
            {
                index--;

                if (index >= 0)
                {
                    item = (RadListBoxItem)this.Items[index];
                    if (item.Visibility == ElementVisibility.Visible)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

#endregion

        #region Property Changes
        protected override void OnPropertyChanging(RadPropertyChangingEventArgs args)
        {
            if (args.Property == RadListBoxElement.SortItemsProperty)
            {
                if (this.DataSource != null && (SortStyle)args.NewValue != SortStyle.None)
                {
                    throw new InvalidOperationException("ListBox that has a DataSource set cannot be sorted. Sort the data using the underlying data model.");
                }
            }
            base.OnPropertyChanging(args);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadObject.BindingContextProperty)
            {
                if (this.dataProvider != null)
                {
                    this.dataProvider.BindingContext = (BindingContext)e.NewValue;
                }
            }

            if (e.Property == RadListBoxElement.SortItemsProperty)
            {
                if ((SortStyle)e.OldValue == SortStyle.None)
                {
                    this.originalItems.Clear();
                    this.originalItems.AddRange(this.Items.ToArray());
                }
                this.Sort((SortStyle)e.NewValue);
                this.OnSortItemsChanged(EventArgs.Empty);
            }
            else if (e.Property == RadListBoxElement.CaseSensitiveProperty)
            {
                this.Sort(this.SortItems);
            }

            else if (e.Property == RadItem.IsFocusedProperty)
            {
                if ((bool)e.NewValue && this.Items.Count > 0 && this.activeItem == null)
                {
                    if (this.SelectedIndex > -1 && this.SelectedItem is RadItem)
                    {
                        this.activeItem = (RadItem)this.SelectedItem;
                    }
                    else
                    {
                        this.activeItem = this.Items[0];
                    }
                }
            }
        }

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "CaseSensitive":
                    break;
                case "FormatInfo":
                    break;
                case "FormatString":
                    break;
                case "FormattingEnabled":
                    break;
                case "SelectedValue":
                    this.OnSelectedValueChanged(EventArgs.Empty);
                    break;
                case "ReadOnly":

                    break;
            }
            base.OnNotifyPropertyChanged(propertyName);
        }

        #endregion

        #region Events

        /// <summary>Fires when the index is changed.</summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the SelectedIndex property has changed.")]
        public event EventHandler SelectedIndexChanged
        {
            add
            {
                this.Events.AddHandler(RadListBoxElement.SelectedIndexChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadListBoxElement.SelectedIndexChangedEventKey, value);
            }
        }

        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description]
        public event RadListBoxSelectionChangeEventHandler SelectedItemChanged
        {
            add
            {
                this.Events.AddHandler(RadListBoxElement.SelectedItemChangedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(RadListBoxElement.SelectedItemChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the SelectedIndexChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSelectedIndexChanged(SelectedIndexChangedEventArgs e)
        {
            if (this.dataProvider != null)
            {
                int boundItemsBeg = FindBoundItemsBeginning();
                this.dataProvider.Position = e.NewIndex - (boundItemsBeg == -1 ? 0 : boundItemsBeg);
            }
            EventHandler handler1 = (EventHandler)this.Events[RadListBoxElement.SelectedIndexChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Used to signal the popup form in RadComboBox that it should close itself.
        /// </summary>
        internal event EventHandler ItemClicked;

        protected virtual void OnItemClicked()
        {
            if(this.ItemClicked != null)
            {
                this.ItemClicked(this, new EventArgs());
            }
        }

        /// <summary>Fires when the selected value is changed.</summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the SelectedValue property has changed.")]
        public event EventHandler SelectedValueChanged
        {
            add
            {
                this.Events.AddHandler(RadListBoxElement.SelectedValueChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadListBoxElement.SelectedValueChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the SelectedValueChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadListBoxElement.SelectedValueChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the SortItems property has changed.
        /// </summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the SortItems property has changed.")]
        public event EventHandler SortItemsChanged
        {
            add
            {
                this.Events.AddHandler(RadListBoxElement.SortItemsChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadListBoxElement.SortItemsChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the SortItemsChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSortItemsChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadListBoxElement.SortItemsChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void OnSelectedItemChanged(RadListBoxSelectionChangeEventArgs args)
        {
            RadListBoxSelectionChangeEventHandler handler = this.Events[RadListBoxElement.SelectedItemChangedEventKey] as RadListBoxSelectionChangeEventHandler;
			if (handler != null)
			{
				handler(this, args);
			}
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Finds the first item in the list box that starts with the specified string. 
        /// </summary>
        /// <param name="startsWith">The string to search for.</param>
        /// <returns>The zero-based index of the first item found; returns null if no match is found.</returns>
        public RadItem FindItem(string startsWith)
        {
            if (!string.IsNullOrEmpty(startsWith))
            {
                foreach (RadItem item in this.Items)
                {
                    if (item.Text.StartsWith(startsWith, !this.CaseSensitive, System.Globalization.CultureInfo.InvariantCulture))
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds all items in the list box that starts with the specified string. 
        /// </summary>
        /// <param name="startsWith">The string to search for.</param>
        /// <returns>Collection of items that match the criteria.</returns>
        public RadItemCollection FindAllItems(string startsWith)
        {
            RadItemCollection col = new RadItemCollection();
            if (!string.IsNullOrEmpty(startsWith))
            {
                foreach (RadItem item in this.Items)
                {
                    if (item.Text.StartsWith(startsWith, !this.CaseSensitive, System.Globalization.CultureInfo.InvariantCulture))
                        col.Add(item);
                }
            }
            return col;
        }

        /// <summary>
        /// Finds the first item in the list box that matches the specified string.
        /// </summary>
        /// <param name="text">The string to search for.</param>
        /// <returns>The zero-based index of the first item found; returns null if no match is found.</returns>
        public RadItem FindItemExact(string text)
        {
            foreach (RadItem item in this.Items)
            {
                if (item.Text.Equals(text, this.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase))
                    return item;
            }

            return null;
        }

        /// <summary>
        /// Call the GetItemHeight member function to retrieve the height of list items in a list box.
        /// </summary>
        /// <param name="index">Specifies the item of the list box whose height is to be retrieved.</param>
        /// <returns></returns>
        public int GetItemHeight(int index)
        {
            if (index < 0 || index >= this.Items.Count)
                return 0;

            int height = 0;

            RadItem item = this.Items[index];
            RadElementCollection children = this.stackViewport.Children;

            if (children.Contains(item))
            {
                height = item.FullSize.Height;
            }
            else
            {
                if (this.stackViewport.EqualChildrenHeight && children.Count > 0)
                {
                    height = children[0].FullSize.Height;
                }
                else
                {
                    if (item.GetBitState(NeverMeasuredStateKey))
                    {
                        item.Measure(new SizeF(float.PositiveInfinity, float.PositiveInfinity));
                        height = (int)Math.Round(item.DesiredSize.Height);
                    }
                    else
                    {
                        //height = item.FullSize.Height;
                        height = (int)Math.Round(item.DesiredSize.Height);
                    }
                }
            }

            return height;
        }

        /// <summary>
        /// Returns the text representation of the specified item. 
        /// </summary>
        /// <param name="index">Specifies the index of the item of the list box whose text is to be retrieved.</param>
        /// <returns></returns>
        [Obsolete("This method is obsolete and will be removed for the next release.")] // Skarlatov 25/09/2009
        public string GetItemText(int index)
        {
            RadItem item = this.Items[index];
            return item.Text;
        }

        /// <summary>
        /// Selects or clears the selection for the specified item in the list box.
        /// </summary>
        /// <param name="index">The zero-based index of the item in the list box to select or clear the selection for. </param>
        /// <param name="value">true to select the specified item; otherwise, false.</param>
        public void SetSelected(int index, bool value)
        {
            switch (this.selectionMode)
            {
                case SelectionMode.MultiExtended:
                case SelectionMode.MultiSimple:
                    if (value)
                        this.SelectedItems.Add(this.items[index]);
                    else
                        this.SelectedItems.Remove(this.items[index]);
                    break;
                case SelectionMode.One:
                    if (value)
                    {
                        this.ClearSelectedCollectionsSilently();
                        this.SelectedItems.Add(this.items[index]);
                    }
                    else
                        this.SelectedItems.Remove(this.items[index]);
                    break;
                case SelectionMode.None:
                    throw new InvalidOperationException("SelectionMode property is set to None");
                default:
                    break;
            }
        }

        /// <summary>Initiates batch update of the items.</summary>
        public void BeginUpdate()
        {
            if (!this.IsInValidState(true))
            {
                return;
            }

            if (this.updateCount == 0)
            {
                // Store initial state
            }

            this.updateCount++;

            if (this.UseNewLayoutSystem)
            {
                RadVirtualizedStackViewport viewport = this.stackViewport as RadVirtualizedStackViewport;
                if (viewport != null)
                    viewport.BeginUpdate();
            }
            else
            {
                this.SuspendLayout();
            }
        }

        /// <summary>Ends batch update of the items.</summary>
        public void EndUpdate()
        {
            if (!this.IsInValidState(true))
            {
                return;
            }

            if (this.updateCount > 0)
                this.updateCount--;

            if (this.UseNewLayoutSystem)
            {
                if (this.updateCount == 0)
                {
                    // Reset the list box and restore the initial state
                    this.originalItems.Clear();
                    this.originalItems.AddRange(this.Items);

                    if (this.SortItems != SortStyle.None)
                    {
                        this.Sort(this.SortItems);
                    }

                    for (int i = 0; i < this.items.Count; ++i)
                    {
                        RadListBoxItem item = this.items[i] as RadListBoxItem;
                        if (item != null)
                        {
                            item.Index = i;
                            item.OwnerElement = this;
                        }
                    }
                }

                RadVirtualizedStackViewport viewport = this.stackViewport as RadVirtualizedStackViewport;
                if (viewport != null)
                    viewport.EndUpdate();
            }
            else
            {
                this.ResumeLayout(false);
                //TODO: This is fix for the layout of the listbox items - it should be forced when the sum of the height of the items
                // are less than the height of the scrollviewer and the vert. scrollbar doesn't appear thus performing layout is skipped
                this.LayoutEngine.InvalidateLayout();
                this.LayoutEngine.PerformParentLayout();
            }
        }
        #endregion

        #region Misc
        private int GetIndex(RadItem item)
        {
            if (item is RadListBoxItem)
            {
                return (item as RadListBoxItem).Index;
            }
            else
            {
                return this.items.IndexOf(item);
            }
        }

        internal void EndInit()
        {
            this.originalItems.AddRange(this.Items.ToArray());
            if (this.SortItems != SortStyle.None)
                this.Sort(this.SortItems);
            this.BitState[IsInitializedStateKey] = true;
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == IntegralHeightStateKey)
            {
                if (!this.Virtualized || newValue)
                {
                    this.stackViewport.EqualChildrenHeight = newValue;
                }
            }
            else if (key == FormattingEnabledStateKey)
            {
                this.OnNotifyPropertyChanged("FormattingEnabled");
            }
        }

        private void Swap(ref int a, ref int b)
        {
            int t = a;
            a = b;
            b = t;
        }

        internal void ProcessItemTextChanged(EventArgs e)
        {
            if (this.SortItems != SortStyle.None)
            {
                this.Sort(this.SortItems);
            }
        }

        private void ClearSelectedCollectionsSilently()
        {
            this.BitState[SelectedCollectionChangeSilentlyStateKey] = true;
            this.selectedItems.Clear();
            this.BitState[SelectedCollectionChangeSilentlyStateKey] = false;
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            if (this.Items.Count == 0 || !this.IsInValidState(true))
            {
                return;
            }

            if (this != sender && args.RoutedEvent == RadElement.MouseDownEvent)
            {
                this.Focus(true);
            }

            if ((args.RoutedEvent == RadItem.KeyDownEvent) && (args.OriginalEventArgs is KeyEventArgs))
            {
                KeyEventArgs keyArgs = (KeyEventArgs)args.OriginalEventArgs;

                OnSpace(keyArgs.KeyCode);

                ProcessKeyboardSelection(keyArgs.KeyCode);

                keyArgs.Handled = true;
            }

            base.OnBubbleEvent(sender, args);
        }
        
        private void OnSpace(Keys keyCode)
        {
            if (keyCode == Keys.Space && this.selectionMode == SelectionMode.MultiSimple)
            {
                if (this.activeItem != null)
                {
                    this.ProcessSelection(GetIndex(this.activeItem), false, InputType.Mouse);
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            this.DataSource = null;
            this.dataProvider.ItemsChanged -= dataProvider_ItemsChanged;
            this.dataProvider.PositionChanged -= dataProvider_PositionChanged;
            this.dataProvider = null;
            base.DisposeManagedResources();
        }
        #endregion
    }
}
