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
using System.Diagnostics;
using System.IO;

namespace GeoDo.FileProject
{
    [Export(typeof(IFileProjector)), ExportMetadata("VERSION", "1")]
    internal class FY3C_VIRRFileProjector : FileProjector
    {
        //太阳天顶角订正？=(定标值)/cos(太阳天顶角/100.0/180*3.14159);
        //一个完整的圆的弧度是2π,1 π rad = 180°,弧度=角度*PI/180
        //1°代表的弧度rad=π /180,(后面又除了100是由于用到的角度值放大了100倍)
        private const double DEG_TO_RAD_P100 = 0.000174532925199432955; // (PI/180)/100;
        private const double PI = 3.14159265358979323e0;
        private const double PI_OVER_2 = (PI / 2.0e0);

        #region 反射通道(可见光/近红外)定标常数
        private const string RefSB_Cal_Coefficients = "RefSB_Cal_Coefficients"; //文件属性,定标系数Scale和Offset
        private const string SolarZenith = "SolarZenith";                       //太阳天顶角数据集
        private const string EV_RefSB = "EV_RefSB";        //反射通道
        #endregion

        #region 发射通道(热红外通道)定标常数
        private const string Emmisive_Centroid_Wave_Number = "Emmisive_Centroid_Wave_Number";
        private const string Emmisive_BT_Coefficients = "Emmisive_BT_Coefficients";
        private const string Prelaunch_Nonlinear_Coefficients = "Prelaunch_Nonlinear_Coefficients";//辐亮度非线性订正系数
        private const string Emissive_Radiance_Scales = "Emissive_Radiance_Scales"; //线性定标系数
        private const string Emissive_Radiance_Offsets = "Emissive_Radiance_Offsets";
        private const double C1 = 1.1910427 / 100000;
        private const double C2 = 1.4387752;
        private const string EV_Emissive = "EV_Emissive";  //发射通道
        #endregion

        #region 经纬度数据集常数
        private const string Longitude = "Longitude";
        private const string Latitude = "Latitude";
        #endregion

        #region 辐射定标变量
        //反射通道定标系数，定长14，依次为：Scalech1、Offsetch1、Scalech2、Offsetch2、Scalech6、Offsetch6、Scalech7、Offsetch7、Scalech8、Offsetch8、Scalech9、Offsetch9、Scalech10、Offsetch10
        //定标值？= offSet+scale*观测值
        private float[] _refSB_Cal_Coefficients = null;
        /// <summary>太阳天顶角数据集(有效区间0-18000),角度值放大了100倍</summary>
        /// <summary>定标辐亮度值订正系数,只用到前9个数值，分别为：CH3的b0、b1、b2、CH4的b0、b1、b2和CH5的b0、b1、b2</summary>
        private float[] _prelaunch_Nonlinear_Coefficients = null;
        private float[] _emissive_Radiance_Scales = null;   //new float[3*height]
        private float[] _emissive_Radiance_Offsets = null;
        private float[] _emmisive_Centroid_Wave_Number = new float[] { 2699.119F, 923.427053F, 830.241775F };//v值
        private float[] _emmisive_BT_Coefficients = new float[] { 2.05806810F, 0.98231665F, 0.20002536F, 0.99791678F, 0.13149949F, 0.99820459F };//A、B值
        #endregion

        private ISpatialReference _srcSpatialRef = null;
        private IRasterDataDriver _outLdfDriver = null;             //输出ldf文件驱动
        private string _solarZenithCacheFilename;                   //太阳高度角文件，缓存文件
        private FY3_VIRR_PrjSettings _prjSettings;
        private PrjBand[] _prjBands = null;
        private IRasterDataProvider _geoDataProvider = null;

        public FY3C_VIRRFileProjector()
            : base()
        {
            _name = "FY3C_VIRR";
            _fullname = "FY3C_VIRR轨道文件投影";
            _rasterProjector = new RasterProjector();
            _srcSpatialRef = SpatialReference.GetDefault();
            _left = 8;
            _right = 8;
            _supportExtBandNames = new string[] { "DEM", "LandCover", "LandSeaMask" };     
        }

        public override bool IsSupport(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;
            return false;
        }

