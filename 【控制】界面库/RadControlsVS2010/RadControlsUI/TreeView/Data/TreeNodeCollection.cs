using System;
using Telerik.Collections.Generic;
using Telerik.WinControls.Data;
using System.ComponentModel;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class RadTreeNodeCollection : NotifyCollection<RadTreeNode>
    {
        #region Fields

        private RadTreeNode owner;

       

        #endregion-

        #region Constructors

        internal RadTreeNodeCollection(RadTreeNode owner) 
            : base(new TreeNodeDataView(owner))
        {
            this.owner = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        [Browsable(false)]
        public RadTreeNode Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// Gets the tree view.
        /// </summary>
        /// <value>The tree view.</value>
        public RadTreeView TreeView
        {
            get
            {
                if (owner != null)
                {
                    return owner.TreeView;
                }
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            Reload();

            if (this.owner.TreeViewElement != null)
            {
                this.owner.TreeViewElement.Update(RadTreeViewElement.UpdateActions.Resume);
            }
        }

        /// <summary>
        /// Adds the tree node with specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public RadTreeNode Add(string text)
        {
            RadTreeNode node = null;
            if (owner != null && owner.TreeViewElement != null)
            {
                node = (RadTreeNode)(owner.TreeViewElement as IDataItemSource).NewItem();
                //node.TreeViewElement = owner.TreeViewElement;
                node.Text = text;
                node.Name = text;
            }
            else
            {
                node = new RadTreeNode(text);
            }
            this.Add(node);

            return node;
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="imageIndex">Index of the image.</param>
        /// <returns></returns>
        public virtual RadTreeNode Add(string text, int imageIndex)
        {
            RadTreeNode node = this.Add(text);
            node.ImageIndex = imageIndex;

            return node;
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="imageKey">The image key.</param>
        /// <returns></returns>
        public virtual RadTreeNode Add(string text, string imageKey)
        {
            RadTreeNode node = this.Add(text);
            node.ImageKey = imageKey;

            return node;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="text">The text.</param>
        /// <param name="imageIndex">Index of the image.</param>
        /// <returns></returns>
        public virtual RadTreeNode Add(string key, string text, int imageIndex)
        {
            RadTreeNode node = this.Add(text);
            node.Name = key;
            node.ImageIndex = imageIndex;

            return node;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="text">The text.</param>
        /// <param name="imageKey">The image key.</param>
        /// <returns></returns>
        public virtual RadTreeNode Add(string key, string text, string imageKey)
        {
            RadTreeNode node = this.Add(text);
            node.Name = key;
            node.ImageKey = imageKey;

            return node;
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            int index = this.IndexOf(name);
            if (index >= 0)
            {
                RadTreeNode node = this[index];

                this.RemoveAt(index);
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return this.IndexOf(name) >= 0;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the <see cref="Telerik.WinControls.UI.RadTreeNode"/> with the specified name.
        /// </summary>
        /// <value></value>
        public RadTreeNode this[string name]
        {
            get
            {
                foreach (RadTreeNode node in this)
                {
                    if (node.Name == name)
                    {
                        return node;
                    }
                }

                foreach (RadTreeNode node in this)
                {
                    if (node.Text == name)
                    {
                        return node;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Internal

        protected internal void Reload()
        {
            this.owner.NodesLoaded = false;
        }

        internal void Reset()
        {
            TreeNodeDataView dataView = this.Items as TreeNodeDataView;
            if (dataView != null)
            {
                dataView.Reset();
            }
        }

        protected override void InsertItem(int index, Telerik.WinControls.UI.RadTreeNode item)
        {
            if (item.treeView != null && !this.owner.TreeViewElement.ListSource.IsDataBound) //!this.owner.TreeViewElement.ListSource.IsDataBound && 
            {
                throw new ArgumentException(string.Format("Cannot add or insert the item '{0}' in more than one place. You must first remove it from its current location or clone it. Parameter name: {0}", item.Text));
            }

            //if (item.Parent == this.owner)
            //{
            //    throw new ArgumentException("Cannot add or insert the item 'Root' in more than one place. You must first remove it from its current location or clone it");
            //}

            //if (item.Parent != null)
            //{
            //    item.Parent.Nodes.Remove(item);
            //}

            item.Parent = this.owner;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, RadTreeNode item)
        {
            item.Parent = this.owner;
            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            RadTreeNode nodeToSelect = null;
            RadTreeNode node = this.Items[index];
            if (node.Current)
            {
                node.TreeViewElement.BeginUpdate();
                node.TreeViewElement.SelectedNode = null;
                node.TreeViewElement.EndUpdate(false, RadTreeViewElement.UpdateActions.ItemRemoved);
                nodeToSelect = node.PrevVisibleNode;
                if (nodeToSelect == null)
                {
                    RadTreeNode temp = node;
                    while (temp != null && nodeToSelect == null)
                    {
                        nodeToSelect = temp.NextNode;
                        temp = temp.Parent;
                    }
                }
            }

            node.deleted = true;
            node.Parent = null;
            node.TreeViewElement = null;
            base.RemoveItem(index);

            if (nodeToSelect != null && nodeToSelect.TreeViewElement != null)
            {
                nodeToSelect.TreeViewElement.SelectedNode = nodeToSelect;
            }
        }

        protected override void ClearItems()
        {
            foreach (RadTreeNode item in this.Items)
            {
                item.Parent = null;
            }

            base.ClearItems();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnCollectionChanged(args);
            if (this.owner != null)
            {
                RadTreeViewElement treeViewElement = this.owner.TreeViewElement;
                if (treeViewElement != null)
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        RadTreeNode node = (RadTreeNode)args.NewItems[0];
                        treeViewElement.Update(RadTreeViewElement.UpdateActions.ItemAdded, node);
                        treeViewElement.OnNodeAdded(new RadTreeViewEventArgs(node));
                    }
                    else if (args.Action == NotifyCollectionChangedAction.Remove)
                    {
                        RadTreeNode node = (RadTreeNode)args.NewItems[0];
                        treeViewElement.Update(RadTreeViewElement.UpdateActions.ItemRemoved, node);
                        treeViewElement.OnNodeRemoved(new RadTreeViewEventArgs(node));
                    }
                    else if (args.Action == NotifyCollectionChangedAction.Move)
                    {
                        RadTreeNode node = (RadTreeNode)args.NewItems[0];
                        treeViewElement.Update(RadTreeViewElement.UpdateActions.ItemMoved, node);
                    }
                    else if (args.Action == NotifyCollectionChangedAction.Reset)
                    {
                        treeViewElement.Update(RadTreeViewElement.UpdateActions.Reset);
                    }
                }
            }
        }

        #endregion
    }
}
