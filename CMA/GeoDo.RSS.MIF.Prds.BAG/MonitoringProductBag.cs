using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductBag : MonitoringProduct
    {
        public MonitoringProductBag()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "BAG");
        }

        protected override void Init(GeoDo.RSS.MIF.Core.ProductDef productDef)
        {
            base.Init(productDef);
            _identify = productDef.Identify;
            SubProductDef subProductDef = productDef.GetSubProductDefByIdentify("DBLV");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBinaryBag(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("0CLM");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBinaryCLM(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("WTGS");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBinaryWTGS(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("BPCD");
            if (subProductDef != null)
                _subProducts.Add(new SubProductPixelCoverRate(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("BACD");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBAGCoverDegree(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("BCCA");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBagStatisticArea(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("BCDA");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBagStatAreaByDegree(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("BCDE");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBagStatAreaByIntensity(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("0IMG");
            if (subProductDef != null)
                _subProducts.Add(new SubProductIMGBAG(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("STAT");
            if (subProductDef != null)
                _subProducts.Add(new SubProductSTATBAG(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("FREQ");
            if (subProductDef != null)
                _subProducts.Add(new SubProductFREQBAG(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("BCDF");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBAGFreqByCoverDegree(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("BCAF");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBAGFreqByCoverArea(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("TFRV");
            if (subProductDef != null)
                _subProducts.Add(new SubProductTFRVBAG(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("THAN");
            if (subProductDef != null)
                _subProducts.Add(new SubProductTHANBAG(subProductDef));
            subProductDef = productDef.GetSubProductDefByIdentify("FREO");
            if (subProductDef != null)
                _subProducts.Add(new SubProductFREQBAGOld(subProductDef));
             subProductDef = productDef.GetSubProductDefByIdentify("BCDO");
            if (subProductDef != null)
                _subProducts.Add(new SubProductBAGOldFreqByCoverDegree(subProductDef));
        }
    }
}
