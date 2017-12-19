using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    /// <summary>
    /// 晴空反射率
    /// </summary>
    public class SubProductCSRFOG : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductCSRFOG(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "BaseProAlgorithm")
            {
                return BaseOrbitAlgorithm(progressTracker);
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "BaseOrbitAlgorithm")
            {
            }
            return null;
        }

        private IExtractResult BaseOrbitAlgorithm(Action<int, string> progressTracker)
        {
            int CSRVisibleCH = (int)_argumentProvider.GetArg("VisibleCH");
            int maxNDVICH = (int)_argumentProvider.GetArg("NDVICH");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double ShortInfraredZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            double VisibleCHZoom = (double)_argumentProvider.GetArg("VisibleCH_Zoom");
            double NDVICHZoom = (double)_argumentProvider.GetArg("NDVICH_Zoom");
            double ndviZoom = (float)_argumentProvider.GetArg("NDVIZoom");
            if (_argumentProvider.GetArg("HistroySRCFile") == null || _argumentProvider.GetArg("CurrentRasterFile") == null)
                return null;
            Dictionary<string, string> csrStr = GetFilemap("HistroyCSRFile");
            string CurrentRasterFile = _argumentProvider.GetArg("CurrentRasterFile").ToString();
            int visibleCH = (int)_argumentProvider.GetArg("Visible");
            int shortInfrared = (int)_argumentProvider.GetArg("ShortInfrared");
            if (CSRVisibleCH == -1 || maxNDVICH == -1 || csrStr == null || csrStr.Count < 2 || !File.Exists(CurrentRasterFile))
            {
                PrintInfo("晴空反射率生产所用文件或通道未设置完全,请检查!");
                return null;
            }

            if (visibleCH == -1 || shortInfrared == -1)
            {
                using (IRasterDataProvider rasterProvider = RasterDataDriver.Open(CurrentRasterFile) as IRasterDataProvider)
                {
                    if (rasterProvider != null)
                    {
                        IBandNameRaster bandNameRaster = rasterProvider as IBandNameRaster;
                        visibleCH = TryGetBandNo(bandNameRaster, "Visible");
                        shortInfrared = TryGetBandNo(bandNameRaster, "ShortInfrared");
                    }
                }
                if (visibleCH == -1 || shortInfrared == -1)
                {
                    PrintInfo("晴空反射率生产所用文件或通道未设置完全,请检查!");
                    return null;
                }
            }

            //是否为自动生成时生成该产品，若是，需要查找"0CSR"与"MAXN"文件
            if (!ResetCsrString(csrStr))
            {
                PrintInfo("晴空反射率生产所用文件不齐备,请检查!");
                return null;
            }

            //查找输入文件对应的云判识结果文件
            RasterProcessModel<short, UInt16> rfr = null;
            List<RasterMaper> fileIns = new List<RasterMaper>();
            RasterMaper[] fileOuts = null;
            IRasterDataProvider skyPrd = null;
            IRasterDataProvider maxNdviPrd = null;
            try
            {
                //输入数据(LDF)
                IRasterDataProvider inRaster = _argumentProvider.DataProvider;
                //输出数据(0CSR)
                string outFileName = GetFileName(new string[] { inRaster.fileName }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                IRasterDataDriver dd = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                string mapInfo = inRaster.CoordEnvelope.ToMapInfoString(new Size(inRaster.Width, inRaster.Height));
                string[] opts = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=MEM",
                "WITHHDR=TRUE",
                "SPATIALREF=" + inRaster.SpatialRef.ToProj4String(),
                mapInfo};
                RasterDataProvider outRaster = dd.Create(outFileName, inRaster.Width, inRaster.Height, 1, enumDataType.UInt16, opts) as RasterDataProvider;
                outRaster.GetRasterBand(1).Fill(Int16.MinValue);
                //栅格数据映射
                fileIns.Add(new RasterMaper(inRaster, new int[] { visibleCH, shortInfrared }));
                skyPrd = GeoDataDriver.Open(csrStr["0CSR"]) as IRasterDataProvider;
                if (skyPrd.BandCount < CSRVisibleCH)
                {
                    PrintInfo("请选择正确的云数据通道进行计算.");
                    return null;
                }
                fileIns.Add(new RasterMaper(skyPrd, new int[] { CSRVisibleCH }));
                maxNdviPrd = GeoDataDriver.Open(csrStr["MAXN"]) as IRasterDataProvider;
                if (maxNdviPrd.BandCount < maxNDVICH)
                {
                    PrintInfo("请选择正确的云数据通道进行计算.");
                    return null;
                }
                fileIns.Add(new RasterMaper(maxNdviPrd, new int[] { maxNDVICH }));

                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                //创建处理模型
                fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<short, UInt16>(progressTracker);
                rfr.SetRaster(fileIns.ToArray(), fileOuts);
                float curNDVI = 0f;
                rfr.RegisterCalcModel(new RasterCalcHandler<short, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData != null)
                    {
                        if (rvInVistor == null)
                            return;
                        short[] inBand0 = rvInVistor[0].RasterBandsData[0];//第1个输入文件的第1个波段的各像素值
                        short[] inBand1 = rvInVistor[0].RasterBandsData[1];//第1个输入文件的第2个波段的各像素值
                        short[] inBand21 = rvInVistor[1].RasterBandsData[0];//第2个输入文件的第1个波段的各像素值
                        short[] inBand31 = rvInVistor[2].RasterBandsData[0];//第3个输入文件的第1个波段的各像素值
                        for (int index = 0; index < inBand0.Length; index++)
                        {
                            curNDVI = GetNDVI(inBand0[index], inBand1[index]);
                            if (curNDVI * NDVICHZoom > inBand31[index])
                                rvOutVistor[0].RasterBandsData[0][index] = (UInt16)inBand1[index];
                            else
                                rvOutVistor[0].RasterBandsData[0][index] = (UInt16)inBand21[index];
                        }
                    }
                }));
                //执行
                rfr.Excute();
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (fileOuts != null)
                    for (int i = 0; i < fileOuts.Length; i++)
                        fileOuts[i].Raster.Dispose();
                if (skyPrd != null)
                    skyPrd.Dispose();
                if (maxNdviPrd != null)
                    maxNdviPrd.Dispose();
            }
            //

            //Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            //filePrdMap.Add("CurrentRasterFile", new FilePrdMap(CurrentRasterFile, VisibleZoom, new VaildPra(UInt16.MinValue, UInt16.MaxValue), new int[] { visibleCH, shortInfrared }));
            //filePrdMap.Add("csrFile", new FilePrdMap(csrStr["0CSR"], VisibleCHZoom, new VaildPra(UInt16.MinValue, UInt16.MaxValue), new int[] { CSRVisibleCH }));
            //filePrdMap.Add("maxNDVIFile", new FilePrdMap(csrStr["MAXN"], NDVICHZoom, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { maxNDVICH }));
            //ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            //IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            //if (vrd == null)
            //{
            //    PrintInfo("当前数据与历史晴空数据不在同一区域内，请重新设置!");
            //    foreach (FilePrdMap value in filePrdMap.Values)
            //    {
            //        if (value.Prd != null)
            //            value.Prd.Dispose();
            //    }
            //    return null;
            //}
            //try
            //{
            //    float curNDVI = 0f;
            //    ArgumentProvider ap = new ArgumentProvider(vrd, null);
            //    RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
            //    IPixelFeatureMapper<UInt16> _curCSR = new MemPixelFeatureMapper<UInt16>("0CSR", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
            //    IPixelFeatureMapper<Int16> _maxNDVI = new MemPixelFeatureMapper<Int16>("MAXN", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
            //    rpVisitor.VisitPixel(new int[] { filePrdMap["CurrentRasterFile"].StartBand,
            //                                     filePrdMap["CurrentRasterFile"].StartBand+1,
            //                                     filePrdMap["csrFile"].StartBand,
            //                                     filePrdMap["maxNDVIFile"].StartBand},
            //        (index, values) =>
            //        {
            //            curNDVI = GetNDVI(values, 1, 0);
            //            if (curNDVI > values[3])
            //            {
            //                _curCSR.Put(index, (UInt16)(values[0] * VisibleZoom));
            //                _maxNDVI.Put(index, (Int16)(curNDVI * ndviZoom));
            //            }
            //            else
            //            {
            //                _curCSR.Put(index, (UInt16)(values[2] * VisibleZoom));
            //                _maxNDVI.Put(index, (Int16)(values[3] * ndviZoom));
            //            }
            //        });
            //    IExtractResultArray extractArray = new ExtractResultArray("0CRS");
            //    extractArray.Add(_curCSR);
            //    extractArray.Add(_maxNDVI);
            //    return extractArray;
            //}
            //finally
            //{
            //    vrd.Dispose();
            //    if (filePrdMap != null && filePrdMap.Count() > 0)
            //    {
            //        foreach (FilePrdMap value in filePrdMap.Values)
            //        {
            //            if (value.Prd != null)
            //                value.Prd.Dispose();
            //        }
            //    }
            //}
        }

        private float GetNDVI(short visibleValue, short shortValue)
        {
            return visibleValue + shortValue == 0 ? 0 : ((float)(shortValue - visibleValue) / (shortValue + visibleValue));
        }

        private bool ResetCsrString(Dictionary<string, string> csrStr)
        {
            string csrFile = null;
            string filename = _argumentProvider.GetArg("CurrentRasterFile") as string;
            if (string.IsNullOrEmpty(csrStr["0CSR"]) || !File.Exists(csrStr["0CSR"]))
            {
                csrFile = GetFileByDirArg.GetFileBySattileSensor(_argumentProvider, "HistroyCSRDir", "DefHistroyCSRDir", filename);
                csrStr["0CSR"] = csrFile;
            }
            if (string.IsNullOrEmpty(csrStr["MAXN"]) || !File.Exists(csrStr["0CSR"]))
            {
                if (csrFile != null)
                {
                    string maxNDVI = csrFile.Replace("_0CSR_", "_MAXN_");
                    if (File.Exists(maxNDVI))
                        csrStr["MAXN"] = maxNDVI;
                }
            }
            return CheckFileExist(csrStr, "0CSR") && CheckFileExist(csrStr, "MAXN");
        }

        private bool CheckFileExist(Dictionary<string, string> csrStr, string fileFlag)
        {
            if (string.IsNullOrEmpty(csrStr[fileFlag]) || !File.Exists(csrStr[fileFlag]))
                return false;
            return true;
        }

        private Dictionary<string, string> GetFilemap(string argument)
        {
            object obj = _argumentProvider.GetArg(argument);
            if (obj == null)
                return null;
            return obj as Dictionary<string, string>;
        }

        private RasterIdentify CreatRasterIndetifyId(string filename)
        {
            RasterIdentify id = new RasterIdentify(filename);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FOG";
            id.SubProductIdentify = "0CSR";
            id.GenerateDateTime = DateTime.Now;
            return id;
        }

        private float GetNDVI(float[] values, int shortInfraredCH, int visibleCH)
        {
            return (values[shortInfraredCH] + values[visibleCH]) == 0f ? 0f : (float)(values[shortInfraredCH] - values[visibleCH]) / (values[shortInfraredCH] + values[visibleCH]);
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
