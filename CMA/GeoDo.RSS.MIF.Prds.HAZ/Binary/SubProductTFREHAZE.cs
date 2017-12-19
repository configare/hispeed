using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductTFREHAZE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductTFREHAZE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "TFREAlgorithm")
            {
                return TFREAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult TFREAlgorithm(Action<int, string> progressTracker)
        {
            IRasterOperator<Int16> roper = new RasterOperator<Int16>();
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            //IInterestedRaster<Int16> timeResult = null;
            //RasterIdentify identify = new RasterIdentify(files);
            //identify.SubProductIdentify = _subProductDef.Identify;
            //timeResult = roper.Times(files, identify, progressTracker, (dstValue, srcValue) =>
            //            {
            //                return (Int16)(srcValue == 0 ? dstValue : dstValue++);
            //            });
            //if (timeResult == null)
            //    return null;
            //timeResult.Dispose();

            //string workFilename = identify.ToWksFullFileName(".dat");
            //File.Copy(timeResult.FileName, workFilename, true);
            //return new FileExtractResult("HAZ", workFilename, true);
            int bandNo = 1;
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(files[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo("请选择正确的数据进行最大值合成。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(files);
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
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        for (int index = 0; index < dataLength; index++)
                            foreach (RasterVirtualVistor<Int16> rvs in rvInVistor)
                            {
                                if (rvs.RasterBandsData != null && rvs.RasterBandsData[0] != null &&
                                    rvs.RasterBandsData[0][index] != 0)
                                    rvOutVistor[0].RasterBandsData[0][index]++;
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

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] rasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            float lonMin = float.Parse(_argumentProvider.GetArg("LonMin").ToString());
            float lonMax = float.Parse(_argumentProvider.GetArg("LonMax").ToString());
            float latMin = float.Parse(_argumentProvider.GetArg("LatMin").ToString());
            float latMax = float.Parse(_argumentProvider.GetArg("LatMax").ToString());
            CoordEnvelope outEnv = new CoordEnvelope(lonMin, lonMax, latMin, latMax);
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
    }
}
