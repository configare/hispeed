using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.Styles
{
    /// <summary>
    /// Represents a node in a StylesheetTree
    /// </summary>
    public class StylesheetTreeNode
    {
        private IElementSelector selector;
		private StylesheetTreeNodeElementDictionary mappedElements;
        private List<PropertySettingGroup> propertySettingGroups;
        private StylesheetTreeNodeCollection nodes;
        private StylesheetTree ownerTree;
        private StylesheetTreeNode parentNode;

        public StylesheetTreeNode(StylesheetTree ownerTree, IElementSelector selector)
        {
            this.ownerTree = ownerTree;
            this.selector = selector;
            this.nodes = new StylesheetTreeNodeCollection(this);
            int selectorKey = 0;
            //Generally RootNode registers with 
            if (selector != null)
            {
                selectorKey = selector.Key;                
            }
            ownerTree.RegisterNodeWithKey(selectorKey, this);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StylesheetTreeNode ParentNode
        {
            get
            {
                return this.parentNode;
            }
            internal set
            {
                this.parentNode = value;
            }
        }

        public IElementSelector Selector
        {
            get
            {
                return selector;
            }
        }

        public StylesheetTree OwnerTree
        {
            get
            {
                return this.ownerTree;
            }
        }

        public int Key
        {
            get
            {
                return this.selector.Key;
            }
        }

        public StylesheetTreeNodeCollection Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        public StylesheetTreeNodeElementDictionary MappedElements
        {
            set
            {
                mappedElements = value;
            }
            get
            {
                return mappedElements;
            }
        }

        public List<PropertySettingGroup> PropertySettingGroups
        {
            get
            {
                if (this.propertySettingGroups == null)
                {
                    this.propertySettingGroups = new List<PropertySettingGroup>(1);
                }

                return this.propertySettingGroups;
            }
        }

        internal void Attach()
        {
            if (this.mappedElements != null)
            {
                foreach (NodeElementEntry elementEntry in this.MappedElements.Values)
                {
                    this.ApplyPropertySettingsOnAttachedElement(elementEntry.Element);
                    this.ManagePropetyChangeSubscription(elementEntry, true);
                }
            }

            foreach (StylesheetTreeNode childNode in this.Nodes)
            {
                childNode.Attach();
            }
        }

        private void EnsureElementsCreated()
        {
            if (this.mappedElements == null)
            {
                this.mappedElements = new StylesheetTreeNodeElementDictionary();
            }
        }

        private void ApplyPropertySettingsOnAttachedElement(RadElement element)
        {
            this.ApplyPropertySettingsOnAttachedElement(element, false);
        }

        private struct NotAppliedSelector
        {
            public IElementSelector Selector;
            public PropertySettingCollection PropertySettings;

            public NotAppliedSelector(IElementSelector selector, PropertySettingCollection propertySettings)
            {
                this.Selector = selector;
                this.PropertySettings = propertySettings;
            }
        }

        private void ApplyPropertySettingsOnAttachedElement(RadElement element, bool onPropertyChanged)
        {
            if (this.propertySettingGroups == null)
            {
                return;
            }

            List<PropertySettingGroup> groupsWithConditions = new List<PropertySettingGroup>();
            List<PropertySettingGroup> groupsWithoutConditions = new List<PropertySettingGroup>();

            for (int i = 0; i < this.propertySettingGroups.Count; i++)
            {
                PropertySettingGroup group = this.propertySettingGroups[i];
                if (!group.Selectors[0].HasApplyCondition)
                {
                    groupsWithoutConditions.Add(group);
                }
                else if (group.Selectors[0].CanSelect(element))
                {
                    groupsWithConditions.Add(group);
                }
            }

            //check whether we need to apply the groups without condition
            if (!onPropertyChanged || groupsWithConditions.Count == 0)
            {
                foreach (PropertySettingGroup group in groupsWithoutConditions)
                {
                    ApplyGroupSettingsOnElement(group, element);
                }
            }

            //groups with condition are always applicable
            foreach (PropertySettingGroup group in groupsWithConditions)
            {
                ApplyGroupSettingsOnElement(group, element);
            }
        }

        private void ApplyDefaultGroup()
        {
            throw new NotImplementedException();
        }

        private static void ApplyGroupSettingsOnElement(PropertySettingGroup group, RadElement element)
        {
            foreach (IPropertySetting setting in group.PropertySettings.EnumeratePropertySettingsForElement(element))
            {
                setting.ApplyValue(element);
            }
        }

        public void AddElements(LinkedList<RadElement> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            foreach (RadElement element in list)
            {
                AddElementEntry(null, element);
            }
        }

        public void AttachElement(RadElement element)
        {
            if (this.propertySettingGroups == null || this.propertySettingGroups.Count == 0)
            {
                return;
            }

            this.ApplyPropertySettingsOnAttachedElement(element);
            if (!this.ContainsElement(element))
            {
                NodeElementEntry newElementEntry = AddElementEntry(null, element);
                this.ManagePropetyChangeSubscription(newElementEntry, true);
            }
        }

        private NodeElementEntry AddElementEntry(RadElement conditionCheckTargetElement, RadElement element)
        {
            if (this.mappedElements == null)
            {
                this.mappedElements = new StylesheetTreeNodeElementDictionary();
            }

            NodeElementEntry result = new NodeElementEntry(conditionCheckTargetElement, element, this);

            this.mappedElements[element] = result;

            return result;
        }

        public bool CheckKeyMatches(IElementSelector elementSelector)
        {
            return this.selector != null && this.selector.Equals(elementSelector);

            //if (this.key.GetType() == typeof(string))
            //{
            //    //TODO: multi-class selector logic
            //    return elementSelector.Key is string && string.CompareOrdinal((string)this.key, (string)elementSelector.Key) == 0;
            //}
            //else if (this.key is Type)
            //{
            //    return elementSelector.Key is Type && (Type)this.key == (Type)elementSelector.Key;
            //}

            //return false;
        }

        public bool CheckKey(RadElement element)
        {
            return this.selector != null && this.selector.CanSelectIgnoringConditions(element);

            ////TODO: extract in different classes
            //if (this.key.GetType() == typeof(string))
            //{
            //    //TODO: multi-class selector logic
            //    return (string)this.key == element.Class;
            //}
            //else if (this.key is Type)
            //{
            //    return (Type)this.key == element.GetThemeEffectiveType();
            //}

            //return false;
        }

        public IEnumerable<StylesheetTreeNode> EnumChildNodes(TreeTraversalMode traverseMode, bool includeSelf)
        {
            if (includeSelf)
            {
                yield return this;
            }

            if (traverseMode == TreeTraversalMode.BreadthFirst)
            {
                Queue<StylesheetTreeNode> children = new Queue<StylesheetTreeNode>();
                children.Enqueue(this);

                while (children.Count > 0)
                {
                    StylesheetTreeNode child = children.Dequeue();
                    foreach (StylesheetTreeNode nestedChild in child.Nodes)
                    {
                        yield return nestedChild;
                        children.Enqueue(nestedChild);
                    }
                }
            }
            else
            {
                Stack<StylesheetTreeNode> children = new Stack<StylesheetTreeNode>();
                children.Push(this);

                while (children.Count > 0)
                {
                    StylesheetTreeNode child = children.Pop();
                    foreach (StylesheetTreeNode nestedChild in child.Nodes)
                    {
                        yield return nestedChild;
                        children.Push(nestedChild);
                    }
                }
            }
        }

        /// <summary>
        /// Support for legacy "conditions" in themes.
        /// </summary>
        /// <param name="elementEntry"></param>
        /// <param name="subscribe"></param>
        private void ManagePropetyChangeSubscription(NodeElementEntry elementEntry, bool subscribe)
        {
            if (this.propertySettingGroups != null)
            {
                List<RadProperty> allProps = new List<RadProperty>();

                foreach (PropertySettingGroup group in this.propertySettingGroups)
                {
                    if (group.Selectors[0].HasApplyCondition)
                    {
                        group.Selectors[0].AddConditionPropertiesToList(allProps);
                    }
                }

                List<RadProperty> propsForSubscribtion = new List<RadProperty>();

                foreach (RadProperty prop in allProps)
                {
                    if (!propsForSubscribtion.Contains(prop))
                    {
                        propsForSubscribtion.Add(prop);
                    }
                }

                if (this.OwnerTree.OwnerStyleMap != null)
                {
                    foreach (RadProperty prop in propsForSubscribtion)
                    {
                        if (subscribe)
                        {
                            this.OwnerTree.OwnerStyleMap.OwnerStyleManager.Owner.ElementTree.SubscribeForElementPropertyChange(
                                elementEntry.OnRadPropertyChanged,
                                elementEntry.ConditionCheckTargetElement,
                                prop);
                        }
                        else
                        {
                            this.OwnerTree.OwnerStyleMap.OwnerStyleManager.Owner.ElementTree.UnsubscribeFromElementPropertyChange(
                               elementEntry.OnRadPropertyChanged,
                               elementEntry.ConditionCheckTargetElement,
                               prop);
                        }
                    }
                }
            }
        }

        public void AttachElementHierarchy(RadElement conditionCheckTargetElement, RadElement element)
        {
            foreach (RadElement radElement in ElementHierarchyEnumerator.TraverseElements(element))
            {
                if (this.CheckKey(radElement))
                {
                    this.EnsureElementsCreated();

                    NodeElementEntry newElementEntry = this.AddElementEntry(conditionCheckTargetElement, radElement);

                    AttachElementChildren(conditionCheckTargetElement, radElement);

                    this.ApplyPropertySettingsOnAttachedElement(radElement);
                    this.ManagePropetyChangeSubscription(newElementEntry, true);
                }
            }
        }

        private void AttachElementChildren(RadElement conditionCheckTargetElement, RadElement element)
        {
            foreach (RadElement childElement in element.Children)
            {
                foreach (StylesheetTreeNode node in this.Nodes)
                {
                    node.AttachElementHierarchy(conditionCheckTargetElement, childElement);
                }
            }
        }

        public void DetachElements()
        {
            if (this.mappedElements != null)
            {
                if (this.propertySettingGroups != null)
                {
                    foreach (NodeElementEntry elementEntry in this.mappedElements.Values)
                    {
                        this.ManagePropetyChangeSubscription(elementEntry, false);

                        for (int i = 0; i < this.propertySettingGroups.Count; i++)
                        {
                            foreach (IPropertySetting setting in this.propertySettingGroups[i].PropertySettings.EnumeratePropertySettingsForElement(elementEntry.Element))
                            {
                                setting.UnapplyValue(elementEntry.Element);

                                if (setting is AnimatedPropertySetting)
                                {
                                    elementEntry.Element.ResetValue(setting.Property, ValueResetFlags.Style);
                                }
                            }
                        }
                    }

                    this.propertySettingGroups.Clear();
                    this.propertySettingGroups = null;
                }

                this.mappedElements.Clear();
                this.mappedElements = null;
            }

            if (this.nodes != null)
            {
                foreach (StylesheetTreeNode node in this.nodes)
                {
                    node.DetachElements();
                }
                this.nodes.Clear();
                this.nodes = null;
            }

            this.ownerTree = null;
        }

        public bool ContainsElement(RadElement element)
        {
            return this.mappedElements != null && this.mappedElements.ContainsKey(element);
        }

        public void DetachElement(RadElement element)
        {
            bool containsElement = this.RemoveMappingForElement(element, false);

            //detach children
            if (this.nodes != null && this.nodes.Count > 0)
            {
                foreach (StylesheetTreeNode node in this.nodes)
                {
                    if (containsElement)
                    {
                        foreach (RadElement child in ElementHierarchyEnumerator.TraverseElements(element, false))
                        {
                            node.DetachElement(child);
                        }
                    }
                    else
                    {
                        node.DetachElement(element);
                    }
                }
            }
            else if (containsElement)
            {
                foreach (RadElement child in ElementHierarchyEnumerator.TraverseElements(element, false))
                {
                    this.parentNode.DetachElement(child);
                }
            }
        }

        internal bool RemoveMappingForElement(RadElement element, bool checkKey)
        {
            if (this.mappedElements == null)
            {
                return false;
            }

            NodeElementEntry elementEntry;
            if (!this.mappedElements.TryGetValue(element, out elementEntry))
            {
                return false;
            }

            if (checkKey && !this.CheckKey(element))
            {
                return false;
            }

            this.ManagePropetyChangeSubscription(elementEntry, false);
            this.mappedElements.Remove(element);

            return true;
        }

        #region Nested classes

        public class StylesheetTreeNodeElementDictionary : Dictionary<RadElement, NodeElementEntry>, IEnumerable<RadElement>
        {
            #region IEnumerable<RadElement> Members

            public new IEnumerator<RadElement> GetEnumerator()
            {
                foreach (RadElement element in this.Keys)
                {
                    yield return element;
                }
            }

            #endregion
        }

        public class NodeElementEntry
        {
            RadElement element;
            StylesheetTreeNode treeNode;
            RadElement conditionCheckTargetElement;

            public NodeElementEntry(RadElement conditionCheckTargetElement, RadElement element, StylesheetTreeNode treeNode)
            {
                this.conditionCheckTargetElement = conditionCheckTargetElement;
                this.element = element;
                this.treeNode = treeNode;
            }

            public void OnRadPropertyChanged(RadElement sender, RadPropertyChangedEventArgs e)
            {
                this.treeNode.ApplyPropertySettingsOnAttachedElement(element, true);
            }

            public RadElement Element
            {
                get
                {
                    return this.element;
                }
            }

            public RadElement ConditionCheckTargetElement
            {
                get
                {
                    if (this.conditionCheckTargetElement != null)
                    {
                        return conditionCheckTargetElement;
                    }

                    return element;
                }
            }
        }

        #endregion        
    }
}
