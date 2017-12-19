using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class RasterClass : IRasterClass, IPersistable
    {
        private bool _isDisposed = false;
        private IRasterDataSource _dataSource = null;
        private static int MaxID = 0;
        private int _id = 0;
        private Envelope _fullEnvelop = null;
        private enumCoordinateType _toCoordinateType = enumCoordinateType.Geographic;
        private IRuntimeProjecter _runtimeProjecter = null;
        private string _name = null;

        public RasterClass(string url)
        {
            _dataSource = new RasterDataSource(url);
            Init();
        }

        public RasterClass(IRasterDataSource datasource)
        {
           _dataSource = datasource;
            Init();
        }

        private void Init()
        {
            _id = MaxID++;
            _fullEnvelop = _dataSource.GetFullEnvelope().Clone() as Envelope;
            _name = _dataSource.Name;
        }

        public bool IsEmpty()
        {
            return false;
        }

        public int ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value;  }
        }

        public Envelope FullEnvelope
        {
            get { return _fullEnvelop; }
        }

        public ISpatialReference SpatialReference
        {
            get { return _dataSource.GetSpatialReference(); }
        }

        public enumCoordinateType CoordinateType
        {
            get { return _toCoordinateType; }
        }

        public enumCoordinateType OriginalCoordinateType
        {
            get { return _dataSource.GetCoordinateType(); }
        }

        public IDataSource DataSource
        {
            get { return _dataSource; }
        }

        public void Project(IRuntimeProjecter projecter, enumCoordinateType toCoordinateType)
        {
            _runtimeProjecter = projecter;
            if (_toCoordinateType == enumCoordinateType.Geographic)
            {
                _runtimeProjecter.Project(_fullEnvelop);
                _toCoordinateType = enumCoordinateType.Projection;
            }
        }

        public bool Disposed
        {
            get { return _isDisposed; }
        }

        public void Dispose()
        {
            _isDisposed = true;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("RasterClass");
            if (_dataSource != null)
                obj.AddSubNode((_dataSource as IPersistable).ToPersistObject());
            return obj;
        }

        #endregion

        public static IRasterClass FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            object ds = PersistObject.ReflectObjFromXElement(ele.Element("DataSource"));
            IDataSource dataSource = (IDataSource)ds;
            return new RasterClass(dataSource as IRasterDataSource);
        }
    }
}
