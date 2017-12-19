using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.ComponentModel;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class LSTDataProcessor
    {
        public static IExtractResult DataProcessor(Action<int, string> progressTracker, IContextMessage contextMessage, bool add2Workspace,
           string[] fileNames, int bandNo, string subProIdentify, UInt16[] cloudValues, UInt16[] waterValues, string outFileName, enumProcessType type)
        {
            switch (type)
            {
                case enumProcessType.AVG:
                    return AVGProcessor(progressTracker, contextMessage, add2Workspace, fileNames, bandNo, subProIdentify, cloudValues, waterValues, outFileName);
                case enumProcessType.MAX:
                    return MAXProcessor(progressTracker, contextMessage, add2Workspace, fileNames, bandNo, subProIdentify, cloudValues, waterValues, outFileName);
                case enumProcessType.MIN:
                    return MINProcessor(progressTracker, contextMessage, add2Workspace, fileNames, bandNo, subProIdentify, cloudValues, waterValues, outFileName);
                default:
                    return null;
            }
        }

        public static IExtractResult AVGProcessor(Action<int, string> progressTracker, IContextMessage contextMessage, bool add2Workspace,
            string[] fileNames, int bandNo, string subProIdentify, UInt16[] cloudValues, UInt16[] waterValues, string outFileName)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(fileNames[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo(contextMessage, "请选择正确的数据进行平均值合成。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<ushort, ushort> rfr = null;
                    rfr = new RasterProcessModel<ushort, ushort>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<ushort, ushort>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        ushort[] sumData = new ushort[dataLength];
                        ushort[] countData = new ushort[dataLength];
                        if (cloudValues != null && cloudValues.Length > 0)
                        {
                            foreach (RasterVirtualVistor<ushort> rv in rvInVistor)
                            {
                                if (rv.RasterBandsData == null)
                                    continue;
                                ushort[] dt = rv.RasterBandsData[0];
                                if (dt != null)
                                {
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        if (IsNanValue(dt[index], cloudValues) || IsNanValue(dt[index], waterValues))
                                        {
                                            sumData[index] = (sumData[index] == 0 && countData[index] == 0) ? dt[index] : sumData[index];
                                            continue;
                                        }
                                        else
                                        {
                                            if (IsNanValue(sumData[index], cloudValues) ||
                                                IsNanValue(dt[index], waterValues) && countData[index] == 0)
                                            {
                                                sumData[index] = 0;
                                            }
                                            if (dt[index] == 0)
                                                continue;
                                            sumData[index] += dt[index];
                                            countData[index]++;
                                        }
                                    }
                                }
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (countData[index] != 0)
                                rvOutVistor[0].RasterBandsData[0][index] = (ushort)(sumData[index] / countData[index]);
                            else
                                rvOutVistor[0].RasterBandsData[0][index] = sumData[index];
                        }
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult(subProIdentify, outFileName, add2Workspace);
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

        public static IExtractResult MAXProcessor(Action<int, string> progressTracker, IContextMessage contextMessage, bool add2Workspace,
            string[] fileNames, int bandNo, string subProIdentify, UInt16[] cloudValues, UInt16[] waterValues, string outFileName)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(fileNames[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo(contextMessage, "请选择正确的数据进行最大值合成。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<ushort, ushort> rfr = null;
                    rfr = new RasterProcessModel<ushort, ushort>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<ushort, ushort>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        ushort[] maxData = new ushort[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            maxData[i] = UInt16.MinValue;
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            foreach (RasterVirtualVistor<ushort> rvs in rvInVistor)
                            {
                                ushort[] dt = rvs.RasterBandsData[0];
                                if (dt == null)
                                    continue;
                                if (IsMaxValue(dt[index], cloudValues, waterValues, maxData[index]))
                                    maxData[index] = dt[index];
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = maxData[index];
                        }
                    }));
                    //执行
                    rfr.Excute(UInt16.MinValue);
                    FileExtractResult res = new FileExtractResult(subProIdentify, outFileName, add2Workspace);
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

        private static bool IsMaxValue(UInt16 pixelValue, UInt16[] cloudValues, UInt16[] waterValues, UInt16 maxValue)
        {
            //若maxData[index]为初始值，可先将云或水的值赋给maxData
            if (pixelValue == UInt16.MinValue)
                return false;
            if (maxValue == UInt16.MinValue)
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

        public static IExtractResult MINProcessor(Action<int, string> progressTracker, IContextMessage contextMessage, bool add2Workspace,
            string[] fileNames, int bandNo, string subProIdentify, UInt16[] cloudValues, UInt16[] waterValues, string outFileName)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(fileNames[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo(contextMessage, "请选择正确的数据进行最小值合成。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<ushort, ushort> rfr = null;
                    rfr = new RasterProcessModel<ushort, ushort>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<ushort, ushort>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        ushort[] minData = new ushort[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            minData[i] = UInt16.MaxValue;
                        }
                        foreach (RasterVirtualVistor<ushort> rv in rvInVistor)
                        {
                            ushort[] dt = rv.RasterBandsData[0];
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    if (IsMinValue(dt[index], cloudValues, waterValues, minData[index]))
                                        minData[index] = dt[index];
                                }
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = minData[index];
                        }
                    }));
                    //执行
                    rfr.Excute(UInt16.MaxValue);
                    FileExtractResult res = new FileExtractResult(subProIdentify, outFileName, add2Workspace);
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

        private static bool IsMinValue(ushort pixelValue, ushort[] cloudValues, ushort[] waterValues, ushort minValue)
        {
            if (pixelValue == UInt16.MaxValue)
                return false;
            if (minValue == UInt16.MaxValue)
                return true;
            if (IsNanValue(minValue, waterValues))
                return false;
            if (IsNanValue(pixelValue, cloudValues))
                return false;
            if (IsNanValue(pixelValue, waterValues))
                return true;
            if (IsNanValue(minValue, cloudValues))
                return true;
            else
            {
                if (minValue > pixelValue)
                    return true;
                else
                    return false;
            }
        }

        private static void PrintInfo(IContextMessage contextMessage, string info)
        {
            if (contextMessage != null)
                contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private static IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, inrasterMaper[0].Raster.DataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private static bool IsNanValue(ushort pixelValue, ushort[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (ushort value in nanValues)
                {
                    if (pixelValue == value)
                        return true;
                }
            }
            return false;
        }
    }

    public enum enumProcessType : int
    {
        [Description("平均值合成")]
        AVG = 0,
        [Description("最大值合成")]
        MAX = 1,
        [Description("最小值合成")]
        MIN = 2
    }
}
