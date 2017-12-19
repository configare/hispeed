using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductRasterFldMix_OLD : CmaMonitoringSubProduct
    {
        //混合象元计算
        public SubProductRasterFldMix_OLD(SubProductDef subProductDef)
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
            if (_argumentProvider == null||_argumentProvider.DataProvider==null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0MIX")
            {
                float fldValue = 0;
                float landValue = 0;
                string[] nearInfValues = _argumentProvider.GetArg("NearInfraredValues") as string[];
                if (nearInfValues == null || nearInfValues.Count() != 2)
                    return null;
                if (!float.TryParse(nearInfValues[0], out fldValue) || !float.TryParse(nearInfValues[1], out landValue))
                    return null;
                int bandNo = (int)_argumentProvider.GetArg("NearInfrared");
                double nearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                IRasterDataProvider prd=_argumentProvider.DataProvider;
                IPixelFeatureMapper<UInt16> result = new MemPixelFeatureMapper<UInt16>("0MIX", prd.Width * prd.Height, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
                ArgumentProvider ap = new ArgumentProvider(prd, null);
                RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap);
                if (fldValue == landValue)
                {
                    visitor.VisitPixel(new int[] { bandNo },
                    (index, values) =>
                    {
                        result.Put(index, 0);
                    });
                    return result;
                }
                visitor.VisitPixel(new int[] { bandNo },
                    (index, values) =>
                    {
                        UInt16 percentValue = (UInt16)GetMixPixelPercent(values[0], fldValue, landValue);
                        result.Put(index, percentValue);
                    });
                return result;
            }
            return null;
        }

        private Int16 GetMixPixelPercent(short pixelValue, float fldValue, float landValue)
        {
            Int16 value = (Int16)((pixelValue - landValue) / (fldValue - landValue) * 100);
            if (value < 0)
                value = 0;
            if (value > 100)
                value = 100;
            return value;
        }
    }
}
