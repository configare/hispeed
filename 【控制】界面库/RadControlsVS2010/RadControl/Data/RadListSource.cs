using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Telerik.WinControls.Data
{
    public class RadListSource<TDataItem> :
        IList<TDataItem>,
        IList,
        ICollection,
        IEnumerable,
        ITypedList,
        ICancelAddNew,
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        ICurrencyManagerProvider
        where TDataItem : IDataItem
    {

        #region Fields

        private int version;
        private int update = 0;
        private bool skipListChangedForItem = false;
        private bool processListChanged = true;

        private object dataSource;
        private string dataMember = string.Empty;
        private IDataItemSource source;
        private List<TDataItem> items = new List<TDataItem>(128);
        private CurrencyManager currencyManager;
        private PropertyDescriptorCollection boundProperties = null;
        private RadCollectionView<TDataItem> collectionView;
        private BindingContext bindingContext;
        private Dictionary<string, RadListSource<TDataItem>> relatedBindingSources;
        private ConstructorInfo itemConstructor = null;
        private Type itemType;

        public event EventHandler PositionChanged;

        #endregion

        #region Constructors

        public RadListSource()
            : this(null)
        {
            

        }

        public RadListSource(IDataItemSource source)
            : this(source, null)
        {
            this.collectionView = new RadDataView<TDataItem>(this);
            if (this.collectionView != null)
            {
                this.collectionView.CurrentChanged += new EventHandler(collectionView_CurrentChanged);
            }
        }

        public RadListSource(IDataItemSource source, RadCollectionView<TDataItem> collectionView)
        {
            this.currencyManager = null;
            this.boundProperties = null;
            this.collectionView = collectionView;
            if (this.collectionView == null)
            {
                this.collectionView = new EmptyCollectionView<TDataItem>(this);
            }

            this.collectionView.CurrentChanged += new EventHandler(collectionView_CurrentChanged);

            if (source == null)
            {
                this.bindingContext = new BindingContext();
                return;
            }

            this.source = source;
            this.bindingContext = this.source.BindingContext;
            this.source.BindingContextChanged += new EventHandler(source_BindingContextChanged);
        }

        #endregion
         
        #region Properties

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position
        {
            get
            {
                if (this.IsDataBound)
                {
                    return this.currencyManager.Position;
                }

                return this.IndexOf(this.CollectionView.CurrentItem);
            }
            set
            {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException("Invalid index.");
                }

                if (value < this.Count)
                {
                    this.CollectionView.MoveCurrentTo(this[value]);
                }
            }
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public TDataItem Current
        {
            get
            {
                return this.CollectionView.CurrentItem;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public virtual void Refresh()
        {
            if (update == 0)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            this.Initialize();
        }

        /// <summary>
        /// Begins the update.
        /// </summary>
        public void BeginUpdate()
        {
            this.update++;
            this.collectionView.BeginUpdate();
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        public void EndUpdate()
        {
            this.EndUpdate(true);
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        /// <param name="notifyUpdates">if set to <c>true</c> [notify updates].</param>
        public void EndUpdate(bool notifyUpdates)
        {
            update--;
            this.collectionView.EndUpdate(false);

            if (update < 0)
            {
                update = 0;
                return;
            }

            if (update == 0 && notifyUpdates)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        public virtual TDataItem AddNew()
        {
            if (this.IsDataBound)
            {
                if (this.currencyManager.List is IBindingList)
                {
                    this.currencyManager.AddNew();
                    if (this.currencyManager.List.Count > this.Count)
                    {
                        InsertItem(this.currencyManager.List.Count - 1, default(TDataItem));
                    }

                    return this[this.Count - 1];
                }

                if (this.itemConstructor == null)
                {
                    throw new InvalidOperationException(string.Format("RadListSource need a parameterless Constructor for {0}", new object[] { (this.itemType == null) ? "(null)" : this.itemType.FullName }));
                }

                this.currencyManager.List.Add(this.itemConstructor.Invoke(null));
            }

            IDataItem newItem = this.source.NewItem();
            this.InsertItem(this.Count, (TDataItem)newItem);
            return (TDataItem)newItem;
        }

        /// <summary>
        /// Moves the specified item.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        public void Move(int oldIndex, int newIndex)
        {
            if (IsDataBound)
            {
                throw new InvalidOperationException("Items cannot be moved to the RadListSource when is in data-bound mode");
            }

            this.MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Gets the collection view.
        /// </summary>
        /// <value>The collection view.</value>
        public RadCollectionView<TDataItem> CollectionView
        {
            get
            {
                return this.collectionView;
            }
        }

        protected virtual void InsertItem(int index, TDataItem item)
        {
            if (item == null)
            {
                item = (TDataItem)this.source.NewItem();
            }

            if (this.IsDataBound && index < this.currencyManager.List.Count)
            {
                this.InitializeBoundRow(item, this.currencyManager.List[index]);
            }

            //Issue:108764 - FIX RadListSource throws exception when item from the binding source is deleted and then another item added
            if (this.IsDataBound && index > this.currencyManager.List.Count)
            {
                index = this.currencyManager.List.Count;
            }

            this.items.Insert(index, item);
            this.version++;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected virtual void SetItem(int index, TDataItem item)
        {
            IDataItem oldItem = this[index];

            if (this.IsDataBound)
            {
                this.InitializeBoundRow(item, this.currencyManager.List[index]);
            }

            this.items[index] = item;
            this.version++;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
        }

        protected virtual void ClearItems()
        {
            this.items.Clear();
            this.version++;

            this.OnNotifyPropertyChanged("Count");
            this.OnNotifyPropertyChanged("Item[]");
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void RemoveItem(int index)
        {
            TDataItem item = this[index];

            this.items.RemoveAt(index);
            this.version++;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, -1, index));
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            TDataItem item = this[oldIndex];
            this.items.RemoveAt(oldIndex);
            this.items.Insert(newIndex, item);
            this.version++;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        protected virtual void ChangeItem(int index, TDataItem item, string propertyName)
        {
            if (item == null)
            {
                if (this.IsDataBound)
                {
                    object dataBoundItem = this.currencyManager.List[index];
                    item = this.collectionView.Find(index, dataBoundItem);
                    if (item == null)
                    {
                        item = this[index];
                        this.InitializeBoundRow(item, dataBoundItem);
                    }
                }
                else
                {
                    item = this.items[index];
                }
            }

            if (item != null)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanged, item, item, index, propertyName));
            }
        }

        /// <summary>
        /// Raises a CollectionChanged notification with action ItemChanging. Must be paired with the NotifyItemChanged method.
        /// </summary>
        /// <param name="item"></param>
        internal void NotifyItemChanging(TDataItem item)
        {
            this.skipListChangedForItem = true;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanging, item));
        }

        internal void NotifyItemChanged(TDataItem item)
        {
            this.skipListChangedForItem = false;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanged, item));
        }

        #endregion

        #region Internal

        void collectionView_CurrentChanged(object sender, EventArgs e)
        {
            if (!this.IsDataBound)
            {
                return;
            }

            if (this.CollectionView.CurrentItem == null)
            {
                return;
            }

            if (this.currencyManager.Position >= 0
                && this.currencyManager.Position < this.Count
                && this[this.currencyManager.Position].Equals(this.CollectionView.CurrentItem))
            {
                return;
            }

            int index = this.IndexOf(this.CollectionView.CurrentItem);
            if (index >= 0)
            {
                this.currencyManager.Position = index;
            }
        }

        void source_BindingContextChanged(object sender, EventArgs e)
        {
            this.bindingContext = this.source.BindingContext;

            try
            {
                if (this.dataSource != null)
                {
                    Bind(this.DataSource, this.DataMember);
                }
            }
            catch (ArgumentException)
            {
                this.DataMember = string.Empty;
                return;
            }
        }

        //TODO: refactoring data bound access
        public object GetBoundValue(object dataBoundItem, string propertyName)
        {
            if (this.IsDataBound)
            {
                PropertyDescriptor descriptor = this.boundProperties.Find(propertyName, true);

                if (descriptor == null)
                {
                    throw new ArgumentException("There is no property descriptor corresponding to property name: " + propertyName);
                }

                object value = descriptor.GetValue(dataBoundItem);

                if (descriptor.PropertyType.IsEnum)
                {
                    value = Enum.ToObject(descriptor.PropertyType, value);
                }

                return value;
            }


            return null;
        }

        private void GetSubPropertyByPath(string propertyPath, object dataObject, out PropertyDescriptor innerDescriptor, out object innerObject)
        {
            string[] names = propertyPath.Split('.');
            innerDescriptor = this.boundProperties[names[0]];

            innerObject = innerDescriptor.GetValue(dataObject);
            for (int index = 1; index < names.Length && (innerDescriptor != null); index++)
            {
                innerDescriptor = innerDescriptor.GetChildProperties()[names[index]];
                if (!(index + 1 == names.Length))
                {
                    innerObject = innerDescriptor.GetValue(innerObject);
                }
            }
        }

        private void SetSubPropertyValue(string propertyPath, object dataObject, object value)
        {
            PropertyDescriptor innerDescriptor = null;
            object innerObject = null;
            this.GetSubPropertyByPath(propertyPath, dataObject, out innerDescriptor, out innerObject);

            if (innerDescriptor != null)
            {
                innerDescriptor.SetValue(innerObject, value);
            }
        }

        public bool SetBoundValue(IDataItem dataItem, string propertyName, object value, string path)
        {
            return this.SetBoundValue(dataItem, propertyName, propertyName, value, path);
        }

        internal bool SetBoundValue(IDataItem dataItem, string propertyName, string columnName, object value, string path)
        {
            if (!this.IsDataBound)
            {
                return false; 
            }

            object dataBoundItem = dataItem.DataBoundItem;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanging, dataItem, dataItem, -1, columnName));   //raise ItemChanging notification
            this.skipListChangedForItem = true;

            if (string.IsNullOrEmpty(path))
            {
                PropertyDescriptor descriptor = this.boundProperties.Find(propertyName, true);
                if (value == null)      //null requires special handling
                {
                    try                 //remove that code when converters are introduced:
                    {
                        descriptor.SetValue(dataBoundItem, value);
                    }
                    catch
                    {
                        descriptor.SetValue(dataBoundItem, DBNull.Value);
                    }
                }
                else
                {
                    Type type = Nullable.GetUnderlyingType(descriptor.PropertyType);
                    if (descriptor.Converter != null && type != null && type.IsGenericType)
                    {
                        value = descriptor.Converter.ConvertFromInvariantString(value.ToString());
                    }

                    descriptor.SetValue(dataBoundItem, value);
                }
            }
            else
            {
                SetSubPropertyValue(path, dataBoundItem, value);
            }

            this.skipListChangedForItem = false;
            int index = this.IndexOf((TDataItem)dataItem);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.ItemChanged, dataItem, dataItem, index, columnName));
            return true;
        }



        public bool SetBoundValue(IDataItem dataItem, string propertyName, object value)
        {
            return this.SetBoundValue(dataItem, propertyName, value, null);
        }

        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the <see cref="RadListSource&lt;T&gt;"/> is bound. 
        /// </summary>
        [Browsable(true), Category("Data"), DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Description("Gets or sets the name of the list or table in the data source for which the GridViewTemplate is displaying data. ")]
        public string DataMember
        {
            get
            {
                return this.dataMember;
            }
            set
            {
                if (value != this.dataMember)
                {
                    this.Bind(this.dataSource, value);
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("DataMember"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the data source of the <see cref="RadListSource&lt;T&gt;"/>.
        /// </summary>
        [Category("Data")]
        [Description("Gets or sets the data source that the GridViewTemplate is displaying data for.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue((string)null)]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (value != this.dataSource)
                {
                    if (this.ShouldChangeDataMember(value))
                    {
                        this.dataMember = string.Empty;
                    }

                    this.Bind(value, this.dataMember);
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("DataSource"));
                }
            }
        }

        private void Bind(object dataSource, string dataMember)
        {
            this.dataSource = dataSource;
            this.dataMember = dataMember;

            this.UnWireEvents();
            object currencyManager = this.currencyManager;
            if (this.bindingContext != null && (this.dataSource != null) && (this.dataSource != Convert.DBNull))
            {
                ISupportInitializeNotification notification = this.dataSource as ISupportInitializeNotification;

                if ((notification != null) && !notification.IsInitialized)
                {
                    notification.Initialized += new EventHandler(notification_Initialized);
                    this.currencyManager = null;
                }
                else
                {
                    this.currencyManager = this.bindingContext[this.dataSource, this.dataMember] as System.Windows.Forms.CurrencyManager;
                    BindToEnumerable();

                    if (this.currencyManager != null)
                    {
                        this.itemType = ListBindingHelper.GetListItemType(this.currencyManager.List);
                        this.itemConstructor = this.itemType.GetConstructor(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                    }
                }
            }
            else
            {
                this.currencyManager = null;
            }

            this.WireEvents();

            if (currencyManager != this.currencyManager)
            {
                Initialize();
            }
        }

        private void BindToEnumerable()
        {
            if (this.currencyManager == null && this.dataSource is IEnumerable)
            {
                List<object> list = new List<object>(255);
                IEnumerator e = (this.dataSource as IEnumerable).GetEnumerator();

                while (e.MoveNext())
                {
                    list.Add(e.Current);
                }

                this.currencyManager = this.bindingContext[new ReadOnlyCollection<object>(list), null] as System.Windows.Forms.CurrencyManager;
            }
        }

        void notification_Initialized(object sender, EventArgs e)
        {
            ISupportInitializeNotification notification = this.dataSource as ISupportInitializeNotification;

            if (notification != null)
            {
                notification.Initialized -= new EventHandler(this.notification_Initialized);
            }

            this.Bind(this.dataSource, this.dataMember);
        }

        private void InitializeBoundRows()
        {
            this.BeginUpdate();

            //reset the current position of the collection view
            this.collectionView.MoveCurrentToPosition(-1);
            this.items.Clear();
            this.items.Capacity = this.currencyManager.List.Count;

            for (int i = 0; i < this.currencyManager.List.Count; i++)
            {
                TDataItem item = (TDataItem)this.source.NewItem();
                this.InitializeBoundRow(item, this.currencyManager.List[i]);
                this.items.Add(item);
            }

            this.EndUpdate();
        }

        protected virtual void InitializeBoundRow(TDataItem item, object dataBoundItem)
        {
            item.DataBoundItem = dataBoundItem;
        }

        private void Initialize()
        {
            this.BeginUpdate();

            this.collectionView.MoveCurrentToPosition(-1);

            if (this.currencyManager != null)
            {
                this.boundProperties = this.currencyManager.GetItemProperties();
                this.InitializeBoundRows();
            }
            else
            {
                this.ClearItems();
                this.boundProperties = null;
            }

            if (this.source != null)
            {
                this.source.Initialize();
            }

            this.EndUpdate();
            this.InitializeCurrentItem();

            if (this.source != null)
            {
                this.source.BindingComplete();
            }
        }

        private void InitializeCurrentItem()
        {
            if (this.currencyManager == null)
            {
                this.collectionView.MoveCurrentToPosition(-1);

                return;
            }

            this.collectionView.MoveCurrentToPosition(this.currencyManager.Position);
        }

        private void WireEvents()
        {
            if (this.currencyManager != null)
            {
                this.currencyManager.PositionChanged += new EventHandler(this.currencyManager_PositionChanged);
                this.currencyManager.ListChanged += new ListChangedEventHandler(this.currencyManager_ListChanged);
            }
        }

        private void UnWireEvents()
        {
            if (this.currencyManager != null)
            {
                this.currencyManager.PositionChanged -= new EventHandler(this.currencyManager_PositionChanged);
                this.currencyManager.ListChanged -= new ListChangedEventHandler(this.currencyManager_ListChanged);
            }
        }

        private void currencyManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (!processListChanged)
            {
                return;
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (e.NewIndex >= 0 && e.NewIndex < this.currencyManager.Count && this.currencyManager.List.Count == this.Count)
                    {
                       this.items[e.NewIndex].DataBoundItem = this.currencyManager.List[e.NewIndex];
                       return;
                    }
                    InsertItem(e.NewIndex, default(TDataItem));
                    break;
                case ListChangedType.ItemChanged:
                    if (!this.skipListChangedForItem)
                    {
                        string propertyName = e.PropertyDescriptor != null ? e.PropertyDescriptor.Name : String.Empty;
                        this.ChangeItem(e.NewIndex, default(TDataItem), propertyName);
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    RemoveItem(e.NewIndex);
                    break;
                case ListChangedType.ItemMoved:
                    MoveItem(e.OldIndex, e.NewIndex);
                    break;
                case ListChangedType.PropertyDescriptorAdded:       //metadata changes
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                    this.Initialize();
                    break;
                case ListChangedType.Reset:
                    this.InitializeBoundRows();
                    this.InitializeCurrentItem();
                    break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is data bound.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is data bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsDataBound
        {
            get
            {
                return (this.dataSource != null && currencyManager != null);
            }
        }

        private void currencyManager_PositionChanged(object sender, EventArgs e)
        {
            if (this.PositionChanged != null)
            {
                this.PositionChanged(this, e);
            }

            if (this.currencyManager.Position >= 0 && this.currencyManager.Position < this.Count)
            {
                TDataItem item = this[this.currencyManager.Position];
                if (item.Equals(this.Current))
                {
                    return;
                }

                this.CollectionView.MoveCurrentTo(item);
            }
        }

        private bool ShouldChangeDataMember(object newDataSource)
        {
            if (this.bindingContext == null)
            {
                return false;
            }

            if (newDataSource != null)
            {
                CurrencyManager manager = this.bindingContext[newDataSource] as CurrencyManager;
                if (manager == null)
                {
                    return false;
                }

                PropertyDescriptorCollection coll = manager.GetItemProperties();
                if ((this.dataMember != null && this.dataMember.Length != 0) && (coll[this.dataMember] != null))
                {
                    return false;
                }
            }

            return true;
        }

        public PropertyDescriptorCollection BoundProperties
        {
            get
            {
                return this.boundProperties;
            }
        }

        #endregion

        #region ITypedList Members

        /// <summary>
        /// Returns the <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties on each item used to bind data.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects to find in the collection as bindable. This can be null.</param>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties on each item used to bind data.
        /// </returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            object list = ListBindingHelper.GetList(this.dataSource);
            if ((list is ITypedList) && !string.IsNullOrEmpty(this.dataMember))
            {
                return ListBindingHelper.GetListItemProperties(list, this.dataMember, listAccessors);
            }

            return ListBindingHelper.GetListItemProperties(this.items, listAccessors);
        }

        /// <summary>
        /// Returns the name of the list.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects, for which the list name is returned. This can be null.</param>
        /// <returns>The name of the list.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return ListBindingHelper.GetListName(this, listAccessors);
        }

        #endregion

        #region ICancelAddNew Members

        void ICancelAddNew.CancelNew(int position)
        {
            if (this.currencyManager == null)
            {
                return;
            }

            this.RemoveAt(position);

            ICancelAddNew list = this.currencyManager.List as ICancelAddNew;
            if (list != null)
            {
                list.CancelNew(position);
            }
        }

        void ICancelAddNew.EndNew(int position)
        {
            if (this.currencyManager != null)
            {
                ICancelAddNew list = this.currencyManager.List as ICancelAddNew;
                if (list != null)
                {
                    list.EndNew(position);
                }
            }

            if (position >= 0 && position < this.Count)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this[position], position));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the NotifyPropertyChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.update == 0 && CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        #endregion

        #region ICurrencyManagerProvider Members

        CurrencyManager ICurrencyManagerProvider.CurrencyManager
        {
            get { return this.currencyManager; }
        }

        CurrencyManager ICurrencyManagerProvider.GetRelatedCurrencyManager(string dataMember)
        {
            if (string.IsNullOrEmpty(dataMember))
            {
                return this.currencyManager;
            }

            if (dataMember.IndexOf(".") != -1)
            {
                return null;
            }

            return ((ICurrencyManagerProvider)this.GetRelatedBindingSource(dataMember)).CurrencyManager;
        }

        private RadListSource<TDataItem> GetRelatedBindingSource(string dataMember)
        {
            if (this.relatedBindingSources == null)
            {
                this.relatedBindingSources = new Dictionary<string, RadListSource<TDataItem>>();
            }
            foreach (string str in this.relatedBindingSources.Keys)
            {
                if (string.Equals(str, dataMember, StringComparison.OrdinalIgnoreCase))
                {
                    return this.relatedBindingSources[str];
                }
            }

            RadListSource<TDataItem> source = new RadListSource<TDataItem>(this.source);
            source.bindingContext = this.bindingContext;
            source.Bind(this.DataSource, dataMember);
            this.relatedBindingSources[dataMember] = source;
            return source;
        }

        #endregion

        #region IList<TDataItem> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(TDataItem item)
        {
            return this.items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, TDataItem item)
        {
            if (IsDataBound)
            {
                throw new InvalidOperationException("Items cannot be inserted to the RadListSource when is in data-bound mode");
            }

            this.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (this.IsDataBound)
            {
                this.currencyManager.List.Remove(this[index].DataBoundItem);

                if (this.currencyManager.List is IBindingList)
                {
                    return;
                }
            }

            this.RemoveItem(index);
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value></value>
        public TDataItem this[int index]
        {
            get
            {
                return this.items[index];
            }
            set
            {
                this.SetItem(index, value);
            }
        }

        #endregion

        #region ICollection<TDataItem> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(TDataItem item)
        {
            if (IsDataBound)
            {
                throw new InvalidOperationException("Items cannot be added to the RadListSource when is in data-bound mode");
            }

            this.InsertItem(this.Count, item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            if (this.IsDataBound)
            {
                this.currencyManager.List.Clear();
                if (this.currencyManager.List is IBindingList)
                {
                    return;
                }
            }

            this.ClearItems();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(TDataItem item)
        {
            return this.items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type T cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(TDataItem[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.items.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                if (this.currencyManager != null)
                {
                    return this.currencyManager.List.IsReadOnly;
                }

                return false;
            }
        }

        /// <summary>
        /// Determines whether this instance is in a Begin/End update block.
        /// </summary>
        public bool IsUpdating
        {
            get
            {
                return this.update > 0;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(TDataItem item)
        {
            int index = this.IndexOf(item);
            if (index != -1)
            {
                if (this.IsDataBound)
                {
                    int count = this.currencyManager.List.Count;

                    IEditableObject itemAsIEditable = item.DataBoundItem as IEditableObject;

                    if (itemAsIEditable != null)
                    {
                        itemAsIEditable.EndEdit();
                    }

                    this.currencyManager.List.Remove(item.DataBoundItem);

                    if (this.currencyManager.List is IBindingList)
                    {
                        return (count != this.currencyManager.List.Count);
                    }
                }

                this.RemoveItem(index);
                return true;
            }

            return false;
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            if (!(value is TDataItem))
            {
                throw new ArgumentException(string.Format("Invalid value type"));
            }

            this.Add((TDataItem)value);
            return this.items.IndexOf((TDataItem)value);
        }

        bool IList.Contains(object value)
        {
            if (value is TDataItem)
            {
                return this.items.Contains((TDataItem)value);
            }

            return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is TDataItem)
            {
                return this.items.IndexOf((TDataItem)value);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (!(value is TDataItem))
            {
                throw new ArgumentException(string.Format("Invalid value type"));
            }

            this.Insert(index, (TDataItem)value);
        }

        bool IList.IsFixedSize
        {
            get
            {
                if (this.currencyManager != null)
                {
                    return this.currencyManager.List.IsFixedSize;
                }

                return false;
            }
        }

        void IList.Remove(object value)
        {
            if (!(value is TDataItem))
            {
                throw new ArgumentException(string.Format("Invalid value type"));
            }

            this.Remove((TDataItem)value);
        }

        object IList.this[int index]
        {
            get
            {
                return this.items[index];
            }
            set
            {
                if (!(value is TDataItem))
                {
                    throw new ArgumentException(string.Format("Invalid value type"));
                }

                this.items[index] = (TDataItem)value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ICollection collection = this.items as ICollection;
            if (collection != null)
            {
                collection.CopyTo(array, index);
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((IList)this.items).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((IList)this.items).SyncRoot; }
        }

        #endregion

        #region IEnumerable<TDataItem> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TDataItem> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion
    }
}
