using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal static class CreateArgument
    {
        public static VertifyFirPixelFiter CreateVertifyFilter(IArgumentProvider argProvider, IContextMessage contextMessage)
        {
            int minWndSize = Obj2Int(argProvider.GetArg("BackWndMin"));
            int maxWndSize = Obj2Int(argProvider.GetArg("BackWndMax"));
            IBandNameRaster bandNameRaster = argProvider.DataProvider as IBandNameRaster;
            int visBandNo = TryGetBandNo(argProvider,bandNameRaster,"Visible");
            int midIfrBandNo = TryGetBandNo(argProvider,bandNameRaster,"MiddleInfrared");
            int farIfrBandNo = TryGetBandNo(argProvider,bandNameRaster,"FarInfrared");
            if (visBandNo < 1 || midIfrBandNo < 1 || farIfrBandNo < 1)
            {
                PrintInfo(contextMessage, "     获取波段序号(可见光、中红外、远红外)时发生错误。");
                return null;
            }
            float bandZoom = Obj2Float(argProvider.GetArg("MiddleInfrared_Zoom"));
            if (bandZoom < float.Epsilon)
                PrintInfo(contextMessage, "     波段数值缩放参数设置错误,使用缺省值1。");
            //
            int maxFarIfrValue_fir = Obj2Int(argProvider.GetArg("Vertify_MaxFarIfr"));
            int minVisValue_fir = Obj2Int(argProvider.GetArg("Vertify_MinVis"));
            int secondMidIfrValue_fir = GetSecondMidIfr(argProvider);
            int backTmpFactor_fir = Obj2Int(argProvider.GetArg("Vertify_BackTmpFactor"));
            //
            float minSolarZentih = Obj2Float(argProvider.GetArg("BackTSolarZenith"));
            float midFarRatio = Obj2Float(argProvider.GetArg("Vertify_MidFarRatio"));
            //
            bool isUsedMidFar = bool.Parse(argProvider.GetArg("isUsedMidFar").ToString());
            float defaultPercent = Obj2Float(argProvider.GetArg("defaultPercent"));
            return new VertifyFirPixelFiter(farIfrBandNo, midIfrBandNo, visBandNo, bandZoom, maxFarIfrValue_fir, minVisValue_fir, secondMidIfrValue_fir, backTmpFactor_fir, midFarRatio, isUsedMidFar, defaultPercent);
        }

        private static int GetSecondMidIfr(IArgumentProvider argProvider)
        {
            RasterIdentify rid = new RasterIdentify(argProvider.DataProvider.fileName);
            if (rid == null || string.IsNullOrEmpty(rid.Satellite))
                return 3300;
            if (rid.Satellite.IndexOf("FY3") != -1)
                return Obj2Int(argProvider.GetArg("Vertify_SecondMidIfr"));
            if (rid.Satellite.IndexOf("NOAA") != -1)
                return 3360;
            return 3300;
        }

        public static BackTmpComputer CreateBackTmpComputer(IArgumentProvider argProvider, IBackTmpComputerHelper helper, IContextMessage contextMessage)
        {
            int minWndSize = Obj2Int(argProvider.GetArg("BackWndMin"));
            int maxWndSize = Obj2Int(argProvider.GetArg("BackWndMax"));
            int localBackTmpMax = Obj2Int(argProvider.GetArg("BackTmp_localBackTmpMax"));
            int localBackTmpMin = Obj2Int(argProvider.GetArg("BackTmp_localBackTmpMin"));
            int glaringVIRR = Obj2Int(argProvider.GetArg("BackTmp_GlaringVIRR"));
            int wildernessCorrect = Obj2Int(argProvider.GetArg("BackTmp_WildernessCorrect"));
            int maxHitedPixelCount = Obj2Int(argProvider.GetArg("BackTmp_SkyPixelCount"));
            IBandNameRaster bandNameRaster = argProvider.DataProvider as IBandNameRaster;
            int visBandNo = TryGetBandNo(argProvider,bandNameRaster,"Visible");
            int nearIfrBandNo = TryGetBandNo(argProvider, bandNameRaster, "NearInfrared");
            int midIfrBandNo = TryGetBandNo(argProvider, bandNameRaster, "MiddleInfrared");
            int farIfrBandNo = TryGetBandNo(argProvider,bandNameRaster,"FarInfrared");
            float fireGradeLevel1 = Obj2Float(argProvider.GetArg("Intensity_Grade1"));
            float fireGradeLevel2 = Obj2Float(argProvider.GetArg("Intensity_Grade2"));
            float fireGradeLevel3 = Obj2Float(argProvider.GetArg("Intensity_Grade3"));
            float fireGradeLevel4 = Obj2Float(argProvider.GetArg("Intensity_Grade4"));
            float fireGradeLevel5 = Obj2Float(argProvider.GetArg("Intensity_Grade5"));
            float vis_FirReliability = Obj2Float(argProvider.GetArg("Vis_FirReliability"));
            float midIfr_farInfr_FirReliability = Obj2Float(argProvider.GetArg("MidIfr_farInfr_FirReliability"));
            float midIfr_FirReliability = Obj2Float(argProvider.GetArg("MidIfr_FirReliability"));
            bool isNight = bool.Parse(argProvider.GetArg("IsNight").ToString());
            if (!isNight && (visBandNo < 1 || midIfrBandNo < 1 || farIfrBandNo < 1 || nearIfrBandNo < 1))
            {
                PrintInfo(contextMessage, "     获取波段序号(可见光、近红外、中红外、远红外)时发生错误。");
                return null;
            }
            else if (isNight && (farIfrBandNo < 1 || midIfrBandNo < 1))
            {
                PrintInfo(contextMessage, "     获取波段序号(可见光、近红外、中红外、远红外)时发生错误。");
                return null;
            }
            float bandZoom = Obj2Float(argProvider.GetArg("MiddleInfrared_Zoom"));
            if (bandZoom < float.Epsilon)
                PrintInfo(contextMessage, "     波段数值缩放参数设置错误,使用缺省值1。");
            ISolarZenithProvider solarZentihProvider = null;
            float minSolarZentih = Obj2Float(argProvider.GetArg("BackTmp_SolarAzimuthMin"));
            float maxSolarZentih = Obj2Float(argProvider.GetArg("BackTmp_SolarAzimuthMax"));
            //亚像元面积计算
            float midIfrCenterWaveNum = Obj2Float(argProvider.GetArg("MiddleInfrared_CenterWaveNum"));
            float farIfrCenterWaveNum = Obj2Float(argProvider.GetArg("FarInfrared_CenterWaveNum"));
            TryCorrectCenterWaveNum(argProvider.DataProvider, midIfrBandNo, farIfrBandNo, ref midIfrCenterWaveNum, ref farIfrCenterWaveNum, contextMessage);
            if (midIfrCenterWaveNum < float.Epsilon || farIfrCenterWaveNum < float.Epsilon)
            {
                PrintInfo(contextMessage, "     获取中红外、远红外中心波数失败,忽略亚像元面积、火点强度计算。");
            }
            int midIfrMaxMidIfrValue = Obj2Int(argProvider.GetArg("BackTmp_MaxMidIfrValue"));
            float firComputeFactor = Obj2Float(argProvider.GetArg("BackTmp_FirComputeFactor"));
            float firIntensityFactor = Obj2Float(argProvider.GetArg("BackTmp_FirIntensityFactor"));
            //
            return new BackTmpComputer(minWndSize, maxWndSize,
                maxHitedPixelCount, solarZentihProvider, minSolarZentih, maxSolarZentih,
                farIfrBandNo, midIfrBandNo, visBandNo, nearIfrBandNo, bandZoom,
                midIfrCenterWaveNum, farIfrCenterWaveNum,
                midIfrMaxMidIfrValue, firComputeFactor, firIntensityFactor,
                localBackTmpMax, localBackTmpMin, glaringVIRR, wildernessCorrect, helper,
                fireGradeLevel1, fireGradeLevel2, fireGradeLevel3, fireGradeLevel4, fireGradeLevel5,
                vis_FirReliability, midIfr_farInfr_FirReliability, midIfr_FirReliability);
        }

        public static void TryCorrectCenterWaveNum(RSS.Core.DF.IRasterDataProvider dataProvider, int midIfrBandNo, int farIfrBandNo, ref float midIfrCenterWaveNum, ref float farIfrCenterWaveNum, IContextMessage contextMessage)
        {
            DataIdentify id = dataProvider.DataIdentify;
            if (id != null && !string.IsNullOrEmpty(id.Satellite) && !string.IsNullOrEmpty(id.Sensor))
            {
                BandnameRefTable tb = BandRefTableHelper.GetBandRefTable(id.Satellite, id.Sensor);
                if (tb != null)
                {
                    float wn = 0;
                    BandnameItem item = tb.GetBandItem(midIfrBandNo);
                    if (item != null)
                    {
                        wn = item.CenterWaveNumber;
                        if (wn > float.Epsilon)
                            midIfrCenterWaveNum = wn;
                    }
                    item = tb.GetBandItem(farIfrBandNo);
                    if (item != null)
                    {
                        wn = item.CenterWaveNumber;
                        if (wn > float.Epsilon)
                            farIfrCenterWaveNum = wn;
                    }
                }
            }
            //
            if (farIfrCenterWaveNum < float.Epsilon)
            {
                farIfrCenterWaveNum = 912;
                PrintInfo(contextMessage, "     通过卫星、传感器获远红外中心波数失败,使用缺省值:912。");
            }
            if (midIfrCenterWaveNum < float.Epsilon)
            {
                midIfrCenterWaveNum = 2640;
                PrintInfo(contextMessage, "     通过卫星、传感器获中红外中心波数失败,使用缺省值:2640。");
            }
        }

        public static DoubtFirPixelFilter CreateDoubtFilter(IArgumentProvider argProvider, IContextMessage contextMessage)
        {
            int minWndSize = Obj2Int(argProvider.GetArg("BackWndMin"));
            int maxWndSize = Obj2Int(argProvider.GetArg("BackWndMax"));
            IBandNameRaster bandNameRaster = argProvider.DataProvider as IBandNameRaster;
            int visBandNo = TryGetBandNo(argProvider,bandNameRaster,"Visible");
            int nearIfrBandNo = TryGetBandNo(argProvider,bandNameRaster,"NearInfrared");
            int midIfrBandNo = TryGetBandNo(argProvider,bandNameRaster,"MiddleInfrared");
            int farIfrBandNo = TryGetBandNo(argProvider, bandNameRaster, "FarInfrared");
            bool isNight = bool.Parse(argProvider.GetArg("IsNight").ToString());
            if (!isNight && (visBandNo < 1 || midIfrBandNo < 1 || farIfrBandNo < 1 || nearIfrBandNo < 1))
            {
                PrintInfo(contextMessage, "     获取波段序号(可见光、近红外、中红外、远红外)时发生错误。");
                return null;
            }
            else if (isNight && (farIfrBandNo < 1 || midIfrBandNo < 1))
            {
                PrintInfo(contextMessage, "     获取波段序号(可见光、近红外、中红外、远红外)时发生错误。");
                return null;
            }
            float bandZoom = Obj2Float(argProvider.GetArg("MiddleInfrared_Zoom"));
            int hitedPixelCount = Obj2Int(argProvider.GetArg("HitedPixelCount"));
            ISolarZenithProvider solarZentihProvider = null;
            float minSolarZentih = Obj2Float(argProvider.GetArg("BackTSolarZenith"));
            if (bandZoom < float.Epsilon)
            {
                PrintInfo(contextMessage, "     波段数值缩放参数设置错误,使用缺省值1。");
            }
            //water
            int minMidInfraredValue_water = Obj2Int(argProvider.GetArg("Water_MinMidIfr"));
            int minNearInfraredValue_water = Obj2Int(argProvider.GetArg("Water_MinNearIfr"));
            int nearInfrared_visibleValue_water = Obj2Int(argProvider.GetArg("Water_NearIfr_Vis"));
            //cloud
            int minVisibleValue_cloud = Obj2Int(argProvider.GetArg("Cloud_MinVis"));
            int farInfraredValue_cloud = Obj2Int(argProvider.GetArg("Cloud_FarIfr"));
            //doubt
            int midInfraredValue_DoubtFir = Obj2Int(argProvider.GetArg("Doubt_MidIfr"));
            int dltMidInfraredValue_DoubtFir = Obj2Int(argProvider.GetArg("Doubt_MidIfr_Diff"));
            int midInfrared_farInfrared_DoubtFir = Obj2Int(argProvider.GetArg("Doubt_MidIfr_FarIfr"));
            int minMidInfraredValue_DoubtFir = Obj2Int(argProvider.GetArg("Doubt_MinMidIfr"));
            int maxHTmpPixelCount_DoubtFir = Obj2Int(argProvider.GetArg("Doubt_MaxHTmpPixelCount"));
            int midInfraredAvg_DoubtFir = Obj2Int(argProvider.GetArg("Doubt_MidIfrAvg"));
            //
            DoubtFirPixelFilter filter = new DoubtFirPixelFilter
                (
                //wnd size
                minWndSize, maxWndSize,
                //bandNo
                farIfrBandNo, midIfrBandNo, nearIfrBandNo, visBandNo,
                //zoom
                bandZoom,
                solarZentihProvider,
                //
                hitedPixelCount,
                minSolarZentih,
                //water
                minMidInfraredValue_water,
                minNearInfraredValue_water,
                nearInfrared_visibleValue_water,
                //cloud
                minVisibleValue_cloud,
                farInfraredValue_cloud,
                //doubt fir
                midInfraredValue_DoubtFir,
                dltMidInfraredValue_DoubtFir,
                midInfrared_farInfrared_DoubtFir,
                minMidInfraredValue_DoubtFir,
                maxHTmpPixelCount_DoubtFir,
                midInfraredAvg_DoubtFir
                );
            return filter;
        }


        public static ICandidatePixelFilter CreateAbnormalHighTmpFile(IArgumentProvider argProvider, IContextMessage contextMessage)
        {
            float bandZoom = Obj2Float(argProvider.GetArg("MiddleInfrared_Zoom"));
            if (bandZoom < float.Epsilon)
            {
                PrintInfo(contextMessage, "     中红外波段数值缩放参数设置错误。");
                return null;
            }
            IBandNameRaster bandNameRaster = argProvider.DataProvider as IBandNameRaster;
            int midInfraredBandNo = TryGetBandNo(argProvider, bandNameRaster,"MiddleInfrared");
            if (midInfraredBandNo < 1)
            {
                PrintInfo(contextMessage, "     无法正确获取中红外波段序号。");
                return null;
            }
            int farInfraredBandNo = TryGetBandNo(argProvider, bandNameRaster,"FarInfrared");
            if (farInfraredBandNo < 1)
            {
                PrintInfo(contextMessage, "     无法正确获取远红外波段序号。");
                return null;
            }
            int maxWndSize = Obj2Int(argProvider.GetArg("HTWndMax"));
            if (maxWndSize < 7)
            {
                PrintInfo(contextMessage, "     未能获取正确的最大邻域窗口大小，设置为默认值7。");
                maxWndSize = 7;
            }
            int minWndSize = 7;
            int midInfraredMax = Obj2Int(argProvider.GetArg("HTMiddleInfraredMax"));
            if (midInfraredMax == 0)
            {
                PrintInfo(contextMessage, "     中红外最大值参数设置错误。");
                return null;
            }
            int dlt_mid_far = Obj2Int(argProvider.GetArg("HTMiddleFarInfraredDiff"));
            if (dlt_mid_far == 0)
            {
                PrintInfo(contextMessage, "     (中红外 - 远红外)差值阈值设置错误。");
                return null;
            }
            int mid_offset = Obj2Int(argProvider.GetArg("HTMiddleInfraredOffset"));
            if (mid_offset == 0)
            {
                PrintInfo(contextMessage, "     中红外偏移阈值设置错误。");
                return null;
            }
            int abnormalPixelCount = Obj2Int(argProvider.GetArg("HTAbnormalPixelCount"));
            if (abnormalPixelCount == 0)
            {
                PrintInfo(contextMessage, "     邻域内异常高温点数量阈值设置错误，设为默认值6。");
                abnormalPixelCount = 6;
            }
            AbnormalHTmpPixelFilter filter = new AbnormalHTmpPixelFilter(minWndSize,
                maxWndSize, midInfraredBandNo, farInfraredBandNo, bandZoom,
                mid_offset,
                midInfraredMax,
                dlt_mid_far,
                abnormalPixelCount,
                argProvider);
            return filter;
        }

        public static ICandidatePixelFilter CreateLandHighTmpFilter(IArgumentProvider argProvider, IContextMessage contextMessage)
        {
            IBandNameRaster bandNameRaster = argProvider.DataProvider as IBandNameRaster;
            int bandNo = TryGetBandNo(argProvider, bandNameRaster,"MiddleInfrared");
            if (bandNo == 0)
            {
                PrintInfo(contextMessage, "     获取中红外波段序号失败。");
                return null;
            }
            float bandZoom = Obj2Float(argProvider.GetArg("MiddleInfrared_Zoom"));
            float minTmp = Obj2Float(argProvider.GetArg("HTMiddleInfraredMin"));
            if (minTmp < float.Epsilon)
            {
                PrintInfo(contextMessage, "     x% 陆地高温点判识-中红外最小值阈值参数设置错误。");
                return null;
            }
            LandHTmpPixelFilter filter = new LandHTmpPixelFilter(bandNo, bandZoom, minTmp, argProvider);
            filter.Name = "     陆地高温点判断疑似火点";
            return filter;
        }

        public static ICandidatePixelFilter CreateHighTmpXPercentFilter(IArgumentProvider argProvider, IContextMessage contextMessage)
        {
            float percent = Obj2Float(argProvider.GetArg("HTPixelPercent"));
            if (percent < float.Epsilon)
            {
                PrintInfo(contextMessage, "     x% 高温像元(中红外通道)参数设置错误。");
                return null;
            }
            percent /= 100f;
            IBandNameRaster bandNameRaster = argProvider.DataProvider as IBandNameRaster;
            int bandNo = TryGetBandNo(argProvider,bandNameRaster,"MiddleInfrared");
            if (bandNo < 1)
            {
                PrintInfo(contextMessage, "     获取中红外波段序号失败。");
                return null;
            }
            ICandidatePixelFilter filter = PercentPixelFilterFactory.GetFilter(argProvider.DataProvider.DataType, bandNo, percent, false);
            if (filter == null)
            {
                PrintInfo(contextMessage, "     获取x% 高温像元(中红外通道)候选像元过滤器失败。");
                return null;
            }
            filter.Name = "     获取x% 高温像元(中红外通道)作为候选像元";
            return filter;
        }

        public static void PrintInfo(IContextMessage contextMessage, string info)
        {
            if (contextMessage != null)
                contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private static double Obj2Double(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return double.NaN;
            double d;
            if (double.TryParse(v.ToString(), out d))
                return d;
            return 0d;
        }

        private static float Obj2Float(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return float.NaN;
            float d;
            if (float.TryParse(v.ToString(), out d))
                return d;
            return 0f;
        }

        private static string Obj2String(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return null;
            return v.ToString();
        }

        private static int Obj2Int(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return -1;
            int d;
            if (int.TryParse(v.ToString(), out d))
                return d;
            return 0;
        }

        private static int TryGetBandNo(IArgumentProvider argumentProvider,IBandNameRaster bandNameRaster, string argName)
        {
            int bandNo = GetBandNoInt(argumentProvider.GetArg(argName));
            if (bandNameRaster != null)
            {
                int newbandNo = -1;
                if (bandNameRaster.TryGetBandNoFromBandName(bandNo, out newbandNo))
                    bandNo = newbandNo;
            }
            return bandNo;
        }

        private static int GetBandNoInt(object bandNoObj)
        {
            if (bandNoObj == null || string.IsNullOrEmpty(bandNoObj.ToString()))
                return -1;
            return (int)bandNoObj;
        }
    }
}
