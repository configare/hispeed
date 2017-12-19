using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductTFREFLD : CmaMonitoringSubProduct
    {

        public SubProductTFREFLD(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "TFREAlgorithm")
            {
                return TFREAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult TFREAlgorithm(Action<int, string> progressTracker)
        {
            IRasterOperator<Int16> roper = new RasterOperator<Int16>();
            IInterestedRaster<Int16> timeResult = null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            RasterIdentify identify = new RasterIdentify(files);
            identify.SubProductIdentify = _subProductDef.Identify;
            timeResult = roper.Times(files, identify, progressTracker, (dstValue, srcValue) =>
                        {
                            return (Int16)(dstValue + srcValue);
                        });
            if (timeResult == null)
                return null;
            timeResult.Dispose();
            string workFilename = identify.ToWksFullFileName(".dat");
            File.Copy(timeResult.FileName, workFilename,true);
            return new FileExtractResult("FLD", workFilename, true);
        }
    }
}
