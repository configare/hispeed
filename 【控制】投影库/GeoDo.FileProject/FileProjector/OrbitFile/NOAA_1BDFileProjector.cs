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
    public class NOAA_1BDFileProjector : FileProjector
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

        public NOAA_1BDFileProjector()
            : base()
        {
            _name = "NOAA_1BD";
            _fullname = "NOAA_1BD轨道文件投影";
            _rasterProjector = new RasterProjector();
            _srcSpatialRef = new SpatialReference(new GeographicCoordSystem());
            _supportAngles = new object[] { "SolarZenith", "SatelliteZenith", "RelativeAzimuth" };
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
                    throw new Exception("读取NOAA 1bd经纬度不在合理范围内[" + _maxPrjEnvelope.ToString() + "]");
                if (dstSpatialRef.ProjectionCoordSystem == null && (prjSettings.OutEnvelope.MaxY > 80 || prjSettings.OutEnvelope.MaxY < -80))
                    throw new Exception(string.Format("高纬度数据[>80]，不适合投影为等经纬度数据[{0}]", _maxPrjEnvelope));
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
            switch (bandNumber)
            {
                case 1:
                case 2://1,2
                    RefSBRadiation(srcBandData, yoffset, srcSize, bandNumber - 1, isSolarZenith, solarZenithData);
                    break;
                case 3:     //第三个通道必须判断是3A还是3B
                    if (_isDay) //3A 按可见光
                        RefSBRadiation(srcBandData, yoffset, srcSize, bandNumber - 1, isSolarZenith, solarZenithData);
                    else        //3B 近红外
                        EmissiveRadiance3B(srcBandData, yoffset, srcSize, bandNumber - 3);
                    break;
                case 4:
                case 5:
                    EmissiveRadiance(srcBandData, yoffset, srcSize, bandNumber - 3);
                    break;
                default:
                    break;
            }
        }

        private void EmissiveRadiance3B(ushort[] srcBandData, int yoffset, Size srcSize, int coefIndex)
        {
            float A = _emmisive_BT_Coefficients[0];
            float B = _emmisive_BT_Coefficients[1];
            float v = _emmisive_BT_Coefficients[2];
            double v3 = v * v * v;
            int height = srcSize.Height;
            int width = srcSize.Width;
            int beginY = 0;
            int endY = height;
            Parallel.For(beginY, endY, (rowIndex) =>
            {
                double scale2 = _emissive_Radiance_Coeff[rowIndex + yoffset, coefIndex * 3 + 1];
                double offset = _emissive_Radiance_Coeff[rowIndex + yoffset, coefIndex * 3 + 2];
                double radiation;
                int index;
                for (int j = 0; j < width; j++)
                {
                    index = rowIndex * width + j;
                    radiation = (scale2 * srcBandData[index] + offset);
                    srcBandData[index] = (UInt16)(10 * (C2 * v / Math.Log(1 + C1 * v3 / radiation) - A) / B);
                }
            });
        }

        private void EmissiveRadiance(ushort[] srcBandData, int yoffset, Size srcSize, int coefIndex)
        {
            float A = _emmisive_BT_Coefficients[coefIndex * 3];
            float B = _emmisive_BT_Coefficients[coefIndex * 3 + 1];
            float v = _emmisive_BT_Coefficients[coefIndex * 3 + 2];
            double v3 = v * v * v;
            int height = srcSize.Height;
            int width = srcSize.Width;
            int beginY = 0;
            int endY = height;
            Parallel.For(beginY, endY, (rowIndex) =>
            {
                double scale = _emissive_Radiance_Coeff[rowIndex + yoffset, coefIndex * 3];
                double scale2 = _emissive_Radiance_Coeff[rowIndex + yoffset, coefIndex * 3 + 1];
                double offset = _emissive_Radiance_Coeff[rowIndex + yoffset, coefIndex * 3 + 2];
                double radiation;
                int index;

                double temperatureBB;
                double sensorZenith;
                double deltaT;
                for (int j = 0; j < width; j++)
                {
                    index = rowIndex * width + j;
                    radiation = (scale * srcBandData[index] * srcBandData[index] + scale2 * srcBandData[index] + offset);
                    temperatureBB = (C2 * v / Math.Log(1 + C1 * v3 / radiation) - A) / B;
                    //"临边变暗订正"。
                    if (_isSensorZenith && _sensorZenithData != null)
                    {
                        sensorZenith = _sensorZenithData[index] * 0.01d;
                        deltaT = temperatureBB + (Math.Pow(Math.E, 0.00012 * sensorZenith * sensorZenith) - 1) * (0.1072 * temperatureBB - 26.81);
                        srcBandData[index] = (UInt16)(deltaT * 10);
                    }
                    else
                        srcBandData[index] = (UInt16)(temperatureBB * 10);
                }
            });
        }

        private void RefSBRadiation(ushort[] srcBandData, int yoffset, Size srcSize, int bandIndex, bool isSolarZenith, float[] solarZenithData)
        {
            int width = srcSize.Width;
            int height = srcSize.Height;
            if (isSolarZenith)
            {
                int beginY = 0;
                int endY = height;
                Parallel.For(beginY, endY, (rowIndex) =>  //Noaa每行一套参数
                {
                    double scale = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5];
                    double offSet = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 1];
                    double scale2 = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 2];
                    double offSet2 = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 3];
                    double inflection = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 4];
                    double radiation = 0;
                    int index = 0;
                    for (int j = 0; j < width; j++)
                    {
                        index = rowIndex * width + j;
                        if (0 < srcBandData[index] && srcBandData[index] < inflection)//用两套斜率和截距
                            radiation = scale * srcBandData[index] + offSet;
                        else if (srcBandData[index] >= inflection)
                            radiation = scale2 * srcBandData[index] + offSet2;
                        //if (solarZenithData[index] > 0 && solarZenithData[index] < 18000) //这里的solarZenithData已经不是真实的高度角，而是经过计算的了
                        {
                            srcBandData[index] = (UInt16)(radiation * solarZenithData[index]);
                            if (srcBandData[index] > 65000)      //理论上讲反射率应当是0-100
                            {
                                srcBandData[index] = 0;
                            }
                        }
                    }
                });
            }
            else
            {
                int beginY = 0;
                int endY = height;
                Parallel.For(beginY, endY, (rowIndex) =>
                {
                    double scale = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5];
                    double offSet = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 1];
                    double scale2 = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 2];
                    double offSet2 = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 3];
                    double inflection = _refSB_Coeff[rowIndex + yoffset, bandIndex * 5 + 4];
                    double radiation = 0;
                    int index = 0;
                    for (int j = 0; j < width; j++)
                    {
                        index = rowIndex * width + j;
                        if (0 < srcBandData[index] && srcBandData[index] < inflection)//用两套斜率和截距
                            radiation = scale * srcBandData[index] + offSet;
                        else if (srcBandData[index] >= inflection)
                            radiation = scale2 * srcBandData[index] + offSet2;
                        srcBandData[index] = (UInt16)(10 * radiation);
                    }
                });
            }
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
            ID1BDDataProvider dp = srcRaster as ID1BDDataProvider;
            try
            {
                if (dp == null)
                    throw new Exception("读取亮温计算参数失败,可能为非NOAA 1BD格式轨道数据");
                D1BDHeader header = dp.Header;
                ushort sateIdentify = (header == null || header.CommonInfoFor1BD == null ? (ushort)0 : header.CommonInfoFor1BD.SatelliteIdentify);
                CreateEvABV(sateIdentify);
                _isDay = false;
                double[,] c = null;
                double[,] b = null;
                double[,] operCoef = null;
                dp.ReadVisiCoefficient(ref operCoef, ref b, ref c);
                _refSB_Coeff = operCoef;
                double[,] evCoefOper = null;
                dp.ReadIRCoefficient(ref evCoefOper, ref b);
                _emissive_Radiance_Coeff = evCoefOper;
            }
            catch(Exception ex)
            {
                throw new Exception("读取亮温计算参数失败",ex);
            }
        }

        /*
NOAA15
        A[0] = 1.621256f; A[1] = 0.337810f; A[2] = 0.304558f;
        B[0] = 0.998015f; B[1] = 0.998719f; B[2] = 0.999024f;
        v[0] = 2695.9743f; v[1] = 925.4075f; v[2] = 839.8979f;
NOAA16
        A[0] = 1.592459f; A[1] = 0.332380f; A[2] = 0.674623f;
        B[0] = 0.998147f; B[1] = 0.998522f; B[2] = 0.998363f;
        v[0] = 2700.1148f; v[1] = 917.2289f; v[2] = 838.1255f;
NOAA17
        A[0] = 1.702380f; A[1] = 0.271683f; A[2] = 0.309180f;
        B[0] = 0.997378f; B[1] = 0.998794f; B[2] = 0.999012f;
        v[0] = 2669.3554f; v[1] = 926.2947f; v[2] = 839.8246f;
NOAA18
        A[0] = 1.698704f; A[1] = 0.436645f; A[2] = 0.253179f;
        B[0] = 0.996960f; B[1] = 0.998607f; B[2] = 0.999057f;
        v[0] = 2659.7952f; v[1] = 928.1460f; v[2] = 833.2532f;
NOAA19
        A[0] = 1.698704f; A[1] = 0.436645f; A[2] = 0.253179f;
        B[0] = 0.996960f; B[1] = 0.998607f; B[2] = 0.999057f;
        v[0] = 2659.7952f; v[1] = 928.1460f; v[2] = 833.2532f;
        */
        private void CreateEvABV(ushort sateIdentify)
        {
            switch (sateIdentify)
            {
                case 4://"NOAA15"
                    _emmisive_BT_Coefficients = new float[]{ 
                        1.621256f, 0.998015f, 2695.9743f,     //通道3B:A B V
                        0.337810f, 0.998719f, 925.4075f,      //通道4 :A B V
                        0.304558f, 0.999024f, 839.8979f       //通道5 :A B V
                    };
                    break;
                case 3://"NOAA16",识别码待确认
                    _emmisive_BT_Coefficients = new float[]{ 
                        1.592459f, 0.998147f, 2700.1148f,
                        0.332380f, 0.998522f, 917.2289f,
                        0.674623f, 0.998363f, 838.1255f
                    };
                    break;
                case 11://"NOAA17"
                    _emmisive_BT_Coefficients = new float[] {
                        1.702380f, 0.997378f, 2669.3554f,
                        0.271683f, 0.998794f, 926.2947f,
                        0.309180f, 0.999012f, 839.8246f
                    };
                    break;
                case 13://"NOAA18"
                case 14://"NOAA19"
                default:
                    _emmisive_BT_Coefficients = new float[] {
                        1.698704f, 0.996960f, 2659.7952f,
                        0.436645f, 0.998607f, 928.1460f,
                        0.253179f, 0.999057f, 833.2532f
                    };
                    break;
            }
        }

        private void ReadySolarZenithArgsToFile(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取太阳天顶角数据失败");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                Size srcSize = new System.Drawing.Size(srcRaster.Width, srcRaster.Height);
                short[] readSolarZenithData = ReadDataSetToInt16(srcbandpro, srcSize, "SolarZenith", 0);
                int length = srcRaster.Width * srcRaster.Height;
                float[] saveSolarZenithData = new float[length];
                Parallel.For(0, length, index =>
                {
                    if (readSolarZenithData[index] > 0 && readSolarZenithData[index] < 18000)
                        saveSolarZenithData[index] = (float)(10.0f / Math.Cos(readSolarZenithData[index] * DEG_TO_RAD_P100));
                    else
                        saveSolarZenithData[index] = 0;
                });
                WriteData(saveSolarZenithData, _szDataFilename, srcSize.Width, srcSize.Height);
                saveSolarZenithData = null;
                readSolarZenithData = null;
            }
            catch (Exception ex)
            {
                throw new Exception("获取太阳天顶角数据失败", ex.InnerException);
            }
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
                prjSettings.OutBandNos = new int[] { 1, 2, 3, 4, 5};
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
