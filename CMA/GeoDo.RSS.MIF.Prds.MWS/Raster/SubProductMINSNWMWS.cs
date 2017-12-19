using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public class SubProductMINSNWMWS : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductMINSNWMWS()
            : base()
        {

        }

        public SubProductMINSNWMWS(SubProductDef subProductDef)
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
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<ushort, ushort> rfr = null;
                    rfr = new RasterProcessModel<ushort, ushort>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    ushort[] nanValues = GetNanValues("CloudyValue");
                    ushort[] waterValues = GetNanValues("WaterValue");
                    ushort[] invailValues = GetNanValues("InvailValue");
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
                                    if (invailValues.Contains(dt[index]))
                                        continue;
                                    if (IsMinValue(dt[index], nanValues, waterValues, minData[index]))
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

        private bool IsMinValue(ushort pixelValue, ushort[] cloudValues,ushort[] waterValues, ushort minValue)
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

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, inrasterMaper[0].Raster.DataType, mapInfo) as RasterDataProvider;
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

        private bool IsNanValue(ushort pixelValue, ushort[] nanValues)
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
    }
}
