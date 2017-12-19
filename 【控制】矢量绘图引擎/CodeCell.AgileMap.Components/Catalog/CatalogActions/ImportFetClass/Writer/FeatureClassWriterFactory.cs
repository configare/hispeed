using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Components
{
    public static class FeatureClassWriterFactory
    {
        public static IFeatureClassWriter GetFetClassWriter(ICatalogItem locationItem)
        {
            if (locationItem is CatalogDatabaseConn ||
                locationItem is CatalogFeatureDataset)
                return new FetClassWriterDb(locationItem);
            throw new NotSupportedException("类型为\"" + locationItem + "\"的位置暂不支持写。");
        }
    }
}
