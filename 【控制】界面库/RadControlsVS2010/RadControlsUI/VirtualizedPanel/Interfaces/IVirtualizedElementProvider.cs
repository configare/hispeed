using System;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public interface IVirtualizedElementProvider<T>
    {
        IVirtualizedElement<T> GetElement(T data, object context);
        bool CacheElement(IVirtualizedElement<T> element);
        bool ShouldUpdate(IVirtualizedElement<T> element, T data, object context);
        bool IsCompatible(IVirtualizedElement<T> element, T data, object context);
        SizeF GetElementSize(T data);
        SizeF GetElementSize(IVirtualizedElement<T> element);
        SizeF DefaultElementSize { get; set; }
        void ClearCache();
    }
}