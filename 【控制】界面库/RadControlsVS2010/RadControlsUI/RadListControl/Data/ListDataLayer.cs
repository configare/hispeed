using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Data;
using System.Collections.Generic;
using Telerik.WinControls.UI.Data;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class ListDataLayer : IDataItemSource
    {
        #region Fields

        private BindingContext bindingContext;
        private RadListSource<RadListDataItem> listSource;
        private string displayMember;
        private string valueMember;
        private RadListDataItemCollection items;
        private RadListElement owner;
        private TypeConverter stringConverter = TypeDescriptor.GetConverter(typeof(string));
        private TypeConverter dataBoundItemConverter = null;
        private bool updateDataItemConverter = false;

        #endregion
        
        #region Initialization

        public ListDataLayer(RadListElement owner)
        {
            this.owner = owner;
            this.listSource = this.CreateListSource();
            this.items = this.CreateItemsCollection(owner);
            this.listSource.CollectionView.GroupDescriptors.Add("GroupKey",ListSortDirection.Ascending);
            this.WireEvents();
        }

        protected virtual void WireEvents()
        {
            this.listSource.CollectionView.CurrentChanging += CollectionView_CurrentChanging;
            this.listSource.CollectionView.CurrentChanged += CollectionView_CurrentChanged;
        }

        protected virtual void UnwireEvents()
        {
            this.listSource.CollectionView.CurrentChanging -= CollectionView_CurrentChanging;
            this.listSource.CollectionView.CurrentChanged -= CollectionView_CurrentChanged;
        }

        protected virtual ListControlListSource CreateListSource()
        {
            return new ListControlListSource(this);
        }

        protected virtual RadListDataItemCollection CreateItemsCollection(RadListElement owner)
        {
            return new RadListDataItemCollection(this, owner);
        }
        
        #endregion

        #region Properties

        public bool ChangeCurrentOnAdd
        {
            get
            {
                return ((RadDataView<RadListDataItem>)this.listSource.CollectionView).ChangeCurrentOnAdd;
            }

            set
            {
                ((RadDataView<RadListDataItem>)this.listSource.CollectionView).ChangeCurrentOnAdd = value;
            }
        }

        public RadListSource<RadListDataItem> ListSource
        {
            get
            {
                return this.listSource;
            }
        }

        public string DataMember 
        { 
            get 
            { 
                return listSource.DataMember; 
            } 
            set 
            { 
                listSource.DataMember = value; 
            } 
        }

        public object DataSource
        {
            get 
            { 
                return listSource.DataSource;
            }
            set
            {
                this.dataBoundItemConverter = null;
                listSource.DataSource = value;
            }
        }

        public string DisplayMember
        {
            get 
            { 
                return this.displayMember; 
            }
            set 
            { 
                this.displayMember = value;
                this.updateDataItemConverter = true;
                if (this.DataSource != null)
                {
                    this.UpdateDataItemsChachedText();
                    this.listSource.CollectionView.LazyRefresh();
                }
            }
        }

        private void UpdateDataItemsChachedText()
        {
            foreach (RadListDataItem item in this.Items)
            {
                item.CachedText = this.GetUnformattedValue(item);
            }
        }

        public string ValueMember
        {
            get 
            { 
                return this.valueMember; 
            }
            set 
            { 
                this.valueMember = value; 
            }
        }

        public RadListDataItem CurrentItem
        {
            get 
            { 
                return this.listSource.CollectionView[this.listSource.CollectionView.CurrentPosition]; 
            }

            set 
            {
                this.listSource.Position = this.listSource.CollectionView.IndexOf(value); 
            }
        }

        public int CurrentPosition
        {
            get 
            {
                return this.DataView.CurrentPosition;
            }

            set 
            {
                if (this.DataView.CurrentPosition == value)
                {
                    return;
                }

                this.DataView.MoveCurrentTo(this.DataView[value]);
            }
        }

        public RadCollectionView<RadListDataItem> DataView
        {
            get 
            { 
                return this.listSource.CollectionView; 
            }
        }

        public RadListDataItemCollection Items
        {
            get 
            { 
                return this.items; 
            }
        }

        public RadListElement Owner
        {
            get
            {
                return this.owner;
            }
        }

        #endregion

        #region Events

        public event PositionChangedEventHandler CurrentPositionChanged;
        public event PositionChangingEventHandler CurrentPositionChanging;
        public event EventHandler Initialized;

        #endregion

        #region Event Handlers

        protected virtual void CollectionView_CurrentChanged(object sender, EventArgs e)
        {
            this.OnCurrentPositionChanged(this.listSource.CollectionView.CurrentPosition);
        }

        protected virtual void CollectionView_CurrentChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = this.OnCurrentPositionChanging(-1);
        }

        #endregion

        #region Public methods

        internal RadListDataGroupItem NewHeaderItem(ListGroup group)
        {
            RadListDataGroupItem result = new RadListDataGroupItem(group);

            result.DataLayer = this;
            result.Owner = this.owner;

            return result;
        }

        public void Refresh()
        {
            this.ListSource.Refresh();
        }

        public object GetDisplayValue(RadListDataItem item)
        {
            if (string.IsNullOrEmpty(this.displayMember))
            {
                return this.GetFormattedValue(item.DataBoundItem);
            }

            object value = null;
            try
            {
                string[] names = this.displayMember.Split('.');
                if (names.Length > 1)
                {
                    value = GetSubPropertyValue(this.displayMember, item.DataBoundItem);
                }
                else
                {
                    value = this.listSource.GetBoundValue(item.DataBoundItem, this.displayMember);
                }
            }
            catch (ArgumentException)
            {
                value = item.DataBoundItem;
                this.DisplayMember = "";
                this.ValueMember = "";
            }

            if (value== null)
            {
                value = "";
            }

            return this.GetFormattedValue(value, TypeDescriptor.GetConverter(value));
        }

        public string GetUnformattedValue(RadListDataItem item)
        {
            if (this.displayMember == "")
            {
                return item.DataBoundItem != null ? item.DataBoundItem.ToString() : "no data to display";
            }

            object boundValue = null;
            try
            {
                string[] names = this.displayMember.Split('.');
                if (names.Length > 1)
                {
                    boundValue = GetSubPropertyValue(this.displayMember, item.DataBoundItem);
                }
                else
                {
                    boundValue = this.listSource.GetBoundValue(item.DataBoundItem, this.displayMember);
                }
            }
            catch (ArgumentException)
            {
                boundValue = item.DataBoundItem;
                this.DisplayMember = "";
                this.ValueMember = "";
            }

            return boundValue != null ? boundValue.ToString() + "" : "";
        }

        public object GetValue(RadListDataItem item)
        {
            object value = null;

            try
            {

                string[] names = this.valueMember.Split('.');
                if (names.Length > 1)
                {
                    value = GetSubPropertyValue(this.valueMember, item.DataBoundItem);
                }
                else
                {
                    value = this.listSource.GetBoundValue(item.DataBoundItem, this.valueMember);
                }
            }
            catch(ArgumentException)
            {
                value = item.DataBoundItem;
                this.DisplayMember = "";
                this.ValueMember = "";
            }

            return value;
        }

        public int GetRowIndex(RadListDataItem item)
        {
            if (this.owner.ShowGroups)
            {
                int position = 0;
                foreach (ListGroup group in this.owner.Groups)
                {
                    if (group.Collapsed)
                    {
                        continue;
                    }

                    foreach (RadListDataItem listItem in group.Items)
                    {
                        if (item.Equals(listItem))
                        {
                            return position;
                        }

                        position++;
                    }
                }
            }
            else
            {
                return this.listSource.CollectionView.IndexOf(item);
            }

            return -1;
        }

        public RadListDataItem GetItemAtIndex(int index)
        {
            if (this.owner.ShowGroups)
            {
                int position = 0;
                foreach (ListGroup group in this.owner.Groups)
                {
                    if (group.Collapsed)
                    {
                        continue;
                    }

                    foreach (RadListDataItem listItem in group.Items)
                    {
                        if (position == index)
                        {
                            return listItem;
                        }

                        position++;
                    }
                }
            }

            return this.Items[index];
        }

        public int GetVisibleItemsCount()
        {
            if (this.owner.ShowGroups)
            {
                int count = 0;
                foreach (ListGroup group in this.owner.Groups)
                {
                    if (group.Collapsed)
                    {
                        continue;
                    }

                    count += group.Items.Count;
                }

                return count;
            }

            return this.Items.Count;
        }

        public List<RadListDataItem> GetItemRange(int startIndex, int endIndex)
        {
            List<RadListDataItem> result = new List<RadListDataItem>();
            if (this.owner.ShowGroups)
            {
                int position = 0;
                foreach (Group<RadListDataItem> group in this.DataView.Groups)
                {
                    foreach (RadListDataItem listItem in group.Items)
                    {
                        if (startIndex <= position && position <= endIndex)
                        {
                            result.Add(listItem);
                        }
                        position++;
                    }
                }
            }
            else
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    result.Add(this.DataView[i]);
                }
            }

            return result;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnCurrentPositionChanged(int newPosition)
        {
            if (this.CurrentPositionChanged != null)
            {
                PositionChangedEventArgs args = new PositionChangedEventArgs(newPosition);
                this.CurrentPositionChanged(this, args);
            }
        }

        protected virtual bool OnCurrentPositionChanging(int newPosition)
        {
            if (this.CurrentPositionChanging != null)
            {
                PositionChangingCancelEventArgs args = new PositionChangingCancelEventArgs(newPosition);
                this.CurrentPositionChanging(this, args);
                return args.Cancel;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void GetSubPropertyByPath(string propertyPath, object dataObject, out PropertyDescriptor innerDescriptor, out object innerObject)
        {
            string[] names = propertyPath.Split('.');
            innerDescriptor = this.ListSource.BoundProperties[names[0]];

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

        private object GetSubPropertyValue(string propertyPath, object dataObject)
        {
            PropertyDescriptor innerDescriptor = null;
            object innerObject = null;
            this.GetSubPropertyByPath(propertyPath, dataObject, out innerDescriptor, out innerObject);

            if (innerDescriptor != null)
            {
                return innerDescriptor.GetValue(innerObject);
            }

            return null;
        }

        private string GetFormattedValue(object value, TypeConverter valueConverter)
        {
            string valueToString = "";

            if (this.owner.FormattingEnabled)
            {
                valueToString = (string)Formatter.FormatObject(value, typeof(string), valueConverter, this.stringConverter, this.owner.FormatString, this.owner.FormatInfo, null, null);
            }
            else
            {
                if (valueConverter != null)
                {
                    if (valueConverter.CanConvertTo(typeof(string)))
                    {
                        valueToString = valueConverter.ConvertToString(value);
                    }
                    else if (value != null)
                    {
                        valueToString = value.ToString();
                    }
                }
                else if (value != null)
                {
                    valueToString = value.ToString();
                }
            }

            return valueToString;
        }

        private string GetFormattedValue(object value)
        {
            if (this.dataBoundItemConverter == null || this.updateDataItemConverter)
            {
                this.dataBoundItemConverter = TypeDescriptor.GetConverter(value);
                this.updateDataItemConverter = false;
            }

            return this.GetFormattedValue(value, this.dataBoundItemConverter);
        }

        #endregion

        #region IDataItemSource Members

        public IDataItem NewItem()
        {
            RadListDataItem result = this.owner.OnListItemDataBinding();

            if(result == null)
            {
                result = new RadListDataItem();
            }
            result.DataLayer = this;
            result.Owner = this.owner;

            return result;
        }

        void IDataItemSource.BindingComplete()
        {
        }

        public void Initialize()
        {
            if (Initialized != null)
            {
                Initialized(this, EventArgs.Empty);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BindingContext BindingContext
        {
            get
            {
                return this.bindingContext;
            }
            internal set
            {
                if (this.bindingContext != value)
                {
                    this.bindingContext = value;
                    if (BindingContextChanged != null)
                    {
                        BindingContextChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public event EventHandler BindingContextChanged;

        #endregion
    }

    public class ListControlListSource : RadListSource<RadListDataItem>
    {
        private ListDataLayer dataLayer;
        private bool resetting = false;

        public ListControlListSource(IDataItemSource owner) : base(owner)
        {
            Debug.Assert(owner is ListDataLayer);
            this.dataLayer = (ListDataLayer)owner;
        }

        public bool Resetting
        {
            get
            {
                return this.resetting;
            }

            private set
            {
                this.resetting = value;
            }
        }

        protected override void InitializeBoundRow(RadListDataItem item, object dataBoundItem)
        {
            item.SetDataBoundItem(true, dataBoundItem);

            if (this.Resetting)
            {
                if (item.Selected)
                {
                    RadListDataItemSelectedCollection selectedItems = (RadListDataItemSelectedCollection)this.dataLayer.Owner.SelectedItems;
                    Debug.Assert(!selectedItems.Contains(item));
                    selectedItems.Add(item);
                }
            }
        }

        public override void Reset()
        {
            this.Resetting = true;
            base.Reset();
            this.Resetting = false;
        }
    }
}
