using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class SubProductBinaryCLM : MonitoringSubProduct
    {
        //private IPixelIndexMapper _result;
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;

        public SubProductBinaryCLM()
        {
        }

        public SubProductBinaryCLM(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public IExtractResult UIMake(IArgumentProvider argumentProvider)
        {
            _curArguments = argumentProvider;
            return Make(null, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider != null)
                _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_curArguments.GetArg("AlgorithmName").ToString() == "CLMAlgorithm")
            {
                return CLMAlgorithm();
            }
            //蓝藻云判识入口，其他采用统一方法
            if (_curArguments.GetArg("AlgorithmName").ToString() == "BAGCLMAlgorithm")
            {
                return BAGAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }
        /// <summary>
        /// 蓝藻云判识实现
        /// 
        /// </summary>
        /// <returns></returns>
        private IExtractResult BAGAlgorithm()
        {
            Dictionary<string, string> dic = Obj2Dic(_curArguments.GetArg("ArgumentSetting"));
            //可见光波段
            int VisibleCH = Obj2int(_curArguments.GetArg("Visible"));
            //近红外波段
            int NearInfraredCH = Obj2int(_curArguments.GetArg("NearInfrared"));
            //短波红外波段
            int ShortInfraredCH = Obj2int(_curArguments.GetArg("ShortInfrared"));

            bool IsPrintFeature = bool.Parse(dic["PrintFeature"]);

            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            //尝试根据当前卫星载荷数据自动匹配波段信息
            if (bandNameRaster != null)
            {
                int newbandNo = -1;
                if (bandNameRaster.TryGetBandNoFromBandName(VisibleCH, out newbandNo))
                    VisibleCH = newbandNo;
                if (bandNameRaster.TryGetBandNoFromBandName(NearInfraredCH, out newbandNo))
                    NearInfraredCH = newbandNo;
                if (bandNameRaster.TryGetBandNoFromBandName(ShortInfraredCH, out newbandNo))
                    ShortInfraredCH = newbandNo;
            }
            //获取放大倍数信息
            double VisibleZoom = Obj2Double(_curArguments.GetArg("Visible_Zoom"));
            double NearInfraredZoom = Obj2Double(_curArguments.GetArg("NearInfrared_Zoom"));
            double ShortInfraredZoom = Obj2Double(_curArguments.GetArg("ShortInfrared_Zoom"));
            //获取界面配置值
            double minvisiable = double.Parse(dic["MinVisiable"]);
            double minnearinfrared = double.Parse(dic["MinNearInFrared"]);
            double maxnsvi = double.Parse(dic["MaxNSVI"]);
            double minndvi = double.Parse(dic["MinNDVI"]);


            //云判识增加 近红外/短波红外>c 的判断 
            // c=nsvi
            string nsvi = dic.ContainsKey("NSVI") ? dic["NSVI"] : string.Empty;
            if (VisibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            int bandCount = _curArguments.DataProvider == null ? -1 : _curArguments.DataProvider.BandCount;


            List<int> bandNos = new List<int>();
            StringBuilder express = new StringBuilder();
            //波段信息
            bandNos.Add(VisibleCH);
            bandNos.Add(NearInfraredCH);
            bandNos.Add(ShortInfraredCH);
            express.Append(string.Format("(band{0}/{1}f" + " >= " + minvisiable + ")||", VisibleCH, VisibleZoom));
            express.Append(string.Format("((band{0}/{1}f)/(band{2}/{3}f)" + "  <= " + maxnsvi + " && ", NearInfraredCH, NearInfraredZoom, ShortInfraredCH, ShortInfraredZoom));
            express.Append(string.Format("band{0}/{1}f" + " >= " + minnearinfrared + " && ", NearInfraredCH, NearInfraredZoom));
            express.Append(string.Format("(float)((band{2}/{1}f - band{0}/{3}f) / (band{2}/{1}f+band{0}/{3}f)) >= " + minndvi + ")", VisibleCH, VisibleZoom, NearInfraredCH, NearInfraredZoom));
            IThresholdExtracter<Int16> extracter = new SimpleThresholdExtracter<Int16>();
            extracter.Reset(_curArguments, bandNos.ToArray(), express.ToString());
            IRasterDataProvider prd = _curArguments.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("0CLM", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            if (IsPrintFeature)
                result.Tag = new CLMFeatureCollection("云辅助信息", GetBagDisplayInfo(VisibleCH, ShortInfraredCH, NearInfraredCH, prd));
            extracter.Extract(result);
            return result;
        }

        private IExtractResult CLMAlgorithm()
        {
            Dictionary<string, string> dic = Obj2Dic(_curArguments.GetArg("ArgumentSetting"));
            //可见光波段
            int VisibleCH = Obj2int(_curArguments.GetArg("Visible"));
            //远红外波段
            int FarInfraredCH = Obj2int(_curArguments.GetArg("FarInfrared"));
            //近红外波段
            int NearInfrared = Obj2int(_curArguments.GetArg("NearInfrared"));
            int NDSIVisibleCH = Obj2int(_curArguments.GetArg("NDSIVisible"));
            //短波红外波段
            int NDSIShortInfraredCH = Obj2int(_curArguments.GetArg("NDSIShortInfrared"));
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            if (bandNameRaster != null)
            {
                int newbandNo = -1;
                if (bandNameRaster.TryGetBandNoFromBandName(VisibleCH, out newbandNo))
                    VisibleCH = newbandNo;
                if (bandNameRaster.TryGetBandNoFromBandName(FarInfraredCH, out newbandNo))
                    FarInfraredCH = newbandNo;
                if (bandNameRaster.TryGetBandNoFromBandName(NDSIVisibleCH, out newbandNo))
                    NDSIVisibleCH = newbandNo;
                if (bandNameRaster.TryGetBandNoFromBandName(NDSIShortInfraredCH, out newbandNo))
                    NDSIShortInfraredCH = newbandNo;
            }
            double VisibleZoom = Obj2Double(_curArguments.GetArg("Visible_Zoom"));
            double FarInfraredZoom = Obj2Double(_curArguments.GetArg("FarInfrared_Zoom"));
            double NDSIVisibleZoom = Obj2Double(_curArguments.GetArg("NDSIVisible_Zoom"));
            double NDSIShortInfraredZoom = Obj2Double(_curArguments.GetArg("NDSIShortInfrared_Zoom"));
            //近红外放大倍数
            double NearInfraredZoom = Obj2Double(_curArguments.GetArg("NearInfrared_Zoom"));
            string useNDSIStr = dic.ContainsKey("UseNDSI") ? dic["UseNDSI"] : string.Empty;
            bool useNDSI = string.IsNullOrEmpty(useNDSIStr) ? false : bool.Parse(useNDSIStr);
            string PrintFeatrueStr = dic.ContainsKey("PrintFeatrue") ? dic["PrintFeatrue"] : string.Empty;
            bool PrintFeatrue = string.IsNullOrEmpty(PrintFeatrueStr) ? false : bool.Parse(PrintFeatrueStr);
            string ndsiMin = dic.ContainsKey("NDSIMin") ? dic["NDSIMin"] : string.Empty;
            string ndsiMax = dic.ContainsKey("NDSIMax") ? dic["NDSIMax"] : string.Empty;
            string useNearVisiableStr = dic.ContainsKey("UseNearVisiable") ? dic["UseNearVisiable"] : string.Empty;
            bool useNearVisiable = string.IsNullOrEmpty(useNearVisiableStr) ? false : bool.Parse(useNearVisiableStr);
            string nearVisiableMin = dic.ContainsKey("NearVisableMin") ? dic["NearVisableMin"] : string.Empty;
            string nearVisiableMax = dic.ContainsKey("NearVisableMax") ? dic["NearVisableMax"] : string.Empty;
            string visibleMin = dic.ContainsKey("VisibleMin") ? dic["VisibleMin"] : string.Empty;
            string useFarInfraredStr = dic.ContainsKey("UseFarInfrared") ? dic["UseFarInfrared"] : string.Empty;
            bool useFarInfrared = string.IsNullOrEmpty(useFarInfraredStr) ? false : bool.Parse(useFarInfraredStr);
            string farInfraredMax = dic.ContainsKey("FarInfraredMax") ? dic["FarInfraredMax"] : string.Empty;
            //云判识增加 近红外/短波红外>c 的判断 
            // c=nsvi
            string nsvi = dic.ContainsKey("NSVI") ? dic["NSVI"] : string.Empty;
            if (VisibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            int bandCount = _curArguments.DataProvider == null ? -1 : _curArguments.DataProvider.BandCount;
            if (useFarInfrared)
            {
                if (FarInfraredCH == -1 || (bandCount != -1 && FarInfraredCH > bandCount))
                {
                    PrintInfo("获取波段序号失败,预选的波段号大于当前影像的最大波段数。");
                    return null;
                }
            }
            if (useNDSI)
            {
                if (bandCount != -1 && (FarInfraredCH > bandCount || NDSIVisibleCH > bandCount || NDSIShortInfraredCH > bandCount))
                {
                    PrintInfo("获取波段序号失败,预选的波段号大于当前影像的最大波段数。");
                    return null;
                }
            }
            if (useNearVisiable)
            {
                if (VisibleCH == -1 || NDSIShortInfraredCH == -1 || (bandCount != -1 && (VisibleCH > bandCount || NDSIShortInfraredCH > bandCount)))
                {
                    PrintInfo("获取波段序号失败,预选的波段号大于当前影像的最大波段数。");
                    return null;
                }
            }
            if (bandCount != -1 && VisibleCH > bandCount)
                VisibleCH = NDSIVisibleCH;

            List<int> bandNos = new List<int>();
            StringBuilder express = new StringBuilder();
            bandNos.Add(VisibleCH);
            express.Append("band" + VisibleCH + "/" + VisibleZoom + @"f  >= " + visibleMin + " && ");
            if (useNDSI)
            {
                bandNos.Add(NDSIVisibleCH);
                bandNos.Add(NDSIShortInfraredCH);
                express.Append(string.Format(@"NDVI(band{0},band{1})  >= " + ndsiMin + @" &&
                                               NDVI(band{0},band{1})  <  " + ndsiMax + " && ",
                                                                           NDSIVisibleCH, NDSIShortInfraredCH));
            }
            if (useFarInfrared)
            {
                bandNos.Add(FarInfraredCH);
                express.Append("band" + FarInfraredCH + "/" + FarInfraredZoom + @"f <= " + farInfraredMax + " && ");
            }
            if (useNearVisiable)
            {
                bandNos.Add(NDSIShortInfraredCH);
                bandNos.Add(VisibleCH);
                express.Append(string.Format(@"((band{0}/" + NDSIShortInfraredZoom + "f)/(band{1}/" + VisibleZoom + "f))  >= " + nearVisiableMin + @" &&
                                               ((band{0}/" + NDSIShortInfraredZoom + "f)/(band{1}/" + VisibleZoom + "f))  <  " + nearVisiableMax + @" &&",
                               NDSIShortInfraredCH, VisibleCH));
            }
            //增加 近红外/短波红外 >c 判识条件
            if (!string.IsNullOrEmpty(nsvi))
            {
                bandNos.Add(NearInfrared);
                bandNos.Add(NDSIShortInfraredCH);
                express.Append(string.Format(@"((band{0}/" + NearInfraredZoom + "f)/(band{1}/" + NDSIShortInfraredZoom + "f))  <= " + nsvi + @" &&",
                              NearInfrared, NDSIShortInfraredCH));
            }

            IThresholdExtracter<Int16> extracter = new SimpleThresholdExtracter<Int16>();
            extracter.Reset(_curArguments, bandNos.ToArray(), express.ToString().Substring(0, express.Length - 3));
            IRasterDataProvider prd = _curArguments.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("0CLM", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            if (PrintFeatrue)
                result.Tag = new CLMFeatureCollection("云辅助信息", GetDisplayInfo(VisibleCH, NDSIShortInfraredCH, VisibleCH, FarInfraredCH,
                    useNDSI, useNearVisiable, useFarInfrared, prd));
            extracter.Extract(result);
            return result;
        }

        private Dictionary<int, CLMFeature> GetDisplayInfo(int NDSIVisibleCH, int ShortInfraredCH, int VisibleCH, int FarInfraredCH, bool useNDSI,
            bool useNearVisiable, bool useFarInfrared, IRasterDataProvider prd)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            Dictionary<int, CLMFeature> features = new Dictionary<int, CLMFeature>();
            CLMFeature tempCLM = null;
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(_argumentProvider);
            if (NDSIVisibleCH == -1)
                NDSIVisibleCH = 1;
            if (ShortInfraredCH == -1)
                ShortInfraredCH = 1;
            if (VisibleCH == -1)
                VisibleCH = 1;
            if (FarInfraredCH == -1)
                FarInfraredCH = 1;
            rpVisitor.VisitPixel(new Rectangle(0, 0, prd.Width, prd.Height), null, new int[] { NDSIVisibleCH, ShortInfraredCH, VisibleCH, FarInfraredCH },
                (index, values) =>
                {
                    tempCLM = new CLMFeature();
                    tempCLM.Ndsi = (Int16)((values[0] - values[1]) * 1000f / (values[0] + values[1]));
                    tempCLM.Visible = values[2];
                    tempCLM.FarInfrared = values[3];
                    tempCLM.nearInfrared = values[1];
                    tempCLM.UseFarInfrared = useFarInfrared;
                    tempCLM.UseNDSI = useNDSI;
                    tempCLM.UseNearVisiable = useNearVisiable;
                    features.Add(index, tempCLM);
                }
            );
            return features;
        }
        private Dictionary<int, CLMFeature> GetBagDisplayInfo(int visiable, int shortInfraredCH, int nearinfraredCH, IRasterDataProvider prd)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            Dictionary<int, CLMFeature> features = new Dictionary<int, CLMFeature>();
            CLMFeature tempCLM = null;
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(_argumentProvider);
            rpVisitor.VisitPixel(new Rectangle(0, 0, prd.Width, prd.Height), null, new int[] { visiable, shortInfraredCH, nearinfraredCH },
                (index, values) =>
                {
                    tempCLM = new CLMFeature();
                    tempCLM.Ndsi = (Int16)((values[1] - values[0]) * 1000f / (values[1] + values[0]));
                    tempCLM.Ndvi = (Int16)((values[2] - values[1]) * 1000f / (values[2] + values[1]));
                    tempCLM.Visible = values[0];
                    tempCLM.UseFarInfrared = false;
                    tempCLM.UseNDSI = false;
                    tempCLM.UseNearVisiable = false;
                    tempCLM.UseNDVI = true;
                    tempCLM.UseNearShort = true;
                    tempCLM.UseNearInfrared = true;
                    tempCLM.shortInfrared = values[1];
                    tempCLM.nearInfrared = values[2];
                    features.Add(index, tempCLM);
                }
            );
            return features;
        }

        private Dictionary<string, string> Obj2Dic(object dicObj)
        {
            if (dicObj == null || string.IsNullOrEmpty(dicObj.ToString()))
                return null;
            return dicObj as Dictionary<string, string>;
        }

        private double Obj2Double(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return -1;
            double value = -1;
            if (double.TryParse(obj.ToString(), out value))
                return value;
            return -1;
        }

        private int Obj2int(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return -1;
            int value = -1;
            if (int.TryParse(obj.ToString(), out value))
                return value;
            return -1;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            return null;
        }
    }
}
