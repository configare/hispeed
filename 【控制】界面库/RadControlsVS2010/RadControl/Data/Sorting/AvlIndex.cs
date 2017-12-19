using System.Collections.Generic;
using Telerik.Collections.Generic;

namespace Telerik.WinControls.Data
{
    internal class AvlIndex<T> : Index<T> where T : IDataItem
    {
        private const int ITEM_CAPACITY = 64;
        private IList<T> items = new List<T>();
        private IEnumerable<T> source;
        private int version = int.MinValue + 1;

        public AvlIndex(RadCollectionView<T> collectionView)
            : base(collectionView)
        {
            this.source = collectionView.SourceCollection;
            this.InitializeItems();
        }

        public AvlIndex(RadCollectionView<T> collectionView, IEnumerable<T> source)
            : base(collectionView)
        {
            this.source = source;
            this.InitializeItems();
        }

        protected internal override IList<T> Items
        {
            get
            {
                this.Perform();
                return this.items;
            }
        }

        protected internal override void SetDirty()
        {
            this.version++;
        }

        public override void Load(IEnumerable<T> source)
        {
            this.source = source;
            this.InitializeItems();
        }

        protected override void Perform()
        {
            if (this.version == this.CollectionView.Version)
            {
                return;
            }

            IEnumerable<T> e = (this.CollectionView.IsIncrementalFiltering) ? this.items : this.source;
            if (this.CollectionView.CanSort && this.CollectionView.SortDescriptors.Count != 0)
            {
                this.items = new AvlTree<T>(this.CollectionView.Comparer);
            }
            else
            {
                this.items = new List<T>(ITEM_CAPACITY);
            }

            if (this.CollectionView.CanFilter && this.CollectionView.Filter != null)
            {
                this.items.Clear();

                foreach (T item in e)
                {
                    if (this.CollectionView.PassesFilter(item))
                    {
                        this.items.Add(item);
                    }
                }

                this.version = this.CollectionView.Version;
                return;
            }

            if (this.CollectionView.CanSort && this.CollectionView.SortDescriptors.Count != 0)
            {
                this.items.Clear();
                foreach (T item in e)
                {
                    this.items.Add(item);
                }

                this.version = this.CollectionView.Version;
                return;
            }

            InitializeItems();
            this.version = this.CollectionView.Version;
        }

        private void InitializeItems()
        {
            if (this.source is IList<T>)
            {
                this.items = (IList<T>)this.source;
                return;
            }

            this.items = new List<T>(255);
            IEnumerator<T> e = this.source.GetEnumerator();
            while (e.MoveNext())
            {
                this.items.Add(e.Current);
            }

            this.version = int.MinValue;
        }
    }
}
