using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductSWIDRTII : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductSWIDRTII()
            : base()
        {
        }

        public SubProductSWIDRTII(SubProductDef subProductDef)
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
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "DemAlgorithm")
            {
                return DemAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult DemAlgorithm(Action<int, string> progressTracker)
        {
            int DNTBandCH = (int)_argumentProvider.GetArg("DNTBand");
            float DNTZoom = float.Parse(_argumentProvider.GetArg("DNTZoom").ToString());
            float DNTVaildMin = float.Parse(_argumentProvider.GetArg("DNTVaildMin").ToString());
            float DNTVaildMax = float.Parse(_argumentProvider.GetArg("DNTVaildMax").ToString());

            Int16 DemMin = (Int16)_argumentProvider.GetArg("DemMin");
            float DemCorrect = (float)_argumentProvider.GetArg("DemCorrect");

            double SWIZoom = (float)_argumentProvider.GetArg("SWIZoom");
            DRTExpCoefficientCollection ExpCoefficient = _argumentProvider.GetArg("ExpCoefficient") as DRTExpCoefficientCollection;

            if (DNTBandCH == -1 || _argumentProvider.GetArg("DNTFile") == null || _argumentProvider.GetArg("DemFile") == null || ExpCoefficient == null)
            {
                PrintInfo("热惯量干旱指数生产所用文件或通道未设置完全,请检查!");
                return null;
            }
            string DNTFile = _argumentProvider.GetArg("DNTFile").ToString();
            string DemFile = _argumentProvider.GetArg("DemFile").ToString();
            if (string.IsNullOrEmpty(ExpCoefficient.EgdesFilename) || !File.Exists(ExpCoefficient.EgdesFilename))
            {
                string fileanme = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\RasterTemplate\\China_Province.dat";
                if (File.Exists(fileanme))
                    ExpCoefficient.EgdesFilename = fileanme;
                else
                {
                    PrintInfo("热惯量干旱指数生产所用的经验系数边界文件不存在,请检查!");
                    return null;
                }
            }
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            string EdgesFile = ExpCoefficient.EgdesFilename;
            int bandNo = 1;
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider dntRaster = null;
            IRasterDataProvider demRaster = null;
            IRasterDataProvider edgeRaster = null;
            try
            {
                dntRaster = RasterDataDriver.Open(DNTFile) as IRasterDataProvider;
                if (dntRaster.BandCount < DNTBandCH)
                {
                    PrintInfo("请选择正确的数据进行热惯量干旱指数计算。");
                    return null;
                }
                RasterMaper rmDnt = new RasterMaper(dntRaster, new int[] { DNTBandCH });
                rms.Add(rmDnt);
                demRaster = RasterDataDriver.Open(DemFile) as IRasterDataProvider;
                if (demRaster.BandCount < bandNo)
                {
                    PrintInfo("请选择正确的数据进行热惯量干旱指数计算。");
                    return null;
                }
                RasterMaper rmDem = new RasterMaper(demRaster, new int[] { bandNo });
                rms.Add(rmDem);
                edgeRaster = RasterDataDriver.Open(EdgesFile) as IRasterDataProvider;
                if (edgeRaster.BandCount < bandNo)
                {
                    PrintInfo("请选择正确的数据进行热惯量干旱指数计算。");
                    return null;
                }
                RasterMaper rmEdge = new RasterMaper(edgeRaster, new int[] { bandNo });
                rms.Add(rmEdge);

                //输出文件准备（作为输入栅格并集处理）
                string outFileName = GetFileName(new string[] { DNTFile }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    DRTExpCoefficientItem item = null;
                    double swiTemp = 0f;
                    float value = 0f;
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null || rvInVistor[2].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null || rvInVistor[2].RasterBandsData[0] == null)
                            return;
                        for (int index = 0; index < dataLength; index++)
                        {
                            short dataValue = rvInVistor[0].RasterBandsData[0][index];
                            if (dataValue == 0 || dataValue == defCloudy)
                                rvOutVistor[0].RasterBandsData[0][index] = dataValue;
                            else
                            {
                                value = (float)rvInVistor[0].RasterBandsData[0][index] / DNTZoom;
                                if (value < DNTVaildMin || value > DNTVaildMax)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                else
                                {
                                    item = ExpCoefficient.GetExpItemByNum((int)rvInVistor[2].RasterBandsData[0][index]);
                                    if (item == null)
                                        rvOutVistor[0].RasterBandsData[0][index] = 0;
                                    else
                                    {
                                        //计算SWI
                                        swiTemp = item.APara * value + item.BPara + item.CPara;
                                        if (rvInVistor[1].RasterBandsData[0][index] > DemMin)
                                            swiTemp += DemCorrect;
                                        rvOutVistor[0].RasterBandsData[0][index] = (Int16)(swiTemp * SWIZoom);
                                    }
                                }
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
                if (dntRaster != null)
                    dntRaster.Dispose();
                if (demRaster != null)
                    demRaster.Dispose();
                if (edgeRaster != null)
                    edgeRaster.Dispose();
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