        public bool IsSuppose()
        {
            return false;
        }

        public override void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            try
            {
                ReadyArgs(srcRaster, prjSettings, dstSpatialRef, progressCallback);
                IRasterDataProvider outwriter = null;
                try
                {
                    List<string> options = new List<string>();
                    options.Add("INTERLEAVE=BSQ");
                    options.Add("VERSION=LDF");
                    options.Add("WITHHDR=TRUE");
                    options.Add("SPATIALREF=" + dstSpatialRef.ToProj4String());
                    options.Add("MAPINFO={" + 1 + "," + 1 + "}:{" + _prjSettings.OutEnvelope.MinX + "," + _prjSettings.OutEnvelope.MaxY + "}:{" + _outResolutionX + "," + _outResolutionY + "}");
                    options.Add("SENSOR=VIRR");
                    if (srcRaster.DataIdentify != null)
                    {
                        string satellite = srcRaster.DataIdentify.Satellite;
                        DateTime dt = srcRaster.DataIdentify.OrbitDateTime;
                        bool asc = srcRaster.DataIdentify.IsAscOrbitDirection;
                        if (!string.IsNullOrWhiteSpace(satellite))
                        {
                            options.Add("SATELLITE=" + satellite);
                        }
                        if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                            options.Add("DATETIME=" + dt.ToString("yyyy/MM/dd HH:mm"));
                        options.Add("ORBITDIRECTION=" + (asc ? "ASC" : "DESC"));
                        if (srcRaster.DataIdentify.DayOrNight == enumDayOrNight.Day)
                            options.Add("DAYNIGHT=D");
                        else if (srcRaster.DataIdentify.DayOrNight == enumDayOrNight.Night)
                            options.Add("DAYNIGHT=N");
                    }
                    List<string> op1 = new List<string>(options);
                    op1.Add("BANDNAMES=" + BandNameString(_prjSettings.OutBandNos));
                    outwriter = CreateOutFile(_outfilename, _dstBandCount, _dstSize, op1.ToArray());
                    ReadyAngleFiles(_geoDataProvider, _outfilename, _prjSettings, _dstSize, options.ToArray());
                    ReadyExtBands(_geoDataProvider, _outfilename, _prjSettings, _dstSize, options.ToArray());
                    ProjectToLDF(srcRaster, outwriter, 0, progressCallback);
                }
                catch (IOException ex)
                {
                    if (ex.Message == "磁盘空间不足。\r\n" && File.Exists(_outfilename))
                        File.Delete(_outfilename);
                    throw ex;
                }
                finally
                {
                    if (outwriter != null)
                    {
                        outwriter.Dispose();
                        outwriter = null;
                    }
                }
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

        private void ReadyArgs(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster");
            if (prjSettings == null)
                throw new ArgumentNullException("prjSettings");
            _readyProgress = 0;
            _prjSettings = prjSettings as FY3_VIRR_PrjSettings;
            _geoDataProvider = (prjSettings as FY3_VIRR_PrjSettings).GeoFile;
            _dstSpatialRef = dstSpatialRef;
            MemoryHelper.MemoryNeed(200, 1536);//剩余200MB,已使用1.2GB
            if (progressCallback != null)
                progressCallback(_readyProgress++, "准备相关参数");

            TryCreateDefaultArgs(srcRaster, _prjSettings, ref dstSpatialRef);
            //这里去除的是读取轨道数据时候的左右像元个数。
            _left = 8;
            _right = 8;
            TrySetLeftRightInvalidPixel(_prjSettings.ExtArgs);
            DoSession(srcRaster, _geoDataProvider, dstSpatialRef, _prjSettings, progressCallback);
            if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
            {
                prjSettings.OutEnvelope = _maxPrjEnvelope;
                _orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = srcRaster.Width - 1, yEnd = srcRaster.Height - 1 };
            }
            else
            {
                GetEnvelope(_xs, _ys, srcRaster.Width, srcRaster.Height, _prjSettings.OutEnvelope, out _orbitBlock);
                if (_orbitBlock == null || _orbitBlock.Width <= 0 || _orbitBlock.Height <= 0)
                    throw new Exception("数据不在目标区间内");
                float invalidPresent = (_orbitBlock.Width * _orbitBlock.Height * 1.0F) / (srcRaster.Width * srcRaster.Height);
                if (invalidPresent < 0.0001f)
                    throw new Exception("数据占轨道数据比例太小,有效率" + invalidPresent * 100 + "%");
                if (invalidPresent > 0.60)
                    _orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = srcRaster.Width - 1, yEnd = srcRaster.Height - 1 };
            }
            //if (dstSpatialRef.ProjectionCoordSystem == null && (prjSettings.OutEnvelope.MaxY > 80 || prjSettings.OutEnvelope.MaxY < -80))
            //    throw new Exception(string.Format("高纬度数据，不适合投影为等经纬度数据[{0}]", _maxPrjEnvelope));
            _dstSpatialRef = dstSpatialRef;
            _dstEnvelope = _prjSettings.OutEnvelope;
            if (!_dstEnvelope.IntersectsWith(_maxPrjEnvelope))
                throw new Exception("数据不在目标区间内");
            _outResolutionX = _prjSettings.OutResolutionX;
            _outResolutionY = _prjSettings.OutResolutionY;
            _outFormat = _prjSettings.OutFormat;
            _outfilename = _prjSettings.OutPathAndFileName;
            _dstProj4 = _dstSpatialRef.ToProj4String();
            _dstBandCount = _prjBands.Length;
            _dstSize = _prjSettings.OutSize;
            _isRadiation = _prjSettings.IsRadiation;
            _isSolarZenith = _prjSettings.IsSolarZenith;
            _isSensorZenith = _prjSettings.IsSensorZenith;
        }

