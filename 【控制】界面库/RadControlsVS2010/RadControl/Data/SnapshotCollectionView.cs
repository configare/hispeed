using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    internal class SnapshotCollectionView<TDataItem> : ISnapshotCollectionView<TDataItem>
         where TDataItem : IDataItem
    {
        private RadCollectionView<TDataItem> sourceView;
        private IEnumerable<TDataItem> sourceCollection;
        private Index<TDataItem> index;
        private GroupBuilder<TDataItem> groupBuilder;

        public SnapshotCollectionView(IEnumerable<TDataItem> sourceCollection, RadCollectionView<TDataItem> sourceView)
        {
            this.sourceView = sourceView;
            this.sourceCollection = sourceCollection;

            RadCollectionView<TDataItem>.RebuildDescriptorIndex(sourceCollection, sourceView.SortDescriptors, sourceView.GroupDescriptors);
        }

        #region Methods

        public void Load(IEnumerable<TDataItem> source)
        {
            this.sourceCollection = source;
            RadCollectionView<TDataItem>.RebuildDescriptorIndex(source, sourceView.SortDescriptors, sourceView.GroupDescriptors);

            this.Indexer.Load(this.sourceCollection);
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

            return this.sourceView.Evaluate(expression, this.GetItems(startIndex, count));
        }

        /// <summary>
        /// Sets the view in dirty state.
        /// </summary>
        public void SetDirty()
        {
            this.Indexer.SetDirty();
        }

        #endregion

        #region Implementation

        IEnumerable<TDataItem> GetItems(int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return this[startIndex + i];
            }
        }

        protected virtual Index<TDataItem> Indexer
        {
            get
            {
                if (this.index == null)
                {
                    index = new AvlIndex<TDataItem>(this.SourceView, this.sourceCollection);
                }

                //this.index.Perform();
                return this.index;
            }
        }

        protected virtual GroupBuilder<TDataItem> GroupBuilder
        {
            get
            {
                if (this.groupBuilder == null)
                {
                    this.groupBuilder = new GroupBuilder<TDataItem>(this.Indexer);
                }

                return this.groupBuilder;
            }
        }

        protected virtual RadCollectionView<TDataItem> SourceView
        {
            get { return this.sourceView; }
        }

        #endregion

        #region ICollectionView<TDataItem> Members

        bool ICollectionView<TDataItem>.CanFilter
        {
            get { return this.sourceView.CanFilter; }
        }

        bool ICollectionView<TDataItem>.CanGroup
        {
            get { return this.sourceView.CanGroup; }
        }

        bool ICollectionView<TDataItem>.CanSort
        {
            get { return this.sourceView.CanSort; }
        }

        Predicate<TDataItem> ICollectionView<TDataItem>.Filter
        {
            get
            {
                return this.sourceView.Filter;
            }
            set
            {
                
            }
        }

        SortDescriptorCollection ICollectionView<TDataItem>.SortDescriptors
        {
            get { return sourceView.SortDescriptors; }
        }

        GroupDescriptorCollection ICollectionView<TDataItem>.GroupDescriptors
        {
            get { return this.sourceView.GroupDescriptors; }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public GroupCollection<TDataItem> Groups
        {
            get 
            {
                return this.GroupBuilder.Groups;
            }
        }

        /// <summary>
        /// Gets or sets the group predicate.
        /// </summary>
        /// <value>The group predicate.</value>
        public virtual GroupPredicate<TDataItem> GroupPredicate
        {
            get
            {
                return this.groupBuilder.GroupPredicate;
            }
            set
            {
                this.groupBuilder.GroupPredicate = value;
            }
        }

        /// <summary>
        /// Gets the source collection.
        /// </summary>
        /// <value>The source collection.</value>
        public IEnumerable<TDataItem> SourceCollection
        {
            get { return this.sourceCollection; }
        }

        void ICollectionView<TDataItem>.Refresh()
        {
            
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        protected virtual void OnCurrentChanged(EventArgs args)
        {
            if (CurrentChanged != null)
            {
                CurrentChanged(this, args);
            }
        }

        protected virtual void OnCurrentChanging(CancelEventArgs args)
        {
            if (CurrentChanging != null)
            {
                CurrentChanging(this, args);
            }
        }

        TDataItem ICollectionView<TDataItem>.CurrentItem
        {
            get { return default(TDataItem); }
        }

        int ICollectionView<TDataItem>.CurrentPosition
        {
            get { return -1; }
        }

        public event EventHandler CurrentChanged;

        public event System.ComponentModel.CancelEventHandler CurrentChanging;

        bool ICollectionView<TDataItem>.MoveCurrentTo(TDataItem item)
        {
            return false;
        }

        bool ICollectionView<TDataItem>.MoveCurrentToFirst()
        {
            return false;
        }

        bool ICollectionView<TDataItem>.MoveCurrentToLast()
        {
            return false;
        }

        bool ICollectionView<TDataItem>.MoveCurrentToNext()
        {
            return false;
        }

        bool ICollectionView<TDataItem>.MoveCurrentToPosition(int position)
        {
            return false;
        }

        bool ICollectionView<TDataItem>.MoveCurrentToPrevious()
        {
            return false;
        }

        #endregion

        #region IReadOnlyCollection<TDataItem> Members

        public int Count
        {
            get 
            {
                return this.Indexer.Count;
            }
        }

        public TDataItem this[int index]
        {
            get { return this.Indexer[index]; }
        }

        public bool Contains(TDataItem value)
        {
            return this.Indexer.Contains(value);
        }

        public void CopyTo(TDataItem[] array, int index)
        {
            this.Indexer.CopyTo(array, index);
        }

        public int IndexOf(TDataItem value)
        {
            int index = this.Indexer.IndexOf(value);

            while (index >= 0 && !this[index].Equals(value))
            {
                index++;

                if (index >= this.Count)
                {
                    return -1;
                }
            }

            return index;
        }

        #endregion

        #region IEnumerable<TDataItem> Members

        public IEnumerator<TDataItem> GetEnumerator()
        {
            return this.Indexer.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
           return this.GetEnumerator();
        }

        #endregion
    }
}
