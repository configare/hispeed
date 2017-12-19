using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    internal class EmptyCollectionView<T> : RadCollectionView<T> where T : IDataItem
    {
         #region Fields

        private bool canFilter = true;
        private bool canSort = true;
        private bool canGroup = true;

        //default implementation
        private IComparer<T> comparer;
        private static List<T> Empty = new List<T>();

        #endregion

        #region Constructors

        public EmptyCollectionView(IEnumerable<T> collection)
            : base(collection)
        {
        }

        protected override void InitializeSource(IEnumerable<T> collection)
        {
            base.InitializeSource(collection);
            this.comparer = new DataItemComparer<T>(this.SortDescriptors);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the comparer.
        /// </summary>
        /// <value>The comparer.</value>
        public override IComparer<T> Comparer
        {
            get { return this.comparer; }
            set
            {
                this.comparer = value;
                this.RefreshOverride();
            }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public override GroupCollection<T> Groups
        {
            get { return GroupCollection<T>.Empty; }
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

        public override int IndexOf(T item)
        {
            return -1;
        }

        public override T Find(int itemIndex, object dataBoundItem)
        {
            return default(T);
        }

        #endregion

        #region Internal

        protected override void RefreshOverride()
        {
            base.RefreshOverride();
        }

        protected override IList<T> Items
        {
            get
            {
                return EmptyCollectionView<T>.Empty;
            }
        }

        protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnNotifyPropertyChanged(e);

            if (e.PropertyName == "FilterExpression" ||
                e.PropertyName == "Filter" ||
                e.PropertyName == "SortDescriptors" ||
                e.PropertyName == "GroupDescriptors")
            {
                RefreshOverride();
            }
        }

        #endregion
    }
}
