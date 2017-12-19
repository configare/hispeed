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

namespace GeoDo.FileProject
{
    [Export(typeof(IFileProjector)), ExportMetadata("VERSION", "1")]
    public class EOS_FileProjector : FileProjector
    {
        private const double DEG_TO_RAD_P100 = 0.000174532925199432955; // (PI/180)/100;
        private const double RAD_TO_DEG = 57.29577951308232087;         // 180/PI
        private const double PI = 3.14159265358979323e0;
        private const double PI_OVER_2 = (PI / 2.0e0);

        #region 反射通道(可见光/近红外)定标常数
        private const string SolarZenith = "SolarZenith";                       //太阳天顶角数据集
        #endregion

        #region 发射通道(热红外通道)定标常数
        #endregion

        #region 经纬度数据集常数
        private const string Longitude = "Longitude";
        private const string Latitude = "Latitude";
        #endregion

        #region 定标变量
        private double[] _solarZenithData = null;
        #endregion

        private IRasterProjector _rasterProjector = null;
        private ISpatialReference _srcSpatialRef = null;

        #region Session
        PrjEnvelope _maxPrjEnvelope = null;
        private double[] _xs = null;    //存储的实际是计算后的值
        private double[] _ys = null;    //存储的实际是计算后的值
        private Size _locationSize;
        #endregion

        public EOS_FileProjector()
            : base()
        {
            _name = "EOS";
            _fullname = "EOS轨道文件投影";
            _rasterProjector = new RasterProjector();
            _srcSpatialRef = new SpatialReference(new GeographicCoordSystem());
        }

        public override void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster");
            if (prjSettings == null)
                throw new ArgumentNullException("prjSettings");
            EOS_MODIS_PrjSettings eosPrjSettings = prjSettings as EOS_MODIS_PrjSettings;
            if (eosPrjSettings.LocationFile == null)
                throw new ArgumentNullException("prjSettings.LocationFile", "EOS投影未设置经纬度坐标(MOD03.hdf)文件");
            if (progressCallback != null)
                progressCallback(0, "准备相关参数");
            string fileType = CheckFile(srcRaster);
            switch (fileType)
            {
                case "1000":
                    TryCreateDefaultArgsKM(srcRaster, eosPrjSettings, ref dstSpatialRef);
                    DoSession(srcRaster, dstSpatialRef, eosPrjSettings, progressCallback);
                    if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
                        prjSettings.OutEnvelope = _maxPrjEnvelope;
                    Project(srcRaster, eosPrjSettings, dstSpatialRef, progressCallback);
                    break;
                case "0500":
                    TryCreateDefaultArgsHKM(srcRaster, eosPrjSettings, ref dstSpatialRef);
                    DoSession(srcRaster, dstSpatialRef, eosPrjSettings, progressCallback);
                    if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
                        prjSettings.OutEnvelope = _maxPrjEnvelope;
                    Project0500(srcRaster, _locationSize, eosPrjSettings, dstSpatialRef, progressCallback);
                    break;
                case "0250":
                    TryCreateDefaultArgsQKM(srcRaster, eosPrjSettings, ref dstSpatialRef);
                    break;
                default:
                    throw new NotSupportedException("不能识别该投影文件为受支持的EOS轨道数据" + srcRaster.fileName);
            }
        }

        private void DoSession(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, EOS_MODIS_PrjSettings fy3prjSettings, Action<int, string> progressCallback)
        {
            if (_curSession != null && _curSession != srcRaster)
                EndSession();
            if (_curSession == null)
            {
                IRasterDataProvider locationFile = fy3prjSettings.LocationFile;
                ReadyLocations(locationFile, dstSpatialRef, out _xs, out _ys, out _maxPrjEnvelope, out _locationSize, progressCallback);
                if (progressCallback != null)
                    progressCallback(3, "准备其他参数"); 
                if (fy3prjSettings.IsRadiation && fy3prjSettings.IsSolarZenith)
                    ReadySolarZenithArgs(locationFile);
            }
        }

