using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface ILayersProvider : ILayerItemGroup
    {
        List<ILayerItem> Items { get; }
        void Update();
        void Remove(ILayerItem item);
        void AdjustOrder(int insertIndex, ILayerItem item);
        void AdjustOrder(int insertIndex, ILayerItem item,ILayerItem parentItem);
        void Group(ILayerItem item, ILayerItemGroup group);
        string SupportedFilters { get; }
        ILayerItem AddLayer(string fname);
        void RefreshViewer();
        void SetLayerName(string newName, ILayerItem item);
        void SaveVectorItemShowMethod(ILayerItem item);
    }
}
