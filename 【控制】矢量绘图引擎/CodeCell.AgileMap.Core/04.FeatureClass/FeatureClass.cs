using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public class FeatureClass:IFeatureClass,IPersistable,IIdentifyFeatures
    {
        protected string _name = null;
        protected GridDefinition _gridDefinition = null;
        protected int _fullGridCount = 0;
        protected List<IGrid> _grids = null;
        protected Envelope _fullEnvelope = null;
        protected Envelope _envelope = null;
        protected ISpatialReference _spatialReference = null;
        protected enumCoordinateType _originalCoordType = enumCoordinateType.Geographic;
        protected enumCoordinateType _coordType = enumCoordinateType.Geographic;
        protected enumShapeType _shapeType = enumShapeType.Point;
        protected MemoryGridLimiter _limiter = new MemoryGridLimiter();
        protected FeatureDataSourceBase _dataSource = null;
        protected bool _disposed = false;
        protected IRuntimeProjecter _currentRuntimeProjecter = null;
        protected string[] _fieldNames = null;
        protected RepeatFeatureRecorder _repeatFeatureRecorder = new RepeatFeatureRecorder();
        protected int _id = 0;
        protected static int MaxID = 0;

        public FeatureClass(string filename)
        {
            _gridDefinition = new GridDefinition();
            _limiter = new MemoryGridLimiter();
            _dataSource = new FileDataSource(Path.GetFileNameWithoutExtension(filename),filename);
            _spatialReference = _dataSource.GetSpatialReference();
            InitConstructor();
        }

        public FeatureClass(FeatureDataSourceBase dataSource)
        {
            _gridDefinition = new GridDefinition();
            _limiter = new MemoryGridLimiter();
            _dataSource = dataSource;
            _spatialReference = _dataSource.GetSpatialReference();
            InitConstructor();
        }

        public FeatureClass(GridDefinition gridDef,MemoryGridLimiter limiter,FeatureDataSourceBase dataSource)
        {
            if (gridDef != null)
                _gridDefinition = gridDef;
            if (limiter != null)
                _limiter = limiter;
            if (dataSource == null)
                throw new ArgumentNullException("构造VectorFeatureClass对象,\"DataSource\"不能为空。");
            _dataSource = dataSource;
            _dataSource.SetFeatureClass(this);
            _spatialReference = _dataSource.GetSpatialReference();
            InitConstructor();
        }

        private void InitConstructor()
        {
            _id = MaxID;
            MaxID++;
            _dataSource.SetGridDefinition(_gridDefinition);
            _dataSource.SetFeatureClass(this);
            _dataSource.DataSourceChanged += new EventHandler(DataSourceChanged);
            GetArgsFromDataSource();
        }

        void DataSourceChanged(object sender, EventArgs e)
        {
            GetArgsFromDataSource();
            ClearMemoryCache();
        }

        private void ClearMemoryCache()
        {
            if (_grids != null && _grids.Count > 0)
            {
                foreach (IGrid grid in _grids)
                    grid.Dispose();
                _grids.Clear();
                //_grids = null;
            }
        }

        internal void GetArgsFromDataSource()
        {
            _fullEnvelope = _dataSource.GetFullEnvelope();
            _fullGridCount = _dataSource.GetFullGridCount();
            _coordType = _dataSource.GetCoordinateType();
            _shapeType = _dataSource.GetShapeType();
            _fieldNames = _dataSource.GetFieldNames();
            _originalCoordType = _coordType;
        }

        #region IFeatureClass Members

        [Browsable(false)]
        public IRuntimeProjecter RuntimeProjecter
        {
            get { return _currentRuntimeProjecter; }
        }

        [DisplayName("空间参考"), ReadOnly(true), TypeConverter(typeof(ExpandableObjectConverter))]
        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
        }

        [Browsable(false)]
        public RepeatFeatureRecorder RepeatFeatureRecorder
        {
            get { return _repeatFeatureRecorder; }
        }

        [Browsable(false)]
        public string[] FieldNames
        {
            get { return _fieldNames; }
        }

        [Browsable(false)]
        public int ID
        {
            get { return _id; }
        }

        [DisplayName("名称")]
        public string Name
        {
            get { return _name == null ? ToString() : _name; }
            set { _name = value; }
        }

        [DisplayName("网格定义"),TypeConverter(typeof(ExpandableObjectConverter))]
        public GridDefinition GridDefinition
        {
            get { return _gridDefinition; }
        }

        [DisplayName("网格总数")]
        public int FullGridCount
        {
            get { return _fullGridCount; }
        }

        [Browsable(false)]
        public IGrid[] Grids
        {
            get { return _grids != null && _grids.Count > 0 ? _grids.ToArray() : null; }
        }

        public bool IsEmpty()
        {
            return _grids == null || _grids.Count == 0 || FeatureCountIsZero();
        }

        private bool FeatureCountIsZero()
        {
            foreach (IGrid gd in _grids)
                if (!gd.IsEmpty())
                    return false;
            return true;
        }

        [DisplayName("地理范围")]
        public Envelope FullEnvelope
        {
            get { return _fullEnvelope; }
            internal set { _fullEnvelope = value; }
        }

        [DisplayName("显示地理范围"),Browsable(false)]
        public Envelope Envelope
        {
            get { return _envelope; }
        }

        [Browsable(false)]
        public enumCoordinateType CoordinateType
        {
            get { return _coordType; }
        }

        [DisplayName("坐标类型")]
        public enumCoordinateType OriginalCoordinateType
        {
            get { return _originalCoordType; }
        }

        [DisplayName("几何类型")]
        public enumShapeType ShapeType
        {
            get { return _shapeType; }
        }

        [DisplayName("数据源"), TypeConverter(typeof(ExpandableObjectConverter))]
        public IDataSource DataSource
        {
            get { return _dataSource as IDataSource; }
        }

        [DisplayName("网格缓存配置"),TypeConverter(typeof(ExpandableObjectConverter))]
        public MemoryGridLimiter GridLimiter
        {
            get { return _limiter; }
        }

        public Feature[] GetVectorFeatures()
        {
            if (_grids == null || _grids.Count == 0)
                return null;
            List<Feature> retFets = new List<Feature>();
            int gridCount = _grids.Count;
            for (int i = 0; i < gridCount; i++)
            {
                if (_grids[i].VectorFeatures == null || _grids[0].VectorFeatures.Count == 0)
                    continue;
                retFets.AddRange(_grids[i].VectorFeatures);
            }
            return retFets.Count > 0 ? retFets.ToArray() : null;
        }

        public void Remove(int[] oids)
        {
            if (_grids == null || _grids.Count == 0 || oids == null || oids.Length == 0)
                return;
            List<int> ids = new List<int>(oids);
            int gridCount = _grids.Count;
            for (int i = 0; i < gridCount; i++)
            {
                if (_grids[i].VectorFeatures == null || _grids[0].VectorFeatures.Count == 0)
                    continue;
                for (int k = ids.Count - 1; k >= 0; k--)
                {
                    var v = _grids[i].VectorFeatures.Where((f) => {return  f.OID == ids[k]; });
                    if (v != null && v.Count() > 0)
                    {
                        foreach (Feature ft in v)
                            _grids[i].VectorFeatures.Remove(ft);
                    }
                }
            }
        }

        public void Project(IRuntimeProjecter projecter, enumCoordinateType toCoordinateType)
        {
            if (_coordType == enumCoordinateType.Geographic)
            {
                _coordType = toCoordinateType;
                _currentRuntimeProjecter = projecter;
                //
                _currentRuntimeProjecter.Project(_fullEnvelope);
                //
                if (_grids == null || _grids.Count == 0)
                    return;
                int n = _grids.Count;
                for (int i = 0; i < n; i++)
                    TryProject(_grids[i]);
            }
        }

        public void TryProject(IGrid grid)
        {
            if (_currentRuntimeProjecter == null)
                return;
            //
            if (!grid.CoordIsConverted)
            {
                _currentRuntimeProjecter.Project(grid.GridEnvelope);
                grid.CoordIsConverted = true;
            }
            //
            if (grid.VectorFeatures == null || grid.VectorFeatures.Count == 0)
                return;
            Shape shape = null;
            Feature[] fets = grid.VectorFeatures.ToArray() ;
            int n = fets.Length;
            Feature fet = null;
            for(int i =0;i<n;i++)
            {
                fet = fets[i];
                //
                fet.SetFeatureClass(this);
                //
                if (fet.Projected)
                    continue;
                //annotations
                if (fet.Annotations != null)
                {
                    foreach (LabelLocation loc in fet.Annotations)
                    {
                        _currentRuntimeProjecter.Project(loc.Location);
                    }
                }
                //
                Envelope newEnvelope = new Envelope(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);
                shape = fet.Geometry;
                if (shape == null || shape.IsProjected)
                    continue;
                shape.IsProjected = true;
                shape.Envelope = newEnvelope;
                if (shape is ShapePoint)
                {
                    _currentRuntimeProjecter.Project(shape as ShapePoint);
                    UpdateEnvelopeByPoints(newEnvelope, new ShapePoint[] { shape as ShapePoint });
                    shape.UpdateCentroid();
                }
                else if (shape is ShapePolyline)
                {
                    ShapePolyline line = shape as ShapePolyline;
                    foreach (ShapeLineString part in line.Parts)
                    {
                        _currentRuntimeProjecter.Project(part.Points);
                        UpdateEnvelopeByPoints(newEnvelope, part.Points);
                    }
                    line.UpdateCentroid();
                }
                else if (shape is ShapePolygon)
                {
                    ShapePolygon ply = shape as ShapePolygon;
                    foreach (ShapeRing ring in ply.Rings)
                    {
                        _currentRuntimeProjecter.Project(ring.Points);
                        UpdateEnvelopeByPoints(newEnvelope, ring.Points);
                    }
                    ply.UpdateCentroid();
                }
                if (fet.LabelLocationService == null)
                    fet.SetLabelLocationService(new LabelLocationServiceDefault(null));
                fet.LabelLocationService.Update(fet.Geometry);
                fet.Projected = true;
            }
            (grid as Grid).UpdateEnvelope();
        }

        private void UpdateEnvelopeByPoints(Envelope newEnvelope, ShapePoint[] shapePoints)
        {
            foreach (ShapePoint pt in shapePoints)
            {
                if (pt.X < newEnvelope.MinX)
                    newEnvelope.MinX = pt.X;
                if (pt.X > newEnvelope.MaxX)
                    newEnvelope.MaxX = pt.X;
                if (pt.Y < newEnvelope.MinY)
                    newEnvelope.MinY = pt.Y;
                if (pt.Y > newEnvelope.MaxY)
                    newEnvelope.MaxY = pt.Y;
            }
        }

        public void AddGrid(IGrid grid)
        {
            if (_grids == null)
                _grids = new List<IGrid>();
            lock (_grids)
            {
                _grids.Add(grid);
                TryProject(grid);
                UpdateEnvelope();
            }
        }

        public void ExChange(int gridNo, IGrid grid)
        {
            lock (_grids)
            {
                Remove(gridNo);
                AddGrid(grid);
            }
        }

        public void Remove(int gridNo)
        {
            lock (_grids)
            {
                if (_grids == null || _grids.Count == 0)
                    return;
                for (int i = _grids.Count -1; i >= 0; i--)
                {
                    if (_grids[i].GridNo == gridNo)
                    {
                        _grids.Remove(_grids[i]);
                        break;
                    }
                }
            }
            UpdateEnvelope();
        }

        private void UpdateEnvelope()
        { 
            if (_grids == null || _grids.Count == 0)
                return;
            int n = _grids.Count;
            _envelope = null;
            for (int i = 0; i < n; i++)
            {
                if (_grids[i].Envelope == null)
                    continue;
                if (_envelope == null)
                    _envelope = _grids[i].Envelope;
                _envelope.UnionWith(_grids[i].Envelope);
            }
        }

        [Browsable(false)]
        public bool Disposed
        {
            get { return _disposed; }
        }

        [DisplayName("立即稀疏化")]
        public bool ImmidateReLeveling
        {
            get { return false; }
            set 
            {
                if (value)
                {
                    ReLeveling();
                }
            }
        }

        public void ReLeveling()
        {
            if (_grids == null || _grids.Count == 0)
                return;
            using (LevelAdjuster set = new LevelAdjuster())
            {
                int n = _grids.Count;
                for (int i = n - 1; i >= 0; i--)
                {
                    set.Features = _grids[i].VectorFeatures.ToArray();
                    set.BeginLevel = _dataSource.ArgOfLeveling.BeginLevel;
                    set.GridSize = _dataSource.ArgOfLeveling.GridSize;
                    set.Do();
                }
            }
        }

        #endregion

        #region IIdentifyFeatures 成员

        public Feature[] Identify(Shape geometry, double tolerance)
        {
            if (_grids == null || _grids.Count == 0)
                return null;
            List<Feature> retfets = new List<Feature>();
            foreach (IGrid grid in _grids)
            {
                if (!grid.GridEnvelope.IsInteractived(geometry.Envelope))
                    continue;
                Feature[] fets = grid.Identify(geometry, tolerance);
                if (fets != null)
                    retfets.AddRange(fets);
            }
            return retfets.Count > 0 ? retfets.ToArray() : null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (_grids != null)
                {
                    foreach (IGrid grid in _grids)
                        grid.Dispose();
                    _grids.Clear();
                }
                if (_dataSource != null)
                {
                    _dataSource.DataSourceChanged -= new EventHandler(DataSourceChanged);
                    _dataSource.Dispose();
                    _dataSource = null;
                } 
                _currentRuntimeProjecter = null;
                _gridDefinition = null;
                _limiter = null;
            }
            finally
            {
                _disposed = true;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Empty;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("FeatureClass");
            if (_gridDefinition != null)
                obj.AddSubNode((_gridDefinition as IPersistable).ToPersistObject());
            if (_limiter != null)
                obj.AddSubNode((_limiter as IPersistable).ToPersistObject());
            if (_dataSource != null)
                obj.AddSubNode((_dataSource as IPersistable).ToPersistObject());
            return obj;
        }

        #endregion

        public static IFeatureClass FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            GridDefinition gridDef =GridDefinition.FromXElement(ele.Element("GridDef"));
            MemoryGridLimiter limiter = MemoryGridLimiter.FromXElement(ele.Element("MemoryLimiter"));
            object ds = PersistObject.ReflectObjFromXElement(ele.Element("DataSource"));
            if (ds == null)
                return null;
            FeatureDataSourceBase dataSource = (FeatureDataSourceBase)ds;
            return new FeatureClass(gridDef, limiter, dataSource);
        }
    }
}
