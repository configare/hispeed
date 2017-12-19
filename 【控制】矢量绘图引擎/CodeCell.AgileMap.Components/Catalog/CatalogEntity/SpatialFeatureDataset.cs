using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Components
{
    /*
     *  <Field name="ID" type="VARCHAR(64)" description="GUID"/>
        <Field name="NAME" type="VARCHAR(64)" description="要素集名称"/>
        <Field name="SPATIALREF" type="VARCHAR(4096)" description="空间参考(OGC WKT标准,eg:ESRI prj File)"/>
        <Field name="CREATETIME" type="DATETIME" description="创建时间"/>
        <Field name="DESCRIPTION" type ="VARCHAR(256)" description="要素集描述信息"/>
        <Field name="MAPSCALE" type="INT32" description="测绘比例尺"/>
        <Field name="SOURCE" type="VARCHAR(256)" description="数据来源"/>
     */
    public class SpatialFeatureDataset:CatalogEntityBase
    {
        private string _spatialref = null; 
        private DateTime _createTime = DateTime.MinValue;
        private string _source = null;
        private int _mapscale = 0;
        private ICatalogEntity[] _spatialFeatureClasses = null;

        public SpatialFeatureDataset()
            :base()
        {
        }

        [AttToFieldMap("SpatialRef",typeof(string))]
        public string SpatialRef
        {
            get { return _spatialref; }
            set { _spatialref = value; }
        }

        [AttToFieldMap("CreateTime", typeof(DateTime))]
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        [AttToFieldMap("MapScale", typeof(int))]
        public int MapScale
        {
            get { return _mapscale; }
            set { _mapscale = value; }
        }

        [AttToFieldMap("Source", typeof(string))]
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && _spatialref == null && _createTime == DateTime.MinValue && _mapscale == 0 && _source == null;
        }

        public ICatalogEntity[] SpatialFeatureClasses
        {
            get 
            {
                LoadSpatialFeatureClasses();
                return _spatialFeatureClasses; 
            }
            set 
            {
                _spatialFeatureClasses = value; 
            }
        }

        private void LoadSpatialFeatureClasses()
        {
            using (ICatalogEntityClass c = new CatalogEntityClassFeatureClass(_connString))
            {
                ICatalogEntity[] objs = c.Query("datasetid='" + Id + "'");
                _spatialFeatureClasses = objs;
            }
        }

        protected override ICatalogEntityClass GetCatalogEntityClass()
        {
            return new CatalogEntityClassFeatureDataset(_connString);
        }
    }
}
