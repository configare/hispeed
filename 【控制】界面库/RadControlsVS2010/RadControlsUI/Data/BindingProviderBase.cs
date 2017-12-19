using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Data
{

    /// <summary>
    /// Represents a base class that the <see cref="Telerik.WinControls.UI.UIElements.ListBox.Data"/> and the ResourceBindingProvider classes extend.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BindingProviderBase<T> : IBindingProvider<T> where T : IDataBoundItem
    {
        private IPropertyMappingInfo mappings = null;
        protected List<T> logicalItems = null;
        private bool innerChange = false;
        private bool bindingComplete = true;

        protected IBindableComponent owner = null;
        protected BindingContext bindingContext = null;
        protected CurrencyManager currencyManager = null;
        protected object dataSource = null;
        protected string bindingPath = "";

        protected PropertyDescriptorCollection dataItemProperties = null;
        protected PropertyDescriptorCollection logicalItemProperties = null;

        public event PositionChangedEventHandler PositionChanged;

        /// <summary>
        /// Fires the PositionChanged event.
        /// </summary>
        /// <param name="newPos">The new position.</param>
        protected virtual void OnPositionChanged(int newPos)
        {
            if (this.PositionChanged != null)
            {
                this.PositionChanged(this, new PositionChangedEventArgs(newPos));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingProviderBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="bindableComponent">The owner.</param>
        protected BindingProviderBase(IBindableComponent bindableComponent)
        {
            this.owner = bindableComponent;
            if (this.owner != null)
            {
                this.bindingContext = owner.BindingContext;
            }

            this.logicalItems = new List<T>();

            this.UpdateLogicalPropertiesCache();
        }

        /// <summary>
        /// Gets or sets the position in the currency manager.
        /// </summary>
        public int Position
        {
            get
            {
                return this.currencyManager != null ? this.currencyManager.Position : -1;
            }

            set
            {
                if (this.currencyManager != null)
                {
                    if (value > -1 && value < this.currencyManager.List.Count)
                    {
                        this.currencyManager.Position = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the binding context. Setting the binding context causes a BindingProviderBase to rebind itself.
        /// </summary>
        public BindingContext BindingContext
        {
            get
            {
                return this.bindingContext;
            }

            set
            {
                if (this.bindingContext == value)
                {
                    return;
                }

                this.bindingContext = value;

                if (this.bindingContext == null)
                {
                    UnwireCurrencyManagerEvents();
                    //this.currencyManager = null;
                }
                else
                {
                    Rebind();
                }
            }
        }

        /// <summary>
        /// Sets the data source to the BindingProviderBase. This triggers items creation and removal or previous binding.
        /// </summary>
        public void SetDataSource(object value, string bindingPath)
        {
            this.bindingPath = bindingPath;

            if (this.dataSource == value)
            {
                return;
            }

            if (this.dataSource is IComponent)
            {
                (this.dataSource as IComponent).Disposed -= BindingProviderBase_Disposed;
            }

            if (this.dataSource is ISupportInitializeNotification)
            {
                (this.dataSource as ISupportInitializeNotification).Initialized -= new EventHandler(BindingProviderBase_Initialized);
            }

            this.dataSource = value;

            if (this.dataSource is ISupportInitializeNotification)
            {
                ISupportInitializeNotification tmp = this.dataSource as ISupportInitializeNotification;
                tmp.Initialized += BindingProviderBase_Initialized;
                if (!tmp.IsInitialized)
                {
                    return;
                }
            }
            
            if (this.bindingContext == null)
            {
                return;
            }

            if (this.dataSource != null)
            {
                Bind(this.dataSource);
            }
            else
            {
                Unbind();
            }
        }

        private void BindingProviderBase_Initialized(object sender, EventArgs e)
        {
            if (this.bindingContext == null)
            {
                return;
            }

            Bind(this.dataSource);
        }

        /// <summary>
        /// Gets the data source that was assigned.
        /// </summary>
        /// <returns>An object reference to the currently set data source.</returns>
        public object GetDataSource()
        {
            return this.dataSource;
        }

        /// <summary>
        /// Rebinds the BindingProviderBase.
        /// </summary>
        public void Rebind()
        {
            if (this.bindingContext == null)
            {
                return;
            }

            if (this.dataSource != null)
            {
                if (this.dataSource is ISupportInitializeNotification)
                {
                    if (!(this.dataSource as ISupportInitializeNotification).IsInitialized)
                    {
                        return;
                    }
                }

                CurrencyManager cm = this.bindingContext[this.dataSource, this.bindingPath] as CurrencyManager;

                if (cm != null && this.currencyManager != null)
                {
                    if (cm.List == this.currencyManager.List)
                    {
                        return;
                    }
                }

                object oldDs = this.dataSource;
                Unbind();
                Bind(oldDs);
            }
        }

        /// <summary>
        /// Subscribes to currency manager events after unsubscribing from the previous CurrencyManager if any.
        /// Unbinds if the BindingManagerBase returned from the BindingContext is not a CurrencyManager.
        /// </summary>
        /// <param name="source"></param>
        protected void Bind(object source)
        {
            if (this.bindingContext == null)
            {
                return;
            }

            BindingManagerBase bmb = null;
            bmb = this.bindingContext[source, this.bindingPath];
            
            if (bmb is CurrencyManager)
            {
                this.dataSource = source;
                UnwireCurrencyManagerEvents();
                this.currencyManager = bmb as CurrencyManager;

                this.UpdateDataPropertiesCache();
                this.UpdateLogicalPropertiesCache();
                this.BuildLogicalItemList(null);

                this.currencyManager.ListChanged += ListChanged;
                this.currencyManager.PositionChanged += currencyManager_PositionChanged;


                if (this.dataSource is IComponent)
                {
                    IComponent tmp = this.dataSource as IComponent;
                    tmp.Disposed += new EventHandler(BindingProviderBase_Disposed);
                }

                OnItemsChanged(new ListChangedEventArgs<T>(ListChangedType.Reset));
            }
            else
            {
                Unbind();
            }
        }

        /// <summary>
        /// Unsubscribes from CurrencyManager events and clears the logical items.
        /// </summary>
        protected void Unbind()
        {
            RemoveLogicalItems();
            UnwireCurrencyManagerEvents();
            this.currencyManager = null;
            this.dataItemProperties = null;
            this.logicalItemProperties = null;

            if (this.dataSource is IComponent)
            {
                (this.dataSource as IComponent).Disposed -= new EventHandler(BindingProviderBase_Disposed);
            }

            if (this.dataSource is ISupportInitializeNotification)
            {
                (this.dataSource as ISupportInitializeNotification).Initialized -= this.BindingProviderBase_Initialized;
            }

            this.dataSource = null;

            OnItemsChanged(new ListChangedEventArgs<T>(ListChangedType.Reset));
        }

        private void RemoveLogicalItems()
        {
            if (this.logicalItems != null)
            {
                this.logicalItems.Clear();
            }
        }

        private void BindingProviderBase_Disposed(object sender, EventArgs e)
        {
            Unbind();
        }

        private void UnwireCurrencyManagerEvents()
        {
            if (this.currencyManager != null)
            {
                this.currencyManager.ListChanged -= ListChanged;
                this.currencyManager.PositionChanged -= currencyManager_PositionChanged;
            }
        }

        protected void currencyManager_PositionChanged(object sender, EventArgs e)
        {
            OnPositionChanged(this.currencyManager.Position);
        }

        protected void UpdateDataPropertiesCache()
        {
            if (this.currencyManager == null)
            {
                return;
            }

            this.dataItemProperties = ListBindingHelper.GetListItemProperties(this.currencyManager.List);
        }

        protected void UpdateLogicalPropertiesCache()
        {
            T logicalItem = this.CreateInstance();
            this.logicalItemProperties = GetListItemPropertiesByType(logicalItem.GetType());
        }

        //private void CurrencyManager_MetaDataChanged(object sender, EventArgs e)
        //{
        //    if (this.bindingComplete)
        //    {
        //        this.BuildLogicalItemList(null);
        //    }
        //}

        ///// <summary>
        ///// Raises the <see cref="E:System.Windows.Forms.BindingSource.BindingComplete"></see> event.
        ///// </summary>
        ///// <param name="e">A <see cref="T:System.Windows.Forms.BindingCompleteEventArgs"></see>  that contains the event data.</param>
        //protected override void OnBindingComplete(BindingCompleteEventArgs e)
        //{
        //    base.OnBindingComplete(e);
        //    this.bindingComplete = true;
        //}

        private bool trackDataSourceChanges = true;

        [DefaultValue(true)]
        public bool TrackDataSourceChanges
        {
            get
            {
                return this.trackDataSourceChanges;
            }
            set
            {
                this.trackDataSourceChanges = value;
            }
        }

        /// <summary>
        /// Updates a logical item according to the added property mappings.
        /// </summary>
        /// <param name="index">The index of the item to update.</param>
        protected void UpdateLogicalItem(int index)
        {
            IEnumerator<PropertyMapping> enumerator = this.mappings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PropertyDescriptor logical = logicalItemProperties.Find(enumerator.Current.LogicalItemProperty, false);
                PropertyDescriptor data = dataItemProperties.Find(enumerator.Current.DataSourceItemProperty, false);

                if (logical != null)
                {
                    if (data != null)
                    {
                        logical.SetValue(this.logicalItems[index], data.GetValue(this.currencyManager.List[index]));
                    }
                    else
                    {
                        if (enumerator.Current.ConvertToLogicalValue != null)
                        {
                            logical.SetValue(this.logicalItems[index], enumerator.Current.ConvertToLogicalValue(this.currencyManager.List[index], data.Converter));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.BindingSource.ListChanged"></see> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected void ListChanged(object sender, ListChangedEventArgs e)
        {
            ListChangedEventArgs<T> args = null;
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    args = new ListChangedEventArgs<T>(e.ListChangedType, this.logicalItems[e.NewIndex], e.NewIndex);
                    UpdateLogicalItem(e.NewIndex);
                    break;

                case ListChangedType.ItemMoved:
                    args = new ListChangedEventArgs<T>(e.ListChangedType);
                    break;

                case ListChangedType.ItemAdded:
                    T newItem = GetLogicalItem(this.dataItemProperties, this.currencyManager.List[e.NewIndex]);
                    this.logicalItems.Insert(e.NewIndex, newItem);
                    args = new ListChangedEventArgs<T>(e.ListChangedType, newItem, e.NewIndex);
                    if (this.currencyManager.List.Count == 1)
                    {
                        this.UpdateDataPropertiesCache();
                    }
                    break;

                case ListChangedType.ItemDeleted:
                    T removedItem = this.logicalItems[e.NewIndex];
                    args = new ListChangedEventArgs<T>(e.ListChangedType, removedItem, e.NewIndex);
                    this.logicalItems.Remove(removedItem);
                    if (this.currencyManager.List.Count == 0)
                    {
                        this.UpdateDataPropertiesCache();
                    }
                    break;

                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                        args = new ListChangedEventArgs<T>(e.ListChangedType);
                        this.UpdateDataPropertiesCache();
                    break;

                case ListChangedType.Reset:
                    {
                        args = new ListChangedEventArgs<T>(e.ListChangedType);
                        this.UpdateDataPropertiesCache();
                        if (this.bindingComplete && !this.innerChange)
                        {
                            this.bindingComplete = false;
                            this.BuildLogicalItemList(null);
                            this.bindingComplete = true;
                        }
                    }
                    break;

                default:
                    args = new ListChangedEventArgs<T>(e.ListChangedType);
                    break;
            }

            if (this.trackDataSourceChanges)
            {
                this.OnItemsChanged(args);
            }
        }

        ///// <summary>
        ///// Raises the <see cref="E:System.Windows.Forms.BindingSource.DataMemberChanged"></see> event.
        ///// </summary>
        ///// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        //protected override void OnDataMemberChanged(EventArgs e)
        //{
        //    base.OnDataMemberChanged(e);

        //    this.UpdateDataPropertiesCache();
        //}

        ///// <summary>
        ///// Raises the <see cref="E:System.Windows.Forms.BindingSource.DataSourceChanged"></see> event.
        ///// </summary>
        ///// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        //protected override void OnDataSourceChanged(EventArgs e)
        //{
        //    base.OnDataSourceChanged(e);

        //    this.UpdateDataPropertiesCache();
        //}

        /// <summary>
        /// Finds the corresponding data item for a given logical item.
        /// </summary>
        /// <param name="logicalItem">The logical item.</param>
        /// <returns></returns>
        protected virtual object FindCorrespondingDataItem(T logicalItem)
        {
            object item = null;
            if (this.logicalItems != null)
            {
                int index = this.logicalItems.IndexOf(logicalItem);
                if (index > -1)
                {
                    item = this.currencyManager.List[index];
                }
            }

            return item;
        }

        #region IBindingProvider<T> Members

        /// <summary>
        /// Creates logical items from the items in the data source and returns them.
        /// </summary>
        /// <param name="filterFunction">An optional filter function.</param>
        /// <returns></returns>
        public IEnumerable<T> GetItems(Predicate<T> filterFunction)
        {
            if(this.currencyManager == null)
            {
                return logicalItems;
            }

            //if (this.logicalItems == null || this.logicalItems.Count == 0)
            {
                this.BuildLogicalItemList(filterFunction);
            }

            return this.logicalItems;
        }

        /// <summary>
        /// Inserts the specified item in the data source.
        /// </summary>
        /// <param name="itemToInsert">The item to insert.</param>
        public void Insert(T itemToInsert)
        {
            //this.innerChange = true;
            //object item = this.AddNew();
            //this.UpdateDataItemProperties(item, itemToInsert);
            //this.EndEdit();
            //this.logicalItems.Add(itemToInsert);
            //this.UpdateChildItems();
            //this.innerChange = false;
        }

        protected virtual void UpdateChildItems()
        {
        }

        /// <summary>
        /// Updates the specified item in the data source.
        /// </summary>
        /// <param name="itemToUpdate">The item to update.</param>
        /// <param name="propertyName">Name of the property which value changed.
        /// Null or an empty string if all properties should be updated.</param>
        public void Update(T itemToUpdate, string propertyName)
        {
            //object item = this.FindCorrespondingDataItem(itemToUpdate);
            //if (item == null)
            //{
            //    return;
            //}

            //this.innerChange = true;

            //PropertyMapping mapping = GetLogicalPropertyMapping(propertyName);
            //if (mapping != null)
            //{
            //    this.UpdateDataItemProperty(item, itemToUpdate, mapping);
            //}
            //else
            //{
            //    this.UpdateDataItemProperties(item, itemToUpdate);
            //}
            //this.EndEdit();
            //this.UpdateChildItems();

            //this.innerChange = false;
        }

        private PropertyMapping GetLogicalPropertyMapping(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            return this.PropertyMappings.FindByLogicalItemProperty(propertyName);
        }

        /// <summary>
        /// Deletes the specified item from the data source.
        /// </summary>
        /// <param name="itemToDelete">The item to delete.</param>
        public void Delete(T itemToDelete)
        {
            //object item = this.FindCorrespondingDataItem(itemToDelete);
            //if (item != null)
            //{
            //    this.innerChange = true;
            //    this.Remove(item);
            //    this.EndEdit();
            //    this.logicalItems.Remove(itemToDelete);
            //    this.innerChange = false;
            //}
        }

        public event EventHandler<ListChangedEventArgs<T>> ItemsChanged;

        /// <summary>
        /// Raises the <see cref="E:ItemsChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Telerik.WinControls.UI.Data.ListChangedEventArgs&lt;T&gt;"/> instance containing the event data.</param>
        protected virtual void OnItemsChanged(ListChangedEventArgs<T> args)
        {
            if (this.ItemsChanged != null)
            {
                this.ItemsChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets the mapping information that connects properties of logical items 
        /// with properties from the data source.
        /// </summary>
        /// <value>The mapping.</value>
        public virtual IPropertyMappingInfo PropertyMappings
        {
            get
            {
                return this.mappings;
            }
            set
            {
                if (this.mappings != value)
                {
                    this.mappings = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Creates a specific logical item.
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateInstance();

        private void BuildLogicalItemList(Predicate<T> filterFunction)
        {
            if (this.currencyManager == null)
            {
                return;
            }

            if (this.logicalItems == null)
            {
                this.logicalItems = new List<T>();
            }
            else
            {
                this.logicalItems.Clear();
            }

            //PropertyDescriptorCollection properties =
            //    ListBindingHelper.GetListItemProperties(this.List);

            this.OnCreateLogicalItemsBegin(this.dataItemProperties);

            // populating events
            if (filterFunction == null)
            {
                filterFunction = this.DefaultFilterFunction;
            }

            object item = null;
            for (int i = 0; i < this.currencyManager.List.Count; i++)
            {
                item = this.currencyManager.List[i];
                T instance = this.GetLogicalItem(this.dataItemProperties, item);

                if (filterFunction(instance))
                {
                    this.logicalItems.Add(instance);
                }
            }

            this.OnCreateLogicalItemsEnd();
        }

        private bool DefaultFilterFunction(T item)
        {
            return true;
        }

        /// <summary>
        /// Called before all logical items are extracted from the data source.
        /// </summary>
        protected virtual void OnCreateLogicalItemsBegin(PropertyDescriptorCollection properties)
        {
        }

        /// <summary>
        /// Called after all logical items are extracted from the data source.
        /// </summary>
        protected virtual void OnCreateLogicalItemsEnd()
        {

        }

        private static PropertyDescriptorCollection GetListItemPropertiesByType(Type type)
        {
            return TypeDescriptor.GetProperties(type);
        }

        /// <summary>
        /// Creates a logical item given a data source item.
        /// </summary>
        /// <param name="sourceProperties">The source properties.</param>
        /// <param name="item">The data source item.</param>
        /// <returns></returns>
        protected virtual T GetLogicalItem(PropertyDescriptorCollection sourceProperties, object item)
        {
            T logicalItem = this.CreateInstance();

            PropertyDescriptorCollection targetProperties =
                GetListItemPropertiesByType(logicalItem.GetType());

            IEnumerator<PropertyMapping> enumerator = this.PropertyMappings.GetEnumerator();

            while (enumerator.MoveNext())
            {
                PropertyMapping mapping = enumerator.Current;
                if (!this.ShouldApplyMapping(mapping))
                {
                    continue;
                }

                PropertyDescriptor sourceProp = sourceProperties.Find(mapping.DataSourceItemProperty, true);
                if (sourceProp != null)
                {
                    object value = sourceProp.GetValue(item);
                    PropertyDescriptor targetProp = targetProperties.Find(mapping.LogicalItemProperty, true);
                    if (targetProp != null)
                    {
                        if (mapping.ConvertToLogicalValue != null)
                        {
                            value = mapping.ConvertToLogicalValue(value, sourceProp.Converter);
                        }
                        targetProp.SetValue(logicalItem, value);
                    }
                    else
                    {
                        this.ProcessDataSourceValue(logicalItem, mapping, value);
                    }
                }
                else
                {
                    PropertyDescriptor targetProp = targetProperties.Find(mapping.LogicalItemProperty, true);
                    if (targetProp != null)
                    {
                        this.OnSourcePropertyNotFound(item, mapping, targetProp, logicalItem);
                    }
                }
            }

            logicalItem.SetDataItem(item);
            return logicalItem;
        }

        protected virtual void OnSourcePropertyNotFound(object dataItem, PropertyMapping mapping, PropertyDescriptor targetProp, object logicalItem)
        {
            //called in inheritors
        }

        protected virtual bool ShouldApplyMapping(PropertyMapping mapping)
        {
            return true;
        }

        /// <summary>
        /// Called to process a value from the data source when the target sheduler item property 
        /// as per the mapping cannot be found.
        /// </summary>
        /// <param name="logicalItem">The logical item.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="value">The value.</param>
        protected virtual void ProcessDataSourceValue(T logicalItem, PropertyMapping mapping, object value)
        {
        }

        /// <summary>
        /// Updates the data item properties.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="logicalItem">The logical item.</param>
        protected virtual void UpdateDataItemProperties(object item, T logicalItem)
        {
            //this.OnUpdateDataSourceItemBegin();

            IEnumerator<PropertyMapping> enumerator = this.PropertyMappings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PropertyMapping mapping = enumerator.Current;
                if (!this.UpdateDataItemProperty(item, logicalItem, mapping))
                {
                    continue;
                }
            }

            //this.OnUpdateDataSourceItemEnd();
        }

        protected virtual bool UpdateDataItemProperty(object item, T logicalItem, PropertyMapping mapping)
        {
            if (!this.ShouldApplyMapping(mapping))
            {
                return false;
            }
            PropertyDescriptor dataProp = this.logicalItemProperties.Find(mapping.LogicalItemProperty, true);
            PropertyDescriptor itemProp = this.dataItemProperties.Find(mapping.DataSourceItemProperty, true);
            if (dataProp != null)
            {
                if (itemProp != null)
                {
                    object value = dataProp.GetValue(logicalItem);
                    if (mapping.ConvertToDataSourceValue != null)
                    {
                        value = mapping.ConvertToDataSourceValue(value, dataProp.Converter);
                    }

                    if (!itemProp.IsReadOnly)
                    {
                        itemProp.SetValue(item, value);
                    }
                }
            }
            else if (itemProp != null)
            {
                this.ProcessLogicalItem(logicalItem, item, mapping, itemProp);
            }

            return true;
        }

        //protected virtual void OnUpdateDataSourceItemBegin()
        //{
        //}

        //protected virtual void OnUpdateDataSourceItemEnd()
        //{
        //}

        protected virtual void ProcessLogicalItem(T logicalItem, object dataSourceItem, PropertyMapping mapping, PropertyDescriptor dataItemProperty)
        {
        }

        protected virtual string GetSourcePropertyName(PropertyDescriptor property)
        {
            return property.Name;
        }
    }
}
