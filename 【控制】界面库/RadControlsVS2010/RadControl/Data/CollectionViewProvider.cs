using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    public class CollectionViewProvider<T> where T : IDataItem, INotifyPropertyChanged
    {
        private SortDescriptorCollection sortDescriptors;
        private GroupDescriptorCollection groupDescriptors;
        private ICollectionView<T> collectionView;
        private Predicate<T> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewProvider&lt;T&gt;"/> class.
        /// </summary>
        public CollectionViewProvider()
        {
            this.collectionView = null;
            this.sortDescriptors = new SortDescriptorCollection();
            this.groupDescriptors = new GroupDescriptorCollection();
            this.filter = null;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewProvider&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="sourceCollectionView">The source collection view.</param>
        public CollectionViewProvider(ICollectionView<T> sourceCollectionView)
        {
            this.collectionView = sourceCollectionView;
            this.sortDescriptors = collectionView.SortDescriptors;
            this.groupDescriptors = collectionView.GroupDescriptors;
            this.filter = collectionView.Filter;
        }

        /// <summary>
        /// Gets or sets the sort descriptors.
        /// </summary>
        /// <value>The sort descriptors.</value>
        public SortDescriptorCollection SortDescriptors
        {
            get { return this.sortDescriptors; }
            set
            {
                if (this.collectionView != null)
                {
                    throw new InvalidOperationException("The property can not be set when source CollectionView is used");
                }

                this.sortDescriptors = value;
                this.OnNotifyPropertyChanged("SortDescriptors");
            }
        }

        /// <summary>
        /// Gets or sets the group descriptors.
        /// </summary>
        /// <value>The group descriptors.</value>
        public GroupDescriptorCollection GroupDescriptors
        {
            get { return this.groupDescriptors; }
            set
            {
                if (this.groupDescriptors != value)
                {
                    if (this.collectionView != null)
                    {
                        throw new InvalidOperationException("The property can not be set when source CollectionView is used");
                    }

                    this.groupDescriptors = value;
                    this.OnNotifyPropertyChanged("GroupDescriptors");
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public Predicate<T> Filter
        {
            get { return filter; }
            set
            {
                if (this.filter != value)
                {
                    if (this.collectionView != null)
                    {
                        throw new InvalidOperationException("The property can not be set when source CollectionView is used");
                    }

                    this.filter = value;
                    this.OnNotifyPropertyChanged("Filter");
                }
            }
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public ISnapshotCollectionView<T> GetView(IEnumerable<T> source)
        {
            return null;
        }

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
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        #endregion
    }
}
