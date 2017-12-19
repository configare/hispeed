using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;

namespace GeoDo.RSS.Core.DF
{
    public abstract class RasterBand : IRasterBand,IBandOperator
    {
        protected IRasterDataProvider _rasterDataProvider = null;
        protected AttributeManager _attributes = new AttributeManager();
        protected int _bandNo = 0;
        protected string _description = null;
        protected int _measureBits = 0;
        protected enumDataType _dataType = enumDataType.Unknow;
        protected int _dataTypeSize = 0;
        protected int _width = 0;
        protected int _height = 0;
        protected double _noDataValue = 0;
        protected double _dataOffset = 0;
        protected double _dataScale = 0;
        protected ISpatialReference _spatialRef = null;
        protected ICoordTransform _coordTransofrm = null;
        private long _minByMeasureBits = 0;
        private long _maxByMeasureBits = 0;
        protected float _resolutionX = 0;
        protected float _resolutionY = 0;
        protected CoordEnvelope _coordEnvelope = null;
        protected object _stretcher = null;

        public RasterBand(IRasterDataProvider rasterDataProvider)
        {
            _rasterDataProvider = rasterDataProvider;
        }

        public int BandNo
        {
            get { return _bandNo; }
            set { _bandNo = value; }
        }

        public object Stretcher
        {
            get
            {
                if (_stretcher == null)
                {
                    _stretcher = _rasterDataProvider.GetStretcher(_bandNo);
                    if (_stretcher == null || !StretcherIsFitDataType())
                        _stretcher = TryGetDefaultStretcher();
                }
                return _stretcher;
            }
            set
            {
                _stretcher = value;
            }
        }

        private bool StretcherIsFitDataType()
        {
            return (_stretcher is LinearRgbStretcherByte && _dataType == enumDataType.Byte) ||
                (_stretcher is LinearRgbStretcherInt16 && _dataType == enumDataType.Int16) ||
                (_stretcher is LinearRgbStretcherUInt16 && _dataType == enumDataType.UInt16) ||
                (_stretcher is LinearRgbStretcherDouble && _dataType == enumDataType.Double) ||
                (_stretcher is LinearRgbStretcherFloat && _dataType == enumDataType.Float) ||
                (_stretcher is LinearRgbStretcherInt32 && _dataType == enumDataType.Int32) ||
                (_stretcher is LinearRgbStretcherInt64 && _dataType == enumDataType.Int64) ||
                (_stretcher is LinearRgbStretcherUInt32 && _dataType == enumDataType.UInt32) ||
                (_stretcher is LinearRgbStretcherUInt64 && _dataType == enumDataType.UInt64);
        }

        private object TryGetDefaultStretcher()
        {
            double minValue, maxValue;
            if (_dataType == enumDataType.Byte)
            {
                minValue = 0;
                maxValue = 255;
            }
            else
            {
                this.ComputeMinMax(out minValue, out maxValue, true, null);
            }
            return RgbStretcherFactory.CreateStretcher(_dataType, minValue, maxValue);
        }

        public IRasterDataProvider RasterDataProvider
        {
            get { return _rasterDataProvider; }
        }

        public AttributeManager Attributes
        {
            get { return _attributes; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int MeasureBits
        {
            get
            {
                return _measureBits;
            }
            set { _measureBits = value; }
        }

        public enumDataType DataType
        {
            get { return _dataType; }
        }

        public int DataTypeSize
        {
            get
            {
                if (_dataTypeSize == 0)
                    _dataTypeSize = DataTypeHelper.SizeOf(_dataType);
                return _dataTypeSize;
            }
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
            get { return _resolutionX; }
        }

        public float ResolutionY
        {
            get { return _resolutionY; }
        }

        public CoordEnvelope CoordEnvelope
        {
            get { return _coordEnvelope; }
        }

        public double NoDataValue
        {
            get { return _noDataValue; }
        }

        public double DataOffset
        {
            get { return _dataOffset; }
        }

        public double DataScale
        {
            get { return _dataScale; }
        }

        public ISpatialReference SpatialRef
        {
            get { return _spatialRef; }
        }

        public ICoordTransform CoordTransform
        {
            get { return _coordTransofrm; }
        }

        public long MinByMeasureBits
        {
            get { return _minByMeasureBits; }
        }

        public long MaxByMeasureBits
        {
            get { return _maxByMeasureBits; }
        }

        protected void ComputeMinMaxByMeasureBits()
        {
            if (_measureBits == 0)
                _minByMeasureBits = _maxByMeasureBits = 0;
            else
            {
                _minByMeasureBits = 0;
                _maxByMeasureBits = 2 >> _measureBits;
            }
        }

        protected virtual void CheckArgumentsisValid(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xOffset < 0 || yOffset < 0 || xOffset + xSize > _width || yOffset + ySize > _height)
                throw new RequestBlockOutOfRasterException(xOffset, yOffset, xSize, ySize);
            if (buffer == IntPtr.Zero || xBufferSize == 0 || yBufferSize == 0)
                throw new BufferIsEmptyException();
        }

        public void Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            CheckArgumentsisValid(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            DirectRead(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        protected abstract void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize);

        public void Write(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            CheckArgumentsisValid(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            DirectWrite(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        protected abstract void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize);

        public abstract void Fill(double noDataValue);

        public abstract void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback);

        public abstract void ComputeMinMax(double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback);

        public abstract void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback);

        public abstract void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback);

        public abstract void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback);

        public virtual void Dispose()
        {
            _rasterDataProvider = null;
            if (_attributes != null)
            {
                _attributes.Clear();
                _attributes = null;
            }
            if (_spatialRef != null)
                _spatialRef = null;
            if (_coordTransofrm != null)
            {
                _coordTransofrm.Dispose();
                _coordTransofrm = null;
            }
        }

        public int CompareTo(object obj)
        {
            return BandNo - (obj as IRasterBand).BandNo;
        }

        int IBandOperator.Width
        {
            get { return _width; }
        }

        int IBandOperator.Height
        {
            get { return _height; }
        }

        enumDataType IBandOperator.DataType
        {
            get { return _dataType; }
        }

        void IBandOperator.Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            (this as IRasterBand).Read(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }
    }
}
