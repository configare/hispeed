using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    public class SubProductBinaryFOG : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinaryFOG(SubProductDef subProductDef)
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
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "EasyAlgorithm")
            {
                return EasyExtractFOG();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ThresholdAlgorithm")
            {
                return ThresholdExtract();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ThresholdAlgorithm_Mersi")
            {
                return ThresholdExtractMersi();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FY2Algorithm")
            {
                return ThresholdAlgorithm_FY2();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ThresholdAlgorithm_RGB")
            {
                return ThresholdExtractRGB();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ImportAlgorithm")
            {
                return AlgorithmImportAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult AlgorithmImportAlgorithm()
        {
            if (_argumentProvider.GetArg("HistoryFile") == null)
                return null;
            return _argumentProvider.GetArg("HistoryFile") as IExtractResult;
        }

        private IExtractResult ThresholdExtractRGB()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int RedCH = TryGetBandNo(bandNameRaster, "Red");
            double RedZoom = (double)_argumentProvider.GetArg("Red_Zoom");

            if (RedCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            int GreenCH = TryGetBandNo(bandNameRaster, "Green");
            double GreenZoom = (double)_argumentProvider.GetArg("Green_Zoom");

            if (GreenCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            int BlueCH = TryGetBandNo(bandNameRaster, "Blue");
            double BlueZoom = (double)_argumentProvider.GetArg("Blue_Zoom");

            if (BlueCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            string express = string.Format(@"(band{0}/" + RedZoom + @"f  > var_RedMin && band{0}/" + RedZoom + @"f  < var_RedMax)&&
                                            (band{1}/" + GreenZoom + @"f  > var_GreenMin && band{1}/" + GreenZoom + @"f  < var_GreenMax)&&
                                            (band{2}/" + BlueZoom + @"f  > var_BlueMin && band{2}/" + BlueZoom + @"f  < var_BlueMax)", RedCH, GreenCH, BlueCH);
            try
            {
                int[] bandNos = new int[] { RedCH, GreenCH, BlueCH };
                if (_argumentProvider.DataProvider.DataType == enumDataType.Byte)
                {
                    IThresholdExtracter<byte> extracter = new SimpleThresholdExtracter<byte>();//这里如果用uint16时候，系统会出现崩溃
                    extracter.Reset(_argumentProvider, bandNos, express);
                    IRasterDataProvider prd = _argumentProvider.DataProvider;
                    IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FOG", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                    extracter.Extract(result);
                    return result;
                }
                else
                {
                    PrintInfo("非真彩色数据，数据类型不是byte的。");
                    return null;
                }
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
                return null;
            }
        }

        private IExtractResult EasyExtractFOG()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int FarInfraredCH = TryGetBandNo(bandNameRaster, "FarInfrared11");
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");

            if (FarInfraredCH == -1 || VisibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string express = string.Format(@"band{0}/" + VisibleZoom + @"f  > var_VisibleMin && band{0}/" + VisibleZoom + @"f  < var_VisibleMax &&
                                             band{1}/" + FarInfrared11Zoom + @"f  > var_FarInfrared11Min && band{1}/" + FarInfrared11Zoom + @"f  < var_FarInfrared11Max",
                                           VisibleCH, FarInfraredCH);

            int[] bandNos = new int[] { FarInfraredCH, VisibleCH };
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();

            extracter.Reset(_argumentProvider, bandNos, express);
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FOG", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            return result;
        }

        private IExtractResult ThresholdExtract()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int MiddleInfraredCH = TryGetBandNo(bandNameRaster, "MiddleInfrared");
            int ShortInfraredCH = TryGetBandNo(bandNameRaster, "ShortInfrared");
            int FarInfrared10CH = TryGetBandNo(bandNameRaster, "FarInfrared10");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11");
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double ShortInfraredZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            double MiddleInfraredZoom = (double)_argumentProvider.GetArg("MiddleInfrared_Zoom");
            double FarInfrared10Zoom = (double)_argumentProvider.GetArg("FarInfrared10_Zoom");
            if (MiddleInfraredCH == -1 || FarInfrared10CH == -1 || FarInfrared11CH == -1 || VisibleCH == -1 || ShortInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            string express = string.Format(@" band{0}/" + VisibleZoom + @"f > var_VisibleMin && band{0}/" + VisibleZoom + @"f < var_VisibleMax && 
                                              band{1}/" + FarInfrared11Zoom + @"f > var_FarInfrared11Min && band{1}/" + FarInfrared11Zoom + @"f < var_FarInfrared11Max && 
                                              band{2}/" + ShortInfraredZoom + @"f > var_NearInfraredMin && band{2}/" + ShortInfraredZoom + @"f < var_NearInfraredMax && 
                                              NDVI(band{0},band{2}) > var_NDSIMin && NDVI(band{0},band{2}) < var_NDSIMax &&
                                              (band{3}/" + MiddleInfraredZoom + @"f  -  band{4}/" + FarInfrared10Zoom + @"f ) > var_MiddleInfraredFarInfrared10Min && (band{3}/" + MiddleInfraredZoom + @"f  -  band{4}/" + FarInfrared10Zoom + @"f ) < var_MiddleInfraredFarInfrared10Max",
                                              VisibleCH, FarInfrared11CH, ShortInfraredCH, MiddleInfraredCH, FarInfrared10CH);
            int[] bandNos = new int[] { VisibleCH, FarInfrared11CH, ShortInfraredCH, MiddleInfraredCH, FarInfrared10CH };
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FOG", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            return result;
        }

        private IExtractResult ThresholdExtractMersi()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int ShortInfraredCH = TryGetBandNo(bandNameRaster, "ShortInfrared");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11");
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double ShortInfraredZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");

            if (FarInfrared11CH == -1 || VisibleCH == -1 || ShortInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string express = string.Format(@" band{0}/" + VisibleZoom + @"f  > var_VisibleMin && band{0}/" + VisibleZoom + @"f  < var_VisibleMax &&
                                              band{1}/" + FarInfrared11Zoom + @"f  > var_FarInfrared11Min && band{1}/" + FarInfrared11Zoom + @"f  < var_FarInfrared11Max && 
                                              band{2}/" + ShortInfraredZoom + @"f  > var_NearInfraredMin && band{2}/" + ShortInfraredZoom + @"f  < var_NearInfraredMax && 
                                              NDVI(band{0},band{2}) > var_NDSIMin && NDVI(band{0},band{2}) < var_NDSIMax",
                                              VisibleCH, FarInfrared11CH, ShortInfraredCH);

            int[] bandNos = new int[] { VisibleCH, FarInfrared11CH, ShortInfraredCH };
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FOG", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            return result;
        }

        private IExtractResult ThresholdAlgorithm_FY2()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");

            if (VisibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string express = string.Format(@"band{0}/" + VisibleZoom + @"f  > var_VisibleMin && band{0}/" + VisibleZoom + @"f  < var_VisibleMax", VisibleCH);
            try
            {
                int[] bandNos = new int[] { VisibleCH };
                if (_argumentProvider.DataProvider.DataType == enumDataType.Byte)
                {
                    IThresholdExtracter<byte> extracter = new SimpleThresholdExtracter<byte>();//这里如果用uint16时候，系统会出现崩溃
                    extracter.Reset(_argumentProvider, bandNos, express);
                    IRasterDataProvider prd = _argumentProvider.DataProvider;
                    IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FOG", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                    extracter.Extract(result);
                    return result;
                }
                else
                {
                    PrintInfo("非静止星数据，数据类型不是byte的。");
                    return null;
                }
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
                return null;
            }
        }


        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
        /// <summary>
        /// 判识结果保存事件
        /// </summary>
        /// <param name="piexd">判识结果</param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        /// 原始影像和判识结果读取到一个结果集中，分为输入输出两个，输出的就是需要保存的结果
        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            //生成判识结果文件
            IInterestedRaster<UInt16> iir = null;
            RasterIdentify id = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FOG";
            id.SubProductIdentify = _identify;
            id.GenerateDateTime = DateTime.Now;
            iir = new InterestedRaster<UInt16>(id, piexd.Size, piexd.CoordEnvelope);
            int[] idxs = piexd.Indexes.ToArray();
            iir.Put(idxs, 1);
            //原始影像raster
            IRasterDataProvider sourceraster = _argumentProvider.DataProvider as IRasterDataProvider;
            List<RasterMaper> listRaster = new List<RasterMaper>();
            RasterMaper rmsoure = new RasterMaper(sourceraster, GetBandArray(sourceraster.BandCount));
            RasterMaper rmpiexd = new RasterMaper(iir.HostDataProvider, new int[] { 1 });
            int totalbandcount = sourceraster.BandCount;
            listRaster.Add(rmpiexd);
            listRaster.Add(rmsoure);
            try
            {
                string outFileName = GetFileName(new string[] { _argumentProvider.DataProvider.fileName }, _subProductDef.ProductDef.Identify, "SRDA", ".ldf", null);
                using (IRasterDataProvider outRaster = CreateOutM_BandRaster(outFileName, listRaster.ToArray(), totalbandcount))
                {
                    //波段总数

                    RasterMaper[] fileIns = listRaster.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, GetBandArray(totalbandcount)) };
                    //创建处理模型
                    RasterProcessModel<UInt16, UInt16> rfr = null;
                    rfr = new RasterProcessModel<UInt16, UInt16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.SetFeatureAOI(_argumentProvider.AOIs);
                    rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                            return;
                        // if (_argumentProvider.AOIs == null)
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (rvInVistor[0].RasterBandsData[0][index] == 1)
                                for (int i = 0; i < totalbandcount; i++)
                                    rvOutVistor[0].RasterBandsData[i][index] = rvInVistor[1].RasterBandsData[i][index];
                        }
                    }
                    ));
                    //执行
                    rfr.Excute(0);
                }
                string dstfilename = outFileName.Replace(".ldf", ".dat");
                if (File.Exists(dstfilename))
                {
                    File.Delete(dstfilename);
                }
                File.Move(outFileName, dstfilename);
                FileExtractResult res = new FileExtractResult("FOG", dstfilename, true);

                res.SetDispaly(false);
                return res;
            }
            finally
            {
                iir.Dispose();
                if (File.Exists(iir.FileName))
                    File.Delete(iir.FileName);
            }

        }
        private int[] GetBandArray(int bandcount)
        {
            List<int> listband = new List<int>();
            for (int i = 1; i <= bandcount; i++)
            {
                listband.Add(i);
            }
            return listband.ToArray();
        }
        protected IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }
        protected IRasterDataProvider CreateOutM_BandRaster(string outFileName, RasterMaper[] inrasterMaper, int bandcount)
        {


            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            

            CoordEnvelope outEnv = null;

            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            string[] optionString = new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=LDF",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" +inrasterMaper[0].Raster.SpatialRef==null?"":("SPATIALREF=" +inrasterMaper[0].Raster.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + outEnv.MinX + "," + outEnv.MaxY + "}:{" + resX + "," + resY + "}"
                    };
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandcount, enumDataType.UInt16, optionString) as RasterDataProvider;
            return outRaster;
        }

    }
}
