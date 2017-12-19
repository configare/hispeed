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
    public class SubProductTVDIDRTII : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductTVDIDRTII()
            : base()
        {
        }

        public SubProductTVDIDRTII(SubProductDef subProductDef)
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
            _argumentProvider = _argumentProvider;
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
            TVDIUCArgs args = _argumentProvider.GetArg("ArgumentSetting") as TVDIUCArgs;
            if (args == null)
            {
                PrintInfo("请设置进行温度植被干旱指数计算的参数。");
                return null;
            }
            string ndviFile = args.NDVIFile;
            string lstFile = args.ECLstFile;
            DryWetEdgeArgs dryWetArg = args.DryWetEdgesFitting;
            if (string.IsNullOrEmpty(ndviFile) || string.IsNullOrEmpty(lstFile))
            {
                PrintInfo("请选择进行温度植被干旱指数计算所需的文件。");
                return null;
            }
            if (dryWetArg == null)
            {
                PrintInfo("请先进行干湿边拟合，计算拟合参数。");
                return null;
            }
            double maxA = dryWetArg.MaxA, maxB = dryWetArg.MaxB, minA = dryWetArg.MinA, minB = dryWetArg.MinB;
            if (args.TVDIParas == null || args.TVDIParas.LstFile == null)
                return null;
            int ndviBand = args.TVDIParas.NdviFile.Band;
            if (ndviBand == -1)
                return null;
            int lstZoom = args.TVDIParas.Zoom; //args.TVDIParas.LstFile.Zoom;
            if (lstZoom == 0)
                return null;
            int lstMin = args.TVDIParas.LstFile.Min;
            int lstMax = args.TVDIParas.LstFile.Max;
            int ndviCloudy = args.TVDIParas.NdviFile.Cloudy;
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider ndviPrd = null;
            IRasterDataProvider lstPrd = null;
            try
            {
                ndviPrd = GeoDataDriver.Open(ndviFile) as IRasterDataProvider;
                if (ndviPrd.BandCount < ndviBand)
                {
                    PrintInfo("请选择正确的植被指数文件进行温度植被干旱指数计算。");
                    return null;
                }
                RasterMaper ndviRm = new RasterMaper(ndviPrd, new int[] { ndviBand });
                rms.Add(ndviRm);

                lstPrd = GeoDataDriver.Open(lstFile) as IRasterDataProvider;
                if (lstPrd.BandCount < 1)
                {
                    PrintInfo("请选择正确的陆表高温文件进行温度植被干旱指数计算。");
                    return null;
                }
                RasterMaper lstRm = new RasterMaper(lstPrd, new int[] { 1 });
                rms.Add(lstRm);

                //输出文件准备
                RasterIdentify ri = GetRasterIdentifyID(new string[] { ndviFile, lstFile });
                string outFileName = ri.ToWksFullFileName(".dat");
                //string outFileName = GetFileName(new string[] { ndviFile, lstFile }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    RasterProcessModel<short, short> rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    double tmin = 0d, tmax = 0d;
                    short value0 = 0, value1 = 0;
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                                rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                                return;
                            for (int index = 0; index < dataLength; index++)
                            {
                                value0 = rvInVistor[0].RasterBandsData[0][index];
                                value1 = rvInVistor[1].RasterBandsData[0][index];

                                #region LST
                                if (value1 == 9000)
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)(2 * lstZoom);//非中国区域陆地
                                    continue;
                                }
                                if (value1 == 9999)
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)Math.Round((2.5 * lstZoom), 0);//海区
                                    continue;
                                }
                                if (value1 == 9998)
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)Math.Round((2.55 * lstZoom), 0);//云区
                                    continue;
                                }
                                if (value1 == 9997)
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)Math.Round((1.5 * lstZoom), 0);//无数据区域
                                    continue;
                                }
                                if (value1 < lstMin || value1 > lstMax)
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)Math.Round((1.5 * lstZoom), 0);
                                    continue;
                                }
                                #endregion

                                #region ndvi检查

                                if (value0 == ndviCloudy)                    //过滤无效值
                                {
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)Math.Round((2.55 * lstZoom), 0);
                                    continue;
                                }
                                if (value0 == 0)
                                    continue;
                                #endregion

                                tmin = minA + minB * value0;
                                tmax = maxA + maxB * value0;
                                if (tmin == tmax)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                else
                                    rvOutVistor[0].RasterBandsData[0][index] = (Int16)(((value1 - tmin) / (tmax - tmin)) * lstZoom);
                            }
                        }));
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                if (ndviPrd != null)
                    ndviPrd.Dispose();
                if (lstPrd != null)
                    lstPrd.Dispose();
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
