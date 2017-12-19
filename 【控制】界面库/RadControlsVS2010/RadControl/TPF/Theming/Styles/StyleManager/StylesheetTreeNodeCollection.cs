using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using Telerik.WinControls;
using System.Diagnostics;

namespace Telerik.WinControls.Styles
{
    
    /// <summary>
    /// Collection of StylesheetTreeNode object used in StylesheetTree
    /// </summary>
    // Implemented by aggregating Dictionary with lazy instantiation
    public class StylesheetTreeNodeCollection : ICollection<StylesheetTreeNode>
    {
        private Dictionary<int, StylesheetTreeNode> nodes;

        private StylesheetTreeNode ownerNode;
		private bool hasVisualStateSelectors;

        public StylesheetTreeNodeCollection(StylesheetTreeNode ownerNode)
        {
            this.ownerNode = ownerNode;
        }

        public void Clear()
        {
            if (this.nodes != null)
            {
                this.nodes.Clear();
            }
            this.nodes = null;
            this.ownerNode = null;
        }

        public StylesheetTreeNode FindSelectorNode(string nodeKey)
        {
            return this.FindSelectorNode(nodeKey.GetHashCode());
        }

        public StylesheetTreeNode FindSelectorNode(Type elementType)
        {
            return this.FindSelectorNode(elementType.GetHashCode());
        }

        public StylesheetTreeNode FindSelectorNode(int selectorKey)
        {
            if (this.nodes == null)
            {
                return null;
            }

            StylesheetTreeNode result;
            if (this.nodes.TryGetValue(selectorKey, out result))
            {
                return result;
            }

            return null;
        }

        private void EnsureNodesCreated()
        {
            if (this.nodes == null)
            {
                Debug.Assert(this.ownerNode != null, "Node already disposed.");
                this.nodes = new Dictionary<int, StylesheetTreeNode>();
            }
        }

        public StylesheetTreeNode AddElementSelector(PropertySettingGroup propertySettings, IElementSelector selector)
        {
            StylesheetTreeNode node = this.AddNodeRecursive(propertySettings, selector, this);
            return node;
        }

        private void RegisterSelectorNode(StylesheetTreeNode node)
        {
            this.EnsureNodesCreated();
            this.nodes[node.Key] = node;
            node.ParentNode = this.ownerNode;

            //Optimization for Old theming
            if (node.Selector is VisualStateSelector)
            {
                this.hasVisualStateSelectors = true;
                this.ownerNode.OwnerTree.HasVisualStateSelectors = true;
            }
        }

        private StylesheetTreeNode AddNodeRecursive(PropertySettingGroup propertySettings, IElementSelector selector, StylesheetTreeNodeCollection parentNodeCollection)
        {
            if (selector.Key == 0)
            {
                return null;
            }

            StylesheetTreeNode node = parentNodeCollection.FindSelectorNode(selector.Key);
            if (node == null)
            {
                node = new StylesheetTreeNode(this.ownerNode.OwnerTree, selector);
                parentNodeCollection.RegisterSelectorNode(node);
            }

            if (selector.ChildSelector != null)
            {
                this.AddNodeRecursive(propertySettings, selector.ChildSelector, node.Nodes);
            }
            else
            {
                node.PropertySettingGroups.Add(propertySettings);
            }

            return node;
        }

        public ICollection<StylesheetTreeNode> FindNodes(RadElement element)
        {
            //TODO too coupled to RadElement and its properties
            LinkedList<StylesheetTreeNode> result = new LinkedList<StylesheetTreeNode>();

            if (nodes == null)
            {
                return result;
            }

            RadItem item = element as RadItem;
            StylesheetTreeNode foundNode;

            if (item != null && item.StateManager != null /*&& !string.IsNullOrEmpty(item.VisualState)*/)
            {
                //Opptimization for Old themes
                if (this.hasVisualStateSelectors)
                {
                    foreach (string itemState in item.StateManager.GetStateFallbackList(item))
                    {
                        if (this.nodes.TryGetValue(VisualStateSelector.GetSelectorKey(itemState), out foundNode))
                        {
                            result.AddLast(foundNode);
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(element.Class))
            {
                if (this.nodes.TryGetValue(ClassSelector.GetSelectorKey(element.Class), out foundNode))
                {
                    result.AddLast(foundNode);
                }
            }

            if (!string.IsNullOrEmpty(element.Name) && this.nodes.TryGetValue(NameSelector.GetSelectorKey(element.Name), out foundNode))
            {
                result.AddLast(foundNode);
            }

            if (this.nodes.TryGetValue(element.GetThemeEffectiveType().GetHashCode(), out foundNode))
            {
                result.AddLast(foundNode);
            }

            return result;
        }

        #region IEnumerable Members

        /// <summary>
        /// Gets enumerator for the collection
        /// </summary>
        /// <returns></returns>
        IEnumerator<StylesheetTreeNode> IEnumerable<StylesheetTreeNode>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets enumerator for the collection
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Enumerator optimized for foreach() statements implementation
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SelectorTreeNodeCollectionEnumerator GetEnumerator()
        {
            return new SelectorTreeNodeCollectionEnumerator(this);
        }

        /// <summary>
        /// Enumerator optimized for foreach() statements implementation
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct SelectorTreeNodeCollectionEnumerator : IEnumerator<StylesheetTreeNode>
        {
            IDictionaryEnumerator ownerEnumerator;

            #region IEnumerator Members

            public SelectorTreeNodeCollectionEnumerator(StylesheetTreeNodeCollection owner)
            {
                if (owner.nodes != null)
                {
                    this.ownerEnumerator = owner.nodes.GetEnumerator();
                }
                else
                {
                    this.ownerEnumerator = null;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public StylesheetTreeNode Current
            {
                get
                {
                    if (ownerEnumerator == null)
                    {
                        throw new InvalidOperationException("Enumerator is empty");
                    }

                    return (StylesheetTreeNode)ownerEnumerator.Entry.Value;
                }
            }

            public bool MoveNext()
            {
                if (ownerEnumerator == null)
                {
                    return false;
                }

                return ownerEnumerator.MoveNext();
            }

            public void Reset()
            {
                if (this.ownerEnumerator != null)
                {
                    ownerEnumerator.Reset();
                }
            }

            public void Dispose()
            {
            }

            #endregion
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Gets a value indicating the number of nodes in the collection
        /// </summary>
        public int Count
        {
            get
            {
                if (this.nodes == null)
                {
                    return 0;
                }

                return this.nodes.Count;
            }
        }

        void ICollection<StylesheetTreeNode>.CopyTo(StylesheetTreeNode[]array, int index)
        {
            if (this.nodes == null)
            {
                return;
            }

            this.nodes.Values.CopyTo(array, index);
        }

        void ICollection<StylesheetTreeNode>.Add(StylesheetTreeNode node)
        {
            this.nodes.Add(node.Key, node);
            node.ParentNode = this.ownerNode;
        }

        bool ICollection<StylesheetTreeNode>.Remove(StylesheetTreeNode node)
        {
            node.ParentNode = null;
            return this.nodes.Remove(node.Key);
        }

        bool ICollection<StylesheetTreeNode>.Contains(StylesheetTreeNode node)
        {
            return this.nodes.ContainsValue(node);
        }		

        bool ICollection<StylesheetTreeNode>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<StylesheetTreeNode>.Clear()
        {
            this.nodes.Clear();
        }

        #endregion
    }    
}
