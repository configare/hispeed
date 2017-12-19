using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DST
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductDst : MonitoringProduct
    {
        public MonitoringProductDst()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "DST");
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);
            _identify = productDef.Identify;
            SubProductDef sub = productDef.GetSubProductDefByIdentify("DBLV");
            if (sub != null)
                _subProducts.Add(new SubProductBinaryDst(sub));

            sub = productDef.GetSubProductDefByIdentify("VISY");
            if (sub != null)
                _subProducts.Add(new SubProductRasterDst(sub));

            sub = productDef.GetSubProductDefByIdentify("STAT");
            if (sub != null)
                _subProducts.Add(new SubProductSTATArea(sub));

            sub = productDef.GetSubProductDefByIdentify("0IMG");
            if (sub != null)
                _subProducts.Add(new SubProductIMGDST(sub));

            sub = productDef.GetSubProductDefByIdentify("FREQ");
            if (sub != null)
                _subProducts.Add(new SubProductFREQDST(sub));

            sub = productDef.GetSubProductDefByIdentify("CYCI");
            if (sub != null)
                _subProducts.Add(new SubProductCYCIDST(sub));
            //
            sub = _productDef.GetSubProductDefByIdentify("0CLM");
            if (sub != null)
                if (!string.IsNullOrEmpty(sub.ToString()))
                    _subProducts.Add(new SubProductBinaryCLM(sub));
            //
            sub = productDef.GetSubProductDefByIdentify("QIPC");
            if (sub != null)
                _subProducts.Add(new SubProductQIPCDST(sub));
            sub = productDef.GetSubProductDefByIdentify("NCIM");
            if (sub != null)
                _subProducts.Add(new SubProductNCIMHDST(sub));
            sub = productDef.GetSubProductDefByIdentify("0MDS");
            if (sub != null)
                _subProducts.Add(new SubProduct0MDSDST(sub));

            sub = productDef.GetSubProductDefByIdentify("COMD");
            if (sub != null)
                _subProducts.Add(new SubProductCompDST(sub));
        }
    }
}
