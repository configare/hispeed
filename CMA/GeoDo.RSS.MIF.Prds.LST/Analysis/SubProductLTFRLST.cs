using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class SubProductLTFRLST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductLTFRLST(SubProductDef subProductDef)
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
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "LTFREAlgorithm")
            {
                return TFREAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult TFREAlgorithm(Action<int, string> progressTracker)
        {
            float tempratureMin = (float)_argumentProvider.GetArg("TempratureMin");
            double lftrZoom = (double)_argumentProvider.GetArg("LTFRZoom");
            Int16 minLst = (Int16)((tempratureMin + 273) * lftrZoom);
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与高温天数统计的数据！");
                return null;
            }
            foreach (string f in fileNames)
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return null;
                }
            int bandNo = 1;
            //输出文件准备（作为输入栅格并集处理）
            RasterIdentify ri = GetRasterIdentifyID(fileNames);
            string outFileName = ri.ToWksFullFileName(".dat");
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(fileNames[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo("请选择正确的数据进行高温天数统计。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
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
                    Int16 currTimeValue = 0;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int16[] timeValue = new Int16[dataLength];
                        for (int i = 0; i < dataLength; i++)
                            timeValue[i] = 0;
                        for (int index = 0; index < dataLength; index++)
                        {
                            foreach (RasterVirtualVistor<Int16> rvs in rvInVistor)
                            {
                                Int16[] dt = rvs.RasterBandsData[0];
                                if (dt == null)
                                    continue;
                                if (invailValues.Contains(dt[index]))
                                    continue;
                                if (TimeValue(dt[index], nanValues, waterValues, invailValues, timeValue[index], minLst, out currTimeValue))
                                    timeValue[index] = currTimeValue;
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = timeValue[index];
                        }
                    }));
                    //执行
                    rfr.Excute(0);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }

            _argumentProvider.SetArg("SelectedPrimaryFiles", new string[] { outFileName });
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            return ThemeGraphyByInstance(instance);

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

        private bool TimeValue(Int16 pixelValue, Int16[] cloudValues, Int16[] waterValues, Int16[] invaildValues, Int16 timeValue, Int16 minLst, out Int16 currTimeValue)
        {
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            Int16 defWater = (Int16)_argumentProvider.GetArg("defWater");
            currTimeValue = 0;
            if (timeValue == 0)
            {
                if (IsNanValue(pixelValue, cloudValues))
                {
                    currTimeValue = defCloudy;
                    return true;
                }
                else if (IsNanValue(pixelValue, waterValues))
                {
                    currTimeValue = defWater;
                    return true;
                }
                if (IsNanValue(pixelValue, invaildValues))
                    return false;
            }
            if (IsNanValue(pixelValue, cloudValues) || IsNanValue(pixelValue, waterValues) || IsNanValue(pixelValue, invaildValues))
                return false;
            if (IsNanValue(timeValue, cloudValues) || IsNanValue(timeValue, waterValues))
                timeValue = 0;
            if (pixelValue >= minLst)
            {
                currTimeValue = ++timeValue;
                return true;
            }
            return false;
        }

        private bool IsNanValue(short pixelValue, short[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (short value in nanValues)
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
                    List<short> values = new List<short>();
                    short value;
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
