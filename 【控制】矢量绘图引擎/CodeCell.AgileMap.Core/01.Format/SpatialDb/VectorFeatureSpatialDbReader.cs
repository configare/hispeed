using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.Common;

namespace CodeCell.AgileMap.Core
{
    public class VectorFeatureSpatialDbReader:IVectorFeatureSpatialDbReader
    {
        protected bool _allowReadAllAtFirst = false;
        protected bool _isLoadedAllFeaturesAtMemory = false;
        protected Feature[] _allFeatures = null;
        protected Envelope _envelope = null;
        protected ISpatialReference _spatialRef = null;
        protected enumShapeType _shapeType = enumShapeType.NullShape;
        protected int _fetCount = 0;
        protected ArgOfLeveling _argOfLeveling = null;
        protected IDbConnection _dbConnection = null;
        protected string _fetclassName = null;
        protected string _datatable = null;
        protected string _annotable = null;
        protected string _id = null;
        protected DiffDbAdapter _adapter = null;
        protected Dictionary<string, Type> _dataTableFields = null;
        protected string _dataTableFieldsSQL = null;
        protected string _shapeField = null;
        protected string _oidField = null;
        protected string _fetSqlFormat = null;
        protected string _annoSqlFormat = null;
        private IDbConnection dbConn;
        private string _datatable_2;

        public VectorFeatureSpatialDbReader(IDbConnection dbConnection,string fetclassName)
        {
            _dbConnection = dbConnection;
            _fetclassName = fetclassName;
            _adapter = DiffDbAdapterFactory.GetDiffDbAdapter(dbConnection);
            ReadBaicProperties();
            ReadDataTableFields();
        }

        public VectorFeatureSpatialDbReader(IDbConnection dbConnection, string fetclassName, bool allowReadAllAtFirst, ArgOfLeveling arg)
            : this(dbConnection, fetclassName)
        {
            _allowReadAllAtFirst = allowReadAllAtFirst;
            _argOfLeveling = arg;
            if (allowReadAllAtFirst)
                ReadAllReaturesIntoMemory();
        }

