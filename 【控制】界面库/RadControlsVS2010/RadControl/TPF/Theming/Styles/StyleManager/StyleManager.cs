using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;



namespace Telerik.WinControls.Styles
{
    //What Stylemanager and StyleMap do
    //StyleManager traverse the whole element tree and search StyleSheets for elements in ThemeResolutionService
    //StyleManager creates StyleMap for each RadElement if there is an entry in TRS
    //In the meanwhile StyleManager collects (caches) element 'Class' and 'Type' information from elements when traversing, to be 
    //used later (cached) in StyleMap    

    //StyleManager attaches to Element Add/Remove events to notify the corresponding StyleMap
    //StyleManager ataches to Style property Change events to update StyleMaps list

    //StyleMap Attaches to Class change events, to update the the map
    //StyleMap Attaches to PopertyChange change events to update the map    

    public class StyleManager : IPropertyChangeListener, IElementTreeChangeListener
    {
        private int stylingSuspendCounter = 0;
        private IComponentTreeHandler owner;
        private StyleManagerState state;
        private byte styleMapCleanUpLock = 0;
        private Dictionary<int, StyleMap> styleElementMap;

        public StyleManager(IComponentTreeHandler owner)
        {
            this.owner = owner;
            this.state = StyleManagerState.Initial;
            this.styleElementMap = new Dictionary<int, StyleMap>();
        }

