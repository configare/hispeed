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
    /// <summary>
    /// 设计仅用于自动产品TCI生成
    /// Add by ca
    /// </summary>
    public class SubProductTCI : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductTCI()
            : base()
        {
        }

        public SubProductTCI(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "LSTAlgorithm")
            {
                return TCIAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult TCIAlgorithm(Action<int, string> progressTracker)
        {

            int LSTBandCH = (int)_argumentProvider.GetArg("LSTBand");
            int LSTBackBandMinCH = (int)_argumentProvider.GetArg("LSTBackBandMin");
            int LSTBackBandMaxCH = (int)_argumentProvider.GetArg("LSTBackBandMax");
            float LSTZoom = (float)_argumentProvider.GetArg("LSTZoom");
            float LSTBackZoom = (float)_argumentProvider.GetArg("LSTBackZoom");
            float lstVaildMin = (float)_argumentProvider.GetArg("lstVaildMin");
            float lstVaildMax = (float)_argumentProvider.GetArg("lstVaildMax");

            double VTIZoom = (float)_argumentProvider.GetArg("VTIZoom");
            bool outVCITCI = true;
            string tciIdentify = (string)_argumentProvider.GetArg("tciIdentify");

            if (string.IsNullOrWhiteSpace(tciIdentify))
                tciIdentify = "0TCIA";

            if (LSTBandCH == -1 || LSTBackBandMinCH == -1 || LSTBackBandMaxCH == -1 || _argumentProvider.GetArg("LSTFile") == null || _argumentProvider.GetArg("LSTBackFile") == null)
            {
                PrintInfo("VTI生产所用文件或通道未设置完全,请检查!");
                return null;
            }
            //假如是一个元素的数组 此处下一行代码将报错
            string[] lstFileNames = GetStringArray("LSTFile");
            if (lstFileNames == null || lstFileNames.Length == 0)
            {
                return null;
            }
            string LSTFile =lstFileNames.Length == 1 ? lstFileNames[0] : MAxValue(progressTracker, lstFileNames, LSTBandCH, (float)LSTZoom);
            //此处原代码if判断与上面三目运算重复
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
            IRasterDataProvider lstPrd = null;
            try
            {
                lstMaxPrd = RasterDataDriver.Open(LSTFile) as IRasterDataProvider;
                if (lstMaxPrd.BandCount < LSTBandCH)
                {
                    PrintInfo("请选择正确的数据进行时序植被温度干旱指数计算。");
                    return null;
                }
                RasterMaper lstRM = new RasterMaper(lstMaxPrd, new int[] { LSTBandCH });
                rasterInputMaps.Add(lstRM);

                 lstPrd = RasterDataDriver.Open(LSTBackFile) as IRasterDataProvider;
                if (lstPrd.BandCount < LSTBackBandMaxCH || lstPrd.BandCount < LSTBackBandMinCH)
                {
                    PrintInfo("请选择正确的数据进行时序植被温度干旱指数计算。");
                    return null;
                }
                RasterMaper lstRm = new RasterMaper(lstPrd, new int[] { LSTBackBandMinCH, LSTBackBandMaxCH });
                rasterInputMaps.Add(lstRm);

                //输出文件准备（作为输入栅格并集处理）
                //string[] inFileArray = GetInFileList(ndviFileNames,lstFileNames);
                string[] inFileArray = lstFileNames;
                string outTci = null;
                if (outVCITCI)
                {
                    outTci = GetFileName(inFileArray, _subProductDef.ProductDef.Identify, tciIdentify, ".dat", null);
                }

                IRasterDataProvider outTciRaster = null;
                try
                {

                    //栅格数据映射
                    RasterMaper[] fileIns = rasterInputMaps.ToArray();
                    RasterMaper[] fileOuts = null;
                    if (outVCITCI)
                    {
                        outTciRaster = CreateOutRaster(outTci, rasterInputMaps.ToArray());
                        fileOuts = new RasterMaper[] { new RasterMaper(outTciRaster, new int[] { 1 }) };
                    }


                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    float tci = 0f;
                    float[] values = null;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        //判断输入的lst及背景库文件01波段数据是否为空
                        if (rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null
                            || rvInVistor[1].RasterBandsData[1] == null)
                            return;
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;//tci
                        Int16[] sumData = new Int16[dataLength];
                        ushort[] countData = new ushort[dataLength];
                        for (int index = 0; index < dataLength; index++)
                        {
                            values = new float[3];

                            values[0] = (float)rvInVistor[0].RasterBandsData[0][index] / LSTZoom;
                            values[1] = (float)rvInVistor[1].RasterBandsData[0][index] / LSTBackZoom;
                            values[2] = (float)rvInVistor[1].RasterBandsData[1][index] / LSTBackZoom;
                            //lst以及背景库数据值如果不在有效范围内
                            if (values[0] < lstVaildMin || values[0] > lstVaildMax ||
                                values[1] < lstVaildMin || values[1] > lstVaildMax ||
                                values[2] < lstVaildMin || values[2] > lstVaildMax)
                                rvOutVistor[0].RasterBandsData[0][index] = 0;
                            else
                            {

                                //计算TCI
                                tci = (values[0] - values[1]) / (values[2] - values[1]);

                                if (outVCITCI)
                                {
                                    //这个就是输出TCI结果可以简化代码
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)(tci * VTIZoom);
                                }
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                }
                finally
                {

                    if (outTciRaster != null)
                        outTciRaster.Dispose();
                }
                //这里保持跟其他自动产品一样 在外面进行输出路径文件移动
                FileExtractResult resTci = new FileExtractResult(tciIdentify, outTci, true);
                resTci.SetDispaly(false);
                return resTci;
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



        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
