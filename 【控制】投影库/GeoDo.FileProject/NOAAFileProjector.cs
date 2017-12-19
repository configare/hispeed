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
    internal class NOAAFileProjector : FileProjector
    {
        //太阳天顶角订正？=(定标值)/cos(太阳天顶角/100.0/180*3.14159);
        //一个完整的圆的弧度是2π,1 π rad = 180°,弧度=角度*PI/180
        //1°代表的弧度rad=π /180,(后面又除了100是由于用到的角度值放大了100倍)
        private const double DEG_TO_RAD_P100 = 0.000174532925199432955; // (PI/180)/100;
        private const double RAD_TO_DEG = 57.29577951308232087;        // 180/PI
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

        #region 定标变量
        //反射通道定标系数，定长14，依次为：Scalech1、Offsetch1、Scalech2、Offsetch2、Scalech6、Offsetch6、Scalech7、Offsetch7、Scalech8、Offsetch8、Scalech9、Offsetch9、Scalech10、Offsetch10
        //定标值？= offSet+scale*观测值
        private float[] _refSB_Cal_Coefficients = null;
        /// <summary>太阳天顶角数据集(有效区间0-18000),角度值放大了100倍</summary>
        private Int16[] _solarZenithData = null;
        /// <summary>定标辐亮度值订正系数,只用到前9个数值，分别为：CH3的b0、b1、b2、CH4的b0、b1、b2和CH5的b0、b1、b2</summary>
        private float[] _prelaunch_Nonlinear_Coefficients = null;
        private float[] _emissive_Radiance_Scales = null;   //new float[3*height]
        private float[] _emissive_Radiance_Offsets = null;
        private float[] _emmisive_Centroid_Wave_Number = new float[] { 2699.119F, 923.427053F, 830.241775F };//v值
        private float[] _emmisive_BT_Coefficients = new float[] { 2.05806810F, 0.98231665F, 0.20002536F, 0.99791678F, 0.13149949F, 0.99820459F};//A、B值
        #endregion

        IRasterProjector _rasterProjector = null;
        ISpatialReference _srcSpatialRef = null;

        #region Session
        PrjEnvelope _maxPrjEnvelope = null;
        double[] _xs = null;    //存储的实际是计算后的值
        double[] _ys = null;    //存储的实际是计算后的值
        #endregion

        public NOAAFileProjector()
            : base()
        {
            _name = "NOAA";
            _fullname = "NOAA轨道文件投影";
            _rasterProjector = new RasterProjector();
            //_srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
            _srcSpatialRef = new SpatialReference(new GeographicCoordSystem()); //SpatialReference.GetDefault(); //
        }

        public override void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster");
            if (prjSettings == null)
                throw new ArgumentNullException("prjSettings");
            FY3_VIRR_PrjSettings fy3prjSettings = prjSettings as FY3_VIRR_PrjSettings;
            TryCreateDefaultArgs(srcRaster, fy3prjSettings,ref dstSpatialRef);
            DoSession(srcRaster, dstSpatialRef, fy3prjSettings, progressCallback);
            if (prjSettings.OutEnvelope == null || prjSettings.OutEnvelope == PrjEnvelope.Empty)
                prjSettings.OutEnvelope = _maxPrjEnvelope;
            //地理坐标投影,下面简单的对地理坐标投影的范围作了限制，不大严谨，仅适合目前的状况。
            if (dstSpatialRef.ProjectionCoordSystem == null && (prjSettings.OutEnvelope.MaxY > 80||prjSettings.OutEnvelope.MaxY < -80))   
                throw new Exception(string.Format("高纬度数据，不适合投影为等经纬度数据[{0}]", _maxPrjEnvelope));      
            Project(srcRaster, fy3prjSettings, dstSpatialRef, progressCallback);
        }

        private void DoSession(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef,FY3_VIRR_PrjSettings fy3prjSettings, Action<int, string> progressCallback)
        {
            if (_curSession != null && _curSession != srcRaster)
                EndSession();
            if (_curSession == null)
            {
                Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                ReadyLocations(srcRaster, dstSpatialRef, srcSize, out _xs, out _ys, out _maxPrjEnvelope, progressCallback);
                if (fy3prjSettings.IsRadiation)
                    ReadyRadiationArgs(srcRaster);
                if (fy3prjSettings.IsSolarZenith)
                    ReadySolarZenithArgs(srcRaster);
            }
        }

        private void Project(IRasterDataProvider srcRaster, FY3_VIRR_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
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
        /// 2、对所有波段执行投影
        /// </summary>
        private void ProjectToLDF(IRasterDataProvider srcRaster, FY3_VIRR_PrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
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
                string mapInfo = "MAPINFO={" + 1 + "," + 1 + "}:{" + dstEnvelope.MinX + "," + dstEnvelope.MaxY + "}:{" + prjSettings.OutResolutionX + "," + prjSettings.OutResolutionY + "}";
                using (IRasterDataProvider prdWriter = drv.Create(outfilename, dstSize.Width, dstSize.Height, dstBandCount,
                    enumDataType.UInt16, "INTERLEAVE=BSQ", "VERSION=LDF", "SPATIALREF=" + dstProj4, mapInfo, "WITHHDR=TRUE") as IRasterDataProvider)
                {
                    //计算查找表
                    //ISpatialReference srcSpatialRef = srcRaster.SpatialRef;
                    UInt16[] dstRowLookUpTable = new UInt16[dstSize.Width * dstSize.Height];
                    UInt16[] dstColLookUpTable = new UInt16[dstSize.Width * dstSize.Height];
                    _rasterProjector.ComputeIndexMapTable(_xs, _ys, srcSize, dstSize, dstEnvelope,_maxPrjEnvelope,
                        out dstRowLookUpTable, out dstColLookUpTable, progressCallback);
                    //执行投影
                    UInt16[] srcBandData = new UInt16[srcSize.Width * srcSize.Height];
                    UInt16[] dstBandData = new UInt16[dstSize.Width * dstSize.Height];
                    int progress=0;
                    for (int i = 0; i < dstBandCount; i++)      //读取原始通道值，投影到目标区域
                    {
                        if (progressCallback != null)
                        {
                            progress = (int)((i + 1)*100 / dstBandCount);
                            progressCallback(progress, string.Format("投影第{0}共{1}通道", i + 1, dstBandCount));
                        }
                        BandMap bandMap = bandMaps[i];
                        ReadBandData(srcBandData, bandMap.File, bandMap.DatasetName, bandMap.BandIndex, srcSize);
                        if (prjSettings.IsRadiation)
                            DoRadiation(srcBandData, srcSize, bandMap.DatasetName, bandMap.BandIndex, prjSettings.IsSolarZenith);
                        _rasterProjector.Project<UInt16>(srcBandData, srcSize, dstRowLookUpTable, dstColLookUpTable, dstSize, dstBandData, 0, progressCallback);
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

        /// <summary> 辐射值计算</summary>
        private void DoRadiation(ushort[] srcBandData, Size srcSize, string datasetName, int bandIndex, bool isSolarZenith)
        {
            switch (datasetName)
	        {
                case EV_RefSB:
                    {
                        float scale = _refSB_Cal_Coefficients[bandIndex * 2];
                        float offSet = _refSB_Cal_Coefficients[bandIndex * 2 + 1];
                        Parallel.For(0, srcBandData.Length,(index) =>
                        {
                            short solarZenithData = _solarZenithData[index];
                            if (solarZenithData > 0 && solarZenithData < 18000)
                            {
                                double radiation = scale * srcBandData[index] + offSet;
                                if (isSolarZenith)                  //做高度角订正
                                    srcBandData[index] = (UInt16)(10 * radiation / Math.Cos(solarZenithData * DEG_TO_RAD_P100));
                                else
                                    srcBandData[index] = (UInt16)(10 * radiation);
                                if (srcBandData[index] > 65000)     //理论上讲反射率应当是0-100
                                    srcBandData[index] = 0;
                            }
                            else
                                srcBandData[index] = 0;
                        });
                    }
                    break;
                case EV_Emissive:
                    {
                        float b0 = _prelaunch_Nonlinear_Coefficients[bandIndex * 3];
                        float b1 = _prelaunch_Nonlinear_Coefficients[bandIndex * 3 + 1];
                        float b2 = _prelaunch_Nonlinear_Coefficients[bandIndex * 3 + 2];
                        float v = _emmisive_Centroid_Wave_Number[bandIndex];
                        double v3 = v*v*v;
                        double c2V = C2 * v;
                        double c1V3 = C1 * v3;
                        float A = _emmisive_BT_Coefficients[bandIndex*2];
                        float B = _emmisive_BT_Coefficients[bandIndex*2+1];
                        int height = srcSize.Height;
                        int width = srcSize.Width;
                        Parallel.For(0, height, (i) =>
                        {
                            float scale = _emissive_Radiance_Scales[i * 3 + bandIndex];
                            float offset = _emissive_Radiance_Offsets[i * 3 + bandIndex];
                            int index;
                            UInt16 dn;
                            double radiationLIN;
                            double radiation;
                            double temperatureBB;
                            for (int j = 0; j < srcSize.Width; j++)
                            {
                                index = i * width + j;
                                dn = srcBandData[index];
                                if (dn > 0 && dn < 50000)//EV_Emissive的有效值区间,理论上亮温应当在150K~350K之间
                                {
                                    radiationLIN = (dn * scale + offset);
                                    radiation = (b0 + (b1 + 1) * radiationLIN + b2 * radiationLIN * radiationLIN);
                                    temperatureBB = 10 * (C2 * v / Math.Log(1 + c1V3 / radiation) - A) / B;
                                    srcBandData[index] = (UInt16)temperatureBB;
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

        /// <summary> 读取通道值</summary>
        private void ReadBandData(UInt16[] bandData,IRasterDataProvider srcRaster, string bandName, int bandNumber, Size srcSize)
        {
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            {
                using (IRasterBand latBand = srcbandpro.GetBands(bandName)[bandNumber])
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

        /// <summary> 准备定位信息,计算投影后的值，并计算范围</summary>
        private void ReadyLocations(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, Size srcSize,
            out double[] xs, out double[] ys, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            ReadLongitudeLatitude(srcRaster, out xs, out ys);
            _rasterProjector.ComputeDstEnvelope(_srcSpatialRef, xs, ys, srcSize, dstSpatialRef, out maxPrjEnvelope, progressCallback);
        }

        //读取定位信息(经纬度数据集)
        private void ReadLongitudeLatitude(IRasterDataProvider srcRaster, out double[] longitudes, out double[] latitudes)
        {
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            {
                Size srSize = new System.Drawing.Size(srcRaster.Width, srcRaster.Height);
                longitudes = new Double[srcRaster.Width * srcRaster.Height];
                latitudes = new Double[srcRaster.Width * srcRaster.Height];
                using (IRasterBand lonsBand = srcbandpro.GetBands(Longitude)[0])
                {
                    using (IRasterBand latBand = srcbandpro.GetBands(Latitude)[0])
                    {
                        unsafe
                        {
                            fixed (Double* ptrLong = longitudes)
                            fixed (Double* ptrLat = latitudes)
                            {
                                {
                                    IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                    IntPtr bufferPtrLong = new IntPtr(ptrLong);
                                    latBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLat, enumDataType.Double, srSize.Width, srSize.Height);
                                    lonsBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLong, enumDataType.Double, srSize.Width, srSize.Height);
                                }
                            }
                        }
                    }
                }
            }
        }

        #region 定标参数读取
        //准备[辐射定标]参数
        private void ReadyRadiationArgs(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取亮温转换参数失败");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                _refSB_Cal_Coefficients = ReadFileAttributeToFloat(srcbandpro, RefSB_Cal_Coefficients, 14);
                _prelaunch_Nonlinear_Coefficients = ReadFileAttributeToFloat(srcbandpro, Prelaunch_Nonlinear_Coefficients, 9);
                Size srcSize = new System.Drawing.Size(srcRaster.Width, srcRaster.Height);
                _solarZenithData = ReadDataSetToInt16(srcbandpro, srcSize, SolarZenith, 0);
                _emissive_Radiance_Scales = ReadDataSetToSingle(srcbandpro, new Size(3, srcSize.Height), Emissive_Radiance_Scales, 0);
                _emissive_Radiance_Offsets = ReadDataSetToSingle(srcbandpro, new Size(3, srcSize.Height), Emissive_Radiance_Offsets, 0);
            }
            catch (Exception ex)
            {
                throw new Exception("获取亮温转换参数失败", ex.InnerException);
            }
        }

        //准备[太阳高度角订正]参数,目前还没有太阳高度角订正的公式。
        private void ReadySolarZenithArgs(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取太阳天顶角数据失败");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                Size srcSize = new System.Drawing.Size(srcRaster.Width, srcRaster.Height);
                _solarZenithData = ReadDataSetToInt16(srcbandpro, srcSize, SolarZenith, 0);
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

        private Int16[] ReadDataSetToInt16(IBandProvider srcbandpro, Size srcSize, string dataSetName, int bandIndex)
        {
            Int16[] data = new Int16[srcSize.Width * srcSize.Height];
            using (IRasterBand rasterBand = srcbandpro.GetBands(dataSetName)[0])
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

        public override IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, IRasterDataProvider dstRaster, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override FilePrjSettings CreateDefaultPrjSettings()
        {
            return new FY3_VIRR_PrjSettings();
        }

        private void TryCreateDefaultArgs(IRasterDataProvider srcRaster, FY3_VIRR_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
        {
            if(dstSpatialRef==null)
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
                bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 0 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 1 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_Emissive", File = srcRaster, BandIndex = 0 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_Emissive", File = srcRaster, BandIndex = 1 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_Emissive", File = srcRaster, BandIndex = 2 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 2 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 3 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 4 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 5 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 6 });
                prjSettings.BandMapTable = bandmapList;
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
    }
}
