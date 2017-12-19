using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtDifference : CmaVgtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtDifference()
            : base()
        {

        }

        public SubProductRasterVgtDifference(SubProductDef subProductDef)
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
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "CHAZ")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            return CalcDiff(progressTracker);
        }

        private IExtractResult CalcDiff(Action<int, string> progressTracker)
        {
            if (_argumentProvider.GetArg("mainfiles") == null)
            {
                PrintInfo("请选择植被指数(被减数)数据。");
                return null;
            }
            string bjianshu = _argumentProvider.GetArg("mainfiles").ToString();
            if (!File.Exists(bjianshu))
            {
                PrintInfo("所选择的数据:\"" + bjianshu + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("jianshu") == null)
            {
                PrintInfo("请选择植被指数(减数)数据。");
                return null;
            }
            string jianshu = _argumentProvider.GetArg("jianshu").ToString();
            if (!File.Exists(jianshu))
            {
                PrintInfo("所选择的数据:\"" + jianshu + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviCH1") == null)
            {
                PrintInfo("参数\"NdviCH1\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviCH2") == null)
            {
                PrintInfo("参数\"NdviCH2\"为空。");
                return null;
            }

            int ch1 = (int)_argumentProvider.GetArg("NdviCH1");
            int ch2 = (int)_argumentProvider.GetArg("NdviCH2");
            if (ch1 < 0 || ch2 < 0)
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
                IRasterDataProvider inRaster = RasterDataDriver.Open(bjianshu) as IRasterDataProvider;
                if (inRaster.BandCount < ch1)
                {
                    PrintInfo("请选择正确的数据进行差值计算。");
                    return null;
                }
                RasterMaper brm = new RasterMaper(inRaster, new int[] { ch1 });
                rms.Add(brm);
                IRasterDataProvider iRaster = RasterDataDriver.Open(jianshu) as IRasterDataProvider;
                if (iRaster.BandCount < ch2)
                {
                    PrintInfo("请选择正确的数据进行差值计算。");
                    return null;
                }
                RasterMaper rm = new RasterMaper(iRaster, new int[] { ch2 });
                rms.Add(rm);
                string[] fileNames = new string[] { bjianshu, jianshu };
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
                        if (rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                        {
                            for (int index = 0; index < dataLength; index++)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = Int16.MinValue;
                            }
                        }
                        else
                        {
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
                        }
                    }));
                    //执行
                    rfr.Excute();
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
