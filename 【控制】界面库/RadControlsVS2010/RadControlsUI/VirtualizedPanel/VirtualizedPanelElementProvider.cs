using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class VirtualizedPanelElementProvider<T, T1>: BaseVirtualizedElementProvider<T> where T1: IVirtualizedElement<T>, new()
    {
        public override IVirtualizedElement<T> CreateElement(T data, object context)
        {
            return new T1();
        }

        public override bool ShouldUpdate(IVirtualizedElement<T> element, T data, object context)
        {
            return !element.Data.Equals(data);
        }        
    }
}
