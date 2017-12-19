using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;
using System.Data;
using System.IO;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class SpatialDbDataSource : FeatureDataSourceBase,IGridReader,ISpatialDbDataSource,IQueryFeatures
    {
        //MySql$Database=fdc;Data Source=localhost;User Id=root;Password=106087;CharSet=utf8@国家
        protected string _connectionString = null;
        protected string _dbConnectionString = null;
        protected IVectorFeatureSpatialDbReader _reader = null;
        protected bool _allowReadAllAtFirst = false;
 
        public SpatialDbDataSource(string name)
            : base(name)
        {
            _dataSourceType = "空间数据库";
        }

        public SpatialDbDataSource(string name,string connectionString)
            : this(name)
        {
            _connectionString = connectionString;
        }

        [DisplayName("空间数据库连接参数"), Category("数据源")]
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                if (_dataSourceChanged != null)
                    _dataSourceChanged(this, null);
            }
        }

        [DisplayName("第一次读取时读出所有要素")]
        public bool AllowReadAllAtFirst
        {
            get { return _allowReadAllAtFirst; }
            set { _allowReadAllAtFirst = value; }
        }

        protected override void Init()
        {
            if (_reader == null)
                BeginRead();
            if (_reader != null)
            {
                _spatialRef = _reader.SpatialReference;
                _coordType = _spatialRef != null && _spatialRef.ProjectionCoordSystem != null ? enumCoordinateType.Projection : enumCoordinateType.Geographic;
                _fields = _reader.Fields;
                _fullEnvelope = _reader.Envelope.Clone() as Envelope; //这个非常重要
                _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
                _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;
                _shapeType = _reader.ShapeType;
                _isInited = true;
            }
        }

        [Browsable(false)]
        public override bool IsReady
        {
            get
            {
                return _reader != null;
            }
        }

        #region IGridReader Members

        public override void BeginRead()
        {
            _readIsFinished = false;
            string[] parts = _connectionString.Split('@');
            _dbConnectionString = parts[0];
            try
            {
                using (IDbConnection conn = DbConnectionFactory.CreateDbConnection(parts[0]))
                {
                    if (conn == null)//数据库连接已经关闭或链接池没有可用连接
                        return;
                    conn.Open();
                    _reader = VectorFeatureSpatialDbReaderFactory.GetVectorFeatureSpatialDbReader(conn,
                                                                                                  parts[1],
                                                                                                  _allowReadAllAtFirst,
                                                                                                  _argOfLeveling);
                }
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
            }
        }

        public override IGrid ReadGrid(int gridNo)
        {
            try
            {
                using (IDbConnection conn = DbConnectionFactory.CreateDbConnection(_dbConnectionString))
                {
                    if (conn == null)//数据库连接已经关闭或链接池没有可用连接
                        return null;
                    conn.Open();
                    Envelope evp = _gridStateIndicator.GetEnvelope(gridNo);
                    Feature[] fets = _reader.Read(evp, conn);
                    if (fets != null && fets.Length > 0)
                    {
                        foreach (Feature f in fets)
                            f.SetFeatureClass(_featureClass);
                        return new Grid(gridNo, evp, fets);
                    }
                    return new Grid(gridNo, evp, null);
                }
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        public override void EndRead()
        {
            try
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }
            }
            finally
            {
                _readIsFinished = true;
                _isReady = false;
            }
        }

        #endregion

        public override Feature[] Query(QueryFilter filter)
        {
            if (filter == null || (filter.Geometry == null && string.IsNullOrEmpty(filter.WhereClause)))
                return null;
            if (filter.Geometry != null && string.IsNullOrEmpty(filter.WhereClause))
            {
                return QueryByGeometry(filter.Geometry, filter.Relation);
            }
            else if (filter.Geometry == null && !string.IsNullOrEmpty(filter.WhereClause))
            {
                return QueryByWhereClause(filter.WhereClause);
            }
            else
            {
                return QueryByComplex(filter.Geometry, filter.Relation, filter.WhereClause);
            }
        }

        private Feature[] QueryByComplex(Shape shape, enumSpatialRelation enumSpatialRelation, string whereClause)
        {
            if (_reader == null)
                BeginRead();
            using (IDbConnection conn = DbConnectionFactory.CreateDbConnection(_dbConnectionString))
            {
                if (conn == null)//数据库连接已经关闭或链接池没有可用连接
                    return null;
                conn.Open();
                //
                Feature[] fets = _reader.Read(shape.Envelope, whereClause, conn);
                if (fets != null)
                    foreach (Feature f in fets)
                        f.SetFeatureClass(_featureClass);
                return fets;
            }
        }

        private Feature[] QueryByWhereClause(string whereClause)
        {
            if (_reader == null)
                BeginRead();
            if (_reader == null)
                return null;
            using (IDbConnection conn = DbConnectionFactory.CreateDbConnection(_dbConnectionString))
            {
                if (conn == null)//数据库连接已经关闭或链接池没有可用连接
                    return null;
                conn.Open();
                //
                 Feature[] fets = _reader.Read(null, whereClause, conn);
                if (fets != null)
                    foreach (Feature f in fets)
                        f.SetFeatureClass(_featureClass);
                return fets;
            }
        }

        private Feature[] QueryByGeometry(Shape shape, enumSpatialRelation enumSpatialRelation)
        {
            if (_reader == null)
                BeginRead();
            if (_reader == null)
                return null;
            using (IDbConnection conn = DbConnectionFactory.CreateDbConnection(_dbConnectionString))
            {
                if (conn == null)//数据库连接已经关闭或链接池没有可用连接
                    return null;
                conn.Open();
                //
                 Feature[] fets = _reader.Read(shape.Envelope, null, conn);
                if (fets != null)
                    foreach (Feature f in fets)
                        f.SetFeatureClass(_featureClass);
                return fets;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("DataSource");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("name", _name != null ? _name : string.Empty);
            obj.AddAttribute("connectionstring", _connectionString != null ? _connectionString : string.Empty);
            if (_argOfLeveling != null)
                obj.AddSubNode(_argOfLeveling.ToPersistObject());
            return obj;
        }

        public static SpatialDbDataSource FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string name = null;
            if (ele.Attribute("name") != null)
                name = ele.Attribute("name").Value;
            SpatialDbDataSource ds = new SpatialDbDataSource(name, ele.Attribute("connectionstring").Value);
            if (ele.Element("ArgsOfLeveling") != null)
                ds.SetArgOfLevel(ArgOfLeveling.FromXElement(ele.Element("ArgsOfLeveling")));
            return ds;
        }
    }
}
