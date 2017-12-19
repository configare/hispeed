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

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class SubProductBinaryLMCZ : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinaryLMCZ(SubProductDef subProductDef)
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
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA11Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA14Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA16Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA18Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "FY3Algorithm")
            {
                Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("OrbitFileSelectType") as Dictionary<string, string[]>;
                if (pathDic == null || pathDic.Keys.Contains("CurrentRaster")) //选择当前影像进行计算
                    //return ComputeByCurrentRaster(progressTracker);
                    return ComputeByCurrentRaster(_argumentProvider.DataProvider, progressTracker);
                if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                    //return ExtractLST(pathDic["DirectoryPath"][0]);
                    return ExtractLST(pathDic["DirectoryPath"][0], progressTracker);
                if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                    //return ComputeByFiles(pathDic["FileNames"]);
                    return ComputeByFiles(pathDic["FileNames"], progressTracker);
                return null;
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FY2DAlgorithm" ||
                     _argumentProvider.GetArg("AlgorithmName").ToString() == "FY2EAlgorithm")
            {
                Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("OrbitFileSelectType") as Dictionary<string, string[]>;
                if (pathDic == null || pathDic.Keys.Contains("CurrentRaster")) //选择当前影像进行计算
                    return ComputeByFilesGeostationary(progressTracker, new string[] { _argumentProvider.DataProvider.fileName });
                if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                    return ExtractLSTGeostationary(progressTracker, pathDic["DirectoryPath"][0]);
                if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                    return ComputeByFilesGeostationary(progressTracker, pathDic["FileNames"]);
                return null;
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "BasePrdAlgorithm")
            {
                return ComputeByPrdFiles(progressTracker);
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        #region 基于产品计算

        private IExtractResult ComputeByPrdFiles(Action<int, string> progressTracker)
        {
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与计算的数据！");
                return null;
            }
            int LstBandCH = (int)_argumentProvider.GetArg("LSTBand");
            double LstBandZoom = (double)_argumentProvider.GetArg("LSTBand_Zoom");
            double LMCZZoom = (double)_argumentProvider.GetArg("LMCZZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");
            UInt16[] nanValues = GetNanValues("CloudyValue");
            UInt16[] waterValues = GetNanValues("WaterValue");
            float LSTMin = float.Parse(_argumentProvider.GetArg("LSTMin").ToString());

            int minValueLength = 50;
            int aoiVaildValueCount = 0;
            float sumAOILstValue = 0;
            MemPixelFeatureMapper<UInt16> resultTemp = null;

            UInt16[] minLstValue = new UInt16[minValueLength];
            for (int i = 0; i < minValueLength; i++)
                minLstValue[i] = UInt16.MaxValue;

            int[] aoi = _argumentProvider.AOI;
            //从AOI中去除云
            string cloudFile = GetStringArg("CloudFile");
            int[] cloudIndex = null;

            int length = fileNames.Length;
            for (int i = 0; i < length; i++)
            {
                IRasterDataProvider currPrd = GeoDataDriver.Open(fileNames[i]) as IRasterDataProvider;
                try
                {
                    if (string.IsNullOrEmpty(cloudFile))
                    {
                        cloudFile = Path.Combine(Path.GetDirectoryName(fileNames[i]), Path.GetFileName(fileNames[i]).Replace("DBLV", "0CLM"));
                        if (!File.Exists(cloudFile))
                            cloudFile = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(cloudFile) && File.Exists(cloudFile))
                        UpdateAOI(ref aoi, currPrd, cloudFile, out cloudIndex);
                    //
                    Size size = new Size(currPrd.Width, currPrd.Height);
                    Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);

                    resultTemp = new MemPixelFeatureMapper<UInt16>("LMCZ", 1000, size, currPrd.CoordEnvelope, currPrd.SpatialRef);
                    ArgumentProvider ap = new ArgumentProvider(currPrd, null);
                    RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);

                    rpVisitor.VisitPixel(new int[] { LstBandCH },
                     (index, values) =>
                     {
                         if (values[0] == 0)
                             resultTemp.Put(index, 0);
                         else if ((cloudIndex != null && cloudIndex.Contains(index)) || nanValues.Contains(values[0]))
                             resultTemp.Put(index, defCloudy);
                         else if (waterValues.Contains(values[0]))
                             resultTemp.Put(index, defWater);
                         else
                         {
                             UpdateMinValueArrary(ref minLstValue, values[0], LSTMin, LMCZZoom, true);
                             if (aoi != null && aoi.Contains(index))
                             {
                                 sumAOILstValue += values[0];
                                 aoiVaildValueCount++;
                             }
                             resultTemp.Put(index, values[0]);
                         }
                     });
                    UInt16 midMinValue = 0;
                    GetMinValue(minLstValue, minValueLength, out midMinValue);
                    UInt16 avgAOILstValue = aoiVaildValueCount == 0 ? midMinValue : (UInt16)(sumAOILstValue / aoiVaildValueCount);
                    IExtractResultArray arrary = new ExtractResultArray("LST");
                    IFileExtractResult tempDBLV = GenrateTempIInterested(resultTemp, currPrd);
                    if (tempDBLV == null)
                        return null;
                    IRasterDataProvider tempDBLVProvider = null;
                    try
                    {
                        tempDBLVProvider = GeoDataDriver.Open(tempDBLV.FileName, null) as IRasterDataProvider;
                        if (tempDBLVProvider == null)
                            return null;
                        IExtractResult resultChaZhiTemp = ChaZhiResult(tempDBLVProvider, avgAOILstValue);
                        if (resultChaZhiTemp == null)
                            return null;
                        arrary.Add(resultChaZhiTemp as IFileExtractResult);
                        return arrary;
                    }
                    finally
                    {
                        if (tempDBLVProvider != null)
                            tempDBLVProvider.Dispose();
                    }

                }
                finally
                {
                    if (currPrd != null)
                        currPrd.Dispose();
                }
            }
            return null;
        }

        private UInt16[] GetNanValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<ushort> values = new List<ushort>();
                    ushort value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (UInt16.TryParse(valueStrings[i], out value))
                            values.Add(value);
                    }
                    if (values.Count > 0)
                    {
                        return values.ToArray();
                    }
                }
            }
            return null;
        }

        #endregion

        #region 极轨星计算

        private IExtractResult ComputeByCurrentRaster(IRasterDataProvider currPrd, Action<int, string> progressTracker)
        {
            int minValueLength = 50;
            int aoiVaildValueCount = 0;
            float sumAOILstValue = 0;
            MemPixelFeatureMapper<UInt16> resultTemp = null;
            float curNDVI = 0f;
            //IRasterDataProvider currPrd = _argumentProvider.DataProvider;
            if (currPrd == null)
                return null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible"); //(int)_argumentProvider.GetArg("Visible");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NearInfrared"); //(int)_argumentProvider.GetArg("NearInfrared");
            int MiddInfraredCH = TryGetBandNo(bandNameRaster, "MiddInfrared"); //(int)_argumentProvider.GetArg("MiddInfrared");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double MiddInfraredZoom = (double)_argumentProvider.GetArg("MiddInfrared_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");

            if (VisibleCH == -1 || NearInfraredCH == -1 || MiddInfraredCH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            bool useCLMFile = true;
            string clmRasterFile = _argumentProvider.GetArg("clmFile").ToString();
            if (string.IsNullOrEmpty(clmRasterFile) || !File.Exists(clmRasterFile))
                useCLMFile = false;

            double lstZoom = (double)_argumentProvider.GetArg("LSTZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");
            float LSTMin = float.Parse(_argumentProvider.GetArg("LSTMin").ToString());
            bool isUseLstDefMin = (bool)_argumentProvider.GetArg("isUseLstDefMin");

            float a0 = (float)_argumentProvider.GetArg("A0");
            float alfa = (float)_argumentProvider.GetArg("alfa");
            float beta = (float)_argumentProvider.GetArg("beta");
            float gama = (float)_argumentProvider.GetArg("gama");
            float alfa2 = (float)_argumentProvider.GetArg("alfa2");
            float beta2 = (float)_argumentProvider.GetArg("beta2");

            float b4m = (float)_argumentProvider.GetArg("b4m");
            float b4n = (float)_argumentProvider.GetArg("b4n");
            float b5m = (float)_argumentProvider.GetArg("b5m");
            float b5n = (float)_argumentProvider.GetArg("b5n");

            float b4Water = (float)_argumentProvider.GetArg("b4Water");
            float b5Water = (float)_argumentProvider.GetArg("b5Water");
            float b4Soil = (float)_argumentProvider.GetArg("b4Soil");
            float b5Soil = (float)_argumentProvider.GetArg("b5Soil");
            float b4VGT = (float)_argumentProvider.GetArg("b4VGT");
            float b5VGT = (float)_argumentProvider.GetArg("b5VGT");

            float NearInfraredCLMMin = float.Parse(_argumentProvider.GetArg("NearInfraredCLMMin").ToString());
            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared1112CLMMin = float.Parse(_argumentProvider.GetArg("FarInfrared1112CLMMin").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            ArgumentProvider ap = new ArgumentProvider(currPrd, null);
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
            float b4Emiss = 0;
            float b5Emiss = 0;
            double pv = 0;
            //4 5 通道比辐射率均值 差值
            float EmissMean = 0;
            float DeltaEmiss = 0;
            //LST计算系数 P M
            double P = 0;
            double M = 0;

            UInt16[] minLstValue = new UInt16[minValueLength];
            for (int i = 0; i < minValueLength; i++)
                minLstValue[i] = UInt16.MaxValue;
            UInt16 tempLstValue = 0;

            int[] aoi = _argumentProvider.AOI;
            //从AOI中去除云
            string cloudFile = GetStringArg("CloudFile");
            int[] cloudIndex = null;
            if (!string.IsNullOrEmpty(cloudFile) && File.Exists(cloudFile))
                UpdateAOI(ref aoi, currPrd, cloudFile, out cloudIndex);
            //
            Size size = new Size(currPrd.Width, currPrd.Height);
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);

            resultTemp = new MemPixelFeatureMapper<UInt16>("DBLV", 1000, size, currPrd.CoordEnvelope, currPrd.SpatialRef);
            MemPixelFeatureMapper<float> ndviTemp = new MemPixelFeatureMapper<float>("NDVI", 1000, size, currPrd.CoordEnvelope, currPrd.SpatialRef);
            //热效应不考虑绘制感兴趣区域进行计算的情况，AOI区域只代表最小值获取区域
            //rpVisitor.VisitPixel(aoiRect, aoi, new int[] { VisibleCH, NearInfraredCH, MiddInfraredCH, FarInfrared11CH, FarInfrared12CH },
            rpVisitor.VisitPixel(new int[] { VisibleCH, NearInfraredCH, MiddInfraredCH, FarInfrared11CH, FarInfrared12CH },
             (index, values) =>
             {
                 curNDVI = GetNDVI(values, 1, 0);
                 ndviTemp.Put(index, curNDVI);
                 if (values[0] == 0)
                     resultTemp.Put(index, 0);
                 else if ((cloudIndex != null && cloudIndex.Contains(index)) || //人机交互判识的云
                           values[1] / NearInfraredZoom > NearInfraredCLMMin && values[3] / FarInfrared11Zoom < FarInfrared11CLMMax && Math.Abs(values[2] / MiddInfraredZoom - values[3] / FarInfrared11Zoom) > FarInfrared1112CLMMin)
                     resultTemp.Put(index, defCloudy);
                 else
                 {
                     if (values[3] / FarInfrared11Zoom > FarInfrared11WaterMin && curNDVI < NDVIWaterMax)
                     {
                         resultTemp.Put(index, defWater);
                         b4Emiss = b4Water;
                         b5Emiss = b5Water;
                     }
                     else
                     {
                         if (curNDVI < 0.2)
                         {
                             b4Emiss = b4Soil;
                             b5Emiss = b5Soil;
                         }
                         else if (curNDVI > 0.5)
                         {
                             b4Emiss = b4VGT;
                             b5Emiss = b5VGT;
                         }
                         else if (curNDVI >= 0.2 && curNDVI <= 0.5)
                         {
                             pv = Math.Pow((curNDVI - 0.2) / (0.5 - 0.2), 2);
                             b4Emiss = (float)(b4m * pv + b4n);
                             b5Emiss = (float)(b5m * pv + b5n);
                         }

                         EmissMean = (b4Emiss + b5Emiss) / 2;
                         DeltaEmiss = (b4Emiss - b5Emiss);

                         P = 1 + alfa * (1 - EmissMean) / EmissMean + beta * DeltaEmiss / Math.Pow(EmissMean, 2);
                         M = gama + alfa2 * (1 - EmissMean) / EmissMean + beta2 * DeltaEmiss / Math.Pow(EmissMean, 2);

                         tempLstValue = (UInt16)((a0 + P * (values[3] / FarInfrared11Zoom + values[4] / FarInfrared12Zoom) / 2
                                                 + M * (values[3] / FarInfrared11Zoom - values[4] / FarInfrared12Zoom) / 2) * lstZoom);
                         UpdateMinValueArrary(ref minLstValue, tempLstValue, LSTMin, lstZoom, isUseLstDefMin);
                         if (aoi != null && aoi.Contains(index))
                         {
                             sumAOILstValue += tempLstValue;
                             aoiVaildValueCount++;
                         }
                         resultTemp.Put(index, tempLstValue);
                     }
                 }
             });
            UInt16 midMinValue = 0;
            GetMinValue(minLstValue, minValueLength, out midMinValue);
            UInt16 avgAOILstValue = aoiVaildValueCount == 0 ? midMinValue : (UInt16)(sumAOILstValue / aoiVaildValueCount);
            IExtractResultArray arrary = new ExtractResultArray("LST");
            IFileExtractResult dblvFile = GenrateIInterested(resultTemp, currPrd);
            if (dblvFile == null)
                return null;
            IRasterDataProvider dblvProvider = null;
            try
            {
                dblvProvider = GeoDataDriver.Open(dblvFile.FileName, null) as IRasterDataProvider;
                if (dblvProvider == null)
                    return null;
                IExtractResult resultChaZhiTemp = ChaZhiResult(dblvProvider, avgAOILstValue);
                if (resultChaZhiTemp == null)
                    return null;
                dblvFile.SetDispaly(false);
                dblvFile.Add2Workspace = true;
                arrary.Add(dblvFile);
                arrary.Add(resultChaZhiTemp as IFileExtractResult);
                return arrary;
            }
            finally
            {
                if (dblvProvider != null)
                    dblvProvider.Dispose();
            }
        }

        private void GetMinValue(UInt16[] minLstValue, int minValueLength, out UInt16 minValue)
        {
            minValue = minLstValue[minValueLength / 2];
            if (minValueLength != 0 && minValue == UInt16.MaxValue)
                GetMinValue(minLstValue, minValueLength / 2, out minValue);
            else
                return;
        }

        private IExtractResult ChaZhiResult(IRasterDataProvider dblvProvider, ushort midMinValue)
        {
            bool isChaZhi = (bool)_argumentProvider.GetArg("isChaZhi");
            if (!isChaZhi)
                return null;
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");

            Size tempSize = new Size(dblvProvider.Width, dblvProvider.Height);
            MemPixelFeatureMapper<Int16> resultChaZhiTemp = new MemPixelFeatureMapper<Int16>("LMCZ", 1000, tempSize, dblvProvider.CoordEnvelope, dblvProvider.SpatialRef);
            ArgumentProvider ap = new ArgumentProvider(dblvProvider, null);
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
            int[] aoi = _argumentProvider.AOI;
            UInt16 tempValue = 0;
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, tempSize);
            //热效应不考虑绘制感兴趣区域进行计算的情况，AOI区域只代表最小值获取区域
            //rpVisitor.VisitPixel(aoiRect, aoi, new int[] { 1 },
            rpVisitor.VisitPixel(new int[] { 1 },
             (index, values) =>
             {
                 tempValue = values[0];
                 if (tempValue == 0)
                     resultChaZhiTemp.Put(index, 0);
                 else if (tempValue == defWater || tempValue == defCloudy)
                     resultChaZhiTemp.Put(index, (Int16)tempValue);
                 else
                     resultChaZhiTemp.Put(index, (tempValue - midMinValue) == 0 ? (Int16)1 : (Int16)(tempValue - midMinValue));
             });
            return GenrateIInterested(resultChaZhiTemp, dblvProvider, "LMCZ");
        }

        private void UpdateMinValueArrary(ref UInt16[] minLstValue, UInt16 tempLstValue, double lstMin, double lstZoom, bool isUseLstDefMin)
        {
            int length = minLstValue.Length;
            int index = 0;
            for (index = 0; index < length; index++)
            {
                if (tempLstValue == 0 || minLstValue[index] == tempLstValue)
                    return;
                if (isUseLstDefMin && tempLstValue < (lstMin + 273) * lstZoom)
                    return;
                if (minLstValue[index] > tempLstValue)
                    break;
            }
            if (index < 50)
            {
                for (int i = length - 1; i > index; i--)
                    minLstValue[i] = minLstValue[i - 1];
                minLstValue[index] = tempLstValue;
            }
        }

        private IExtractResult ExtractLST(string dirPath, Action<int, string> progressTracker)
        {
            string[] files = GetFiles(dirPath);
            return ComputeByFiles(files, progressTracker);
        }

        /// <summary>
        /// 批量不同区域，同卫星、同传感器、同数据波段，但计算结果不太好，采用的是2013年8月的方法
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        /*
        private IExtractResult ComputeByFiles(string[] files)
        {
            MemPixelFeatureMapper<UInt16> resultTemp = null;
            IExtractResultArray result = new ExtractResultArray("DBLV");
            IRasterDataProvider currPrd = null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible"); //(int)_argumentProvider.GetArg("Visible");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NearInfrared"); //(int)_argumentProvider.GetArg("NearInfrared");
            int MiddInfraredCH = TryGetBandNo(bandNameRaster, "MiddInfrared"); //(int)_argumentProvider.GetArg("MiddInfrared");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double MiddInfraredZoom = (double)_argumentProvider.GetArg("MiddInfrared_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");

            if (VisibleCH == -1 || NearInfraredCH == -1 || MiddInfraredCH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            bool useCLMFile = true;
            string clmRasterFile = _argumentProvider.GetArg("clmFile").ToString();
            if (string.IsNullOrEmpty(clmRasterFile) || !File.Exists(clmRasterFile))
                useCLMFile = false;

            double lstZoom = (double)_argumentProvider.GetArg("LSTZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");

            float a0 = (float)_argumentProvider.GetArg("A0");
            float alfa = (float)_argumentProvider.GetArg("alfa");
            float beta = (float)_argumentProvider.GetArg("beta");
            float gama = (float)_argumentProvider.GetArg("gama");
            float alfa2 = (float)_argumentProvider.GetArg("alfa2");
            float beta2 = (float)_argumentProvider.GetArg("beta2");

            float b4m = (float)_argumentProvider.GetArg("b4m");
            float b4n = (float)_argumentProvider.GetArg("b4n");
            float b5m = (float)_argumentProvider.GetArg("b5m");
            float b5n = (float)_argumentProvider.GetArg("b5n");

            float b4Water = (float)_argumentProvider.GetArg("b4Water");
            float b5Water = (float)_argumentProvider.GetArg("b5Water");
            float b4Soil = (float)_argumentProvider.GetArg("b4Soil");
            float b5Soil = (float)_argumentProvider.GetArg("b5Soil");
            float b4VGT = (float)_argumentProvider.GetArg("b4VGT");
            float b5VGT = (float)_argumentProvider.GetArg("b5VGT");

            float NearInfraredCLMMin = float.Parse(_argumentProvider.GetArg("NearInfraredCLMMin").ToString());
            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared1112CLMMin = float.Parse(_argumentProvider.GetArg("FarInfrared1112CLMMin").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            //by chennan 上海 批量生产的AOI如何设置？？？
            for (int i = 0; i < files.Length; i++)
            {
                float curNDVI = 0f;
                currPrd = GeoDataDriver.Open(files[i], null) as IRasterDataProvider;
                if (currPrd != null)
                    try
                    {
                        ArgumentProvider ap = new ArgumentProvider(currPrd, null);
                        RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
                        float b4Emiss = 0;
                        float b5Emiss = 0;
                        double pv = 0;
                        //4 5 通道比辐射率均值 差值
                        float EmissMean = 0;
                        float DeltaEmiss = 0;
                        //LST计算系数 P M
                        double P = 0;
                        double M = 0;
                        resultTemp = new MemPixelFeatureMapper<UInt16>("DBLV", 1000, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope, currPrd.SpatialRef);
                        rpVisitor.VisitPixel(new int[] { VisibleCH, NearInfraredCH, MiddInfraredCH, FarInfrared11CH, FarInfrared12CH },
                         (index, values) =>
                         {
                             if (values[0] == 0)
                                 resultTemp.Put(index, 0);
                             else if (values[1] / NearInfraredZoom > NearInfraredCLMMin && values[3] / FarInfrared11Zoom < FarInfrared11CLMMax && Math.Abs(values[2] / MiddInfraredZoom - values[3] / FarInfrared11Zoom) > FarInfrared1112CLMMin)
                                 resultTemp.Put(index, defCloudy);
                             else
                             {
                                 curNDVI = GetNDVI(values, 1, 0);
                                 if (values[3] / FarInfrared11Zoom > FarInfrared11WaterMin && curNDVI < NDVIWaterMax)
                                 {
                                     resultTemp.Put(index, defWater);
                                     b4Emiss = b4Water;
                                     b5Emiss = b5Water;
                                 }
                                 else
                                 {
                                     if (curNDVI < 0.2)
                                     {
                                         b4Emiss = b4Soil;
                                         b5Emiss = b5Soil;
                                     }
                                     else if (curNDVI > 0.5)
                                     {
                                         b4Emiss = b4VGT;
                                         b5Emiss = b5VGT;
                                     }
                                     else if (curNDVI >= 0.2 && curNDVI <= 0.5)
                                     {
                                         pv = Math.Pow((curNDVI - 0.2) / (0.5 - 0.2), 2);
                                         b4Emiss = (float)(b4m * pv + b4n);
                                         b5Emiss = (float)(b5m * pv + b5n);
                                     }

                                     EmissMean = (b4Emiss + b5Emiss) / 2;
                                     DeltaEmiss = (b4Emiss - b5Emiss);

                                     P = 1 + alfa * (1 - EmissMean) / EmissMean + beta * DeltaEmiss / Math.Pow(EmissMean, 2);
                                     M = gama + alfa2 * (1 - EmissMean) / EmissMean + beta2 * DeltaEmiss / Math.Pow(EmissMean, 2);

                                     resultTemp.Put(index, (UInt16)((a0 + P * (values[3] / FarInfrared11Zoom + values[4] / FarInfrared12Zoom) / 2
                                                       + M * (values[3] / FarInfrared11Zoom - values[4] / FarInfrared12Zoom) / 2) * lstZoom));
                                 }
                             }
                         });
                        result.Add(GenrateIInterested(resultTemp, currPrd));
                    }
                    finally
                    {
                        currPrd.Dispose();
                    }
            }
            return result;
        }
        */

        /// <summary>
        /// 批量相同区域、同卫星、同传感器、同数据波段，采用2013年11月的方法
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private IExtractResult ComputeByFiles(string[] files, Action<int, string> progressTracker)
        {
            IExtractResult resultTemp = null;
            IExtractResultArray result = new ExtractResultArray("DBLV");
            IExtractResultArray tempArray = null;
            IRasterDataProvider currPrd = null;
            for (int i = 0; i < files.Length; i++)
            {
                currPrd = GeoDataDriver.Open(files[i], null) as IRasterDataProvider;
                if (currPrd != null)
                    try
                    {
                        resultTemp = ComputeByCurrentRaster(currPrd, progressTracker);
                        tempArray = resultTemp as IExtractResultArray;
                        if (resultTemp != null && resultTemp != null)
                        {
                            foreach (IFileExtractResult item in tempArray.PixelMappers)
                            {
                                item.Add2Workspace = true;
                                result.Add(item as IFileExtractResult);

                            }
                        }
                    }
                    finally
                    {
                        currPrd.Dispose();
                    }
            }
            return result;
        }

        #endregion

        #region 静止星计算

        private IExtractResult ComputeByCurrentRasterGeostationary(Action<int, string> progressTracker, IRasterDataProvider currPrd, bool progressControl)
        {
            if (currPrd == null)
                return null;
            int FarInfrared11CH = -1, FarInfrared12CH = -1, CloudCH = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            if (!progressControl)
            {
                FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
                FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
                CloudCH = TryGetBandNo(bandNameRaster, "CloudBand"); //(int)_argumentProvider.GetArg("CloudBand");
            }
            else
            {
                FarInfrared11CH = 1;
                FarInfrared12CH = 2;
                CloudCH = 3;
            }
            int NDVICH = TryGetBandNo(bandNameRaster, "NDVIBand"); //(int)_argumentProvider.GetArg("NDVIBand");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            double NDVIBandZoom = (double)_argumentProvider.GetArg("NDVIBand_Zoom");
            double CloudZoom = (double)_argumentProvider.GetArg("CloudBand_Zoom");

            if (CloudCH == -1 || NDVICH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string NDVIFile = GetStringArg("NDVIFile");
            if (string.IsNullOrEmpty(NDVIFile))
            {
                PrintInfo("获取NDVI文件失败。");
                return null;
            }
            float NDVIZoom = (float)_argumentProvider.GetArg("NDVIZoom");
            List<Int16> CloudyMark = GetArrayByStringArg("Cloudy");
            if (CloudyMark == null || CloudyMark.Count == 0)
            {
                PrintInfo("获取云标识失败。");
                return null;
            }
            List<Int16> WaterMark = GetArrayByStringArg("Water");
            if (WaterMark == null || WaterMark.Count == 0)
            {
                PrintInfo("获取水标识失败。");
                return null;
            }

            bool useCLMFile = true;
            string clmRasterFile = _argumentProvider.GetArg("clmFile").ToString();
            if (string.IsNullOrEmpty(clmRasterFile) || !File.Exists(clmRasterFile))
                useCLMFile = false;

            double lstZoom = (double)_argumentProvider.GetArg("LSTZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");

            float a0 = (float)_argumentProvider.GetArg("A0");
            float alfa = (float)_argumentProvider.GetArg("alfa");
            float beta = (float)_argumentProvider.GetArg("beta");
            float gama = (float)_argumentProvider.GetArg("gama");
            float alfa2 = (float)_argumentProvider.GetArg("alfa2");
            float beta2 = (float)_argumentProvider.GetArg("beta2");

            float b4m = (float)_argumentProvider.GetArg("b4m");
            float b4n = (float)_argumentProvider.GetArg("b4n");
            float b5m = (float)_argumentProvider.GetArg("b5m");
            float b5n = (float)_argumentProvider.GetArg("b5n");

            float b4Water = (float)_argumentProvider.GetArg("b4Water");
            float b5Water = (float)_argumentProvider.GetArg("b5Water");
            float b4Soil = (float)_argumentProvider.GetArg("b4Soil");
            float b5Soil = (float)_argumentProvider.GetArg("b5Soil");
            float b4VGT = (float)_argumentProvider.GetArg("b4VGT");
            float b5VGT = (float)_argumentProvider.GetArg("b5VGT");

            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared1112CLMMin = float.Parse(_argumentProvider.GetArg("FarInfrared1112CLMMin").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            ArgumentProvider ap = new ArgumentProvider(currPrd, null);
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
            float b4Emiss = 0;
            float b5Emiss = 0;
            double pv = 0;
            //4 5 通道比辐射率均值 差值
            float EmissMean = 0;
            float DeltaEmiss = 0;
            //LST计算系数 P M
            double P = 0;
            double M = 0;

            //int[] aoi = _argumentProvider.AOI;
            //Size size = new Size(currPrd.Width, currPrd.Height);
            //Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider ndviPrd = null;
            try
            {
                if (currPrd.BandCount < CloudCH)
                {
                    PrintInfo("未投影静止星云产品数据集。");
                    return null;
                }
                RasterMaper rasterRm = new RasterMaper(currPrd, new int[] { FarInfrared11CH, FarInfrared12CH, CloudCH });
                rms.Add(rasterRm);
                ndviPrd = GeoDataDriver.Open(NDVIFile) as IRasterDataProvider;
                if (ndviPrd.BandCount < NDVICH)
                {
                    PrintInfo("请选择正确的植被指数文件进行静止星陆表温度计算。");
                    return null;
                }
                RasterMaper ndviRm = new RasterMaper(ndviPrd, new int[] { NDVICH });
                rms.Add(ndviRm);

                //输出文件准备
                string outFileName = GetFileNameDefTime(new string[] { currPrd.fileName, NDVIFile }, _subProductDef.ProductDef.Identify, _identify, ".dat", null, currPrd.fileName);
                using (IRasterDataProvider outRaster = CreateOutRasterDat(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    RasterProcessModel<Int16, UInt16> rfr = new RasterProcessModel<Int16, UInt16>(progressControl ? null : progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    Int16 valueNdvi = 0, valueFar11 = 0, valueFar12 = 0, valueCloud = 0;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                                rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                                return;
                            for (int index = 0; index < dataLength; index++)
                            {
                                valueFar11 = rvInVistor[0].RasterBandsData[0][index];
                                valueFar12 = rvInVistor[0].RasterBandsData[1][index];
                                valueCloud = rvInVistor[0].RasterBandsData[2][index];
                                valueNdvi = rvInVistor[1].RasterBandsData[0][index];
                                if (isCloudy(CloudyMark, valueCloud) || isCloudy(CloudyMark, valueNdvi))
                                    rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                                else if (isWater(WaterMark, valueNdvi))
                                    rvOutVistor[0].RasterBandsData[0][index] = defWater;
                                else if (valueNdvi == Int16.MinValue || valueFar11 == Int16.MinValue || valueFar12 == Int16.MinValue)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                else if (valueFar11 == 0)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                else
                                {
                                    if (valueFar11 / FarInfrared11Zoom > FarInfrared11WaterMin && valueNdvi < NDVIWaterMax)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = defWater;
                                        b4Emiss = b4Water;
                                        b5Emiss = b5Water;
                                    }
                                    else
                                    {
                                        if (valueNdvi < 0.2)
                                        {
                                            b4Emiss = b4Soil;
                                            b5Emiss = b5Soil;
                                        }
                                        else if (valueNdvi > 0.5)
                                        {
                                            b4Emiss = b4VGT;
                                            b5Emiss = b5VGT;
                                        }
                                        else if (valueNdvi >= 0.2 && valueNdvi <= 0.5)
                                        {
                                            pv = Math.Pow((valueNdvi - 0.2) / (0.5 - 0.2), 2);
                                            b4Emiss = (float)(b4m * pv + b4n);
                                            b5Emiss = (float)(b5m * pv + b5n);
                                        }

                                        EmissMean = (b4Emiss + b5Emiss) / 2;
                                        DeltaEmiss = (b4Emiss - b5Emiss);

                                        P = 1 + alfa * (1 - EmissMean) / EmissMean + beta * DeltaEmiss / Math.Pow(EmissMean, 2);
                                        M = gama + alfa2 * (1 - EmissMean) / EmissMean + beta2 * DeltaEmiss / Math.Pow(EmissMean, 2);

                                        rvOutVistor[0].RasterBandsData[0][index] = (UInt16)((a0 + P * (valueFar11 / FarInfrared11Zoom + valueFar12 / FarInfrared12Zoom) / 2
                                                         + M * (valueFar11 / FarInfrared11Zoom - valueFar12 / FarInfrared12Zoom) / 2) * lstZoom);
                                    }
                                }
                            }
                        }));
                    rfr.Excute(Int16.MinValue);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                if (ndviPrd != null)
                    ndviPrd.Dispose();
            }
        }

        private IExtractResult ExtractLSTGeostationary(Action<int, string> progressTracker, string dirPath)
        {
            string[] files = GetFiles(dirPath);
            return ComputeByFilesGeostationary(progressTracker, files);
        }

        private IExtractResult ComputeByFilesGeostationary(Action<int, string> progressTracker, string[] files)
        {
            if (files == null || files.Length == 0)
                return null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11");//(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12");//(int)_argumentProvider.GetArg("FarInfrared12");
            int CloudCH = TryGetBandNo(bandNameRaster, "CloudBand");//(int)_argumentProvider.GetArg("CloudBand");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            double CloudZoom = (double)_argumentProvider.GetArg("CloudBand_Zoom");

            if (CloudCH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            IRasterDataProvider currPrd = null;
            IExtractResultArray array = new ExtractResultArray("LST");
            Dictionary<string, string[]> fileGroup = FileSortAndGroup(files);
            if (fileGroup == null)
                return null;
            string outFilename;
            int interval = 100 / (fileGroup.Count + 1);
            int index = 0;
            foreach (string key in fileGroup.Keys)
            {
                outFilename = CompositeGeostationaryData(progressTracker, fileGroup[key], ref index, interval);
                if (outFilename == null)
                    continue;
                currPrd = GeoDataDriver.Open(outFilename, null) as IRasterDataProvider;
                if (currPrd != null)
                    try
                    {
                        IExtractResult oneFile = ComputeByCurrentRasterGeostationary(progressTracker, currPrd, true);
                        if (oneFile == null)
                            continue;
                        array.Add(oneFile as IFileExtractResult);
                    }
                    finally
                    {
                        currPrd.Dispose();
                    }
            }
            return array;
        }

        private string CompositeGeostationaryData(Action<int, string> progressTracker, string[] groupFiles, ref int progressIndex, int interval)
        {
            int groupLength = groupFiles.Length;
            string outFilename = MifEnvironment.GetFullFileName(Path.GetFileNameWithoutExtension(groupFiles[groupLength - 1]) + ".ldf");
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11");//(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12");//(int)_argumentProvider.GetArg("FarInfrared12");
            int CloudCH = TryGetBandNo(bandNameRaster, "CloudBand");//(int)_argumentProvider.GetArg("CloudBand");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            double CloudZoom = (double)_argumentProvider.GetArg("CloudBand_Zoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider currPrd = null;
            IRasterDataProvider nextPrd = null;
            try
            {
                for (int i = 0; i < groupLength; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(groupFiles[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < CloudCH)
                    {
                        PrintInfo("未投影静止星云产品数据集。");
                        continue;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { FarInfrared11CH, FarInfrared12CH, CloudCH });
                    rms.Add(rm);
                }
                if (rms.Count == 0)
                    return null;
                using (IRasterDataProvider outRaster = CreateOutRasterLDF(outFilename, rms.ToArray(), 3))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1, 2, 3 }) };
                    RasterProcessModel<UInt16, UInt16> rfr = new RasterProcessModel<UInt16, UInt16>(null);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        UInt16[] maxFar11Data = new UInt16[dataLength];
                        byte[] nonCloudData = new byte[dataLength];//0代表有云 1代表无云
                        UInt16 valueCloud = 0, valueFar11 = 0;
                        foreach (RasterVirtualVistor<UInt16> rvs in rvInVistor)
                        {
                            for (int index = 0; index < dataLength; index++)
                            {
                                valueFar11 = rvs.RasterBandsData[0][index];
                                valueCloud = rvs.RasterBandsData[2][index];
                                if (valueCloud > 10 && valueCloud != 255)
                                {
                                    if (nonCloudData[index] == 0)
                                        rvOutVistor[0].RasterBandsData[2][index] = defCloudy;
                                    else
                                        continue;
                                }
                                else
                                {
                                    if (nonCloudData[index] == 0)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = rvs.RasterBandsData[0][index];
                                        rvOutVistor[0].RasterBandsData[1][index] = rvs.RasterBandsData[1][index];
                                        rvOutVistor[0].RasterBandsData[2][index] = rvs.RasterBandsData[2][index];
                                        maxFar11Data[index] = rvs.RasterBandsData[0][index];
                                        nonCloudData[index] = 1;
                                    }
                                    else
                                    {
                                        if (maxFar11Data[index] < valueFar11)
                                        {
                                            rvOutVistor[0].RasterBandsData[0][index] = rvs.RasterBandsData[0][index];
                                            rvOutVistor[0].RasterBandsData[1][index] = rvs.RasterBandsData[1][index];
                                            rvOutVistor[0].RasterBandsData[2][index] = rvs.RasterBandsData[2][index];
                                            maxFar11Data[index] = rvs.RasterBandsData[0][index];
                                            nonCloudData[index] = 1;
                                        }
                                        else
                                            continue;
                                    }
                                }
                            }
                        }
                    }));
                    rfr.Excute(UInt16.MinValue);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFilename, false);
                    res.SetDispaly(false);
                    return outFilename;
                }
            }
            finally
            {
                progressIndex++;
                if (progressTracker != null)
                    progressTracker(progressIndex * interval, "");
                if (currPrd != null)
                    currPrd.Dispose();
                if (nextPrd != null)
                    nextPrd.Dispose();
            }

        }

        #endregion

        private Dictionary<string, string[]> FileSortAndGroup(string[] files)
        {
            if (files == null || files.Length == 0)
                return null;
            string dateMatch = "yyyyMMdd";
            Dictionary<DateTime, string> sortTemp = (new RasterIdentify()).SortByOrbitDate(files);
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();
            List<string> fileTemp = new List<string>();
            string groupStandard = sortTemp.Keys.First().ToString(dateMatch);
            foreach (DateTime key in sortTemp.Keys)
            {
                if (groupStandard == key.ToString(dateMatch))
                {
                    fileTemp.Add(sortTemp[key]);
                }
                else
                {
                    result.Add(groupStandard, fileTemp.ToArray());
                    fileTemp = new List<string>();
                    fileTemp.Add(sortTemp[key]);
                    groupStandard = key.ToString(dateMatch);
                }
            }
            if (fileTemp.Count != 0)
                result.Add(groupStandard, fileTemp.ToArray());
            return result.Count == 0 ? null : result;
        }

        private bool isCloudy(List<Int16> mark, Int16 value)
        {
            return isContaints(mark, value);
        }

        private bool isWater(List<Int16> mark, Int16 value)
        {
            return isContaints(mark, value);
        }

        private bool isContaints(List<Int16> mark, Int16 value)
        {
            if (mark.Count == 0)
                return false;
            return mark.Contains(value);
        }

        private string GetStringArg(string argContent)
        {
            object obj = _argumentProvider.GetArg(argContent);
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return string.Empty;
            return obj.ToString();
        }

        private List<Int16> GetArrayByStringArg(string argContent)
        {
            string content = GetStringArg(argContent);
            if (string.IsNullOrEmpty(argContent))
                return null;
            string[] tempSprit = content.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            if (tempSprit == null || tempSprit.Length == 0)
                return null;
            List<Int16> result = new List<Int16>();
            int length = tempSprit.Length;
            Int16 listTemp = 0;
            for (int i = 0; i < length; i++)
            {
                if (Int16.TryParse(tempSprit[i], out listTemp))
                    result.Add(listTemp);
            }
            return result.Count == 0 ? null : result;
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
                PrintInfo("计算陆表高温的主文件未提供!");
                return null;
            }
            return files;
        }

        private IFileExtractResult GenrateIInterested(MemPixelFeatureMapper<UInt16> result, IRasterDataProvider currPrd)
        {
            RasterIdentify id = new RasterIdentify(currPrd.fileName.ToUpper());
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = "DBLV";
            id.IsOutput2WorkspaceDir = true;
            using (IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope.Clone(), currPrd.SpatialRef))
            {
                iir.Put(result);
                IFileExtractResult fileResult = new FileExtractResult("DBLV", iir.FileName);
                fileResult.SetDispaly(false);
                return fileResult;
            }
        }

        private IFileExtractResult GenrateIInterested(MemPixelFeatureMapper<Int16> result, IRasterDataProvider currPrd, string subProductIndentify)
        {
            RasterIdentify id = new RasterIdentify(currPrd.fileName.ToUpper());
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = subProductIndentify;
            id.IsOutput2WorkspaceDir = true;
            using (IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(id, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope.Clone(), currPrd.SpatialRef))
            {
                iir.Put(result);
                IFileExtractResult fileResult = new FileExtractResult(subProductIndentify, iir.FileName);
                fileResult.SetDispaly(false);
                return fileResult;
            }
        }

        private IFileExtractResult GenrateTempIInterested(MemPixelFeatureMapper<UInt16> result, IRasterDataProvider currPrd)
        {
            RasterIdentify id = new RasterIdentify(currPrd.fileName.ToUpper());
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = "DBLV";
            string filename = MifEnvironment.GetFullFileName(Path.GetFileName(id.ToWksFullFileName(".dat")));
            using (IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(filename, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope.Clone(), currPrd.SpatialRef))
            {
                iir.Put(result);
                IFileExtractResult fileResult = new FileExtractResult("DBLV", iir.FileName, false);
                fileResult.SetDispaly(false);
                return fileResult;
            }
        }

        private float GetNDVI(UInt16[] values, int nearInfraredCH, int visibleCH)
        {
            return (values[nearInfraredCH] + values[visibleCH]) == 0 ? 0f : (float)(values[nearInfraredCH] - values[visibleCH]) / (values[nearInfraredCH] + values[visibleCH]);
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private IRasterDataProvider CreateOutRasterLDF(string outFileName, RasterMaper[] inrasterMaper, int bandcount)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            ISpatialReference spatialRef = inrasterMaper[0].Raster.SpatialRef;
            string bandNames = CreateBandNames(bandcount);
            string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + outEnv.MinX + "," + outEnv.MaxY + "}:{" + resX + "," + resY + "}",
                            "BANDNAMES="+ bandNames
                        };
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandcount, enumDataType.UInt16, options) as RasterDataProvider;
            return outRaster;
        }

        private IRasterDataProvider CreateOutRasterDat(string outFileName, RasterMaper[] rasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in rasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = rasterMaper[0].Raster.ResolutionX;
            float resY = rasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private string CreateBandNames(int bandcount)
        {
            string result = null;
            for (int i = 1; i <= bandcount; i++)
            {
                result += "band " + i + ",";
            }
            return result.Substring(0, result.Length - 1);
        }

        private float[] ComputeAK(float[] xValue, float[] yValue)
        {
            if (xValue.Length != yValue.Length)
                return null;
            if (xValue.Length < 2 || yValue.Length < 2)
                return null;
            if (xValue[0] == xValue[1])
                return null;
            float a = (yValue[0] - yValue[1]) / (xValue[0] - xValue[1]);
            float b = yValue[0] - a * xValue[0];
            return new float[] { a, b };
        }

        /// <summary>
        /// 将云信息从AOI中去除
        /// </summary>
        /// <param name="aoi"></param>
        /// <param name="cloudFile"></param>
        private void UpdateAOI(ref int[] aoi, IRasterDataProvider srcData, string cloudFile, out int[] cloudIndex)
        {
            List<int> resultAOI = new List<int>();
            if (aoi != null && aoi.Length != 0)
                resultAOI.AddRange(aoi);
            List<RasterMaper> rms = new List<RasterMaper>();
            cloudIndex = null;
            List<int> cloudIndexList = new List<int>();
            IRasterDataProvider inRaster = RasterDataDriver.Open(cloudFile) as IRasterDataProvider;
            if (inRaster == null)
            {
                PrintInfo("请选择正确的云结果.");
                return;
            }
            try
            {
                RasterMaper brm = new RasterMaper(inRaster, new int[] { 1 });
                rms.Add(brm);
                RasterMaper rm = new RasterMaper(srcData, new int[] { 1 });
                rms.Add(rm);
                //输出文件准备（作为输入栅格并集处理）
                string outFileName = MifEnvironment.GetFullFileName(".dat");
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, Int16> rfr = null;
                    rfr = new RasterProcessModel<short, Int16>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, Int16>((rvInVistor, rvOutVistor, rfrAOI) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                        {
                            if (resultAOI.Count != 0)
                                for (int index = 0; index < dataLength; index++)
                                {
                                    Int16 data1 = rvInVistor[0].RasterBandsData[0][index];
                                    if (data1 != 0)
                                    {
                                        cloudIndexList.Add(index);
                                        if (resultAOI.Contains(index))
                                            resultAOI.Remove(index);
                                        continue;
                                    }
                                }
                            else
                                for (int index = 0; index < dataLength; index++)
                                {
                                    Int16 data1 = rvInVistor[0].RasterBandsData[0][index];
                                    if (data1 != 0)
                                    {
                                        cloudIndexList.Add(index);
                                        continue;
                                    }
                                }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    if (aoi != null && aoi.Length != 0)
                        aoi = resultAOI.Count == 0 ? null : resultAOI.ToArray();
                    cloudIndex = cloudIndexList.Count == 0 ? null : cloudIndexList.ToArray();
                }
            }
            finally
            {
                if (inRaster != null)
                    inRaster.Dispose();
            }
        }

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

    }
}