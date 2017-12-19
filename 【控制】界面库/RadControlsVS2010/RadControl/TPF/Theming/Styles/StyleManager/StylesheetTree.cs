using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.Styles
{
    /// <summary>
    /// Maps nodes of a Stylesheet to nodes in ElementTree of a control
    /// </summary>
    public class StylesheetTree
    {
        private class MapElementContext
        {
            public MapElementContext(RadElement toAttach)
            {
                this.attachElement = toAttach;
            }

            public RadElement attachElement;
            public List<RadElement> unmappedElements = new List<RadElement>();
            public Dictionary<RadElement, object> mappedElements = new Dictionary<RadElement,object>(5);
        }

        private StyleMap ownerStyleMap;
        private Dictionary<int, LinkedList<StylesheetTreeNode>> nodesByKey = new Dictionary<int, LinkedList<StylesheetTreeNode>>();

        private StylesheetTreeNode rootNode;
        private bool hasVisualSelectors;

        /// <summary>
        /// Creates an instance of the <see cref="StylesheetTree"/> class.
        /// This class represents a parent-child selector hierarchy that
        /// comes from the loaded stylesheet.
        /// </summary>
        /// <param name="ownerStyeMap">ownerMap is used only for legacy </param>
        public StylesheetTree(StyleMap ownerStyeMap)
        {
            this.ownerStyleMap = ownerStyeMap;
            rootNode = new StylesheetTreeNode(this, null);
        }

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="StylesheetTreeNode"/>
        /// class that represents the root node of this tree.
        /// </summary>
        public StylesheetTreeNode RootNode
        {
            get
            {
                return this.rootNode;
            }
        }

        /// <summary>
        /// Determines whether visual selectors are registered for this tree.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasVisualStateSelectors
        {
            get
            {
                return this.hasVisualSelectors;
            }
            internal set
            {
                this.hasVisualSelectors = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="StyleMap"/> class
        /// that represents the owner style map of this stylesheet tree.
        /// </summary>
        public StyleMap OwnerStyleMap
        {
            get
            {
                return this.ownerStyleMap;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tries to retrieve all stylesheet tree nodes marked by the provided selector key.
        /// </summary>
        /// <param name="selectorKey">The selector key based on which the nodes will be retrieved.</param>
        /// <param name="nodes">A reference of the <see cref="LinkedList&lt;T&gt;"/> type which
        /// will be initialized with an instane of this class containing the matched nodes
        /// in case the operation succeeds.</param>
        /// <returns>True if the operation is successful, otherwise false.</returns>
        public bool TryGetNodeListByKey(int selectorKey, out LinkedList<StylesheetTreeNode> nodes)
        {
            return this.nodesByKey.TryGetValue(selectorKey, out nodes);
        }

        /// <summary>
        /// Adds tree nodes from the PropertySettingGroups in the stylesheet. Root nodes are grouped by groups' Selector Key. 
        /// Child nodes correspond to child selectors.
        /// </summary>
        /// <param name="styleSheet"></param>
        public void CreateNodesFromStyle(StyleSheet styleSheet)
        {
            for (int i = 0; i < styleSheet.PropertySettingGroups.Count; i++)
            {
                PropertySettingGroup group = styleSheet.PropertySettingGroups[i];
                group.SetIsFromStyleSheet();
                for (int k = 0; k < group.Selectors.Count; k++)
                {
                    IElementSelector selector = group.Selectors[k];

                    this.RootNode.Nodes.AddElementSelector(group, selector);                    
                }
            }
        }

        internal void RegisterNodeWithKey(int key, StylesheetTreeNode node)
        {
            LinkedList<StylesheetTreeNode> nodeList;            
            if (!this.nodesByKey.TryGetValue(key, out nodeList))
            {
                nodeList = new LinkedList<StylesheetTreeNode>();
                this.nodesByKey[key] = nodeList;
            }

            nodeList.AddLast(node);
        }

        public void OnElementSelectorKeyChanged(RadElement element, RadPropertyChangedEventArgs changeArgs)
        {
            if (changeArgs.Property == RadItem.VisualStateProperty && !this.hasVisualSelectors)
            {
                return;
            }

            string oldSelectorValue = changeArgs.OldValue as string;
            int oldSelectorKey = -1;

            Debug.Assert(oldSelectorValue != null);

            //TODO: decouple
            if (changeArgs.Property == RadElement.ClassProperty)
            {
                oldSelectorKey = ClassSelector.GetSelectorKey(oldSelectorValue);
            }
            else if (changeArgs.Property == RadItem.VisualStateProperty)
            {
                oldSelectorKey = VisualStateSelector.GetSelectorKey(oldSelectorValue);
            }

            IEnumerable<StylesheetTreeNode> nodeList = null;
            if (oldSelectorKey != -1)
            {
                //Optimization
                LinkedList<StylesheetTreeNode> mappedNodes;
                if (this.nodesByKey.TryGetValue(oldSelectorKey, out mappedNodes))
                {
                    nodeList = mappedNodes;
                }
            }
            else
            {
                nodeList = this.RootNode.Nodes;
            }

            if (nodeList != null)
            {
                //TODO: optimization possible
                //Detach element should traverse the element tree and prepare cache for AttachElement call later
                foreach (StylesheetTreeNode node in nodeList)
                {
                    node.DetachElement(element);
                }
            }

            this.AttachElement(element);
        }

        public void DetachElement(RadElement element)
        {
            this.DetachSingleElement(element, !element.IsDisposing);
        }

        private void DetachSingleElement(RadElement element, bool recursive)
        {
            //check whether we are removing element, attached to another stylemap
            if (element.Style != null && element.Style != this.ownerStyleMap.Style)
            {
                this.ownerStyleMap.OwnerStyleManager.RemoveStyleOwner(element);
            }
            else
            {
                foreach (LinkedList<StylesheetTreeNode> nodes in this.nodesByKey.Values)
                {
                    foreach (StylesheetTreeNode node in nodes)
                    {
                        node.RemoveMappingForElement(element, false);
                    }
                }

                if (recursive)
                {
                    foreach (RadElement child in element.Children)
                    {
                        this.DetachSingleElement(child, recursive);
                    }
                }
            }
        }

        public void UnmapNodes()
        {
            this.rootNode.DetachElements();
            this.nodesByKey.Clear();
            this.hasVisualSelectors = false;
        }

        internal void AttachElement(RadElement addedElement)
        {
            //try to process element hierarchy, starting from the root node
            MapElementContext context = new MapElementContext(addedElement);
            this.MapStyleNodesToElementHierarchy(context);

            if (context.unmappedElements.Count == 0)
            {
                return;
            }

            foreach (RadElement unmapped in context.unmappedElements)
            {
                ICollection<StylesheetTreeNode> ancestorMatches = this.FindAncestorMatchingNodes(addedElement, unmapped);
                if (ancestorMatches == null || ancestorMatches.Count == 0)
                {
                    continue;
                }

                foreach (StylesheetTreeNode node in ancestorMatches)
                {
                    this.NodeMapElement(node, unmapped, context);
                }
            }
        }

        private void MapStyleNodesToElementHierarchy(MapElementContext context)
        {
            foreach (RadElement childElement in ElementHierarchyEnumerator.TraverseElements(context.attachElement))
            {
                if (context.mappedElements.ContainsKey(childElement))
                {
                    continue;
                }

                if (!this.MapStyleNodesToElement(childElement, context))
                {
                    context.unmappedElements.Add(childElement);
                }
            }
        }

        private bool MapStyleNodesToElement(RadElement element, MapElementContext context)
		{
            ICollection<StylesheetTreeNode> matchingNodes = this.rootNode.Nodes.FindNodes(element);
            if (matchingNodes == null || matchingNodes.Count == 0)
            {
                return false;
            }

			foreach (StylesheetTreeNode childNode in matchingNodes)
			{
				this.NodeMapElement(childNode, element, context);
			}

            return true;
		}

        private void NodeMapElement(StylesheetTreeNode node, RadElement element, MapElementContext context)
        {
            if (node.Nodes == null)
            {
                return;
            }
            foreach (RadElement childElement in ElementHierarchyEnumerator.TraverseElements(element, false))
            {
                ICollection<StylesheetTreeNode> matchingNodes = node.Nodes.FindNodes(childElement);
                foreach (StylesheetTreeNode childNode in matchingNodes)
                {
                    NodeMapElement(childNode, childElement, context);
                }
            }

            node.AttachElement(element);

            if (context != null)
            {
                context.mappedElements[element] = null;
            }
        }

        /// <summary>
        /// Traverses the parent chain of the specified element and attempt to find matching nodes for it.
        /// </summary>
        /// <param name="matchRoot"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private ICollection<StylesheetTreeNode> FindAncestorMatchingNodes(RadElement matchRoot, RadElement element)
        {
            RadElement parent = matchRoot.Parent;
            ICollection<StylesheetTreeNode> parentMatches = null;

            while (parent != null)
            {
                parentMatches = this.rootNode.Nodes.FindNodes(parent);
                parent = parent.Parent;

                if (parentMatches == null || parentMatches.Count == 0)
                {
                    continue;
                }
                
                foreach (StylesheetTreeNode node in parentMatches)
                {
                    if (node.Nodes == null)
                    {
                        continue;
                    }
                    ICollection<StylesheetTreeNode> matches = node.Nodes.FindNodes(element);
                    if (matches != null && matches.Count > 0)
                    {
                        return matches;
                    }
                }
            }

            return null;
        }

        #endregion

    }
}