        public override void EndSession()
        {
            base.EndSession();
            _xs = null;
            _ys = null;
            _refSB_Cal_Coefficients = null;
            _prelaunch_Nonlinear_Coefficients = null;
            _emissive_Radiance_Scales = null;
            _emissive_Radiance_Offsets = null;
            if (_outLdfDriver != null)
            {
                _outLdfDriver.Dispose();
                _outLdfDriver = null;
            }
            _geoDataProvider = null;
        }

        private void DoSession(IRasterDataProvider srcRaster, IRasterDataProvider geoRaster, ISpatialReference dstSpatialRef, FY3_VIRR_PrjSettings prjSettings, Action<int, string> progressCallback)
        {
            if (_curSession == null || _curSession != srcRaster || _isBeginSession)
            {
                if (progressCallback != null)
                    progressCallback(_readyProgress++, "读取及预处理经纬度数据集");
                ReadyLocations(_geoDataProvider, SpatialReference.GetDefault(), dstSpatialRef, out _srcLocationSize, out _xs, out _ys, out _maxPrjEnvelope, progressCallback);
                if (progressCallback != null)
                    progressCallback(_readyProgress++, "准备其他参数");
                if (prjSettings.IsRadiation)
                    ReadyRadiationArgs(srcRaster);
                if (prjSettings.IsSolarZenith && prjSettings.IsRadiation)
                {
                    _solarZenithCacheFilename = GetSolarZenithCacheFilename(geoRaster.fileName);    //太阳天顶角数据
                    if (!File.Exists(_solarZenithCacheFilename))
                        ReadySolarZenithArgsToFile(geoRaster);
                    else
                        _solarZenithCacheRaster = GeoDataDriver.Open(_solarZenithCacheFilename) as IRasterDataProvider;
                    if (prjSettings.IsSensorZenith)
                    {
                        ReadySensorZenith(geoRaster);
                    }
                }
                _rasterDataBands = TryCreateRasterDataBands(srcRaster, prjSettings, progressCallback);
                _isBeginSession = false;
            }
        }

