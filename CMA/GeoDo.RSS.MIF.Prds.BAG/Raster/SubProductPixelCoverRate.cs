using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.DF.MEM;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductPixelCoverRate : CmaMonitoringSubProduct
    {
        public SubProductPixelCoverRate(SubProductDef productDef)
            : base(productDef)
        {
            _identify = productDef.Identify;
            _isBinary = false;
            _algorithmDefs = new List<AlgorithmDef>(productDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            string algname = _argumentProvider.GetArg("AlgorithmName").ToString();
            string ndviFName = _argumentProvider.GetArg("mainfiles").ToString();
            if (string.IsNullOrEmpty(algname) || string.IsNullOrEmpty(ndviFName) || !File.Exists(ndviFName))
                return null;
            if (algname == "BPCD")
            {
                //NDVI结果文件
                using (IRasterDataProvider ndviDataProvider = GeoDataDriver.Open(ndviFName) as IRasterDataProvider)
                {
                    NDVISetValue setValue = (ndviDataProvider as MemoryRasterDataProvider).GetExtHeader<NDVISetValue>();
                    double minNDVI = setValue.MinNDVI;
                    double maxNDVI = setValue.MaxNDVI;
                    double dst = maxNDVI - minNDVI;
                    IPixelFeatureMapper<float> memResult = new MemPixelFeatureMapper<float>("BPCD", 1000, new Size(ndviDataProvider.Width, ndviDataProvider.Height), ndviDataProvider.CoordEnvelope.Clone(), ndviDataProvider.SpatialRef);
                    ArgumentProvider ap = new ArgumentProvider(ndviDataProvider, null);
                    RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
                    visitor.VisitPixel(new int[] { 1 }, (index, values) =>
                    {
                        if (values[0] == -9999f)
                            memResult.Put(index, -9999);
                        else if (dst == 0)
                            memResult.Put(index, -9999);
                        else
                            memResult.Put(index, (float)((values[0] - minNDVI) / dst));
                    });
                    return memResult;
                }
            }
            return null;
        }
    }
}
