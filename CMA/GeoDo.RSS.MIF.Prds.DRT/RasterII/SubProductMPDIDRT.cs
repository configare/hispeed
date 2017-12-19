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
    public class SubProductMPDIDRT : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private List<int> _indexiex = new List<int>();
        private Action<int, string> _progressTracker = null;
        private UInt16[][] output = null;
        private UInt16 normalizationFillVlue = NormalizationRedNir.laiFillVlue;
        private int _prjbandfillvalue = 0;
        private float[] _zooms = null;


        public SubProductMPDIDRT(SubProductDef subProductDef)
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
            float curNDVI = 0f;
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

            string normalizationFile = string.Empty;
            AngleParModel angleArg = _argumentProvider.GetArg("Angle") as AngleParModel;
            if (angleArg != null && angleArg.ApplyN)
            {
                if (string.IsNullOrEmpty(angleArg.FileAsatA) || !File.Exists(angleArg.FileAsatA) ||
                    string.IsNullOrEmpty(angleArg.FileAsatZ) || !File.Exists(angleArg.FileAsatZ) ||
                    string.IsNullOrEmpty(angleArg.FileAsunA) || !File.Exists(angleArg.FileAsunA) ||
                    string.IsNullOrEmpty(angleArg.FileAsunZ) || !File.Exists(angleArg.FileAsunZ) ||
                    string.IsNullOrEmpty(angleArg.FileLandConvery) || !File.Exists(angleArg.FileLandConvery))
                {
                    PrintInfo("归一化处理所需的角度信息、土地覆盖信息填写不正确或不存在,请检查...");
                    return null;
                }
                normalizationFile = NormalizationProcess(currPrd, progressTracker);
                if (string.IsNullOrEmpty(normalizationFile) || !File.Exists(normalizationFile))
                {
                    PrintInfo("归一化处理失败");
                    return null;
                }
            }

            bool normalizationSuccess = angleArg != null && angleArg.ApplyN &&
                                        !string.IsNullOrEmpty(normalizationFile) && File.Exists(normalizationFile) ? true : false;

            float NearInfraredCLMMin = float.Parse(_argumentProvider.GetArg("NearInfraredCLMMin").ToString());
            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            Int16 defWater = (Int16)_argumentProvider.GetArg("defWater");
            Int16 InvaildValue = (Int16)_argumentProvider.GetArg("InvaildValue");

            int cloudCH = (int)_argumentProvider.GetArg("CLMBand");
            string clmFile = GetClmFile(currPrd);

            bool isSetMinMax = (bool)_argumentProvider.GetArg("isSetMinMax");
            float ndviMax = (float)_argumentProvider.GetArg("NDVIMax");
            float ndviMin = (float)_argumentProvider.GetArg("NDVIMin");

            float ndviCalcMax = float.MinValue;
            float ndviCalcMin = float.MaxValue;

            float PNVIZoom = (float)_argumentProvider.GetArg("PNVIZoom");

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider currRaster = null;
            IRasterDataProvider clmPrd = null;
            IRasterDataProvider ndviPrd = null;
            IFileExtractResult pviFile = null;
            IExtractResultArray array = new ExtractResultArray("DRT");
            if (progressTracker != null)
                progressTracker.Invoke(normalizationSuccess ? 77 : 0, "计算改进型垂直干旱指数");

            try
            {
                currRaster = normalizationSuccess ? GeoDataDriver.Open(normalizationFile) as IRasterDataProvider : currPrd;
                if (!normalizationSuccess && (currRaster.BandCount < VisibleCH || currRaster.BandCount < NearInfraredCH || (isAutoCloud && currRaster.BandCount < FarInfrared11CH)))
                {
                    PrintInfo("请选择正确的数据进行垂直干旱指数计算。");
                    return null;
                }
                RasterMaper rmCurr = normalizationSuccess ? new RasterMaper(currRaster, new int[] { 1, 2, 3 }) :
                                                            new RasterMaper(currRaster, new int[] { VisibleCH, NearInfraredCH, FarInfrared11CH });
                rms.Add(rmCurr);

                bool isContainClm = false;
                if (isAppCloud && !string.IsNullOrEmpty(clmFile) && File.Exists(clmFile))
                {
                    clmPrd = GeoDataDriver.Open(clmFile) as IRasterDataProvider;
                    if (clmPrd.BandCount < cloudCH)
                    {
                        PrintInfo("请选择正确的云数据通道进行计算。");
                        return null;
                    }
                    RasterMaper clmRm = new RasterMaper(clmPrd, new int[] { cloudCH });
                    rms.Add(clmRm);
                    isContainClm = true;
                }

                bool isContainNDVI = false;
                string ndviFile = string.Empty;
                string[] ndviFiles = GetStringArray("SelectedPrimaryFiles");
                if (ndviFiles == null || ndviFiles.Length == 0 || string.IsNullOrEmpty(ndviFiles[0]) || !File.Exists(ndviFiles[0]))
                    isContainNDVI = false;
                else
                {
                    isContainNDVI = true;
                    ndviFile = ndviFiles[0];
                }

                int ndviCH = (int)_argumentProvider.GetArg("NDVIBand");
                double ndviZoom = (double)_argumentProvider.GetArg("NDVIBand_Zoom");
                if (isContainNDVI && !string.IsNullOrEmpty(ndviFile))
                {
                    ndviPrd = GeoDataDriver.Open(ndviFile) as IRasterDataProvider;
                    if (ndviPrd.BandCount < ndviCH)
                    {
                        PrintInfo("请选择正确的植被指数数据通道进行计算。");
                        return null;
                    }
                    RasterMaper ndviRm = new RasterMaper(ndviPrd, new int[] { ndviCH });
                    rms.Add(ndviRm);
                    isContainNDVI = true;
                }

                //输出文件准备（作为输入栅格并集处理）
                string outFileName = GetFileName(new string[] { currRaster.fileName }, _subProductDef.ProductDef.Identify, "PNVI", ".dat", null);
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        for (int index = 0; index < dataLength; index++)
                        {
                            Int16 visiableValue = rvInVistor[0].RasterBandsData[0][index];
                            Int16 nearInfraredValue = rvInVistor[0].RasterBandsData[1][index];
                            Int16 farInfraredVale = rvInVistor[0].RasterBandsData[2][index];
                            curNDVI = isContainNDVI ? (float)(rvInVistor[2 - (isContainClm ? 0 : 1)].RasterBandsData[0][index] / ndviZoom) : GetNDVI(nearInfraredValue, visiableValue);
                            if (visiableValue == 0 && nearInfraredValue == 0)
                                rvOutVistor[0].RasterBandsData[0][index] = InvaildValue;
                            else if (isContainClm && rvInVistor[1].RasterBandsData[0][index] != 0)
                                rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                            else if (isAutoCloud && (nearInfraredValue / NearInfraredZoom > NearInfraredCLMMin && farInfraredVale / FarInfrared11Zoom < FarInfrared11CLMMax))
                                rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                            else
                            {
                                if (farInfraredVale / FarInfrared11Zoom > FarInfrared11WaterMin && curNDVI < NDVIWaterMax)
                                    rvOutVistor[0].RasterBandsData[0][index] = defWater;
                                else
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)(curNDVI * PNVIZoom);
                                    if (curNDVI > ndviCalcMax)
                                        ndviCalcMax = curNDVI;
                                    else if (curNDVI < ndviCalcMin)
                                        ndviCalcMin = curNDVI;
                                }
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    pviFile = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    pviFile.SetDispaly(false);

                    if (normalizationSuccess)
                    {
                        FileExtractResult nrsb = new FileExtractResult(_subProductDef.Identify, normalizationFile, true);
                        nrsb.SetDispaly(false);
                        array.Add(nrsb);
                    }

                    _argumentProvider.SetArg("CursorInfo:Image-NDVIMax", ndviCalcMax);
                    _argumentProvider.SetArg("CursorInfo:Image-NDVIMin", ndviCalcMin);
                    array.Add(pviFile);
                }
            }
            finally
            {
                if (clmPrd != null)
                    clmPrd.Dispose();
                if (ndviPrd != null)
                    ndviPrd.Dispose();
            }
            FileExtractResult mpdiFile = null;
            if (File.Exists(pviFile.FileName))
            {
                mpdiFile = CalcMPDI(pviFile.FileName, ndviCalcMin, ndviCalcMax, currRaster, normalizationSuccess, progressTracker);
                if (mpdiFile != null)
                    array.Add(mpdiFile);
            }

            return array;
        }

        private FileExtractResult CalcMPDI(string pviFile, float ndviCalcMin, float ndviCalcMax, IRasterDataProvider currPrd, bool normalizationSuccess, Action<int, string> progressTracker)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int VisibleCH = TryGetBandNo(bandNameRaster, "RedBand");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NirBand");
            double VisibleZoom = (double)_argumentProvider.GetArg("RedBand_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NirBand_Zoom");

            if (VisibleCH == -1 || NearInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            float mpdiZoom = (float)_argumentProvider.GetArg("MPDIZoom");
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            Int16 defWater = (Int16)_argumentProvider.GetArg("defWater");
            Int16 InvaildValue = (Int16)_argumentProvider.GetArg("InvaildValue");

            DRTExpCoefficientCollection ExpCoefficient = _argumentProvider.GetArg("ExpCoefficient") as DRTExpCoefficientCollection;
            string EdgesFile = ExpCoefficient.EgdesFilename;

            DRTExpCoefficientCollection LandExpCoefficient = _argumentProvider.GetArg("LandExpCoefficient") as DRTExpCoefficientCollection;
            string LandFile = LandExpCoefficient.EgdesFilename;

            int bandNo = TryGetBandNo(bandNameRaster, "DemBand");
            int LandbandNo = TryGetBandNo(bandNameRaster, "LandBand");

            float ndviExp = (float)_argumentProvider.GetArg("NDVIExp");
            bool isSetMinMax = (bool)_argumentProvider.GetArg("isSetMinMax");
            float ndviMax = (float)_argumentProvider.GetArg("NDVIMax");
            float ndviMin = (float)_argumentProvider.GetArg("NDVIMin");

            float RegionExp1 = (float)_argumentProvider.GetArg("RegionExp1");
            float RegionExp2 = (float)_argumentProvider.GetArg("RegionExp2");

            float LandExp1 = (float)_argumentProvider.GetArg("LandExp1");
            float LandExp2 = (float)_argumentProvider.GetArg("LandExp2");

            string[] aois = _argumentProvider.GetArg("AOITemplate") as string[];
            string aoiTemplate = (aois == null || aois.Length == 0) ? null : aois[0];

            if (isSetMinMax)
            {
                ndviCalcMin = ndviMin;
                ndviCalcMax = ndviMax;
            }

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider currRaster = null;
            IRasterDataProvider pviPrd = null;
            IRasterDataProvider edgeRaster = null;
            IRasterDataProvider edgeLand = null;
            FileExtractResult mpdiRes = null;
            float PNVIZoom = (float)_argumentProvider.GetArg("PNVIZoom");
            try
            {
                currRaster = currPrd;
                if (!normalizationSuccess && (currRaster.BandCount < VisibleCH || currRaster.BandCount < NearInfraredCH))
                {
                    PrintInfo("请选择正确的数据进行垂直干旱指数计算。");
                    return null;
                }
                RasterMaper rmCurr = normalizationSuccess ? new RasterMaper(currRaster, new int[] { 1, 2 }) :
                                                            new RasterMaper(currRaster, new int[] { VisibleCH, NearInfraredCH });
                rms.Add(rmCurr);

                pviPrd = GeoDataDriver.Open(pviFile) as IRasterDataProvider;
                RasterMaper clmRm = new RasterMaper(pviPrd, new int[] { 1 });
                rms.Add(clmRm);

                bool isContainEdges = false;
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
                    isContainEdges = true;
                }

                bool isContainEdgesLand = false;
                if (!string.IsNullOrEmpty(LandFile) && File.Exists(LandFile))
                {
                    edgeLand = RasterDataDriver.Open(LandFile) as IRasterDataProvider;
                    if (edgeLand.BandCount < LandbandNo)
                    {
                        PrintInfo("请正确选择土地参数中的边界文件,波段数不足!");
                        return null;
                    }
                    RasterMaper rmLand = new RasterMaper(edgeLand, new int[] { LandbandNo });
                    rms.Add(rmLand);
                    isContainEdgesLand = true;
                }

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
                    DRTExpCoefficientItem landitem = null;
                    int demCode = -1;
                    int landCode = -1;
                    Int16 curNDVI = 0;
                    float tempNDVIExp = 0f;
                    float argsM = 0f;
                    float argsM2 = 0f;
                    float argsLand = 0f;
                    float argsLand2 = 0f;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        if (rvInVistor[0] == null || rvInVistor[0].RasterBandsData[0] == null ||
                            rvInVistor[1] == null || rvInVistor[1].RasterBandsData[0] == null ||
                           (isContainEdges && (rvInVistor[2] == null || rvInVistor[2].RasterBandsData[0] == null)) ||
                           (isContainEdgesLand && (rvInVistor[3 - (isContainEdges ? 0 : 1)] == null || rvInVistor[3 - (isContainEdges ? 0 : 1)].RasterBandsData[0] == null)))
                        {
                            return;
                        }
                        int dataLength = aoi == null || aoi.Length == 0 ? rvOutVistor[0].SizeY * rvOutVistor[0].SizeX : aoi.Length;
                        for (int i = 0; i < dataLength; i++)
                        {
                            int index = aoi == null || aoi.Length == 0 ? i : aoi[i];
                            Int16 visiableValue = rvInVistor[0].RasterBandsData[0][index];
                            Int16 nearInfraredValue = rvInVistor[0].RasterBandsData[1][index];
                            if (isContainEdges && rvInVistor[2].RasterBandsData[0] != null)
                                demCode = (int)rvInVistor[2].RasterBandsData[0][index];
                            else
                                demCode = -1;
                            if (isContainEdgesLand && rvInVistor[3 - (isContainEdges ? 0 : 1)].RasterBandsData[0] != null)
                                landCode = (int)rvInVistor[3 - (isContainEdges ? 0 : 1)].RasterBandsData[0][index];
                            else
                                landCode = -1;
                            curNDVI = rvInVistor[1].RasterBandsData[0][index];
                            if (visiableValue == 0 && nearInfraredValue == 0)
                                rvOutVistor[0].RasterBandsData[0][index] = InvaildValue;
                            else if (curNDVI == defCloudy)
                                rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                            else if (curNDVI == defWater)
                                rvOutVistor[0].RasterBandsData[0][index] = defWater;
                            else
                            {
                                if (ndviCalcMax - ndviCalcMin == 0)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                tempNDVIExp = (float)(1 - Math.Pow(((ndviCalcMax - curNDVI / PNVIZoom) / (ndviCalcMax - ndviCalcMin)), ndviExp));
                                if (1 - tempNDVIExp == 0)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                item = ExpCoefficient.GetExpItemByNum(demCode);
                                landitem = LandExpCoefficient.GetExpItemByNum(landCode);
                                argsM = item == null ? RegionExp1 : (float)item.APara;
                                argsM2 = item == null ? RegionExp2 : (float)item.BPara;
                                argsLand = landitem == null ? LandExp1 : (float)landitem.APara;
                                argsLand2 = landitem == null ? LandExp2 : (float)landitem.BPara;
                                rvOutVistor[0].RasterBandsData[0][index] = (Int16)((visiableValue / VisibleZoom + (argsM * nearInfraredValue / NearInfraredZoom) - tempNDVIExp * (argsLand + argsM * argsLand2)) / ((1 - tempNDVIExp) * (Math.Sqrt(argsM2 * argsM2 + 1))) * mpdiZoom);
                            }
                        }
                    }));
                    //执行
                    rfr.Excute(InvaildValue);
                    mpdiRes = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    mpdiRes.SetDispaly(false);

                    _argumentProvider.SetArg("CursorInfo:MPDI-NDVIMax", ndviCalcMax);
                    _argumentProvider.SetArg("CursorInfo:MPDI-NDVIMin", ndviCalcMin);
                    return mpdiRes;
                }
            }
            finally
            {
                if (edgeLand != null)
                    edgeLand.Dispose();
                if (pviPrd != null)
                    pviPrd.Dispose();
                if (normalizationSuccess && currPrd != null)
                    currPrd.Dispose();
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

        #region 归一化反射率

        private string NormalizationProcess(IRasterDataProvider currPrd, Action<int, string> progressTracker)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int VisibleCH = TryGetBandNo(bandNameRaster, "RedBand");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NirBand");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarBand");
            double VisibleZoom = (double)_argumentProvider.GetArg("RedBand_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NirBand_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarBand_Zoom");

            int SunZCH = TryGetBandNo(bandNameRaster, "SunZBand");
            int SatZCH = TryGetBandNo(bandNameRaster, "SatZBand");
            int SunACH = TryGetBandNo(bandNameRaster, "SunABand");
            int SatACH = TryGetBandNo(bandNameRaster, "SatABand");
            int LandConveryCH = TryGetBandNo(bandNameRaster, "LandCoveryBand");
            double SunZZoom = (double)_argumentProvider.GetArg("SunZBand_Zoom");
            double SatZZoom = (double)_argumentProvider.GetArg("SatZBand_Zoom");
            double SunAZoom = (double)_argumentProvider.GetArg("SunABand_Zoom");
            double SatAZoom = (double)_argumentProvider.GetArg("SatABand_Zoom");
            double LandCoveryZoom = (double)_argumentProvider.GetArg("LandCoveryBand_Zoom");
            AngleParModel angleArg = _argumentProvider.GetArg("Angle") as AngleParModel;

            _zooms = new float[] { (float)VisibleZoom, (float)NearInfraredZoom, (float)FarInfrared11Zoom,
                                   (float)SunZZoom,(float) SunAZoom, (float)SatZZoom,(float) SatAZoom, (float)LandCoveryZoom };
            if (_progressTracker != null)
                _progressTracker(5, "开始读取数据文件...");
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider landcoverRaster = null;
            int outwidth, outheight;
            try
            {
                #region 波段数据读取
                if (_progressTracker != null)
                    _progressTracker(7, "正在读取当前影像文件...");
                float prjReslX = currPrd.ResolutionX;
                float prjReslY = currPrd.ResolutionY;
                double prjMinLon = currPrd.CoordEnvelope.MinX;
                double prjMaxLat = currPrd.CoordEnvelope.MaxY;
                outwidth = currPrd.Width;
                outheight = currPrd.Height;
                UInt16[] reddata = GetDataValue<UInt16>(currPrd.GetRasterBand(VisibleCH), 0, 0, outwidth, outheight);
                UInt16[] IRdata = GetDataValue<UInt16>(currPrd.GetRasterBand(NearInfraredCH), 0, 0, outwidth, outheight);
                UInt16[] Firdata = GetDataValue<UInt16>(currPrd.GetRasterBand(FarInfrared11CH), 0, 0, outwidth, outheight);
                if (_progressTracker != null)
                    _progressTracker(9, "正在读取角度数据文件...");
                Int16[] sza, saa, vza, vaa;
                angleRaster = GeoDataDriver.Open(angleArg.FileAsunZ) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return null;
                sza = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(angleArg.FileAsunA) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return null;
                saa = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(angleArg.FileAsatZ) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return null;
                vza = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(angleArg.FileAsatA) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return null;
                vaa = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                if (_progressTracker != null)
                    _progressTracker(11, "正在读取LandCover数据文件...");
                landcoverRaster = GeoDataDriver.Open(angleArg.FileLandConvery) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return null;
                #endregion
                output = new ushort[3][];
                output[0] = new ushort[reddata.Length];
                output[1] = new ushort[reddata.Length];
                output[2] = new ushort[reddata.Length];
                #region 分块并行处理
                int parts = 7;
                int step = reddata.Length / parts;
                int[] istart = new int[parts];
                int[] iend = new int[parts];
                istart[0] = 0;
                iend[0] = 0 + step;
                for (int p = 1; p < parts; p++)
                {
                    istart[p] = istart[p - 1] + step;
                    iend[p] = iend[p - 1] + step;
                }
                iend[parts - 1] = reddata.Length;
                var tasks = new Action[] { 
                    () => CalNormalizationParts(istart[0], iend[0], outwidth,reddata, IRdata, Firdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat) ,
                    () => CalNormalizationParts(istart[1], iend[1], outwidth,reddata, IRdata, Firdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                    () => CalNormalizationParts(istart[2], iend[2], outwidth,reddata, IRdata, Firdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                    () => CalNormalizationParts(istart[3], iend[3], outwidth,reddata, IRdata, Firdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                    () => CalNormalizationParts(istart[4], iend[4], outwidth,reddata, IRdata, Firdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                    () => CalNormalizationParts(istart[5], iend[5], outwidth,reddata, IRdata, Firdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                    () => CalNormalizationParts(istart[6], iend[6], outwidth,reddata, IRdata, Firdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                };
                if (_progressTracker != null)
                    _progressTracker(13, "正在逐点归一化反射率,请稍候...");
                System.Threading.Tasks.Parallel.Invoke(tasks);

                #endregion

                #region 建立输出文件
                if (_progressTracker != null)
                    _progressTracker(75, "正在输出归一化反射率结果,请稍候...");
                return OutputNormalization(currPrd);
                #endregion
            }
            finally
            { }
        }

        public void CalNormalizationParts(int istart, int iend, int outwidth, UInt16[] reddata, UInt16[] IRdata, UInt16[] Firdata, IRasterDataProvider landcoverRaster, Int16[] sza, Int16[] saa, Int16[] vza, Int16[] vaa, float prjresl, double minlon, double maxlat)
        {
            float pixelX, pixelY;
            for (int i = istart; i < iend; i++)
            {
                try
                {
                    if (reddata[i] == _prjbandfillvalue && IRdata[i] == _prjbandfillvalue )
                    {
                        output[0][i] = normalizationFillVlue;
                        output[1][i] = normalizationFillVlue;
                        output[2][i] = normalizationFillVlue;
                        continue;
                    }
                    int y = i / outwidth;
                    int x = i % outwidth;
                    pixelX = (float)(minlon + x * prjresl);
                    pixelY = (float)(maxlat - y * prjresl);
                    if (sza[i] == _prjbandfillvalue || saa[i] == _prjbandfillvalue || vza[i] == _prjbandfillvalue || vaa[i] == _prjbandfillvalue)
                    {
                        output[0][i] = normalizationFillVlue;
                        output[1][i] = normalizationFillVlue;
                        output[2][i] = normalizationFillVlue;
                        continue;
                    }
                    float[] angles = new float[] { sza[i] / _zooms[3], saa[i] / _zooms[4], vza[i] / _zooms[5], vaa[i] / _zooms[6] };
                    float lctReslX = landcoverRaster.ResolutionX;
                    float lctReslY = landcoverRaster.ResolutionY;
                    double lctMinLon = landcoverRaster.CoordEnvelope.MinX;
                    double lctMaxLat = landcoverRaster.CoordEnvelope.MaxY;
                    //根据像元的经纬度去LandCoverType文件中找相应的偏移量xoffset和yoffset，offset =xoffset+yoffset*landcoverRaster.Width;
                    int xoffset = (int)Math.Floor((pixelX - lctMinLon) / lctReslX);
                    int yoffset = (int)Math.Floor((lctMaxLat - pixelY) / lctReslY);
                    if (xoffset < 0 || yoffset < 0)
                    {
                        output[0][i] = normalizationFillVlue;
                        output[1][i] = normalizationFillVlue;
                        output[2][i] = normalizationFillVlue;
                        continue;
                    }
                    byte[] lct = GetDataValue<byte>(landcoverRaster.GetRasterBand(1), xoffset, yoffset, 1, 1);
                    UInt16[] redNir = NormalizationRedNir.NRedNir(new UInt16[] { reddata[i], IRdata[i] }, angles, lct[0], new float[] { _zooms[0], _zooms[1] });
                    output[0][i] = redNir[0];
                    output[1][i] = redNir[1];
                    output[2][i] = Firdata[i];
                }
                catch (System.Exception ex)
                {
                    PrintInfo(ex.Message);
                    output[0][i] = normalizationFillVlue;
                    output[1][i] = normalizationFillVlue;
                    output[2][i] = normalizationFillVlue;
                    continue;
                }
            }
        }

        public string OutputNormalization(IRasterDataProvider currPrd)
        {
            int outwidth = currPrd.Width, outheight = currPrd.Height;
            try
            {
                #region 建立输出文件
                RasterIdentify datid = new RasterIdentify(Path.GetFileName(currPrd.fileName));
                datid.ProductIdentify = _subProductDef.ProductDef.Identify;
                datid.SubProductIdentify = "NRSB";
                float outResolution = currPrd.ResolutionX;
                string normalizationFileName = datid.ToWksFullFileName(".ldf");
                if (!CreateRaster(normalizationFileName, 3, enumDataType.UInt16, currPrd))
                    return null;
                string resultFile = Path.Combine(Path.GetDirectoryName(normalizationFileName), Path.GetFileNameWithoutExtension(normalizationFileName) + ".dat");
                if (File.Exists(normalizationFileName))
                {
                    if (File.Exists(resultFile))
                        File.Delete(resultFile);
                    File.Move(normalizationFileName, resultFile);
                    return resultFile;
                }
                else
                    return null;
                #endregion
            }
            catch (System.Exception ex)
            {
                PrintInfo(ex.Message);
                return null;
            }
        }

        public bool CreateRaster(string outFileName, int bandCount, enumDataType datatype, IRasterDataProvider referProvider)
        {
            CoordEnvelope env = referProvider.CoordEnvelope;
            float resX = referProvider.ResolutionX;
            float resY = referProvider.ResolutionY;
            int width = referProvider.Width;
            int height = referProvider.Height;
            Project.ISpatialReference spatialRef = referProvider.SpatialRef;
            List<string> options = new List<string>();
            options.Add("INTERLEAVE=BSQ");
            options.Add("VERSION=LDF");
            options.Add("WITHHDR=TRUE");
            options.Add("SPATIALREF=" + spatialRef.ToProj4String());
            options.Add("MAPINFO={" + 1 + "," + 1 + "}:{" + env.MinX + "," + env.MaxY + "}:{" + resX + "," + resY + "}"); //=env.ToMapInfoString(new Size(width, height));
            options.Add("BANDNAMES= " + "red,nir,fir");
            if (!Directory.Exists(Path.GetDirectoryName(outFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
            using (IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
            {
                using (RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandCount, datatype, options.ToArray()) as RasterDataProvider)
                {
                    unsafe
                    {
                        fixed (UInt16* ptr = output[0])
                        {
                            IntPtr buffer = new IntPtr(ptr);
                            outRaster.GetRasterBand(1).Write(0, 0, width, height, buffer, enumDataType.UInt16, width, height);
                        }
                        fixed (UInt16* ptr = output[1])
                        {
                            IntPtr buffer = new IntPtr(ptr);
                            outRaster.GetRasterBand(2).Write(0, 0, width, height, buffer, enumDataType.UInt16, width, height);
                        }
                        fixed (UInt16* ptr = output[2])
                        {
                            IntPtr buffer = new IntPtr(ptr);
                            outRaster.GetRasterBand(3).Write(0, 0, width, height, buffer, enumDataType.UInt16, width, height);
                        }
                    }
                    return true;
                }
            }
        }

        public static unsafe T[] GetDataValue<T>(IRasterBand band, int xoffset = 0, int yoffset = 0, int width = 1, int height = 1)
        {
            enumDataType dataType = band.DataType;
            //width = band.Width;
            //height = band.Height;
            int length = width * height;
            switch (dataType)
            {
                case enumDataType.Float:
                    {
                        float[] buffer = new float[length];
                        fixed (float* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Float, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.UInt16:
                    {
                        UInt16[] srcBuffer = new UInt16[length];
                        fixed (UInt16* dataPtr = srcBuffer)
                        {
                            IntPtr srcBufferPtr = new IntPtr(dataPtr);
                            band.Read(0, 0, width, height, srcBufferPtr, dataType, width, height);//读一个波段的数据
                        }
                        return srcBuffer as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] buffer = new short[length];
                        fixed (short* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Int16, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] buffer = new Byte[width * height];
                        fixed (Byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Byte, width, height);
                        }
                        return buffer as T[];
                    }

            }
            return null;
        }

        #endregion

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