        private void ReadBaicProperties()
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();
            string sql = "select id,datatable,annotable,"+_adapter.SQLToWktForShapeField("envelope")+",featurecount,shapetype from " + BudGISMetadata.cstFeatureClassTableName +
                             " t where datatable = '" + _fetclassName+"'";
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = sql;
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        _id = _adapter.DrToString(dr, 0);
                        _datatable = _adapter.DrToString(dr, 1);
                        _annotable = _adapter.DrToString(dr, 2);
                        //
                        ShapePolygon ply = _adapter.DrToShape(dr, 3) as ShapePolygon;
                        if (ply != null)
                        {
                            ShapeRing ring = ply.Rings[0];
                            _envelope = new Envelope(ring.Points[0].X, ring.Points[2].Y, ring.Points[1].X, ring.Points[0].Y);
                        }
                        //
                        _fetCount = _adapter.DrToInt32(dr, 4);
                        //
                        string shapeTypeStr = _adapter.DrToString(dr, 5);
                        foreach (enumShapeType st in Enum.GetValues(typeof(enumShapeType)))
                        {
                            if (st.ToString() == shapeTypeStr.ToString())
                            {
                                _shapeType = st;
                                break;
                            }
                        }
                        //
                    }
                    else
                    {
                        throw new Exception("要素类\""+_fetclassName+"\"在指定的空间数据库中不存在。");
                    }
                }
            }
        }

        private void ReadDataTableFields()
        {
            _dataTableFields = _adapter.GetFieldsOfTable(_dbConnection, _datatable, out _shapeField, out _oidField);
            if (_shapeField == null || _oidField == null)
                throw new Exception("要素类表\"" + _datatable+"\"缺少几何形状字段或者要素OID字段。");
            _dataTableFieldsSQL = _oidField + "," + _adapter.SQLToWktForShapeField(_shapeField);
            if (_dataTableFields != null && _dataTableFields.Count > 0)
            {
                foreach (string fld in _dataTableFields.Keys)
                    _dataTableFieldsSQL += ("," + fld);
            }
            //
            _fetSqlFormat = "select " + _dataTableFieldsSQL + " from " + _datatable + " t ";
            if (!string.IsNullOrEmpty(_annotable))
                _annoSqlFormat = "select OID,ANGLE," + _adapter.SQLToWktForShapeField("SHAPE") + " from  " + _annotable+" t ";
        }

        #region IVectorFeatureSpatialDbReader Members

        public string[] Fields
        {
            get { return _dataTableFields.Keys.ToArray(); }
        }

        public string Name
        {
            get { return _fetclassName; }
        }

        public string Id
        {
            get { return _id; }
        }

        public bool AllowReadAllAtFirst
        {
            get { return _allowReadAllAtFirst; }
            set 
            {
                _allowReadAllAtFirst = value;
                if (_allowReadAllAtFirst)
                {
                    if (!_isLoadedAllFeaturesAtMemory)
                    {
                        ReadAllReaturesIntoMemory();
                    }
                }
                else 
                {
                    _isLoadedAllFeaturesAtMemory = false;
                    _allFeatures = null;
                }
            }
        }

        public Envelope Envelope
        {
            get { return _envelope; }
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialRef; }
        }

        public int FeaureCount
        {
            get { return _fetCount; }
        }

        public enumShapeType ShapeType
        {
            get { return _shapeType; }
        }

        private void ReadAllReaturesIntoMemory()
        {
            _allFeatures = Read(null);
            _isLoadedAllFeaturesAtMemory = true;
        }

        public Feature[] Read(Envelope envelope)
        {
            return Read(envelope, _dbConnection);
        }

        public Feature[] Read(Envelope envelope, string whereClause, IDbConnection dbConnection)
        {
            string where = GetWhere(whereClause);
            string fetsql = null, annsql = null;
            GetSqlString(envelope, out fetsql, out annsql);
            if (string.IsNullOrEmpty(fetsql) && string.IsNullOrEmpty(where))
                return null;
            if (fetsql.Contains(" where "))
                fetsql = fetsql + " and " + where;
            else
                fetsql = fetsql + " where " + where;
            List<Feature> features = new List<Feature>();
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = fetsql;
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    int oid = -1;
                    string[] fieldValues = null;
                    if (_dataTableFields != null && _dataTableFields.Count > 0)
                        fieldValues = new string[_dataTableFields.Count];
                    //
                    while (dr.Read())
                    {
                        //read oid and shape
                        oid = _adapter.DrToInt32(dr, 0);
                        Shape shape = _adapter.DrToShape(dr, 1);
                        //read other field values
                        GetFieldValues(dr, ref fieldValues);
                        Feature fet = new Feature(oid, shape, _dataTableFields.Keys.ToArray(), fieldValues.Clone() as string[], null);
                        features.Add(fet);
                    }
                }
            }
            return features.Count >0 ? features.ToArray() : null;
        }

        private string GetWhere(string whereClause)
        {
            if (string.IsNullOrEmpty(whereClause))
                return string.Empty;
            string retWhere = string.Empty;
            string[] keys = whereClause.Split(' ');
            foreach (string fld in _dataTableFields.Keys)
            {
                if (_dataTableFields[fld].Equals(typeof(string)))
                {
                    foreach (string k in keys)
                    {
                        if (string.IsNullOrEmpty(k))
                            continue;
                        retWhere += (fld + " LIKE '%" + k + "%' or ");
                    }
                }
            }
            if (retWhere.EndsWith("or "))
                retWhere = retWhere.Substring(0, retWhere.Length - 3);
            return retWhere;
        }

        public Feature[] Read(Envelope envelope, IDbConnection dbConnection)
        {
            //generate sql statement
            string fetSql = null;
            string annoSql = null;
            GetSqlString(envelope, out fetSql, out annoSql);
            bool hasAnno = !string.IsNullOrEmpty(annoSql);
            //read annotation
            Dictionary<int, List<LabelLocation>> annoLocations = null;
            if (hasAnno)
                annoLocations = ReadAnnoLocations(annoSql,dbConnection);
            //read feature
            List<Feature> features = new List<Feature>();
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = fetSql;
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    int oid = -1;
                    string[] fieldValues = null;
                    if (_dataTableFields != null && _dataTableFields.Count > 0)
                        fieldValues = new string[_dataTableFields.Count];
                    //
                    while (dr.Read())
                    {
                        //read oid and shape
                        oid = _adapter.DrToInt32(dr, 0);
                        Shape shape = _adapter.DrToShape(dr, 1);
                        //read other field values
                        GetFieldValues(dr, ref fieldValues);
                        //
                        LabelLocation[] annos = null;
                        if (hasAnno && annoLocations.ContainsKey(oid))
                            annos = annoLocations[oid].ToArray();
                        Feature fet = new Feature(oid, shape, _dataTableFields.Keys.ToArray(), fieldValues.Clone() as string[], annos);
                        //是否跨越指定的Envelope
                        if (envelope != null)
                        {
                            if (!(fet.Geometry is ShapePoint) && fet.Geometry.Envelope.IsInteractived(envelope))
                                fet.IsRepeatedOverGrids = true;
                        }
                        features.Add(fet);
                    }
                }
            }
            //Auto do Leveling
            Feature[] fets = features.Count > 0 ? features.ToArray() : null;
            if (fets != null && fets.Length > 0)
            {
                AutoLeveling(fets);
            }
            return fets;
        }

        private void GetFieldValues(IDataReader dr, ref string[] fldValues)
        {
            if (fldValues != null && fldValues.Length > 0)
            {
                  int i = 0;
                  foreach (string fldName in _dataTableFields.Keys)
                  {
                      Type fldType = _dataTableFields[fldName];
                      if (fldType.Equals(typeof(string)))
                          fldValues[i] = _adapter.DrToString(dr, i + 2); //2=OID,SHAPE
                      else if (fldType.Equals(typeof(int)))
                          fldValues[i] = _adapter.DrToInt32(dr, i + 2).ToString();
                      else
                          throw new Exception("类型\"" + fldType.ToString() + "\"的字段如何读暂没有实现。");
                      i++;
                  }
            }
        }

        private void GetSqlString(Envelope envelope, out string fetSql, out string annoSql)
        {
            if (envelope != null)
            {
                fetSql = _fetSqlFormat + " where " + _adapter.GeomeryToSqlWhere(_shapeField, envelope.ToPolygon());
                if (_annoSqlFormat != null)
                    annoSql = _annoSqlFormat + " where " + _adapter.GeomeryToSqlWhere(_shapeField, envelope.ToPolygon());
                else
                    annoSql = null;
            }
            else
            {
                fetSql = _fetSqlFormat;
                annoSql = _annoSqlFormat;
            }
        }

        private Dictionary<int, List<LabelLocation>> ReadAnnoLocations(string sqlanno,IDbConnection dbConn)
        {
            using (IDbCommand cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = sqlanno;
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    int oid = -1;
                    float angle = 0;
                    Dictionary<int,List<LabelLocation>> retLocs = new Dictionary<int,List<LabelLocation>>();
                    while (dr.Read())
                    {
                        oid = _adapter.DrToInt32(dr, 0);
                        angle = _adapter.DrToFloat(dr, 1);
                        Shape shape = _adapter.DrToShape(dr, 2);
                        LabelLocation loc = new LabelLocation(shape as ShapePoint, (int)angle);
                        //
                        if (retLocs.ContainsKey(oid))
                            retLocs[oid].Add(loc);
                        else
                            retLocs.Add(oid, new List<LabelLocation>(new LabelLocation[] { loc}));
                    }
                    return retLocs;
                }
            }
        }

        private void AutoLeveling(Feature[] features)
        {
            if (_argOfLeveling != null && _argOfLeveling.Enabled && _shapeType == enumShapeType.Point)
            {
                //int bTime = Environment.TickCount;
                using (LevelAdjuster set = new LevelAdjuster())
                {
                    set.BeginLevel = _argOfLeveling.BeginLevel;
                    set.GridSize = _argOfLeveling.GridSize;
                    set.Features = features.ToArray();
                    set.Do();
                }
                //int eTime = Environment.TickCount - bTime;
            }
        }

        public void SetArgsOfLeveling(ArgOfLeveling arg)
        {
            _argOfLeveling = arg;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _allFeatures = null;
        }

        #endregion
    }
}
