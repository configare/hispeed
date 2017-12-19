using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface ILayerItemGroup : ILayerItem
    {
        List<ILayerItem> Items { get; }
        ILayerItem FindParent(ILayerItem item);
    }
}
