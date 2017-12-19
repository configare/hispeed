using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    /// <summary>
    /// 使用新的栅格计算框架处理时序数据举例
    /// </summary>
    public class SubProductRasterVgtAvgNdvi : CmaVgtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtAvgNdvi()
            : base()
        {
        }

        public SubProductRasterVgtAvgNdvi(SubProductDef subProductDef)
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
            if (algorith != "0AVG")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与平均值合成的数据。");
                return null;
            }
            foreach (string f in fileNames)
            {
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return null;
                }
            }
            int bandNo = 1;
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            double vaildRegionMin = (double)_argumentProvider.GetArg("VaildRegionMin");
            double vaildRegionMax = (double)_argumentProvider.GetArg("VaildRegionMax");
            float resolution = (float)_argumentProvider.GetArg("resolution");
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(fileNames[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo("请选择正确的数据进行平均值合成。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(fileNames);
                string outFileName = ri.ToWksFullFileName(".dat");
                //创建结果数据
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray(),resolution))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    //rfr.SetTemplateAOI(aoiTemplate);
                    short[] nanValues = GetNanValues("CloudyValue");
                    short[] waterValues = GetNanValues("WaterValue");
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        short[] sumData = new short[dataLength];
                        ushort[] countData = new ushort[dataLength];
                        foreach (RasterVirtualVistor<short> rv in rvInVistor)
                        {
                            if (rv.RasterBandsData == null)
                                continue;
                            short[] dt = rv.RasterBandsData[0];
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    if (VaildRegionAndCloudyProcess.isNanValue(dt[index], nanValues) ||
                                        VaildRegionAndCloudyProcess.isNanValue(dt[index], waterValues))
                                    {
                                        sumData[index] = (sumData[index] == 0&&countData[index]==0) ? dt[index] : sumData[index];
                                        continue;
                                    }
                                    else
                                    {
                                        if (dt[index] < vaildRegionMin || dt[index] > vaildRegionMax)
                                            continue;
                                        if (VaildRegionAndCloudyProcess.isNanValue(sumData[index], nanValues) ||
                                            VaildRegionAndCloudyProcess.isNanValue(dt[index], waterValues)&&countData[index]==0)
                                        {
                                            sumData[index] = 0;
                                        }
                                        sumData[index] += dt[index];
                                        countData[index]++;
                                    }
                                }
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (countData[index] != 0)
                                rvOutVistor[0].RasterBandsData[0][index] = (short)(sumData[index] / countData[index]);
                            else
                                rvOutVistor[0].RasterBandsData[0][index] = sumData[index];
                        }
                    }));
                    //执行
                    rfr.Excute(Int16.MinValue);
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
