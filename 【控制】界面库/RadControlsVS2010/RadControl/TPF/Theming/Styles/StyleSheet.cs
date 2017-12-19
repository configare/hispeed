using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Telerik.WinControls.Themes.Serialization;
using System;
using System.Diagnostics;
using System.Collections.Specialized;
using Telerik.WinControls.Styles;


namespace Telerik.WinControls
{
	public interface ISelectorCacheHelper
	{
		bool TraverseChildren
		{
			get;
		}

		void RegisterElementsInCache(IDictionary cache);
	}

    /// <summary>
    /// 	<strong>StyleSheet</strong> object defines the appearance and/or certain aspects
    /// of behavior of one <strong>RadControl</strong> or <strong>RadItem</strong>.
    /// </summary>
    /// <remarks>
    /// 	<strong>StyleSheet</strong> objects can be persisted (in XML) and used within
    ///     <see cref="Telerik.WinControls.Theme"/> (described in a topic above). This
    ///     class can also be used programmatically to define values of certain properties of
    ///     <strong>RadElements</strong> by assigning an instance of
    ///     <strong>StyleSheet</strong> class to <see cref="RadElement.Style"/>
    ///     property.<br/>
    ///     Values can also be applied to properties of <strong>RadElement</strong>, only when
    ///     certain condition occurs (on mouse hover for example).
    /// </remarks>
	[ToolboxItem(false), ComVisible(false)]
	public class StyleSheet : RadComponent
	{
        private PropertySettingGroupCollection propertySettingGroups = new PropertySettingGroupCollection();
        private string themeLocation;

        public StyleSheet()
        {
        }

        public StyleSheet(string themeLocation)
        {
            this.themeLocation = themeLocation;
        }

        /// <summary>
		/// Saves the current Stylesheet object tree to stream
		/// </summary>
		/// <param name="toStream"></param>
        public void SaveStylesheet(Stream toStream)
		{
			//XmlSerializer ser = new XmlSerializer(typeof(XmlStyleSheet));
            //ser.Serialize(toStream, new XmlStyleSheet(this));
			XmlWriter writer = XmlWriter.Create(toStream);
			XmlStyleSheet converted = new XmlStyleSheet(this);
			new StyleXmlSerializer().WriteObjectElement(writer, converted);
		}
		
