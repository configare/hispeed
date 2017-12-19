using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RasterProject;
using GeoDo.Project;
using System.Drawing;
using System.Threading.Tasks;
using GeoDo.RSS.DF.NOAA;
using System.IO;

namespace GeoDo.FileProject
{
    [Export(typeof(IFileProjector)), ExportMetadata("VERSION", "1")]
    public class FY1X_1A5FileProjector : FileProjector
    {
        private const double DEG_TO_RAD_P100 = 0.000174532925199432955; // (PI/180)/100;
        private const double C2 = 1.438833;
        private const double C1 = 1.1910659 / 100000;

        #region 定标变量
        /// <summary>
        /// 反射通道定标系数（业务用，通道1、2、3A），存储为：[行,3*5]//scale1，offset1，scale2，offset2，inflection
        /// band1业务用 斜率1、截距1、斜率2、截距2、交叉点
        /// band1测试用 斜率1、截距1、斜率2、截距2、交叉点
        /// band1发射前 斜率1、截距1、斜率2、截距2、交叉点
        /// band2业务用...
        /// band2测试用...
        /// band2发射前...
        /// 3A   业务用...
        /// </summary>
        private double[,] _refSB_Coeff = null;
        /// <summary>
        /// 发射通道定标系数（业务用，通道3B、4、5），存储为：[行,3*3] scale1，scale2，offset
        /// 3B业务用 scale1，scale2，offset
        /// 3B测试用 scale1，scale2，offset
        /// 3B发射前 scale1，scale2，offset
        /// 4 业务用 scale1，scale2，offset
        /// 4 测试用 scale1，scale2，offset
        /// 4 发射前 scale1，scale2，offset
        /// 5 业务用 scale1，scale2，offset
        /// 5 测试用 scale1，scale2，offset
        /// 5 发射前 scale1，scale2，offset
        /// </summary>
        private double[,] _emissive_Radiance_Coeff = null;
        /// <summary>
        /// 发射通道A、B、V系数
        /// 3B A、B、V
        /// 4  A、B、V
        /// 5  A、B、V
        /// </summary>
        private float[] _emmisive_BT_Coefficients = null;
        private bool _isDay = false;        //白天
        #endregion

        ISpatialReference _srcSpatialRef = null;
        //private Block _orbitBlock = null;              //当前投影范围，需要使用的原始轨道数据最小范围
        private IRasterDataDriver _outLdfDriver = null;
        private string _szDataFilename;
        //private IRasterDataProvider _solarZenithCacheRaster = null; 
        //protected short[] _sensorZenithData = null;

        //#region Session
        //PrjEnvelope _maxPrjEnvelope = null;
        //double[] _xs = null;    //存储的实际是计算后的值
        //double[] _ys = null;    //存储的实际是计算后的值
        //#endregion

        public FY1X_1A5FileProjector()
            : base()
        {
            _name = "FY1X_1A5";
            _fullname = "FY1X_1A5轨道文件投影";
            _rasterProjector = new RasterProjector();
            _srcSpatialRef = new SpatialReference(new GeographicCoordSystem());
            _supportAngles = new object[] { "SolarZenith", "SatelliteZenith", "RelativeAzimuth" };
            //_supportAngles = new object[] { "NOMSatelliteZenith", "NOMSunGlintAngle", "NOMSunZenith" };
        }

        public override bool IsSupport(string fileName)
        {
            return false;
        }