        private void Project(IRasterDataProvider srcRaster, EOS_MODIS_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
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
                throw new Exception("数据不在目标区间内");
        }

        /// <summary>
        /// 0、先生成目标文件，以防止目标空间不足。
        /// 1、计算查找表
        /// 2、对所有波段执行投影
        /// </summary>
        private void ProjectToLDF(IRasterDataProvider srcRaster, EOS_MODIS_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            string outFormat = prjSettings.OutFormat;
            string outfilename = prjSettings.OutPathAndFileName;
            string dstProj4 = dstSpatialRef.ToProj4String();
            List<BandMap> bandMaps = prjSettings.BandMapTable;
            int dstBandCount = bandMaps.Count;
            Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
            Size dstSize = prjSettings.OutSize;
            using (IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
            {
                PrjEnvelope dstEnvelope = prjSettings.OutEnvelope;
                string[] options = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF=" + dstSpatialRef.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + dstEnvelope.MinX + "," + dstEnvelope.MaxY + "}:{" + prjSettings.OutResolutionX + "," + prjSettings.OutResolutionY + "}"
                };
                if (progressCallback != null)
                    progressCallback(4, "生成输出文件");
                using (IRasterDataProvider prdWriter = drv.Create(outfilename, dstSize.Width, dstSize.Height, dstBandCount,
                    enumDataType.UInt16, options) as IRasterDataProvider)
                {
                    UInt16[] dstRowLookUpTable = new UInt16[dstSize.Width * dstSize.Height];
                    UInt16[] dstColLookUpTable = new UInt16[dstSize.Width * dstSize.Height];
                    _rasterProjector.ComputeIndexMapTable(_xs, _ys, _locationSize, dstSize, dstEnvelope, _maxPrjEnvelope,
                        out dstRowLookUpTable, out dstColLookUpTable, null);
                    //执行投影
                    UInt16[] srcBandData = new UInt16[srcSize.Width * srcSize.Height];
                    UInt16[] dstBandData = new UInt16[dstSize.Width * dstSize.Height];
                    int progress = 0;
                    for (int i = 0; i < dstBandCount; i++)      //读取原始通道值，投影到目标区域
                    {
                        if (progressCallback != null)
                        {
                            progress = (int)((i + 1) * 100 / dstBandCount);
                            progressCallback(progress, string.Format("投影第{0}通道", i + 1));
                        }
                        BandMap bandMap = bandMaps[i];
                        ReadBandData(srcBandData, bandMap.File, bandMap.DatasetName, bandMap.BandIndex, srcSize);
                        DoRadiation(srcBandData, srcSize, bandMap.DatasetName, bandMap.BandIndex, srcRaster, prjSettings.IsRadiation, prjSettings.IsSolarZenith);
                        _rasterProjector.Project<UInt16>(srcBandData, srcSize, dstRowLookUpTable, dstColLookUpTable, dstSize, dstBandData, 0, null);
                        using (IRasterBand band = prdWriter.GetRasterBand(i + 1))
                        {
                            unsafe
                            {
                                fixed (UInt16* ptr = dstBandData)
                                {
                                    IntPtr bufferPtr = new IntPtr(ptr);
                                    band.Write(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DoRadiation(ushort[] srcBandData, Size srcSize, string datasetName, int bandIndex,
            IRasterDataProvider dataRaster, bool isRadiation, bool isSolarZenith)
        {
            DoRadiation(srcBandData, srcSize, srcSize, datasetName, bandIndex, dataRaster, isRadiation, isSolarZenith);
        }

        /// <summary> 辐射值计算</summary>
        private void DoRadiation(ushort[] srcBandData, Size srcDataSize,Size srcLocationSize, string datasetName, int bandIndex, 
            IRasterDataProvider dataRaster,bool isRadiation,bool isSolarZenith)
        {
            if (!isRadiation)
                return;
            ReadyRadiationArgs(dataRaster, datasetName);
            switch (datasetName)
	        {
                case "EV_1KM_RefSB":
                case "EV_500_Aggr1km_RefSB":
                case "EV_250_Aggr1km_RefSB":
                    {
                        float scale = 1000 * _radiance_Scales[bandIndex];
                        float offSet = _radiance_Offsets[bandIndex];
                        RefSBRadiation(srcBandData, isSolarZenith, scale, offSet);
                    }
                    break;
                case "EV_1KM_Emissive":
                    {
                        float scale = _radiance_Scales[bandIndex];
                        float offSet = _radiance_Offsets[bandIndex];
                        EmissiveRadistion(srcBandData, bandIndex, scale, offSet);
                    }
                    break;
                case "EV_250_RefSB":
                case "EV_500_RefSB":
                case "EV_250_Aggr500_RefSB":
                    {
                        float scale = 1000 * _radiance_Scales[bandIndex];
                        float offSet = _radiance_Offsets[bandIndex];
                        RefSBRadiation(srcBandData, isSolarZenith, scale, offSet, srcDataSize, srcLocationSize);
                    }
                    break;
		        default:
                    break;
	        }
        }

        private void EmissiveRadistion(ushort[] srcBandData, int bandIndex, float scale, float offSet)
        {
            double ws = 10 * c2 / (1.0e-6 * _v[bandIndex] / 1000);
            double ws5 = Math.Pow(ws, 5);
            double c1ws5 = c1 / ws5;
            Parallel.For(0, srcBandData.Length, (index) =>
            {
                double radiation = 1.0e06 * scale * (srcBandData[index] - offSet);
                srcBandData[index] = (ushort)planck_m(ws, c1ws5, radiation);
            });
        }

        const double h = 6.6256e-34;  //Planck constant (Joule second)
        const double c = 3e+8;        //Speed of light in vacuum (meters per second)
        const double k = 1.38e-23;    //Boltzmann constant (Joules per Kelvin)
        //Derived constants
        const double c1 = 2.0 * h * c * c;
        const double c2 = (h * c) / k;
        private double planck_m(double ws, double c1ws5, double t)
        {
            //double ws = 1.0e-6 * w;   // Convert wavelength to meters
            //double tmp = c2 / (ws * Math.Log(c1 / (Math.Pow(ws, 5) * t) + 1));
            //return tmp * 10;
            return ws / Math.Log(c1ws5 / t + 1);
        }

        private void RefSBRadiation(ushort[] srcBandData, bool isSolarZenith, float scale, float offSet)
        {
            if (isSolarZenith)//高度角订正
            {
                Parallel.For(0, srcBandData.Length, (index) =>
                {
                    double radiation = scale * (srcBandData[index] - offSet);
                    if (srcBandData[index] > 65000)     //理论上讲反射率应当是0-100
                        srcBandData[index] = 0;
                    else if (_solarZenithData[index] > 0 && _solarZenithData[index] < 18000)
                        srcBandData[index] = (UInt16)(radiation * _solarZenithData[index]);
                });
            }
            else
            {
                Parallel.For(0, srcBandData.Length, (index) =>
                {
                    if (srcBandData[index] > 65000)     //理论上讲反射率应当是0-100
                        srcBandData[index] = 0;
                    else
                        srcBandData[index] = (UInt16)(scale * (srcBandData[index] - offSet));
                });
            }
        }

        private void RefSBRadiation(ushort[] srcBandData, bool isSolarZenith, float scale, float offSet, Size dataSize, Size locationSize)
        {
            if (dataSize == locationSize)
            {
                RefSBRadiation(srcBandData, isSolarZenith, scale, offSet);
                return;
            }
            float scoreX = (float)dataSize.Width / locationSize.Width;
            float scoreY = (float)dataSize.Height / locationSize.Height;
            int dataHeight = dataSize.Height;
            int dataWidth = dataSize.Width;
            int lcWidth = locationSize.Width;
            if (isSolarZenith)//高度角订正
            {
                Parallel.For(0, dataHeight, (dataRow) =>
                {
                    for (int dataCol = 0; dataCol < dataWidth; dataCol++)
                    {
                        int offset = dataRow * dataWidth;
                        int index = offset + dataCol;
                        ushort curData = srcBandData[index];
                        double radiation = scale * (curData - offSet);
                        if (curData > 65000)     //理论上讲反射率应当是0-100
                            srcBandData[index] = 0;
                        else
                        {
                            int szRow = (int)(dataRow / scoreY);
                            int szCol = (int)(dataCol / scoreX);
                            double solarZenith = _solarZenithData[szRow * lcWidth + szCol];//;
                            if (solarZenith > 0 && solarZenith < 18000)
                            {
                                srcBandData[index] = (UInt16)(radiation * solarZenith);
                            }
                        }
                    }
                });
            }
            else
            {
                Parallel.For(0, srcBandData.Length, (index) =>
                {
                    if (srcBandData[index] > 65000)     //理论上讲反射率应当是0-100
                        srcBandData[index] = 0;
                    else
                        srcBandData[index] = (UInt16)(scale * (srcBandData[index] - offSet));
                });
            }
        }

        #region 定标参数读取
        //准备[辐射定标]参数
        float[] _radiance_Scales;
        float[] _radiance_Offsets;
        private void ReadyRadiationArgs(IRasterDataProvider srcRaster, string dataSetName)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取亮温转换参数失败");
            if (dataSetName == null)
                throw new ArgumentNullException("dataSetName", "获取亮温转换参数失败");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                switch (dataSetName)
                {
                    case "EV_1KM_RefSB":
                        _radiance_Scales = ReadDataSetAttrToFloat(srcbandpro, "EV_1KM_RefSB", "reflectance_scales", 15);
                        _radiance_Offsets = ReadDataSetAttrToFloat(srcbandpro, "EV_1KM_RefSB", "reflectance_offsets", 15);
                        break;
                    case "EV_250_Aggr1km_RefSB":
                        _radiance_Scales = ReadDataSetAttrToFloat(srcbandpro, "EV_250_Aggr1km_RefSB", "reflectance_scales", 2);
                        _radiance_Offsets = ReadDataSetAttrToFloat(srcbandpro, "EV_250_Aggr1km_RefSB", "reflectance_offsets", 2);
                        break;
                    case "EV_500_Aggr1km_RefSB":
                        _radiance_Scales = ReadDataSetAttrToFloat(srcbandpro, "EV_500_Aggr1km_RefSB", "reflectance_scales", 5);
                        _radiance_Offsets = ReadDataSetAttrToFloat(srcbandpro, "EV_500_Aggr1km_RefSB", "reflectance_offsets", 5);
                        break;
                    case "EV_1KM_Emissive"://热红外satellite Terra (EOS AM-1)  Aqua (EOS PM-1)
                        _radiance_Scales = ReadDataSetAttrToFloat(srcbandpro, "EV_1KM_Emissive", "radiance_scales", 16);
                        _radiance_Offsets = ReadDataSetAttrToFloat(srcbandpro, "EV_1KM_Emissive", "radiance_offsets", 16);
                        if (_fileAliasName == "Terra")
                        {
                            _v = new float[] { 3788.3F, 3992.2F, 3972.0F,4056.7F,4473.2F,4545.4F,6770.5F,7342.9F,8528.7F,
                                9734.1F,11018.9F,12032.1F,13365.0F,13683.3F,13925.2F,14195.6F};
                        }
                        else if (_fileAliasName == "Aqua")
                        {
                            _v = new float[] { 3780.2F ,3981.8F,3972.0F,4061.6F,4448.3F,4526.3F,6786.8F,7349.3F,8555.3F,
                                9723.7F,11026.2F,12042.3F,13364.7F,13685.9F,13913.2F,14215.2F};
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取亮温转换参数失败", ex.InnerException);
            }
        }
        float[] _v;

        //用于可见光太阳高度角订正，准备[太阳高度角订正]参数,目前还没有太阳高度角订正的公式。
        private void ReadySolarZenithArgs(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取太阳天顶角数据失败");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                short[] solarZenithDataS = ReadDataSetToInt16(srcbandpro, SolarZenith, 0);
                int length = solarZenithDataS.Length;
                _solarZenithData = new double[length];
                Parallel.For(0, length, i =>
                {
                    _solarZenithData[i] = 1.0 / Math.Cos(solarZenithDataS[i] * DEG_TO_RAD_P100);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("获取太阳天顶角数据失败", ex.InnerException);
            }
        }

        private float[] ReadDataSetAttrToFloat(IBandProvider srcbandpro, string dataSetName, string attrName, int length)
        {
            float[] value = new float[length];
            IDictionary<string, string> dsAtts = srcbandpro.GetDatasetAttributes(dataSetName);
            string refSbCalStr = dsAtts[attrName];
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

        private Int16[] ReadDataSetToInt16(IBandProvider srcbandpro, string dataSetName, int bandIndex)
        {
            Int16[] data;
           IRasterBand[] rasterBands = srcbandpro.GetBands(dataSetName);
           using (IRasterBand rasterBand = rasterBands[0])
            {
                data = new Int16[rasterBand.Width * rasterBand.Height];
                unsafe
                {
                    fixed (Int16* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        rasterBand.Read(0, 0, rasterBand.Width, rasterBand.Height, bufferPtr, enumDataType.Int16, rasterBand.Width, rasterBand.Height);
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

        /// <summary> 读取通道值</summary>
        private void ReadBandData(UInt16[] bandData,IRasterDataProvider srcRaster, string bandName, int bandNumber, Size srcSize)
        {
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            {
                IRasterBand[] latBands = srcbandpro.GetBands(bandName);
                using (IRasterBand latBand = latBands[bandNumber])
                {
                    unsafe
                    {
                        fixed (UInt16* ptr = bandData)
                        {
                            IntPtr bufferptr = new IntPtr(ptr);
                            latBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferptr, enumDataType.UInt16, srcSize.Width, srcSize.Height);
                        }
                    }
                }
            }
        }

        /// <summary> 
        /// 准备定位信息,计算投影后的值，并计算范围
        /// </summary>
        private void ReadyLocations(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef,
            out double[] xs, out double[] ys, out PrjEnvelope maxPrjEnvelope, out Size locationSize, Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(1, "读取经纬度数据集");
            ReadLongitudeLatitude(srcRaster, out xs, out ys, out locationSize);
            if (progressCallback != null)
                progressCallback(2, "预处理经纬度数据集");
            _rasterProjector.ComputeDstEnvelope(_srcSpatialRef, xs, ys, locationSize, dstSpatialRef, out maxPrjEnvelope, progressCallback);
        }

        //读取定位信息(经纬度数据集)
        private void ReadLongitudeLatitude(IRasterDataProvider raster, out double[] longitudes, out double[] latitudes, out Size lonSize)
        {
            IBandProvider srcbandpro = raster.BandProvider as IBandProvider;
            {
                IRasterBand[] lonsBands = srcbandpro.GetBands(Longitude);
                using (IRasterBand lonsBand = lonsBands[0])
                {
                    lonSize = new Size(lonsBand.Width , lonsBand.Height);
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
                IRasterBand[] latBands = srcbandpro.GetBands(Latitude);
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
        
        public override IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, IRasterDataProvider dstRaster, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override FilePrjSettings CreateDefaultPrjSettings()
        {
            return new FY3_VIRR_PrjSettings();
        }

        private void TryCreateDefaultArgsKM(IRasterDataProvider srcRaster, EOS_MODIS_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
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
            if (prjSettings.BandMapTable == null || prjSettings.BandMapTable.Count == 0)
            {
                List<BandMap> bandmapList = new List<BandMap>();
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr1km_RefSB", File = srcRaster, BandIndex = 0 });//1
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr1km_RefSB", File = srcRaster, BandIndex = 1 });//2
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_Aggr1km_RefSB", File = srcRaster, BandIndex = 0 });//3
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_Aggr1km_RefSB", File = srcRaster, BandIndex = 1 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_Aggr1km_RefSB", File = srcRaster, BandIndex = 2 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_Aggr1km_RefSB", File = srcRaster, BandIndex = 3 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_Aggr1km_RefSB", File = srcRaster, BandIndex = 4 });//7
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 0 });//8
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 1 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 2 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 3 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 4 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 5 });//13lo
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 6 });//13hi
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 7 });//14lo
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 8 });//14hi
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 9 });//15
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 10 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 11 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 12 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 13 });//19
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 0 });//20
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 1 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 2 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 3 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 4 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 5 });//25
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 14 });//26
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 6 });//27
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 7 });//28
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 8 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 9 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 10 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 11 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 12 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 13 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 14 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_Emissive", File = srcRaster, BandIndex = 15 });//36
                prjSettings.BandMapTable = bandmapList;
            }
        }

        private void TryCreateDefaultArgsHKM(IRasterDataProvider srcRaster, EOS_MODIS_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = _srcSpatialRef;
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
                prjSettings.OutFormat = "LDF";
            if (prjSettings.OutResolutionX == 0 || prjSettings.OutResolutionY == 0)
            {
                if (dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjSettings.OutResolutionX = 0.005F;
                    prjSettings.OutResolutionY = 0.005F;
                }
                else
                {
                    prjSettings.OutResolutionX = 500F;
                    prjSettings.OutResolutionY = 500F;
                }
            }
            if (prjSettings.BandMapTable == null || prjSettings.BandMapTable.Count == 0)
            {
                List<BandMap> bandmapList = new List<BandMap>();
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr500_RefSB", File = srcRaster, BandIndex = 0 });//1
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr500_RefSB", File = srcRaster, BandIndex = 1 });//2
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_RefSB", File = srcRaster, BandIndex = 0 });//3
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_RefSB", File = srcRaster, BandIndex = 1 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_RefSB", File = srcRaster, BandIndex = 2 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_RefSB", File = srcRaster, BandIndex = 3 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_RefSB", File = srcRaster, BandIndex = 4 });//7                
                prjSettings.BandMapTable = bandmapList;
            }
        }

        private void TryCreateDefaultArgsQKM(IRasterDataProvider srcRaster, EOS_MODIS_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = _srcSpatialRef;
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
                prjSettings.OutFormat = "LDF";
            if (prjSettings.OutResolutionX == 0 || prjSettings.OutResolutionY == 0)
            {
                if (dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjSettings.OutResolutionX = 0.0025F;
                    prjSettings.OutResolutionY = 0.0025F;
                }
                else
                {
                    prjSettings.OutResolutionX = 250F;
                    prjSettings.OutResolutionY = 250F;
                }
            }
            if (prjSettings.BandMapTable == null || prjSettings.BandMapTable.Count == 0)
            {
                List<BandMap> bandmapList = new List<BandMap>();
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_RefSB", File = srcRaster, BandIndex = 0 });//1
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_RefSB", File = srcRaster, BandIndex = 1 });//2
                prjSettings.BandMapTable = bandmapList;
            }
        }
        
        private string _fileAliasName;
        private string CheckFile(IRasterDataProvider srcRaster)
        {
            IBandProvider band = srcRaster.BandProvider;
            Dictionary<string, string> filaAttrs = band.GetAttributes();
            if (filaAttrs == null || !filaAttrs.ContainsKey("Satellite"))
                throw new Exception("不能确认为合法的EOS MODIS轨道数据，尝试获取文件属性Satellite的值为空");
            _fileAliasName = filaAttrs["Satellite"];
            if (string.IsNullOrWhiteSpace(_fileAliasName) || (_fileAliasName != "Terra" && _fileAliasName != "Aqua"))
                return "";
            else
            {
                string[] datasets = band.GetDatasetNames();
                if (datasets.Contains("EV_1KM_RefSB") &&
                    datasets.Contains("EV_250_Aggr1km_RefSB") &&
                    datasets.Contains("EV_500_Aggr1km_RefSB"))
                    return "1000";
                else if (datasets.Contains("EV_500_RefSB") &&
                    datasets.Contains("EV_250_Aggr500_RefSB") &&
                    datasets.Contains("EV_500_Aggr1km_RefSB"))
                    return "0500";
                else if (datasets.Contains("EV_250_RefSB"))
                    return "0250";
            }
            return "";
        }

        public override void ComputeDstEnvelope(IRasterDataProvider loncationRaster, ISpatialReference dstSpatialRef, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (loncationRaster != null)
            {
                Size LoncationSize;
                double[] xs, ys;
                ReadyLocations(loncationRaster, dstSpatialRef, out xs, out ys, out maxPrjEnvelope, out LoncationSize, progressCallback);
            }
            else
            {
                maxPrjEnvelope = PrjEnvelope.Empty;
            }
        }

        private void Project0500(IRasterDataProvider srcRaster, Size srcLocationSize, EOS_MODIS_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            PrjEnvelope envelops = prjSettings.OutEnvelope;
            if (envelops.IntersectsWith(_maxPrjEnvelope))
            {
                switch (prjSettings.OutFormat)
                {
                    case "LDF":
                        ProjectToLDF0500(srcRaster,srcLocationSize, prjSettings, dstSpatialRef, progressCallback);
                        break;
                    case "MEMORY":
                    default:
                        throw new NotSupportedException(string.Format("暂不支持的输出格式", prjSettings.OutFormat));
                }
            }
            else
                throw new Exception("数据不在目标区间内");
        }

        private void ProjectToLDF0500(IRasterDataProvider srcRaster, Size srcLocationSize, EOS_MODIS_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (srcRaster == null || srcRaster.Width == 0 || srcRaster.Height == 0)
                throw new Exception("投影250米MERSI数据失败：无法读取源数据,或者源数据高或宽为0。");
            Size srcDataSize = new Size(srcRaster.Width, srcRaster.Height);
            string outFormat = prjSettings.OutFormat;
            string outfilename = prjSettings.OutPathAndFileName;
            string dstProj4 = dstSpatialRef.ToProj4String();
            List<BandMap> bandMaps = prjSettings.BandMapTable;
            int dstBandCount = bandMaps.Count;
            Size dstSize = prjSettings.OutSize;
            using (IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
            {
                PrjEnvelope dstEnvelope = prjSettings.OutEnvelope;
                string mapInfo = "MAPINFO={" + 1 + "," + 1 + "}:{" + dstEnvelope.MinX + "," + dstEnvelope.MaxY + "}:{" + prjSettings.OutResolutionX + "," + prjSettings.OutResolutionY + "}";
                using (IRasterDataProvider prdWriter = drv.Create(outfilename, dstSize.Width, dstSize.Height, dstBandCount,
                    enumDataType.UInt16, "INTERLEAVE=BSQ", "VERSION=LDF", "SPATIALREF=" + dstProj4, mapInfo, "WITHHDR=TRUE") as IRasterDataProvider)
                {
                    //考虑分块
                    float outResolutionX = prjSettings.OutResolutionX;
                    float outResolutionY = prjSettings.OutResolutionY;
                    Size outSize = dstEnvelope.GetSize(outResolutionX, outResolutionY);
                    #region 计算分块个数、分块高宽
                    int blockXNum;
                    int blockYNum;
                    int blockWidth;
                    int blockHeight;
                    GetBlockNumber(outSize, out blockXNum, out blockYNum, out blockWidth, out blockHeight);
                    #endregion
                    Size blockSize = new Size(blockWidth, blockHeight);
                    for (int blockYIndex = 0; blockYIndex < blockYNum; blockYIndex++)
                    {
                        for (int blockXIndex = 0; blockXIndex < blockXNum; blockXIndex++)
                        {
                            //当前块的四角范围
                            double blockMinX = dstEnvelope.MinX + blockWidth * outResolutionX * blockXIndex;
                            double blockMaxX = blockMinX + blockWidth * outResolutionX;
                            double blockMaxY = dstEnvelope.MaxY - blockHeight * outResolutionY * blockYIndex;
                            double blockMinY = blockMaxY - blockHeight * outResolutionY;
                            PrjEnvelope blockEnvelope = new PrjEnvelope(blockMinX, blockMaxX, blockMinY, blockMaxY);
                            UInt16[] dstRowLookUpTable = new UInt16[blockWidth * blockHeight];
                            UInt16[] dstColLookUpTable = new UInt16[blockWidth * blockHeight];
                            _rasterProjector.ComputeIndexMapTable(_xs, _ys, srcLocationSize, srcDataSize, blockSize, blockEnvelope,
                                out dstRowLookUpTable, out dstColLookUpTable, progressCallback);
                            //执行投影
                            UInt16[] srcBandData = null;
                            UInt16[] dstBandData = new UInt16[blockWidth * blockHeight];
                            for (int i = 0; i < dstBandCount; i++)  //读取原始通道值，投影到目标区域
                            {
                                BandMap bandMap = bandMaps[i];
                                ReadBandData(out srcBandData, bandMap.File, bandMap.DatasetName, bandMap.BandIndex, srcDataSize);
                                if (prjSettings.IsRadiation)        //亮温转换，这里处理的不好，如果是做了一个小块的分幅，这里花的代价太高,
                                    DoRadiation(srcBandData, srcDataSize,srcLocationSize, bandMap.DatasetName, bandMap.BandIndex, srcRaster, prjSettings.IsRadiation, prjSettings.IsSolarZenith);
                                _rasterProjector.Project<UInt16>(srcBandData, srcDataSize, dstRowLookUpTable, dstColLookUpTable, blockSize, dstBandData, 0, progressCallback);
                                using (IRasterBand band = prdWriter.GetRasterBand(i + 1))
                                {
                                    unsafe
                                    {
                                        fixed (UInt16* ptr = dstBandData)
                                        {
                                            IntPtr bufferPtr = new IntPtr(ptr);
                                            int blockOffsetY = blockYIndex * blockSize.Height;
                                            int blockOffsetX = blockWidth * blockXIndex;
                                            band.Write(blockOffsetX, blockOffsetY, blockWidth, blockHeight, bufferPtr, enumDataType.UInt16, blockWidth, blockHeight);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GetBlockNumber(Size size, out int blockXNum, out int blockYNum, out int blockWidth, out int blockHeight)
        {
            blockXNum = 1;
            blockYNum = 1;
            blockWidth = size.Width;
            blockHeight = size.Height;
            if (size.Width * size.Height <= 8000 * 8000)
                return;
            int MaxX = 18000;
            int MaxY = 4000;
            if (size.Width < size.Height)
            {
                MaxX = 4000;
                MaxY = 18000;
            }
            while (blockWidth >= MaxX)
            {
                blockXNum++;
                blockWidth = (int)Math.Floor((double)size.Width / blockXNum);
            }
            while (blockHeight >= MaxY)
            {
                blockYNum++;
                blockHeight = (int)Math.Floor((double)size.Height / blockYNum);
            }
        }

        private void ReadBandData(out UInt16[] bandData, IRasterDataProvider srcRaster, string bandName, int bandNumber, Size srcSize)
        {
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            {
                IRasterBand[] latBands = srcbandpro.GetBands(bandName);
                using (IRasterBand latBand = latBands[bandNumber])
                {
                    bandData = new UInt16[latBand.Width * latBand.Height];
                    unsafe
                    {
                        fixed (UInt16* ptr = bandData)
                        {
                            IntPtr bufferptr = new IntPtr(ptr);
                            latBand.Read(0, 0, latBand.Width, latBand.Height, bufferptr, enumDataType.UInt16, latBand.Width, latBand.Height);
                        }
                    }
                }
            }
        }

    }
}