        protected override void ReadLocations(IRasterDataProvider geoRaster, out double[] longitudes, out double[] latitudes, out Size lonSize)
        {
            IBandProvider srcbandpro = geoRaster.BandProvider as IBandProvider;
            {
                IRasterBand[] lonsBands = srcbandpro.GetBands("Longitude");
                using (IRasterBand lonsBand = lonsBands[0])
                {
                    lonSize = new Size(lonsBand.Width, lonsBand.Height);
                    longitudes = new Double[lonsBand.Width * lonsBand.Height];
                    unsafe
                    {
                        fixed (Double* ptrLong = longitudes)
                        {
                            IntPtr bufferPtrLong = new IntPtr(ptrLong);
                            lonsBand.Read(0, 0, lonsBand.Width, lonsBand.Height, bufferPtrLong, enumDataType.Double, lonsBand.Width, lonsBand.Height);
                        }
                    }
                }
                IRasterBand[] latBands = srcbandpro.GetBands("Latitude");
                using (IRasterBand latBand = latBands[0])
                {
                    latitudes = new Double[lonSize.Width * lonSize.Height];
                    unsafe
                    {
                        fixed (Double* ptrLat = latitudes)
                        {
                            {
                                IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                latBand.Read(0, 0, latBand.Width, latBand.Height, bufferPtrLat, enumDataType.Double, latBand.Width, latBand.Height);
                            }
                        }
                    }
                }
            }
        }

        private void ReadySensorZenith(IRasterDataProvider srcRaster)
        {
            _sensorSenithRaster = srcRaster;
            IRasterBand[] bands = srcRaster.BandProvider.GetBands("SensorZenith");
            if (bands != null || bands.Length != 1)
                _sensorSenithBand = bands[0];
        }

        private IRasterBand[] TryCreateRasterDataBands(IRasterDataProvider srcRaster, FY3_VIRR_PrjSettings prjSettings, Action<int, string> progressCallback)
        {
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            List<IRasterBand> rasterBands = new List<IRasterBand>();
            for (int i = 0; i < _prjBands.Length; i++)
            {
                if (progressCallback != null)
                    progressCallback(_readyProgress++, "准备第" + i + "个输入数据通道");
                PrjBand bandMap = _prjBands[i];
                IRasterBand[] latBands = srcbandpro.GetBands(bandMap.DataSetName);
                IRasterBand band = latBands[bandMap.DataSetIndex];
                rasterBands.Add(band);
            }
            return rasterBands.ToArray();
        }

        /// <summary> 
        /// 辐射值订正
        /// 
        /// </summary>
        private void DoRadiation(ushort[] srcBandData, Size srcSize, string datasetName, int bandIndex, bool isSolarZenith, float[] solarZenithData)
        {
            switch (datasetName)
            {
                case EV_RefSB://->反射率
                    {
                        float scale = _refSB_Cal_Coefficients[bandIndex * 2];
                        float offSet = _refSB_Cal_Coefficients[bandIndex * 2 + 1];
                        RefSbRadiation(srcBandData, isSolarZenith, scale, offSet, solarZenithData);
                    }
                    break;
                case EV_Emissive://->辐亮度
                    {
                        float b0 = _prelaunch_Nonlinear_Coefficients[bandIndex * 3];
                        float b1 = _prelaunch_Nonlinear_Coefficients[bandIndex * 3 + 1];
                        float b2 = _prelaunch_Nonlinear_Coefficients[bandIndex * 3 + 2];
                        float v = _emmisive_Centroid_Wave_Number[bandIndex];
                        float A = _emmisive_BT_Coefficients[bandIndex * 2];
                        float B = _emmisive_BT_Coefficients[bandIndex * 2 + 1];
                        int height = srcSize.Height;
                        int width = srcSize.Width;
                        double v3 = v * v * v;
                        double c2v = C2 * v;
                        double c1v3 = C1 * v * v * v;
                        Parallel.For(0, height, (i) =>
                        {
                            float scale = _emissive_Radiance_Scales[i * 3 + bandIndex];
                            float offset = _emissive_Radiance_Offsets[i * 3 + bandIndex];
                            int index;
                            UInt16 dn;
                            double radiationLIN;
                            double radiation;
                            double temperatureBB;
                            double sensorZenith;
                            double deltaT;
                            int rowOffset = i * width;
                            for (int j = 0; j < width; j++)
                            {
                                index = rowOffset + j;
                                dn = srcBandData[index];
                                if (dn > 0 && dn < 50000)   //EV_Emissive的有效值区间,理论上亮温应当在150K~350K之间
                                {
                                    radiationLIN = (dn * scale + offset);
                                    radiation = (b0 + (b1 + 1) * radiationLIN + b2 * radiationLIN * radiationLIN);
                                    temperatureBB = (c2v / Math.Log(1 + c1v3 / radiation) - A) / B;
                                    //以上计算出来的值为亮温值
                                    //下面这行为对亮温，使用卫星天顶角，执行"临边变暗订正"。
                                    if (_isSensorZenith && _sensorZenithData != null)
                                    {
                                        sensorZenith = _sensorZenithData[index] * 0.01d;
                                        deltaT = temperatureBB + (Math.Pow(Math.E, 0.00012 * sensorZenith * sensorZenith) - 1) * (0.1072 * temperatureBB - 26.81);
                                        srcBandData[index] = (UInt16)(deltaT * 10);
                                    }
                                    else
                                        srcBandData[index] = (UInt16)(temperatureBB * 10);
                                }
                                else
                                    srcBandData[index] = 0;
                            }
                        });

                    }
                    break;
                default:
                    break;
            }
        }

