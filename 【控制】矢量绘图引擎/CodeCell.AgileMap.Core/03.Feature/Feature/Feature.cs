using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class Feature:ICloneable,IDisposable,ISupportOutsideIndicator
    {
        protected Shape _geometry = null;
        protected string[] _fieldNames = null;
        protected string[] _fieldValues = null;
        protected int _oid = -1;
        [NonSerialized]
        protected ILabelLocationService _labelLocationService = null;
        protected bool _repeatedOverGirds = false;
        protected IOutsideIndicator _outsideIndicator = null;
        protected LabelLocation[] _annotations = null;
        protected bool _projected = false;
        public bool TempFlag = false;
        protected IFeatureClass _featureClass = null;

        public Feature(int oid, Shape geometry, string[] fieldnames, string[] fieldvalues,LabelLocation[] annotations)
        {
            _oid = oid;
            _geometry = geometry;
            _fieldNames = fieldnames;
            _fieldValues = fieldvalues;
            _annotations = annotations;
            _outsideIndicator = new OutsideIndicator();
        }

        internal void SetLabelLocationService(ILabelLocationService service)
        {
            _labelLocationService = service;
        }

        public IFeatureClass FeatureClass
        {
            get { return _featureClass; }
        }

        internal void SetFeatureClass(IFeatureClass fetclass)
        {
            _featureClass = fetclass;
        }

        public bool Projected
        {
            get { return _projected; }
            set { _projected = value; }
        }

        public bool IsRepeatedOverGrids
        {
            get { return _repeatedOverGirds; }
            set { _repeatedOverGirds = value; }
        }

        public ILabelLocationService LabelLocationService
        {
            get
            {
                if (_labelLocationService == null)
                {
                    _labelLocationService = new LabelLocationServiceDefault(null);
                    _labelLocationService.Update(_geometry);
                }
                return _labelLocationService; 
            }
        }

        public LabelLocation[] Annotations
        {
            get { return _annotations; }
        }

        public int OID
        {
            get { return _oid; }
        }

        public Shape Geometry
        {
            get { return _geometry; }
            set 
            {
                if (_geometry != null && _geometry.Equals(value))
                    return;
                _geometry = value;
                if (_labelLocationService != null)
                    _labelLocationService.Update(_geometry);
            }
        }

        public string[] FieldNames
        {
            get { return _fieldNames; }
        }

        internal void SetFieldAndValues(string[] fields,string[] values)
        {
            _fieldNames = fields;
            _fieldValues = values;
        }

        public string[] FieldValues
        {
            get { return _fieldValues; }
        }

        public void ResetDisplayLevel()
        {
            _isGetLevel = false;
            _level = -1;
        }

        private int _level = -1;
        private bool _isGetLevel = false;
        public int DisplayLevel
        {
            get
            {
                if (_isGetLevel)
                    return _level;
                else
                {
                    string v = GetFieldValue(Constants.cstLevelField);
                    if (int.TryParse(v, out _level))
                    {
                    }
                    else
                    {
                        _level = -1;
                    }
                    _isGetLevel = true;
                }
                return _level;
            }
        }

        private int _idxLevelField = -1;
        internal void SetDisplayLevel(int level)
        {
            if (_idxLevelField == -1)
            {
                for (int i = 0; i < _fieldNames.Length; i++)
                {
                    if (_fieldNames[i].ToUpper() == Constants.cstLevelField.ToUpper())
                    {
                        _idxLevelField = i;
                        break;
                    }
                }
            }
            if(_idxLevelField>=0)
                _fieldValues[_idxLevelField] = level.ToString();
            ResetDisplayLevel();
        }

        public int IndexOfField(string field)
        {
            for (int i = 0; i < _fieldNames.Length; i++)
            {
                if (_fieldNames[i].ToUpper() == field.ToUpper())
                {
                    return i;
                }
            }
            return -1;
        }

        public string GetFieldValue(int index)
        {
            if (_fieldValues == null || index < 0 || index >= _fieldValues.Length)
                return null;
            return _fieldValues[index];
        }

        public string GetFieldValue(string fieldName)
        {
            if (_fieldNames == null || string.IsNullOrEmpty(fieldName))
                return null;
            int idx = 0;
            foreach (string f in _fieldNames)
            {
                if (f.ToUpper() == fieldName.ToUpper())
                    return GetFieldValue(idx);
                idx++;
            }
            return null;
        }

        public void SetFieldValue(string fieldName, string fieldValue)
        {
            for (int i = 0; i < _fieldNames.Length; i++)
            {
                if (_fieldNames[i].ToUpper() == fieldName.ToUpper())
                {
                    if (_fieldValues == null || _fieldValues.Length != _fieldNames.Length)
                        _fieldValues = new string[_fieldNames.Length];
                    _fieldValues[i] = fieldValue;
                    return;
                }
            }
        }

        public void SetFieldValue(int fieldIndex, string fieldValue)
        {
            if (fieldIndex > _fieldNames.Length)
                return;
            _fieldValues[fieldIndex] = fieldValue;
        }

        #region ICloneable 成员

        private static int maxOID = int.MaxValue;
        public object Clone()
        {
            string[] flds = new string[_fieldNames.Length];
            for (int i = 0; i < _fieldNames.Length; i++)
                flds[i] = _fieldNames[i];
            string[] fldValuess = new string[_fieldValues.Length];
            for (int i = 0; i < _fieldValues.Length; i++)
                fldValuess[i] = _fieldValues[i];
            //Shape geo = _geometry.Clone();
            LabelLocation[] anns = null;
            if (_annotations != null)
            {
                anns = new LabelLocation[_annotations.Length];
                for (int i = 0; i < _annotations.Length; i++)
                    anns[i] = _annotations[i].Clone() as LabelLocation;
            }
            return new Feature(maxOID--, null, flds, fldValuess,anns);
        }

        public bool IsOutsideByLongitude(double minLon, double maxLon)
        {
            Envelope evp = _geometry.Envelope;
            return evp.MinX < minLon || evp.MaxX > maxLon;
        }

        public bool IsOutsideByLatitude(double minLat, double maxLat)
        {
            Envelope evp = _geometry.Envelope;
            return evp.MinY < minLat || evp.MaxY > maxLat;
        }

        public bool IsOutsideByLatitude(double splitLat,bool isLess)
        {
            Envelope evp = _geometry.Envelope;
            if(isLess)
                return evp.MinY <= splitLat && evp.MaxY <= splitLat;
            else
                return evp.MinY >= splitLat && evp.MaxY >= splitLat;
        }

        public bool IsNeedSplitByLongitude(double splitLon)
        {
            Envelope evp = _geometry.Envelope;
            return  evp.MinX < splitLon && evp.MaxX > splitLon;
        }

        public bool IsNeedSplitByLatitude(double splitLat)
        {
            Envelope evp = _geometry.Envelope;
            return evp.MinY < splitLat && evp.MaxY > splitLat;
        }

        public void SplitByLatitude(double splitLat, out Feature lfet, out Feature rfet)
        {
            lfet = null;
            rfet = null;
            if (_geometry is ShapePoint)
            {
                ShapePoint pt = _geometry as ShapePoint;
                lfet = Clone() as Feature;
                lfet.Geometry = new ShapePoint(pt.X, pt.Y);
                rfet = null;
            }
            else if (_geometry is ShapePolyline)
            {
                ShapePolyline lLine = null, rLine = null;
                (_geometry as ShapePolyline).SplitByLatitude(splitLat, out lLine, out rLine);
                if (lLine != null)
                {
                    lfet = Clone() as Feature;
                    lfet.Geometry = lLine;
                }
                //
                if (rLine != null)
                {
                    rfet = Clone() as Feature;
                    rfet.Geometry = rLine;
                }
            }
            else if (_geometry is ShapePolygon)
            {
                ShapePolygon lPly = null, rPly = null;
                (_geometry as ShapePolygon).SplitByLatitude(splitLat, out lPly, out rPly);
                if (lPly != null)
                {
                    lfet = Clone() as Feature;
                    lfet.Geometry = lPly;
                }
                //
                if (rPly != null)
                {
                    rfet = Clone() as Feature;
                    rfet.Geometry = rPly;
                }
            }
            else
            {
                throw new NotSupportedException("矢量要素分割操作不支持请求的几何类型。");
            }
        }

        public void SplitByLongitude(double splitLon, out Feature lfet, out Feature rfet)
        {
            lfet = null;
            rfet = null;
            if (_geometry is ShapePoint)
            {
                ShapePoint pt = _geometry as ShapePoint;
                lfet = Clone() as Feature;
                lfet.Geometry = new ShapePoint(pt.X, pt.Y);
                rfet = null;
            }
            else if (_geometry is ShapePolyline)
            {
                ShapePolyline lLine = null, rLine = null;
                (_geometry as ShapePolyline).SplitByLongitude(splitLon, out lLine, out rLine);
                if (lLine != null)
                {
                    lfet = Clone() as Feature;
                    lfet.Geometry = lLine;
                }
                //
                if (rLine != null)
                {
                    rfet = Clone() as Feature;
                    rfet.Geometry = rLine;
                }
            }
            else if (_geometry is ShapePolygon)
            {
                ShapePolygon lPly = null, rPly = null;
                (_geometry as ShapePolygon).SplitByLongitude(splitLon, out lPly, out rPly);
                if (lPly != null)
                {
                    lfet = Clone() as Feature;
                    lfet.Geometry = lPly;
                }
                //
                if (rPly != null)
                {
                    rfet = Clone() as Feature;
                    rfet.Geometry = rPly;
                }
            }
            else
            {
                throw new NotSupportedException("矢量要素分割操作不支持请求的几何类型。");
            }
        }

        #endregion

        public int EstimateSize()
        {
            int estimateSize = 0;
            if (_fieldNames != null)
                foreach (string fname in _fieldNames)
                    if (fname != null)
                        estimateSize += (fname.Length * 2);
            if (_fieldValues != null)
                foreach (string fvalue in _fieldValues)
                    if (fvalue != null)
                        estimateSize += (fvalue.Length * 2);
            if(_geometry != null)
                estimateSize += _geometry.EstimateSize();
            return estimateSize;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _fieldNames = null;
            _fieldValues = null;
            if (_geometry != null)
                _geometry.Dispose();
            _featureClass = null;
        }

        #endregion

        #region ISupportOutsideIndicator Members

        [Browsable(false)]
        public IOutsideIndicator OutsideIndicator
        {
            get { return _outsideIndicator; }
        }

        #endregion

        public override string ToString()
        {
            return "{OID:"+_oid.ToString()+"}";
        }
    }
}
