using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.Styles;
using System.Diagnostics;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.Themes
{
    /// <summary>
    /// This class provides information for an instance of the <see cref="Telerik.WinControls.RadItem"/> class used by the VSB.
    /// In general, here the state manager for the associated item and its child elements are described.
    /// In this way the VSB can see them and expose their properties for styling.
    /// </summary>
    public class VsbItemMetadata : VsbElementMetadata
    {
        #region Fields

        private List<VsbElementMetadata> elementSelectorInfos = new List<VsbElementMetadata>();
        private List<VsbItemMetadata> childItemInfos = new List<VsbItemMetadata>(0);
        private string itemThemeRole;
        private ItemStateManagerBase stateManager;
		private bool isStateProvider = true;
        private LinkedList<string> addedVisibleStates = new LinkedList<string>();

        #endregion

        #region constructors

        public VsbItemMetadata(string displayText)
            : base(displayText)
        {
        }

        public VsbItemMetadata(RadItem item)
            : base(item)
        {
            this.itemThemeRole = item.ThemeRole;
            this.stateManager = item.StateManager;

            //Add the element into the child infos if it is RadItem and is IPrimitiveElement (such as LightVisualElement)
            //TODO: Examine this functionality carefully
            //if (this.IsContainerElement(item) && (item is IPrimitiveElement))
            this.CheckAddChildMetadata(item);
        }


        #endregion

        #region Properties

        public virtual bool IsChildControlMetadata
        {
            get
            {
                if (this.ParentMetadata.StyleRelatedTreeHandlerType != null)
                {
                    return this.ParentMetadata.StyleRelatedTreeHandlerType != this.StyleRelatedTreeHandlerType;
                }

                return false;
            }
        }

        public override VsbMetadataType MetadataType
        {
            get
            {
                return VsbMetadataType.Item;
            }
        }


        /// <summary>
        /// Gets an instance of the <see cref="ItemStateManagerBase"/> class
        /// that represents the state manager for the current item.
        /// </summary>
        public ItemStateManagerBase StateManager
        {
            get
            {
                return this.stateManager;
            }
        }

        /// <summary>
        /// Gets or sets a string representing the item's theme role.
        /// </summary>
        public string ItemThemeRole
        {
            get
            {
                return this.itemThemeRole;
            }
            set
            {
                this.itemThemeRole = value;
            }
        }
       
        /// <summary>
        /// Gets a <see cref="System.Collections.Generic.IList&lt;T&gt;"/> object that holds instances of
        /// the <see cref="VsbItemMetadata"/> class and represent
        /// child items of the current item. These items appear
        /// in the tree view of the Visual Style Builder as children
        /// of the currently selected item.
        /// </summary>
        public IList<VsbItemMetadata> ChildItemInfos
        {
            get
            {
                return childItemInfos;
            }
        }
        
        /// <summary>
        /// Gets a <see cref="System.Collections.Generic.IList&lt;T&gt;"/> object that holds instances of
        /// the <see cref="VsbElementMetadata"/> class and represent
        /// child element items of the current item. These items appear
        /// as styleable elements in the elements list of the VSB.
        /// In general, such elements can be assigned property setting group
        /// for the currently selected state in the state list view of the VSB.
        /// </summary>
        public IList<VsbElementMetadata> ChildElementInfos
        {
            get
            {
                return this.elementSelectorInfos;
            }
        }

        public int ChildItemInfosCount
        {
            get
            {
                return this.childItemInfos.Count;
            }
        }

        public int ChildElementInfosCount
        {
            get
            {
                return this.elementSelectorInfos.Count;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether this
        /// item is state provider. This property is used to specify
        /// that the state management comes from a parent item.
        /// </summary>
        public bool IsStateProvider
        {
            get
            {
                return this.isStateProvider;
            }
            set
            {
                this.isStateProvider = value;
            }
        }

        #endregion

        #region Child Metadata


        private void CheckAddChildMetadata(RadItem item)
        {
            if (this.ShouldShowInChildMetadataList())
            {
                this.AddChildMetadata(this.CreateChildElementMetadata(item));
            }
        }
        public T GetChildMetadata<T>(int index) where T : VsbElementMetadata
        {
            T result = null;

            if (typeof(VsbItemMetadata) == typeof(T))
            {
                result = this.childItemInfos[index] as T;
            }
            else
            {
                result = this.elementSelectorInfos[index] as T;
            }

            return result;
        }

        public void AddChildMetadata(VsbElementMetadata metadata)
        {
            metadata.ParentMetadata = this;
            switch(metadata.MetadataType)
            {
                case VsbMetadataType.Element:
                    this.elementSelectorInfos.Add(metadata);
                    break;
                case VsbMetadataType.Item:
                    this.childItemInfos.Add(metadata as VsbItemMetadata);
                    break;
            }
        }

        public void RemoveChildMetadata(VsbElementMetadata metadata)
        {
            metadata.ParentMetadata = null;
            switch(metadata.MetadataType)
            {
                case VsbMetadataType.Element:
                    this.elementSelectorInfos.Remove(metadata);
                    break;
                case VsbMetadataType.Item:
                    this.childItemInfos.Remove(metadata as VsbItemMetadata);
                    break;
            }
        }

        protected virtual VsbItemMetadata CreateChildMetadata(RadItem childItem)
        {
            return new VsbItemMetadata(childItem);
        }

        protected virtual VsbElementMetadata CreateChildElementMetadata(RadElement element)
        {
            return new VsbElementMetadata(element);
        }

        public VsbItemMetadata AddItemInfoFromElement(RadItem item)
        {
            VsbItemMetadata info = new VsbItemMetadata(item);
            this.AddElementInfos(info, item);
            this.AddChildMetadata(info);

            return info;
        }

        public VsbItemMetadata AddItemInfoFromElementRecursive(RadItem item)
        {
            VsbItemMetadata info = new VsbItemMetadata(item);
            this.AddElementInfos(info, item, true);
            this.AddChildMetadata(info);

            return info;
        }

        protected virtual bool IsContainerElement(RadElement element)
        {
            return element is RadItem;
        }

        public void AddElementInfos(VsbItemMetadata info, RadElement element)
        {
            this.AddElementInfos(info, element, false);
        }

        public void AddElementInfos(VsbItemMetadata info, RadElement element, bool addChildItems)
        {
            foreach (RadElement child in element.Children)
            {
                if (this.AddElementInfosOverride(info, child, addChildItems))
                {
                    continue;
                }

                if (!child.VsbVisible)
                {
                    this.AddElementInfos(info, child, addChildItems);
                    continue;
                }

                if (!this.IsContainerElement(child))
                {
                    info.AddChildMetadata(this.CreateChildElementMetadata(child));
                    this.AddElementInfos(info, child, addChildItems);
                }
                else if (addChildItems)
                {
                    RadItem childItem = child as RadItem;
                    if (childItem == null)
                    {
                        continue;
                    }

                    VsbItemMetadata childInfo = this.CreateChildMetadata(childItem);
                    info.AddChildMetadata(childInfo);
                    this.AddElementInfos(childInfo, child, true);
                }
            }
        }

        protected virtual bool AddElementInfosOverride(VsbItemMetadata info, RadElement element, bool addChildItems)
        {
            return false;
        }

        #endregion

        #region Selectors

        public virtual IElementSelector CreateOwnSelector(string state)
        {
            if (this.UserSelector != null)
            {
                return this.UserSelector;
            }

            if (this.stateManager == null)
            {
                return CreateDefaultSelector(this);
            }

            IElementSelector result;

            if (string.IsNullOrEmpty(state))
            {
                StateDescriptionNode descr = this.stateManager.GetAvailableStates(this.itemThemeRole);
                result = new VisualStateSelector(descr.StateName);
            }
            else
            {
                bool verified = this.stateManager.VerifyState(this.itemThemeRole, state);
                Debug.Assert(verified, string.Format("State '{1}' on item with theme role '{0}' not valid for the specified StateManager: {2}", this.itemThemeRole, state, this.stateManager));
                result = new VisualStateSelector(state);
            }

            return result;
        }

        public virtual IElementSelector CreateSelectorForChildMetadata(VsbElementMetadata child, string state)
        {
            if (child.UserSelector != null)
            {
                return child.UserSelector;
            }

            IElementSelector defaultSelector = CreateDefaultSelector(child);
            if (!child.CanHaveChildSelector)
            {
                // Element metadata equals our theme type. For example GridDataCellElement.
                if (child.ElementThemeType == this.ElementThemeType)
                {
                    return this.CreateOwnSelector(state);
                }
                return defaultSelector;
            }

            IElementSelector ownSelector = this.CreateOwnSelector(state);
            if (ownSelector != null)
            {
                ownSelector.ChildSelector = defaultSelector;
                return ownSelector;
            }

            return defaultSelector;
        }

        public static IElementSelector CreateDefaultSelector(VsbElementMetadata metadata)
        {
            //- ClassSelector if element has StateManager or Class assigned
            //- TypeSelector otherwise
            //No name selector by default

            if (!string.IsNullOrEmpty(metadata.ElementClass))
            {
                return new ClassSelector(metadata.ElementClass);
            }
            else if (metadata.ElementThemeType != null)
            {
                return new TypeSelector(metadata.ElementThemeType);
            }

            return null;
        }

        #endregion

        #region Methods

        protected virtual bool ShouldShowInChildMetadataList()
        {
            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> instance that contains
        /// the visible states associated with this item.
        /// </summary>
        public IEnumerable<string> GetVisibleStates()
        {
            //Return merged enumerable from added states and default states
            //TODO: avoid duplication of states
            foreach (string state in this.addedVisibleStates)
            {
                yield return state;
            }

            if (this.stateManager != null)
            {
                foreach (string state in this.stateManager.DefaultVisibleStates)
                {
                    yield return state;
                }
            }
        }

        public bool StateExistsInAddedStates(string state)
        {
            return this.addedVisibleStates.Contains(state);
        }

        public bool StateExists(string state)
        {
            bool exists = StateExistsInAddedStates(state);
            
            if (exists)
            {
                return true;
            }

            foreach (string s in this.stateManager.DefaultVisibleStates)
            {
                if (s == state)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a visible state to the visible states collection of this item.
        /// </summary>
        /// <param name="state">The name of the visible state.</param>
        public void AddVisibleState(string state)
        {
            if (this.stateManager == null)
            {
                throw new InvalidOperationException("Cannot add visible state when StateManager is not defined");
            }

            //Debug.Assert(this.stateManager.VerifyState(this.itemThemeRole, state), "State not valid " + state);

            this.stateManager.AddDefaultVisibleState(state);
            this.addedVisibleStates.AddLast(state);
        }

        /// <summary>
        /// Adds a visible state to the visible states collection of this item.
        /// </summary>
        /// <param name="state">The name of the visible state.</param>
        public void RemoveVisibleState(string state)
        {
            if (this.stateManager == null)
            {
                throw new InvalidOperationException("Cannot remove visible state when StateManager is not defined");
            }

            this.addedVisibleStates.Remove(state);
            this.stateManager.RemoveDefaultVisibleState(state);
        }

        protected override string EvaluateDisplayTextFor(RadElement element)
        {
            RadItem item = element as RadItem;
            if (item == null)
            {
                return string.Empty;
            }

            if (item.StateManager == null)
            {
                return base.EvaluateDisplayTextFor(element);
            }

            string themeRole = item.ThemeRole;
            string themeEffectiveTypeName = item.GetThemeEffectiveType().Name;

            if (themeRole == themeEffectiveTypeName)
            {
                return themeEffectiveTypeName;
            }

            return themeRole + " (" + themeEffectiveTypeName + ")";
        }

        #endregion
    }   
}
