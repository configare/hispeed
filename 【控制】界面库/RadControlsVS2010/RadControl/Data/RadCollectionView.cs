using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Data.Expressions;

namespace Telerik.WinControls.Data
{
    public abstract class RadCollectionView<TDataItem> :
        ICollectionView<TDataItem>,
        IReadOnlyCollection<TDataItem>,
        IEnumerable<TDataItem>,
        IEnumerable,
        INotifyCollectionChanged, 
        INotifyPropertyChanged
        where TDataItem : IDataItem
    {
        #region Fields

        private int update = 0;
        private int position = -1;
        private int currentItemHashCode = -1;
        private bool caseSensitive;
        private bool positionChangedInUpdate;

        private string filterExpression;
        private string prevExpression;

        private StringCollection filterContext;
        private ExpressionNode filterNode;
        private SortDescriptorCollection sortDescriptors;
        private GroupDescriptorCollection groupDescriptors;
        private Predicate<TDataItem> filter = null;

        private IEnumerable<TDataItem> sourceCollection;
        private CollectionResetReason resetReason;
        private bool changeCurrentOnAdd = true;
        protected int version;

        private IGroupFactory<TDataItem> groupFactory;
        private ISortDescriptorCollectionFactory sortDescriptorCollectionFactory;
        private IGroupDescriptorCollectionFactory groupDescriptorCollectionFactory;


        private class DeferHelper : IDisposable
        {
            // Fields
            private RadCollectionView<TDataItem> collectionView;

            // Methods
            public DeferHelper(RadCollectionView<TDataItem> collectionView)
            {
                this.collectionView = collectionView;
            }

            public void Dispose()
            {
                if (this.collectionView != null)
                {
                    this.collectionView.EndDefer();
                    this.collectionView = null;
                }
            }
        }

        #endregion

        #region Constructors

        public RadCollectionView(IEnumerable<TDataItem> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            this.InitializeFields();
            this.InitializeSource(collection);
        }

        internal RadCollectionView()
        {
            this.InitializeFields();

         
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the comparer.
        /// </summary>
        /// <value>The comparer.</value>
        [Browsable(false)]
        public virtual IComparer<TDataItem> Comparer
        {
            get
            {
                return (this as IComparer<TDataItem>);
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the comparer.
        /// </summary>
        /// <value>The comparer.</value>
        [Browsable(false)]
        public virtual IComparer<Group<TDataItem>> GroupComparer
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [change current on add].
        /// </summary>
        /// <value><c>true</c> if [change current on add]; otherwise, <c>false</c>.</value>
        public bool ChangeCurrentOnAdd
        {
            get
            {
                return this.changeCurrentOnAdd;
            }

            set
            {
                this.changeCurrentOnAdd = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item collection is empty.
        /// </summary>
        /// <value><c>true</c> if this item collection is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return (this.Items.Count == 0);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the underlying collection provides change notifications.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dynamic; otherwise, <c>false</c>.
        /// </value>
        public bool IsDynamic
        {
            get
            {
                return (this.sourceCollection is INotifyCollectionChanged);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <value></value>
        public TDataItem this[int index]
        {
            get
            {
                return this.Items[index];
            }
        }

        /// <summary>
        /// Indicates whether string comparisons of data are case-sensitive. 
        /// </summary>
        [Browsable(true), Category("Behavior"),
        DefaultValue(false)]
        public bool CaseSensitive
        {
            get
            {
                return this.caseSensitive;
            }
            set
            {
                if (this.caseSensitive != value)
                {
                    this.caseSensitive = value;
                    if (!string.IsNullOrEmpty(this.filterExpression))
                    {
                        this.filterNode = ExpressionParser.Parse(this.filterExpression, this.CaseSensitive);
                        this.filterContext.Clear();
                        List<NameNode> nameNodes = ExpressionNode.GetNodes<NameNode>(this.filterNode);
                        foreach (NameNode nameNode in nameNodes)
                        {
                            if (!this.filterContext.Contains(nameNode.Name))
                            {
                                this.filterContext.Add(nameNode.Name);
                            }
                        }

                        this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("FilterExpression"));
                    }

                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("CaseSensitive"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter expression.
        /// </summary>
        /// <value>The filter expression.</value>
        public virtual string FilterExpression
        {
            get
            {
                return this.filterExpression;
            }
            set
            {
                if (this.filterExpression != value)
                {
                    this.filterExpression = value;
                    if (string.IsNullOrEmpty(this.filterExpression))
                    {
                        if (this.filter == this.PerformExpressionFilter)
                        {
                            this.filter = null;
                        }

                        this.OnNotifyPropertyChanged("FilterExpression");
                        return;
                    }

                    this.filterNode = ExpressionParser.Parse(this.filterExpression, this.CaseSensitive);
                    this.filterContext.Clear();
                    List<NameNode> nameNodes = ExpressionNode.GetNodes<NameNode>(this.filterNode);
                    foreach (NameNode nameNode in nameNodes)
                    {
                        if (!this.filterContext.Contains(nameNode.Name))
                        {
                            this.filterContext.Add(nameNode.Name);
                        }
                    }

                    if (this.filter == null)
                    {
                        this.filter = this.PerformExpressionFilter;
                    }
                    this.OnNotifyPropertyChanged("FilterExpression");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Passeses the filter.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual bool PassesFilter(TDataItem item)
        {
            Predicate<TDataItem> filter = this.Filter;
            if (filter != null)
            {
                return filter(item);
            }

            return true;
        }

        /// <summary>
        /// Suspends event notification.
        /// </summary>
        public void BeginUpdate()
        {
            this.update++;
        }

        /// <summary>
        /// Resumes event notification.
        /// </summary>
        public void EndUpdate(bool notifyUpdates)
        {
            if (update == 0)
            {
                return;
            }

            update--;
            if (update > 0)
            {
                return;
            }

            if (notifyUpdates)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            if (notifyUpdates && this.positionChangedInUpdate)
            {
                this.OnCurrentChanged(EventArgs.Empty);
                this.positionChangedInUpdate = false;
            }
        }

        /// <summary>
        /// Resumes event notification.
        /// </summary>
        public void EndUpdate()
        {
            this.EndUpdate(true);
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

        /// <summary>
        /// Copies to array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(TDataItem[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public virtual void LoadData(IEnumerable<TDataItem> collection)
        {
            if (this.sourceCollection == collection)
            {
                return;
            }

            InitializeSource(collection);
            RefreshOverride();
        }

        /// <summary>
        /// Finds the specified item index.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="dataBoundItem">The data bound item.</param>
        /// <returns></returns>
        public virtual TDataItem Find(int itemIndex, object dataBoundItem)
        {
            TDataItem itemAtIndex = this[itemIndex];
            if (object.Equals(itemAtIndex.DataBoundItem, dataBoundItem))
            {
                return itemAtIndex;
            }

            foreach (TDataItem item in this)
            {
                if (object.Equals(item.DataBoundItem, dataBoundItem))
                {
                    return item;
                }
            }

            return default(TDataItem);
        }

        /// <summary>
        /// Searches the Groups collection for a match, using the Keys in the provided group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public Group<TDataItem> FindGroup(Group<TDataItem> group)
        {
            if (this.Groups == null || this.Groups.Count == 0)
            {
                return null;
            }

            return this.FindGroup(group, this.Groups);
        }

        /// <summary>
        /// Determines whether the specified group is present within this view.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool ContainsGroup(Group<TDataItem> group)
        {
            return this.FindGroup(group, this.Groups) != null;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual int IndexOf(TDataItem item)
        {
            return this.Items.IndexOf(item);
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TDataItem item)
        {
            return (this.IndexOf(item) >= 0);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public object Evaluate(string expression, TDataItem item)
        {
            int index = this.IndexOf(item);
            if (index != -1)
            {
                return Evaluate(expression, index, 1);
            }

            return null;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public object Evaluate(string expression, int startIndex, int count)
        {
            if (startIndex >= this.Count)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return Evaluate(expression, this.GetItems(startIndex, count));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public object Evaluate(string expression, IEnumerable<TDataItem> items)
        {
            ExpressionNode node = ExpressionParser.Parse(expression, this.CaseSensitive);
            List<NameNode> nameNodes = ExpressionNode.GetNodes<NameNode>(node);

            StringCollection contextProperties = new StringCollection();
            foreach (NameNode nameNode in nameNodes)
            {
                if (!contextProperties.Contains(nameNode.Name))
                {
                    contextProperties.Add(nameNode.Name);
                }
            }

            IEnumerator<TDataItem> e = items.GetEnumerator();
            e.MoveNext();

            ExpressionContext context = ExpressionContext.Context;
            context.Clear();
            for (int i = 0; i < contextProperties.Count; i++)
            {
                if (context.ContainsKey(contextProperties[i]))
                {
                    context[contextProperties[i]] = e.Current[contextProperties[i]];
                }
                else
                {
                    context.Add(contextProperties[i], e.Current[contextProperties[i]]);
                }
            }

            if (ExpressionNode.GetNodes<AggregateNode>(node).Count > 0)
            {
                return node.Eval(new AggregateItems<TDataItem>(items), context);
            }

            return node.Eval(null, context);
        }

        internal bool TryEvaluate(string expression, IEnumerable<TDataItem> items, out object result)
        {
            return TryEvaluate(expression, items, 0, out result);
        }

        /// <summary>
        /// Try to evaluate the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="items">The items.</param>
        /// <param name="index">Index of item, which the result will be calculated for</param>
        /// <param name="result">Expression result</param>
        /// <returns></returns>
        internal bool TryEvaluate(string expression, IEnumerable<TDataItem> items, int index, out object result)
        {
            result = null;

            ExpressionNode node = null;
            if (!ExpressionParser.TryParse(expression, this.CaseSensitive, out node))
            {
                return false;
            } 

            List<NameNode> nameNodes = ExpressionNode.GetNodes<NameNode>(node);

            StringCollection contextProperties = new StringCollection();
            foreach (NameNode nameNode in nameNodes)
            {
                if (!contextProperties.Contains(nameNode.Name))
                {
                    contextProperties.Add(nameNode.Name);
                }
            }

            IEnumerator<TDataItem> e = items.GetEnumerator();

            while (index >= 0)
            {
                if (!e.MoveNext())
                {
                    throw new IndexOutOfRangeException();
                }
                index--;
            }
            
            if (e.Current == null)
            {
                return false;
            }

            ExpressionContext context = ExpressionContext.Context;
            context.Clear();

            for (int i = 0; i < contextProperties.Count; i++)
            {
                if (e.Current.IndexOf(contextProperties[i]) < 0)
                {
                    return false;
                }
                if (context.ContainsKey(contextProperties[i]))
                {
                    context[contextProperties[i]] = e.Current[contextProperties[i]];
                }
                else
                {
                    context.Add(contextProperties[i], e.Current[contextProperties[i]]);
                }
            }

            try
            {
                if (ExpressionNode.GetNodes<AggregateNode>(node).Count > 0)
                {
                    result = node.Eval(new AggregateItems<TDataItem>(items), context);
                }
                else
                {
                    result = node.Eval(null, context);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Internal

        internal static void RebuildDescriptorIndex(IEnumerable<TDataItem> source, SortDescriptorCollection sortDescriptors, GroupDescriptorCollection groupDescriptors)
        {
            IEnumerator<TDataItem> e = source.GetEnumerator();
            e.MoveNext();
            if (e.Current != null)
            {
                for (int i = 0; i < sortDescriptors.Count; i++)
                {
                    if (string.IsNullOrEmpty(sortDescriptors[i].PropertyName))
                    {
                        continue;
                    }

                    sortDescriptors[i].PropertyIndex = e.Current.IndexOf(sortDescriptors[i].PropertyName);
                }

                for (int i = 0; i < groupDescriptors.Count; i++)
                {
                    for (int j = 0; j < groupDescriptors[i].GroupNames.Count; j++)
                    {
                        if (string.IsNullOrEmpty(groupDescriptors[i].GroupNames[j].PropertyName))
                        {
                            continue;
                        }

                        groupDescriptors[i].GroupNames[j].PropertyIndex = e.Current.IndexOf(groupDescriptors[i].GroupNames[j].PropertyName);
                    }
                }
            }
        }

        private bool PerformExpressionFilter(TDataItem item)
        {
            if (this.filterContext.Count == 0)
            {
                return false;
            }

            try
            {
                ExpressionContext context = ExpressionContext.Context;
                context.Clear();
                for (int i = 0; i < this.filterContext.Count; i++)
                {
                    string fieldName = this.filterContext[i];
                    if (context.ContainsKey(fieldName))
                    {
                        context[fieldName] = item[fieldName];
                    }
                    else
                    {
                        context.Add(fieldName, item[fieldName]);
                    }
                }

                object result = this.filterNode.Eval(null, context);
                if (result is bool)
                {
                    return (bool)result;
                }
            }
            catch (Exception ex)
            {
                throw new FilterExpressionException("Invalid filter expression.", ex);
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has filter applied.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has filter applied; otherwise, <c>false</c>.
        /// </value>
        protected bool HasFilter
        {
            get
            {
                return (this.CanFilter && (Filter != null || !string.IsNullOrEmpty(this.FilterExpression)));
            }
        }

        protected bool IsInUpdate
        {
            get
            {
                return (this.update > 0);
            }
        }

        private void EndDefer()
        {
            this.EndUpdate();
        }

        protected virtual IList<TDataItem> Items
        {
            get
            {
                return null;
            }
        }

        protected bool HasDataOperation
        {
            get
            {
                return (this.HasFilter || this.HasSort || this.HasGroup);
            }
        }

        protected internal virtual IGroupFactory<TDataItem> GroupFactory
        {
            get { return groupFactory; }
            set { groupFactory = value; }
        }

        private void groupDescriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.resetReason = CollectionResetReason.GroupingChanged;
            this.OnNotifyPropertyChanged("GroupDescriptors");
        }

        /// <summary>
        /// Gets a value indicating whether this instance has group applied.
        /// </summary>
        /// <value><c>true</c> if this instance has group applied; otherwise, <c>false</c>.</value>
        protected bool HasGroup
        {
            get
            {
                return (this.CanGroup && this.GroupDescriptors.Count > 0);
            }
        }

        private void InitializeFields()
        {
            this.filter = null;
            this.filterExpression = string.Empty;
            this.filterContext = new StringCollection();

            this.sortDescriptorCollectionFactory = new DefaultSortDescriptorCollectionFactory();
            this.sortDescriptors = this.SortDescriptorCollectionFactory.CreateCollection();
            this.sortDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(sortDescriptors_CollectionChanged);

            this.groupFactory = new DefaultGroupFactory<TDataItem>();
            this.groupDescriptorCollectionFactory = new DefaultGroupDescriptorCollectionFactory();
            this.groupDescriptors = this.GroupDescriptorCollectionFactory.CreateCollection();
            this.groupDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(groupDescriptors_CollectionChanged);
        }

        internal void Unload()
        {
            INotifyCollectionChanged prevNotifier = this.sourceCollection as INotifyCollectionChanged;
            if (prevNotifier != null)
            {
                prevNotifier.CollectionChanged -= new NotifyCollectionChangedEventHandler(source_CollectionChanged);
            }

            this.sortDescriptors.CollectionChanged -= new NotifyCollectionChangedEventHandler(sortDescriptors_CollectionChanged);
            this.groupDescriptors.CollectionChanged -= new NotifyCollectionChangedEventHandler(groupDescriptors_CollectionChanged);
        }

        protected internal int Version
        {
            get { return this.version; }
        }

        protected internal void LazyRefresh()
        {
            this.version++;
        }

        protected virtual void InitializeSource(IEnumerable<TDataItem> collection)
        {
            if (this.sourceCollection != null)
            {
                INotifyCollectionChanged prevNotifier = this.sourceCollection as INotifyCollectionChanged;
                if (prevNotifier != null)
                {
                    prevNotifier.CollectionChanged -= new NotifyCollectionChangedEventHandler(source_CollectionChanged);
                }
            }

            this.sourceCollection = collection;
            INotifyCollectionChanged changed = collection as INotifyCollectionChanged;
            if (changed != null)
            {
                changed.CollectionChanged += new NotifyCollectionChangedEventHandler(source_CollectionChanged);
            }
        }

        private void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Batch ||
                e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.resetReason = CollectionResetReason.Refresh;
            }

            this.ProcessCollectionChanged(e);
        }

        internal ISortDescriptorCollectionFactory SortDescriptorCollectionFactory
        {
            get { return this.sortDescriptorCollectionFactory; }
            set
            {
                if (this.sortDescriptorCollectionFactory != value)
                {
                    this.sortDescriptorCollectionFactory = value;

                    if (this.sortDescriptors != null)
                    {
                        this.sortDescriptors.CollectionChanged -= new NotifyCollectionChangedEventHandler(sortDescriptors_CollectionChanged);
                    }

                    this.sortDescriptors = this.sortDescriptorCollectionFactory.CreateCollection();
                    this.sortDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(sortDescriptors_CollectionChanged);
                }
            }
        }

        internal IGroupDescriptorCollectionFactory GroupDescriptorCollectionFactory
        {
            get { return this.groupDescriptorCollectionFactory; }
            set
            {
                if (this.groupDescriptorCollectionFactory != value)
                {
                    this.groupDescriptorCollectionFactory = value;

                    if (this.groupDescriptors != null)
                    {
                        this.groupDescriptors.CollectionChanged -= new NotifyCollectionChangedEventHandler(groupDescriptors_CollectionChanged);
                    }

                    this.groupDescriptors = this.groupDescriptorCollectionFactory.CreateCollection();
                    this.groupDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(groupDescriptors_CollectionChanged);
                }
            }
        }

        private void sortDescriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.resetReason = CollectionResetReason.SortingChanged;
            this.OnNotifyPropertyChanged("SortDescriptors");
        }

        /// <summary>
        /// Gets a value indicating whether this instance has sort applied.
        /// </summary>
        /// <value><c>true</c> if this instance has sort applied; otherwise, <c>false</c>.</value>
        protected bool HasSort
        {
            get
            {
                return (this.CanSort && this.SortDescriptors.Count > 0);
            }
        }

        private Group<TDataItem> FindGroup(Group<TDataItem> group, GroupCollection<TDataItem> groups)
        {
            if (groups.Count == 0)
            {
                return null;
            }

            int index = groups.IndexOf(group);
            if (index >= 0)
            {
                return groups[index];
            }

            foreach (Group<TDataItem> childGroup in groups)
            {
                Group<TDataItem> match = this.FindGroup(group, childGroup.Groups);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        #endregion

        #region Current Item

        public event EventHandler CurrentChanged;
        public event CancelEventHandler CurrentChanging;

        protected virtual void OnCurrentChanged(EventArgs args)
        {
            if (this.update == 0 && CurrentChanged != null)
            {
                CurrentChanged(this, args);
            }
        }

        protected virtual void OnCurrentChanging(CancelEventArgs args)
        {
            if (this.update == 0 && CurrentChanging != null)
            {
                CurrentChanging(this, args);
            }
        }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        /// <value>The current item.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TDataItem CurrentItem
        {
            get
            {
                if (this.position >= 0 && this.position < this.Count)
                {
                    return this[this.position];
                }

                return default(TDataItem);
            }
            internal set
            {
                this.position = this.IndexOf(value);
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        /// <value>The current position.</value>
        public int CurrentPosition
        {
            get { return this.position; }
            protected set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// Moves the current to.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool MoveCurrentTo(TDataItem item)
        {
            return MoveCurrentToPosition(IndexOf(item));
        }

        /// <summary>
        /// Moves the current to first.
        /// </summary>
        /// <returns></returns>
        public bool MoveCurrentToFirst()
        {
            return MoveCurrentToPosition(0);
        }

        /// <summary>
        /// Moves the current to last.
        /// </summary>
        /// <returns></returns>
        public bool MoveCurrentToLast()
        {
            return MoveCurrentToPosition(this.Count - 1);
        }

        /// <summary>
        /// Moves the current to next.
        /// </summary>
        /// <returns></returns>
        public bool MoveCurrentToNext()
        {
            return MoveCurrentToPosition(this.position + 1);
        }

        /// <summary>
        /// Moves the current to position.
        /// </summary>
        /// <param name="newPosition">The position.</param>
        /// <returns></returns>
        public bool MoveCurrentToPosition(int newPosition)
        {
            return this.SetCurrentPositionCore(newPosition, false);
        }

        /// <summary>
        /// The core update routine for the current position.
        /// </summary>
        /// <param name="newPosition">New position of the current item.</param>
        /// <param name="forceNotify">True to raise CurrentChanged regardless of whether actual position change is available.</param>
        /// <returns></returns>
        protected virtual bool SetCurrentPositionCore(int newPosition, bool forceNotify)
        {
            if (newPosition >= this.Count)
            {
                newPosition = this.Count - 1;
            }

            TDataItem newCurrentItem = default(TDataItem);

            if (newPosition >= 0 && newPosition < this.Count)
            {
                newCurrentItem = this.Items[newPosition];
            }

            int hashCode = newCurrentItem != null ? newCurrentItem.GetHashCode() : -1;

            if (hashCode != this.currentItemHashCode)
            {
                forceNotify = true;
                this.currentItemHashCode = hashCode;
            }

            if (this.position == newPosition && !forceNotify)
            {
                return false;
            }

            if (this.update == 0)
            {
                CancelEventArgs args = new CancelEventArgs();
                OnCurrentChanging(args);
                if (args.Cancel)
                {
                    return false;
                }
            }

            this.position = newPosition;
            this.positionChangedInUpdate = this.update > 0;

            if (this.update == 0)
            {
                this.OnCurrentChanged(new EventArgs());
            }

            return true;
        }

        /// <summary>
        /// Moves the current to previous.
        /// </summary>
        /// <returns></returns>
        public bool MoveCurrentToPrevious()
        {
            return MoveCurrentToPosition(this.position - 1);
        }

        #endregion

        #region ICollectionView<TDataItem> Members

        /// <summary>
        /// Gets or sets a value indicating whether this data view can filter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can filter; otherwise, <c>false</c>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanFilter
        {
            get { return true; }
            internal set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this data view can group.
        /// </summary>
        /// <value><c>true</c> if this instance can group; otherwise, <c>false</c>.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanGroup
        {
            get { return true; }
            internal set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this data view can sort.
        /// </summary>
        /// <value><c>true</c> if this instance can sort; otherwise, <c>false</c>.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanSort
        {
            get { return true; }
            internal set { }
        }

        /// <summary>
        /// Gets the source collection.
        /// </summary>
        /// <value>The source collection.</value>
        public virtual IEnumerable<TDataItem> SourceCollection
        {
            get { return this.sourceCollection; }
        }

        /// <summary>
        /// Gets the sort descriptions.
        /// </summary>
        /// <value>The sort descriptions.</value>
        public SortDescriptorCollection SortDescriptors
        {
            get { return this.sortDescriptors; }
        }

        /// <summary>
        /// Gets the group descriptions.
        /// </summary>
        /// <value>The group descriptions.</value>
        public GroupDescriptorCollection GroupDescriptors
        {
            get { return this.groupDescriptors; }
        }

        /// <summary>
        /// Provides a callback so that the default filtering expression parser can be substituted. 
        /// </summary>
        public virtual Predicate<TDataItem> Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                if (!this.CanFilter)
                {
                    throw new NotSupportedException();
                }

                if (this.filter != value)
                {
                    this.filter = value;
                    OnNotifyPropertyChanged("Filter");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is incremental filtering.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is incremental filtering; otherwise, <c>false</c>.
        /// </value>
        public bool IsIncrementalFiltering
        {
            get 
            {
                if (string.IsNullOrEmpty(this.prevExpression))
                {
                    return false;
                }

                if (this.prevExpression == this.filterExpression)
                {
                    return true;
                }

                BinaryOpNode prev = ExpressionParser.Parse(this.prevExpression, this.caseSensitive) as BinaryOpNode;
                BinaryOpNode current = ExpressionParser.Parse(this.filterExpression, this.caseSensitive) as BinaryOpNode;
                this.prevExpression = this.filterExpression;

                if (prev == null || current == null)
                {
                    return false;
                }

                if (prev.Op != Operator.Like || current.Op != Operator.Like)
                {
                    return false;
                }

                NameNode prevName = prev.Left as NameNode;
                NameNode currentName = current.Left as NameNode;
                if(prevName == null || currentName == null || prevName.Name != currentName.Name)
                {
                    return false;
                }

                ConstNode prevValue = prev.Right as ConstNode;
                ConstNode currentValue = current.Right as ConstNode;
                if (prevValue == null || currentValue == null)
                {
                    return false;
                }

                string prevVal = prevValue.Value.ToString();
                string currentVal = currentValue.Value.ToString();
                if (prevVal.Contains("%"))
                {
                    prevVal = prevVal.Replace("%",string.Empty);
                    currentVal = currentVal.Replace("%", string.Empty);

                    if (currentVal.Length > 1 && currentVal.Substring(0, currentVal.Length - 1) == prevVal)
                    {
                        return true;
                    }
                }

                return false; 
            }
        }

        /// <summary>
        /// Default callback so that the default filtering expression parser can be substituted. 
        /// </summary>
        public virtual Predicate<TDataItem> DefaultFilter
        {
            get
            {
                return this.PerformExpressionFilter;
            }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public virtual GroupCollection<TDataItem> Groups
        {
            get { return GroupCollection<TDataItem>.Empty; }
        }

        public virtual GroupPredicate<TDataItem> GroupPredicate
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Gets the default group predicate.
        /// </summary>
        /// <value>The default group predicate.</value>
        public virtual GroupPredicate<TDataItem> DefaultGroupPredicate
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Refreshes this data view.
        /// </summary>
        public virtual void Refresh()
        {
            this.RefreshOverride();
        }

        protected virtual void RefreshOverride()
        {
            if (this.IsInUpdate)
            {
                return;
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.OnCollectionChanged(args);
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.update == 0)
            {
                if (this.VersionUpdateNeeded(args))
                {
                    this.version++;
                }

                if (this.CollectionChanged != null)
                {
                    args.ResetReason = this.resetReason;
                    this.resetReason = CollectionResetReason.Refresh;
                    this.CollectionChanged(this, args);
                }
            }
        }

        protected virtual bool VersionUpdateNeeded(NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedAction action = args.Action;
            string propertyName = args.PropertyName;
            
            if ((action == NotifyCollectionChangedAction.ItemChanging ||
                action == NotifyCollectionChangedAction.ItemChanged) && !String.IsNullOrEmpty(propertyName))
            {
                if (this.filterContext.Contains(propertyName) ||
                    this.GroupDescriptors.Contains(propertyName) ||
                    this.SortDescriptors.Contains(propertyName))
                {
                    return true;
                }

                return false;                
            }

            return true;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        private void OnNotifyPropertyChanged(string propertyName)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the NotifyPropertyChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FilterExpression" ||
                e.PropertyName == "Filter")
            {
                this.resetReason = CollectionResetReason.FilteringChanged;
            }

            if (this.PropertyChanged != null && this.update == 0)
            {
                this.PropertyChanged(this, e);
            }
        }

        #endregion

        #region IReadOnlyCollection<TDataItem> Members

        void IReadOnlyCollection<TDataItem>.CopyTo(TDataItem[] array, int index)
        {
            this.Items.CopyTo(array, index);
        }

        #endregion

        #region IEnumerable<TDataItem> Members

        public IEnumerator<TDataItem> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerable<TDataItem> GetItems(int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return this[startIndex + i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion
    }
}
