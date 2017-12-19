using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtDvi : CmaVgtMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductRasterVgtDvi()
            : base()
        {
        }

        public SubProductRasterVgtDvi(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "0DVI1" && algorith != "0DVI2")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            return CalcDVI(algorith, progressTracker);
        }

        private IExtractResult CalcDVI(string algorithmName, Action<int, string> progressTracker)
        {
            if (string.IsNullOrEmpty(algorithmName))
                return null;
            if (algorithmName == "0DVI1")
                return DVIAlgorithm1(progressTracker);
            else if (algorithmName == "0DVI2")
                return DVIAlgorithm2(progressTracker);
            return null;

        }

        private IExtractResult DVIAlgorithm2(Action<int, string> progressTracker)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int bandV = TryGetBandNo(bandNameRaster, "Visible");        //可见光
            int bandNear = TryGetBandNo(bandNameRaster, "NearInfrared");//近红外
            if (bandV == -1 || bandNear == -1)
            {
                PrintInfo("通道序号设置不正确");
                return null;
            }
            int[] bandNos = new int[] { bandV, bandNear };
            double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double nearInfrared = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double[] bandzoom = new double[] { visibleZoom, nearInfrared };
            Dictionary<string, string> dic = Obj2Dic(_argumentProvider.GetArg("ArgumentSetting"));
            string nearVisiableMinStr = dic.ContainsKey("NearVisableMin") ? dic["NearVisableMin"] : string.Empty;
            string nearVisiableMaxStr = dic.ContainsKey("NearVisableMax") ? dic["NearVisableMax"] : string.Empty;
            string visibleMinStr = dic.ContainsKey("VisibleMin") ? dic["VisibleMin"] : string.Empty;
            float nearVisiableMin = float.Parse(nearVisiableMinStr);
            float nearVisiableMax = float.Parse(nearVisiableMaxStr);
            float visibleMin = float.Parse(visibleMinStr);
            float[] cloudyArgs = new float[] { visibleMin, nearVisiableMax, nearVisiableMin };
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            float zoom = (ushort)_argumentProvider.GetArg("resultZoom");
            zoom = zoom == 0f ? 1000 : zoom;
            string[] aois = _argumentProvider.GetArg("AOITemplate") as string[];
            string aoiTemplate = (aois == null || aois.Length == 0) ? null : aois[0];
            string[] inputFiles = GetStringArray("RasterFile");
            if (inputFiles == null || inputFiles.Length == 0)
            {
                PrintInfo("没有设置输入数据");
                return null;
            }
            bool isFitterCloud;
            if (!bool.TryParse(_argumentProvider.GetArg("isFilterCloudy").ToString(), out isFitterCloud))
            {
                PrintInfo("是否过滤云参数设置不正确！");
                return null;
            }
            IExtractResultArray results = new ExtractResultArray("NDVI");
            foreach (string inputFile in inputFiles)
            {
                //计算NDVI
                IExtractResult ret = CalcDVIAlgorithm2(inputFile, bandNos, zoom, bandzoom, cloudyArgs, defCloudy, isFitterCloud, aoiTemplate, progressTracker);
                if (ret != null)
                    results.Add(ret as IExtractResultBase);
            }
            return results;
        }

        private IExtractResult CalcDVIAlgorithm2(string inputFile, int[] bandNos, float zoom, double[] bandzoom, float[] cloudyArgs, short defCloudy, bool isFitterCloud, string aoiTemplate, Action<int, string> progressTracker)
        {
            RasterProcessModel<short, short> rfr = null;
            List<RasterMaper> fileIns = new List<RasterMaper>();
            RasterMaper[] fileOuts = null;
            bool isAppCloud;
            if (!bool.TryParse(_argumentProvider.GetArg("isAppCloud").ToString(), out isAppCloud))
            {
                PrintInfo("合并交互云参数设置不正确！");
                return null;
            }
            IRasterDataProvider clmPrd = null;
            try
            {
                //输入数据(LDF)
                IRasterDataProvider inRaster = RasterDataDriver.Open(inputFile) as IRasterDataProvider;
                if (inRaster == null)
                {
                    PrintInfo("读取栅格文件失败：" + inRaster);
                    return null;
                }
                //输出数据(DVI)
                string outFileName = GetFileName(new string[] { inRaster.fileName }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                IRasterDataDriver dd = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                string mapInfo = inRaster.CoordEnvelope.ToMapInfoString(new Size(inRaster.Width, inRaster.Height));
                string[] opts = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=MEM",
                "WITHHDR=TRUE",
                "SPATIALREF=" + inRaster.SpatialRef.ToProj4String(),
                mapInfo};

                RasterDataProvider outRaster = dd.Create(outFileName, inRaster.Width, inRaster.Height, 1, enumDataType.Int16, opts) as RasterDataProvider;
                string clmFile = GetClmFile(inputFile);
                int cloudCH = GetCloudCHNO();
                //栅格数据映射
                fileIns.Add(new RasterMaper(inRaster, bandNos));
                if (isAppCloud)
                {
                    if (!string.IsNullOrEmpty(clmFile) && File.Exists(clmFile))
                    {
                        clmPrd = GeoDataDriver.Open(clmFile) as IRasterDataProvider;
                        if (clmPrd.BandCount < cloudCH)
                        {
                            PrintInfo("请选择正确的云数据通道进行计算.");
                            isAppCloud = false;
                        }
                        else
                            fileIns.Add(new RasterMaper(clmPrd, new int[] { cloudCH }));
                    }
                    else
                        isAppCloud = false;
                }

                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                fileOuts = new RasterMaper[] { fileOut };
                //创建处理模型
                rfr = new RasterProcessModel<short, short>(progressTracker);
                rfr.SetRaster(fileIns.ToArray(), fileOuts);
                rfr.SetTemplateAOI(aoiTemplate);
                rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData != null)
                    {
                        if (rvInVistor == null)
                            return;
                        short[] inBand0 = rvInVistor[0].RasterBandsData[0];//第1个输入文件的第1个波段的各像素值
                        short[] inBand1 = rvInVistor[0].RasterBandsData[1];//第1个输入文件的第2个波段的各像素值
                        short[] inBand2 = rvInVistor[1].RasterBandsData[0];//第2个输入文件的第1个波段的各像素值
                        if (string.IsNullOrWhiteSpace(aoiTemplate))
                        {
                            for (int index = 0; index < inBand0.Length; index++)
                            {
                                if ((isAppCloud && inBand2[index] != 0) || (isFitterCloud && inBand0[index] / bandzoom[0] >= cloudyArgs[0] && inBand1[index] / inBand0[index] < cloudyArgs[1] && inBand1[index] / inBand0[index] >= cloudyArgs[2]))
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                                    continue;
                                }
                                //第1个输出文件的第1个波段存储NDVI值
                                rvOutVistor[0].RasterBandsData[0][index] = (short)((inBand1[index] / bandzoom[1] - inBand0[index] / bandzoom[0]) * zoom / 100);
                            }
                        }
                        else if (aoi != null && aoi.Length != 0)
                        {
                            int index;
                            for (int i = 0; i < aoi.Length; i++)
                            {
                                index = aoi[i];
                                if ((isAppCloud && inBand2[index] != 0) || (isFitterCloud && inBand0[index] / bandzoom[0] >= cloudyArgs[0] && inBand1[index] / inBand0[index] < cloudyArgs[1] && inBand1[index] / inBand0[index] >= cloudyArgs[2]))
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                                    continue;
                                }
                                //第1个输出文件的第1个波段存储NDVI值
                                rvOutVistor[0].RasterBandsData[0][index] = (short)((inBand1[index] / bandzoom[1] - inBand0[index] / bandzoom[0]) * zoom / 100);
                            }
                        }
                    }
                }));
                //执行
                rfr.Excute();
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (fileIns != null)
                    for (int i = 0; i < fileIns.Count; i++)
                        fileIns[i].Raster.Dispose();
                if (fileOuts != null)
                    for (int i = 0; i < fileOuts.Length; i++)
                        fileOuts[i].Raster.Dispose();
                if (clmPrd != null)
                    clmPrd.Dispose();
            }
        }

        private IExtractResult DVIAlgorithm1(Action<int, string> progressTracker)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int bandV = TryGetBandNo(bandNameRaster, "Visible");        //可见光
            int bandNear = TryGetBandNo(bandNameRaster, "NearInfrared");//近红外
            int bandMid = TryGetBandNo(bandNameRaster, "MiddInfrared"); //中红外
            int bandFar = TryGetBandNo(bandNameRaster, "FarInfrared11");//远红外
            if (bandV == -1 || bandNear == -1 || bandMid < 1 || bandFar < 1)
            {
                PrintInfo("通道序号设置不正确");
                return null;
            }
            int[] bandNos = new int[] { bandV, bandNear, bandMid, bandFar };
            double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double nearInfrared = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double middZoom = (double)_argumentProvider.GetArg("MiddInfrared_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double[] bandzoom = new double[] { visibleZoom, nearInfrared, middZoom, farZoom };
            float NearInfraredCLMMin = float.Parse(_argumentProvider.GetArg("NearInfraredCLMMin").ToString());
            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared1112CLMMin = float.Parse(_argumentProvider.GetArg("FarInfrared1112CLMMin").ToString());
            float[] cloudyArgs = new float[] { NearInfraredCLMMin, FarInfrared11CLMMax, FarInfrared1112CLMMin };
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            float zoom = (ushort)_argumentProvider.GetArg("resultZoom");
            zoom = zoom == 0f ? 1000 : zoom;
            string[] aois = _argumentProvider.GetArg("AOITemplate") as string[];
            string aoiTemplate = (aois == null || aois.Length == 0) ? null : aois[0];
            string[] inputFiles = GetStringArray("RasterFile");
            if (inputFiles == null || inputFiles.Length == 0)
            {
                PrintInfo("没有设置输入数据");
                return null;
            }
            bool isFitterCloud;
            if (!bool.TryParse(_argumentProvider.GetArg("isFilterCloudy").ToString(), out isFitterCloud))
            {
                PrintInfo("是否过滤云参数设置不正确！");
                return null;
            }
            IExtractResultArray results = new ExtractResultArray("NDVI");
            foreach (string inputFile in inputFiles)
            {
                //计算NDVI
                IExtractResult ret = CalcDVIAlgorithm1(inputFile, bandNos, zoom, bandzoom, cloudyArgs, defCloudy, isFitterCloud, aoiTemplate, progressTracker);
                if (ret != null)
                    results.Add(ret as IExtractResultBase);
            }
            return results;
        }

        /// <summary>
        /// 单文件计算其DVI
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="bandNos"></param>
        /// <param name="zoom"></param>
        /// <param name="aoiTemplate"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        private IExtractResult CalcDVIAlgorithm1(string inputFile, int[] bandNos, float zoom, double[] bandZoom, float[] cloudyArgs, Int16 defCloudy, bool isFitterCloud, string aoiTemplate, Action<int, string> progressTracker)
        {
            RasterProcessModel<short, short> rfr = null;
            List<RasterMaper> fileIns = new List<RasterMaper>();
            RasterMaper[] fileOuts = null;
            bool isAppCloud;
            if (!bool.TryParse(_argumentProvider.GetArg("isAppCloud").ToString(), out isAppCloud))
            {
                PrintInfo("合并交互云参数设置不正确！");
                return null;
            }
            IRasterDataProvider clmPrd = null;
            try
            {
                //输入数据(LDF)
                IRasterDataProvider inRaster = RasterDataDriver.Open(inputFile) as IRasterDataProvider;
                if (inRaster == null)
                {
                    PrintInfo("读取栅格文件失败：" + inRaster);
                    return null;
                }
                //输出数据(DVI)
                string outFileName = GetFileName(new string[] { inRaster.fileName }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                IRasterDataDriver dd = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                string mapInfo = inRaster.CoordEnvelope.ToMapInfoString(new Size(inRaster.Width, inRaster.Height));
                string[] opts = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=MEM",
                "WITHHDR=TRUE",
                "SPATIALREF=" + inRaster.SpatialRef.ToProj4String(),
                mapInfo};
                RasterDataProvider outRaster = dd.Create(outFileName, inRaster.Width, inRaster.Height, 1, enumDataType.Int16, opts) as RasterDataProvider;
                string clmFile = GetClmFile(inputFile);
                int cloudCH = GetCloudCHNO();
                //栅格数据映射
                fileIns.Add(new RasterMaper(inRaster, bandNos));
                if (isAppCloud)
                {
                    if (!string.IsNullOrEmpty(clmFile) && File.Exists(clmFile))
                    {
                        clmPrd = GeoDataDriver.Open(clmFile) as IRasterDataProvider;
                        if (clmPrd.BandCount < cloudCH)
                        {
                            PrintInfo("请选择正确的云数据通道进行计算.");
                            isAppCloud = false;
                        }
                        else
                            fileIns.Add(new RasterMaper(clmPrd, new int[] { cloudCH }));
                    }
                    else
                        isAppCloud = false;
                }
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                fileOuts = new RasterMaper[] { fileOut };
                //创建处理模型
                rfr = new RasterProcessModel<short, short>(progressTracker);
                rfr.SetRaster(fileIns.ToArray(), fileOuts);
                rfr.SetTemplateAOI(aoiTemplate);
                rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData != null)
                    {
                        if (rvInVistor == null)
                            return;
                        short[] inBand0 = rvInVistor[0].RasterBandsData[0];//第1个输入文件的第1个波段的各像素值
                        short[] inBand1 = rvInVistor[0].RasterBandsData[1];//第1个输入文件的第2个波段的各像素值
                        short[] inBand2 = rvInVistor[0].RasterBandsData[2];//第1个输入文件的第3个波段的各像素值
                        short[] inBand3 = rvInVistor[0].RasterBandsData[3];//第1个输入文件的第4个波段的各像素值
                        short[] inBand12 = isAppCloud ? rvInVistor[1].RasterBandsData[0] : null;//第2个输入文件的第1个波段的各像素值
                        short[] ndvi = new short[inBand0.Length];
                        if (string.IsNullOrWhiteSpace(aoiTemplate))
                        {
                            for (int index = 0; index < inBand0.Length; index++)
                            {
                                if ((isAppCloud && inBand12[index] != 0) || (isFitterCloud && inBand1[index] / bandZoom[1] > cloudyArgs[0] && inBand3[index] / bandZoom[3] < cloudyArgs[1] && Math.Abs(inBand2[index] / bandZoom[2] - inBand3[index] / bandZoom[3]) > cloudyArgs[2]))
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                                    continue;
                                }
                                //第1个输出文件的第1个波段存储NDVI值
                                rvOutVistor[0].RasterBandsData[0][index] = (short)((inBand1[index] / bandZoom[1] - inBand0[index] / bandZoom[0]) * zoom / 100);
                            }
                        }
                        else if (aoi != null && aoi.Length != 0)
                        {
                            int index;
                            for (int i = 0; i < aoi.Length; i++)
                            {
                                index = aoi[i];
                                if ((isAppCloud && inBand12[index] != 0) || (isFitterCloud && inBand1[index] / bandZoom[1] > cloudyArgs[0] && inBand3[index] / bandZoom[3] < cloudyArgs[1] && Math.Abs(inBand2[index] / bandZoom[2] - inBand3[index] / bandZoom[3]) > cloudyArgs[2]))
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                                    continue;
                                }
                                //第1个输出文件的第1个波段存储NDVI值
                                rvOutVistor[0].RasterBandsData[0][index] = (short)((inBand1[index] / bandZoom[1] - inBand0[index] / bandZoom[0]) * zoom / 100);
                            }
                        }
                    }
                }));
                //执行
                rfr.Excute();
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (fileIns != null)
                    for (int i = 0; i < fileIns.Count; i++)
                        fileIns[i].Raster.Dispose();
                if (fileOuts != null)
                    for (int i = 0; i < fileOuts.Length; i++)
                        fileOuts[i].Raster.Dispose();
                if (clmPrd != null)
                    clmPrd.Dispose();
            }
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        #region 增加交互云修正

        private string GetClmFile(string fname)
        {
            RasterIdentify rid = new RasterIdentify(Path.GetFileName(fname));
            rid.ProductIdentify = "VGT";
            rid.SubProductIdentify = "0CLM";
            string clmFile = rid.ToWksFullFileName(".dat");
            if (File.Exists(clmFile))
                return clmFile;
            return null;
        }

        private int GetCloudCHNO()
        {
            int cloudCH;
            if (_argumentProvider.GetArg("cloudCH") == null)
                return 1;
            if (string.IsNullOrEmpty(_argumentProvider.GetArg("cloudCH").ToString()))
                return 1;
            if (int.TryParse(_argumentProvider.GetArg("cloudCH").ToString(), out cloudCH))
                return cloudCH;
            else
                return 1;

        }
        #endregion

    }
}
