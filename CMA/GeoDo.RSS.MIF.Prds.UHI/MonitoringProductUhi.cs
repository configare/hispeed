using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductUhi : MonitoringProduct
    {
        public MonitoringProductUhi()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "UHI");
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);
            _identify = productDef.Identify;
            SubProductDef spd = productDef.GetSubProductDefByIdentify("DBLV");
            if (spd != null)
                if (!string.IsNullOrEmpty(spd.ToString()))
                    _subProducts.Add(new SubProductBinaryLST(spd));
            spd = productDef.GetSubProductDefByIdentify("0IMG");
            if (spd != null)
                if (!string.IsNullOrEmpty(spd.ToString()))
                    _subProducts.Add(new SubProductIMGUHI(spd));
            //
            spd = _productDef.GetSubProductDefByIdentify("0CLM");
            if (spd != null)
                if (!string.IsNullOrEmpty(spd.ToString()))
                    _subProducts.Add(new SubProductBinaryCLM(spd));
            //
            spd = _productDef.GetSubProductDefByIdentify("CMAI");
            if (spd != null)
                if (!string.IsNullOrEmpty(spd.ToString()))
                    _subProducts.Add(new SubProductCIMGUHI(spd));
        }
    }
}
