using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductVTIDRTII : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductVTIDRTII()
            : base()
        {
        }

        public SubProductVTIDRTII(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NDVILSTAlgorithm")
            {
                return VTIAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult VTIAlgorithm(Action<int, string> progressTracker)
        {
            int NDVIBandCH = (int)_argumentProvider.GetArg("NDVIBand");
            int NDVIBackBandMinCH = (int)_argumentProvider.GetArg("NDVIBackBandMin");
            int NDVIBackBandMaxCH = (int)_argumentProvider.GetArg("NDVIBackBandMax");

            int LSTBandCH = (int)_argumentProvider.GetArg("LSTBand");
            int LSTBackBandMinCH = (int)_argumentProvider.GetArg("LSTBackBandMin");
            int LSTBackBandMaxCH = (int)_argumentProvider.GetArg("LSTBackBandMax");

            float NDVIZoom = (float)_argumentProvider.GetArg("NDVIZoom");
            float NDVIBackZoom = (float)_argumentProvider.GetArg("NDVIBackZoom");

            float LSTZoom = (float)_argumentProvider.GetArg("LSTZoom");
            float LSTBackZoom = (float)_argumentProvider.GetArg("LSTBackZoom");

            float ndviVaildMin = (float)_argumentProvider.GetArg("ndviVaildMin");
            float ndviVaildMax = (float)_argumentProvider.GetArg("ndviVaildMax");

            float lstVaildMin = (float)_argumentProvider.GetArg("lstVaildMin");
            float lstVaildMax = (float)_argumentProvider.GetArg("lstVaildMax");

            double VTIZoom = (float)_argumentProvider.GetArg("VTIZoom");
            bool outVCITCI = (bool)_argumentProvider.GetArg("outVCITCI");
            string vciIdentify = (string)_argumentProvider.GetArg("vciIdentify");
            string tciIdentify = (string)_argumentProvider.GetArg("tciIdentify");
            if (string.IsNullOrWhiteSpace(vciIdentify))
                vciIdentify = "0VCI";
            if (string.IsNullOrWhiteSpace(tciIdentify))
                tciIdentify = "0TCI";

            if (NDVIBandCH == -1 || NDVIBackBandMinCH == -1 || NDVIBackBandMaxCH == -1 || LSTBandCH == -1 || LSTBackBandMinCH == -1 || LSTBackBandMaxCH == -1 ||
                _argumentProvider.GetArg("NDVIFile") == null || _argumentProvider.GetArg("NDVIBackFile") == null ||
                _argumentProvider.GetArg("LSTFile") == null || _argumentProvider.GetArg("LSTBackFile") == null)
            {
                PrintInfo("VTI生产所用文件或通道未设置完全,请检查!");
                return null;
            }
            string[] ndviFileNames = GetStringArray("NDVIFile");
            string[] lstFileNames = GetStringArray("LSTFile");
            string NDVIFile;
            if (ndviFileNames == null || ndviFileNames.Length == 0)
            {
                NDVIFile = _argumentProvider.GetArg("NDVIFile").ToString();
            }
            else if (ndviFileNames.Length == 1)
            {
                NDVIFile = ndviFileNames[0];
            }
            else
            {
                NDVIFile = MAxValue(progressTracker, ndviFileNames, NDVIBandCH, (float)NDVIZoom);
            }
            if (!File.Exists(NDVIFile))
            {
                PrintInfo("NDVI数据不存在");
                return null;
            }
            if (ndviFileNames != null && ndviFileNames.Length != 1)
                NDVIBandCH = 1;
            string NDVIBackFile = _argumentProvider.GetArg("NDVIBackFile").ToString();
            string LSTFile = lstFileNames == null || lstFileNames.Length == 1 ? _argumentProvider.GetArg("LSTFile").ToString() : MAxValue(progressTracker, lstFileNames, LSTBandCH, (float)LSTZoom);
            //if (lstFileNames == null || lstFileNames.Length == 0)
            //{
            //    LSTFile = _argumentProvider.GetArg("LSTFile").ToString();
            //}
            //else if (lstFileNames.Length == 1)
            //{
            //    LSTFile = lstFileNames[0];
            //}
            //else
            //{
            //    LSTFile = MAxValue(progressTracker, lstFileNames, LSTBandCH, (float)LSTZoom);
            //}
            if (!File.Exists(LSTFile))
            {
                PrintInfo("LST数据不存在");
                return null;
            }
            if (lstFileNames != null && lstFileNames.Length != 1)
                LSTBandCH = 1;
            string LSTBackFile = _argumentProvider.GetArg("LSTBackFile").ToString();

            //输入文件准备
            List<RasterMaper> rasterInputMaps = new List<RasterMaper>();

            IRasterDataProvider ndviMaxPrd = null;
            IRasterDataProvider ndviPrd = null;
            IRasterDataProvider lstMaxPrd = null;
            try
            {
                ndviMaxPrd = RasterDataDriver.Open(NDVIFile) as IRasterDataProvider;
                if (ndviMaxPrd.BandCount < NDVIBandCH)
                {
                    PrintInfo("请选择正确的数据进行时序植被温度干旱指数计算。");
                    return null;
                }
                RasterMaper rm = new RasterMaper(ndviMaxPrd, new int[] { NDVIBandCH });
                rasterInputMaps.Add(rm);

                ndviPrd = RasterDataDriver.Open(NDVIBackFile) as IRasterDataProvider;
                if (ndviPrd.BandCount < NDVIBackBandMaxCH || ndviPrd.BandCount < NDVIBackBandMinCH)
                {
                    PrintInfo("请选择正确的数据进行时序植被温度干旱指数计算。");
                    return null;
                }
                RasterMaper rmNdvi = new RasterMaper(ndviPrd, new int[] { NDVIBackBandMinCH, NDVIBackBandMaxCH });
                rasterInputMaps.Add(rmNdvi);

                lstMaxPrd = RasterDataDriver.Open(LSTFile) as IRasterDataProvider;
                if (lstMaxPrd.BandCount < LSTBandCH)
                {
                    PrintInfo("请选择正确的数据进行时序植被温度干旱指数计算。");
                    return null;
                }
                RasterMaper lstRM = new RasterMaper(lstMaxPrd, new int[] { LSTBandCH });
                rasterInputMaps.Add(lstRM);

                IRasterDataProvider lstPrd = RasterDataDriver.Open(LSTBackFile) as IRasterDataProvider;
                if (lstPrd.BandCount < LSTBackBandMaxCH || lstPrd.BandCount < LSTBackBandMinCH)
                {
                    PrintInfo("请选择正确的数据进行时序植被温度干旱指数计算。");
                    return null;
                }
                RasterMaper lstRm = new RasterMaper(lstPrd, new int[] { LSTBackBandMinCH, LSTBackBandMaxCH });
                rasterInputMaps.Add(lstRm);

                //输出文件准备（作为输入栅格并集处理）
                string[] inFileArray = GetInFileList(ndviFileNames,lstFileNames);
                string outFileName = GetFileName(inFileArray, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                string outVci = null, outTci = null;
                if (outVCITCI)
                {
                    outVci = GetFileName(inFileArray, _subProductDef.ProductDef.Identify, vciIdentify, ".dat", null);
                    outTci = GetFileName(inFileArray, _subProductDef.ProductDef.Identify, tciIdentify, ".dat", null);
                }
                IRasterDataProvider outRaster = null;
                IRasterDataProvider outVciRaster = null;
                IRasterDataProvider outTciRaster = null;
                try
                {
                    outRaster = CreateOutRaster(outFileName, rasterInputMaps.ToArray());
                    //栅格数据映射
                    RasterMaper[] fileIns = rasterInputMaps.ToArray();
                    RasterMaper[] fileOuts;
                    if (outVCITCI)
                    {
                        outVciRaster = CreateOutRaster(outVci, rasterInputMaps.ToArray());
                        outTciRaster = CreateOutRaster(outTci, rasterInputMaps.ToArray());
                        fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }), new RasterMaper(outVciRaster, new int[] { 1 }), new RasterMaper(outTciRaster, new int[] { 1 }) };
                    }
                    else
                        fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    float vci = 0f;
                    float tci = 0f;
                    float[] values = null;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        if (rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null
                            || rvInVistor[1].RasterBandsData[1] == null || rvInVistor[2].RasterBandsData[0] == null
                            || rvInVistor[3].RasterBandsData[0] == null || rvInVistor[3].RasterBandsData[1] == null)
                            return;
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int16[] sumData = new Int16[dataLength];
                        ushort[] countData = new ushort[dataLength];
                        for (int index = 0; index < dataLength; index++)
                        {
                            values = new float[6];
                            values[0] = (float)rvInVistor[0].RasterBandsData[0][index] / NDVIZoom;
                            values[1] = (float)rvInVistor[1].RasterBandsData[0][index] / NDVIBackZoom;
                            values[2] = (float)rvInVistor[1].RasterBandsData[1][index] / NDVIBackZoom;
                            values[3] = (float)rvInVistor[2].RasterBandsData[0][index] / LSTZoom;
                            values[4] = (float)rvInVistor[3].RasterBandsData[0][index] / LSTBackZoom;
                            values[5] = (float)rvInVistor[3].RasterBandsData[1][index] / LSTBackZoom;
                            if (values[0] < ndviVaildMin || values[0] > ndviVaildMax ||
                                values[1] < ndviVaildMin || values[1] > ndviVaildMax ||
                                values[2] < ndviVaildMin || values[2] > ndviVaildMax ||
                                values[3] < lstVaildMin || values[3] > lstVaildMax ||
                                values[4] < lstVaildMin || values[4] > lstVaildMax ||
                                values[5] < lstVaildMin || values[5] > lstVaildMax)
                                rvOutVistor[0].RasterBandsData[0][index] = 0;
                            else
                            {
                                //计算VCI
                                vci = (values[0] - values[1]) / (values[2] - values[1]);
                                //计算TCI
                                tci = (values[3] - values[4]) / (values[5] - values[4]);
                                rvOutVistor[0].RasterBandsData[0][index] = (Int16)((0.5 * vci + 0.5 * tci) * VTIZoom);
                                if (outVCITCI)
                                {
                                    rvOutVistor[1].RasterBandsData[0][index] = (Int16)(vci * VTIZoom);
                                    rvOutVistor[2].RasterBandsData[0][index] = (Int16)(tci * VTIZoom);
                                }
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
                    if (outVciRaster != null)
                        outVciRaster.Dispose();
                    if (outTciRaster != null)
                        outTciRaster.Dispose();
                }
                if (outVCITCI)
                {
                    ExtractResultArray resultArray = new ExtractResultArray(_subProductDef.Identify);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    resultArray.Add(res);
                    FileExtractResult resVci = new FileExtractResult(vciIdentify, outVci, true);
                    resVci.SetDispaly(false);
                    resultArray.Add(resVci);
                    FileExtractResult resTci = new FileExtractResult(tciIdentify, outTci, true);
                    resTci.SetDispaly(false);
                    resultArray.Add(resTci);
                    return resultArray;
                }
                else
                {
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                if (ndviMaxPrd != null)
                    ndviMaxPrd.Dispose();
                if (ndviPrd != null)
                    ndviPrd.Dispose();
                if (lstMaxPrd != null)
                    lstMaxPrd.Dispose();
            }
        }

        private string[] GetInFileList(string[] ndviFileNames, string[] lstFileNames)
        {
            List<string> ndvis = ndviFileNames.ToList();
            ndvis.AddRange(lstFileNames);
            return ndvis.ToArray();
        }

        private string MAxValue(Action<int, string> progressTracker, string[] fileNames, int bandNo, float zoom)
        {
            foreach (string f in fileNames)
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
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
                        PrintInfo("请选择正确的数据进行最大值合成。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = new RasterIdentify(fileNames);
                ri.GenerateDateTime = DateTime.Now;
                string outFileName = MifEnvironment.GetFullFileName(ri.ToWksFileName(".dat"));
                IRasterDataProvider outRaster = null;
                try
                {
                    outRaster = CreateOutRaster(outFileName, rms.ToArray());
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int16[] maxData = new Int16[dataLength];
                        foreach (RasterVirtualVistor<Int16> rv in rvInVistor)
                        {
                            if (rv.RasterBandsData == null)
                                continue;
                            Int16[] dt = rv.RasterBandsData[0];
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    if (dt[index] > maxData[index])
                                    {
                                        maxData[index] = dt[index];
                                        rvOutVistor[0].RasterBandsData[0][index] = maxData[index];
                                    }
                                }
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return outFileName;
                }
                finally
                {
                    outRaster.Dispose();
                    outRaster = null;
                }
            }
            finally
            {
                if (rms.Count != 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
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
