using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;
using System.Collections.ObjectModel;

namespace Telerik.WinControls.UI
{
    internal class TreeNodeDataView : IList<RadTreeNode>
    {
        #region Fields

        private RadTreeNode owner;
        private TreeNodeCollectionProvider collectionProvider;
        private IReadOnlyCollection<RadTreeNode> dataView;
        private int version = 0;

        #endregion

        #region Constructors

        public TreeNodeDataView(RadTreeNode owner)
        {
            this.owner = owner;
            Reset();
        }

        #endregion

        #region Internal Types

        class TreeNodeCollectionProvider
        {
            class LazyTreeNodes : Collection<RadTreeNode>
            {
                private RadTreeNode parent;

                public LazyTreeNodes(RadTreeNode parent)
                {
                    this.parent = parent;
                }

                protected override void InsertItem(int index, RadTreeNode item)
                {
                    item.Parent = this.parent;
                    base.InsertItem(index, item);
                }

                protected override void RemoveItem(int index)
                {
                    this[index].Parent = null;
                    base.RemoveItem(index);
                }

                protected override void SetItem(int index, RadTreeNode item)
                {
                    item.Parent = this.parent;
                    base.SetItem(index, item);
                }

                protected override void ClearItems()
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        this[i].Parent = null;

                    }

                    base.ClearItems();
                }
            }

            protected IList<RadTreeNode> items;

            private RadTreeNode owner;
            public TreeNodeCollectionProvider(RadTreeNode owner)
            {
                this.owner = owner;
                this.items = new LazyTreeNodes(this.owner);
            }

            public virtual void Load()
            {
                if (this.items.Count > 0 || this.owner.TreeViewElement == null)
                {
                    return;
                }

                bool rootLevel = this.owner is RadTreeViewElement.RootTreeNode;
                bool innerMode = !this.owner.TreeViewElement.FullLazyMode && (this.owner.Parent == null || this.owner.Parent.Expanded);

                if (rootLevel || innerMode || this.owner.Expanded)
                {
                    this.owner.NodesLoaded = true;

                    this.owner.TreeViewElement.BeginUpdate();
                    NodesNeededEventArgs args = new NodesNeededEventArgs((this.owner is RadTreeViewElement.RootTreeNode) ? null : this.owner, this.items);
                    this.owner.TreeViewElement.OnNodesNeeded(args);
                    this.owner.TreeViewElement.EndUpdate(false, RadTreeViewElement.UpdateActions.Resume);

                    this.owner.NodesLoaded = false;
                }
            }

            public RadTreeNode Owner
            {
                get { return owner; }
            }

            public virtual IList<RadTreeNode> Items
            {
                get
                {
                    return this.items;
                }
            }
        }

        class BindingCollectionProvider : TreeNodeCollectionProvider
        {
            public BindingCollectionProvider(RadTreeNode owner)
                : base(owner)
            {
            }

            public override void Load()
            {
                if (this.Owner.NodesLoaded || this.Owner.TreeViewElement == null)
                {
                    return;
                }

                this.items = this.Owner.TreeViewElement.TreeNodeProvider.GetNodes(this.Owner);
            }
        }

        class ReadOnlyDataView : IReadOnlyCollection<RadTreeNode>
        {
            private IList<RadTreeNode> nodes;
            public ReadOnlyDataView(IList<RadTreeNode> nodes)
            {
                this.nodes = nodes;
            }

            public int Count
            {
                get { return this.nodes.Count; }
            }

            public RadTreeNode this[int index]
            {
                get { return this.nodes[index]; }
            }

            public bool Contains(RadTreeNode value)
            {
                return this.nodes.Contains(value);
            }

            public void CopyTo(RadTreeNode[] array, int index)
            {
                this.nodes.CopyTo(array, index);
            }

            public int IndexOf(RadTreeNode value)
            {
                return this.nodes.IndexOf(value);
            }

            public IEnumerator<RadTreeNode> GetEnumerator()
            {
                return this.nodes.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        #endregion

        #region Internal

        private void Load()
        {
            this.collectionProvider.Load();
            if (this.owner != null && this.owner.TreeViewElement != null && (this.owner.TreeViewElement.SortDescriptors.Count > 0 || this.owner.TreeViewElement.FilterDescriptors.Count > 0))
            {
                if (this.dataView is SnapshotCollectionView<RadTreeNode> && this.owner.TreeViewElement.ListSource.CollectionView.Version != this.version)
                {
                    ((SnapshotCollectionView<RadTreeNode>)this.dataView).Load(this.collectionProvider.Items);
                    this.version = this.owner.TreeViewElement.ListSource.CollectionView.Version;
                }
                else
                {
                    this.dataView = new SnapshotCollectionView<RadTreeNode>(this.collectionProvider.Items, this.owner.TreeViewElement.ListSource.CollectionView);
                }

                ((SnapshotCollectionView<RadTreeNode>)this.dataView).SetDirty();
            }
            else
            {
                this.dataView = new ReadOnlyDataView(this.collectionProvider.Items);
            }
        }

        public void Reset()
        {
            this.collectionProvider = new TreeNodeCollectionProvider(this.owner);

            if (this.owner != null && this.owner.TreeViewElement != null && this.owner.TreeViewElement.TreeNodeProvider != null)
            {
                this.collectionProvider = new BindingCollectionProvider(this.owner);
            }
        }

        #endregion

        #region IList<RadTreeNode> Members

        public int IndexOf(RadTreeNode item)
        {
            this.Load();
            return this.dataView.IndexOf(item);
        }

        public void Insert(int index, RadTreeNode item)
        {
            this.collectionProvider.Items.Insert(index, item);
            this.version++;
        }

        public void RemoveAt(int index)
        {
            this.collectionProvider.Items.RemoveAt(index);
            this.version++;
        }

        public RadTreeNode this[int index]
        {
            get
            {
                return this.dataView[index];
            }
            set
            {
                int itemIndex = this.collectionProvider.Items.IndexOf(this.dataView[index]);
                if (itemIndex >= 0 && this.collectionProvider.Items[itemIndex] != value)
                {
                    this.collectionProvider.Items[itemIndex] = value;
                }
            }
        }

        #endregion

        #region ICollection<RadTreeNode> Members

        public void Add(RadTreeNode item)
        {
            this.collectionProvider.Items.Add(item);
            this.version++;
        }

        public void Clear()
        {
            this.collectionProvider.Items.Clear();
            this.version++;
        }

        public bool Contains(RadTreeNode item)
        {
            this.Load();
            return this.dataView.Contains(item);
        }

        public void CopyTo(RadTreeNode[] array, int arrayIndex)
        {
            this.Load();
            this.dataView.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                this.Load();
                return this.dataView.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(RadTreeNode item)
        {
            this.version++;
            return this.collectionProvider.Items.Remove(item);
        }

        #endregion

        #region IEnumerable<RadTreeNode> Members

        public IEnumerator<RadTreeNode> GetEnumerator()
        {
            this.Load();
            return dataView.GetEnumerator();
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
