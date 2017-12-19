using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.Collections.Generic;
using Telerik.Data.Expressions;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    internal class BindingProvider : TreeNodeProvider
    {
        #region Nested types 

        class RangeList : IList<RadTreeNode>
        {
            private BindingProvider owner;
            private IList items;
            private IList<RadTreeNode> index;
            private Telerik.WinControls.Data.Range range; 

            public RangeList(BindingProvider owner, IList items, AvlTree<RadTreeNode> index, Telerik.WinControls.Data.Range range)
            {
                this.owner = owner;
                this.items = items;
                this.index = index;
                this.range = range;
            }

            #region IList<RadTreeNode> Members

            public int IndexOf(RadTreeNode item)
            {
                int index = this.index.IndexOf(item);

                while (index >= this.range.Min && !this.index[index].Equals(item))
                {
                    index++;

                    if (index > this.range.Max)
                    {
                        return -1;
                    }
                }

                return index - this.range.Min;
            }

            public void Insert(int index, RadTreeNode item)
            {
                this.owner.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                this.owner.SuspendUpdate();
                RadTreeNode node = (RadTreeNode)this[index];
                node.NodesLoaded = true;
                this.index.RemoveAt(this.range.Min + index);
                this.items.Remove(node);
                this.owner.ResumeUpdate();
            }

            public RadTreeNode this[int index]
            {
                get
                {
                    return this.index[this.range.Min + index];
                }
                set
                {
                    RadTreeNode node = this.index[this.range.Min + index];
                    if (node != value)
                    {
                        int boundIndex = this.items.IndexOf(node);
                        if (boundIndex >= 0)
                        {
                            this.items[boundIndex] = value;
                        }
                    }
                }
            }

            #endregion

            #region ICollection<RadTreeNode> Members

            public void Add(RadTreeNode item)
            {
                this.owner.AddNew(item);
            }

            public void Clear()
            {
                this.items.Clear();
            }

            public bool Contains(RadTreeNode item)
            {
                return this.IndexOf(item) >= 0;
            }

            public void CopyTo(RadTreeNode[] array, int arrayIndex)
            {
                this.index.CopyTo(array, this.range.Min + arrayIndex);
            }

            public int Count
            {
                get { return this.range.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(RadTreeNode item)
            {
                int count = this.Count;

                int index = this.items.IndexOf(item);
                if (index >= 0)
                {
                    this.RemoveAt(index);
                }

                return this.Count != count;
            }

            #endregion

            #region IEnumerable<RadTreeNode> Members

            public IEnumerator<RadTreeNode> GetEnumerator()
            {
                if (this.range.IsNull)
                {
                    yield break;
                }

                for (int i = this.range.Min; i <= this.range.Max; i++)
                {
                    yield return this.index[i];
                }
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        class ParentNode : RadTreeNode
        {
            private object dataBoundItem;
            private PropertyDescriptor descriptor;

            public ParentNode(PropertyDescriptor descriptor, object dataBoundItem)
            {
                this.descriptor = descriptor;
                this.dataBoundItem = dataBoundItem;
            }

            internal override object ParentValue
            {
                get
                {
                    return this.descriptor.GetValue(this.dataBoundItem);
                }
            }
        }

        class ParentComparer : IComparer<RadTreeNode>
        {
            public int Compare(RadTreeNode x, RadTreeNode y)
            {
                object xValue = x.ParentValue;
                object yValue = y.ParentValue;

                IComparable xCompVal = xValue as IComparable;
                if (xCompVal != null && yValue != null && yValue.GetType() == xValue.GetType())
                {
                    return ((IComparable)xValue).CompareTo(yValue);
                }
                else
                {
                    return DataStorageHelper.CompareNulls(xValue, yValue);
                }
            }
        }

        class ChildNode : RadTreeNode
        {
            private object dataBoundItem;
            private PropertyDescriptor descriptor;

            public ChildNode(PropertyDescriptor descriptor, object dataBoundItem)
            {
                this.descriptor = descriptor;
                this.dataBoundItem = dataBoundItem;
            }

            internal override object ChildValue
            {
                get
                {
                    return this.descriptor.GetValue(this.dataBoundItem);
                }
            }
        }

        class ChildComparer : IComparer<RadTreeNode>
        {
            public int Compare(RadTreeNode x, RadTreeNode y)
            {
                object xValue = x.ChildValue;
                object yValue = y.ChildValue;

                IComparable xCompVal = xValue as IComparable;
                if (xCompVal != null && yValue != null && yValue.GetType() == xValue.GetType())
                {
                    return ((IComparable)xValue).CompareTo(yValue);
                }
                else
                {
                    return DataStorageHelper.CompareNulls(xValue, yValue);
                }
            }
        }

        #endregion

        #region Fields

        private AvlTree<RadTreeNode> rootIndex = null;
        private List<AvlTree<RadTreeNode>> relationIndex = new List<AvlTree<RadTreeNode>>();
        private List<CurrencyManager> relationBindings = new List<CurrencyManager>();
        private List<bool> relationLevelLoaded = new List<bool>();

        #endregion

        #region Constructors

        public BindingProvider(RadTreeViewElement treeView)
            :base(treeView)
        {
            this.TreeView.RelationBindings.CollectionChanged += new NotifyCollectionChangedEventHandler(RelationBindings_CollectionChanged);
            this.TreeView.ListSource.CollectionChanged += new NotifyCollectionChangedEventHandler(ListSource_CollectionChanged);
            this.TreeView.ListSource.PositionChanged += new EventHandler(ListSource_PositionChanged); 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override IList<RadTreeNode> GetNodes(RadTreeNode parent)
        {
            if (this.rootIndex != null && this.rootIndex.Count > 0)
            {
               
                if (parent == null)
                {
                    parent = this.TreeView.Root;
                }

                if (parent.deleted)
                {
                    return RadTreeNodeCollection.Empty;
                }

                IList<RadTreeNode> list = GetSelfReferenceNodes(parent);
                if (list.Count == 0)
                {
                    list = GetRelationNodes(parent);
                }
                return list;
            }

            if (parent is RadTreeViewElement.RootTreeNode)
            {
                //this.TreeView.Root.NodesLoaded = true;
                return this.TreeView.ListSource;
            }

            if (this.relationBindings.Count > 0)
            {
                return GetRelationNodes(parent);
            }
            return RadTreeNodeCollection.Empty;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public override void Reset()
        {
            ResetRoot();
            InitilizeRelationBinding();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public override void Dispose()
        {
            this.TreeView.RelationBindings.CollectionChanged -= new NotifyCollectionChangedEventHandler(RelationBindings_CollectionChanged);
            this.TreeView.ListSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(ListSource_CollectionChanged);
            this.TreeView.ListSource.PositionChanged -= new EventHandler(ListSource_PositionChanged);

            ClearRelationBinding();
            this.rootIndex = null;

            base.Dispose();
        }

        /// <summary>
        /// Sets the current.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void SetCurrent(RadTreeNode node)
        {
            if (node == null || this.IsSuspended )
            {
                return;
            }

            if (node.BoundIndex == 0)
            {
                this.SuspendUpdate();
                CurrencyManager cm = ((ICurrencyManagerProvider)this.TreeView.ListSource).CurrencyManager;
                int position = cm.List.IndexOf(node.DataBoundItem);
                if (position >= 0)
                {
                    cm.Position = position;
                }
                this.ResumeUpdate();
                return;
            }

            this.SuspendUpdate();
            int index = node.BoundIndex - 1;
            if (index >= 0 && index < this.relationBindings.Count)
            {
                CurrencyManager cm = this.relationBindings[index];
                index = cm.List.IndexOf(node.DataBoundItem);
                if (index >= 0)
                {
                    cm.Position = index;
                }
            }

            this.ResumeUpdate();
        }

        #endregion

        #region Internal

        private void ClearRelationBinding()
        {
            while (this.TreeView.BoundDescriptors.Count > 1)
            {
                this.TreeView.BoundDescriptors.RemoveAt(1);
            }

            for (int i = 0; i < this.relationBindings.Count; i++)
            {
                CurrencyManager cm = this.relationBindings[i];
                UnwireEvents(cm);
            }

            for (int i = 0; i < this.TreeView.RelationBindings.Count; i++)
            {
                this.TreeView.RelationBindings[i].PropertyChanged -= new PropertyChangedEventHandler(relation_PropertyChanged);
            }

            this.relationBindings.Clear();
            this.relationIndex.Clear();
            this.relationLevelLoaded.Clear();
        }

        private void InitilizeRelationBinding()
        {
            ClearRelationBinding();
            for (int i = 0; i < this.TreeView.RelationBindings.Count; i++)
            {
                RelationBinding relation = this.TreeView.RelationBindings[i];
                relation.PropertyChanged += new PropertyChangedEventHandler(relation_PropertyChanged);

                CurrencyManager cm = null;
                if (relation.DataSource is BindingSource)
                {
                    cm = ((BindingSource)relation.DataSource).CurrencyManager;
                }

                if (cm == null && relation.DataSource != null)
                {
                    cm = this.TreeView.BindingContext[relation.DataSource, relation.DataMember] as CurrencyManager;
                }

                if (cm != null)
                {
                    WireEvents(cm);

                    PropertyDescriptorCollection properties = null;
                    if (i == 0)
                    {
                        if (((ICurrencyManagerProvider)this.TreeView.ListSource).CurrencyManager != null)
                        {
                            properties = ((ICurrencyManagerProvider)this.TreeView.ListSource).CurrencyManager.GetItemProperties();
                        }
                    }
                    else
                    {
                        properties = this.relationBindings[i - 1].GetItemProperties();
                    }

                    PropertyDescriptor descriptor = null;
                    if (properties != null)
                    {
                        descriptor = properties.Find(relation.ParentMember, true);
                    }

                    if (descriptor == null)
                    {
                        return;
                    }
                    this.TreeView.BoundDescriptors[i].ParentDescriptor = descriptor;

                    properties = cm.GetItemProperties();
                    TreeNodeDescriptor relationDescriptor = new TreeNodeDescriptor();

                    string childPath = relation.ChildMember.Split('\\')[0];
                    string[] names = childPath.Split('.');
                    PropertyDescriptor pd = properties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            relationDescriptor.SetChildDescriptor(pd, childPath);
                        }
                        else
                        {
                            relationDescriptor.ChildDescriptor = pd;
                        }
                    }

                    string valuePath = relation.ValueMember.Split('\\')[0];
                    names = valuePath.Split('.');
                    pd = properties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            relationDescriptor.SetValueDescriptor(pd, valuePath);
                        }
                        else
                        {
                            relationDescriptor.ValueDescriptor = pd;
                        }
                    }

                    string displayPath = relation.DisplayMember.Split('\\')[0];
                    names = displayPath.Split('.');
                    pd = properties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            relationDescriptor.SetDisplaytDescriptor(pd, valuePath);
                        }
                        else
                        {
                            relationDescriptor.DisplayDescriptor = pd;
                        }
                    }



                    this.TreeView.BoundDescriptors.Add(relationDescriptor);

                    AvlTree<RadTreeNode> nodes = new AvlTree<RadTreeNode>(new ChildComparer());
                    this.relationIndex.Add(nodes);
                    this.relationBindings.Add(cm);
                    this.relationLevelLoaded.Add(false);
                }
            }
        }

        private void LoadLevel(int level)
        {
            if (this.relationLevelLoaded[level])
            {
                return;
            }

            CurrencyManager cm = this.relationBindings[level];
            AvlTree<RadTreeNode> nodes = this.relationIndex[level];
            nodes.Clear();

            for (int j = 0; j < cm.Count; j++)
            {
                RadTreeNode node = this.TreeView.CreateNewNode();
                ((IDataItem)node).DataBoundItem = cm.List[j];
                node.BoundIndex = level + 1;
                nodes.Add(node);
            }

            this.relationLevelLoaded[level] = true;
        }

        private void WireEvents(CurrencyManager cm)
        {
            cm.ListChanged += new ListChangedEventHandler(cm_ListChanged);
            cm.PositionChanged += new EventHandler(cm_PositionChanged);
        }

        private void UnwireEvents(CurrencyManager cm)
        {
            cm.ListChanged -= new ListChangedEventHandler(cm_ListChanged);
            cm.PositionChanged -= new EventHandler(cm_PositionChanged);
        }

        private IList<RadTreeNode> GetRelationNodes(RadTreeNode parent)
        {
            int level = parent.BoundIndex;
            if(level >= this.relationBindings.Count)
            {
                if (this.IsSelfReference)
                {
                    return new RangeList(this, this.TreeView.ListSource, this.rootIndex, new Telerik.WinControls.Data.Range());
                }

                return RadTreeNodeCollection.Empty;
            }

            //lazy load
            LoadLevel(level);

            ChildNode node = new ChildNode(this.TreeView.BoundDescriptors[parent.BoundIndex].ParentDescriptor, parent.DataBoundItem);
            int firstIndex = this.relationIndex[level].Index(node);
            int lastIndex = this.relationIndex[level].LastIndex(node);

            if (firstIndex < 0 || lastIndex < 0)
            {
                if (this.IsSelfReference)
                {
                    return new RangeList(this, this.TreeView.ListSource, this.rootIndex, new Telerik.WinControls.Data.Range());
                }

                return RadTreeNodeCollection.Empty;
            }

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                this.relationIndex[level][i].Parent = parent;
            }

            return new RangeList(this, this.relationBindings[level].List, this.relationIndex[level], new Telerik.WinControls.Data.Range(firstIndex, lastIndex));
        }

        private IList<RadTreeNode> GetSelfReferenceNodes(RadTreeNode parent)
        {
            PropertyDescriptor descriptor = this.TreeView.RootDescriptor.ChildDescriptor;
            object dataBoundItem = parent.DataBoundItem;
            if (parent is RadTreeViewElement.RootTreeNode)
            {
                descriptor = this.TreeView.RootDescriptor.ParentDescriptor;    
                dataBoundItem = this.rootIndex[0].DataBoundItem;
            }
            if (dataBoundItem == null )
            {
                return new RangeList(this, this.TreeView.ListSource, this.rootIndex, new Telerik.WinControls.Data.Range());
            }
            
            ParentNode node = new ParentNode(descriptor, dataBoundItem);
            int firstIndex = this.rootIndex.Index(node);
            int lastIndex = this.rootIndex.LastIndex(node);

            if (firstIndex < 0 || lastIndex < 0 || (firstIndex == lastIndex && firstIndex == 0 && parent.Level > 0))
            {
                return new RangeList(this, this.TreeView.ListSource, this.rootIndex, new Telerik.WinControls.Data.Range());
            }

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if (RootCircularReference(parent, i))
                {
                    return new RangeList(this, this.TreeView.ListSource, this.rootIndex, new Telerik.WinControls.Data.Range());
                }

                this.rootIndex[i].Parent = parent;
            }

            return new RangeList(this, this.TreeView.ListSource, this.rootIndex, new Telerik.WinControls.Data.Range(firstIndex, lastIndex));
        }

        private bool RootCircularReference(RadTreeNode parent, int i)
        {
            //bool error = parent.Level > this.rootIndex[i].Level;
            //if (error)
            //{
            //    return error;
            //}

            return this.rootIndex[i] == parent || parent.Parent == this.rootIndex[i];
        }

        private void ExpandEnsureVisible()
        {
            if (this.TreeView.SelectedNode != null)
            {
                this.TreeView.BeginUpdate();

                RadTreeNode parent = this.TreeView.SelectedNode.Parent;
                while (parent != null && !parent.Expanded)
                {
                    parent.Expand();
                    parent = parent.Parent;
                }

                this.TreeView.EndUpdate(true, RadTreeViewElement.UpdateActions.Resume);
                this.TreeView.EnsureVisible(this.TreeView.SelectedNode);
            }
        }

        private void ResetRoot()
        {
            if (IsSelfReference)
            {
                this.rootIndex = new AvlTree<RadTreeNode>(new ParentComparer());
                for (int i = 0; i < this.TreeView.ListSource.Count; i++)
                {
                    this.rootIndex.Add(this.TreeView.ListSource[i]);
                }
            }
            else
            {
                this.rootIndex = null;
            }

            //this.TreeView.Root.NodesLoaded = false;
            this.TreeView.Update(RadTreeViewElement.UpdateActions.Reset);
        }

        private bool IsSelfReference
        {
            get
            {
                return (this.TreeView.RootDescriptor.ParentDescriptor != null && this.TreeView.RootDescriptor.ChildDescriptor != null);
            }
        }

        private void AddRoot(NotifyCollectionChangedEventArgs e)
        {
            if (this.IsSuspended)
            {
                return;
            }

            if (e.NewItems.Count > 0 )
            {
                RadTreeNode node = e.NewItems[0] as RadTreeNode;
                if (node != null && this.rootIndex != null)
                {
                    this.rootIndex.Add(node);
                }

                this.TreeView.Update(RadTreeViewElement.UpdateActions.Resume);
            }
        }

        private void ChangeRoot(NotifyCollectionChangedEventArgs e)
        {
            if (this.IsSuspended)
            {
                return;
            }

            this.TreeView.Update(RadTreeViewElement.UpdateActions.Resume);
        }

        private void ResetRelationNodes(object sender)
        {
            if (this.IsSuspended)
            {
                return;
            }

            CurrencyManager cm = sender as CurrencyManager;
            int index = this.relationBindings.IndexOf(cm);
            if (index >= 0)
            {
                this.relationLevelLoaded[index] = false;
                this.TreeView.Update(RadTreeViewElement.UpdateActions.Resume);
            }
        }

        private void ChangeRelationNode(object sender)
        {
            if (this.IsSuspended)
            {
                return;
            }

            this.TreeView.Update(RadTreeViewElement.UpdateActions.Resume);
        }

        private void AddRelationNode(object sender, int p)
        {
            if (this.IsSuspended)
            {
                return;
            }

            CurrencyManager cm = sender as CurrencyManager;
            int index = this.relationBindings.IndexOf(cm);
            if (index >= 0)
            {
                this.relationLevelLoaded[index] = false;
                this.TreeView.Update(RadTreeViewElement.UpdateActions.Resume);
            }
        }

        internal void AddNew(RadTreeNode item)
        {
            if (item.Parent == null)
            {
                return;
            }

            if (item.Parent.BoundIndex == 0)
            {
                SuspendUpdate();
                RadTreeNode newNode = this.TreeView.ListSource.AddNew();
                newNode.Parent = item.Parent;
                newNode.BoundIndex = item.Parent.BoundIndex;

                newNode.ParentValue = this.TreeView.BoundDescriptors[newNode.Parent.BoundIndex].ChildDescriptor.GetValue(newNode.Parent.DataBoundItem);
                newNode.Text = item.Text;
                item.Parent = null;
                item.NodesLoaded = true;
                item.TreeViewElement = null;

                if (rootIndex.Count > 0)
                {
                    ParentComparer comp = new ParentComparer();
                    if (comp.Compare(newNode, rootIndex[0]) <= 0)
                    {
                        this.TreeView.ListSource.Remove(newNode);
                        if (this.TreeView != null)
                        {
                            this.TreeView.SetError("Invalid Parent or Child Value. Can not build self-reference relation.", newNode.Parent);
                        }
                        ResumeUpdate();
                        return;
                    }
                }

                this.rootIndex.Add(newNode);
                ResumeUpdate();
                return;
            }

            SuspendUpdate();
            CurrencyManager cm = this.relationBindings[item.Parent.BoundIndex];
            AvlTree<RadTreeNode> nodes = this.relationIndex[item.Parent.BoundIndex];
            cm.AddNew();
            RadTreeNode childNode = this.TreeView.CreateNewNode();
            ((IDataItem)childNode).DataBoundItem = cm.List[cm.Count - 1];
            childNode.Parent = item.Parent;
            childNode.ParentValue = this.TreeView.BoundDescriptors[childNode.Parent.BoundIndex].ChildDescriptor.GetValue(childNode.Parent.DataBoundItem);
            childNode.Text = item.Text;
            item.Parent = null;
            item.NodesLoaded = true;
            item.TreeViewElement = null;
            ResumeUpdate();
        }

        internal void Insert(int index, RadTreeNode item)
        {
            AddNew(item);
        }

        void RelationBindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InitilizeRelationBinding();
        }

        void ListSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddRoot(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Batch:
                    ResetRoot();
                    break;
                case NotifyCollectionChangedAction.ItemChanging:
                    break;
                case NotifyCollectionChangedAction.ItemChanged:
                    ChangeRoot(e);
                    break;
            }
        }

        void relation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ValueMember")
            {

            }
            else if (e.PropertyName == "ChildMember")
            {

            }
            else if (e.PropertyName == "DisplayMember")
            {

            }
            else if (e.PropertyName == "DataSource")
            {

            }
            else if (e.PropertyName == "DataMember")
            {

            }
        }

        void cm_PositionChanged(object sender, EventArgs e)
        {
            if (this.IsSuspended)
            {
                return;
            }

            SuspendUpdate();
            CurrencyManager cm = sender as CurrencyManager;
            if (cm != null && cm.Position >= 0 && cm.Position < cm.Count)
            {
                int level = this.relationBindings.IndexOf(cm);
                Debug.Assert(level >= 0, "Invalid level in Relation Binding");

                PropertyDescriptor pd = this.TreeView.BoundDescriptors[level + 1].ChildDescriptor;
                if (pd != null)
                {
                    ChildNode node = new ChildNode(pd, cm.Current);
                    int index = this.relationIndex[level].IndexOf(node);
                    while (index >= 0)
                    {
                        if (cm.Current == this.relationIndex[level][index].DataBoundItem)
                        {
                            break;
                        }
                        index++;
                        if (index > this.relationIndex[level].Count)
                        {
                            return;
                        }
                    }

                    if (index >= 0)
                    {
                        this.TreeView.SelectedNode = this.relationIndex[level][index];
                        ExpandEnsureVisible();
                    }
                }
            }

            ResumeUpdate();
        }

        void cm_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    AddRelationNode(sender, e.NewIndex);
                    break;
                case ListChangedType.ItemChanged:
                    ChangeRelationNode(sender);
                    break;
                case ListChangedType.ItemDeleted:
                case ListChangedType.Reset:
                    ResetRelationNodes(sender);
                    break;
            }
        }

        void ListSource_PositionChanged(object sender, EventArgs e)
        {
            if (this.IsSuspended)
            {
                return;
            }

            this.SuspendUpdate();

            RadListSource<RadTreeNode> source = this.TreeView.ListSource;
            if (source.Position >= 0 && source.Position < source.Count)
            {
                RadTreeNode node = source[source.Position];
                this.TreeView.SelectedNode = node;
                ExpandEnsureVisible();
            }

            this.ResumeUpdate();
        }

        #endregion
    }
}
