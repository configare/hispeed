using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using OSGeo.GDAL;
using CodeCell.Bricks.Runtime;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 1、定位数据集
    /// 2、定位字段:经度列，纬度列，其他属性列
    /// </summary>
    public class HdfGlobalFirePointReader : IVectorFeatureDataReader, IDisposable
    {
        private const int longIndex = 4;
        private const int latIndex = 3;

        protected Dataset _dataset = null;
        protected Band _band = null;
        private float[] _vectData;
        private int _fieldCount;
        private int _pointCount;
        private object lockObj = new object();

        private Envelope _envelope;
        private ISpatialReference _spatialReference;
        private enumShapeType _shapeType;
        private string[] _fields;
        private ArgOfLeveling _arg;
        private Feature[] _features;

        public HdfGlobalFirePointReader()
        {
            Gdal.AllRegister();
        }

        public Envelope Envelope
        {
            get { return _envelope; }
        }

        public int FeatureCount
        {
            get { return 1; }
        }

        public Feature[] Features
        {
            get { return _features; }
        }

        public string[] Fields
        {
            get { return _fields; }
        }

        public enumShapeType ShapeType
        {
            get { return _shapeType; }
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
        }

        public Feature[] GetFeatures(Envelope envelope)
        {
            if (_features == null)
                return null;
            if (envelope == null)
                return null;
            Envelope validExtent = _envelope.IntersectWith(envelope);
            if (validExtent == null)
                return null;
            List<Feature> retFets = new List<Feature>();
            foreach (Feature fet in _features)
            {
                if (validExtent.Contains(fet.Geometry.Envelope))
                {
                    retFets.Add(fet);
                }
                else
                {
                    if (fet.Geometry.Envelope.IsInteractived(validExtent))
                    {
                        retFets.Add(fet);
                        fet.IsRepeatedOverGrids = true;
                    }
                }
            }
            return retFets.Count > 0 ? retFets.ToArray() : null;
        }

        public void SetArgsOfLeveling(ArgOfLeveling arg)
        {
            _arg = arg;
        }

        public bool IsOK
        {
            get { return true; }
        }

        public bool TryOpen(string fileName, byte[] bytes, params object[] args)
        {
            try
            {
                string ext = Path.GetExtension(fileName).ToUpper();
                if (ext != ".HDF")
                    return false;
                _dataset = Gdal.Open(fileName, Access.GA_ReadOnly);
                if(_dataset.RasterCount != 1)
                    return false;
                _band = _dataset.GetRasterBand(1);
                if (_band.XSize != 9)
                    return false;

                _envelope = new Envelope(-180d, -90d, 180d, 90d);
                _spatialReference = Core.SpatialReference.GetDefault();
                _shapeType = enumShapeType.Point;

                _fields = ConstructFields();
                _vectData = ReadVectorBand();
                _features = ConstructPoint();
                return true;
            }
            catch (Exception ex)
            {
                Log.WriterException("HdfGlobalFirePointReader", "TryOpen", ex);
                return false;
            }
        }
        private float[] ReadVectorBand()
        {
            _fieldCount = _band.XSize;
            _pointCount = _band.YSize;
            Band band = _dataset.GetRasterBand(1);
            unsafe
            {
                float[] vcData = new float[_fieldCount * _pointCount];
                lock (lockObj)
                {
                    _band.ReadRaster(0, 0, _band.XSize, _band.YSize, vcData, _band.XSize, _band.YSize, 0, 0);
                }
                return vcData;
            }
        }

        private string[] ConstructFields()
        {
            return new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" };
        }

        private Feature[] ConstructPoint()
        {
            List<Feature> features = new List<Feature>();
            ShapePoint pt;
            string[] fieldValues;
            for (int oid = 0; oid < _pointCount; oid++)
            {
                fieldValues = new string[_fieldCount];
                pt = new ShapePoint(_vectData[oid * _fieldCount + longIndex], _vectData[oid * _fieldCount + latIndex]);
                for (int j = 0; j < _fieldCount; j++)
                {
                    fieldValues[j] = _vectData[oid * _fieldCount + j].ToString();
                }
                Feature f = new Feature(oid, pt, _fields, fieldValues, null);
                features.Add(f);
            }
            return features.ToArray();
        }

        public void Dispose()
        {
            if (_band != null)
                _band.Dispose();
            if (_dataset != null)
                _dataset.Dispose();
        }

        public Feature FetchFirstFeature()
        {
            return _features.First();
        }

        public Feature FetchFeature(Func<Feature, bool> where)
        {
            return null;
        }

        public Feature[] FetchFeatures(Func<Feature, bool> where)
        {
            return null;
        }

        public Feature[] FetchFeatures()
        {
            return _features;
        }
    }
}
