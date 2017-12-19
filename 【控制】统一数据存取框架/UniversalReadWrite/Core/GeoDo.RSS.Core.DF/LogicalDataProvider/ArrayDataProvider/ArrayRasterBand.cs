using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.DF
{
    public class ArrayRasterBand<T> : IRasterBand, IBandOperator,IArrayRasterBand<T>
    {
        protected int _bandNo;
        protected IRasterDataProvider _dataProvider;
        protected AttributeManager _attManager = new AttributeManager();
        protected string _description;
        protected int _width;
        protected int _height;
        protected T[] _bandValues;

        public ArrayRasterBand(int bandNo, T[] bandValues, int xSize, int ySize, IRasterDataProvider dataProvider)
        {
            _bandNo = bandNo;
            _dataProvider = dataProvider;
            _width = xSize;
            _height = ySize;
            _bandValues = bandValues;
        }

        public T[] BandValues
        {
            get { return _bandValues; }
        }

        public int BandNo
        {
            get { return _bandNo; }
            set { _bandNo = value; }
        }

        public IRasterDataProvider RasterDataProvider
        {
            get { return _dataProvider; }
        }

        public AttributeManager Attributes
        {
            get { return _attManager; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int MeasureBits
        {
            get { return DataTypeHelper.SizeOf(DataTypeHelper.DataType2Enum(typeof(T))); }
            set { ;}
        }

        public enumDataType DataType
        {
            get { return DataTypeHelper.DataType2Enum(typeof(T)); }
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
            get { return (float)(_dataProvider.CoordEnvelope.Width / _width); }
        }

        public float ResolutionY
        {
            get { return (float)(_dataProvider.CoordEnvelope.Height / _height); }
        }

        public CoordEnvelope CoordEnvelope
        {
            get { return _dataProvider.CoordEnvelope; }
        }

        public object Stretcher
        {
            get { return TryGetDefaultStretcher(); }
            set { ;}
        }

        private object TryGetDefaultStretcher()
        {
            double minValue, maxValue;
            if (DataType == enumDataType.Byte)
            {
                minValue = 0;
                maxValue = 255;
            }
            else
            {
                this.ComputeMinMax(out minValue, out maxValue, true, null);
            }
            return RgbStretcherFactory.CreateStretcher(DataType, minValue, maxValue);
        }

        public double NoDataValue
        {
            get { return 0; }
        }

        public double DataOffset
        {
            get { return 0; }
        }

        public double DataScale
        {
            get { return 1d; }
        }

        public Project.ISpatialReference SpatialRef
        {
            get { return _dataProvider.SpatialRef; }
        }

        public ICoordTransform CoordTransform
        {
            get { return _dataProvider.CoordTransform; }
        }

        public long MinByMeasureBits
        {
            get { return DataTypeHelper.MinValue(DataType); }
        }

        public long MaxByMeasureBits
        {
            get { return DataTypeHelper.MaxValue(DataType); }
        }

        protected virtual void CheckArgumentsisValid(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xOffset < 0 || yOffset < 0 || xOffset > _width || yOffset > _height)
                throw new RequestBlockOutOfRasterException(xOffset, yOffset, xSize, ySize);
            if (buffer == IntPtr.Zero || xBufferSize == 0 || yBufferSize == 0)
                throw new BufferIsEmptyException();
        }

        public void Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            CheckArgumentsisValid(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            DirectRead(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        public void Write(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            CheckArgumentsisValid(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            DirectWrite(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        protected void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
                DirectReadBSQNormal(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            else
                DirectReadBSQSample(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        private void DirectReadBSQNormal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            int dataTypeSize = DataTypeHelper.SizeOf(DataType);
            int endRow = yOffset + ySize;
            int offset = (yOffset * _width + xOffset) * dataTypeSize;
            int rowStep = (xSize + xOffset) * dataTypeSize;
            int copySize = xSize * dataTypeSize;// == xBufferSize * dataTypeSize
            GCHandle handle = GCHandle.Alloc(_bandValues, GCHandleType.Pinned);
            try
            {
                IntPtr srcPtr = handle.AddrOfPinnedObject();
                srcPtr = IntPtr.Add(srcPtr, offset);
                for (int r = yOffset; r < endRow; r++, srcPtr = IntPtr.Add(srcPtr, rowStep), buffer = IntPtr.Add(buffer, copySize))
                {
                    WinAPI.MemoryCopy(buffer, srcPtr, copySize);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        private void DirectReadBSQSample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            float rowScale = (float)yBufferSize / ySize;
            float colScale = (float)xBufferSize / xSize;
            int endRow = yOffset + ySize;
            int endCol = xOffset + xSize;
            int dstRow = 0, dstCol = 0;
            int srcIdx = ySize * _width, dstIdx = 0;
            int dataTypeSize = DataTypeHelper.SizeOf(DataType);
            GCHandle handle = GCHandle.Alloc(_bandValues, GCHandleType.Pinned);
            IntPtr srcPtr0 = handle.AddrOfPinnedObject();
            try
            {
                IntPtr srcPtr = srcPtr0;
                IntPtr dstPrt = buffer;
                for (int srcRow = yOffset; srcRow < endRow; srcRow++)
                {
                    dstRow = (int)(srcRow * rowScale);
                    for (int srcCol = xOffset; srcCol < endCol; srcCol++)
                    {
                        srcIdx = srcRow * Width + srcCol;
                        dstCol = (int)(srcCol * colScale);
                        dstIdx = dstRow * xBufferSize + dstCol;
                        //
                        srcPtr = IntPtr.Add(srcPtr0, srcIdx * dataTypeSize);
                        dstPrt = IntPtr.Add(buffer, dstIdx * dataTypeSize);
                        WinAPI.MemoryCopy(dstPrt, srcPtr, dataTypeSize);
                    }
                }
            }
            finally 
            {
                handle.Free();
            }
        }

        protected void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        public void Fill(double noDataValue)
        {
            throw new NotImplementedException();
        }

        public void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            MaxMinValuesComputer.ComputeMinMax(this, 1, out min, out max, isCanApprox, progressCallback);
        }

        public void ComputeMinMax(double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            MaxMinValuesComputer.ComputeMinMax(this, 1, begin, end, out min, out max, isCanApprox, progressCallback);
        }

        public void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            StatValuesComputer.Stat(this, 1, out min, out max, out mean, out stddev, isCanApprox, progressCallback);
        }

        public void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            StatValuesComputer.Stat(this, 1, begin, end, out min, out max, out mean, out stddev, isCanApprox, progressCallback);
        }

        public void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback)
        {
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            StatValuesComputer.ComputeHistogram(this, buckets, interleave, begin, end, histogram, isCanApprox, progressCallback);
        }

        public int CompareTo(object obj)
        {
            return BandNo - (obj as IRasterBand).BandNo;
        }

        public void Dispose()
        {
        }
    }
}
