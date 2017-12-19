using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.AgileMap.Components
{
    public static class FeatureClassReaderFactory
    {
        public static IFeatureClassReader GetFeatureClassReader(ICatalogItem fetclassItem)
        {
            if (fetclassItem is CatalogFile)
                return GetFeatureClassReaderForFile(fetclassItem as CatalogFile);
            throw new NotSupportedException("数据源\"" + fetclassItem.Tag.ToString() + "\"的要素读取器未实现。");
        }

        private static IFeatureClassReader GetFeatureClassReaderForFile(CatalogFile catalogFile)
        {
            string file = catalogFile.Tag.ToString().ToUpper();
            if (file.EndsWith(".SHP"))
                return new FetClassReaderForShpFile(file) as IFeatureClassReader;
            throw new NotSupportedException("数据源\"" +file + "\"的要素读取器未实现。");
        }
    }
}
