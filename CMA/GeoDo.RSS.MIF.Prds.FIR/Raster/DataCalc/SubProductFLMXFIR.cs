using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductFLMXFIR : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductFLMXFIR()
            : base()
        {
        }

        public SubProductFLMXFIR(SubProductDef subProductDef)
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
            if (algorith != "FLMX")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与最大值合成的数据！");
                return null;
            }
            foreach (string f in fileNames)
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return null;
                }

            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int bandNo = TryGetBandNo(bandNameRaster, "FIFLBandNo");
            if (bandNo == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(fileNames[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo("请选择正确的数据进行最大值合成,请检查文件[" + fileNames[i] + "].");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(fileNames);
                string outFileName = ri.ToWksFullFileName(".dat");
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    Int16[] nanValues = GetNanValues("CloudyValue");
                    Int16[] waterValues = GetNanValues("WaterValue");
                    Int16[] invailValues = GetNanValues("InvailValue");
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int16[] maxData = new Int16[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            maxData[i] = Int16.MinValue;
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            foreach (RasterVirtualVistor<Int16> rvs in rvInVistor)
                            {
                                Int16[] dt = rvs.RasterBandsData[0];
                                if (dt == null)
                                    continue;
                                if (invailValues.Contains(dt[index]))
                                    continue;
                                if (IsMaxValue(dt[index], nanValues, waterValues, maxData[index]))
                                    maxData[index] = dt[index];
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = maxData[index];
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

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] rasterMaper)
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

        private bool IsMaxValue(Int16 pixelValue, Int16[] cloudValues, Int16[] waterValues, Int16 maxValue)
        {
            //若maxData[index]为初始值，可先将云或水的值赋给maxData
            if (pixelValue == Int16.MinValue)
                return false;
            if (maxValue == Int16.MinValue)
                return true;
            if (IsNanValue(maxValue, waterValues))
                return false;
            if (IsNanValue(pixelValue, cloudValues))
                return false;
            else
            {
                if (IsNanValue(pixelValue, waterValues))
                {
                    return true;
                }
                if (IsNanValue(maxValue, cloudValues))
                {
                    return true;
                }
                else
                {
                    if (maxValue < pixelValue)
                        return true;
                    else
                        return false;
                }
            }
        }

        private bool IsNanValue(Int16 pixelValue, Int16[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (Int16 value in nanValues)
                {
                    if (pixelValue == value)
                        return true;
                }
            }
            return false;
        }

        private Int16[] GetNanValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<Int16> values = new List<Int16>();
                    Int16 value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (Int16.TryParse(valueStrings[i], out value))
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
    }
}
