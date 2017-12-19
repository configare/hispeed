using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductBinaryFIRG : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinaryFIRG(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FireGroundAlgorithm")
            {
                return FIRGAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult FIRGAlgorithm()
        {
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IBandNameRaster bandNameRaster = prd as IBandNameRaster;
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NearInfrared");
            int FarInfraredCH = TryGetBandNo(bandNameRaster, "FarInfrared");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double FarInfraredZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");

            if (VisibleCH == -1 || FarInfraredCH == -1 || NearInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string express = string.Format(@"NDVI(band{1},band{0})  <= var_NDVIMax &&
                                             band{0}/" + VisibleZoom + @"f <= var_VisibleMax &&
                                             band{1}/" + NearInfraredZoom + @"f <= var_NearInfraredMax &&
                                             band{2}/" + FarInfraredZoom + @"f >= var_FarInfraredMin",
                                             VisibleCH, NearInfraredCH, FarInfraredCH);
            int[] bandNos = new int[] { VisibleCH, NearInfraredCH, FarInfraredCH };
            IThresholdExtracter<Int16> extracter = new SimpleThresholdExtracter<Int16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FIRE", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            result.Tag = new FirGFeatureCollection("火区辅助信息", GetDisplayInfo(VisibleCH, NearInfraredCH, FarInfraredCH));
            return result;
        }

        private Dictionary<int, FirGFeature> GetDisplayInfo(int VisibleCH, int NearInfraredCH, int FarInfraredCH)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            Dictionary<int, FirGFeature> features = new Dictionary<int, FirGFeature>();
            FirGFeature tempFirG = null;
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(_argumentProvider);
            rpVisitor.VisitPixel(new int[] { VisibleCH, NearInfraredCH, FarInfraredCH },
                (index, values) =>
                {
                    tempFirG = new FirGFeature();
                    tempFirG.Ndvi = (Int16)((values[1] - values[0]) * 1000f / (values[1] + values[0]));
                    tempFirG.Visible = values[0];
                    tempFirG.NearInfrared = values[1];
                    tempFirG.FarInfrared = values[2];
                    features.Add(index, tempFirG);
                }
            );
            return features;
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
