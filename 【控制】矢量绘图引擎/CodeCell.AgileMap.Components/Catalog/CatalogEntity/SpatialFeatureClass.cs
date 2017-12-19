using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    /*
     *  <Field name="ID" type="VARCHAR(64)" description="GUID"/>
        <Field name="DATASETID"  type="VARCHAR(64)" description="要素集ID,NULL单独要素集"/>
        <Field name="NAME" type="VARCHAR(64)" description="要素类名称"/>
        <Field name="DATATABLE" type="VARCHAR(128)" description ="要素数据表"/>
        <Field name="SPATIALREF" type="VARCHAR(4096)" description="空间参考(OGC WKT标准,eg:ESRI prj File)"/>
        <Field name="CREATETIME" type="DATETIME" description="创建时间"/>
        <Field name="MAPSCALE" type="INT32" description="测绘比例尺"/>
        <Field name="SOURCE" type="VARCHAR(256)" description="数据来源"/>
        <Field name="DESCRIPTION" type="VARCHAR(256)" description="要素类描述"/>
     */
    public class SpatialFeatureClass:CatalogEntityBase
    {
        private string _datasetid = null;
        private string _datatable = null;
        private string _spatialref = null;
        private DateTime _createTime = DateTime.MinValue;
        private int _mapscale = 0;
        private SpatialFeatureDataset _dataset = null;
        private string _source = null;
        private string _annoTable = null;
        private enumShapeType _shapeType = enumShapeType.NullShape;
        private Envelope _envelope = null;
        private int _featureCount = 0;

        public SpatialFeatureClass()
            : base()
        { 
        }

        [AttToFieldMap("DatasetId", typeof(string))]
        public string DatasetId
        {
            get { return _datasetid; }
            set { _datasetid = value; }
        }

        [AttToFieldMap("AnnoTable", typeof(string))]
        public string AnnoTable
        {
            get { return _annoTable; }
            set { _annoTable = value; }
        }

        [AttToFieldMap("ShapeType", typeof(enumShapeType))]
        public enumShapeType ShapeType
        {
            get { return _shapeType; }
            set { _shapeType = value; }
        }

        [AttToFieldMap("Envelope", typeof(Envelope))]
        public Envelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        [AttToFieldMap("FeatureCount", typeof(int))]
        public int FeatureCount
        {
            get { return _featureCount; }
            set { _featureCount = value; }
        }

        private StatInfoFetClass _statInfo = null;
        internal StatInfoFetClass GetStatInfo()
        {
            if (_statInfo == null)
                _statInfo = new StatInfoFetClass(_shapeType, _featureCount, _envelope);
            return _statInfo;
        }

        public SpatialFeatureDataset SpatialFeatureDataset
        {
            get 
            {
                if (_dataset == null && _datasetid != null)
                {
                    using (ICatalogEntityClass c = new CatalogEntityClassFeatureDataset(_connString))
                    {
                        ICatalogEntity[] dses = c.Query("id='" + _datasetid+"'");
                        if(dses != null && dses.Length>0)
                            _dataset = dses[0] as SpatialFeatureDataset;
                    }
                }
                return _dataset;
            }
        }

        [AttToFieldMap("DataTable", typeof(string))]
        public string DataTable
        {
            get { return _datatable; }
            set { _datatable = value; }
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

        [AttToFieldMap("SpatialRef", typeof(string))]
        public string SpatialRef
        {
            get { return _spatialref; }
            set { _spatialref = value; }
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && _datasetid == null && _datatable == null && _createTime == DateTime.MinValue && _mapscale == 0 && _spatialref == null;
        }

        protected override ICatalogEntityClass GetCatalogEntityClass()
        {
            return new CatalogEntityClassFeatureClass(_connString);
        }
    }
}
