using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public static class DataSourceDialog
    {
        public static IDataSource OpenDataSource()
        {
            using (frmDataSource frm = new frmDataSource())
            {
                frm.ucBudGISDataSource1.SelectableType = enumCatalogItemType.FeatureClass;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ICatalogItem citem = frm.ucBudGISDataSource1.SelectedCatalogItem;
                    if (citem is CatalogFile)
                    {
                        return GetDataSource(citem as CatalogFile);
                    }
                    else if (citem is CatalogFeatureClass)
                    {
                        return GetDataSource(citem as CatalogFeatureClass);
                    }
                    else if (citem is CatalogNetFeatureClass)
                    {
                        return GetDataSource(citem as CatalogNetFeatureClass);
                    }
                    else
                    {
                        throw new NotSupportedException("暂时不支持类型为\"" + citem.Tag.ToString() + "\"的数据源。");
                    }
                }
            }
            return null;
        }

        private static FeatureDataSourceBase GetDataSource(CatalogNetFeatureClass catalogNetFeatureClass)
        {
            string uri = null;
            InstanceIdentify instanceId = null ;
            if (catalogNetFeatureClass.Parent is CatalogNetFeatureDataset)
            {
                instanceId = catalogNetFeatureClass.Parent.Parent.Tag as InstanceIdentify;
                uri = catalogNetFeatureClass.Parent.Parent.Parent.Tag.ToString();//CatalogNetServer
                //                                        CatalogNetFeatureDataset.CatalogNetInstance.CatalogNetServer
            }
            else
            {
                instanceId = catalogNetFeatureClass.Parent.Tag as InstanceIdentify;
                uri = catalogNetFeatureClass.Parent.Parent.Tag.ToString();//CatalogNetServer
                //                                         CatalogNetFeatureClass.CatalogInstance.CatalogNetServer
            }
            FetClassIdentify fetclassId = catalogNetFeatureClass.Tag as FetClassIdentify;
            return new ServerDataSource(catalogNetFeatureClass.Name, uri, instanceId, fetclassId);
        }

        private static FeatureDataSourceBase GetDataSource(CatalogFeatureClass catalogFeatureClass)
        {
            FeatureDataSourceBase ds = null;
            string name = catalogFeatureClass.Name;
            SpatialFeatureClass fetc = catalogFeatureClass.Tag as SpatialFeatureClass;
            ds = new SpatialDbDataSource(name, fetc._connString + "@" + fetc.DataTable);
            return ds;
        }

        private static IDataSource GetDataSource(CatalogFile catalogFile)
        {
            IDataSource ds = null;
            string name = Path.GetFileNameWithoutExtension(catalogFile.Name);
            //if (catalogFile is CatalogRasterFile)
            //{
            //    ds = new RasterDataSource(catalogFile.Tag.ToString());
            //}
            if (catalogFile is CatalogFile)
            {
                ds = new FileDataSource(name, catalogFile.Tag.ToString());
            }
            return ds;
        }
    }
}
