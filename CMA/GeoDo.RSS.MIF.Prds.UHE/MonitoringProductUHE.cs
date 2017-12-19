using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.UHE
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductUHE : MonitoringProduct
    {
        public MonitoringProductUHE()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "UHE");
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);
            SubProductDef subUHE = productDef.GetSubProductDefByIdentify("LMCZ");
            if (subUHE != null)
                if (!string.IsNullOrEmpty(subUHE.ToString()))
                    _subProducts.Add(new SubProductBinaryLMCZ(subUHE));
            subUHE = productDef.GetSubProductDefByIdentify("0IMG");
            if (subUHE != null)
                if (!string.IsNullOrEmpty(subUHE.ToString()))
                    _subProducts.Add(new SubProductIMGUHE(subUHE));
            subUHE = productDef.GetSubProductDefByIdentify("STAT");
            if (subUHE != null)
                if (!string.IsNullOrEmpty(subUHE.ToString()))
                    _subProducts.Add(new SubProductSTATUHE(subUHE));

            subUHE = productDef.GetSubProductDefByIdentify("HFII");
            if (subUHE != null)
                if (!string.IsNullOrEmpty(subUHE.ToString()))
                    _subProducts.Add(new SubProductHFIILST(subUHE));
            subUHE = productDef.GetSubProductDefByIdentify("UHPI");
            if (subUHE != null)
                if (!string.IsNullOrEmpty(subUHE.ToString()))
                    _subProducts.Add(new SubProductUHPILST(subUHE));

            subUHE = productDef.GetSubProductDefByIdentify("0CLM");
            if (subUHE != null)
            {
                SubProductBinaryCLM binaryClm = new SubProductBinaryCLM(subUHE);
                _subProducts.Add(binaryClm);
            }
        }
    }
}
