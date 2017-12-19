using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductBinarySMOK : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinarySMOK(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FireSmokeAlgorithm")
            {
                return SMOKAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult SMOKAlgorithm()
        {
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IBandNameRaster bandNameRaster = prd as IBandNameRaster;
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            int FarInfraredCH = TryGetBandNo(bandNameRaster, "FarInfrared");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double FarInfraredZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");

            if (VisibleCH == -1 || FarInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string express = string.Format(@"band{0}/" + VisibleZoom + @"f  >= var_VisibleMin && band{0}/" + VisibleZoom + @"f  < var_VisibleMax &&
                                             band{1}/" + FarInfraredZoom + @"f >= var_FarInfraredMin && band{1}/" + FarInfraredZoom + @"f < var_FarInfraredMax",
                                             VisibleCH, FarInfraredCH);
            int[] bandNos = new int[] { VisibleCH, FarInfraredCH };
            IThresholdExtracter<Int16> extracter = new SimpleThresholdExtracter<Int16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FIRE", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
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
