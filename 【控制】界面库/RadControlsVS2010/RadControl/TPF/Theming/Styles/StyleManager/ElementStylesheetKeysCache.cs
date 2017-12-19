using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.Styles
{
    //Class is part of ptimization attempt - now not used
    public class ElementStylesheetKeysCache
    {
        IDictionary elementClassCache = new Hashtable();
        IDictionary elementTypeCache = new Hashtable();

        public bool IsCacheEmlty
        {
            get
            {
                return this.elementClassCache == null || this.elementTypeCache == null ||
                    (this.elementClassCache.Count == 0 && this.elementTypeCache.Count == 0);
            }
        }


        public IDictionary ElementClassCache
        {
            get
            {
                return elementClassCache;
            }
            set
            {
                this.elementClassCache = value;
            }
        }

        public IDictionary ElementTypeCache
        {
            get
            {
                return elementTypeCache;
            }
            set
            {
                this.elementTypeCache = value;
            }
        }

        private static void AddElementToCacheByClass(RadElement element, IDictionary classCache)
        {
            string elementClass = element.Class;

            if (string.IsNullOrEmpty(elementClass))
            {
                return;
            }

            LinkedList<RadElement> elementList = classCache[elementClass] as LinkedList<RadElement>;
            if (elementList == null)
            {
                elementList = new LinkedList<RadElement>();
                classCache[elementClass] = elementList;
            }

            elementList.AddLast(element);
        }

        private static void AddElementToCacheByType(RadElement element, IDictionary typeCache)
        {
            Type elementType = element.GetThemeEffectiveType();
            LinkedList<RadElement> elementList = typeCache[elementType] as LinkedList<RadElement>;
            if (elementList == null)
            {
                elementList = new LinkedList<RadElement>();
                typeCache[elementType] = elementList;
            }

            elementList.AddLast(element);
        }

        internal static void AddElementToCache(RadElement element, IDictionary elementClassCache, IDictionary elementTypeCache)
        {
            if (!string.IsNullOrEmpty(element.Class))
            {
                AddElementToCacheByClass(element, elementClassCache);
            }

            AddElementToCacheByType(element, elementTypeCache);
        }

        public void PrepareCache(IEnumerable<RadElement> elements)
        {
            foreach (RadElement element in elements)
            {
                this.PrepareCache(element);
            }
        }

        public void PrepareCache(RadElement rootElement)
        {
            if (this.elementClassCache == null)
            {
                this.elementClassCache = new Hashtable();
            }
            if (this.elementTypeCache == null)
            {
                this.elementTypeCache = new Hashtable();
            }

            foreach (RadElement element in ElementHierarchyEnumerator.TraverseElements(rootElement))
            {
                AddElementToCache(element, this.elementClassCache, this.elementTypeCache);
            }
        }
    }
}
