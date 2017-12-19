using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Components
{
    /*
     *  <Field name="DATASETID"  type="VARCHAR(64)" description="要素集ID,NULL单独要素集"/>
        <Field name="NAME" type="VARCHAR(64)" description="要素类名称"/>
        <Field name="DATATABLE" type="VARCHAR" description ="要素数据表"/>
        <Field name="SPATIALREF" type="VARCHAR(4096)" description="空间参考(OGC WKT标准,eg:ESRI prj File)"/>
        <Field name="CREATETIME" type="DATETIME" description="创建时间"/>
        <Field name="MAPSCALE" type="INT32" description="测绘比例尺"/>
        <Field name="SOURCE" type="VARCHAR(256)" description="数据来源"/>
        <Field name="DESCRIPTION" type="VARCHAR(256)" description="要素类描述"/>
     */
    public class MetaFeatureClassDef
    {
        private string _datasetid = string.Empty;
        private string _name = string.Empty;
        private string _datatable = string.Empty;
        private string _spatialref = string.Empty;
        private DateTime _createtime = DateTime.Now ;
        private int _mapscale = 0;
        private string _source = string.Empty;
        private string _description = string.Empty;

        public MetaFeatureClassDef()
        { 
        }

        public string DatasetId
        {
            get { return _datasetid; }
            set { _datasetid = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string DataTable
        {
            get { return _datatable; }
            set { _datatable = value; }
        }

        public string SpatialRef
        {
            get { return _spatialref; }
            set { _spatialref = value; }
        }

        public DateTime DataTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }

        public int MapScale
        {
            get { return _mapscale; }
            set { _mapscale = value; }
        }

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
