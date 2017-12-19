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
    public class SubProductHFIILST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductHFIILST(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "HFIIAlgorithm")
                return HFIIAlgorithmCompute(progressTracker);
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult HFIIAlgorithmCompute(Action<int, string> progressTracker)
        {
            MemPixelFeatureMapper<UInt16> resultTemp = null;
            int LstBandCH = (int)_argumentProvider.GetArg("LstBand");
            double LstBandZoom = (double)_argumentProvider.GetArg("LstBand_Zoom");
            double HFIIZoom = (double)_argumentProvider.GetArg("HFIIZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");
            UInt16 minCount = (UInt16)(_argumentProvider.GetArg("minCount"));
            UInt16[] nanValues = GetNanValues("CloudyValue");
            UInt16[] waterValues = GetNanValues("WaterValue");
            float LSTMin = (float)((float.Parse(_argumentProvider.GetArg("VaildMin").ToString()) + 273) * LstBandZoom);
            float LSTMax = (float)((float.Parse(_argumentProvider.GetArg("VaildMax").ToString()) + 273) * LstBandZoom);
            string lstFile = GetStringArg("LSTFile");
            if (string.IsNullOrEmpty(lstFile) || !File.Exists(lstFile))
            {
                PrintInfo("获取数据失败,没有设定地表高温数据或数据不存在。");
                return null;
            }
            Dictionary<UInt16, int> vaildLstList = new Dictionary<UInt16, int>();
            IRasterDataProvider currPrd = GeoDataDriver.Open(lstFile) as IRasterDataProvider;
            IRasterDataProvider cloudPrd = null;
            ArgumentProvider ap = new ArgumentProvider(currPrd, null);
            resultTemp = new MemPixelFeatureMapper<UInt16>("HFII", 1000, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope, currPrd.SpatialRef);
            RasterPixelsVisitor<UInt16> rpVisitor = null;

            string cloudFile = GetStringArg("CloudFile");
            if (progressTracker != null)
                progressTracker.Invoke(5, "开始计算热效应强度指数,请稍候...");
            try
            {
                if (!string.IsNullOrEmpty(cloudFile) && File.Exists(cloudFile))
                {
                    if (progressTracker != null)
                        progressTracker.Invoke(15, "开始提取云信息,请稍候...");
                    cloudPrd = GeoDataDriver.Open(cloudFile) as IRasterDataProvider;
                    if (cloudPrd == null)
                    {
                        PrintInfo("请选择正确的云结果.");
                        return null;
                    }
                    List<RasterMaper> rms = new List<RasterMaper>();
                    RasterMaper brm = new RasterMaper(cloudPrd, new int[] { 1 });
                    rms.Add(brm);
                    RasterMaper rm = new RasterMaper(currPrd, new int[] { 1 });
                    rms.Add(rm);
                    //输出文件准备（作为输入栅格并集处理）
                    string outFileName = MifEnvironment.GetFullFileName(".dat");
                    using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                    {
                        //栅格数据映射
                        RasterMaper[] fileIns = rms.ToArray();
                        RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                        //创建处理模型
                        RasterProcessModel<UInt16, UInt16> rfr = null;
                        rfr = new RasterProcessModel<UInt16, UInt16>();
                        rfr.SetRaster(fileIns, fileOuts);
                        int totalIndex = -1;
                        if (progressTracker != null)
                            progressTracker.Invoke(35, "开始处理云和地表温度信息,请稍候...");
                        rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, rfrAOI) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    totalIndex++; ;
                                    UInt16 cloudData = rvInVistor[0].RasterBandsData[0][index];
                                    UInt16 valueData = rvInVistor[1].RasterBandsData[0][index];
                                    if (cloudData != 0 || nanValues.Contains(valueData))
                                        resultTemp.Put(totalIndex, defCloudy);
                                    else if (waterValues.Contains(valueData))
                                        resultTemp.Put(totalIndex, defWater);
                                    else if (valueData == 0)
                                        resultTemp.Put(totalIndex, 0);
                                    else
                                    {
                                        if (vaildLstList.ContainsKey(valueData))
                                            vaildLstList[valueData]++;
                                        else
                                            vaildLstList.Add(valueData, 1);
                                    }
                                }
                            }
                        }));
                        //执行
                        rfr.Excute();
                    }
                }
                else
                {
                    if (progressTracker != null)
                        progressTracker.Invoke(15, "开始提取地表温度信息,请稍候...");
                    rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
                    rpVisitor.VisitPixel(new int[] { LstBandCH },
                     (index, values) =>
                     {
                         if (nanValues.Contains(values[0]))
                             resultTemp.Put(index, defCloudy);
                         else if (waterValues.Contains(values[0]))
                             resultTemp.Put(index, defWater);
                         else if (values[0] == 0)
                             resultTemp.Put(index, 0);
                         else
                         {
                             if (vaildLstList.ContainsKey(values[0]))
                                 vaildLstList[values[0]]++;
                             else
                                 vaildLstList.Add(values[0], 1);
                         }
                     });
                }
                UInt16 minValue = 0;
                UInt16 maxValue = UInt16.MaxValue;
                GetMinMaxValue(vaildLstList, LSTMin, LSTMax, minCount, out minValue, out maxValue);
                if (maxValue == minValue)
                    return null;
                UInt16 result = 0;
                if (rpVisitor == null)
                    rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
                if (progressTracker != null)
                    progressTracker.Invoke(65, "开始计算热效应强度信息,请稍候...");
                int[] tempIndexes = resultTemp.Indexes.ToArray();
                double resultHFII = 0f;
                rpVisitor.VisitPixel(new int[] { LstBandCH },
                 (index, values) =>
                 {
                     if (!tempIndexes.Contains(index))
                     {
                         resultHFII = ((float)(values[0] - minValue)) / (maxValue - minValue) * HFIIZoom;
                         if (resultHFII <= 0)
                             result = (UInt16)1;
                         else
                             result = (UInt16)resultHFII;
                         resultTemp.Put(index, result);
                     }
                 });
                if (progressTracker != null)
                    progressTracker.Invoke(85, "开始保存热效应强度信息,请稍候...");
                return GenrateIInterested(resultTemp, currPrd, "HFII");
            }
            finally
            {
                if (currPrd != null)
                    currPrd.Dispose();
                if (cloudPrd != null)
                    cloudPrd.Dispose();
            }
        }

        private IFileExtractResult GenrateIInterested(MemPixelFeatureMapper<UInt16> result, IRasterDataProvider currPrd, string subProductIndentify)
        {
            RasterIdentify id = new RasterIdentify(currPrd.fileName.ToUpper());
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = subProductIndentify;
            id.IsOutput2WorkspaceDir = true;
            using (IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope.Clone(), currPrd.SpatialRef))
            {
                iir.Put(result);
                IFileExtractResult fileResult = new FileExtractResult(subProductIndentify, iir.FileName);
                fileResult.SetDispaly(false);
                return fileResult;
            }
        }

        private void GetMinMaxValue(Dictionary<UInt16, int> vaildLstList, float LSTMin, float LSTMax, UInt16 minCount, out UInt16 minValue, out UInt16 maxValue)
        {
            minValue = 0;
            maxValue = UInt16.MaxValue;
            vaildLstList = (from entry in vaildLstList
                            where entry.Key >= LSTMin & entry.Value >= minCount
                            orderby entry.Key ascending
                            select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            minValue = vaildLstList.First().Key;

            vaildLstList = (from entry in vaildLstList
                            where entry.Key <= LSTMax & entry.Value >= minCount
                            orderby entry.Key descending
                            select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            maxValue = vaildLstList.First().Key;
        }

        private string GetStringArg(string argContent)
        {
            object obj = _argumentProvider.GetArg(argContent);
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return string.Empty;
            return obj.ToString();
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

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}