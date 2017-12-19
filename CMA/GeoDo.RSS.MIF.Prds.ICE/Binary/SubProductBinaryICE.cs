using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductBinaryICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinaryICE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NDSIAlgorithm")
            {
                return NDSIExtractICE();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NDSIAlgorithm_NOAA")
            {
                return NDSIExtractICE_NOAA();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ISTAlgorithm")
            {
                return null;
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult NDSIExtractICE()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int NDSIVisibleCH = TryGetBandNo(bandNameRaster, "NDSIVisible");
            int nearInfraredCH = TryGetBandNo(bandNameRaster, "NearInfrared");
            int FarInfraredCH = TryGetBandNo(bandNameRaster, "FarInfrared");
            int NDSIShortInfraredCH = TryGetBandNo(bandNameRaster, "NDSIShortInfrared");
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double FarInfraredZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");

            if (NDSIVisibleCH == -1 || nearInfraredCH == -1 || FarInfraredCH == -1 || NDSIShortInfraredCH == -1 || VisibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string express = string.Format(@"NDVI(band{0},band{1})  > var_NDSIMin &&
                                             NDVI(band{0},band{1})  < var_NDSIMax &&
                                             band{2}/" + NearInfraredZoom + @"f > var_NearInfraredMin && band{2}/" + NearInfraredZoom + @"f < var_NearInfraredMax &&
                                             band{3}/" + VisibleZoom + @"f  > var_VisibleMin && band{3}/" + VisibleZoom + @"f  < var_VisibleMax &&
                                             band{4}/" + FarInfraredZoom + @"f  > var_FarInfraredMin && band{4}/" + FarInfraredZoom + @"f  < var_FarInfraredMax",
                                             NDSIVisibleCH, NDSIShortInfraredCH, nearInfraredCH, VisibleCH, FarInfraredCH);

            int[] bandNos = new int[] { NDSIVisibleCH, nearInfraredCH, FarInfraredCH, NDSIShortInfraredCH, VisibleCH };
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("SEAICE", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            return result;
        }

        private IExtractResult NDSIExtractICE_NOAA()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int nearInfraredCH = TryGetBandNo(bandNameRaster, "NearInfrared");
            int FarInfraredCH = TryGetBandNo(bandNameRaster, "FarInfrared");
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double FarInfraredZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");

            if (nearInfraredCH == -1 || FarInfraredCH == -1 || VisibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string express = string.Format(@"band{0}/" + NearInfraredZoom + @"f  > var_NearInfraredMin && band{0}/" + NearInfraredZoom + @"f  < var_NearInfraredMax &&
                                             band{1}/" + VisibleZoom + @"f  > var_VisibleMin && band{1}/" + VisibleZoom + @"f  < var_VisibleMax &&
                                             band{2}/" + FarInfraredZoom + @"f  > var_FarInfraredMin && band{2}/" + FarInfraredZoom + @"f  < var_FarInfraredMax",
                                             nearInfraredCH, VisibleCH, FarInfraredCH);

            int[] bandNos = new int[] { nearInfraredCH, FarInfraredCH, VisibleCH };
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("SEAICE", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            return result;
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