        private void RefSbRadiation(ushort[] srcBandData, bool isSolarZenith, float scale, float offSet, float[] solarZenithData)
        {
            int length = srcBandData.Length;
            if (isSolarZenith)
            {
                Parallel.For(0, length, (index) =>
                {
                    float solarZenith = solarZenithData[index];
                    srcBandData[index] = (UInt16)((scale * srcBandData[index] + offSet) * solarZenith);//高度角已经转为订正的值了。
                    if (srcBandData[index] > 65000)
                        srcBandData[index] = 0;
                });
            }
            else
            {
                Parallel.For(0, length, (index) =>
                {
                    srcBandData[index] = (UInt16)(10 * (scale * srcBandData[index] + offSet));
                    if (srcBandData[index] > 65000)
                        srcBandData[index] = 0;
                });
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
        
        #region 辐射反射定标计算        
        private void ReadyRadiationArgs(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取亮温转换参数失败：参数srcRaster为空");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                _refSB_Cal_Coefficients = ReadFileAttributeToFloat(srcbandpro, RefSB_Cal_Coefficients, 14);
                _prelaunch_Nonlinear_Coefficients = ReadFileAttributeToFloat(srcbandpro, Prelaunch_Nonlinear_Coefficients, 9);
                Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                _emissive_Radiance_Scales = ReadDataSetToSingle(srcbandpro, new Size(3, srcSize.Height), Emissive_Radiance_Scales, 0);
                _emissive_Radiance_Offsets = ReadDataSetToSingle(srcbandpro, new Size(3, srcSize.Height), Emissive_Radiance_Offsets, 0);
            }
            catch (Exception ex)
            {
                throw new Exception("获取亮温转换参数失败:" + ex.Message, ex.InnerException);
            }
        }

        private void ReadySolarZenithArgsToFile(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取太阳天顶角数据失败");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                Size srcSize = Size.Empty;
                short[] readSolarZenithData = ReadDataSetToInt16(srcbandpro, SolarZenith, 0,out srcSize);
                int length = srcSize.Width * srcSize.Height;
                float[] saveSolarZenithData = new float[length];
                Parallel.For(0, length, index =>
                {
                    if (readSolarZenithData[index] > 0 && readSolarZenithData[index] < 18000)
                        saveSolarZenithData[index] = (float)(10.0f / Math.Cos(readSolarZenithData[index] * DEG_TO_RAD_P100));
                    else
                        saveSolarZenithData[index] = 0;
                });
                _solarZenithCacheRaster = WriteData(saveSolarZenithData, _solarZenithCacheFilename, srcSize.Width, srcSize.Height);
                saveSolarZenithData = null;
                readSolarZenithData = null;
            }
            catch (Exception ex)
            {
                throw new Exception("获取太阳天顶角数据失败", ex.InnerException);
            }
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

        private Int16[] ReadDataSetToInt16(IBandProvider srcbandpro, string dataSetName, int bandIndex, out Size srcSize)
        {
            Int16[] data = null;
            using (IRasterBand rasterBand = srcbandpro.GetBands(dataSetName)[0])
            {
                srcSize = new Size(rasterBand.Width, rasterBand.Height);
                data = new Int16[srcSize.Width * srcSize.Height];
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
            using (IRasterBand rasterBand = srcbandpro.GetBands(dataSetName)[0])
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
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster");
            if (prjSettings == null)
                throw new ArgumentNullException("prjSettings");
            if (dstRaster == null && prjSettings.OutPathAndFileName == null)
                throw new ArgumentNullException("dstRaster");
            try
            {
                FY3_VIRR_PrjSettings fy3prjSettings = prjSettings as FY3_VIRR_PrjSettings;
                ISpatialReference dstSpatialRef = dstRaster.SpatialRef;
                TryCreateDefaultArgs(srcRaster, fy3prjSettings, ref dstSpatialRef);
                DoSession(srcRaster,_geoDataProvider, dstSpatialRef, fy3prjSettings, progressCallback);
                if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
                    prjSettings.OutEnvelope = _maxPrjEnvelope;
                if (dstSpatialRef.ProjectionCoordSystem == null && (prjSettings.OutEnvelope.MaxY > 80 || prjSettings.OutEnvelope.MaxY < -80))
                    throw new Exception(string.Format("高纬度数据[>80]，不适合投影为等经纬度数据[{0}]", _maxPrjEnvelope));
                if (dstRaster == null)
                {
                    PrjEnvelope dstEnvelope = prjSettings.OutEnvelope;
                    string outFormat = prjSettings.OutFormat;
                    string outfilename = prjSettings.OutPathAndFileName;
                    string dstProj4 = dstSpatialRef.ToProj4String();
                    int dstBandCount = _prjBands.Length;
                    Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                    Size dstSize = prjSettings.OutSize;
                    IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;

                    string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + dstProj4,
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + dstEnvelope.MinX + "," + dstEnvelope.MaxY + "}:{" + prjSettings.OutResolutionX + "," + prjSettings.OutResolutionY + "}"
                        };
                    dstRaster = drv.Create(outfilename, dstSize.Width, dstSize.Height, dstBandCount, enumDataType.UInt16, options) as IRasterDataProvider;
                }
                ProjectToLDF(srcRaster, dstRaster, 0, progressCallback);
                return dstRaster;
            }
            catch
            {
                EndSession();
                TryDeleteCurCatch();
                throw;
            }
            finally
            {
                EndSession();
                if (prjSettings.IsClearPrjCache)
                    TryDeleteCurCatch();
            }
        }

        public override FilePrjSettings CreateDefaultPrjSettings()
        {
            return new FY3_VIRR_PrjSettings();
        }

        private void TryCreateDefaultArgs(IRasterDataProvider srcRaster, FY3_VIRR_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = _srcSpatialRef;
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
                prjSettings.OutFormat = "LDF";
            if (dstSpatialRef.ProjectionCoordSystem == null)
            {
                _srcImgResolution = 0.01F;
            }
            else
            {
                _srcImgResolution = 1000f;
            }
            if (prjSettings.OutResolutionX <= 0 || prjSettings.OutResolutionY <= 0)
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
                _prjBands = PrjBand.VIRR_1000_Orbit;
            }
            else
            {
                List<PrjBand> bands = new List<PrjBand>();
                PrjBand[] defbands = PrjBand.VIRR_1000_Orbit;
                foreach (int bandNo in prjSettings.OutBandNos)
                {
                    bands.Add(defbands[bandNo - 1]);
                }
                _prjBands = bands.ToArray();
            }
        }

        public override void ComputeDstEnvelope(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (srcRaster != null)
            {
                Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                double[] xs, ys;
                ReadyLocations(srcRaster, SpatialReference.GetDefault(), dstSpatialRef, out srcSize, out xs, out ys, out maxPrjEnvelope, progressCallback);
            }
            else
            {
                maxPrjEnvelope = PrjEnvelope.Empty;
            }
        }

        protected override void DoRadiation(IRasterDataProvider srcImgRaster, int i, ushort[] srcBandData, float[] solarZenithData, Size srcBlockImgSize, Size angleSize)
        {
            if (_isRadiation)
            {
                string ds = _prjBands[i].DataSetName;
                int dsIndex = _prjBands[i].DataSetIndex;
                DoRadiation(srcBandData, srcBlockImgSize, ds, dsIndex, _isSolarZenith, solarZenithData);
            }
        }
    }
}
