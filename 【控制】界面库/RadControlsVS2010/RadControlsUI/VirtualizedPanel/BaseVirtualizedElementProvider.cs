using System;
using System.Collections.Generic;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public abstract class BaseVirtualizedElementProvider<T> : IVirtualizedElementProvider<T>
    {
        private SizeF elementSize = new SizeF(20, 20);
        private List<IVirtualizedElement<T>> cachedElements = new List<IVirtualizedElement<T>>();

        public SizeF DefaultElementSize
        {
            get { return this.elementSize; }
            set { this.elementSize = value; }
        }

        public abstract IVirtualizedElement<T> CreateElement(T data, object context);

        public virtual IVirtualizedElement<T> GetElementFromCache(T data, object context)
        {
            foreach (IVirtualizedElement<T> element in cachedElements)
            {
                if (IsCompatible(element, data, context))
                {
                    this.cachedElements.Remove(element);
                    this.PreInitializeCachedElement(element, context);
                    return element;
                }
            }
            return null;
        }

        public virtual IVirtualizedElement<T> GetElement(T data, object context)
        {
            IVirtualizedElement<T> element = GetElementFromCache(data, context);
            if (element != null)
            {
                return element;
            }
            return this.CreateElement(data, context);
        }

        protected virtual void PreInitializeCachedElement(IVirtualizedElement<T> element, object context)
        {

        }

        public virtual bool CacheElement(IVirtualizedElement<T> element)
        {
            cachedElements.Add(element);
            return true;
        }

        public virtual bool ShouldUpdate(IVirtualizedElement<T> element, T data, object context)
        {
            return !element.Data.Equals(data) || !IsCompatible(element, data, context);
        }

        public virtual bool IsCompatible(IVirtualizedElement<T> element, T data, object context)
        {
            return element.IsCompatible(data, context);
        }

        public virtual SizeF GetElementSize(T item)
        {
            return elementSize;
        }

        public virtual SizeF GetElementSize(IVirtualizedElement<T> element)
        {
            return GetElementSize(element.Data);
        }

        public virtual void ClearCache()
        {
            foreach (IVirtualizedElement<T> virtualizedElement in this.cachedElements)
            {
                IDisposable disposable = virtualizedElement as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            this.cachedElements.Clear();
        }

        public virtual int CachedElementsCount
        {
            get { return this.cachedElements.Count; }
        }

    }
}