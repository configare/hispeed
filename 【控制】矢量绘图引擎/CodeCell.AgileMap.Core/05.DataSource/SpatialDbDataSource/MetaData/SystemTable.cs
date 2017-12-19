using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    /*
       <SystemTable name="BGIS_FeatureDataset" description="要素集，等价于图层集合,共享同样空间参考的一组矢量数据">
          <Fields>
            <Field name="ID" type="VARCHAR(64)" description="GUID"/>
            <Field name="NAME" type="VARCHAR(64)" description="要素集名称"/>
            <Field name="SPATIALREF" type="VARCHAR(4096)" description="空间参考(OGC WKT标准,eg:ESRI prj File)"/>
            <Field name="CREATETIME" type="DATETIME" description="创建时间"/>
            <Field name="DESCRIPTION" type ="VARCHAR(256)" description="要素集描述信息"/>
            <Field name="MAPSCALE" type="INT" description="测绘比例尺"/>
            <Field name="SOURCE" type="VARCHAR()256" description="数据来源"/>
          </Fields>
        </SystemTable>
     */
    public class SystemTable
    {
        private string _name = null;
        private string _description = null;
        private List<SystemField> _fields = new List<SystemField>();

        public SystemTable(XElement xelement)
        {
            _name = xelement.Attribute("name").Value;
            _description = xelement.Attribute("description").Value;
            var flds = xelement.Element("Fields").Elements();
            foreach (XElement ele in flds)
                _fields.Add(new SystemField(ele));
        }

        public string Name
        {
            get { return _name; }
        }

        public SystemField[] Fields
        {
            get { return _fields.ToArray(); }
        }

        public string Description
        {
            get { return _description; }
        }
    }
}
