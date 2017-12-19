using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public class ArrayRasterDataProvider<T> : IRasterDataProvider, IArrayRasterDataProvider
    {
        protected string _name;
        protected CoordEnvelope _coordEnvelope;
        protected ISpatialReference _spatialRef;
        protected AttributeManager _attManager = new AttributeManager();
        protected int _width;
        protected int _height;
        protected DataIdentify _dataIdentify = new DataIdentify();
        protected List<ArrayRasterBand<T>> _bands = new List<ArrayRasterBand<T>>();
        protected int[] _bandNos;
        protected enumCoordType _coordType = enumCoordType.GeoCoord;
        protected IOverviewGenerator _overviewGenerator;
        protected ICoordTransform _coordTransform = null;
        protected int _maxBandNo;
        protected object _tag;

        public ArrayRasterDataProvider(string name, T[][] bandValues, int xSize, int ySize)
            :this(name,bandValues,xSize,ySize,null,null)
        { 
        }

        public ArrayRasterDataProvider(string name, T[][] bandValues, int xSize, int ySize, CoordEnvelope coordEnvelope, ISpatialReference spatialRef)
        {
            _name = name;
            _coordEnvelope = coordEnvelope;       
            _spatialRef = spatialRef;
            _width = xSize;
            _height = ySize;
            _bandNos = new int[bandValues.Length];
            for (int i = 0; i < bandValues.Length; i++)
            {
                _bands.Add(new ArrayRasterBand<T>(i + 1, bandValues[i], xSize, ySize, this));
                _bandNos[i] = i + 1;
            }
            _maxBandNo = _bandNos.Length;
            TrySetCoordTypeAndCoordEnvelope();
            TryCreateCoordTransform();
            TrySetEnvelopeAndResolutions();
        }

        private void TrySetCoordTypeAndCoordEnvelope()
        {
            if (_coordEnvelope == null)
                _coordEnvelope = new DF.CoordEnvelope(0, _width, 0, _height);
            if (_spatialRef == null)
            {
                _coordType = enumCoordType.Raster;
            }
            else
            {
                if (_spatialRef.ProjectionCoordSystem == null)
                    _coordType = enumCoordType.GeoCoord;
                else
                    _coordType = enumCoordType.PrjCoord;
            }
        }

        public void ResetStretcher()
        {
            if (_bands == null || _bands.Count == 0)
                return;
            foreach (IRasterBand band in _bands)
                band.Stretcher = null;
        }

        protected void TrySetEnvelopeAndResolutions()
        {
            double[] coord1 = new double[2];
            _coordTransform.Raster2DataCoord(0, 0, coord1);
            double[] coord2 = new double[2];
            _coordTransform.Raster2DataCoord(_height, _width, coord2);
            _coordEnvelope = new CoordEnvelope(coord1[0], coord2[0], coord2[1], coord1[1]);
        }

        private void TryCreateCoordTransform()
        {
            if (_spatialRef == null)
                _coordTransform = CoordTransoformFactory.GetCoordTransform(null, null, _width, _height);
            else
            {
                _coordTransform = CoordTransoformFactory.GetCoordTransform(
                    new Point(0, 0),
                    new Point(_width, _height),
                    new double[] { _coordEnvelope.MinX, _coordEnvelope.MaxY },
                    new double[] { _coordEnvelope.MaxX, _coordEnvelope.MinY });
            }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public AttributeManager Attributes
        {
            get { return _attManager; }
        }

        public enumDataType DataType
        {
            get { return DataTypeHelper.DataType2Enum(typeof(T)); }
        }

        public int BandCount
        {
            get { return _bands.Count; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public float ResolutionX
        {
            get { return (float)(CoordEnvelope.Width / _width); }
        }

        public float ResolutionY
        {
            get { return (float)(CoordEnvelope.Height / _height); }
        }

        public CoordEnvelope CoordEnvelope
        {
            get { return _coordEnvelope; }
        }

        public ICoordTransform CoordTransform
        {
            get { throw new NotImplementedException(); }
        }

        public string[] GetFileList()
        {
            return null;
        }

        public DataIdentify DataIdentify
        {
            get { return _dataIdentify; }
        }

        public object GetStretcher(int bandNo)
        {
            if (bandNo < 1 || bandNo > _bandNos.Length)
                throw new BandIndexOutOfRangeException(BandCount, bandNo);
            return _bands[bandNo - 1].Stretcher;
        }

        public int[] GetDefaultBands()
        {
            return _bandNos;
        }

        public IRasterBand GetRasterBand(int bandNo)
        {
            if(bandNo<1 || bandNo > _bandNos.Length)
                throw new BandIndexOutOfRangeException(BandCount,bandNo);
            return _bands[bandNo - 1];
        }

        public IBandProvider BandProvider
        {
            get { return null; }
        }

        public void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public void Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, enumInterleave interleave)
        {
            CheckArgumentsisValid(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, bandCount, bandMap, interleave);
            IntPtr ptr0 = buffer;
            ReadBandsToBSQ(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, bandCount, bandMap, ptr0);
        }

        private void ReadBandsToBSQ(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, IntPtr ptr0)
        {
            for (int i = 0; i < bandCount; i++, buffer = IntPtr.Add(buffer, xBufferSize * yBufferSize * 2))
            {
                _bands[bandMap[i] - 1].Read(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            }
        }

        public void Write(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, enumInterleave interleave)
        {
            throw new NotImplementedException();
        }

        private void CheckArgumentsisValid(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, enumInterleave interleave)
        {
            if (xOffset < 0 || yOffset < 0 || xOffset >= _width || yOffset >= _height)
                throw new RequestBlockOutOfRasterException(xOffset, yOffset, xSize, ySize);
            if (buffer == IntPtr.Zero || xBufferSize == 0 || yBufferSize == 0)
                throw new BufferIsEmptyException();
            if (xOffset + xSize > _width || yOffset + ySize > _height)
                throw new Exception("Access window out of range in RasterIO()!");
            if (bandCount == 0)
                return;
            if (bandMap == null || bandMap.Length == 0)
            {
                bandMap = new int[bandCount];
                for (int i = 0; i < bandCount; i++)
                    bandMap[i] = i + 1;
            }
            foreach (int b in bandMap)
                if (b < 1 || b > _bandNos.Length)
                    throw new BandIndexOutOfRangeException(_bandNos.Length, b);
        }

        public IOrbitProjectionTransformControl OrbitProjectionTransformControl
        {
            get { return null; }
        }

        public string fileName
        {
            get { return _name; }
        }

        public IGeoDataDriver Driver
        {
            get { return null; }
        }

        public enumCoordType CoordType
        {
            get { return _coordType; }
        }

        public Project.ISpatialReference SpatialRef
        {
            get { return _spatialRef; }
        }

        void IOverviewGenerator.Generate(int[] bandNos, ref Bitmap bitmap)
        {
            TryCreateOverviewGenerator();
            if (_overviewGenerator != null)
                _overviewGenerator.Generate(bandNos, ref bitmap);
        }

        void IOverviewGenerator.Generate(int[] bandNos, object[] stretchers, ref Bitmap bitmap)
        {
            TryCreateOverviewGenerator();
            if (_overviewGenerator != null)
                _overviewGenerator.Generate(bandNos, stretchers, ref bitmap);
        }

        Size IOverviewGenerator.ComputeSize(int maxSize)
        {
            TryCreateOverviewGenerator();
            if (_overviewGenerator != null)
                return _overviewGenerator.ComputeSize(maxSize);
            return Size.Empty;
        }

        private void TryCreateOverviewGenerator()
        {
            switch (DataType)
            {
                case enumDataType.Byte:
                    _overviewGenerator = new OverviewGenerator<byte>(this);
                    break;
                case enumDataType.UInt16:
                    _overviewGenerator = new OverviewGenerator<UInt16>(this);
                    break;
                case enumDataType.Int16:
                    _overviewGenerator = new OverviewGenerator<Int16>(this);
                    break;
                case enumDataType.Int32:
                    _overviewGenerator = new OverviewGenerator<Int32>(this);
                    break;
                case enumDataType.UInt32:
                    _overviewGenerator = new OverviewGenerator<UInt32>(this);
                    break;
                case enumDataType.Int64:
                    _overviewGenerator = new OverviewGenerator<Int64>(this);
                    break;
                case enumDataType.UInt64:
                    _overviewGenerator = new OverviewGenerator<UInt64>(this);
                    break;
                case enumDataType.Float:
                    _overviewGenerator = new OverviewGenerator<float>(this);
                    break;
                case enumDataType.Double:
                    _overviewGenerator = new OverviewGenerator<double>(this);
                    break;
            }
        }

        public void Dispose()
        {
        }


        public void BuildOverviews(int[] levels, Action<int, string> progressTracker)
        {
            throw new NotImplementedException();
        }

        public void BuildOverviews(Action<int, string> progressTracker)
        {
            throw new NotImplementedException();
        }

        public bool IsSupprtOverviews
        {
            get { return false; }
        }

        public bool IsOverviewsBuilded
        {
            get { return false; }
        }
    }
}
