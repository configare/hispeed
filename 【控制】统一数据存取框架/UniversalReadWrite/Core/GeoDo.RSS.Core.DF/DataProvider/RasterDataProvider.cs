using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using GeoDo.Project;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public unsafe abstract class RasterDataProvider : GeoDataProvider,
        IRasterDataProvider, IOrbitProjectionTransformControl,
        IOverviewGenerator,IFileDataProvider
    {
        protected AttributeManager _attributes = new AttributeManager();
        protected enumDataType _dataType = enumDataType.Atypism;
        protected int _bandCount = 0;
        protected int _dataTypeSize = 0;
        protected int _width = 0;
        protected int _height = 0;
        protected float _resolutionX = 0;
        protected float _resolutionY = 0;
        protected CoordEnvelope _coordEnvelope = null;
        protected ICoordTransform _coordTransform = null;
        protected string[] _filelist = null;
        protected List<IRasterBand> _rasterBands = new List<IRasterBand>();
        protected IBandProvider _bandProvider = null;
        protected DataIdentify _dataIdentify = new DataIdentify();
        protected OrbitProjectionTransform _orbitProjectionTransform;
        protected IOverviewGenerator _overviewGenerator;
        protected object _tag;

        public RasterDataProvider(string fileName, IGeoDataDriver dataDriver)
            : base(fileName, dataDriver)
        {
            TryGetDataIdentity();
        }

        protected void TryGetDataIdentity()
        {
            DataIdentify id = DataIdentifyMatcher.Match(_fileName);
            if (id != null)
                id.CopyAttributesToIfNull(_dataIdentify);
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public IOrbitProjectionTransformControl OrbitProjectionTransformControl
        {
            get { return this; }
        }

        public AttributeManager Attributes
        {
            get { return _attributes; }
        }

        public enumDataType DataType
        {
            get { return _dataType; }
        }

        public int BandCount
        {
            get { return _bandCount; }
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

        public ICoordTransform CoordTransform
        {
            get { return _coordTransform; }
        }

        public string[] GetFileList()
        {
            return _filelist;
        }

        public DataIdentify DataIdentify
        {
            get { return _dataIdentify; }
        }

        /// <summary>
        /// 用这种方法获取的band，不能直接Dispose()，因为如果调用了，再次使用此band即会出错。
        /// 上层直接调用Provider的Dispose即可。
        /// </summary>
        /// <param name="bandNo"></param>
        /// <returns></returns>
        public virtual IRasterBand GetRasterBand(int bandNo)
        {
            if (_rasterBands.Count == 0)
                throw new RasterBandsIsEmptyException();
            if (bandNo < 1 || bandNo > _rasterBands.Count)
                throw new BandIndexOutOfRangeException(_rasterBands.Count, bandNo);
            return _rasterBands[bandNo - 1];
        }

        public virtual object GetStretcher(int bandNo)
        {
            if (_dataIdentify == null)
                return null;
            if (RgbStretcherFactory.IsUseAutoStretcher(_fileName))
                return null;
            if (_dataIdentify.IsProduct)
            {
                return RgbStretcherFactory.GetStretcher(_dataIdentify.ProductIdentify, _dataIdentify.SubProductIdentify);
            }
            else
            {
                return RgbStretcherFactory.GetStretcher(_dataIdentify.Satellite, _dataIdentify.Sensor, _dataIdentify.IsOrbit, bandNo);
            }
        }

        public void ResetStretcher()
        {
            if (_rasterBands == null || _rasterBands.Count == 0)
                return;
            foreach (IRasterBand band in _rasterBands)
                band.Stretcher = null;
        }

        public virtual int[] GetDefaultBands()
        {
            if (_dataIdentify == null)
                return null;
            if (_dataType == enumDataType.Byte)     //图像类型不挑选通道
                return null;
            return RgbStretcherFactory.GetDefaultBands(_dataIdentify.Satellite, _dataIdentify.Sensor, _dataIdentify.IsOrbit);
        }

        public IBandProvider BandProvider
        {
            get { return _bandProvider; }
        }

        public abstract void AddBand(enumDataType dataType);

        public virtual void Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, enumInterleave interleave)
        {
            CheckArgumentsisValid(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, bandCount, bandMap, interleave);
            IntPtr ptr0 = buffer;

            switch (interleave)
            {
                case enumInterleave.BSQ:
                    ReadBandsToBSQ(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, bandCount, bandMap, ptr0);
                    break;
                case enumInterleave.BIL:
                    ReadBandsToBIL(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, bandCount, bandMap, ptr0);
                    break;
                case enumInterleave.BIP:
                    ReadBandToBIP(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, bandCount, bandMap, ptr0);
                    break;
                default:
                    throw new InterleaveIsNotSupportException(interleave);
            }
        }

        private void ReadBandsToBSQ(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, IntPtr ptr0)
        {
            for (int i = 0; i < bandCount; i++, buffer = IntPtr.Add(buffer, xBufferSize * yBufferSize * 2))
            {
                _rasterBands[bandMap[i] - 1].Read(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            }
        }

        private unsafe void ReadBandsToBIL(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, IntPtr ptr0)
        {
            int length = xBufferSize * yBufferSize;
            int dataTypeSize = DataTypeHelper.SizeOf(dataType);
            int bufferLength = length * dataTypeSize * bandCount;
            byte[] srcBuffer = new byte[length * dataTypeSize];   //存放一个波段的数据
            int offset = bandCount * xBufferSize * dataTypeSize;  //每读一行数据指针buffer偏移量
            int bandLength = xBufferSize * dataTypeSize;  //每读一个波段ptr0偏移量
            int rowSrcOffset;

            fixed (byte* dataPtr = srcBuffer)
            {
                IntPtr srcBufferPtr = new IntPtr(dataPtr);
                for (int i = 0; i < bandCount; i++)
                {
                    _rasterBands[bandMap[i] - 1].Read(xOffset, yOffset, xSize, ySize, srcBufferPtr, dataType, xBufferSize, yBufferSize);//读一个波段的数据
                    for (int row = 0; row < yBufferSize; row++)  //读一个波段内一行数据
                    {
                        buffer = IntPtr.Add(ptr0, offset * row);     //每拷贝完一行指针偏移
                        rowSrcOffset = row * bandLength;      //源数据指针偏移
                        Marshal.Copy(srcBuffer, rowSrcOffset, buffer, bandLength);  //每次拷贝一行数据
                    }
                    ptr0 = IntPtr.Add(ptr0, bandLength);
                }
            }
        }

        private unsafe void ReadBandToBIP(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, IntPtr ptr0)
        {
            int length = xBufferSize * yBufferSize;
            int dataTypeSize = DataTypeHelper.SizeOf(dataType);
            byte[] srcBuffer = new byte[length * dataTypeSize];   //存放一个波段的数据
            int offset = xBufferSize * bandCount * dataTypeSize;
            int colOffset = dataTypeSize * bandCount;
            int srcOffset;
            int rowOffset;

            fixed (byte* dataPtr = srcBuffer)
            {
                IntPtr srcBufferPtr = new IntPtr(dataPtr);
                for (int band = 0; band < bandCount; band++)    //每次读一个波段
                {
                    _rasterBands[bandMap[band] - 1].Read(xOffset, yOffset, xSize, ySize, srcBufferPtr, dataType, xBufferSize, yBufferSize);
                    for (int row = 0; row < yBufferSize; row++)  //行循环
                    {
                        rowOffset = row * offset;
                        buffer = IntPtr.Add(ptr0, rowOffset);  //每读完一行数据的指针偏移量
                        for (int col = 0; col < xBufferSize; col++)
                        {
                            srcOffset = col * dataTypeSize + row * xBufferSize * dataTypeSize; //源数据指针偏移量
                            Marshal.Copy(srcBuffer, srcOffset, buffer, dataTypeSize);
                            buffer = IntPtr.Add(buffer, colOffset);  //每读完一列数据指针偏移量
                        }
                    }
                    ptr0 = IntPtr.Add(ptr0, dataTypeSize);  //每读完一个波段的数据首指针偏移量
                }
            }

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
                if (b < 1 || b > _bandCount)
                    throw new BandIndexOutOfRangeException(_bandCount, b);
        }

        public virtual void Write(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, enumInterleave interleave)
        {
            throw new NotImplementedException();
        }

        protected void TrySetEnvelopeAndResolutions()
        {
            double[] coord1 = new double[2];
            _coordTransform.Raster2DataCoord(0, 0, coord1);
            double[] coord2 = new double[2];
            _coordTransform.Raster2DataCoord(_height, _width, coord2);
            _coordEnvelope = new CoordEnvelope(coord1[0], coord2[0], coord2[1], coord1[1]);

            _resolutionX = (float)(_coordEnvelope.Width / (_width));
            _resolutionY = (float)(_coordEnvelope.Height / (_height));
        }

        public override void Dispose()
        {
            base.Dispose();
            _overviewGenerator = null;
            if (_rasterBands != null && _rasterBands.Count > 0)
            {
                foreach (IRasterBand band in _rasterBands)
                    band.Dispose();
                _rasterBands.Clear();
                _rasterBands = null;
            }
            this.OrbitProjectionTransformControl.Free();
        }

        unsafe void IOrbitProjectionTransformControl.BuildAsync()
        {
            if (_orbitProjectionTransform != null)
                return;
            if (_dataIdentify != null && !_dataIdentify.IsOrbit)
                return;
            EventHandler h = new EventHandler(BuildOrbitPrjTransoform);
            h.BeginInvoke(null, null, null, null);
        }

        void IOrbitProjectionTransformControl.Build()
        {
            if (_orbitProjectionTransform != null)
                return;
            if (_dataIdentify != null && !_dataIdentify.IsOrbit)
                return;
            BuildOrbitPrjTransoform(null, null);
        }

        void BuildOrbitPrjTransoform(object sender, EventArgs e)
        {
            IRasterBand lonBand, latBand;
            IRasterBand[] bs = _bandProvider.GetBands("Longitude");
            if (bs == null)
                return;
            lonBand = bs[0];
            bs = _bandProvider.GetBands("Latitude");
            if (bs == null)
                return;
            latBand = bs[0];
          
            float[] lonBuffer = null;
            float[] latBuffer = null;
            //
            switch (lonBand.DataType)
            {
                case enumDataType.Float:
                    lonBuffer = new float[lonBand.Width * lonBand.Height];
                    latBuffer = new float[latBand.Width * latBand.Height];
                    fixed (float* lonPtr = lonBuffer, latPtr = latBuffer)
                    {
                        IntPtr lonIntPtr = new IntPtr(lonPtr);
                        IntPtr latIntPtr = new IntPtr(latPtr);
                        lonBand.Read(0, 0, lonBand.Width, lonBand.Height, lonIntPtr, enumDataType.Float, lonBand.Width, latBand.Height);
                        latBand.Read(0, 0, latBand.Width, latBand.Height, latIntPtr, enumDataType.Float, latBand.Width, latBand.Height);
                    }
                    break;
                case enumDataType.Double:
                    double[] lonBufferDouble = new double[lonBand.Width * lonBand.Height];
                    double[] latBufferDouble = new double[latBand.Width * latBand.Height];
                    fixed (double* lonPtrd = lonBufferDouble, latPtrd = latBufferDouble)
                    {
                        IntPtr lonIntPtr = new IntPtr(lonPtrd);
                        IntPtr latIntPtr = new IntPtr(latPtrd);
                        lonBand.Read(0, 0, lonBand.Width, lonBand.Height, lonIntPtr, enumDataType.Double, lonBand.Width, latBand.Height);
                        latBand.Read(0, 0, latBand.Width, latBand.Height, latIntPtr, enumDataType.Double, latBand.Width, latBand.Height);
                    }
                    lonBuffer = new float[lonBand.Width * lonBand.Height];
                    latBuffer = new float[latBand.Width * latBand.Height];
                    for (int i = 0; i < lonBuffer.Length; i++)
                    {
                        lonBuffer[i] = (float)lonBufferDouble[i];
                        latBuffer[i] = (float)latBufferDouble[i];
                    }
                    break;
            }
            if (lonBuffer != null && latBuffer != null)
            {
                int lonsWidth = lonBand.Width;
                int latsHeight = latBand.Height;
                float[] retLonBuffer, retLatBuffer;
                int retWidth, retHeight;
                int sample = 4;//经纬度数据集缩小4倍
                SampleMatrix(sample, lonBuffer, latBuffer, lonsWidth, latsHeight,
                    out retLonBuffer, out retLatBuffer, out retWidth, out retHeight);
                _orbitProjectionTransform = new OrbitProjectionTransform(retLonBuffer, retLatBuffer,
                     new Size(retWidth, retHeight),
                    _width / retWidth);
            }
        }

        private void SampleMatrix(int sample, float[] lonBuffer, float[] latBuffer, int width, int height, out float[] retLonBuffer, out float[] retLatBuffer, out int retWidth, out int retHeight)
        {
            retWidth = width / sample;
            retHeight = height / sample;
            retLonBuffer = new float[retWidth * retHeight];
            retLatBuffer = new float[retWidth * retHeight];
            float scaleRow = height / (float)retHeight;
            float scaleCol = width / (float)retWidth;
            int r = 0, c = 0, oIdx = 0, tIdx = 0;
            for (int row = 0; row < retHeight; row++)
            {
                r = (int)(scaleRow * row);
                oIdx = r * width;
                for (int col = 0; col < retWidth; col++, tIdx++)
                {
                    c = (int)(scaleCol * col);
                    //
                    retLonBuffer[tIdx] = lonBuffer[oIdx + c];
                    retLatBuffer[tIdx] = latBuffer[oIdx + c];
                }
            }
        }

        IOrbitProjectionTransform IOrbitProjectionTransformControl.OrbitProjectionTransform
        {
            get { return _orbitProjectionTransform; }
        }

        void IOrbitProjectionTransformControl.Free()
        {
            if (_orbitProjectionTransform != null)
            {
                _orbitProjectionTransform.Dispose();
                _orbitProjectionTransform = null;
            }
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
            switch (_dataType)
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

        public string FileName
        {
            get { return _filelist == null || _filelist.Length == 0 ? string.Empty : _filelist[0]; }
        }


        public virtual void BuildOverviews(int[] levels, Action<int, string> progressTracker)
        {
            throw new NotImplementedException();
        }

        public virtual void BuildOverviews(Action<int, string> progressTracker)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsSupprtOverviews
        {
            get { return false; }
        }

        public virtual bool IsOverviewsBuilded { get{return false ;} }
    }
}
