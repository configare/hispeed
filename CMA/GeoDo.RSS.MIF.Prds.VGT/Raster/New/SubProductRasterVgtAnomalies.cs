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
    public class SubProductRasterVgtAnomalies : CmaVgtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtAnomalies()
            : base()
        {

        }

        public SubProductRasterVgtAnomalies(SubProductDef subProductDef)
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
            if (algorith != "ANMI")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            return CalcAnmi(progressTracker);
        }

        private IExtractResult CalcAnmi(Action<int, string> progressTracker)
        {
            if (_argumentProvider.GetArg("NDVIFile") == null)
            {
                PrintInfo("请选择植被指数数据。");
                return null;
            }
            string ndvi = _argumentProvider.GetArg("NDVIFile").ToString();
            if (!File.Exists(ndvi))
            {
                PrintInfo("所选择的数据:\"" + ndvi + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviCH") == null)
            {
                PrintInfo("参数\"NdviCH\"为空。");
                return null;
            }
            int ndviCh = (int)_argumentProvider.GetArg("NdviCH");

            if (_argumentProvider.GetArg("NDVIAvgFile") == null)
            {
                PrintInfo("请选择植被指数年均值数据。");
                return null;
            }
            string avg = _argumentProvider.GetArg("NDVIAvgFile").ToString();
            if (!File.Exists(avg))
            {
                PrintInfo("所选择的数据:\"" + avg + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviAvgCH") == null)
            {
                PrintInfo("参数\"NdviAvgCH\"为空。");
                return null;
            }
            int avgCH = (int)_argumentProvider.GetArg("NdviAvgCH");
            if (ndviCh < 1 || avgCH < 1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            Int16 cloudyInvaildResult = Int16.MinValue;
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                IRasterDataProvider inRaster = RasterDataDriver.Open(ndvi) as IRasterDataProvider;
                if (inRaster.BandCount < ndviCh)
                {
                    PrintInfo("请选择正确的数据进行距平计算。");
                    return null;
                }
                RasterMaper brm = new RasterMaper(inRaster, new int[] { ndviCh });
                rms.Add(brm);
                IRasterDataProvider iRaster = RasterDataDriver.Open(avg) as IRasterDataProvider;
                if (iRaster.BandCount < avgCH)
                {
                    PrintInfo("请选择正确的数据进行距平计算。");
                    return null;
                }
                RasterMaper rm = new RasterMaper(iRaster, new int[] { avgCH });
                rms.Add(rm);
                string[] fileNames = new string[] { ndvi, avg };
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(fileNames);
                string outFileName = ri.ToWksFullFileName(".dat");
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    short[] nanValues = GetNanValues("CloudyValue");
                    short[] waterValues = GetNanValues("WaterValue");
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                            return;
                        for (int index = 0; index < dataLength; index++)
                        {
                            Int16 data1 = rvInVistor[0].RasterBandsData[0][index];
                            Int16 data2 = rvInVistor[1].RasterBandsData[0][index];
                            if (VaildRegionAndCloudyProcess.isNanValue(data1, nanValues) ||
                                VaildRegionAndCloudyProcess.isNanValue(data1, waterValues))
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = data1;
                                continue;
                            }
                            if (VaildRegionAndCloudyProcess.isNanValue(data2, nanValues) ||
                                VaildRegionAndCloudyProcess.isNanValue(data2, waterValues))
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = data2;
                                continue;
                            }
                            rvOutVistor[0].RasterBandsData[0][index] = (short)(data1 - data2);
                        }
                    }));
                    //执行
                    rfr.Excute(0);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
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
