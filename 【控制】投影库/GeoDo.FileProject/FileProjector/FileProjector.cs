using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoDo.MEF;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GeoDo.FileProject
{
    public abstract class FileProjector : IFileProjector
    {
        protected string _name = null;
        protected string _fullname = null;
        protected IRasterDataProvider _curSession = null;
        protected bool _isBeginSession = false;
        protected IRasterProjector _rasterProjector = null;
        protected IRasterBand[] _rasterDataBands = null; //用于投影的通道数据集。其数组顺序即为输出目标文件的通道顺序
        protected IRasterBand[] _dstDataBands = null; //用于投影的通道数据集。其数组顺序即为输出目标文件的通道顺序

        //protected Action<int, string> progressCallback;
        protected int progress = 0;
        protected int progressCount = 100;
        protected int percent = 0;

        protected PrjEnvelope _maxPrjEnvelope = null;
        protected IRasterDataProvider _solarZenithCacheRaster = null; //太阳高度角通道
        protected Size _srcLocationSize;
        protected ISpatialReference _dstSpatialRef;
        //当前投影范围，需要使用的原始轨道数据最小范围
        protected Block _orbitBlock = null;
        protected double[] _xs = null;                //经纬度值或者公里数，存储的实际是计算后的值
        protected double[] _ys = null;                //经纬度值或者公里数，存储的实际是计算后的值
        
        protected bool _isRadiation = false;        //亮温订正
        protected bool _isSolarZenith = false;      //太阳天顶角订正
        /// <summary>
        /// 是否对亮温（辐亮度）进行临边变暗订正，公式如下
        /// T = Tb + deltaT
        /// deltaT == (Math.Pow(Math.E,0.00012*theta*theta)-1)*(0.1072*Tb-26.81)  //theta是卫星天顶角
        /// </summary>
        protected bool _isSensorZenith = false;
        //卫星天顶角通道。
        protected IRasterDataProvider _sensorSenithRaster = null;
        protected IRasterBand _sensorSenithBand;
        //protected short[] _sensorZenithData = null;

        //计算前要赋值
        protected string _outFormat = "LDF";
        protected string _outfilename = "";
        protected string _dstProj4 = "";
        protected int _dstBandCount = 0;
        protected Size _dstSize;
        protected PrjEnvelope _dstEnvelope = null;
        protected float _outResolutionX = 0;
        protected float _outResolutionY = 0;
        protected float _srcImgResolution = 1.0f;
        
        protected object[] _supportAngles = new object[] { "SensorAzimuth", "SensorZenith", "SolarAzimuth", "SolarZenith" };
        protected IRasterBand[] _angleBands = null;                   //输出的角度文件
        protected IRasterDataProvider[] _dstAngleRasters = null;
        protected IRasterBand[] _dstAngleBands = null;

        protected int _readyProgress = 0;
        protected IRasterBand _longitudeBand = null;     //用于投影的经纬度通道
        protected IRasterBand _latitudeBand = null;
        protected double _geoIntercept = 0d; //地理数据截距
        protected double _geoSlope = 1d;     //地理数据斜率

        //左右去除像元个数格式LeftRightInvalid={8,8}
        private Regex _leftRightInvalidArgReg = new Regex(@"LeftRightInvalid=\{(?<left>\d+?)\|(?<right>\d+?)\}", RegexOptions.Compiled);
        protected int _left = 0; //读取轨道数据时候左右去除像元个数
        protected int _right = 0;
        protected int _sacnLineWidth = 10;//扫描线宽度,默认为10，MERSI和MODIS的经纬度数据的扫描线。
        //投影扩展通道
        protected string[] _supportExtBandNames = null;
        protected IRasterBand[] _extSrcBands = null;
        protected IRasterBand[] _extDstBands = null;
        protected IRasterDataProvider[] _extDstRasters = null;

        public string Name
        {
            get { return _name; }
        }

        public string FullName
        {
            get { return _fullname; }
        }

        public abstract bool IsSupport(string fileName);

        public virtual void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatial, Action<int, string> progressCallback)
        {
            throw new NotImplementedException(); 
        }

        public virtual IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, RSS.Core.DF.IRasterDataProvider dstRaster, int beginBandIndex, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public virtual void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback, double weight, float zoom)
        {
            throw new NotImplementedException(); 
        }

        public abstract void ComputeDstEnvelope(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, out RasterProject.PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback);

        public abstract FilePrjSettings CreateDefaultPrjSettings();

        public virtual void BeginSession(IRasterDataProvider srcRaster)
        {
            _curSession = srcRaster;
            _isBeginSession = true;
        }

        public virtual void EndSession()
        {
            _curSession = null;
            _isBeginSession = false;
            if (_rasterDataBands != null && _rasterDataBands.Length != 0)
            {
                for (int i = 0; i < _rasterDataBands.Length; i++)
                {
                    if (_rasterDataBands[i] != null)
                    {
                        _rasterDataBands[i].Dispose();
                        _rasterDataBands[i] = null;
                    }
                }
                _rasterDataBands = null;
            }
            if (_dstAngleRasters != null && _dstAngleRasters.Length != 0)
            {
                for (int i = 0; i < _dstAngleRasters.Length; i++)
                {
                    if (_dstAngleRasters[i] != null)
                    {
                        _dstAngleRasters[i].Dispose();
                        _dstAngleRasters[i] = null;
                    }
                }
                _dstAngleRasters = null;
            }
            if (_solarZenithCacheRaster != null)
            {
                _solarZenithCacheRaster.Dispose();
                _solarZenithCacheRaster = null;
            }
            if (_sensorSenithBand != null)
            {
                _sensorSenithBand.Dispose();
                _sensorSenithBand = null;
            }
            if (_sensorSenithRaster != null)
            {
                _sensorSenithRaster.Dispose();
                _sensorSenithRaster = null;
            }
            if (_extSrcBands != null && _extSrcBands.Length != 0)
            {
                for (int i = 0; i < _extSrcBands.Length; i++)
                {
                    if (_extSrcBands[i] != null)
                    {
                        _extSrcBands[i].Dispose();
                        _extSrcBands[i] = null;
                    }
                }
                _extSrcBands = null;
            }
            if (_extDstBands != null && _extDstBands.Length != 0)
            {
                for (int i = 0; i < _extDstBands.Length; i++)
                {
                    if (_extDstBands[i] != null)
                    {
                        _extDstBands[i].Dispose();
                        _extDstBands[i] = null;
                    }
                }
                _extDstBands = null;
            }
            if (_extDstRasters != null && _extDstRasters.Length != 0)
            {
                for (int i = 0; i < _extDstRasters.Length; i++)
                {
                    if (_extDstRasters[i] != null)
                    {
                        _extDstRasters[i].Dispose();
                        _extDstRasters[i] = null;
                    }
                }
                _extDstRasters = null;
            }
        }

        public virtual void Dispose()
        {
        }

        internal void GetEnvelope(double[] xs, double[] ys, int w, int h, PrjEnvelope envelope, out Block block)
        {
            int length = xs.Length;
            int rOffset;
            int index;
            double x;
            double y;
            int xMin = w;
            int xMax = 0;
            int yMin = h;
            int yMax = 0;
            bool hasContain = false;
            for (int i = 0; i < h; i++)
            {
                rOffset = i * w;
                for (int j = 0; j < w; j++)
                {
                    index = rOffset + j;
                    x = xs[index];
                    y = ys[index];
                    if (envelope.Contains(x, y))
                    {
                        if (!hasContain)
                            hasContain = true;
                        if (xMin > j)
                            xMin = j;
                        if (xMax < j)
                            xMax = j;
                        if (yMin > i)
                            yMin = i;
                        if (yMax < i)
                            yMax = i;
                    }
                }
            }
            if (!hasContain)
            {
                block = Block.Empty;
                return;
            }
            //扩大16个像素，防止投影变形造成边缘像素缺失
            int sc = 16;
            xMin = xMin - sc < 0 ? 0 : xMin - sc;
            xMax = xMax + sc >= w - 1 ? w - 1 : xMax + sc;
            yMin = yMin - sc < 0 ? 0 : yMin - 10;
            yMax = yMax + sc >= h - 1 ? h - 1 : yMax + sc;
            //设置从整个扫描线行开始,为了有效去除条带
            if (_sacnLineWidth > 1)
            {
                int pYMin = yMin % _sacnLineWidth;
                if (pYMin != 0)
                    yMin = yMin - pYMin;
                int pYMax = (yMax + 1) % _sacnLineWidth;
                if (pYMax != 0)
                    yMax = yMax + (_sacnLineWidth - pYMax);
            }
            //防止偏移扫描线偏移后,超过范围
            yMin = yMin < 0 ? 0 : yMin;
            yMax = yMax >= h - 1 ? h - 1 : yMax;
            block = new Block { xOffset = xMin, xEnd = xMax, yBegin = yMin, yEnd = yMax };
        }

        internal void GetBlockDatas(double[] xs, double[] ys, int w, int h, int offBeginx, int offBeginy, int blockW, int blockH, out double[] blockXs, out double[] blockYs)
        {
            blockXs = new double[blockW * blockH];
            blockYs = new double[blockW * blockH];
            for (int i = 0; i < blockH; i++)
            {
                for (int j = 0; j < blockW; j++)
                {
                    blockXs[i * blockW + j] = xs[(i + offBeginy) * w + j + offBeginx];
                    blockYs[i * blockW + j] = ys[(i + offBeginy) * w + j + offBeginx];
                }
            }
        }

        internal string GetSolarZenithCacheFilename(string srcFilename)
        {
            return GetCacheFilename(srcFilename, "solarZenith.ldf");
        }

        protected string _catchDir = null;

        internal string GetCacheFilename(string srcFilename, string dstFilename)
        {
            _catchDir = System.AppDomain.CurrentDomain.BaseDirectory+ ".prjChche\\" + Path.GetFileName(srcFilename) + "\\";
            string dataFilename = _catchDir + dstFilename;
            if (!Directory.Exists(_catchDir))
                Directory.CreateDirectory(_catchDir);
            return dataFilename;
        }

        protected void TryDeleteCurCatch()
        {
            if (!string.IsNullOrWhiteSpace(_catchDir) && Directory.Exists(_catchDir))
            {
                try
                {
                    Directory.Delete(_catchDir,true);
                }
                catch (Exception ex)
                {
                    LogFactory.WriteLine("删除缓存目录失败：\r\n" + _catchDir + "\r\n错误信息：" + ex.Message);
                }
            }
        }

        internal void ReadBandData(out float[] bandData, IRasterDataProvider srcRaster, int bandNo, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            bandData = new float[blockWidth * blockHeight];
            IRasterBand latBand = null;
            try
            {
                latBand = srcRaster.GetRasterBand(bandNo);
                unsafe
                {
                    fixed (float* ptr = bandData)
                    {
                        IntPtr bufferptr = new IntPtr(ptr);
                        latBand.Read(xOffset, yOffset, blockWidth, blockHeight, bufferptr, enumDataType.Float, blockWidth, blockHeight);
                    }
                }
            }
            finally
            {
                if (latBand != null)
                    latBand.Dispose();
            }
        }
        internal void ReadBandData(out float[] bandData, IRasterBand band, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            bandData = null;
            if (band == null)
                return;
            try
            {
                bandData = new float[blockWidth * blockHeight];
                unsafe
                {
                    fixed (float* ptr = bandData)
                    {
                        IntPtr bufferptr = new IntPtr(ptr);
                        band.Read(xOffset, yOffset, blockWidth, blockHeight, bufferptr, enumDataType.Float, blockWidth, blockHeight);
                    }
                }
            }
            finally
            {
            }
        }
        
        internal short[] ReadBandData(IRasterBand band, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            if (band == null)
                return null;
            try
            {
                short[] bandData = new short[blockWidth * blockHeight];
                unsafe
                {
                    fixed (short* ptr = bandData)
                    {
                        IntPtr bufferptr = new IntPtr(ptr);
                        band.Read(xOffset, yOffset, blockWidth, blockHeight, bufferptr, enumDataType.Int16, blockWidth, blockHeight);
                    }
                }
                return bandData;
            }
            finally
            {
            }
        }

        internal void ReadBandData(IRasterBand band, out float[] bandData, out Size srcSize)
        {
            int width = band.Width;
            int height = band.Height;
            srcSize = new Size(width, height);
            bandData = new float[width * height];
            unsafe
            {
                fixed (float* ptrLong = bandData)
                {
                    IntPtr bufferPtrLong = new IntPtr(ptrLong);
                    band.Read(0, 0, width, height, bufferPtrLong, enumDataType.Float, width, height);
                }
            }
        }

        /// <summary>
        /// 仅用于用于投影的通道数据读取。非角度等数据
        /// </summary>
        /// <param name="bandData"></param>
        /// <param name="srcRaster"></param>
        /// <param name="dstBandIndex"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        internal void ReadImgBand(out ushort[] bandData, int dstBandIndex, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            IRasterBand latBand = _rasterDataBands[dstBandIndex];//
            bandData = new ushort[blockWidth * blockHeight];
            try
            {
                unsafe
                {
                    fixed (ushort* ptr = bandData)
                    {
                        IntPtr bufferptr = new IntPtr(ptr);
                        latBand.Read(xOffset, yOffset, blockWidth, blockHeight, bufferptr, enumDataType.UInt16, blockWidth, blockHeight);
                    }
                }
            }
            finally
            {
                //latBand.Dispose();//现在貌似还不能释放...
            }
        }

        internal void ReadImgBand<T>(out T[] bandData, enumDataType dataType, int dstBandIndex, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            IRasterBand latBand = _rasterDataBands[dstBandIndex];//
            bandData = new T[blockWidth * blockHeight];
            GCHandle h = GCHandle.Alloc(bandData, GCHandleType.Pinned);
            try
            {
                IntPtr bufferPtr = h.AddrOfPinnedObject();
                latBand.Read(xOffset, yOffset, blockWidth, blockHeight, bufferPtr, dataType, blockWidth, blockHeight);
            }
            finally
            {
                h.Free();
            }
        }

        internal void ReadImgBand(IRasterBand band, int xOffset, int yOffset, int xSize, int ySize, Size bufferSize, out ushort[] bandData)
        {
            bandData = new ushort[bufferSize.Width * bufferSize.Height];
            unsafe
            {
                fixed (ushort* ptr = bandData)
                {
                    IntPtr bufferptr = new IntPtr(ptr);
                    band.Read(xOffset, yOffset, xSize, ySize, bufferptr, enumDataType.UInt16, bufferSize.Width, bufferSize.Height);
                }
            }
        }

        internal void InvalidLongLat(double[] xs, double[] ys, int width, int height, int left, int right)
        {
            int rightBegin = width - right;
            for (int j = 0; j < height; j++)
            {
                for (int m = 0; m < left; m++)
                {
                    int index = j * width + m;
                    xs[index] = 999d;
                }
                for (int n = rightBegin; n < width; n++)
                {
                    int index = j * width + n;
                    xs[index] = 999d;
                }
            }
        }

        internal IRasterDataProvider WriteData(float[] data, string fileName, int width, int height)
        {
            string[] options = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
               };
            IRasterDataDriver outLdfDriver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            IRasterDataProvider cacheWriter = outLdfDriver.Create(fileName, width, height, 1, enumDataType.Float, options) as IRasterDataProvider;
            {
                using (IRasterBand band = cacheWriter.GetRasterBand(1))
                {
                    unsafe
                    {
                        fixed (float* ptr = data)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Write(0, 0, width, height, bufferPtr, enumDataType.Float, width, height);
                        }
                    }
                }
            }
            return cacheWriter;
        }

        internal IRasterDataProvider CreateOutFile(string outfilename, int dstBandCount, Size outSize, string[] options)
        {
            CheckAndCreateDir(Path.GetDirectoryName(outfilename));
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount, enumDataType.UInt16, options) as IRasterDataProvider;
        }

        internal IRasterDataProvider CreateOutFile(string outfilename, int dstBandCount, Size outSize, enumDataType dataType, string[] options)
        {
            CheckAndCreateDir(Path.GetDirectoryName(outfilename));
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount, dataType, options) as IRasterDataProvider;
        }
        
        /// <summary>
        /// 准备要投影的四个角度波段
        /// </summary>
        /// <param name="srcAngleRaster"></param>
        /// <param name="mainFilename"></param>
        /// <param name="prjSettings"></param>
        /// <param name="outSize"></param>
        /// <param name="options"></param>
        internal void ReadyAngleFiles(IRasterDataProvider srcAngleRaster, string mainFilename, FilePrjSettings prjSettings, Size outSize, string[] options)
        {
            _angleBands = null;
            if (prjSettings.ExtArgs == null || prjSettings.ExtArgs.Length == 0)
                return;
            object[] extArgs = prjSettings.ExtArgs;
            List<IRasterBand> srcAngleBands = new List<IRasterBand>();
            List<IRasterDataProvider> dstAngleRasters = new List<IRasterDataProvider>();
            List<IRasterBand> dstAngleBands = new List<IRasterBand>();
            IBandProvider srcbandpro = srcAngleRaster.BandProvider as IBandProvider;
            foreach (string extarg in extArgs)
            {
                if (_supportAngles.Contains(extarg))
                {
                    IRasterBand[] band = srcbandpro.GetBands(extarg);
                    if (band != null && band.Length != 0)
                    {
                        srcAngleBands.Add(band[0]);
                        string fileName = Path.ChangeExtension(mainFilename, extarg + ".ldf");

                        List<string> opts = new List<string>(options);
                        opts.Add("BANDNAMES=" + extarg);
                        IRasterDataProvider dstAngleRaster = CreateOutFile(fileName, 1, outSize, enumDataType.Int16, opts.ToArray());
                        IRasterBand dstband = dstAngleRaster.GetRasterBand(1);
                        dstAngleRasters.Add(dstAngleRaster);
                        dstAngleBands.Add(dstband);
                    }
                }
            }
            _angleBands = srcAngleBands.ToArray();
            _dstAngleRasters = dstAngleRasters.ToArray();
            _dstAngleBands = dstAngleBands.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dstbufferSize"></param>
        /// <param name="srcBufferSize"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <param name="blockYIndex"></param>
        /// <param name="blockXIndex"></param>
        /// <param name="curOrbitblock"></param>
        /// <param name="dstRowLookUpTable"></param>
        /// <param name="dstColLookUpTable"></param>
        internal void ProjectAngle(Size dstbufferSize, Size srcBufferSize, int blockWidth, int blockHeight, int blockYIndex, int blockXIndex, Block curOrbitblock, UInt16[] dstRowLookUpTable, UInt16[] dstColLookUpTable, Action<int, string> progressCallback)
        {
            if (_angleBands != null && _angleBands.Length != 0)
            {
                short[] srcBandData = null;
                short[] dstBandData = new short[dstbufferSize.Width * dstbufferSize.Height];
                for (int i = 0; i < _angleBands.Length; i++)
                {
                    if (progressCallback != null)
                    {
                        progress++;
                        percent = progress * 100 / progressCount;
                        progressCallback(percent, string.Format("投影角度数据{0}%", percent));
                    }
                    IRasterBand srcAngleBand = _angleBands[i];
                    ReadAgileBand(srcAngleBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, srcBufferSize, out srcBandData);
                    _rasterProjector.Project<short>(srcBandData, srcBufferSize, dstRowLookUpTable, dstColLookUpTable, dstbufferSize, dstBandData, 0, null);
                    IRasterBand dstAngleBand = _dstAngleBands[i];
                    unsafe
                    {
                        fixed (short* ptr = dstBandData)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            int blockOffsetY = blockYIndex * blockHeight;
                            int blockOffsetX = blockXIndex * blockWidth;
                            dstAngleBand.Write(blockOffsetX, blockOffsetY, blockWidth, blockHeight, bufferPtr, enumDataType.Int16, dstbufferSize.Width, dstbufferSize.Height);
                        }
                    }
                }
            }
        }
        
        #region 投影扩展的波段数据，如Height、LandSea...
        internal virtual void ReadyExtBands(IRasterDataProvider srcRaster, string mainFilename, FilePrjSettings prjSettings, Size outSize, string[] options)
        {
            _extSrcBands = null;
            if (prjSettings.ExtArgs == null || prjSettings.ExtArgs.Length == 0)
                return;
            if (_supportExtBandNames == null)
                return;
            object[] extArgs = prjSettings.ExtArgs;
            string[] extBandNames = TryParseExtBands(extArgs);
            if (extBandNames == null || extBandNames.Length==0)
                return;
            List<IRasterBand> srcBands = new List<IRasterBand>();
            List<IRasterDataProvider> dstRasters = new List<IRasterDataProvider>();
            List<IRasterBand> dstBands = new List<IRasterBand>();
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            foreach (string extBandName in extBandNames)
            {
                if (_supportExtBandNames.Contains(extBandName))
                {
                    IRasterBand[] band = srcbandpro.GetBands(extBandName);
                    if (band != null && band.Length != 0)
                    {
                        srcBands.Add(band[0]);
                        string fileName = Path.ChangeExtension(mainFilename, extBandName + ".ldf");
                        List<string> opts = new List<string>(options);
                        opts.Add("BANDNAMES=" + extBandName);
                        IRasterDataProvider dstAngleRaster = CreateOutFile(fileName, 1, outSize, band[0].DataType, opts.ToArray());
                        IRasterBand dstband = dstAngleRaster.GetRasterBand(1);
                        dstRasters.Add(dstAngleRaster);
                        dstBands.Add(dstband);
                    }
                }
            }
            _extSrcBands = srcBands.ToArray();
            _extDstRasters = dstRasters.ToArray();
            _extDstBands = dstBands.ToArray();
        }

        private Regex _extKeyValues = new Regex(@"(?<key>\w+)\s*=\s*(?<value>[\w|\W]*)\s*", RegexOptions.Compiled);

        private string[] TryParseExtBands(object[] extArgs)
        {
            if (extArgs != null && extArgs.Length != 0)
            {
                foreach (object extArg in extArgs)
                {
                    if (extArg is string)
                    {
                        string strExtArg = extArg as string;
                        Match match = _extKeyValues.Match(strExtArg);
                        if (match.Success)
                        {
                            string key = match.Groups["key"].Value;
                            string value = match.Groups["value"].Value;
                            if (key == "ExtBands")
                            {
                                return value.Split(';');
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal void ProjectExtBands(Size dstbufferSize, Size srcBufferSize, int blockWidth, int blockHeight, int blockYNo, int blockXNo, Block curOrbitblock, UInt16[] dstRowLookUpTable, UInt16[] dstColLookUpTable, Action<int, string> progressCallback)
        {
            if (_extSrcBands != null && _extSrcBands.Length != 0)
            {
                for (int i = 0; i < _extSrcBands.Length; i++)
                {
                    IRasterBand srcBand = _extSrcBands[i];
                    IRasterBand dstBand = _extDstBands[i];
                    enumDataType dataType = srcBand.DataType;
                    switch (dataType)
                    {
                        case enumDataType.Atypism://不支持
                            Console.WriteLine("不支持混合类型的波段数据投影");
                            break;
                        case enumDataType.Bits:
                            Console.WriteLine("不支持Bits类型的波段数据投影");
                            break;
                        case enumDataType.Byte:
                            ProjectExtBands<Byte>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.Double:
                            ProjectExtBands<Double>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.Float:
                            ProjectExtBands<Single>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.Int16:
                            ProjectExtBands<short>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.Int32:
                            ProjectExtBands<Int32>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.Int64:
                            ProjectExtBands<Int64>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.UInt16:
                            ProjectExtBands<UInt16>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.UInt32:
                            ProjectExtBands<UInt32>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.UInt64:
                            ProjectExtBands<UInt64>(srcBand, dstBand, dstbufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                            break;
                        case enumDataType.Unknow:
                            Console.WriteLine("不支持Unknow类型的波段数据投影");
                            break;
                        default:
                            break;
                    }
                    if (progressCallback != null)
                    {
                        progress++;
                        percent = progress * 100 / progressCount;
                        progressCallback(percent, string.Format("投影扩展波段数据", _extSrcBands[i].Description));
                    }
                }
            }
        }

        internal void ProjectExtBands<T>(IRasterBand srcBand,IRasterBand dstBand, Size dstbufferSize, Size srcBufferSize, int blockWidth, int blockHeight, int blockYIndex, int blockXIndex, Block curOrbitblock, UInt16[] dstRowLookUpTable, UInt16[] dstColLookUpTable, Action<int, string> progressCallback)
        {
            T[] srcBandData = null;
            ReadBandForPrj(srcBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, srcBufferSize, out srcBandData);
            T[] dstBandData = new T[dstbufferSize.Width * dstbufferSize.Height];
            GCHandle h = GCHandle.Alloc(dstBandData, GCHandleType.Pinned);
            try
            {
                IntPtr bufferPtr = h.AddrOfPinnedObject();
                _rasterProjector.Project<T>(srcBandData, srcBufferSize, dstRowLookUpTable, dstColLookUpTable, dstbufferSize, dstBandData, default(T), null);
                int blockOffsetY = blockYIndex * blockHeight;
                int blockOffsetX = blockXIndex * blockWidth;
                dstBand.Write(blockOffsetX, blockOffsetY, blockWidth, blockHeight, bufferPtr, enumDataType.Int16, dstbufferSize.Width, dstbufferSize.Height);
            }
            finally
            {
                h.Free();
            }
        }

        internal virtual void ReadBandForPrj<T>(IRasterBand band, int xOffset, int yOffset, int xSize, int ySize, Size bufferSize, out T[] bandData)
        {
            bandData = new T[bufferSize.Width * bufferSize.Height];
            GCHandle h = GCHandle.Alloc(bandData, GCHandleType.Pinned);
            try
            {
                IntPtr bufferPtr = h.AddrOfPinnedObject();
                band.Read(xOffset, yOffset, xSize, ySize, bufferPtr, band.DataType, bufferSize.Width, bufferSize.Height);
            }
            finally
            {
                h.Free();
            }
        }
        #endregion

        internal virtual void ReadAgileBand(IRasterBand band, int xOffset, int yOffset, int xSize, int ySize, Size bufferSize, out short[] bandData)
        {
            bandData = new short[bufferSize.Width * bufferSize.Height];
            unsafe
            {
                fixed (short* ptr = bandData)
                {
                    IntPtr bufferptr = new IntPtr(ptr);
                    band.Read(xOffset, yOffset, xSize, ySize, bufferptr, enumDataType.Int16, bufferSize.Width, bufferSize.Height);
                }
            }
        }

        protected void ProjectRaster(IRasterDataProvider srcRaster, IRasterDataProvider prdWriter, int beginBandIndex, Action<int, string> progressCallback)
        {
            switch (srcRaster.DataType)
            {
                case enumDataType.Atypism:
                    throw new Exception("不支持混合类型数据");
                case enumDataType.Bits:
                    ProjectRaster<sbyte>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.Byte:
                    ProjectRaster<byte>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.Double:
                    ProjectRaster<Double>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.Float:
                    ProjectRaster<float>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.Int16:
                    ProjectRaster<Int16>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.Int32:
                    ProjectRaster<Int32>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.Int64:
                    ProjectRaster<Int64>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.UInt16:
                    ProjectRaster<UInt16>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.UInt32:
                    ProjectRaster<UInt32>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.UInt64:
                    ProjectRaster<UInt64>(srcRaster, prdWriter, 0, progressCallback);
                    break;
                case enumDataType.Unknow:
                default:
                    throw new Exception("未知数据类型");
                    break;
            }
        }

        protected void ProjectRaster(IRasterDataProvider srcRaster, IRasterDataProvider prdWriter, int beginBandIndex, double invalidValue, Action<int, string> progressCallback)
        {
            switch (srcRaster.DataType)
            {
                case enumDataType.Atypism:
                    throw new Exception("不支持混合类型数据");
                case enumDataType.Bits:
                    ProjectRaster<sbyte>(srcRaster, prdWriter, 0, (sbyte)invalidValue, progressCallback);
                    break;
                case enumDataType.Byte:
                    ProjectRaster<byte>(srcRaster, prdWriter, 0, (byte)invalidValue, progressCallback);
                    break;
                case enumDataType.Double:
                    ProjectRaster<Double>(srcRaster, prdWriter, 0, (double)invalidValue, progressCallback);
                    break;
                case enumDataType.Float:
                    ProjectRaster<float>(srcRaster, prdWriter, 0, (float)invalidValue, progressCallback);
                    break;
                case enumDataType.Int16:
                    ProjectRaster<Int16>(srcRaster, prdWriter, 0, (Int16)invalidValue, progressCallback);
                    break;
                case enumDataType.Int32:
                    ProjectRaster<Int32>(srcRaster, prdWriter, 0, (Int32)invalidValue, progressCallback);
                    break;
                case enumDataType.Int64:
                    ProjectRaster<Int64>(srcRaster, prdWriter, 0, (Int64)invalidValue, progressCallback);
                    break;
                case enumDataType.UInt16:
                    ProjectRaster<UInt16>(srcRaster, prdWriter, 0, (UInt16)invalidValue, progressCallback);
                    break;
                case enumDataType.UInt32:
                    ProjectRaster<UInt32>(srcRaster, prdWriter, 0, (UInt32)invalidValue, progressCallback);
                    break;
                case enumDataType.UInt64:
                    ProjectRaster<UInt64>(srcRaster, prdWriter, 0, (UInt64)invalidValue, progressCallback);
                    break;
                case enumDataType.Unknow:
                default:
                    throw new Exception("未知数据类型");
            }
        }

        private void ProjectRaster<T>(IRasterDataProvider srcImgRaster, IRasterDataProvider prdWriter, int beginBandIndex, Action<int, string> progressCallback)
        {
            ProjectRaster<T>(srcImgRaster, prdWriter, beginBandIndex, default(T), progressCallback);
        }

        private void ProjectRaster<T>(IRasterDataProvider srcImgRaster, IRasterDataProvider prdWriter, int beginBandIndex, T invalidValue, Action<int, string> progressCallback)
        { 
            if (srcImgRaster == null || srcImgRaster.Width == 0 || srcImgRaster.Height == 0)
                throw new Exception("投影数据失败：无法读取源数据,或者源数据高或宽为0。");
            Size srcImgSize = new Size(srcImgRaster.Width, srcImgRaster.Height);
            Size outSize = _dstEnvelope.GetSize(_outResolutionX, _outResolutionY);
            float bufferResolutionX = 0f;
            float bufferResolutionY = 0f;
            float outXScale = _srcImgResolution / _outResolutionX;
            float outYScale = _srcImgResolution / _outResolutionY;
            if (outXScale > 1.5f || outYScale > 1.5f)
            {
                bufferResolutionX = _srcImgResolution;
                bufferResolutionY = _srcImgResolution;
            }
            else
            {
                bufferResolutionX = _outResolutionX;
                bufferResolutionY = _outResolutionY;
            }
            int blockXNum;
            int blockYNum;
            int blockWidth;
            int blockHeight;
            GetBlockNumber(outSize, _srcLocationSize, outXScale, outYScale, out blockXNum, out blockYNum, out blockWidth, out blockHeight);
            int imgLocationRatioX = srcImgSize.Width / _srcLocationSize.Width;
            int imgLocationRatioY = srcImgSize.Height / _srcLocationSize.Height;
            progressCount = blockYNum * blockXNum * (_dstBandCount + (_angleBands == null ? 0 : _angleBands.Length) + (_extSrcBands == null ? 0 : _extSrcBands.Length));
            progress = 0;
            percent = 0;
            Size bufferSize;
            for (int blockYIndex = 0; blockYIndex < blockYNum; blockYIndex++)
            {
                for (int blockXIndex = 0; blockXIndex < blockXNum; blockXIndex++)
                {
                    //起始偏移，结束偏移
                    int beginX = blockWidth * blockXIndex;
                    int beginY = blockHeight * blockYIndex;
                    if (beginX >= outSize.Width || beginY >= outSize.Height)
                        continue;
                    if (beginX + blockWidth > outSize.Width)
                        blockWidth = outSize.Width - beginX;
                    if (beginY + blockHeight > outSize.Height)
                        blockHeight = outSize.Height - beginY;

                    //当前块的四角范围
                    double blockMinX = _dstEnvelope.MinX + beginX * _outResolutionX;
                    double blockMaxX = blockMinX + blockWidth * _outResolutionX;
                    double blockMaxY = _dstEnvelope.MaxY - beginY * _outResolutionY;
                    double blockMinY = blockMaxY - blockHeight * _outResolutionY;
                    PrjEnvelope blockEnvelope = new PrjEnvelope(blockMinX, blockMaxX, blockMinY, blockMaxY);
                    bufferSize = blockEnvelope.GetSize(bufferResolutionX, bufferResolutionY);
                    //根据当前输出块,反算出对应的源数据块起始行列，为了减小后面需要读取的源数据大小
                    Block curOrbitblock = null;//经纬度数据集，计算轨道数据范围偏移
                    double[] srcBlockXs;
                    double[] srcBlockYs;
                    if (blockYNum == 1 && blockXNum == 1)                           //没分块的情况
                    {
                        curOrbitblock = _orbitBlock.Clone() as Block;
                        if (curOrbitblock.xOffset < _left)
                            curOrbitblock.xOffset = _left;
                        if (curOrbitblock.xEnd > _srcLocationSize.Width - 1 - _right)
                            curOrbitblock.xEnd = _srcLocationSize.Width - 1 - _right;

                        if (curOrbitblock.Width == _srcLocationSize.Width && curOrbitblock.Height == _srcLocationSize.Height)
                        {
                            srcBlockXs = _xs;
                            srcBlockYs = _ys;
                        }
                        else
                            GetBlockDatas(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, out srcBlockXs, out srcBlockYs);
                    }
                    else
                    {
                        GetEnvelope(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, blockEnvelope, out curOrbitblock);
                        if (curOrbitblock.Width <= 0 || curOrbitblock.Height <= 0)      //当前分块不在图像内部
                        {
                            progress += _dstBandCount;
                            continue;
                        }
                        if (curOrbitblock.xOffset > _left)
                            curOrbitblock.xOffset = _left;
                        if (curOrbitblock.xEnd > _srcLocationSize.Width - 1 - _right)
                            curOrbitblock.xEnd = _srcLocationSize.Width - 1 - _right;
                        GetBlockDatas(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, out srcBlockXs, out srcBlockYs);
                    }
                    int srcBlockJdWidth = curOrbitblock.Width;
                    int srcBlockJdHeight = curOrbitblock.Height;
                    int srcBlockImgWidth = curOrbitblock.Width * imgLocationRatioX;
                    int srcBlockImgHeight = curOrbitblock.Height * imgLocationRatioY;
                    Size srcBlockLocationSize = new Size(srcBlockJdWidth, srcBlockJdHeight);
                    Size srcBlockImgSize = new Size(srcBlockImgWidth, srcBlockImgHeight);

                    //计算当前分块的投影查算表
                    UInt16[] dstRowLookUpTable = new UInt16[bufferSize.Width * bufferSize.Height];
                    UInt16[] dstColLookUpTable = new UInt16[bufferSize.Width * bufferSize.Height];
                    if (imgLocationRatioX == 1)
                        _rasterProjector.ComputeIndexMapTable(srcBlockXs, srcBlockYs, srcBlockImgSize, bufferSize, blockEnvelope, _maxPrjEnvelope,
                            out dstRowLookUpTable, out dstColLookUpTable, null);
                    else
                        _rasterProjector.ComputeIndexMapTable(srcBlockXs, srcBlockYs, srcBlockLocationSize, srcBlockImgSize, bufferSize, blockEnvelope, //_maxPrjEnvelope,
                            out dstRowLookUpTable, out dstColLookUpTable, null, _sacnLineWidth);
                    enumDataType dataType = srcImgRaster.DataType;
                    //执行投影
                    for (int i = 0; i < _dstBandCount; i++)  //读取原始通道值，投影到目标区域
                    {
                        T[] srcBandData = null;
                        T[] dstBandData = new T[bufferSize.Width * bufferSize.Height];
                        if (progressCallback != null)
                        {
                            progress++;
                            percent = (int)(progress * 100 / progressCount);
                            progressCallback(percent, string.Format("投影完成{0}%", percent));
                        }
                        ReadImgBand(out srcBandData, dataType, i, curOrbitblock.xOffset * imgLocationRatioX, curOrbitblock.yBegin * imgLocationRatioY, srcBlockImgWidth, srcBlockImgHeight);
                        Size angleSize = new Size(srcBlockJdWidth, srcBlockJdHeight);
                        _rasterProjector.Project<T>(srcBandData, srcBlockImgSize, dstRowLookUpTable, dstColLookUpTable, bufferSize, dstBandData, invalidValue, null);
                        srcBandData = null;
                        IRasterBand band = prdWriter.GetRasterBand(i + 1);
                        GCHandle h = GCHandle.Alloc(dstBandData, GCHandleType.Pinned);
                        try
                        {
                            IntPtr bufferPtr = h.AddrOfPinnedObject();
                            int blockOffsetY = blockHeight * blockYIndex;
                            int blockOffsetX = blockWidth * blockXIndex;
                            band.Write(blockOffsetX, blockOffsetY, blockWidth, blockHeight, bufferPtr, dataType, bufferSize.Width, bufferSize.Height);
                        }
                        finally
                        {
                            h.Free();
                        }
                        dstBandData = null;
                    }
                    //ReleaseZenithData();
                    //Size srcBufferSize = new Size(srcBlockImgWidth, srcBlockImgHeight);
                    //ProjectAngle(bufferSize, srcBufferSize, blockWidth, blockHeight, blockYIndex, blockXIndex, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                    dstRowLookUpTable = null;
                    dstColLookUpTable = null;
                }
            }
        }

        protected void ProjectToLDF(IRasterDataProvider srcImgRaster, IRasterDataProvider dstImgRaster, int beginBandIndex, Action<int, string> progressCallback)
        {
            //progressCallback = progressCallback;
            if (srcImgRaster == null || srcImgRaster.Width == 0 || srcImgRaster.Height == 0)
                throw new Exception("投影数据失败：无法读取源数据,或者源数据高或宽为0。");
            Size srcImgSize = new Size(srcImgRaster.Width, srcImgRaster.Height);
            Size outSize = _dstEnvelope.GetSize(_outResolutionX, _outResolutionY);
            float bufferResolutionX = 0f;
            float bufferResolutionY = 0f;
            float outXScale = _srcImgResolution / _outResolutionX;
            float outYScale = _srcImgResolution / _outResolutionY;
            if (outXScale > 1.5f || outYScale > 1.5f)
            {
                bufferResolutionX = _srcImgResolution;
                bufferResolutionY = _srcImgResolution;
            }
            else
            {
                bufferResolutionX = _outResolutionX;
                bufferResolutionY = _outResolutionY;
            }
            int blockXCount;
            int blockYCount;
            int blockWidth;
            int blockHeight;
            //后面投影需要的内存：（double）经纬度数据、（int16）原始通道数据、（int16）投影后通道、（int16）其他（如角度数据等）
            GetBlockNumber(outSize,_srcLocationSize, outXScale, outYScale, out blockXCount, out blockYCount, out blockWidth, out blockHeight);
            int imgLocationRatioX = srcImgSize.Width / _srcLocationSize.Width;
            int imgLocationRatioY = srcImgSize.Height / _srcLocationSize.Height;
            progressCount = blockYCount * blockXCount * (_dstBandCount + (_angleBands == null ? 0 : _angleBands.Length) + (_extSrcBands == null ? 0 : _extSrcBands.Length));
            progress = 0;
            percent = 0;
            Size bufferSize;

            #region 在需要分块的情况下，采样经纬度数据集
            int bC = 1;
            int tmpWidth = 0;
            int tmpHeight = 0;
            double[] tmpxs = null;
            double[] tmpys = null;
            if (blockYCount * blockXCount>1 && (_xs == null || _ys == null))
            {
                bC = (int)Math.Sqrt(blockXCount * blockYCount) + 1;
                tmpWidth = _srcLocationSize.Width / bC;
                tmpHeight = _srcLocationSize.Height / bC;
                tmpxs = ReadSampleDatas(_longitudeBand, 0, 0, tmpWidth, tmpHeight);
                tmpys = ReadSampleDatas(_latitudeBand, 0, 0, tmpWidth, tmpHeight);
                TryApplyGeoInterceptSlope(tmpxs, tmpys);
                _rasterProjector.Transform(SpatialReference.GetDefault(), tmpxs, tmpys, _dstSpatialRef);
            }
            #endregion

            for (int blockXNo = 0; blockXNo < blockXCount; blockXNo++)
            {
                for (int blockYNo = 0; blockYNo < blockYCount; blockYNo++)
                {
                    //起始偏移，结束偏移
                    int beginX = blockWidth* blockXNo;
                    int beginY = blockHeight* blockYNo;
                    if (beginX >= outSize.Width || beginY >= outSize.Height)
                        continue;
                    if (beginX + blockWidth > outSize.Width)
                        blockWidth = outSize.Width - beginX;
                    if (beginY + blockHeight > outSize.Height)
                        blockHeight = outSize.Height - beginY;

                    //当前块的四角范围
                    double blockMinX = _dstEnvelope.MinX + beginX * _outResolutionX;
                    double blockMaxX = blockMinX + blockWidth * _outResolutionX;
                    double blockMaxY = _dstEnvelope.MaxY - beginY * _outResolutionY;
                    double blockMinY = blockMaxY - blockHeight * _outResolutionY;
                    PrjEnvelope blockEnvelope = new PrjEnvelope(blockMinX, blockMaxX, blockMinY, blockMaxY);
                    bufferSize = blockEnvelope.GetSize(bufferResolutionX, bufferResolutionY);
                    //根据当前输出块,反算出对应的源数据块(轨道)起始行列，为了减小后面需要读取的源数据大小
                    Block curOrbitblock = null;
                    //开始获取当前分块的经纬度数据集，计算轨道数据范围偏移
                    double[] srcBlockXs;
                    double[] srcBlockYs;
                    if (blockYCount == 1 && blockXCount == 1)                           //没分块的情况
                    {
                        curOrbitblock = _orbitBlock.Clone() as Block;
                        if (curOrbitblock.xOffset < _left)
                            curOrbitblock.xOffset = _left;
                        if (curOrbitblock.xEnd > _srcLocationSize.Width - 1 - _right)
                            curOrbitblock.xEnd = _srcLocationSize.Width - 1 - _right;
                        if (curOrbitblock.Width == _srcLocationSize.Width && curOrbitblock.Height == _srcLocationSize.Height)
                        {
                            if (_xs != null && _ys != null)
                            {
                                srcBlockXs = _xs;
                                srcBlockYs = _ys;
                            }
                            else
                            {
                                srcBlockXs = ReadBlockDatas(_longitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                                srcBlockYs = ReadBlockDatas(_latitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);

                                TryApplyGeoInterceptSlope(srcBlockXs, srcBlockYs);
                                _rasterProjector.Transform(SpatialReference.GetDefault(), srcBlockXs, srcBlockYs, _dstSpatialRef);
                            }
                        }
                        else
                        {
                            if (_xs != null && _ys != null)
                            {
                                GetBlockDatas(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, out srcBlockXs, out srcBlockYs);
                            }
                            else
                            {
                                srcBlockXs = ReadBlockDatas(_longitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                                srcBlockYs = ReadBlockDatas(_latitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                                TryApplyGeoInterceptSlope(srcBlockXs, srcBlockYs);
                                _rasterProjector.Transform(SpatialReference.GetDefault(), srcBlockXs, srcBlockYs, _dstSpatialRef);
                            }
                        }
                    }
                    else
                    {
                        if (_xs != null && _ys != null)
                        {
                            GetEnvelope(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, blockEnvelope, out curOrbitblock);
                        }
                        else
                        {
                            //计算偏移。
                            GetEnvelope(tmpxs, tmpys, tmpWidth, tmpHeight, blockEnvelope, out curOrbitblock);
                            curOrbitblock = curOrbitblock.Zoom(bC, bC);
                        }
                        if (curOrbitblock.Width <= 0 || curOrbitblock.Height <= 0)      //当前分块不在图像内部
                        {
                            progress += _dstBandCount;
                            continue;
                        }
                        if (curOrbitblock.xOffset < _left)
                            curOrbitblock.xOffset = _left;
                        if (curOrbitblock.xEnd > _srcLocationSize.Width - 1 - _right)
                            curOrbitblock.xEnd = _srcLocationSize.Width - 1 - _right;
                        if (_xs != null && _ys != null)
                        {
                            GetBlockDatas(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, out srcBlockXs, out srcBlockYs);
                        }
                        else
                        {
                            srcBlockXs = ReadBlockDatas(_longitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                            srcBlockYs = ReadBlockDatas(_latitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                            TryApplyGeoInterceptSlope(srcBlockXs, srcBlockYs);
                            _rasterProjector.Transform(SpatialReference.GetDefault(), srcBlockXs, srcBlockYs, _dstSpatialRef);
                        }
                    }
                    int srcBlockJdWidth = curOrbitblock.Width;
                    int srcBlockJdHeight = curOrbitblock.Height;
                    int srcBlockImgWidth = curOrbitblock.Width * imgLocationRatioX;
                    int srcBlockImgHeight = curOrbitblock.Height * imgLocationRatioY;
                    Size srcBlockLocationSize = new Size(srcBlockJdWidth, srcBlockJdHeight);
                    Size srcBlockImgSize = new Size(srcBlockImgWidth, srcBlockImgHeight);
                    //亮温订正，天顶角修正：下面获取用到的部分经纬度和太阳高度角修正系数数据,下面修改为从临时文件直接读取。
                    //最新的FY3C，250米数据，可以使用250米地理数据，但是角度数据还是1km的，这时候就需要将角度数据，读取为250米大小。
                    float[] solarZenithData = null;
                    Size srcAngleBlockSize = Size.Empty;
                    if (_isRadiation && _isSolarZenith)
                    {
                        if (_solarZenithCacheRaster != null)    //太阳天顶角数据
                        {
                            int angle2geoRatioX = _srcLocationSize.Width / _solarZenithCacheRaster.Width;
                            int angle2geoRatioY = _srcLocationSize.Height / _solarZenithCacheRaster.Height;
                            srcAngleBlockSize = new Size(curOrbitblock.Width / angle2geoRatioX, curOrbitblock.Height / angle2geoRatioY);
                            ReadBandData(out solarZenithData, _solarZenithCacheRaster, 1, curOrbitblock.xOffset / angle2geoRatioX, curOrbitblock.yBegin / angle2geoRatioY,
                                srcAngleBlockSize.Width, srcAngleBlockSize.Height);
                            //亮温临边变暗订正,读取卫星天顶角数据。
                            //if (_isSensorZenith)
                            //    ReadBandData(out _sensorZenithData, _sensorSenithBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                            TryReadZenithData(curOrbitblock.xOffset / angle2geoRatioX, curOrbitblock.yBegin / angle2geoRatioY, srcAngleBlockSize.Width, srcAngleBlockSize.Height);
                        }
                        else
                            srcAngleBlockSize = new Size(srcBlockJdWidth, srcBlockJdHeight);//认为角度和经纬度数据一致
                    }
                    //计算当前分块的投影查算表
                    UInt16[] dstRowLookUpTable = new UInt16[bufferSize.Width * bufferSize.Height];
                    UInt16[] dstColLookUpTable = new UInt16[bufferSize.Width * bufferSize.Height];
                    if (imgLocationRatioX == 1)
                        _rasterProjector.ComputeIndexMapTable(srcBlockXs, srcBlockYs, srcBlockImgSize, bufferSize, blockEnvelope, _maxPrjEnvelope,
                            out dstRowLookUpTable, out dstColLookUpTable, null);
                    else
                        _rasterProjector.ComputeIndexMapTable(srcBlockXs, srcBlockYs, srcBlockLocationSize, srcBlockImgSize, bufferSize, blockEnvelope, //_maxPrjEnvelope,
                            out dstRowLookUpTable, out dstColLookUpTable, null, _sacnLineWidth);

                    ////处理AOI。
                    //if (false)
                    //{
                    //    int[] aoiIndex = null;
                    //    aoiIndex = GetAoiIndex(bufferSize, blockEnvelope, _dstSpatialRef);
                    //    SetAoiIndex(aoiIndex, dstRowLookUpTable, RasterProjector.InvalidValue);
                    //    SetAoiIndex(aoiIndex, dstRowLookUpTable, RasterProjector.InvalidValue);
                    //}

                    //执行投影
                    UInt16[] srcBandData = null;
                    UInt16[] dstBandData = new UInt16[bufferSize.Width * bufferSize.Height];
                    for (int i = 0; i < _dstBandCount; i++)  //读取原始通道值，投影到目标区域
                    {
                        if (progressCallback != null)
                        {
                            progress++;
                            percent = (int)(progress * 100 / progressCount);
                            progressCallback(percent, string.Format("投影完成{0}%", percent));
                        }
                        ReadImgBand(out srcBandData, i, curOrbitblock.xOffset * imgLocationRatioX, curOrbitblock.yBegin * imgLocationRatioY, srcBlockImgWidth, srcBlockImgHeight);
                        //Size angleSize = new Size(srcBlockJdWidth, srcBlockJdHeight);
                        DoRadiation(srcImgRaster, i, srcBandData, solarZenithData, srcBlockImgSize, srcAngleBlockSize);
                        _rasterProjector.Project<UInt16>(srcBandData, srcBlockImgSize, dstRowLookUpTable, dstColLookUpTable, bufferSize, dstBandData, 0, null);
                        srcBandData = null;
                        using (IRasterBand band = dstImgRaster.GetRasterBand(i + 1 + beginBandIndex))
                        {
                            unsafe
                            {
                                fixed (UInt16* ptr = dstBandData)
                                {
                                    IntPtr bufferPtr = new IntPtr(ptr);
                                    int blockOffsetY = blockHeight * blockYNo;
                                    int blockOffsetX = blockWidth * blockXNo;
                                    band.Write(blockOffsetX, blockOffsetY, blockWidth, blockHeight, bufferPtr, enumDataType.UInt16, bufferSize.Width, bufferSize.Height);
                                }
                            }
                        }
                    }
                    srcBandData = null;
                    dstBandData = null;
                    GC.Collect();
                    ReleaseZenithData();
                    Size srcBufferSize = new Size(srcBlockImgWidth, srcBlockImgHeight);
                    ProjectAngle(bufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                    ProjectExtBands(bufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                    dstRowLookUpTable = null;
                    dstColLookUpTable = null;
                }
            }
        }

        protected void TryApplyGeoInterceptSlope(double[] xs, double[] ys)
        {
            if (_geoIntercept != 0d || _geoSlope != 1d)
            {
                for (int i = 0; i < xs.Length; i++)
                {
                    xs[i] = xs[i] * _geoSlope + _geoIntercept;
                }
                for (int i = 0; i < ys.Length; i++)
                {
                    ys[i] = ys[i] * _geoSlope + _geoIntercept;
                }
            }
        }

        /// <summary>
        /// 按照指定的偏移量读取
        /// </summary>
        /// <param name="rasterBand"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        protected double[] ReadBlockDatas(IRasterBand rasterBand, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            double[] bandData = new double[blockWidth * blockHeight];
            GCHandle h = GCHandle.Alloc(bandData, GCHandleType.Pinned);
            try
            {
                IntPtr bufferPtr = h.AddrOfPinnedObject();
                rasterBand.Read(xOffset, yOffset, blockWidth, blockHeight, bufferPtr, enumDataType.Double, blockWidth, blockHeight);
                return bandData;
            }
            finally
            {
                h.Free();
            }
        }

        /// <summary>
        /// 按照指定的区域采样读取
        /// </summary>
        /// <param name="rasterBand"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        internal double[] ReadSampleDatas(IRasterBand rasterBand, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            double[] bandData = new double[blockWidth * blockHeight];
            GCHandle h = GCHandle.Alloc(bandData, GCHandleType.Pinned);
            try
            {
                IntPtr bufferPtr = h.AddrOfPinnedObject();
                rasterBand.Read(xOffset, yOffset, rasterBand.Width, rasterBand.Height, bufferPtr, enumDataType.Double, blockWidth, blockHeight);
                return bandData;
            }
            finally
            {
                h.Free();
            }
        }

        protected virtual void TryReadZenithData(int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            return;
        }

        protected virtual void ReleaseZenithData()
        {
            return;
        }

        private void SetAoiIndex(int[] aoiIndex, ushort[] dstRowLookUpTable, ushort value)
        {
            if (aoiIndex != null && aoiIndex.Length != 0)
            {
                foreach (int aoi in dstRowLookUpTable)
                {
                    dstRowLookUpTable[aoi] = value;
                }
            }
        }

        private int[] GetAoiIndex(Size bufferSize, PrjEnvelope blockEnvelope, ISpatialReference dstSpatialRef)
        {//
            return null;
        }

        protected abstract void DoRadiation(IRasterDataProvider srcImgRaster, int i, ushort[] srcBandData, float[] solarZenithData, Size srcBlockImgSize, Size angleSize);

        /// <summary>
        /// 修改分块判断综合输出数据尺寸和经纬度数据尺寸
        /// </summary>
        /// <param name="size"></param>
        /// <param name="geoSize"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="blockXNum"></param>
        /// <param name="blockYNum"></param>
        /// <param name="blockWidth"></param>
        /// <param name="blockHeight"></param>
        protected virtual void GetBlockNumber(Size size, Size geoSize, float xScale, float yScale, out int blockXNum, out int blockYNum, out int blockWidth, out int blockHeight)
        {
            if (size.Width <= 0 )
                throw new Exception("指定的投影区域的宽度过窄:" + _dstEnvelope.Width + ",无法满足" + _outResolutionX + "分辨率输出的最小要求！宽度" + size.Width + "<1");
            if (size.Height <= 0)
                throw new Exception("指定的投影区域的高度度过窄:" + _dstEnvelope.Height + ",无法满足" + _outResolutionY + "分辨率输出的最小要求！高度" + size.Height + "<1");
            int w = size.Width;
            int h = size.Height;
            blockXNum = 1;
            blockYNum = 1;
            blockWidth = w;
            blockHeight = h;
            int MaxX = (int)(18000 * xScale);
            int MaxY = (int)(1000 * yScale);
            uint mem = MemoryHelper.GetAvalidPhyMemory();      //系统剩余内存
            int byteArrayCount = (2 + 2 + 2 + 2 * 2 + 2 * 8) / 2;  //原数据一个int16，目标数据一个int16，用于订正的天顶角一个int16，查找表2*UInt16(可能还需要两个经纬度数据集)，不过如果用到，应该在之前已经申请
            long maxByteArray = MemoryHelper.GetMaxArrayLength<UInt16>(byteArrayCount);
            double memMB = ((maxByteArray * byteArrayCount * maxByteArray) / (1024.0d * 1024)) * 2;
            if (memMB < 200)
                throw new Exception("系统可申请系统资源（小于200MB）不足以完成该操作，请释放部分资源后再试。");
            double canUsemem = mem > maxByteArray ? maxByteArray : mem;
            //#if !WIN64
            //            canUsemem = mem > 1800 * 1024.0f * 1024 - workingSet64 ? 1800 * 1024.0f * 1024 - workingSet64 : mem - workingSet64;
            //#endif
            if (geoSize.Width >= 8000 && geoSize.Height >= 8000)//针对FY3C MERSI250米轨道数据特殊处理。
            {
                blockXNum = 2;
                MaxX = MaxX / 2;
                MaxY = (int)(canUsemem / 100f / w * xScale * yScale);
            }
            else
                MaxY = (int)(canUsemem / w * xScale * yScale);//有些geoSize比较大，比如MERSI250M

            if (size.Width * size.Height <= MaxX * MaxY)
                return;
            while (blockWidth > MaxX)
            {
                blockXNum++;
                blockWidth = (int)Math.Floor((double)w / blockXNum);
            }
            while (blockHeight > MaxY)
            {
                blockYNum++;
                blockHeight = (int)Math.Floor((double)h / blockYNum);
            }
#if test
            Console.WriteLine("blockXNum,blockXNum{0},{1}", blockXNum, blockYNum);
#endif
        }

        protected string BandNameString(int[] outBandNos)
        {
            return BandNameString(null, outBandNos);
        }

        protected string BandNameString(IRasterDataProvider srcRaster, int[] outBandNos)
        {
            if (outBandNos == null || outBandNos.Length == 0)
                return "";
            if (srcRaster != null && srcRaster is IBandNameRaster)
            { 
                int[] bandNams = null;
                if ((srcRaster as IBandNameRaster).TryGetBandNameFromBandNos(outBandNos, out bandNams))
                {
                    return string.Join<int>(",", bandNams);
                }
                else
                    return string.Join(",", outBandNos);
            }
            else
            {
                return string.Join(",", outBandNos);
            }
        }

        protected virtual void TrySetLeftRightInvalidPixel(object[] extArgs)
        {
            if (extArgs != null && extArgs.Length != 0)
            {
                foreach (object extArg in extArgs)
                {
                    if (extArg is string)
                    {
                        string strExtArg = extArg as string;
                        Match match = _leftRightInvalidArgReg.Match(strExtArg);
                        if (match.Success)
                        {
                            string left = match.Groups["left"].Value;
                            string right = match.Groups["right"].Value;
                            int.TryParse(left, out _left);
                            int.TryParse(right, out _right);
                        }
                    }
                }
            }
        }

        #region STATIC

        public static IFileProjector GetFileProjectByName(string name)
        {
            if (_loadedFileProjectos == null)
                _loadedFileProjectos = LoadAllFileProjectors();
            if (_loadedFileProjectos == null || _loadedFileProjectos.Length == 0)
                return null;
            foreach (IFileProjector prj in _loadedFileProjectos)
                if (prj.Name == name)
                    return prj;
            return null;
        }

        private static IFileProjector[] _loadedFileProjectos = null;
        public static IFileProjector[] LoadAllFileProjectors()
        {
            if (_loadedFileProjectos == null)
            {
                string[] files = MefConfigParser.GetAssemblysByCatalog("投影");
                using (IComponentLoader<IFileProjector> loader = new ComponentLoader<IFileProjector>())
                    _loadedFileProjectos = loader.LoadComponents(files);
            }
            return _loadedFileProjectos;
        }
        #endregion

        public bool HasVaildEnvelope(IRasterDataProvider geoRaster, PrjEnvelope validEnv, ISpatialReference dstSpatialRef)
        {
            if (geoRaster == null)
                throw new ArgumentNullException("locationRaster", "参数[经纬度数据文件]不能为空");
            //double[] xs, ys;
            //Size locationSize;
            //ReadLocations(locationRaster, out xs, out ys, out locationSize);
            //return _rasterProjector.HasVaildEnvelope(xs, ys, validEnv, null, envSpatialReference);

            double[] xs = null;
            double[] ys = null;
            Size geoSize;
            Size maxGeoSize = new Size(1024, 1024);
            IRasterBand longitudeBand, latitudeBand;
            ReadLocations(geoRaster, out longitudeBand, out latitudeBand);//GetGeoBand
            Size srcLocationSize = new Size(longitudeBand.Width, longitudeBand.Height);
            ReadLocations(longitudeBand, latitudeBand, maxGeoSize, out xs, out ys, out geoSize);
            return _rasterProjector.HasVaildEnvelope(xs, ys, validEnv, null, dstSpatialRef);
        }

        public bool ValidEnvelope(IRasterDataProvider locationRaster, PrjEnvelope validEnv, SpatialReference envSpatialReference, out double validRate, out PrjEnvelope outEnv)
        {
            if (locationRaster == null)
                throw new ArgumentNullException("locationRaster", "参数[经纬度数据文件]不能为空");
            Size srcSize = new Size(locationRaster.Width, locationRaster.Height);
            double[] xs, ys;
            Size locationSize;
            ReadLocations(locationRaster, out xs, out ys, out locationSize);
            return _rasterProjector.VaildEnvelope(xs, ys, validEnv, SpatialReference.GetDefault(), envSpatialReference, out validRate, out outEnv);
        }

        protected abstract void ReadLocations(IRasterDataProvider srcRaster, out double[] xs, out double[] ys, out Size locationSize);

        protected void TryResetLonlatForLeftRightInvalid(double[] longitudes, double[] latitudes, Size locationSize, Size geoRasterSize)
        {
            int height = locationSize.Height;
            int width = locationSize.Width;
            if (_left + _right >= width)
                return;
            double sample = locationSize.Width * 1d / geoRasterSize.Width;
            if (_left > 0)
            {
                int left = (int)((_left - 1) * sample);
                for (int i = 0; i < left; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        longitudes[j * width + i] = double.MinValue;
                        latitudes[j * width + i] = double.MinValue;
                    }
                }
            }
            if (_right > 0)
            {
                //int right = _right - 1;
                int right = (int)((_right - 1) * sample);
                for (int i = width - right; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        longitudes[j * width + i] = double.MinValue;
                        latitudes[j * width + i] = double.MinValue;
                    }
                }
            }
        }

        protected void TryResetLonlatForLeftRightInvalid(double[] longitudes, double[] latitudes, Size locationSize)
        {
            int height = locationSize.Height;
            int width = locationSize.Width;
            if (_left + _right >= width)
                return;
            if (_left > 0)
            {
                int left = _left - 1;
                for (int i = 0; i < left; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        longitudes[j * width + i] = double.MinValue;
                        latitudes[j * width + i] = double.MinValue;
                    }
                }
            }
            if (_right > 0)
            {
                int right = _right - 1;
                for (int i = width - right; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        longitudes[j * width + i] = double.MinValue;
                        latitudes[j * width + i] = double.MinValue;
                    }
                }
            }
        }

        protected virtual void ReadLocations(IRasterDataProvider geoRaster, out IRasterBand longitudeBand, out IRasterBand latitudeBand)
        {
            IBandProvider srcbandpro = geoRaster.BandProvider as IBandProvider;
            IRasterBand[] lonsBands = srcbandpro.GetBands("Longitude");
            IRasterBand[] latBands = srcbandpro.GetBands("Latitude");
            if (lonsBands == null || latBands == null || lonsBands.Length == 0 || latBands.Length == 0 || lonsBands[0] == null || latBands[0] == null)
                throw new Exception("获取经纬度数据集失败");
            longitudeBand = lonsBands[0];
            latitudeBand = latBands[0];
        }

        /// <summary> 
        /// 准备定位信息,计算投影后的值，并计算范围
        /// </summary>
        protected void ReadyLocations(IRasterDataProvider geoRaster, ISpatialReference srcSpatialRef, ISpatialReference dstSpatialRef, out Size geoSize,
            out double[] xs, out double[] ys, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            ReadLocations(geoRaster, out xs, out ys, out geoSize);
            TryResetLonlatForLeftRightInvalid(xs, ys, geoSize);
            PrjEnvelope maskEnvelope = new PrjEnvelope(-180d, 180d, -90d, 90d);
            _rasterProjector.ComputeDstEnvelope(srcSpatialRef, xs, ys, geoSize, dstSpatialRef, out maxPrjEnvelope, progressCallback);
        }

        /// <summary>
        /// 指定采样大小，读取经纬度数据集数据
        /// </summary>
        /// <param name="longitudeBand"></param>
        /// <param name="latitudeBand"></param>
        /// <param name="maxGeoSize"></param>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="locationSize"></param>
        protected void ReadLocations(IRasterBand longitudeBand, IRasterBand latitudeBand, Size maxGeoSize, out double[] xs, out double[] ys, out Size locationSize)
        {
            int sampleWidth = longitudeBand.Width;
            int sampleHeight = longitudeBand.Height;
            if (maxGeoSize.Width < sampleWidth)
                sampleWidth = maxGeoSize.Width;
            if (maxGeoSize.Height < sampleHeight)
                sampleHeight = maxGeoSize.Height;
            Size sampleSize = new Size(sampleWidth, sampleHeight);
            ReadBandData(longitudeBand, sampleSize, out xs);
            ReadBandData(latitudeBand, sampleSize, out ys);
            locationSize = sampleSize;
        }

        /// <summary>
        /// 按照指定的采样比例大小读取数据
        /// </summary>
        /// <param name="band"></param>
        /// <param name="sampleSize"></param>
        /// <param name="bandData"></param>
        /// <param name="srcSize"></param>
        private void ReadBandData(IRasterBand band, Size sampleSize, out double[] bandData)
        {
            int width = band.Width;
            int height = band.Height;
            int sampleWidth = sampleSize.Width;
            int sampleHeight = sampleSize.Height;
            bandData = new Double[sampleSize.Width * sampleSize.Height];
            unsafe
            {
                fixed (Double* ptrLong = bandData)
                {
                    IntPtr bufferPtrLong = new IntPtr(ptrLong);
                    band.Read(0, 0, width, height, bufferPtrLong, enumDataType.Double, sampleWidth, sampleHeight);
                }
            }
        }

        protected void CheckAndCreateDir(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