        public IComponentTreeHandler Owner
        { 
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// Gets the state the manager is currently into.
        /// </summary>
        public StyleManagerState State
        {
            get
            {
                return this.state;
            }
        }

        public void SuspendStyling()
        {
            this.stylingSuspendCounter++;
        }

        public void ResumeStyling()
        {
            this.stylingSuspendCounter--;
        }

        public bool IsStylingSuspended
        {
            get
            {
                return this.stylingSuspendCounter > 0;
            }
        }

        public void AttachStylesToElementTree()
        {
            if (this.IsStylingSuspended)
            {
                return;
            }

            this.owner.ElementTree.SuspendAnimations();

            this.DetachStylesFromElementTree();

            this.state = StyleManagerState.Attaching;

            this.owner.ElementTree.AddElementPropertyChangeListener(this, RadElement.StyleProperty);            

            this.MapStylesToElementsRecursive(owner.RootElement);

            this.owner.ElementTree.AddElementTreeChangeListener(this);
            this.owner.ElementTree.AddElementPropertyChangeListener(this, RadElement.ClassProperty);
            this.owner.ElementTree.AddElementPropertyChangeListener(this, RadItem.VisualStateProperty);

            this.state = StyleManagerState.Attached;

            this.owner.ElementTree.ResumeAnimations();
        }

        public void DetachStylesFromElementTree()
        {
            if (this.state != StyleManagerState.Attached)
            {
                return;
            }

            this.state = StyleManagerState.Detaching;

            owner.ElementTree.RemoveElementTreeChangeListener(this);
            owner.ElementTree.RemoveElementPropertyChangeListener(this, RadElement.ClassProperty);
            owner.ElementTree.RemoveElementPropertyChangeListener(this, RadElement.StyleProperty);

            foreach (StyleMap styleMap in styleElementMap.Values)
            {
                styleMap.UnapplyStyle();
            }

            this.styleElementMap.Clear();

            this.state = StyleManagerState.Detached;
        }

        public StylesheetTree GetStylesheetTree(RadElement element)
        {
            if(this.state != StyleManagerState.Attached)
            {
                return null;
            }

            bool parentNotPropagateStyleToChildren;
            StyleMap map = this.FindStyleMapForElement(element, out parentNotPropagateStyleToChildren);
            if (map != null)
            {
                return map.StylesheetTree;
            }

            return null;
        }

        private void RemapStylesToElementsRecursive(RadElement element)
        {
            owner.ElementTree.RemoveElementTreeChangeListener(this);
            this.MapStylesToElementsRecursive(element);
            owner.ElementTree.AddElementTreeChangeListener(this);
        }

        /// <summary>
        /// Traverse element ierarchy and attach style to each element according to info in ThemeRsolutionService
        /// </summary>
        /// <param name="element"></param>
        private void MapStylesToElementsRecursive(RadElement element)
        {
            StyleMap elementStyleMap = null;

            if (element.CanHaveOwnStyle && element.ElementTree!=null) //by default Styles should be applied only on RadItems
            {
                if (element.ElementTree.ComponentTreeHandler != null)
                {
                    element.ElementTree.ComponentTreeHandler.SuspendUpdate();
                    if (!element.UseNewLayoutSystem)
                    {
                        element.ElementTree.RootElement.SuspendLayout();
                    }
                }

                elementStyleMap = this.MapStyleToElement(element);

                if (element.ElementTree.Control != null)
                {
                    if (!element.UseNewLayoutSystem)
                    {
                        element.ElementTree.RootElement.ResumeLayout(true);
                    }

                    element.ElementTree.ComponentTreeHandler.ResumeUpdate();
                }
            }

            if (element.PropagateStyleToChildren)
            {
                for (int i = 0; i < element.Children.Count; i++)
                {
                    this.MapStylesToElementsRecursive(element.Children[i]);
                }
            }

            if (elementStyleMap != null)
            {
                //BuildStyle would generaly set element's Style property to the corresponding StyleSheet
                elementStyleMap.BuildStyle();
            }

            element.IsThemeApplied = true;
        }        

        protected StyleMap MapStyleToElement(RadElement element)
        {
            //reset any style
            element.Style = null;

            StyleBuilder builder = ThemeResolutionService.GetStyleSheetBuilder(element);
            if (builder == null)
            {
                return null;
            }
            
            StyleMap styleMap = new StyleMap(this, builder, element);
            this.styleElementMap[element.GetHashCode()] = styleMap;

            return styleMap;
        }

        public void ReApplyStyle(RadElement radElement, bool traverseElementTree)
        {
            if (this.state != StyleManagerState.Attached || this.styleElementMap.Count == 0)
            {
                return;
            }

            if (!this.ShouldProcessElement(radElement))
            {
                return;
            }

            bool parentNotPropagateStyleToChildren;
            StyleMap styleMap = this.FindStyleMapForElement(radElement, out parentNotPropagateStyleToChildren);
            if (styleMap != null)
            {
                styleMap.RemapElement(radElement);
            }

            if (traverseElementTree)
            {
                int elementHash = radElement.GetHashCode();
                foreach (KeyValuePair<int, StyleMap> mapEntry in this.styleElementMap)
                {
                    if (elementHash == mapEntry.Key)
                    {
                        continue;
                    }

                    if (radElement.IsAncestorOf(mapEntry.Value.StyleRootElement))
                    {
                        mapEntry.Value.RemapElement(null);
                    }
                }
            }
        }

        public void ReApplyStyle(RadElement radElement)
        {
            this.ReApplyStyle(radElement, false);
        }

        internal void RemoveStyleOwner(RadElement element)
        {
            int hashCode = element.GetHashCode();
            StyleMap map;
            styleElementMap.TryGetValue(hashCode, out map);
            Debug.Assert(map != null, "Invalid style mapping for element " + element);

            if (this.styleMapCleanUpLock > 0)
            {
                //do not clean-up style map, simply detach all elements
                map.StylesheetTree.DetachElement(element);
            }
            else
            {
                map.UnapplyStyle();
                this.styleElementMap.Remove(hashCode);
            }
        }

        /// <summary>
        /// Resolves the style for the specified element without attaching it to the style map.
        /// Used when the style of a virtualized element should be applied. The element is actually not present on an element tree.
        /// </summary>
        /// <param name="element"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ApplyStyleToVirtualElement(RadElement element)
        {
            if (!element.IsInValidState(false))
            {
                return;
            }

            if (!element.PropagateStyleToChildren || element.IsThemeRefreshSuspended)
            {
                return;
            }

            if (this.state == StyleManagerState.Detaching || this.state == StyleManagerState.Detached)
            {
                return;
            }

            ComponentThemableElementTree oldTree = element.ElementTree;
            element.UpdateReferences(this.owner.ElementTree, false, true);

            this.MapStylesToElementsRecursive(element);

            bool parentNotPropagateStyleToChildren;
            StyleMap map = this.FindStyleMapForElement(element, out parentNotPropagateStyleToChildren);

            if(map != null)
            {
                if (this.styleElementMap.ContainsKey(element.GetHashCode()))
                {
                    this.styleElementMap.Remove(element.GetHashCode());
                }
                else
                {
                    map.BuildStyle();
                    map.SetStyleSheet(element.ComposeStyle());
                }
                map.OnElementRemoved(element);
            }

            element.UpdateReferences(oldTree, false, true);
        }

        private bool ShouldProcessElement(RadElement element)
        {
            if (this.state != StyleManagerState.Attached ||
                this.stylingSuspendCounter > 0 ||
                !element.IsInValidState(false))
            {
                return false;
            }

            return !element.IsThemeRefreshSuspended;
        }

        internal void SuspendStyleMapCleanUp()
        {
            this.styleMapCleanUpLock++;
        }

        internal void ResumeStyleMapCleanUp()
        {
            if (this.styleMapCleanUpLock > 0)
            {
                this.styleMapCleanUpLock--;
            }
        }

        #region IElementTreeChangeListener Members

        void IElementTreeChangeListener.OnElementAdded(RadElement addedElement)
        {
            if (!this.ShouldProcessElement(addedElement))
            {
                return;
            }

            //do not animate initial element state when adding it
            this.owner.ElementTree.SuspendAnimations();
            this.AddElementCore(addedElement);
            this.owner.ElementTree.ResumeAnimations();
        }

        private void AddElementCore(RadElement addedElement)
        {
            this.RemapStylesToElementsRecursive(addedElement);
            //test if element needs style from its parent
            if (addedElement.Style != null)
            {
                return;
            }

            bool parentNotPropagateStyleToChildren;

            StyleMap styleMap = this.FindStyleMapForElement(addedElement, out parentNotPropagateStyleToChildren);
            if (styleMap != null)
            {
                styleMap.OnElementAdded(addedElement);
            }
            else
            {
                //TODO: elaborate more on this case
                if (!parentNotPropagateStyleToChildren && this.styleElementMap.Count > 0)
                {
                    //this happens when the control does not have any Style, like RadCarousel in the QSF
                    //Debug.Fail("Elemenet style not found");
                }
            }
        }

        void IElementTreeChangeListener.OnElementRemoved(RadElement formerParent, RadElement removedElement)
        {
            if (this.state != StyleManagerState.Attached ||
                this.stylingSuspendCounter > 0 ||
                this.styleElementMap.Count == 0)
            {
                return;
            }

            if (removedElement.IsDisposed || removedElement.IsThemeRefreshSuspended)
            {
                return;
            }

            this.RemoveElementCore(formerParent, removedElement);
        }

        void IElementTreeChangeListener.OnElementDisposed(RadElement formerParent, RadElement removedElement)
        {
            if (this.state != StyleManagerState.Attached ||
                this.stylingSuspendCounter > 0 ||
                this.styleElementMap.Count == 0)
            {
                return;
            }

            this.RemoveElementCore(formerParent, removedElement);
        }

        private void RemoveElementCore(RadElement formerParent, RadElement removedElement)
        {
            if (removedElement.Style != null)
            {
                this.RemoveStyleOwner(removedElement);
                return;
            }

            StyleMap styleMap = null;
            if(formerParent != null)
            {
                bool parentNotPropagateStyleToChildren;
                styleMap = this.FindStyleMapForElement(formerParent, out parentNotPropagateStyleToChildren);
            }
            if (styleMap != null)
            {
                styleMap.OnElementRemoved(removedElement);
            }
        }

        #endregion

        private StyleMap FindStyleMapForElement(RadElement radElement, out bool parentNotPropagateStyleToChildren)
        {
            parentNotPropagateStyleToChildren = false;
            StyleMap styleMap = null;

            for (RadElement toTest = radElement; toTest != null; toTest = toTest.Parent)
            {
                if (!toTest.PropagateStyleToChildren)
                {
                    parentNotPropagateStyleToChildren = true;
                    break;
                }

                if (toTest.Style == null)
                {
                    continue;
                }

                this.styleElementMap.TryGetValue(toTest.GetHashCode(), out styleMap);
                break;
            }

            return styleMap;
        }

        #region IPropertyChangeListener Members

        void IPropertyChangeListener.OnRadPropertyChanged(RadElement element, RadPropertyChangedEventArgs changeArgs)
        {
            if (changeArgs.Property == RadElement.StyleProperty && element.CanHaveOwnStyle)
            {
                this.ElementStyleChanged(element, changeArgs);
            }
            else if (changeArgs.Property == RadElement.ClassProperty ||
                     changeArgs.Property == RadItem.VisualStateProperty)
            {
                this.ElementStyleSelectorKeyPropertyChanged(element, changeArgs);
            }
        }

        private void ElementStyleSelectorKeyPropertyChanged(RadElement element, RadPropertyChangedEventArgs changeArgs)
        {
            if (this.state != StyleManagerState.Attached || this.styleElementMap.Count == 0 || 
                element.IsDisposing || element.IsDisposed)
            {
                return;
            }

            bool parentNotPropagateStyleToChildren;

            StyleMap styleMap = this.FindStyleMapForElement(element, out parentNotPropagateStyleToChildren);
            if (styleMap != null)
            {
                styleMap.OnElementStyleSelectorKeyPropertyChanged(element, changeArgs);
            }
        }

		private void ElementStyleChanged(RadElement element, RadPropertyChangedEventArgs changeArgs)
		{
			StyleMap styleMap;
            this.styleElementMap.TryGetValue(element.GetHashCode(), out styleMap);

#if DEBUG
            if (changeArgs.NewValue != null && element.IsThemeApplied)
            {
                for (RadElement toTest = element.Parent; toTest != null; toTest = toTest.Parent)
                {
                    if (!toTest.IsThemeApplied ||
                        !toTest.PropagateStyleToChildren)
                    {
                        break;
                    }

                    //Parent Style alredy set
                    if (toTest.Style != null)
                    {
                        StyleMap parentStyleMap = null;
                        this.styleElementMap.TryGetValue(toTest.GetHashCode(), out parentStyleMap);
                        if (styleMap != null)
                        {
                            styleMap.OnElementRemoved(element);
                        }
                        else if(this.state != StyleManagerState.Attaching)
                        {
                            Debug.Fail("Unknown Style instance found");
                        }
                    }
                }
            }
#endif

			if (changeArgs.NewValue == null)
			{
				if (styleMap != null)
				{
                    styleMap.SetStyleSheet(null);
					this.styleElementMap.Remove(element.GetHashCode());
				}
			}
			else
			{
				if (styleMap == null)
				{
					styleMap = new StyleMap(this, (StyleSheet)changeArgs.NewValue, element);
					this.styleElementMap.Add(element.GetHashCode(), styleMap);
				}
				else
				{
					styleMap.SetStyleSheet((StyleSheet)changeArgs.NewValue);
				}                    
			}
		}

        #endregion

        #region Internals For Unit Tests

        internal Dictionary<int, StyleMap> StyleMaps
        {
            get
            {
                return this.styleElementMap;
            }
        }

        #endregion
    }
}
