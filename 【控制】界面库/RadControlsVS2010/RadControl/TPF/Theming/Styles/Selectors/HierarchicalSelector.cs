using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls
{	
    /// <summary>
    /// Represents a base class for other selectors. telerik presentation framework
    /// selectors are similar to CSS selectors.
    /// </summary>
    public abstract class HierarchicalSelector: SelectorBase
    {
        private IDictionary childrenHierarchyByElement = null;

        internal abstract protected bool CanUseCache
        { 
            get;
        }        

        //Method supports old theming infrastructure and will be depricated
		/// <summary>Retrieves the selected elements of the given element.</summary>
        public override LinkedList<RadElement> GetSelectedElements(RadElement element)
        {
            LinkedList<RadElement> res;

            if (childrenHierarchyByElement == null || !CanUseCache)
            {
                res = new LinkedList<RadElement>();

                if (this.CanSelectOverride(element))
                {
                    res.AddLast(element);
                }

                if (this.CanSelectOverride(element))
                {
                    res.AddLast(element);
                }

                RadElementReadonlyList selectedElements = new RadElementReadonlyList(element.ChildrenHierarchy);
                for (int i = 0; i < selectedElements.Count; i++)
                {
                    RadElement child = selectedElements[i];
                    if (this.CanSelectIgnoringConditions(child))
                    {
                        res.AddLast(child);
                    }
                }

                return res;
            }
            else
            {
                res = this.FindElements(childrenHierarchyByElement);                
            }

			if (res == null)
			{
				res = new LinkedList<RadElement>();
			}			

            return res;
        }

		protected virtual LinkedList<RadElement> FindElements(IDictionary childrenHierarchyByElement)
		{			
			return null;
		}

		internal void SetCache(IDictionary cache)
		{
			this.childrenHierarchyByElement = cache;
		}		
    }
}
