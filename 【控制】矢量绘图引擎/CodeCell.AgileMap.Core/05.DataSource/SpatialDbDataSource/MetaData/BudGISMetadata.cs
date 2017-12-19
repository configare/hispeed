using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace CodeCell.AgileMap.Core
{
    /*
     * <BudGISMetadata version ="1.0">
          <SystemTables>
            <SystemTable name="BGIS_FeatureDataset" description="要素集，等价于图层集合,共享同样空间参考的一组矢量数据">
              <Fields>
                <Field name="ID" type="VARCHAR(64)" description="GUID"/>
                <Field name="NAME" type="VARCHAR(64)" description="要素集名称"/>
                <Field name="SPATIALREF" type="VARCHAR(4096)" description="空间参考(OGC WKT标准,eg:ESRI prj File)"/>
                <Field name="CREATETIME" type="DATETIME" description="创建时间"/>
                <Field name="DESCRIPTION" type ="VARCHAR(256)" description="要素集描述信息"/>
                <Field name="MAPSCALE" type="INT32" description="测绘比例尺"/>
                <Field name="SOURCE" type="VARCHAR()256" description="数据来源"/>
              </Fields>
            </SystemTable>
         </SystemTables>
      </BudGISMetadata version ="1.0">
     */
    public class BudGISMetadata
    {
        private List<SystemTable> _tables = new List<SystemTable>();
        public  const string cstFeatureDatasetTableName = "BGIS_FeatureDataset";
        public  const string cstFeatureClassTableName = "BGIS_FeatureClass";
        public  const string cstMapTableName = "BGIS_Map";

        public BudGISMetadata()
        {
            using (Stream st = GetType().Assembly.GetManifestResourceStream("CodeCell.AgileMap.Core.BudGISMetadata.xml"))
            {
                XmlReader r = new XmlTextReader(st);
                XDocument doc = XDocument.Load(r);
                Parse(doc);
                r.Close();
            }
        }

        public BudGISMetadata(string metadatafile)
        {
            XDocument doc = XDocument.Load(metadatafile);
            Parse(doc);
        }

        private void Parse(XDocument doc)
        {
            var tables = doc.Element("BudGISMetadata").Element("SystemTables").Elements();
            foreach (XElement ele in tables)
                _tables.Add(new SystemTable(ele));
        }

        public SystemTable[] Tables
        {
            get { return _tables.ToArray(); }
        }
    }
}
