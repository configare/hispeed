using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public abstract class FeatureDataSourceBase:IDisposable,IPersistable,IGridReader,IDataSource,IQueryFeatures,IFeatureDataSource
    {
        protected string _dataSourceType = string.Empty;
        [NonSerialized]
        protected EventHandler _dataSourceChanged = null;
        [NonSerialized]
        protected GridDefinition _gridDefinition = new GridDefinition();
        [NonSerialized]
        internal ArgOfLeveling _argOfLeveling = null;
        [NonSerialized]
        protected bool _isReady = false;
        protected string _name = null;
        protected GridStateIndicator _gridStateIndicator = null;
        protected Envelope _fullEnvelope = null;
        protected ISpatialReference _spatialRef = null;
        protected enumShapeType _shapeType = enumShapeType.NullShape;
        protected string[] _fields = null;
        protected enumCoordinateType _coordType = enumCoordinateType.Geographic;
        protected int _fullGridCount = 0;
        protected bool _isInited = false;
        protected int _featureCount = 0;
        protected bool _readIsFinished = false;
        protected IFeatureClass _featureClass = null;
      
        public FeatureDataSourceBase(string name)
        {
            _name = name;
            _argOfLeveling = new ArgOfLeveling();
            _argOfLeveling.Enabled = false;
        }

        internal void SetGridDefinition(GridDefinition gridDef)
        {
            _gridDefinition = gridDef;
        }

        internal void SetFeatureClass(IFeatureClass fetClass)
        {
            _featureClass = fetClass;
        }

        [Browsable(false)]
        public bool ReadIsFinished
        {
            get { return _readIsFinished; }
        }

        [Browsable(false), DisplayName("数据源类型"),Category("数据源")]
        public string DataSourceType
        {
            get { return _dataSourceType; }
        }

        [Browsable(false)]
        public EventHandler DataSourceChanged 
        {
            get { return _dataSourceChanged; }
            set { _dataSourceChanged = value; }
        }

        [Browsable(false)]
        public GridStateIndicator GridStateIndicator
        {
            get { return _gridStateIndicator; }
        }

        [Browsable(false)]
        public virtual bool IsReady
        {
            get { return _isInited; }
        }

        [DisplayName("稀疏处理"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ArgOfLeveling ArgOfLeveling
        {
            get { return _argOfLeveling; }
        }

        [Browsable(false)]
        public string Name
        {
            get { return _name; }
        }

        internal void SetArgOfLevel(ArgOfLeveling arg)
        {
            _argOfLeveling = arg;
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            _dataSourceChanged = null;
        }

        #endregion

        public  Envelope GetFullEnvelope()
        {
            if (!_isInited)
                Init();
            return _fullEnvelope;
        }

        public int GetFullGridCount()
        {
            if (!_isInited)
                Init();
            return _fullGridCount;
        }

        public enumCoordinateType GetCoordinateType()
        {
            if (!_isInited)
                Init();
            return _coordType;
        }

        public enumShapeType GetShapeType()
        {
            if (!_isInited)
                Init();
            return _shapeType;
        }

        public string[] GetFieldNames()
        {
            if (!_isInited)
                Init();
            return _fields;
        }

        public ISpatialReference GetSpatialReference()
        {
            if (!_isInited)
                Init();
            return _spatialRef;
        }

        internal void TryInit()
        {
            if (!_isInited)
                Init();
        }

        public override string ToString()
        {
            return string.Empty;
        }

        protected abstract void Init();

        #region IPersistable Members

        public abstract PersistObject ToPersistObject();

        #endregion

        #region IGridReader Members

        public virtual void BeginRead()
        {
        }

        public virtual IGrid ReadGrid(int gridNo)
        {
            throw new NotImplementedException();
        }

        public  virtual void EndRead()
        {
        }

        #endregion

        #region IQueryFeatures 成员

        public virtual Feature[] Query(QueryFilter filter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
