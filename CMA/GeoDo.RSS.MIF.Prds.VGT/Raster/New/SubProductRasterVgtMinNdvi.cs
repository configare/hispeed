using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    //最小值合成
    public class SubProductRasterVgtMinNdvi : CmaVgtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtMinNdvi()
            : base()
        {

        }

        public SubProductRasterVgtMinNdvi(SubProductDef subProductDef)
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
                return null;
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "0MIN")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }

            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与最小值合成的数据！");
                return null;
            }
            foreach (string f in fileNames)
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return null;
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
                        PrintInfo("请选择正确的数据进行最小值合成。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(fileNames);
                string outFileName = ri.ToWksFullFileName(".dat");
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray(), resolution))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    Int16 result;
                    short[] nanValues = GetNanValues("CloudyValue");
                    short[] waterValues = GetNanValues("WaterValue");
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        short[] minData = new short[dataLength];
                        InitMinData(ref minData);
                        foreach (RasterVirtualVistor<short> rv in rvInVistor)
                        {
                            short[] dt = rv.RasterBandsData[0];
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    if (VaildRegionAndCloudyProcess.ProcessMinNanValues(new Int16[] { dt[index], minData[index] }, nanValues, waterValues, vaildRegionMax, vaildRegionMin, defCloudy,out result))
                                    {
                                        minData[index] = result;
                                    }
                                }
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = minData[index];
                        }
                    }));
                    //执行
                    rfr.Excute(Int16.MaxValue);
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

        private void InitMinData(ref short[] maxData)
        {
            int length = maxData.Length;
            for (int i = 0; i < length; i++)
            {
                maxData[i] = Int16.MaxValue;
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
