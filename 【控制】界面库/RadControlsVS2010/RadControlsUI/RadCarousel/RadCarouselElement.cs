using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;



namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides values for ItemClickDefaultAction property
    /// </summary>
    public enum CarouselItemClickAction
    {
        /// <summary>
        /// Indicates that item click will not be handeled by default
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that item will be set selected when clicked.
        /// </summary>
        SelectItem = 1
    }

    /// <summary>
    /// RadElement that animates a list of items using Carousel-style view, used by <see cref="RadCarousel"/> control
    /// </summary>
    public class RadCarouselElement : RadItem, IDisposable
    {
        private FillPrimitive backgroundPrimitive;
        private BorderPrimitive borderPrimitive;
        private CarouselItemsContainer carouselItemContainer;

        internal static readonly object SelectedIndexChangedEventKey = new object();
        internal static readonly object SelectedValueChangedEventKey = new object();
        internal static readonly object NewCarouselItemCreatingEventKey = new object();
        internal static readonly object SelectedItemChangedEventKey = new object();
        internal static readonly object ItemDataBoundEventKey = new object();
        private static object ItemLeavingEventKey = new object();
        private static object ItemEnteringEventKey = new object();

        //TODO: change event type
        public event EventHandler AnimationStarted;
        public event EventHandler AnimationFinished;
        
        private CarouselItemClickAction itemClickDefaultAction = CarouselItemClickAction.SelectItem;
        //private bool virtualMode;
        //private bool enableLooping;
        private RadItem selectedItem;
        private int selectedIndex;
        //private object selectedValue;
        private BindingMemberInfo valueMember;
        private bool caseSensitive;
        private int oldIndex = -1;
        private bool clearItemsSilently = false;

        private int updateCount;

        //binding
        private bool dataSourceDisposing = false;
        private CurrencyManager dataManager;
        private object dataSource;
        private bool inSetDataConnection;
        private bool isDataSourceInitEventHooked;
        private bool isDataSourceInitialized;
        private RadItemCollection boundItems;
        private bool formattingEnabled;
        private double itemReflectionPercentage = 0.333;

        private RadRepeatButtonElement btnPrev;
        private RadRepeatButtonElement btnNext;
        private Size navigationButtonsOffset = new Size(0,0);

        private NavigationButtonsPosition buttonsPositon = NavigationButtonsPosition.Bottom;

        private int autoLoopPauseInterval = 3;
                
        #region PROPERTIES FOR TESTING PURPOSES ONLY!

        public CarouselItemsContainer CarouselItemContainer
        {
            get
            {
                return this.carouselItemContainer;
            }
        }

        #endregion

        #region Constructors & Initialization


        static RadCarouselElement()
        {
            new Themes.ControlDefault.Carousel().DeserializeTheme();
            new Themes.ControlDefault.CarouselItem().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.boundItems = new RadItemCollection();
            this.boundItems.ItemsChanged += new ItemChangedDelegate(boundItems_ItemsChanged);
            this.BypassLayoutPolicies = true;

            //TODO: Explicitly create an entry in the property system for BindingContext, so that we can receive update through the inheritance chain
            if(this.BindingContext != null)
            {
            }
        }

        protected override void CreateChildElements()
        {
            if (this.backgroundPrimitive == null)
            {
                this.backgroundPrimitive = new FillPrimitive();
                this.Children.Insert(0, this.backgroundPrimitive);
            }

            if (this.borderPrimitive == null)
            {
                this.borderPrimitive = new BorderPrimitive();
                this.borderPrimitive.ZIndex = 12;
                this.Children.Insert(1, this.borderPrimitive);
            }

            if (this.carouselItemContainer == null)
            {
                this.carouselItemContainer = new CarouselItemsContainer(this);
                this.carouselItemContainer.ZIndex = 5;
                this.Children.Insert(2, this.carouselItemContainer);

                this.carouselItemContainer.Items.ItemsChanged += new ItemChangedDelegate(ContainerItems_ItemsChanged);
            }

            btnPrev = new RadRepeatButtonElement("Previous");
            btnPrev.Class = "PreviousButton";
            btnPrev.ImagePrimitive.Class = "PreviousButtonImage";
            btnPrev.ZIndex = 10;
            btnPrev.Click += new EventHandler(btnPrev_Click);
            btnPrev.Interval = 500;
            this.Children.Add(btnPrev);

            btnNext = new RadRepeatButtonElement("Next");
            btnNext.Class = "NextButton";
            btnNext.ImagePrimitive.Class = "NextButtonImage";
            btnNext.Click += new EventHandler(btnNext_Click);
            btnNext.Interval = 500;
            btnNext.ZIndex = 11;
            this.Children.Add(btnNext);

            base.CreateChildElements();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.ItemsContainer.EnableAutoLoop)
            {
                this.suspendAutoLoopTicks = SuspendTicksCount;
            }
            if (this.Items.Count == 0)
                return;
            if (this.SelectedIndex < this.Items.Count - 1)
                this.SelectedIndex++;
            else
                this.SelectedIndex = 0;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (this.ItemsContainer.EnableAutoLoop)
            {
                this.suspendAutoLoopTicks = SuspendTicksCount;
            }
            if (this.Items.Count == 0)
                return;
            if (this.SelectedIndex > 0)
                this.SelectedIndex--;
            else
                this.SelectedIndex = this.Items.Count - 1;
        }
        
        //private int itemsAdded = 0;
        //private int itemsRemoved = 0;

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF availableSize = base.ArrangeOverride(finalSize);
            RectangleF prevBtnRect = RectangleF.Empty, 
                       nextBtnRect=RectangleF.Empty;
 
            switch( buttonsPositon )
            {
                case NavigationButtonsPosition.Bottom:
                    prevBtnRect = new RectangleF(
                        new PointF(NavigationButtonsOffset.Width,
                        availableSize.Height - navigationButtonsOffset.Height - btnPrev.DesiredSize.Height),
                        btnPrev.DesiredSize
                        );

                    nextBtnRect = new RectangleF(
                       new PointF(availableSize.Width - NavigationButtonsOffset.Width - btnNext.DesiredSize.Width,
                       availableSize.Height - NavigationButtonsOffset.Height - btnNext.DesiredSize.Height),
                       btnNext.DesiredSize
                       );
                    break;

                case NavigationButtonsPosition.Top:
                    prevBtnRect = new RectangleF(
                        new PointF(NavigationButtonsOffset.Width,
                        navigationButtonsOffset.Height + btnPrev.DesiredSize.Height),
                        btnPrev.DesiredSize
                        );

                    nextBtnRect = new RectangleF(
                       new PointF(availableSize.Width - NavigationButtonsOffset.Width - btnNext.DesiredSize.Width,
                       NavigationButtonsOffset.Height + btnNext.DesiredSize.Height),
                       btnNext.DesiredSize
                       );
                    break;

                case NavigationButtonsPosition.Left:
                    prevBtnRect = new RectangleF(
                        new PointF(NavigationButtonsOffset.Width,
                        navigationButtonsOffset.Height + btnPrev.DesiredSize.Height),
                        btnPrev.DesiredSize
                        );

                    nextBtnRect = new RectangleF(
                       new PointF(NavigationButtonsOffset.Width,
                       availableSize.Height - NavigationButtonsOffset.Height - btnNext.DesiredSize.Height),
                       btnNext.DesiredSize
                       );

                    break;

                    

                case NavigationButtonsPosition.Right:

                    prevBtnRect = new RectangleF(
                        new PointF( availableSize.Width - NavigationButtonsOffset.Width - btnPrev.DesiredSize.Width,
                        navigationButtonsOffset.Height + btnPrev.DesiredSize.Height),
                        btnPrev.DesiredSize
                        );

                    nextBtnRect = new RectangleF(
                       new PointF( availableSize.Width - NavigationButtonsOffset.Width - btnPrev.DesiredSize.Width,
                       availableSize.Height - NavigationButtonsOffset.Height - btnNext.DesiredSize.Height),
                       btnNext.DesiredSize
                       );

                    break;
            }
             

            this.btnPrev.Arrange(prevBtnRect);
            this.btnNext.Arrange(nextBtnRect);

            return availableSize;
        }

        private void ContainerItems_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted ||
                operation == ItemsChangeOperation.Set)
            {
                target.Click += new EventHandler(this.CarouselItem_Click);

                target.MouseEnter += new EventHandler(element_MouseEnter);
                target.MouseLeave += new EventHandler(element_MouseLeave);
            }
            else if (operation == ItemsChangeOperation.Removed)
            {
                target.Click -= new EventHandler(this.CarouselItem_Click);

                target.MouseEnter -= new EventHandler(element_MouseEnter);
                target.MouseLeave -= new EventHandler(element_MouseLeave);
            }

            this.hoveredItem = null;
            this.ReEvaluateAutoLoopPauseCondition();
        }

        #endregion

        #region Events        

        internal void CallOnItemLeaving(ItemLeavingEventArgs args)
        {
            this.OnItemLeaving(args);
        }

        internal void CallOnItemEntering(ItemEnteringEventArgs args)
        {
            this.OnItemEntering(args);
        }

        /// <summary>
        /// Fires the ItemLeaving event
        /// </summary>
        /// <param name="args">Event specific arguemtns</param>        
        protected void OnItemLeaving(ItemLeavingEventArgs args)
        {
            ItemLeavingEventHandler eventHandler = (ItemLeavingEventHandler)this.Events[ItemLeavingEventKey];
            if (eventHandler != null)
            {
                eventHandler(this, args);
            }
        }

        /// <summary>
        /// Occurs when an Item is about to leave carousel view
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when an Item is about to leave carousel view")]
        public event ItemLeavingEventHandler ItemLeaving
        {
            add
            {
                this.Events.AddHandler(ItemLeavingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemLeavingEventKey, value);
            }
        }

        /// <summary>
        /// Fires the ItemEntering event
        /// </summary>
        /// <param name="args">Event specific arguemtns</param>
        protected void OnItemEntering(ItemEnteringEventArgs args)
        {
            ItemEnteringEventHandler eventHandler = (ItemEnteringEventHandler)this.Events[ItemEnteringEventKey];
            if (eventHandler != null)
            {
                eventHandler(this, args);
            }
        }

        /// <summary>
        /// Occurs when an Item is about to enter carousel view
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when an Item is about to enter carousel view")]
        public event ItemEnteringEventHandler ItemEntering
        {
            add { this.Events.AddHandler(ItemEnteringEventKey, value); }
            remove { this.Events.RemoveHandler(ItemEnteringEventKey, value); }
        }

        /// <summary>
        /// Occurs before a new databound carousel item is created. You can use this event to
        /// replace the default item.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when new databound carousel item is created.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event NewCarouselItemCreatingEventHandler NewCarouselItemCreating
        {
            add
            {
                this.Events.AddHandler(RadCarouselElement.NewCarouselItemCreatingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadCarouselElement.NewCarouselItemCreatingEventKey, value);
            }
        }

        /// <summary>
        /// Raises the CreateNewCarouselItem event.
        /// </summary>
        protected virtual void OnNewCarouselItemCreating(NewCarouselItemCreatingEventArgs e)
        {
            NewCarouselItemCreatingEventHandler handler1 = (NewCarouselItemCreatingEventHandler)this.Events[RadCarouselElement.NewCarouselItemCreatingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        internal void CallOnNewCarouselItemCreating(NewCarouselItemCreatingEventArgs e)
        {
            this.OnNewCarouselItemCreating(e);
        }

        /// <summary>Occurs after an Item is databound.</summary>
        [Category(RadDesignCategory.DataCategory),
        Description("Occurs after an Item is databound.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event ItemDataBoundEventHandler ItemDataBound
        {
            add
            {
                this.Events.AddHandler(RadCarouselElement.ItemDataBoundEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadCarouselElement.ItemDataBoundEventKey, value);
            }
        }

        /// <summary>
        /// Raises the ItemDataBound event.
        /// </summary>
        protected virtual void OnItemDataBound(ItemDataBoundEventArgs e)
        {
            ItemDataBoundEventHandler handler1 = (ItemDataBoundEventHandler)this.Events[RadCarouselElement.ItemDataBoundEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        internal void CallOnItemDataBound(ItemDataBoundEventArgs e)
        {
            this.OnItemDataBound(e);
        }

        /// <summary>Occurs when the selected items is changed.</summary>
        [Category(RadDesignCategory.BehaviorCategory),
       Description("Occurs when the selected items is changed.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler SelectedItemChanged
        {
            add
            {
                this.Events.AddHandler(RadCarouselElement.SelectedItemChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadCarouselElement.SelectedItemChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the SelectedItemChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadCarouselElement.SelectedItemChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        internal void CallOnSelectedItemChanged(EventArgs e)
        {
            this.OnSelectedItemChanged(e);
        }

        /// <summary>Fires when the selected value is changed.</summary>
        [Browsable(true),
        Category("Property Changed"),
        Description("Occurs when the SelectedValue property has changed.")]
        public event EventHandler SelectedValueChanged
        {
            add
            {
                this.Events.AddHandler(RadCarouselElement.SelectedValueChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadCarouselElement.SelectedValueChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the SelectedValueChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadCarouselElement.SelectedValueChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
            //this.SetActiveItem(null);
        }

        internal void CallOnSelectedValueChanged(EventArgs e)
        {
            this.OnSelectedValueChanged(e);
        }

        /// <summary>Fires when the selected index is changed.</summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the SelectedIndex property has changed.")]
        public event EventHandler SelectedIndexChanged
        {
            add
            {
                this.Events.AddHandler(RadCarouselElement.SelectedIndexChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadCarouselElement.SelectedIndexChangedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the SelectedIndexChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSelectedIndexChanged(SelectedIndexChangedEventArgs e)
        {
            this.OnNotifyPropertyChanged("SelectedIndex");

            EventHandler handler1 = (EventHandler)this.Events[RadCarouselElement.SelectedIndexChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        internal void CallOnSelectedIndexChanged(SelectedIndexChangedEventArgs e)
        {
            this.OnSelectedIndexChanged(e);
        }

        internal void AddEventHandler(object key, Delegate value)
        {
            this.Events.AddHandler(key, value);
        }

        internal void RemoveEventHandler(object key, Delegate value)
        {
            this.Events.RemoveHandler(key, value);
        }        

        #endregion

        #region Properties        

        /// <summary>Gets a collection of RadItem objects managed by RadCarousel.</summary>
        /// <remarks>
        /// Items are populated automatically when RadCarousel is data-bound. When using <see cref="CarouselItemsContainer.Virtualized"/>, carousel displays only <see cref="CarouselItemsContainer.VisibleItemCount"/> number of items at a time.
        /// </remarks>
		[Description("Gets a collection representing the items contained in this RadCarousel.")]
		public RadItemCollection Items
        {
            get 
            {
                return this.carouselItemContainer.Items;
            }
        }

        /// <summary>Gets the element, which contains all visible carousel items</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CarouselItemsContainer ItemsContainer
        {
            get
            {
                return this.carouselItemContainer;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sorting of carousel items is
        /// case-sensitive.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether sorting is case-sensitive."),
        DefaultValue(false)]
        public bool CaseSensitive
        {
            get { return this.caseSensitive; }
            set { this.caseSensitive = value; }
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

        /// <summary>Gets or sets the item in the carousel that is currently selected.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the currently selected item."),
        Browsable(false),
        Bindable(true)]
        public virtual Object SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (value is RadItem)
                {
                    RadItem item = (RadItem)value;
                    if (this.Items.Contains(item))
                    {
                        this.SelectedIndex = this.Items.IndexOf(item);
                        this.selectedItem = item;
                    }
                }
                else if (value is string)
                {
                    RadItem item = this.FindItemExact((string)value);
                    if (item != null)
                    {
                        this.SelectedIndex = this.Items.IndexOf(item);
                        this.selectedItem = item;
                    }
                }
                else if (value == null)
                {
                    this.SelectedIndex = -1;
                    this.selectedItem = null;
                }
            }
        }

        /// <summary>Gets or sets the index the currently selected item.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false), Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets the index specifying the currently selected item.")]
        public virtual int SelectedIndex
        {
            get { return this.selectedIndex; }
            set
            {
                if (this.selectedIndex != value && !this.clearItemsSilently)
                {
                    this.CarouselItemContainer.SelectedIndex = value;
                    
                    this.ClearSelectedCollectionsSilently();
                    if( value >= 0 && value < this.Items.Count )
                    {
                        this.selectedItem = (RadItem)this.Items[value];
                        this.selectedIndex = value;
                    }
                    else
                    {
                        this.selectedItem = null;
                        this.selectedIndex = -1;
                    }

                    this.hoveredItem = null;
                    this.ReEvaluateAutoLoopPauseCondition();

                    this.RaiseSelectedIndexChanged();
                    //ToDo: place code to visualy select item here
                    //this.ScrollElementIntoView(this.selectedItem);
                }
            }
        }

        private void boundItems_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            switch (operation)
            {
                case ItemsChangeOperation.Inserted:
                    if (target == null)
                        return;
                    this.Items.Add(target);
                    break;
                case ItemsChangeOperation.Clearing:
                    this.BeginUpdate();
                    try
                    {
                        foreach (RadItem boundItem in this.boundItems)
                        {
                            this.Items.Remove(boundItem);
                        }
                    }
                    finally
                    {
                        this.EndUpdate();
                    }
                    break;
            }            

            //if (this.SortItems != SortStyle.None && operation == ItemsChangeOperation.Inserted && isInitialized)
            //{
            //    this.Sort(this.SortItems);
            //}
        }

        private void ClearSelectedCollectionsSilently()
        {
            //this.selectedCollectionChangeSilently = true;
            //this.selectedIndices.Clear();
            //this.selectedItems.Clear();
            //this.selectedCollectionChangeSilently = false;
        }

        private void RaiseSelectedIndexChanged()
        {
            this.OnSelectedIndexChanged(new SelectedIndexChangedEventArgs(this.oldIndex, this.selectedIndex));
            this.oldIndex = this.selectedIndex;
        }

        /// <summary>Gets or sets a value defining the currently selected item.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets value specifying the currently selected item."),
        Browsable(false),
        Bindable(true)]
        public Object SelectedValue
        {
            get
            {
                //if ((this.SelectedIndex != -1) && (this.valueMember.BindingField.Length > 0))
                if (this.SelectedIndex >= 0 && this.SelectedIndex<this.Items.Count)
                {
                    RadListBoxItem listBoxItem = this.Items[this.SelectedIndex] as RadListBoxItem;
                    if (listBoxItem != null)
                        return listBoxItem.Value;
                }
                return null;
            }
            set
            {
                if (this.dataManager != null)
                {
                    string text1 = this.valueMember.BindingField;
                    if (string.IsNullOrEmpty(text1))
                    {
                        throw new InvalidOperationException("List Control Empty ValueMember");
                    }
                }

                if (this.SelectedValue != null && this.SelectedValue.Equals(value))
                    return;

                int index = -1;
                if (value != null)
                {
                    for (int num1 = 0; num1 < this.Items.Count; num1++)
                    {
                        RadListBoxItem listBoxItem = this.Items[num1] as RadListBoxItem;
                        if (listBoxItem != null && value.Equals(listBoxItem.Value))
                        {
                            index = num1;
                            break;
                        }
                    }
                }
                this.SelectedIndex = index;                
                this.OnNotifyPropertyChanged("SelectedValue");
            }
        }

        /// <summary>
        /// Gets or sets the field from the data source to use as the actual value for the
        /// carousel items.
        /// </summary>
        [Description("Gets or sets the property to use as the actual value for the items."),
        Category(RadDesignCategory.DataCategory),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ValueMember
        {
            get
            {
                return this.valueMember.BindingMember;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                BindingMemberInfo info1 = new BindingMemberInfo(value);
                if (!info1.Equals(this.valueMember))
                {
                    if (this.ValueMember.Length == 0)
                    {
                        this.SetDataConnection(this.DataSource, false);
                    }
                    if (((this.dataManager != null) && (value != null)) && ((value.Length != 0) &&
                        !this.BindingMemberInfoInDataManager(info1)))
                    {
                        throw new ArgumentException("List Control Wrong Value Member", "value");
                    }
                    this.valueMember = info1;
                    this.OnNotifyPropertyChanged("ValueMember");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether formatting is applied to the DisplayMember property.
        /// </summary>
        [Description("Gets or sets a value indicating whether formatting is applied to the DisplayMember property."),
        DefaultValue(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FormattingEnabled
        {
            get
            {
                return this.formattingEnabled;
            }
            set
            {
                if (value != this.formattingEnabled)
                {
                    this.formattingEnabled = value;
                    this.RefreshItems();
                    this.OnNotifyPropertyChanged("FormattingEnabled");
                }
            }
        }

        private bool BindingMemberInfoInDataManager(BindingMemberInfo bindingMemberInfo)
        {
            if (this.dataManager != null)
            {
                PropertyDescriptorCollection collection1 = this.dataManager.GetItemProperties();
                int num1 = collection1.Count;
                for (int num2 = 0; num2 < num1; num2++)
                {
                    if (!typeof(IList).IsAssignableFrom(collection1[num2].PropertyType) &&
                        collection1[num2].Name.Equals(bindingMemberInfo.BindingField))
                    {
                        return true;
                    }
                }
                for (int num3 = 0; num3 < num1; num3++)
                {
                    if (!typeof(IList).IsAssignableFrom(collection1[num3].PropertyType) &&
                        (string.Compare(collection1[num3].Name, bindingMemberInfo.BindingField, true, CultureInfo.CurrentCulture) == 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Gets or sets the data source that the carousel will bind to.</summary>
        [DefaultValue((string)null),
        AttributeProvider(typeof(IListSource)),
        Description("Gets or sets the data source."),
        Category(RadDesignCategory.DataCategory),
        RefreshProperties(RefreshProperties.Repaint)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (((value != null) && !(value is IList)) && !(value is IListSource))
                {
                    throw new ArgumentException("Bad Data Source For Complex Binding");
                }
                if (this.dataSource != value)
                {
                    try
                    {
                        this.SetDataConnection(value, false);
                        //this.OnNotifyPropertyChanged("DataSource");
                    }
                    catch
                    {
                        //
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the default action when item is clicked as <see cref="CarouselItemClickAction"/> member.
        /// </summary>
        /// <value>The item click default action.</value>
        [DefaultValue(CarouselItemClickAction.SelectItem)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public CarouselItemClickAction ItemClickDefaultAction
        {
            get { return this.itemClickDefaultAction; }
            set { this.itemClickDefaultAction = value; }
        }

        /// <summary>
        /// Gets or sets value indicating the height (in percentage - values from 0.0. to 1.0) of reflection that will be painted bellow each carousel item.
        /// </summary>
        /// <value>The item reflection percentage.</value>
        /// <remarks>
        /// 0.0 indicates no reflection and 1.0 indicates 100% of the height of the original item
        /// </remarks>
        [DefaultValue(0.333)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ItemReflectionPercentage
        {
            get
            {
                return this.itemReflectionPercentage;
            }
            set
            {
                this.itemReflectionPercentage = value;
                foreach (RadElement entry in this.Children)
                {
                   CarouselItemsContainer contentItem = entry as CarouselItemsContainer;
                   if (contentItem != null)
                   {
                       foreach (CarouselContentItem carouselContentItem in contentItem.Children)
                       {
                           carouselContentItem.reflectionPrimitive.ItemReflectionPercentage = value;
                       }
                   }
                }
            }
        }

        /// <summary>
        /// Set ot get the Carousel animation frames
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(30)]
        public int AnimationFrames
        {
            get { return this.CarouselItemContainer.AnimationFrames; }
            set { this.CarouselItemContainer.AnimationFrames = value; }
        }

        /// <summary>
        /// Set ot get the Carousel animation frames delay
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(40)]
        public int AnimationDelay
        {
            get { return this.CarouselItemContainer.AnimationDelay; }
            set { this.CarouselItemContainer.AnimationDelay = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating the interval (in seconds) after which the carousel will resume looping when in auto-loop mode.
        /// </summary>
        [Description("Gets or sets a value indicating the interval (in seconds) after which the carousel will resume looping when in auto-loop mode.")]
        [DefaultValue(3)]
        [Category("AutoLoopBehavior")]
        public int AutoLoopPauseInterval
        {
            get { return this.autoLoopPauseInterval; }

            set { this.autoLoopPauseInterval = value; }
        }

        #endregion


        /// <summary>Initiates batch update of the items.</summary>
        public void BeginUpdate()
        {
            this.updateCount++;

            if (!this.UseNewLayoutSystem)
            {
                this.SuspendLayout();
            }

            this.ItemsContainer.BeginUpdate();
        }

        /// <summary>Ends batch update of the items.</summary>
        public void EndUpdate()
        {
            if (this.updateCount > 0)
                this.updateCount--;

            this.ItemsContainer.EndUpdate();
            if (!this.UseNewLayoutSystem)
            {
                this.ResumeLayout(false);
                //TODO: This is fix for the layout of the listbox items - it should be forced when the sum of the height of the items
                // are less than the height of the scrollviewer and the vert. scrollbar doesn't appear thus performing layout is skipped
                this.LayoutEngine.InvalidateLayout();
                this.LayoutEngine.PerformParentLayout();
            }
        }

        protected virtual void RefreshItems()
        {
            try
            {
                this.BeginUpdate();
                if ((this.DataManager == null) || (this.DataManager.Count == 0))
                {
                    return;
                }

                this.clearItemsSilently = true;
                this.Items.Clear();
                this.boundItems.Clear();
                this.clearItemsSilently = false;
                for (int i = 0; i < this.DataManager.Count; i++)
                {
                    object dataItem = this.DataManager.List[i];
                    RadItem item = this.CreateDataBoundCarouselItem(dataItem);
                    //this.Items.Add(item);
                    this.boundItems.Add(item);
                    this.RaiseItemDataBound(item, dataItem);
                }

                if (this.DataManager != null)
                {
                    this.SelectedIndex = this.DataManager.Position;
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        #region DataBinding

        protected virtual void SetItemCore(int index, object value)
        {
            RadItem item = this.CreateDataBoundCarouselItem(value);
            this.Items[index] = item;
            this.RaiseItemDataBound(item, value);
        }

        private void RaiseItemDataBound(RadItem item, object dataItem)
        {
            ItemDataBoundEventArgs args = new ItemDataBoundEventArgs(item, dataItem);
            this.OnItemDataBound(args);
        }

        internal virtual RadItem CreateDataBoundCarouselItem(object item)
        {
            if (item is RadItem)
            {
                return item as RadItem;
            }

            //string text = this.GetItemText(item);
            object value = this.GetItemValue(item);

            RadItem newItem = CreateNewCarouselItem();
            //ToDo use ValueChangingEventArgs property
            //newItem.Text = value != null? value.ToString() : item.ToString();
            newItem.Tag = value;

            return newItem;
        }

        protected virtual RadItem CreateNewCarouselItem()
        {
            RadItem newItem = new CarouselGenericItem();            

            NewCarouselItemCreatingEventArgs args = new NewCarouselItemCreatingEventArgs(newItem);
            this.OnNewCarouselItemCreating(args);
            newItem = args.NewCarouselItem;
            return newItem;
        }

        private void CarouselItem_Click(object sender, EventArgs e)
        {
            if (this.ItemClickDefaultAction == CarouselItemClickAction.SelectItem)
            {
                this.SelectedItem = sender;
                if (this.ItemsContainer.EnableAutoLoop)
                {
                    this.suspendAutoLoopTicks = SuspendTicksCount;
                }
            }
        }

        /// <summary>Gets the value of the given item.</summary>
        public virtual object GetItemValue(object item)
        {
            object value = null;
            if (item != null)
            {
                if (item is RadListBoxItem)
                {
                    return (item as RadListBoxItem).Value;
                }
                try
                {
                    PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(item).Find(this.valueMember.BindingField, true);
                    if (descriptor1 != null)
                    {
                        value = descriptor1.GetValue(item);
                    }
                }
                catch
                {
                }
            }
            return value;
        }

        private void SetDataConnection(object newDataSource, bool force)
        {
            bool flag1 = this.dataSource != newDataSource;
            if (!this.inSetDataConnection)
            {
                try
                {
                    if (force || flag1)
                    {
                        this.inSetDataConnection = true;
                        IList list1 = (this.DataManager != null) ? this.DataManager.List : null;
                        bool flag3 = this.DataManager == null;
                        this.DisposeDataSource();
                        this.dataSource = newDataSource;
                        this.InitializeDataSource();
                        if (this.isDataSourceInitialized)
                        {
                            CurrencyManager manager1 = null;
                            if (((newDataSource != null) && (this.BindingContext != null)) && (newDataSource != Convert.DBNull))
                            {
                                manager1 = (CurrencyManager)this.BindingContext[newDataSource];
                            }
                            if (this.dataManager != manager1)
                            {
                                if (this.dataManager != null)
                                {
                                    this.dataManager.ItemChanged -= new ItemChangedEventHandler(this.DataManagerItemChanged);
                                    this.dataManager.PositionChanged -= new EventHandler(this.DataManagerPositionChanged);
                                }
                                this.dataManager = manager1;
                                if (this.dataManager != null)
                                {
                                    this.dataManager.ItemChanged += new ItemChangedEventHandler(this.DataManagerItemChanged);
                                    this.dataManager.PositionChanged += new EventHandler(this.DataManagerPositionChanged);
                                }
                            }
                            if ((this.dataManager != null) && 
                                (force && ((list1 != this.dataManager.List) || flag3)))
                            {
                                this.DataManagerItemChanged();
                            }
                        }                        
                    }
                    if (flag1)
                    {
                        this.OnNotifyPropertyChanged("DataSource");
                        //this.OnDataSourceChanged(EventArgs.Empty);
                    }                    
                }
                finally
                {
                    this.inSetDataConnection = false;
                }
            }
        }

        

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "CaseSensitive":
                    break;
                case "DataSource":
                    //if ((this.SortItems != SortStyle.None) && (this.DataSource != null))
                    //{
                    //    this.SortItems = SortStyle.None;
                    //    //throw new InvalidOperationException("Data Source and Sorted property cannot be set together");
                    //}

                    if (!this.dataSourceDisposing)
                    {
                        this.BeginUpdate();
                        //this.Items.Clear();
                        this.SelectedIndex = -1;
                        this.boundItems.Clear();
                        this.EndUpdate();
                    }
                    this.RefreshItems();
                    break;
                case "FormatInfo":
                    break;
                case "FormatString":
                    break;
                case "FormattingEnabled":
                    break;
                case "DisplayMember":
                    this.RefreshItems();
                    break;
                case "ValueMember":
                    this.RefreshItems();
                    break;
                case "SelectedIndex":

                    //if (this.selectedItems.Count > 0)
                    //{
                    //    if (this.SelectedItem != this.selectedItems[0])
                    //    {
                    //        this.selectedItem = this.selectedItems[0];
                    //        this.selectedIndex = this.Items.IndexOf(this.selectedItem);
                    //    }
                    //}	
                    //else
                    //{ 
                    //    this.selectedItem = null;
                    //    this.selectedIndex = -1;
                    //}
                    
                    //if (((this.DataManager != null) && (this.DataManager.Position != this.SelectedIndex))
                    //	&& (!this.FormattingEnabled || (this.SelectedIndex != -1)))						
                    if ((this.DataManager != null) && (!this.FormattingEnabled || (this.SelectedIndex != -1)))
                    {
                        if (this.SelectedIndex >= this.Items.Count - this.boundItems.Count)
                        {
                            this.DataManager.Position = this.SelectedIndex - this.Items.Count + this.boundItems.Count;
                            //if (!string.IsNullOrEmpty(this.ValueMember))
                            //    this.OnSelectedValueChanged(EventArgs.Empty);
                        }
                    }
                    this.OnSelectedValueChanged(EventArgs.Empty);
                    break;
                case "SelectedValue":
                    break;
                case "AnimationFrames":
                case "AnimationDelay":
                    this.ItemsContainer.ForceUpdate();
                    break;

            }
            base.OnNotifyPropertyChanged(propertyName);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.IsMouseOverElementProperty)
            {
                this.ReEvaluateAutoLoopPauseCondition();
            }
            else if (e.Property == BindingContextProperty)
            {
                if (this.dataSource != null)
                {
                    this.SetDataConnection(this.dataSource, true);
                }
            }
        }

        private Timer timer = null;
        private const int autoloopTimerInterval = 200;

        private int suspendAutoLoopTicks = 0;
        //private int suspendTicksCount = (1000 / autoloopTimerInterval) * 3;

        
        private int SuspendTicksCount
        {
         
            get
            {
                return (1000/autoloopTimerInterval)*this.AutoLoopPauseInterval;
            }
        }

        private object hoveredItem = null;
        private int autoAnimationCount = -1;
        private RadEasingType? originalEasingType = null;
        private bool autoLoopPaused = false;

        internal void ChangedAutoLoop()
        {
            if (this.IsDesignMode)
                return;

            if (this.ItemsContainer.EnableAutoLoop)
            {
                this.originalEasingType = null;

                this.InitLoopTimer();

                for (int i = 0; i< this.Items.Count; i++)
                {
                    RadItem entry = this.Items[i];
                    entry.MouseEnter += new EventHandler(element_MouseEnter);
                    entry.MouseLeave += new EventHandler(element_MouseLeave);
                }
                
                this.timer.Start();
            }
            else
            {
                this.timer.Stop();
                this.autoAnimationCount = -1;
                if (originalEasingType != null)
                    this.ItemsContainer.EasingType = originalEasingType.Value;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    RadItem entry = this.Items[i];
                    entry.MouseEnter -= new EventHandler(element_MouseEnter);
                    entry.MouseLeave -= new EventHandler(element_MouseLeave);
                }
            }
        }        

        private void InitLoopTimer()
        {
            if (this.timer == null)
            {
                this.timer = new Timer();
                this.timer.Interval = autoloopTimerInterval;
                this.timer.Tick += new EventHandler(timer_Tick);
            }
        }

        //TODO: review code for auto-loop feature
        private void timer_Tick(object sender, EventArgs e)
        {
            AutoLoopNextFrame();
        }

        private void AutoLoopNextFrame()
        {
            this.suspendAutoLoopTicks--;
            if (!this.autoLoopPaused && this.suspendAutoLoopTicks <= 0)
            {
                if (!CanApplyAutoLoop())
                    return;

                this.suspendAutoLoopTicks = 0;

                if (originalEasingType == null)
                {
                    originalEasingType = this.ItemsContainer.EasingType;
                }

                this.ItemsContainer.EasingType = RadEasingType.Linear;

                if (this.autoAnimationCount <= 0 && this.ItemsContainer.Items.Count > 0)
                {
                    this.autoAnimationCount = 1;
                    if (this.ItemsContainer.AutoLoopDirection == AutoLoopDirections.Forward)
                    {
                        this.ItemsContainer.SelectedIndex++;
                    }
                    else
                        if (this.ItemsContainer.AutoLoopDirection == AutoLoopDirections.Backward)
                        {
                            this.ItemsContainer.SelectedIndex--;
                        }
                    //else... depend on mouse pos and so on
                }
            }
        }

        private bool CanApplyAutoLoop()
        {
            if (!this.ItemsContainer.EnableAutoLoop)
                return false;

            if (this.ElementTree != null)
            {
                if (!this.ElementTree.Control.IsHandleCreated ||
                    !this.ElementTree.Control.Visible)
                {
                    return false;
                }

                Form frm = this.ElementTree.Control.FindForm();
                if (frm == null || frm.WindowState == FormWindowState.Minimized)
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }


        //TODO: review code for auto-loop feature
        public virtual void OnAnimationFinished()
        {
            if (this.AnimationFinished != null)
            {
                this.AnimationFinished(this, EventArgs.Empty);
            }

            this.autoAnimationCount--;

            if (!CanApplyAutoLoop())
                return;

            if (!this.autoLoopPaused &&
                this.suspendAutoLoopTicks <= 0 &&
                this.autoAnimationCount == 0)
            {
                this.ElementTree.Control.BeginInvoke(new MethodInvoker(AutoLoopNextFrame));                
            }
        }

        public virtual void OnAnimationStarted()
        {
            if (this.AnimationStarted != null)
            {
                this.AnimationStarted(this, EventArgs.Empty);
            }
        }

        private void element_MouseEnter(object sender, EventArgs e)
        {
            this.hoveredItem = sender;

            this.ReEvaluateAutoLoopPauseCondition();
        }

        private void element_MouseLeave(object sender, EventArgs e)
        {
            if (sender == this.hoveredItem)
            {
                this.hoveredItem = null;
                
                this.ReEvaluateAutoLoopPauseCondition();
            }
        }

        private void ReEvaluateAutoLoopPauseCondition()
        {
            if (!this.ItemsContainer.EnableAutoLoop)
            {
                this.autoLoopPaused = false;
                return;
            }

            bool oldAutoLoopPaused = autoLoopPaused;

            AutoLoopPauseConditions condition = this.ItemsContainer.AutoLoopPauseCondition;

            if ((condition & AutoLoopPauseConditions.OnMouseOverCarousel) == 
                AutoLoopPauseConditions.OnMouseOverCarousel )
            {
                this.autoLoopPaused = this.IsMouseOverElement;
            }
            
            if ((condition & AutoLoopPauseConditions.OnMouseOverItem) ==
                AutoLoopPauseConditions.OnMouseOverItem)
            {
                this.autoLoopPaused = this.hoveredItem != null;
            }

            if (autoLoopPaused)
            {
                if (originalEasingType != null)
                    this.ItemsContainer.EasingType = originalEasingType.Value;
            }
            else
            {
                if (oldAutoLoopPaused/*  != autoLoopPaused */)
                    this.suspendAutoLoopTicks = SuspendTicksCount;
            }
        }

        protected virtual void DataManagerPositionChanged(object sender, EventArgs e)
        {
            //if ((this.dataManager != null) && (this.Items.Count > 0) && this.AllowSelection)
            if ((this.dataManager != null) && (this.boundItems.Count > 0))
            {
                this.SelectedIndex = this.dataManager.Position + this.Items.Count - this.boundItems.Count;
            }

            //this.SetActiveItem(this.SelectedItem as RadListBoxItem);
        }

        private void InitializeDataSource()
        {
            if (this.dataSource is IComponent)
            {
                ((IComponent)this.dataSource).Disposed += new EventHandler(this.DataSourceDisposed);
            }
            ISupportInitializeNotification notification1 = this.dataSource as ISupportInitializeNotification;
            if ((notification1 != null) && !notification1.IsInitialized)
            {
                notification1.Initialized += new EventHandler(this.DataSourceInitialized);
                this.isDataSourceInitEventHooked = true;
                this.isDataSourceInitialized = false;
            }
            else
            {
                this.isDataSourceInitialized = true;
            }
        }

        private void DataSourceInitialized(object sender, EventArgs e)
        {
            this.SetDataConnection(this.dataSource, true);
        }

        private void DisposeDataSource()
        {
            if (this.dataSource is IComponent)
            {
                ((IComponent)this.dataSource).Disposed -= new EventHandler(this.DataSourceDisposed);
            }
            ISupportInitializeNotification notification1 = this.dataSource as ISupportInitializeNotification;
            if ((notification1 != null) && this.isDataSourceInitEventHooked)
            {
                notification1.Initialized -= new EventHandler(this.DataSourceInitialized);
                this.isDataSourceInitEventHooked = false;
            }
        }        

        private void DataSourceDisposed(object sender, EventArgs e)
        {
            this.dataSourceDisposing = true;
            this.SetDataConnection(null, true);
        }

        protected virtual CurrencyManager DataManager
        {
            get
            {
                return this.dataManager;
            }
        }

        /// <summary>
        /// Present the Previous button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRepeatButtonElement ButtonPrevious
        {
            get
            {
                return btnPrev;
            }           
        }

        /// <summary>
        /// Pressent the Next button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRepeatButtonElement ButtonNext
        {
            get
            {
                return btnNext;
            }
        }


        /// <summary>
        /// Get or sets the minimum size to apply on an element when layout is calculated.
        /// </summary>
        [Description("Represents the navigation buttons offset")]
        [DefaultValue(typeof(Size), "0,0")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Size NavigationButtonsOffset
        {
            get
            {
                return this.navigationButtonsOffset;
            }
            set
            {
                this.navigationButtonsOffset = value;
                this.InvalidateArrange();
            }
        }


        /// <summary>
        /// Represent the Navigation buttons Possitions
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual NavigationButtonsPosition ButtonPositions
        {
            get
            {
                return this.buttonsPositon;
            }
            set
            {
                this.buttonsPositon = value;
                this.InvalidateArrange();
            }
        }    

        protected virtual void DataManagerItemChanged(object sender, System.Windows.Forms.ItemChangedEventArgs e)
        {
            if (this.dataManager != null)
            {
                if (e.Index == -1)
                {
                    this.DataManagerItemChanged();
                }
                else
                {
                    this.SetItemCore(e.Index, this.dataManager.List[e.Index]);
                }
            }
        }

        protected virtual void DataManagerItemChanged()
        {
            this.SetItemsCore(this.dataManager.List);
            //if (!this.AllowSelection)
            //{
            //    return;
            //}
            int position = this.dataManager.Position + this.Items.Count - this.boundItems.Count;

            if (this.SelectedIndex == position)
            {
                //if ((this.SelectedIndex > -1) && (this.SelectedIndex < this.Items.Count))
                if ((position > -1) && (position < this.boundItems.Count))
                {
                    //this.SelectedItem = this.Items[this.SelectedIndex].Text;
                    this.selectedIndex = -1;
                    this.selectedItem = null;
                    this.SelectedItem = this.boundItems[this.dataManager.Position].Text;
                    this.OnSelectedItemChanged(EventArgs.Empty);
                }
            }
            else
                this.SelectedIndex = position;
        }

        protected virtual void SetItemsCore(IList items)
        {
            this.BeginUpdate();
            //This is necessary to prevent firing SelectedIndexChanged event
            this.clearItemsSilently = true;
            this.Items.Clear();
            this.boundItems.Clear();
            this.clearItemsSilently = false;

            if (items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    object dataItem = items[i];
                    RadItem item = this.CreateDataBoundCarouselItem(dataItem);
                    this.boundItems.Add(item);
                    this.RaiseItemDataBound(item, dataItem);
                }
            }

            //if (this.DataManager != null)
            //{
            //    this.SelectedIndex = this.DataManager.Position;
            //}			

            this.EndUpdate();
        }
        
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this.timer != null)
            {
                this.timer.Tick -= new EventHandler(timer_Tick);
                this.timer.Dispose();
                this.timer = null;
            }

            this.DisposeDataSource();
            this.boundItems.ItemsChanged -= new ItemChangedDelegate(boundItems_ItemsChanged);
            if (carouselItemContainer != null)
            {
                this.carouselItemContainer.Items.ItemsChanged -= new ItemChangedDelegate(ContainerItems_ItemsChanged);                
            }

            if (btnPrev != null)
            {
                btnPrev.Click -= new EventHandler(btnPrev_Click);
            }

            if (btnNext != null)
            {
                btnNext.Click -= new EventHandler(btnNext_Click);
            }
        }

        #endregion

        /// <summary>
        /// Finds the first item in the list with Text that starts with the specified string. 
        /// </summary>
        /// <param name="startsWith">The string to search for.</param>
        /// <returns>The zero-based index of the first item found; returns null if no match is found.</returns>
        public RadItem FindItemStartingWith(string startsWith)
        {
            if (!string.IsNullOrEmpty(startsWith))
            {
                foreach (RadItem item in this.Items)
                {
                    if (item.Text != null && 
                        item.Text.StartsWith(startsWith, !this.CaseSensitive, System.Globalization.CultureInfo.InvariantCulture))
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the first item in the list with Text containing the specified string. 
        /// </summary>
        /// <param name="subString">The string to search for.</param>
        /// <returns>The zero-based index of the first item found; returns null if no match is found.</returns>
        public RadItem FindItemContaining(string subString)
        {
            if (!string.IsNullOrEmpty(subString))
            {
                foreach (RadItem item in this.Items)
                {
                    StringComparison comparison =
                        this.CaseSensitive
                            ?
                        StringComparison.InvariantCulture
                            : StringComparison.InvariantCultureIgnoreCase;

                    if (item.Text != null &&
                        item.Text.IndexOf(subString, 0, comparison) >= 0)
                        return item;
                }
            }

            return null;
        }        
    }
}
