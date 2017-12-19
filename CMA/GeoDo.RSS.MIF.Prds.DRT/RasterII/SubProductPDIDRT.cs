using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductPDIDRT : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private List<int> _indexiex = new List<int>();
        private float _tempzoom = 1000;
        private Action<int, string> _progressTracker = null;


        public SubProductPDIDRT(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            _progressTracker = progressTracker;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FYMersiAlgorithm")
            {
                Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("OrbitFileSelectType") as Dictionary<string, string[]>;
                if (pathDic == null || pathDic.Keys.Contains("CurrentRaster")) //选择当前影像进行计算
                    return ComputeByCurrentRaster(null, progressTracker);
                if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                    return ExtractPDI(pathDic["DirectoryPath"][0]);
                if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                    return ComputeByFiles(pathDic["FileNames"]);
                return null;
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult ComputeByCurrentRaster(IRasterDataProvider currPrd, Action<int, string> progressTracker)
        {
            currPrd = currPrd != null ? currPrd : _argumentProvider.DataProvider;
            if (currPrd == null)
                return null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int VisibleCH = TryGetBandNo(bandNameRaster, "RedBand");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NirBand");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarBand");
            double VisibleZoom = (double)_argumentProvider.GetArg("RedBand_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NirBand_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarBand_Zoom");
            bool isAutoCloud = (bool)_argumentProvider.GetArg("isAutoCloud");
            bool isAppCloud = (bool)_argumentProvider.GetArg("isAppCloud");

            if (VisibleCH == -1 || NearInfraredCH == -1 || (isAutoCloud && FarInfrared11CH == -1))
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            float pdiZoom = (float)_argumentProvider.GetArg("PDIZoom");
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            Int16 defWater = (Int16)_argumentProvider.GetArg("defWater");
            Int16 InvaildValue = (Int16)_argumentProvider.GetArg("InvaildValue");

            float NearInfraredCLMMin = float.Parse(_argumentProvider.GetArg("NearInfraredCLMMin").ToString());
            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            int cloudCH = (int)_argumentProvider.GetArg("CLMBand");
            string clmFile = GetClmFile(currPrd);

            float RegionExp1 = (float)_argumentProvider.GetArg("RegionExp1");
            float RegionExp2 = (float)_argumentProvider.GetArg("RegionExp2");

            DRTExpCoefficientCollection ExpCoefficient = _argumentProvider.GetArg("ExpCoefficient") as DRTExpCoefficientCollection;
            string EdgesFile = ExpCoefficient.EgdesFilename;
            int bandNo = TryGetBandNo(bandNameRaster, "DemBand");

            string[] aois = _argumentProvider.GetArg("AOITemplate") as string[];
            string aoiTemplate = (aois == null || aois.Length == 0) ? null : aois[0];

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider currRaster = null;
            IRasterDataProvider edgeRaster = null;
            IRasterDataProvider clmPrd = null;
            try
            {
                currRaster = currPrd;
                if (currRaster.BandCount < VisibleCH || currRaster.BandCount < NearInfraredCH || (isAutoCloud && currRaster.BandCount < FarInfrared11CH))
                {
                    PrintInfo("请选择正确的数据进行垂直干旱指数计算。");
                    return null;
                }
                RasterMaper rmCurr = new RasterMaper(currRaster, new int[] { VisibleCH, NearInfraredCH, FarInfrared11CH });
                rms.Add(rmCurr);

                bool isContainEdgesFile = false;
                if (!string.IsNullOrEmpty(EdgesFile) && File.Exists(EdgesFile))
                {
                    edgeRaster = RasterDataDriver.Open(EdgesFile) as IRasterDataProvider;
                    if (edgeRaster.BandCount < bandNo)
                    {
                        PrintInfo("请正确选择经验系数中的边界文件,波段数不足!");
                        return null;
                    }
                    RasterMaper rmEdge = new RasterMaper(edgeRaster, new int[] { bandNo });
                    rms.Add(rmEdge);
                    isContainEdgesFile = true;
                }

                bool isContainClm = false;
                if (isAppCloud && !string.IsNullOrEmpty(clmFile) && File.Exists(clmFile))
                {
                    clmPrd = GeoDataDriver.Open(clmFile) as IRasterDataProvider;
                    if (clmPrd.BandCount < cloudCH)
                    {
                        PrintInfo("请选择正确的云数据进行计算,波段数不足!");
                        return null;
                    }
                    RasterMaper clmRm = new RasterMaper(clmPrd, new int[] { cloudCH });
                    rms.Add(clmRm);
                    isContainClm = true;
                }
                float curNDVI = 0f;
                //输出文件准备（作为输入栅格并集处理）
                string outFileName = GetFileName(new string[] { currRaster.fileName }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.SetTemplateAOI(aoiTemplate);
                    DRTExpCoefficientItem item = null;
                    int demCode = -1;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        if (rvInVistor[0] == null || rvInVistor[0].RasterBandsData[0] == null ||
                           (isContainEdgesFile && (rvInVistor[1] == null || rvInVistor[1].RasterBandsData[0] == null)) ||
                           (isContainClm && (rvInVistor[2 - (isContainEdgesFile ? 0 : 1)] == null || rvInVistor[2 - (isContainEdgesFile ? 0 : 1)].RasterBandsData[0] == null)))
                        {
                            return;
                        }
                        int dataLength = aoi == null || aoi.Length == 0 ? rvOutVistor[0].SizeY * rvOutVistor[0].SizeX : aoi.Length;
                        for (int i = 0; i < dataLength; i++)
                        {
                            int index = aoi == null || aoi.Length == 0 ? i : aoi[i];
                            Int16 visiableValue = rvInVistor[0].RasterBandsData[0][index];
                            Int16 nearInfraredValue = rvInVistor[0].RasterBandsData[1][index];
                            Int16 farInfraredVale = rvInVistor[0].RasterBandsData[2][index];
                            if (isContainEdgesFile && rvInVistor[1].RasterBandsData[0] != null)
                                demCode = (int)rvInVistor[1].RasterBandsData[0][index];
                            else
                                demCode = -1;
                            curNDVI = GetNDVI(nearInfraredValue, visiableValue);
                            if (visiableValue == 0 && nearInfraredValue == 0)
                                rvOutVistor[0].RasterBandsData[0][index] = InvaildValue;
                            else if (isContainClm && rvInVistor[2 - (isContainEdgesFile ? 0 : 1)].RasterBandsData[0][index] != 0)
                                rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                            else if (isAutoCloud && (nearInfraredValue / NearInfraredZoom > NearInfraredCLMMin && farInfraredVale / FarInfrared11Zoom < FarInfrared11CLMMax))
                                rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                            else
                            {
                                if (farInfraredVale / FarInfrared11Zoom > FarInfrared11WaterMin && curNDVI < NDVIWaterMax)
                                    rvOutVistor[0].RasterBandsData[0][index] = defWater;
                                else
                                {
                                    item = ExpCoefficient.GetExpItemByNum(demCode);
                                    if (item == null)
                                        rvOutVistor[0].RasterBandsData[0][index] = (Int16)((visiableValue / VisibleZoom + RegionExp1 * nearInfraredValue / NearInfraredZoom) / Math.Sqrt(RegionExp2 * RegionExp2 + 1) * pdiZoom);
                                    else
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = (Int16)((visiableValue / VisibleZoom + item.APara * nearInfraredValue / NearInfraredZoom) / Math.Sqrt(item.BPara * item.BPara + 1) * pdiZoom);
                                    }
                                }
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    IFileExtractResult pdiFile = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    pdiFile.SetDispaly(false);
                    return pdiFile;
                }
            }
            finally
            {
                if (clmPrd != null)
                    clmPrd.Dispose();
                if (edgeRaster != null)
                    edgeRaster.Dispose();
            }
        }

        private IExtractResult ExtractPDI(string dirPath)
        {
            string[] files = GetFiles(dirPath);
            return ComputeByFiles(files);
        }

        private IExtractResult ComputeByFiles(string[] files)
        {
            IExtractResultArray result = new ExtractResultArray("DRT");
            IRasterDataProvider currPrd = null;
            for (int i = 0; i < files.Length; i++)
            {
                currPrd = GeoDataDriver.Open(files[i], null) as IRasterDataProvider;
                if (currPrd != null)
                    try
                    {
                        IExtractResult temp = ComputeByCurrentRaster(currPrd, _progressTracker);
                        if (temp == null || (temp as FileExtractResult) == null)
                            continue;
                        result.Add(temp as FileExtractResult);
                    }
                    finally
                    {
                        currPrd.Dispose();
                    }
            }
            return result;
        }

        private string[] GetFiles(string dirPath)
        {
            ObritFileFinder off = new ObritFileFinder();
            string extInfo = null;
            string[] files = null;
            if (string.IsNullOrEmpty(dirPath))
                return null;
            string flag = _argumentProvider.GetArg("orbitDataIdentify").ToString();
            files = off.Find(string.Empty, ref extInfo, "Dir=" + dirPath + ",Flag=" + flag + ",Sort=asc,ProFlag=true,FindRegion=all");
            if (files == null || files.Length == 0)
            {
                PrintInfo("计算垂直干旱指数的主文件未提供!");
                return null;
            }
            return files;
        }

        private float GetNDVI(Int16 nearInfrared, Int16 visible)
        {
            return (nearInfrared + visible) == 0 ? 0f : (float)(nearInfrared - visible) / (nearInfrared + visible);
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private string GetClmFile(IRasterDataProvider currPrd)
        {
            RasterIdentify rid = new RasterIdentify(Path.GetFileName(currPrd.fileName));
            rid.ProductIdentify = "DRT";
            rid.SubProductIdentify = "0CLM";
            string clmFile = rid.ToWksFullFileName(".dat");
            if (File.Exists(clmFile))
                return clmFile;
            return null;
        }

    }
}