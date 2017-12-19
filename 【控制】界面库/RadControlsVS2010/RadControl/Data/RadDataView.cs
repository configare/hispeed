using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    public class RadDataView<TDataItem> : RadCollectionView<TDataItem>
         where TDataItem : IDataItem
    {
        #region Fields

        private bool canFilter = true;
        private bool canSort = true;
        private bool canGroup = true;

        //default implementation
        private IComparer<TDataItem> comparer;
        private Index<TDataItem> indexer;
        private GroupBuilder<TDataItem> groupBuilder;
        private int changingIndex;

        #endregion

        #region Constructors

        public RadDataView(IEnumerable<TDataItem> collection)
            : base(collection)
        {
            this.comparer = new DataItemComparer<TDataItem>(this.SortDescriptors);
            this.indexer = new AvlIndex<TDataItem>(this);
            this.groupBuilder = new GroupBuilder<TDataItem>(this.indexer);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sort comparer.
        /// </summary>
        /// <value>The comparer.</value>
        public override IComparer<TDataItem> Comparer
        {
            get { return this.comparer; }
            set
            {
                if (this.comparer != value)
                {
                    this.comparer = value;

                    this.indexer = new AvlIndex<TDataItem>(this);
                    this.groupBuilder = new GroupBuilder<TDataItem>(this.indexer);
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("Comparer"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the group comparer.
        /// </summary>
        /// <value>The group comparer.</value>
        public override IComparer<Group<TDataItem>> GroupComparer
        {
            get
            {
                return this.groupBuilder.Comparer;
            }
            set
            {
                this.groupBuilder.Comparer = value;
            }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public override GroupCollection<TDataItem> Groups
        {
            get { return this.groupBuilder.Groups; }
        }

        /// <summary>
        /// Gets or sets the group predicate.
        /// </summary>
        /// <value>The group predicate.</value>
        public override GroupPredicate<TDataItem> GroupPredicate
        {
            get
            {
                return this.groupBuilder.GroupPredicate;
            }
            set
            {
                if (this.groupBuilder.GroupPredicate != value)
                {
                    this.groupBuilder.GroupPredicate = value;
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("GroupPredicate"));
                }
            }
        }

        /// <summary>
        /// Gets the default group predicate.
        /// </summary>
        /// <value>The default group predicate.</value>
        public override GroupPredicate<TDataItem> DefaultGroupPredicate
        {
            get
            {
                return this.groupBuilder.DefaultGroupPredicate;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this data view can filter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can filter; otherwise, <c>false</c>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool CanFilter
        {
            get { return this.canFilter; }
            internal set
            {
                if (this.canFilter != value)
                {
                    this.canFilter = value;
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("Filter"));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this data view can group.
        /// </summary>
        /// <value><c>true</c> if this instance can group; otherwise, <c>false</c>.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool CanGroup
        {
            get { return this.canGroup; }
            internal set
            {
                if (this.CanGroup != value)
                {
                    this.canGroup = value;
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("GroupDescriptors"));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this data view can sort.
        /// </summary>
        /// <value><c>true</c> if this instance can sort; otherwise, <c>false</c>.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool CanSort
        {
            get { return this.canSort; }
            internal set
            {
                if (this.canSort != value)
                {
                    this.canSort = value;
                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("SortDescriptors"));
                }
            }
        }

        #endregion

        #region Methods

        public override int IndexOf(TDataItem item)
        {
            int index = base.IndexOf(item);

            while (index >= 0 && !this[index].Equals(item))
            {
                index++;

                if (index >= this.Count)
                {
                    return -1;
                }
            }

            return index;
        }

        public override TDataItem Find(int itemIndex, object dataBoundItem)
        {
            if (!this.HasDataOperation)
            {
                return base.Find(itemIndex, dataBoundItem);
            }

            TDataItem result = default(TDataItem);

            //TODO: Think of a look-up semantic by data-bound item
            //IAvlEnumerator<TDataItem> en = this.indexer.GetAvlEnumerator();
            //while (en.MoveNext())
            //{
            //    if (object.Equals(en.Current.DataBoundItem, dataBoundItem))
            //    {
            //        result = en.Current;
            //        break;
            //    }
            //}

            //en.Dispose();

            return result;
        }

        #endregion

        #region Internal

        protected override void RefreshOverride()
        {
            if (this.IsInUpdate)
            {
                this.EnsureDescriptorIndex();
                return;
            }

            RebuildData(true);
        }

        protected override IList<TDataItem> Items
        {
            get
            {
                return this.indexer.Items;
            }
        }

        protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnNotifyPropertyChanged(e);

            if (e.PropertyName == "FilterExpression" ||
                e.PropertyName == "Filter" ||
                e.PropertyName == "SortDescriptors" ||
                e.PropertyName == "Comparer" ||
                e.PropertyName == "GroupDescriptors" ||
                e.PropertyName == "GroupPredicate")
            {
                RefreshOverride();
            }
        }

        protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (!this.HasDataOperation)
            {
                //this.view = this.sourceList;
                //this.readOnlyGroups = this.GroupFactory.CreateCollection(new ObservableCollection<Group<TDataItem>>());
            }

            bool notify = false;
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    notify = AddItem((TDataItem)args.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    notify = RemoveItem(args);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    notify = ReplaceItem(args);
                    break;
                case NotifyCollectionChangedAction.Move:
                    notify = MoveItem(args);
                    break;
                case NotifyCollectionChangedAction.ItemChanging:
                    this.OnItemChanging((TDataItem)args.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.ItemChanged:
                    notify = ChangeItem((TDataItem)args.NewItems[0], args.PropertyName);
                    this.changingIndex = -1;
                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Batch:
                    RefreshOverride();
                    return;
            }

            if (notify)
            {
                base.ProcessCollectionChanged(args);
            }
        }

        private void RebuildData(bool notify)
        {
            EnsureDescriptorIndex(); //TODO: optimize this!
            //this.indexer.Perform();

            if (notify)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            SyncCurrent(this.CurrentItem);
        }

        private void SyncCurrent(TDataItem item)
        {
            if (item == null)
            {
                if (this.Count > 0)
                {
                    item = this.Items[0];
                }
            }

            if (item == null)
            {
                return;
            }

            if (!this.PassesFilter(item))
            {
                //previous current item does not pass the filter, set current to the first available item.
                if (this.Count >= 0)
                {
                    int position = this.Count == 0 ? -1 : 0;
                    this.SetCurrentPositionCore(position, true);
                }
                return;
            }

            //consider position changed upon data operation
            if (this.HasDataOperation)
            {
                int index = this.IndexOf(item);
                if (index != this.CurrentPosition)
                {
                    this.MoveCurrentToPosition(index);
                }
            }
        }

        private void OnItemChanging(TDataItem item)
        {
            if (!this.HasDataOperation)
            {
                return;
            }

            this.changingIndex = this.Items.IndexOf(item);
        }

        private void EnsureDescriptorIndex()
        {
            if (this.SourceCollection == null)
            {
                return;
            }

            RadCollectionView<TDataItem>.RebuildDescriptorIndex(this.SourceCollection, this.SortDescriptors, this.GroupDescriptors);
        }

        private void UpdateItemSorted(TDataItem item, string propertyName)
        {
            if (this.SortDescriptors.Contains(propertyName))
            {
                this.indexer.Items.RemoveAt(this.changingIndex);
                this.indexer.Items.Add(item);
            }
        }

        private bool HasFields
        {
            get
            {
                IEnumerator<TDataItem> e = this.SourceCollection.GetEnumerator();
                e.MoveNext();
                if (e.Current != null)
                {
                    return (e.Current.FieldCount != 0);
                }

                return false;
            }
        }

        private bool AddItem(TDataItem item)
        {
            if (!this.PassesFilter(item))
            {
                return false;
            }

            ////bool group = true;
            //if (this.SortDescriptors.Count > 0)
            //{
            //    ////view will not be updated to the avl tree if data operation is applied without any item present
            //    //if (this.view != this.indexer.Items)
            //    //{
            //    //    this.RebuildData(false);
            //    //    //group = false;
            //    //}
            //    //else
            //    {
            //        this.indexer.Items.Add(item);
            //    }
            //}
            //else if (this.HasFilter)
            //{
            //    this.view.Add(item);
            //}

            // this.indexer.Items.Add(item);

            if (this.ChangeCurrentOnAdd)
            {
                this.SetCurrentPositionCore(this.IndexOf(item), true);
            }

            return true;
        }

        private bool RemoveItem(NotifyCollectionChangedEventArgs args)
        {
            TDataItem item = (TDataItem)args.NewItems[0];

            int index = -1;
            if (this.HasDataOperation)
            {
                index = this.Items.IndexOf(item);
                if (index >= 0)
                {
                    this.Items.RemoveAt(index);
                    //PerformGrouping();
                }
                else
                {
                    this.RebuildData(false);
                }
            }


            int last = this.Count - 1;

            int position = (this.CurrentPosition < last) ? this.CurrentPosition : last;
            if (index >= 0)
            {
                while (index >= this.Count)
                {
                    index--;
                }

                position = index;
            }
            this.MoveCurrentToPosition(position);

            return true;
        }

        private bool ReplaceItem(NotifyCollectionChangedEventArgs args)
        {
            if (!this.HasDataOperation)
            {
                return true;
            }

            bool update = false;
            TDataItem oldItem = (TDataItem)args.OldItems[0];
            if (this.PassesFilter(oldItem))
            {
                this.Items.Remove(oldItem);
                update = true;
            }

            TDataItem item = (TDataItem)args.NewItems[args.NewStartingIndex];
            if (this.PassesFilter(item))
            {
                bool sort = (this.SortDescriptors.Count != 0);
                if (sort)
                {
                    this.indexer.Items.Add(item);
                }
                else
                {
                    //this.view.Insert(args.NewStartingIndex, item);
                    this.indexer.Items.Insert(args.NewStartingIndex, item);
                }


                update = true;
            }

            //if (update)
            //{
            //    PerformGrouping();
            //}

            return update;
        }

        private bool MoveItem(NotifyCollectionChangedEventArgs args)
        {
            TDataItem item = (TDataItem)args.NewItems[0];
            if (!this.PassesFilter(item))
            {
                return false;
            }

            //this.Items.RemoveAt(args.OldStartingIndex);
            //bool sort = (this.UsedDescriptors.Count != 0);
            //if (sort)
            //{
            //    indexer.InsertWithDuplicates(item);
            //}
            //else
            //{
            //    this.view.Insert(args.NewStartingIndex, item);
            //}
            //PerformGrouping();

            return true;
        }

        private bool ChangeItem(TDataItem item, string propertyName)
        {
            //no need to update if we do not have data operation
            if (!this.HasDataOperation)
            {
                return true;
            }

            if (!this.PassesFilter(item))
            {
                if (this.changingIndex >= 0)
                {
                    this.Items.RemoveAt(this.changingIndex);
                    //PerformGrouping();
                    return true;
                }
                else
                {
                    int index = this.Items.IndexOf(item);
                    if (index < 0)
                    {
                        this.RebuildData(false);
                        return true;
                    }
                }

                return false;
            }

            if (this.changingIndex >= 0 && this.SortDescriptors.Count > 0)
            {
                this.UpdateItemSorted(item, propertyName);
            }
            else
            {
                int index = this.Items.IndexOf(item);
                if (index < 0)
                {
                    this.RebuildData(false);
                }
            }

            return true;
        }

        #endregion
    }
}
