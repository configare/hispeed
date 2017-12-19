using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductVSWIDRT : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductVSWIDRT()
            : base()
        {
        }

        public SubProductVSWIDRT(SubProductDef subProductDef)
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
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "TNDVIAlgorithm")
            {
                return TNDVIAlgorithm_VSWI(progressTracker);
            }
            return null;
        }

        /// <summary>
        /// 算法主要计算过程
        /// </summary>
        /// <param name="progressTracker"></param>
        /// <returns>导出文件结果句柄</returns>
        private IExtractResult TNDVIAlgorithm_VSWI(Action<int, string> progressTracker)
        {
            #region 参数
            int NDVIBandCH = (int)_argumentProvider.GetArg("NDVIBand");
            if (NDVIBandCH == -1 || _argumentProvider.GetArg("NDVIFile") == null)
            {
                PrintInfo("NDVI生产所用文件或通道未设置完全,请检查!");
                return null;
            }
            //NDVI文件 
            string[] ndviFileNames = GetStringArray("NDVIFile");

            int TSBandCH = (int)_argumentProvider.GetArg("TSBand");
            if (TSBandCH == -1 || _argumentProvider.GetArg("TSFile") == null)
            {
                PrintInfo("TS生产所用文件或通道未设置完全,请检查!");
                return null;
            }
            //地表温度文件
            string[] tsFileNames = GetStringArray("TSFile");
            if (ndviFileNames.Length <= 0 || tsFileNames.Length <= 0)
            {
                PrintInfo("未选择有效输入文件！");
                return null;
            }
            if (!File.Exists(ndviFileNames[0]))
            {
                PrintInfo("NDVI文件不存在！");
                return null;
            }
            if (!File.Exists(tsFileNames[0]))
            {
                PrintInfo("TS地表温度数据不存在！");
                return null;
            }
            //NDVI缩放倍数
            double NDVIZoom = (double)_argumentProvider.GetArg("NDVIBand_Zoom");
            //地表温度缩放倍数
            double TSZoom = (double)_argumentProvider.GetArg("TSBand_Zoom");
            //生成结果放大倍数
            double BandZoom = (double)_argumentProvider.GetArg("BandZoom");
            Int16[] CloudValues = GetNanValues("CloudyValue");
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");

            Int16[] WaterValues = GetNanValues("WaterValue");
            Int16 defWater = (Int16)_argumentProvider.GetArg("defCloudy");

            Int16[] NullValues = GetNanValues("NullValue");
            Int16 defNull = (Int16)_argumentProvider.GetArg("defNullValue");
            #endregion

            //输入文件准备
            List<RasterMaper> rasterInputMaps = new List<RasterMaper>();
            IRasterDataProvider ndviPrd = null;
            IRasterDataProvider tsPrd = null;
            try
            {
                ndviPrd = RasterDataDriver.Open(ndviFileNames[0]) as IRasterDataProvider;
                if (ndviPrd.BandCount < NDVIBandCH)
                {
                    PrintInfo("请选择正确的NDVI数据进行植被供水指数计算。");
                    return null;
                }
                RasterMaper rmNdvi = new RasterMaper(ndviPrd, new int[] { NDVIBandCH });
                rasterInputMaps.Add(rmNdvi);
                tsPrd = RasterDataDriver.Open(tsFileNames[0]) as IRasterDataProvider;
                if (tsPrd.BandCount < NDVIBandCH)
                {
                    PrintInfo("请选择正确的Ts地表温度数据进行植被供水指数计算。");
                    return null;
                }
                RasterMaper rmTs = new RasterMaper(tsPrd, new int[] { NDVIBandCH });
                rasterInputMaps.Add(rmTs);
                //输出文件准备（作为输入栅格并集处理）
                string outFileName = GetFileName(tsFileNames, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                IRasterDataProvider outRaster = null;
                try
                {
                    outRaster = CreateOutRaster(outFileName, rasterInputMaps.ToArray());
                    //栅格数据映射
                    RasterMaper[] fileIns = rasterInputMaps.ToArray();
                    RasterMaper[] fileOuts;
                    fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                            return;
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int16[] ndviValues = rvInVistor[0].RasterBandsData[0];
                        Int16[] tValues = rvInVistor[1].RasterBandsData[0];
                        for (int index = 0; index < dataLength; index++)
                        {
                            Int16 refValue = 0;
                            //检验输入的云水值
                            if (GetAndCheckRefValue(ndviValues[index], CloudValues, defCloudy, WaterValues, defWater, NullValues, defNull, out refValue)
                                || GetAndCheckRefValue(tValues[index], CloudValues, defCloudy, WaterValues, defWater, out refValue))
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = refValue;
                            }
                            else if (ndviValues[index] == 0)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = defNull;
                            }
                            else
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = (Int16)((float)(tValues[index] * NDVIZoom * BandZoom / ndviValues[index] / TSZoom));
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                }
                finally
                {
                    if (outRaster != null)
                        outRaster.Dispose();
                }
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                res.SetDispaly(false);

                return res;
            }
            finally
            {
                if (ndviPrd != null)
                    ndviPrd.Dispose();
            }
        }
        /// <summary>
        /// 检验输入的云水
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="CloudValues"云标识></param>
        /// <param name="refCloudValue">缺省值（云）</param>
        /// <param name="WaterValues">水标识</param>
        /// <param name="RefWaterValue">缺省值（水）</param>
        /// <param name="NullValues">无效值</param>
        /// <param name="NullValue">无效值替换值</param>
        /// <param name="RefValue">替换目标值</param>
        /// <returns></returns>
        private bool GetAndCheckRefValue(Int16 CurrentValue, Int16[] CloudValues, Int16 refCloudValue, Int16[] WaterValues, Int16 RefWaterValue, Int16[] NullValues, Int16 NullValue, out Int16 RefValue)
        {
            if (CloudValues.Contains(CurrentValue))
            {
                RefValue = refCloudValue;
                return true;
            }
            if (WaterValues.Contains(CurrentValue))
            {
                RefValue = RefWaterValue;
                return true;
            }
            if (NullValues.Contains(CurrentValue))
            {
                RefValue = NullValue;
                return true;
            }
            RefValue = 0;
            return false;

        }
        /// <summary>
        /// 检验输入的云水
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="CloudValues"云标识></param>
        /// <param name="refCloudValue">缺省值（云）</param>
        /// <param name="WaterValues">水标识</param>
        /// <param name="RefWaterValue">缺省值（水）</param>
        /// <param name="RefValue">替换目标值</param>
        /// <returns></returns>
        private bool GetAndCheckRefValue(Int16 CurrentValue, Int16[] CloudValues, Int16 refCloudValue, Int16[] WaterValues, Int16 RefWaterValue, out Int16 RefValue)
        {
            if (CloudValues.Contains(CurrentValue))
            {
                RefValue = refCloudValue;
                return true;
            }
            if (WaterValues.Contains(CurrentValue))
            {
                RefValue = RefWaterValue;
                return true;
            }

            RefValue = 0;
            return false;

        }

        private RasterIdentify GetRasterIdentifyID(string[] fileNames)
        {
            RasterIdentify rst = new RasterIdentify(fileNames);
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;

            object obj = _argumentProvider.GetArg("OutFileIdentify");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.SubProductIdentify = obj.ToString();

            rst.IsOutput2WorkspaceDir = true;
            return rst;
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
