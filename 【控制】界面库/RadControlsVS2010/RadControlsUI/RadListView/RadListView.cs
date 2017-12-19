using System.ComponentModel;
using System.Windows.Forms;
using System;
using System.Drawing;
using Telerik.WinControls.Data;
using System.Drawing.Design;
using System.Collections.Generic;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Displays a flat collection of labeled items, each represented by a <see cref="ListViewDataItem">ListViewDataItem</see>. 
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.DataControlsGroup)]
    [ToolboxItem(true)]
    [Description("Displays a flat collection of labeled items, each represented by a ListViewDataItem.")]
    [DefaultProperty("Items"), DefaultEvent("SelectedItemChanged")]
    [ComplexBindingProperties("DataSource", "DataMember")]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "CurrentItem")]
    [Designer(DesignerConsts.RadListViewDesignerString)]
    public class RadListView : RadControl
    {
        #region Events

        /// <summary>
        /// Occurs when the BindingContext has changed.
        /// </summary>
        public event EventHandler BindingContextChanged
        {
            add
            {
                this.listViewElement.BindingContextChanged += value;
            }
            remove
            {
                this.listViewElement.BindingContextChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the procces of binding <see cref="RadListViewElement"/> to a data source has finished
        /// </summary>
        public event EventHandler BindingCompleted
        {
            add
            {
                this.listViewElement.BindingCompleted += value;
            }
            remove
            {
                this.listViewElement.BindingCompleted -= value;
            }
        }

        /// <summary>
        /// Occurs when the content of the SelectedItems collection has changed.
        /// </summary>
        public event EventHandler SelectedItemsChanged
        {
            add
            {
                this.listViewElement.SelectedItemsChanged += value;
            }
            remove
            {
                this.listViewElement.SelectedItemsChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        public event ListViewItemEventHandler SelectedItemChanged
        {
            add
            {
                this.listViewElement.SelectedItemChanged += value;
            }
            remove
            {
                this.listViewElement.SelectedItemChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the ViewType of <see cref="RadListView">RadListView</see> is changed.
        /// </summary>
        public event EventHandler ViewTypeChanged
        {
            add
            {
                this.ListViewElement.ViewTypeChanged += value;
            }
            remove
            {
                this.ListViewElement.ViewTypeChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the ViewType of <see cref="RadListView">RadListView</see> is about to change. Cancelable.
        /// </summary>
        public event ViewTypeChangingEventHandler ViewTypeChanging
        {
            add
            {
                this.ListViewElement.ViewTypeChanging += value;
            }
            remove
            {
                this.ListViewElement.ViewTypeChanging -= value;
            }
        }

        /// <summary>
        /// Occurs when the user presses a mouse button over a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemMouseEventHandler ItemMouseDown
        {
            add
            {
                this.listViewElement.ItemMouseDown += value;
            }
            remove
            {
                this.listViewElement.ItemMouseDown -= value;
            }
        }

        /// <summary>
        /// Occurs when the user presses a mouse button over a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemMouseEventHandler ItemMouseUp
        {
            add
            {
                this.listViewElement.ItemMouseUp += value;
            }
            remove
            {
                this.listViewElement.ItemMouseUp -= value;
            }
        }

        /// <summary>
        /// Occurs when the user moves the mouse over a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemMouseEventHandler ItemMouseMove
        {
            add
            {
                this.listViewElement.ItemMouseMove += value;
            }
            remove
            {
                this.listViewElement.ItemMouseMove -= value;
            }
        }

        /// <summary>
        /// Occurs when the user hovers a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseHover
        {
            add
            {
                this.listViewElement.ItemMouseHover += value;
            }
            remove
            {
                this.listViewElement.ItemMouseHover -= value;
            }
        }

        /// <summary>
        /// Occurs when the mouse pointer enters a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseEnter
        {
            add
            {
                this.listViewElement.ItemMouseEnter += value;
            }
            remove
            {
                this.listViewElement.ItemMouseEnter -= value;
            }
        }

        /// <summary>
        /// Occurs when the mouse pointer leaves a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseLeave
        {
            add
            {
                this.listViewElement.ItemMouseLeave += value;
            }
            remove
            {
                this.listViewElement.ItemMouseLeave -= value;
            }
        }

        /// <summary>
        /// Occurs when the user clicks a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseClick
        {
            add
            {
                this.listViewElement.ItemMouseClick += value;
            }
            remove
            {
                this.listViewElement.ItemMouseClick -= value;
            }
        }

        /// <summary>
        /// Occurs when the user double-clicks a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemMouseDoubleClick
        {
            add
            {
                this.listViewElement.ItemMouseDoubleClick += value;
            }
            remove
            {
                this.listViewElement.ItemMouseDoubleClick -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> is about to be checked. Cancelable.
        /// </summary>
        public event ListViewItemCancelEventHandler ItemCheckedChanging
        {
            add
            {
                this.listViewElement.ItemCheckedChanging += value;
            }
            remove
            {
                this.listViewElement.ItemCheckedChanging -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> is checked.
        /// </summary>
        public event ListViewItemEventHandler ItemCheckedChanged
        {
            add
            {
                this.listViewElement.ItemCheckedChanged += value;
            }
            remove
            {
                this.listViewElement.ItemCheckedChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> changes its state and needs to be formatted.
        /// </summary>
        public event ListViewVisualItemEventHandler VisualItemFormatting
        {
            add
            {
                this.listViewElement.VisualItemFormatting += value;
            }
            remove
            {
                this.listViewElement.VisualItemFormatting -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> needs to be created.
        /// </summary>
        public event ListViewItemCreatingEventHandler ItemCreating
        {
            add
            {
                this.listViewElement.ItemCreating += value;
            }
            remove
            {
                this.listViewElement.ItemCreating -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="BaseListViewVisualItem">BaseListViewVisualItem</see> needs to be created;
        /// </summary>
        public event ListViewVisualItemCreatingEventHandler VisualItemCreating
        {
            add
            {
                this.listViewElement.VisualItemCreating += value;
            }
            remove
            {
                this.listViewElement.VisualItemCreating -= value;
            }
        }

        /// <summary>
        /// Occurs when a DetailsView cell needs to be formatted.
        /// </summary>
        public event ListViewCellFormattingEventHandler CellFormatting
        {
            add
            {
                this.listViewElement.CellFormatting += value;
            }
            remove
            {
                this.listViewElement.CellFormatting -= value;
            }
        }

        /// <summary>
        /// Occurs when a data-bound item is being attached to a <see cref="ListViewDataItem">ListViewDataItem</see>.
        /// </summary>
        public event ListViewItemEventHandler ItemDataBound
        {
            add
            {
                this.listViewElement.ItemDataBound += value;
            }
            remove
            {
                this.listViewElement.ItemDataBound -= value;
            }
        }

        /// <summary>
        /// Occurs when the CurrentItem property is changed.
        /// </summary>
        public event ListViewItemEventHandler CurrentItemChanged
        {
            add
            {
                this.listViewElement.CurrentItemChanged += value;
            }
            remove
            {
                this.listViewElement.CurrentItemChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the CurrentItem property is about to change. Cancelable.
        /// </summary>
        public event ListViewItemChangingEventHandler CurrentItemChanging
        {
            add
            {
                this.listViewElement.CurrentItemChanging += value;
            }
            remove
            {
                this.listViewElement.CurrentItemChanging -= value;
            }
        }

        /// <summary>
        /// Occurs when an editor is required.
        /// </summary>
        public event ListViewItemEditorRequiredEventHandler EditorRequired
        {
            add
            {
                this.listViewElement.EditorRequired += value;
            }
            remove
            {
                this.listViewElement.EditorRequired -= value;
            }
        }

        /// <summary>
        /// Occurs when an edit operation is about to begin. Cancelable.
        /// </summary>
        public event ListViewItemEditingEventHandler ItemEditing
        {
            add
            {
                this.listViewElement.ItemEditing += value;
            }
            remove
            {
                this.listViewElement.ItemEditing -= value;
            }
        }

        /// <summary>
        /// Occurs when an editor is initialized.
        /// </summary>
        public event ListViewItemEditorInitializedEventHandler EditorInitialized
        {
            add
            {
                this.listViewElement.EditorInitialized += value;
            }
            remove
            {
                this.listViewElement.EditorInitialized -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="ListViewDataItem">ListViewDataItem</see> is edited.
        /// </summary>
        public event ListViewItemEditedEventHandler ItemEdited
        {
            add
            {
                this.listViewElement.ItemEdited += value;
            }
            remove
            {
                this.listViewElement.ItemEdited -= value;
            }
        }

        /// <summary>
        /// Fires when a validation error occurs.
        /// </summary>
        public event EventHandler ValidationError
        {
            add
            {
                this.listViewElement.ValidationError += value;
            }
            remove
            {
                this.listViewElement.ValidationError -= value;
            }
        }

        /// <summary>
        /// Occurs when an edit operation needs to be validated.
        /// </summary>
        public event ListViewItemValidatingEventHandler ItemValidating
        {
            add
            {
                this.listViewElement.ItemValidating += value;
            }
            remove
            {
                this.listViewElement.ItemValidating -= value;
            }
        }

        /// <summary>
        /// Occurs when the value of a <see cref="ListViewDataItem">ListViewDataItem</see> is changed.
        /// </summary>
        public event ListViewItemValueChangedEventHandler ItemValueChanged
        {
            add
            {
                this.listViewElement.ItemValueChanged += value;
            }
            remove
            {
                this.listViewElement.ItemValueChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the value of a <see cref="ListViewDataItem">ListViewDataItem</see> is about to change. Cancelable.
        /// </summary>
        public event ListViewItemValueChangingEventHandler ItemValueChanging
        {
            add
            {
                this.listViewElement.ItemValueChanging += value;
            }
            remove
            {
                this.listViewElement.ItemValueChanging -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="ListViewDetailColumn"/> needs to be created.
        /// </summary>
        public event ListViewColumnCreatingEventHandler ColumnCreating
        {
            add
            {
                this.listViewElement.ColumnCreating += value;
            }
            remove
            {
                this.listViewElement.ColumnCreating -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="DetailListViewCellElement"/> needs to be created.
        /// </summary>
        public event ListViewCellElementCreatingEventHandler CellCreating
        {
            add
            {
                this.listViewElement.CellCreating += value;
            }
            remove
            {
                this.listViewElement.CellCreating -= value;
            }
        }

        /// <summary>
        /// Occurs when an item is about to be removed using the Delete key. Cancelable.
        /// </summary>
        public event ListViewItemCancelEventHandler ItemRemoving
        {
            add
            {
                this.listViewElement.ItemRemoving += value;
            }
            remove
            {
                this.listViewElement.ItemRemoving -= value;
            }
        }

        /// <summary>
        /// Occurs when an item is removed using the Delete key.
        /// </summary>
        public event ListViewItemEventHandler ItemRemoved
        {
            add
            {
                this.listViewElement.ItemRemoved += value;
            }
            remove
            {
                this.listViewElement.ItemRemoved -= value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="RadListView"/>.
        /// </summary>
        public RadListView()
        {
            this.Initialized += new EventHandler(RadListView_Initialized);
        } 

        #endregion

        #region Fields

        private Dictionary<string, object> initValues = new Dictionary<string, object>();
        private RadListViewElement listViewElement;
 
        #endregion
         
        #region Initialization

        /// <summary>
        /// Executed on EndInit() method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void RadListView_Initialized(object sender, EventArgs e)
        {
            this.Initialized -= new EventHandler(RadListView_Initialized);

            if (this.initValues.ContainsKey("AllowArbitraryItemHeight"))
            {
                this.AllowArbitraryItemHeight = (bool)this.initValues["AllowArbitraryItemHeight"];
            }

            if (this.initValues.ContainsKey("AllowArbitraryItemWidth"))
            {
                this.AllowArbitraryItemWidth = (bool)this.initValues["AllowArbitraryItemWidth"];
            }

            if (this.initValues.ContainsKey("FullRowSelect"))
            {
                this.FullRowSelect = (bool)this.initValues["FullRowSelect"];
            }

            if (this.initValues.ContainsKey("ItemSize"))
            {
                this.ItemSize = (Size)this.initValues["ItemSize"];
            }

            if (this.initValues.ContainsKey("GroupItemSize"))
            {
                this.GroupItemSize = (Size)this.initValues["GroupItemSize"];
            }

            if (this.initValues.ContainsKey("GroupIndent"))
            {
                this.GroupIndent = (int)this.initValues["GroupIndent"];
            }

            if (this.initValues.ContainsKey("ItemSpacing"))
            {
                this.ItemSpacing = (int)this.initValues["ItemSpacing"];
            }

            if (this.initValues.ContainsKey("DataSource"))
            {
                this.DataSource = this.initValues["DataSource"];
            }

            if (this.initValues.ContainsKey("DisplayMember"))
            {
                this.DisplayMember = (string)this.initValues["DisplayMember"];
            }

            if (this.initValues.ContainsKey("ValueMember"))
            {
                this.ValueMember = (string)this.initValues["ValueMember"];
            }
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            listViewElement = this.CreateListViewElement();
            this.RootElement.Children.Add(listViewElement);
        }

        protected virtual RadListViewElement CreateListViewElement()
        {
            return new RadListViewElement();
        }

        #endregion
       
        #region Properties

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
                return this.listViewElement.EnableKineticScrolling;
            }
            set
            {
                this.listViewElement.EnableKineticScrolling = value;
            }
        }

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
            get 
            {
                return this.listViewElement.HotTracking; 
            }
            set
            {
                this.listViewElement.HotTracking = value;
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
                return this.listViewElement.EnableColumnSort;
            }
            set
            {
                this.listViewElement.EnableColumnSort = value;
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
                return this.listViewElement.ItemSize;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["ItemSize"] = value;
                }
                else
                {
                    this.listViewElement.ItemSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default item size.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "0,0")]
        [Description("Gets or sets the default item size.")]
        [Category(RadDesignCategory.LayoutCategory)]
        public Size GroupItemSize
        {
            get
            {
                return this.listViewElement.GroupItemSize;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["GroupItemSize"] = value;
                }
                else
                {
                    this.listViewElement.GroupItemSize = value;
                }
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
                return this.listViewElement.GroupIndent;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["GroupIndent"] = value;
                }
                else
                {
                    this.listViewElement.GroupIndent = value;
                }
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
                return this.listViewElement.ItemSpacing;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["ItemSpacing"] = value;
                }
                else
                {
                    this.listViewElement.ItemSpacing = value;
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
                return this.listViewElement.FilterDescriptors;
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
                return this.listViewElement.IsDataBound;
            }
        }

        /// <summary>
        /// Gets a collection containg the groups of the RadListView.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.ListViewGroupCollectionDesignerString, typeof(UITypeEditor))]
        [Description("Gets a collection containg the groups of the RadListView.")]
        [Category(RadDesignCategory.DataCategory)]
        public ListViewDataItemGroupCollection Groups
        {
            get
            {
                return this.listViewElement.Groups;
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
                return this.listViewElement.ValueMember;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["ValueMember"] = value;
                }
                else
                {
                    this.listViewElement.ValueMember = value;
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
                return this.listViewElement.DisplayMember;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["DisplayMember"] = value;
                }
                else
                {
                    this.listViewElement.DisplayMember = value;
                }
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
                return this.listViewElement.EnableSorting;
            }
            set
            {
                this.listViewElement.EnableSorting = value;
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
                return this.listViewElement.EnableFiltering;
            }
            set
            {
                this.listViewElement.EnableFiltering = value;
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
                return this.listViewElement.EnableGrouping;
            }
            set
            {
                this.listViewElement.EnableGrouping = value;
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
                return this.listViewElement.EnableCustomGrouping;
            }
            set
            {
                this.listViewElement.EnableCustomGrouping = value;
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
                return this.listViewElement.SortDescriptors;
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
                return this.listViewElement.GroupDescriptors;
            }
        }

        /// <summary>
        /// Gets or sets the data source of a RadListView.
        /// </summary>
        [Browsable(true)]
        [AttributeProvider(typeof(IListSource))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets the data source of a RadListView.")]
        public object DataSource
        {
            get
            {
                return this.listViewElement.DataSource;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["DataSource"] = value;
                }
                else
                {
                    this.listViewElement.DataSource = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the <see cref="RadListView"/> is displaying data. 
        /// </summary>
        [Browsable(true), Category("Data"), DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Description("Gets or sets the name of the list or table in the data source for which the RadListView is displaying data. ")]
        public string DataMember
        {
            get { return this.listViewElement.DataMember; }
            set
            {
                if (this.listViewElement.DataMember != value)
                {
                    this.listViewElement.DataMember = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the selected item.")]
        public ListViewDataItem SelectedItem
        {
            get
            {
                return this.listViewElement.SelectedItem;
            }
            set
            {
                this.listViewElement.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the index of the selected item.")]
        public int SelectedIndex
        {
            get
            {
                return this.listViewElement.SelectedIndex;
            }
            set
            {
                this.listViewElement.SelectedIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the current item.")]
        public ListViewDataItem CurrentItem
        {
            get
            {
                return this.listViewElement.CurrentItem;
            }
            set
            {
                this.listViewElement.CurrentItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the current column in Details View.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the current column in Details View.")] 
        public ListViewDetailColumn CurrentColumn
        {
            get
            {
                return this.listViewElement.CurrentColumn;
            }
            set
            {
                this.listViewElement.CurrentColumn = value;
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
                return this.listViewElement.IsEditing;
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
                return this.listViewElement.Columns;
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="ListViewDataItem">ListViewDataItem</see> object which represent the items in RadListView.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.ListViewItemCollectionDesignerString, typeof(UITypeEditor))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets or sets a collection of ListViewDataItem object which represent the items in RadListView.")]
        public ListViewDataItemCollection Items
        {
            get
            {
                return this.listViewElement.Items;
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
                return this.listViewElement.ShowColumnHeaders;
            }
            set
            {
                this.listViewElement.ShowColumnHeaders = value;
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
                return this.listViewElement.ShowGroups;
            }
            set
            {
                this.listViewElement.ShowGroups = value;
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
                return this.listViewElement.SelectedItems;
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
                return this.listViewElement.CheckedItems;
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
                return this.listViewElement.ShowCheckBoxes;
            }
            set
            {
                this.listViewElement.ShowCheckBoxes = value;
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
                return this.listViewElement.AllowColumnResize;
            }
            set
            {
                this.listViewElement.AllowColumnResize = value;
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
                return this.listViewElement.AllowColumnReorder ;
            }
            set
            {
                this.listViewElement.AllowColumnReorder = value;
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
                return this.listViewElement.FullRowSelect;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["FullRowSelect"] = value;
                }
                else
                {
                    this.listViewElement.FullRowSelect = value;
                }
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
                return this.listViewElement.AllowArbitraryItemWidth;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["AllowArbitraryItemWidth"] = value;
                }
                else
                {
                    this.listViewElement.AllowArbitraryItemWidth = value;
                }
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
                return this.listViewElement.AllowArbitraryItemHeight;
            }
            set
            {
                if (this.IsInitializing)
                {
                    this.initValues["AllowArbitraryItemHeight"] = value;
                }
                else
                {
                    this.listViewElement.AllowArbitraryItemHeight = value;
                }
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
                return this.listViewElement.MultiSelect;
            }
            set
            {
                this.listViewElement.MultiSelect = value;
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
                return this.listViewElement.AllowEdit;
            }
            set
            {
                this.listViewElement.AllowEdit = value;
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
                return this.listViewElement.AllowRemove;
            }
            set
            {
                this.listViewElement.AllowRemove = value;
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
                return this.listViewElement.ActiveEditor;
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
                return this.listViewElement.ViewType;
            }
            set
            {
                this.listViewElement.ViewType = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadListViewElement"/> of the control.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the RadListViewElement of RadListView")]
        public RadListViewElement ListViewElement
        {
            get
            {
                return listViewElement;
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
            get { return this.listViewElement.HeaderHeight; }
            set { this.listViewElement.HeaderHeight = value; }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(120, 95);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Selects a set of items.
        /// </summary>
        /// <param name="items">The items to select.</param>
        public void Select(ListViewDataItem[] items)
        {
            this.listViewElement.Select(items);
        }

        /// <summary>
        /// Begins an edit operation over the currently sellected item.
        /// </summary>
        /// <returns>[true] if success, [false] otherwise</returns>
        public void BeginEdit()
        {
            this.listViewElement.BeginEdit();
        }

        /// <summary>
        /// Ends the current edit operations if such. Saves the changes.
        /// </summary>
        /// <returns>[true] if success, [false] otherwise</returns>
        public void EndEdit()
        {
            this.listViewElement.EndEdit();
        }

        /// <summary>
        /// Ends the current edit operations if such. Discards the changes.
        /// </summary>
        /// <returns>[true] if success, [false] otherwise</returns>
        public void CancelEdit()
        {
            this.listViewElement.CancelEdit();
        }

        #endregion

        #region Overrides

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (this.ContainsFocus && this.IsEditing)
            {
                return;
            }

            if (!this.ContainsFocus)
            {
                this.listViewElement.EndEdit();
            }

            this.listViewElement.SynchronizeVisualItems();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.listViewElement.SynchronizeVisualItems();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.End:
                case Keys.Home:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right | Keys.Shift:
                case Keys.Up | Keys.Shift:
                case Keys.Left | Keys.Shift:
                case Keys.Down | Keys.Shift:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (this.listViewElement.ProcessMouseDown(e))
            {
                return;
            }

            base.OnMouseDown(e);
        }


        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (this.listViewElement.ProcessMouseUp(e))
            {
                return;
            }

            base.OnMouseUp(e);
        }


        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (this.listViewElement.ProcessMouseMove(e))
            {
                return;
            }

            base.OnMouseMove(e);
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (this.listViewElement.ProcessKeyDown(e))
            {
                return;
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            if (this.listViewElement.ProcessMouseWheel(e))
            {
                return;
            }

            base.OnMouseWheel(e);
        }

        protected internal override void OnThemeNameChanged(ThemeNameChangedEventArgs e)
        {
            base.OnThemeNameChanged(e);
            this.listViewElement.ViewElement.ViewElement.ElementProvider.ClearCache();
            this.listViewElement.ViewElement.ViewElement.SuspendLayout();
            this.listViewElement.ViewElement.ViewElement.DisposeChildren();
            this.listViewElement.ViewElement.ViewElement.ResumeLayout(true);
        }

        #endregion
    }
}
