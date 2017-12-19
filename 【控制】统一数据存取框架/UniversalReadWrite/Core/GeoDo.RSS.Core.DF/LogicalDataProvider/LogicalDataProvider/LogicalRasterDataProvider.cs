using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public class LogicalRasterDataProvider : IRasterDataProvider, ILogicalRasterDataProvider
    {
        private List<IRasterDataProvider> _needDisposeObjects = new List<IRasterDataProvider>();
        private List<IRasterBand> _rasterBands = new List<IRasterBand>();
        private AttributeManager _attManager = new AttributeManager();
        private int _width;
        private int _height;
        private enumCoordType _coordType;
        private CoordEnvelope _coordEnvelope;
        private enumDataType _dataType;
        private ISpatialReference _spatialRef;
        private ICoordTransform _coordTransform;
        private string[] _fileNames;
        private DataIdentify _dataIdentify = new DataIdentify();
        private IOverviewGenerator _overviewGenerator;
        private object _tag;

        public LogicalRasterDataProvider(string fileName,IRasterBand[] rasterBands,object tag)
        {
            if (rasterBands == null || rasterBands.Length == 0)
                throw new ArgumentNullException("rasterBands");
            _tag = tag;
            _fileNames = new string[] { fileName };            
            List<IRasterBand> bs = new List<IRasterBand>();
            bool isFirst = true;
            foreach (IRasterBand band in rasterBands)
            {
                if (isFirst)
                {
                    _dataType = band.DataType;
                    _width = band.Width;
                    _height = band.Height;
                    _coordEnvelope = band.CoordEnvelope;
                    _coordType = band.RasterDataProvider.CoordType;
                    _spatialRef = band.SpatialRef;
                    _coordTransform = band.CoordTransform;
                    isFirst = false;
                }
                else
                {
                    if (_dataType != band.DataType ||                        
                        _width != band.Width ||
                        _height != band.Height)
                        continue;
                    if (_coordType != null && _coordType != band.RasterDataProvider.CoordType)
                        continue;
                    if (_coordEnvelope != null && !CoordEnvelopeEquals(_coordEnvelope, band.CoordEnvelope))
                        continue;
                    if (_spatialRef != null && !_spatialRef.IsSame(band.SpatialRef))
                        continue;                       
                }
                bs.Add(band);
            }
            _rasterBands.AddRange(bs);
        }

        public LogicalRasterDataProvider(string fileName,string[] fnames,object tag)
        {
            if (fnames == null || fnames.Length == 0)
                throw new ArgumentNullException("fnames");
            _tag = tag;
            _fileNames = new string[fnames.Length + 1];
            _fileNames[0] = fileName;
            int i = 1;
            bool isFirst = true;
            foreach (string fname in fnames)
            {
                _fileNames[i++] = fname;
                IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (prd == null)
                    continue;
                if (isFirst)
                {
                    _dataType = prd.DataType;
                    _width = prd.Width;
                    _height = prd.Height;
                    _coordEnvelope = prd.CoordEnvelope;
                    _coordType = prd.CoordType;
                    _spatialRef = prd.SpatialRef;
                    _coordTransform = prd.CoordTransform;
                    isFirst = false;
                }
                else
                {
                    if (_dataType != prd.DataType ||
                        _coordType != prd.CoordType ||
                        !CoordEnvelopeEquals(_coordEnvelope, prd.CoordEnvelope) ||
                        _coordType != prd.CoordType ||
                        _width != prd.Width ||
                        _height != prd.Height ||
                        !_spatialRef.IsSame(prd.SpatialRef)
                        )
                        continue;
                }
                for (int b = 0; b < prd.BandCount; b++)
                {
                    IRasterBand band = prd.GetRasterBand(b + 1);
                    band.Description = prd.fileName;
                    _rasterBands.Add(band);
                    band.BandNo = _rasterBands.Count;
                }
                _needDisposeObjects.Add(prd);
            }
        }

        public void ResetStretcher()
        {
            if (_rasterBands == null || _rasterBands.Count == 0)
                return;
            foreach (IRasterBand band in _rasterBands)
                band.Stretcher = null;
        }

        private bool CoordEnvelopeEquals(DF.CoordEnvelope a, DF.CoordEnvelope b)
        {
            if (a == null || b == null)
                return false;
            return Math.Abs(a.MinX - b.MinX) < double.Epsilon &&
                Math.Abs(a.MinY - b.MinY) < double.Epsilon &&
                Math.Abs(a.MaxX - b.MaxX) < double.Epsilon &&
                Math.Abs(a.MaxY - b.MaxY) < double.Epsilon;
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
            get { return _dataType; }
        }

        public int BandCount
        {
            get { return _rasterBands.Count; }
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
            get { return (float)(_coordEnvelope.Width / _width); }
        }

        public float ResolutionY
        {
            get { return (float)(_coordEnvelope.Height / _height); }
        }

        public CoordEnvelope CoordEnvelope
        {
            get { return _coordEnvelope; }
        }

        public ICoordTransform CoordTransform
        {
            get { return _coordTransform; }
        }

        public string[] GetFileList()
        {
            return _fileNames;
        }

        public DataIdentify DataIdentify
        {
            get { return new DataIdentify(); }
        }

        public object GetStretcher(int bandNo)
        {
            if (bandNo < 1 || bandNo > _rasterBands.Count)
                return null;
            return _rasterBands[bandNo - 1].Stretcher;
        }

        public int[] GetDefaultBands()
        {
            if (_rasterBands.Count == 0)
                return null;
            if (_rasterBands.Count > 2)
                return new int[] { 1, 2, 3 };
            else
                return new int[] { 1 };
        }

        public IRasterBand GetRasterBand(int bandNo)
        {
            if (bandNo < 1 || bandNo > _rasterBands.Count)
                return null;
            return _rasterBands[bandNo - 1];
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
            throw new NotImplementedException();
        }

        public void Write(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, enumInterleave interleave)
        {
            throw new NotImplementedException();
        }

        public IOrbitProjectionTransformControl OrbitProjectionTransformControl
        {
            get { return null; }
        }

        public string fileName
        {
            get { return _fileNames != null && _fileNames.Length > 0 ? _fileNames[0] : string.Empty; }
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
            if (_needDisposeObjects.Count > 0)
            {
                foreach (IRasterDataProvider prd in _needDisposeObjects)
                    prd.Dispose();
                _needDisposeObjects.Clear();
            }
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