		/// <summary>
		/// Read the current Stylesheet object from stream /file/, previously created with <see cref="SaveStylesheet"/>
		/// </summary>
		/// <param name="fromStream"></param>
		/// <returns></returns>
        public static StyleSheet LoadStylesheet(Stream fromStream)
        {
			//XmlSerializer ser = new XmlSerializer(typeof(XmlStyleSheet));
			XmlStyleSheet res = new XmlStyleSheet();
			XmlReader reader = XmlReader.Create(fromStream);
			new StyleXmlSerializer().ReadObjectElement(reader, res);

            return res.GetStyleSheet();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual PropertySettingGroupCollection PropertySettingGroups
        {
            get 
            {
                return this.propertySettingGroups;
            }
        }

        /// <summary>
        /// Gets theme location if the StyleSheet originates from a theme file.
        /// </summary>
        [Description("Gets theme location if the StyleSheet originates from a theme file")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ThemeLocation
        {
            get { return themeLocation; }            
        }

        private Hashtable cachedChildrenHierarchyByElement = new Hashtable();

		private void AddElementToCacheByClass(RadElement element)
		{
            LinkedList<RadElement> elementList = cachedChildrenHierarchyByElement[element.Class] as LinkedList<RadElement>;
			if (elementList == null)
			{
                elementList = new LinkedList<RadElement>();				
				cachedChildrenHierarchyByElement[element.Class] = elementList;
			}

			elementList.AddLast(element);
		}

		private void AddElementToCacheByType(RadElement element)
		{
            LinkedList<RadElement> elementList = cachedChildrenHierarchyByElement[element.GetThemeEffectiveType()] as LinkedList<RadElement>;
			if (elementList == null)
			{
                elementList = new LinkedList<RadElement>();
				cachedChildrenHierarchyByElement[element.GetThemeEffectiveType()] = elementList;
			}

			elementList.AddLast(element);
		}

		public void ProcessStyle(RadElement rootElement, bool isDynamicallyAddedElement)
		{
			PrepareCache(rootElement);

			for (int i = 0; i < this.PropertySettingGroups.Count; i++)
			{
				PropertySettingGroup group = this.PropertySettingGroups[i];
                group.SetIsFromStyleSheet();
				for (int k = 0; k < group.Selectors.Count; k++)
				{
					IElementSelector selector = group.Selectors[k];
                    if (selector is HierarchicalSelector)
					{
						((HierarchicalSelector) selector).SetCache(this.cachedChildrenHierarchyByElement);
					}

                    //if (isDynamicallyAddedElement && selector is GeneralSelector)
                    //{
                    //    continue;
                    //}

					LinkedList<RadElement> selectedElements = selector.GetSelectedElements(rootElement);
					foreach(RadElement selected in selectedElements)
					{
						selected.AddRangeBehavior(selector.GetBehaviors(group));
						selected.RoutedEventBehaviors.AddRange(selector.GetRoutedEventBehaviors(group));
					}
				}
			}
		}

		private void AddElementToCache(RadElement element)
		{
			if (!string.IsNullOrEmpty(element.Class))
			{
				AddElementToCacheByClass(element);
			}

			AddElementToCacheByType(element);
		}

		/*private void PrepareCache(RadElement rootElement)
		{
			this.AddElementToCache(rootElement);

			RadElementReadonlyList children = rootElement.ChildrenHierarchy;
			for (int i = 0; i < children.Count; i++)
			{
				AddElementToCache(children[i]);
			}
		}*/

		private void PrepareCache(RadElement rootElement)
		{
			this.AddElementToCache(rootElement);

			ISelectorCacheHelper helper = rootElement as ISelectorCacheHelper;
			if (helper != null)
			{
				helper.RegisterElementsInCache(cachedChildrenHierarchyByElement);
				return;
			}

			RadElementCollection children = rootElement.Children;
			int depth = 1;
			int currIndex = 0;
			bool traversingChildren = true;
			do
			{
				if (currIndex >= children.Count)
				{
					depth -= 1;
					RadElement parent = children.Owner.Parent;
					if (parent != null)
					{
						currIndex = parent.Children.IndexOf(children.Owner);
						children = parent.Children;
						traversingChildren = false;
					}
				}
				else
				{
					RadElement child = children[currIndex];
                    StyleSheet childStyle = child.Style;

					if (traversingChildren)
					{						
						if (child.PropagateStyleToChildren && 
                            (childStyle == null || !child.ElementThemeAffectsChildren) && 
                            child.Children.Count > 0)
						{
							helper = child as ISelectorCacheHelper;
							if (helper != null)
							{
								helper.RegisterElementsInCache(cachedChildrenHierarchyByElement);
							}
							else
							{
								children = child.Children;
								currIndex = 0;
								depth++;
								continue;
							}
						}
					}

					if (childStyle == null /*&& child.PropagateStyleToChildren*/)
					{
						AddElementToCache(child);
					}
					currIndex++;
					traversingChildren = true;
				}
			}
			while (depth > 0);			
		}

		public void ProcessStyle(RadElement rootElement)
		{
			this.ProcessStyle(rootElement, false);
		}

        private class NotAppliedSelector
        {
            private IElementSelector selector;
            private PropertySettingCollection propertySettings;

            public NotAppliedSelector(IElementSelector selector, PropertySettingCollection propertySettings)
            {
                this.selector = selector;
                this.propertySettings = propertySettings;
            }

            public IElementSelector Selector
            {
                get { return selector; }
            }

            public PropertySettingCollection PropertySettings
            {
                get { return propertySettings; }
            }
        }

		public void ApplyStyle(RadElement rootElement)
		{
			this.ApplyStyle(rootElement, false);
		}

		public void ApplyStyle(RadElement rootElement, bool isDynamicallyAddedElement)
        {
			//bool animationsEnabled = AnimatedPropertySetting.AnimationsEnabled;
            //AnimatedPropertySetting.AnimationsEnabled = false;

            //first should be applied selectors without any apply conditions
            LinkedList<NotAppliedSelector> selectorsWithConditions = new LinkedList<NotAppliedSelector>();

			for (int i = 0; i < this.PropertySettingGroups.Count; i++)
			{
				PropertySettingGroup group = this.PropertySettingGroups[i];
				for (int k = 0; k < group.Selectors.Count; k++)
				{
					IElementSelector selector = group.Selectors[k];
                    //if (isDynamicallyAddedElement && selector is GeneralSelector)
                    //{
                    //    continue;
                    //}

					if (selector.HasApplyCondition)
					{
						selectorsWithConditions.AddLast(new NotAppliedSelector(selector, group.PropertySettings));
					}
					else
					{
						//Selector selects internally all applicable elements
						//selector.Apply(rootElement, group.PropertySettings);

						LinkedList<RadElement> selectedElements = selector.GetSelectedElements(rootElement);
						foreach(RadElement selected in selectedElements)
						{
                            selector.Apply(selected, group.PropertySettings);
						}
					}
				}
			}

            foreach(NotAppliedSelector notApplied in selectorsWithConditions)
            {
                //Selector selects internally all applicable elements
                //notApplied.Selector.Apply(rootElement, notApplied.PropertySettings);                
                foreach (RadElement selected in notApplied.Selector.GetSelectedElements(rootElement))
                {
                    notApplied.Selector.Apply(selected, notApplied.PropertySettings);
                }
            }

			//Temp optimization
			this.cachedChildrenHierarchyByElement.Clear();
        }
       
        public virtual void Unapply(RadElement radElement)
        {
			if (cachedChildrenHierarchyByElement.Count == 0)
				PrepareCache(radElement);

            for (int i = 0; i < this.PropertySettingGroups.Count; i++)
        	{
				PropertySettingGroup group = this.PropertySettingGroups[i];
        		for(int k = 0; k < group.Selectors.Count; k++)
        		{
					IElementSelector selector = group.Selectors[k];
        			if (selector is HierarchicalSelector)
        			{
        				((HierarchicalSelector) selector).SetCache(this.cachedChildrenHierarchyByElement);
        			}

					LinkedList<RadElement> selectedElements = selector.GetSelectedElements(radElement);
                    foreach (RadElement selected in selectedElements)
					{
						selector.Unregister(selected, group.PropertySettings);
					}
        		}
        	}

			for (int i = 0; i < this.PropertySettingGroups.Count; i++)
			{
				PropertySettingGroup group = this.PropertySettingGroups[i];
				for (int k = 0; k < group.Selectors.Count; k++)
				{
					IElementSelector selector = group.Selectors[k];
                    LinkedList<RadElement> selectedElements = selector.GetSelectedElements(radElement);
                    foreach (RadElement selected in selectedElements)
					{
						PropertyChangeBehaviorCollection propertyChangeBehaviors = selector.GetBehaviors(group);
                        for( int behaviorIndex =0; behaviorIndex < propertyChangeBehaviors.Count; behaviorIndex++ )
                        {
							PropertyChangeBehavior toRemove = propertyChangeBehaviors[behaviorIndex];
                            toRemove.BehaviorRemoving(selected);
                        }

                        RoutedEventBehaviorCollection routedEventBehaviors = selector.GetRoutedEventBehaviors(group);
						for(int eventIndex = 0; eventIndex < routedEventBehaviors.Count; eventIndex++)
                        {
							RoutedEventBehavior routedEventBehavior = routedEventBehaviors[eventIndex];
                            routedEventBehavior.BehaviorRemoving(selected);
                        }

                        selected.RemoveRangeBehaviors(propertyChangeBehaviors);
                        selected.RemoveRangeRoutedEventBehaviors(routedEventBehaviors);
                    }					
                }
            }

			this.cachedChildrenHierarchyByElement.Clear();
        }

        public void ProcessGroupsInheritance(XmlStyleRepository repository)
        {
            foreach( PropertySettingGroup group in this.PropertySettingGroups )
            {
                if (string.IsNullOrEmpty(group.BasedOn))
                {
                    continue;
                }

                group.PropertySettings.ResetInheritedProperties();

                foreach (XmlRepositoryItem item in this.GetRepositoryItems(repository, group))
                {
                    group.InheritRepositoryItem(item);
                }
            }
        }

        public XmlRepositoryItem[] GetRepositoryItems(XmlStyleRepository repository, PropertySettingGroup group)
        {
            string[] keys = group.GetBasedOnRepositoryItems();
            List<XmlRepositoryItem> items = new List<XmlRepositoryItem>();

            foreach (string key in keys)
            {
                foreach (XmlRepositoryItem item in repository.RepositoryItems)
                {
                    if (item.Key == key)
                    {
                        items.Add(item);
                    }
                }
            }

            return items.ToArray();
        }
    }    
}