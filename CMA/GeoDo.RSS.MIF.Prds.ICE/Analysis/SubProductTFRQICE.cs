using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductTFRQICE : CmaMonitoringSubProduct
    {

        public SubProductTFRQICE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "TFRQAlgorithm")
            {
                return TFRQAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult TFRQAlgorithm(Action<int, string> progressTracker)
        {
            IRasterOperator<Int16> roper = new RasterOperator<Int16>();
            IInterestedRaster<Int16> timeResult = null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            RasterIdentify identify = new RasterIdentify(files);
            identify.SubProductIdentify = _subProductDef.Identify;
            int maxNum = 0;
            timeResult = roper.Times(files, identify, progressTracker, (dstValue, srcValue) =>
            {
                dstValue += srcValue;
                if (maxNum < dstValue)
                    maxNum = dstValue;
                return dstValue;
            });
            if (timeResult == null || maxNum == 0)
                return null;
            double zoom = (double)_argumentProvider.GetArg("Zoom");
            return CalcTreq(timeResult, identify, maxNum, zoom);
        }

        private IExtractResult CalcTreq(IInterestedRaster<Int16> timeResult, RasterIdentify id, int maxNum, double zoom)
        {
            IPixelFeatureMapper<Int16> memresult = new MemPixelFeatureMapper<Int16>("TFRQ", 1000, timeResult.Size, timeResult.CoordEnvelope, timeResult.SpatialRef);
            ArgumentProvider ap = new ArgumentProvider(timeResult.HostDataProvider, null);
            RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap);
            visitor.VisitPixel(new int[] { 1 },
                (index, values) =>
                {
                    if (values[0] == 0)
                        memresult.Put(index, 0);
                    else
                        memresult.Put(index, (Int16)(Math.Round(values[0] / (float)maxNum * zoom)));
                });
            timeResult.Dispose();
            return memresult;
        }
    }
}