        public override void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster");
            if (prjSettings == null)
                throw new ArgumentNullException("prjSettings");
            if (progressCallback != null)
                progressCallback(0, "准备相关参数");
            _dstSpatialRef = dstSpatialRef;
            if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
            {
                MemoryHelper.MemoryNeed(200, 1536);
            }
            else
            {
                MemoryHelper.MemoryNeed(200, 1536);     //剩余900MB,已使用1.2GB
            }
            try
            {
                NOAA_PrjSettings noaaPrjSettings = prjSettings as NOAA_PrjSettings;
                TryCreateDefaultArgs(srcRaster, noaaPrjSettings, ref dstSpatialRef);
                _isSensorZenith = noaaPrjSettings.IsSensorZenith;
                DoSession(srcRaster, dstSpatialRef, noaaPrjSettings, progressCallback);
                if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
                {
                    prjSettings.OutEnvelope = _maxPrjEnvelope;
                    _orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = srcRaster.Width - 1, yEnd = srcRaster.Height - 1 };
                }
                else
                {
                    GetEnvelope(_xs, _ys, srcRaster.Width, srcRaster.Height, prjSettings.OutEnvelope, out _orbitBlock);
                    if (_orbitBlock == null || _orbitBlock.Width <= 0 || _orbitBlock.Height <= 0)
                        throw new Exception("数据不在目标区间内");
                    float invalidPresent = (_orbitBlock.Width * _orbitBlock.Height * 1.0F) / (srcRaster.Width * srcRaster.Height);
                    if (invalidPresent < 0.0001f)
                        throw new Exception("数据占轨道数据比例太小，有效率" + invalidPresent * 100 + "%");
                    if (invalidPresent > 0.60)
                        _orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = srcRaster.Width - 1, yEnd = srcRaster.Height - 1 };
                }
                if (dstSpatialRef.ProjectionCoordSystem == null && _maxPrjEnvelope.MaxX > 180 && _maxPrjEnvelope.MinX < -180 && _maxPrjEnvelope.MaxY > 90 && _maxPrjEnvelope.MinY < -90)
                    throw new Exception("读取FY1X 1a5经纬度不在合理范围内[" + _maxPrjEnvelope.ToString() + "]");
                //if (dstSpatialRef.ProjectionCoordSystem == null && (prjSettings.OutEnvelope.MaxY > 80 || prjSettings.OutEnvelope.MaxY < -80))
                //    throw new Exception(string.Format("高纬度数据[>80]，不适合投影为等经纬度数据[{0}]", _maxPrjEnvelope));
                PrjEnvelope envelops = prjSettings.OutEnvelope;
                if (!envelops.IntersectsWith(_maxPrjEnvelope))
                    throw new Exception("数据不在目标区间内");
                float outResolutionX = prjSettings.OutResolutionX;
                float outResolutionY = prjSettings.OutResolutionY;
                int dstBandCount = prjSettings.OutBandNos.Length;
                Size outSize = prjSettings.OutSize;
                string[] angleOptions = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + dstSpatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + prjSettings.OutEnvelope.MinX + "," + prjSettings.OutEnvelope.MaxY + "}:{" + outResolutionX + "," + outResolutionY + "}"
                        };
                string outfilename = prjSettings.OutPathAndFileName;
                ReadyAngleFiles(srcRaster, outfilename, prjSettings, outSize, angleOptions);
                Project(srcRaster, noaaPrjSettings, dstSpatialRef, progressCallback);
            }
            catch
            {
                EndSession();
                TryDeleteCurCatch();
                throw;
            }
            finally
            {
                if (_curSession == null)
                {
                    EndSession();
                    if (prjSettings.IsClearPrjCache)
                        TryDeleteCurCatch();
                }
            }
        }

        public override void EndSession()
        {
            base.EndSession();
            _xs = null;
            _ys = null;
            _refSB_Coeff = null;
            _emissive_Radiance_Coeff = null;
            _emmisive_BT_Coefficients = null;
            if (_solarZenithCacheRaster != null)
            {
                _solarZenithCacheRaster.Dispose();
                _solarZenithCacheRaster = null;
            }
            if (_outLdfDriver != null)
            {
                _outLdfDriver.Dispose();
                _outLdfDriver = null;
            }
        }

        private void DoSession(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, NOAA_PrjSettings prjSettings, Action<int, string> progressCallback)
        {
            if (_curSession == null || _curSession != srcRaster || _isBeginSession)
            {
                Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                ReadyLocations(srcRaster, dstSpatialRef, srcSize, out _xs, out _ys, out _maxPrjEnvelope, progressCallback);
                if (progressCallback != null)
                    progressCallback(4, "准备亮温计算参数");
                if (prjSettings.IsRadiation)
                    ReadyRadiationArgs(srcRaster);
                if (progressCallback != null)
                    progressCallback(5, "准备亮温计算参数");
                if (prjSettings.IsSolarZenith && prjSettings.IsRadiation)
                {
                    _szDataFilename = GetSolarZenithCacheFilename(srcRaster.fileName);
                    if (!File.Exists(_szDataFilename))
                        ReadySolarZenithArgsToFile(srcRaster);
                    else
                        _solarZenithCacheRaster = GeoDataDriver.Open(_szDataFilename) as IRasterDataProvider;
                    if (prjSettings.IsSensorZenith)
                    {
                        ReadySensorZenith(srcRaster);
                    }
                }
                _isBeginSession = false;
            }
        }

        private void ReadySensorZenith(IRasterDataProvider srcRaster)
        {
            _sensorSenithRaster = srcRaster;//
            IRasterBand[] bands = srcRaster.BandProvider.GetBands("SatelliteZenith");
            if (bands != null || bands.Length != 1)
                _sensorSenithBand = bands[0];
        }

        private void Project(IRasterDataProvider srcRaster, NOAA_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            PrjEnvelope envelops = prjSettings.OutEnvelope;
            if (envelops.IntersectsWith(_maxPrjEnvelope))
            {
                switch (prjSettings.OutFormat)
                {
                    case "LDF":
                        ProjectToLDF(srcRaster, prjSettings, dstSpatialRef, progressCallback);
                        break;
                    case "MEMORY":
                    default:
                        throw new NotSupportedException(string.Format("暂不支持的输出格式", prjSettings.OutFormat));
                }
            }
            else
            {
                throw new Exception("数据不在目标区间内");
            }
        }

        /// <summary>
        /// 0、先生成目标文件，以防止目标空间不足。
        /// 1、计算查找表
        /// 2、读取通道数据
        /// 3、计算通道数据亮温
        /// 4、投影通道。
        /// </summary>
        private void ProjectToLDF(IRasterDataProvider srcRaster, NOAA_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            string outFormat = prjSettings.OutFormat;
            string outfilename = prjSettings.OutPathAndFileName;
            string dstProj4 = dstSpatialRef.ToProj4String();
            int[] outBandNos = prjSettings.OutBandNos;
            int dstBandCount = outBandNos.Length;
            Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
            Size dstSize = prjSettings.OutSize;
            Size srcJdSize = srcSize;
            using (IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
            {
                PrjEnvelope dstEnvelope = prjSettings.OutEnvelope;

                List<string> opts = new List<string>();
                opts.AddRange(new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF=" + dstSpatialRef.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + dstEnvelope.MinX + "," + dstEnvelope.MaxY + "}:{" + prjSettings.OutResolutionX + "," + prjSettings.OutResolutionY + "}",
                "BANDNAMES="+ BandNameString(prjSettings.OutBandNos),
                "SENSOR=AVHRR"
                });
                if (srcRaster.DataIdentify != null)
                {
                    string satellite = srcRaster.DataIdentify.Satellite;
                    DateTime dt = srcRaster.DataIdentify.OrbitDateTime;
                    bool asc = srcRaster.DataIdentify.IsAscOrbitDirection;
                    if (!string.IsNullOrWhiteSpace(satellite))
                    {
                        opts.Add("SATELLITE=" + satellite);
                    }
                    if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                        opts.Add("DATETIME=" + dt.ToString("yyyy/MM/dd HH:mm"));
                    opts.Add("ORBITDIRECTION=" + (asc ? "ASC" : "DESC"));
                }
                if (progressCallback != null)
                    progressCallback(6, "生成输出文件");
                using (IRasterDataProvider prdWriter = drv.Create(outfilename, dstSize.Width, dstSize.Height, dstBandCount, enumDataType.UInt16, opts.ToArray()) as IRasterDataProvider)
                {
                    float outResolutionX = prjSettings.OutResolutionX;
                    float outResolutionY = prjSettings.OutResolutionY;
                    Size outSize = dstEnvelope.GetSize(outResolutionX, outResolutionY);
                    int blockXNum = 0;
                    int blockYNum = 0;
                    int blockWidth = 0;
                    int blockHeight = 0;
                    GetBlockNumber(outSize, out blockXNum, out blockYNum, out blockWidth, out blockHeight);
                    Size dstBlockSize = new Size(blockWidth, blockHeight);
                    UInt16[] dstRowBlockLUT = new UInt16[blockWidth * blockHeight];
                    UInt16[] dstColBlockLUT = new UInt16[blockWidth * blockHeight];
                    int blockCount = blockYNum * blockXNum;
                    progress = 0;
                    progressCount = blockCount * (dstBandCount + (_angleBands == null ? 0 : _angleBands.Length));
                    percent = 0;
                    for (int blockYIndex = 0; blockYIndex < blockYNum; blockYIndex++)
                    {
                        for (int blockXIndex = 0; blockXIndex < blockXNum; blockXIndex++)
                        {
                            PrjEnvelope blockEnvelope = null;
                            Block orbitBlock = null;         //经纬度数据集，计算轨道数据范围偏移
                            double[] blockOrbitXs = null;
                            double[] blockOrbitYs = null;
                            if (blockCount == 1)            //没分块的情况
                            {
                                orbitBlock = _orbitBlock;
                                if (_orbitBlock.Width == srcJdSize.Width && _orbitBlock.Height == srcJdSize.Height)
                                {
                                    blockOrbitXs = _xs;
                                    blockOrbitYs = _ys;
                                }
                                else                        //源
                                {
                                    GetBlockDatas(_xs, _ys, srcJdSize.Width, srcJdSize.Height, orbitBlock.xOffset, orbitBlock.yBegin, orbitBlock.Width, orbitBlock.Height, out blockOrbitXs, out blockOrbitYs);
                                }
                                blockEnvelope = dstEnvelope;
                            }
                            else
                            {
                                //当前块的四角范围
                                double blockMinX = dstEnvelope.MinX + blockWidth * outResolutionX * blockXIndex;
                                double blockMaxX = blockMinX + blockWidth * outResolutionX;
                                double blockMaxY = dstEnvelope.MaxY - blockHeight * outResolutionY * blockYIndex;
                                double blockMinY = blockMaxY - blockHeight * outResolutionY;
                                blockEnvelope = new PrjEnvelope(blockMinX, blockMaxX, blockMinY, blockMaxY);
                                //根据当前输出块，反推出对应的源数据块起始行列
                                GetEnvelope(_xs, _ys, srcJdSize.Width, srcJdSize.Height, blockEnvelope, out orbitBlock);
                                if (orbitBlock.Width <= 0 || orbitBlock.Height <= 0) //当前分块不在图像内部
                                {
                                    progress += dstBandCount;
                                    continue;
                                }
                                GetBlockDatas(_xs, _ys, srcJdSize.Width, srcJdSize.Height, orbitBlock.xOffset, orbitBlock.yBegin, orbitBlock.Width, orbitBlock.Height, out blockOrbitXs, out blockOrbitYs);
                            }
                            float[] solarZenithData = null;
                            if (prjSettings.IsRadiation && prjSettings.IsSolarZenith)
                            {
                                if (File.Exists(_szDataFilename))
                                    ReadBandData(out solarZenithData, _solarZenithCacheRaster, 1, orbitBlock.xOffset, orbitBlock.yBegin, orbitBlock.Width, orbitBlock.Height);
                                TryReadZenithData(orbitBlock.xOffset, orbitBlock.yBegin, orbitBlock.Width, orbitBlock.Height);
                            }
                            Size orbitBlockSize = new Size(orbitBlock.Width, orbitBlock.Height);
                            _rasterProjector.ComputeIndexMapTable(blockOrbitXs, blockOrbitYs, orbitBlockSize, dstBlockSize, blockEnvelope, _maxPrjEnvelope, out dstRowBlockLUT, out dstColBlockLUT, null);
                            //执行投影
                            UInt16[] srcBandData = new UInt16[orbitBlock.Width * orbitBlock.Height];
                            UInt16[] dstBandData = new UInt16[blockWidth * blockHeight];
                            for (int i = 0; i < dstBandCount; i++)      //读取原始通道值，投影到目标区域
                            {
                                if (progressCallback != null)
                                {
                                    progress++;
                                    percent = progress * 100 / progressCount;
                                    progressCallback(percent, string.Format("投影完成{0}%", percent));
                                }
                                int bandNo = outBandNos[i];
                                ReadBandData(srcBandData, srcRaster, bandNo, orbitBlock.xOffset, orbitBlock.yBegin, orbitBlock.Width, orbitBlock.Height);
                                if (prjSettings.IsRadiation)
                                {
                                    DoRadiation(srcBandData, orbitBlock.xOffset, orbitBlock.yBegin, orbitBlock.Size, bandNo, prjSettings.IsRadiation, prjSettings.IsSolarZenith, solarZenithData);
                                }
                                _rasterProjector.Project<UInt16>(srcBandData, orbitBlock.Size, dstRowBlockLUT, dstColBlockLUT, dstBlockSize, dstBandData, 0, null);
                                IRasterBand band = prdWriter.GetRasterBand(i + 1);
                                {
                                    unsafe
                                    {
                                        fixed (UInt16* ptr = dstBandData)
                                        {
                                            IntPtr bufferPtr = new IntPtr(ptr);
                                            int blockOffsetY = blockYIndex * dstBlockSize.Height;
                                            int blockOffsetX = blockXIndex * dstBlockSize.Width;
                                            band.Write(blockOffsetX, blockOffsetY, blockWidth, blockHeight, bufferPtr, enumDataType.UInt16, blockWidth, blockHeight);
                                        }
                                    }
                                }
                            }
							ReleaseZenithData();
                            srcBandData = null;
                            dstBandData = null;
                            blockOrbitXs = null;
                            blockOrbitYs = null;
                            Size srcBufferSize = new Size(orbitBlock.Width, orbitBlock.Height);
                            ProjectAngle(dstBlockSize, srcBufferSize, blockWidth, blockHeight, blockYIndex, blockXIndex, orbitBlock, dstRowBlockLUT, dstColBlockLUT, progressCallback);
                        }
                    }
                    dstRowBlockLUT = null;
                    dstColBlockLUT = null;
                }
            }
        }

        protected short[] _sensorZenithData = null;
        protected override void TryReadZenithData(int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            //亮温临边变暗订正,读取卫星天顶角数据。
            if (_isSensorZenith && _sensorSenithBand != null)
            {
                _sensorZenithData = ReadBandData(_sensorSenithBand, xOffset, yOffset, blockWidth, blockHeight);
            }
        }

        protected override void ReleaseZenithData()
        {
            _sensorZenithData = null;
        }

        private void GetBlockNumber(Size size, out int blockXNum, out int blockYNum, out int blockWidth, out int blockHeight)
        {
            int w = size.Width;
            int h = size.Height;
            blockXNum = 1;
            blockYNum = 1;
            blockWidth = w;
            blockHeight = h;
            int MaxX = 7000;
            int MaxY = 2000;
            uint mem = MemoryHelper.GetAvalidPhyMemory();      //系统剩余内存
            long workingSet64 = MemoryHelper.WorkingSet64();   //为该进程已分配内存
            if (mem < 200 * 1024.0f * 1024)
                throw new Exception("当前系统资源不足以完成该操作，请释放部分资源后再试。");
            double usemem = mem;//;
#if !WIN64
            usemem = mem > 1800 * 1024.0f * 1024 - workingSet64 ? 1800 * 1024.0f * 1024 - workingSet64 : mem - workingSet64;
#endif
            MaxY = (int)(usemem / 100 / w );
            MaxY = MaxY < 2000 ? 2000 : MaxY;
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
        }

        private void ReadBandData(UInt16[] bandData, IRasterDataProvider srcRaster, int bandNumber, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            IRasterBand latBand = srcRaster.GetRasterBand(bandNumber);//目前没加using，应为加了有错误
            {
                unsafe
                {
                    fixed (UInt16* ptr = bandData)
                    {
                        IntPtr bufferptr = new IntPtr(ptr);
                        latBand.Read(xOffset, yOffset, blockWidth, blockHeight, bufferptr, enumDataType.UInt16, blockWidth, blockHeight);
                    }
                }
            }
        }

        private void ReadBandData(UInt16[] bandData, IRasterBand latBand, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            try
            {
                unsafe
                {
                    fixed (UInt16* ptr = bandData)
                    {
                        IntPtr bufferptr = new IntPtr(ptr);
                        latBand.Read(xOffset, yOffset, blockWidth, blockHeight, bufferptr, enumDataType.UInt16, blockWidth, blockHeight);
                    }
                }
            }
            finally
            {
            }
        }

        /// <summary> 
        /// 辐射值计算
        /// </summary>
        private void DoRadiation(ushort[] srcBandData,int xoffset, int yoffset, Size srcSize, int bandNumber, bool isRadiation, bool isSolarZenith, float[] solarZenithData)
        {
            if (!isRadiation)
                return;
        }

        /// <summary>
        /// 读取通道值
        /// </summary>
        private void ReadBandData(UInt16[] bandData, IRasterDataProvider srcRaster, int bandNumber, Size srcSize)
        {
            IRasterBand latBand = srcRaster.GetRasterBand(bandNumber);
            unsafe
            {
                fixed (UInt16* ptr = bandData)
                {
                    IntPtr bufferptr = new IntPtr(ptr);
                    latBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferptr, enumDataType.UInt16, srcSize.Width, srcSize.Height);
                }
            }
        }

        /// <summary> 
        /// 准备定位信息,计算投影后的值，并计算范围
        /// </summary>
        private void ReadyLocations(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, Size srcSize,
            out double[] xs, out double[] ys, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(1, "读取并插值经度数据集");
            Size locationSize;
            ReadLocations(srcRaster, out xs, out ys, out locationSize);
            TryResetLonlatForLeftRightInvalid(xs, ys, locationSize);
            if (xs == null || xs == null)
                throw new Exception("读取经纬度数据失败");
            if (progressCallback != null)
                progressCallback(3, "预处理经纬度数据集");
            _rasterProjector.ComputeDstEnvelope(_srcSpatialRef, xs, ys, srcSize, dstSpatialRef, out maxPrjEnvelope, progressCallback);
        }

        //读取定位信息(经纬度数据集)
        protected override void ReadLocations(IRasterDataProvider srcRaster, out double[] longitudes, out double[] latitudes, out Size locationSize)
        {
            longitudes = null;
            latitudes = null;
            IRasterDataProvider _longitudeRaster = null;
            IRasterDataProvider _latitudeRaster = null;
            try
            {
                string longCacheFilename = GetCacheFilename(srcRaster.fileName, "longitude.ldf");
                string latCacheFilename = GetCacheFilename(srcRaster.fileName, "latitude.ldf");
                if (File.Exists(longCacheFilename))
                {
                    _longitudeRaster = GeoDataDriver.Open(longCacheFilename) as IRasterDataProvider;
                    longitudes = ReadBandData(_longitudeRaster, 1);
                    locationSize = new Size(_longitudeRaster.Width, _longitudeRaster.Height);
                }
                else
                {
                    longitudes = ReadBandByName(srcRaster, "Longitude");
                    WriteCacheFile(longitudes, longCacheFilename, srcRaster.Width, srcRaster.Height, out _longitudeRaster);
                    locationSize = new Size(srcRaster.Width, srcRaster.Height);
                }
                if (File.Exists(latCacheFilename))
                {
                    _latitudeRaster = GeoDataDriver.Open(latCacheFilename) as IRasterDataProvider;
                    latitudes = ReadBandData(_latitudeRaster, 1);
                }
                else
                {
                    latitudes = ReadBandByName(srcRaster, "Latitude");
                    WriteCacheFile(latitudes, latCacheFilename, srcRaster.Width, srcRaster.Height, out _latitudeRaster);
                }
            }
            finally
            {
                if (_longitudeRaster != null)
                    _longitudeRaster.Dispose();
                if (_latitudeRaster != null)
                    _latitudeRaster.Dispose();
            }
        }

        private double[] ReadBandData(IRasterDataProvider raster, int bandNo)
        {
            using (IRasterBand band = raster.GetRasterBand(bandNo))
            {
                double[] data = new double[band.Width * band.Height];
                unsafe
                {
                    fixed (double* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Double, band.Width, band.Height);
                    }
                }
                return data;
            }
        }

        private IRasterBand WriteCacheFile(double[] data, string fileName, int width, int height, out IRasterDataProvider cacheWriter)
        {
            string[] options = new string[]
            {
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
            };
            if (_outLdfDriver == null)
                _outLdfDriver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            cacheWriter = _outLdfDriver.Create(fileName, width, height, 1, enumDataType.Double, options) as IRasterDataProvider;
            IRasterBand band = cacheWriter.GetRasterBand(1);
            {
                unsafe
                {
                    fixed (double* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Write(0, 0, width, height, bufferPtr, enumDataType.Double, width, height);
                    }
                }
            }
            return band;
        }

        private Double[] ReadBandByName(IRasterDataProvider srcRaster, string bandName)
        {
            IBandProvider bandPrd = srcRaster.BandProvider;
            IRasterBand[] bands = bandPrd.GetBands(bandName);
            if (bands == null || bands.Length == 0 || bands[0] == null)
                throw new Exception("读取波段" + bandName + "失败:无法获取该通道信息");
            try
            {
                using (IRasterBand band = bands[0])
                {
                    double[] data = new double[band.Width * band.Height];
                    unsafe
                    {
                        fixed (Double* ptr = data)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Double, band.Width, band.Height);
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取波段" + bandName + "失败:" + ex.Message, ex);
            }
        }

        private Int16[] ReadBandByNameToInt16(IRasterDataProvider srcRaster, string bandName)
        {
            IBandProvider bandPrd = srcRaster.BandProvider;
            IRasterBand[] bands = bandPrd.GetBands(bandName);
            if (bands == null || bands.Length == 0 || bands[0] == null)
                throw new Exception("读取波段" + bandName + "失败:无法获取该通道信息");
            try
            {
                using (IRasterBand band = bands[0])
                {
                    Int16[] data = new Int16[band.Width * band.Height];
                    unsafe
                    {
                        fixed (Int16* ptr = data)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Int16, band.Width, band.Height);
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取波段" + bandName + "失败:" + ex.Message, ex);
            }
        }

        #region 定标参数读取
        //准备[辐射定标]参数
        private void ReadyRadiationArgs(IRasterDataProvider srcRaster)
        {
        }

        private void ReadySolarZenithArgsToFile(IRasterDataProvider srcRaster)
        {
            //if (srcRaster == null)
            //    throw new ArgumentNullException("srcRaster", "获取太阳天顶角数据失败");
            //try
            //{
            //    IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            //    Size srcSize = new System.Drawing.Size(srcRaster.Width, srcRaster.Height);
            //    short[] readSolarZenithData = ReadDataSetToInt16(srcbandpro, srcSize, "SolarZenith", 0);
            //    int length = srcRaster.Width * srcRaster.Height;
            //    float[] saveSolarZenithData = new float[length];
            //    Parallel.For(0, length, index =>
            //    {
            //        if (readSolarZenithData[index] > 0 && readSolarZenithData[index] < 18000)
            //            saveSolarZenithData[index] = (float)(10.0f / Math.Cos(readSolarZenithData[index] * DEG_TO_RAD_P100));
            //        else
            //            saveSolarZenithData[index] = 0;
            //    });
            //    WriteData(saveSolarZenithData, _szDataFilename, srcSize.Width, srcSize.Height);
            //    saveSolarZenithData = null;
            //    readSolarZenithData = null;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("获取太阳天顶角数据失败", ex.InnerException);
            //}
        }

        private void WriteData(float[] data, string fileName, int width, int height)
        {
            string[] options = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
               };
            if (_outLdfDriver == null)
                _outLdfDriver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            IRasterDataProvider cacheWriter = _outLdfDriver.Create(fileName, width, height, 1, enumDataType.Float, options) as IRasterDataProvider;
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
            _solarZenithCacheRaster = cacheWriter;
        }

        private float[] ReadFileAttributeToFloat(IBandProvider srcbandpro, string AttrName, int length)
        {
            float[] value = new float[length];
            Dictionary<string, string> dsAtts = srcbandpro.GetAttributes();
            string refSbCalStr = dsAtts[AttrName];
            string[] refSbCals = refSbCalStr.Split(',');
            if (refSbCals.Length >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    value[i] = float.Parse(refSbCals[i]);
                }
                return value;
            }
            else
                return null;
        }

        private Int16[] ReadDataSetToInt16(IBandProvider srcbandpro, Size srcSize, string dataSetName, int bandIndex)
        {
            Int16[] data = new Int16[srcSize.Width * srcSize.Height];
            IRasterBand[] rasterBands = srcbandpro.GetBands(dataSetName);
            using (IRasterBand rasterBand = rasterBands[0])
            {
                unsafe
                {
                    fixed (Int16* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        rasterBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferPtr, enumDataType.Int16, srcSize.Width, srcSize.Height);
                    }
                }
            }
            return data;
        }

        private float[] ReadDataSetToSingle(IBandProvider srcbandpro, Size srcSize, string dataSetName, int bandIndex)
        {
            Single[] data = new Single[srcSize.Width * srcSize.Height];
            IRasterBand[] rasterBands = srcbandpro.GetBands(dataSetName);
            using (IRasterBand rasterBand = rasterBands[0])
            {
                unsafe
                {
                    fixed (Single* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        rasterBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferPtr, enumDataType.Float, srcSize.Width, srcSize.Height);
                    }
                }
            }
            return data;
        }

        #endregion

        public override IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, IRasterDataProvider dstRaster, int beginBandIndex, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override FilePrjSettings CreateDefaultPrjSettings()
        {
            return new FY3_VIRR_PrjSettings();
        }

        private void TryCreateDefaultArgs(IRasterDataProvider srcRaster, NOAA_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = _srcSpatialRef;
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
                prjSettings.OutFormat = "LDF";
            if (prjSettings.OutResolutionX == 0 || prjSettings.OutResolutionY == 0)
            {
                if (dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjSettings.OutResolutionX = 0.01F;
                    prjSettings.OutResolutionY = 0.01F;
                }
                else
                {
                    prjSettings.OutResolutionX = 1000F;
                    prjSettings.OutResolutionY = 1000F;
                }
            }
            if (prjSettings.OutBandNos == null || prjSettings.OutBandNos.Length == 0)
            {
                prjSettings.OutBandNos = new int[] { 1, 2, 3, 4};
            }
        }

        public override void ComputeDstEnvelope(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (srcRaster != null)
            {
                Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                double[] xs, ys;
                ReadyLocations(srcRaster, dstSpatialRef, srcSize, out xs, out ys, out maxPrjEnvelope, progressCallback);
            }
            else
            {
                maxPrjEnvelope = PrjEnvelope.Empty;
            }
        }

        protected override void DoRadiation(IRasterDataProvider srcImgRaster, int i, ushort[] srcBandData, float[] solarZenithData, Size srcBlockImgSize, Size angleSize)
        {
            throw new NotImplementedException();
        }
    }
}
