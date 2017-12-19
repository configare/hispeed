using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductBinaryHAZ : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinaryHAZ(SubProductDef subProductDef)
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
                    IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("HAZ", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
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
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("HAZ", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
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
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("HAZ", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
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
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("HAZ", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
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
                    IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("HAZ", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
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

    }
}
