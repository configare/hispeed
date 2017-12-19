using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.LST
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductLST : MonitoringProduct
    {
        public MonitoringProductLST()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "LST");
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);
            SubProductDef subLST = productDef.GetSubProductDefByIdentify("DBLV");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductBinaryLST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("0IMG");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductIMGLST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("CYCA");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductCYCALST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("STAT");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductSTATLST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("0MIN");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductMINLST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("0MAX");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductMAXLST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("0AVG");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductAVGLST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("ANMI");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductANMILST(subLST));

            subLST = productDef.GetSubProductDefByIdentify("CHAZ");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductCHAZLST(subLST));

            subLST = productDef.GetSubProductDefByIdentify("HFII");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductHFIILST(subLST));

            subLST = productDef.GetSubProductDefByIdentify("0CLM");
            if (subLST != null)
            {
                SubProductBinaryCLM binaryClm = new SubProductBinaryCLM(subLST);
                _subProducts.Add(binaryClm);
            }
            subLST = productDef.GetSubProductDefByIdentify("LMCZ");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductBinaryLMCZ(subLST));

            subLST = productDef.GetSubProductDefByIdentify("LTFR");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductLTFRLST(subLST));
            subLST = productDef.GetSubProductDefByIdentify("LTLR");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductLTLRLST(subLST));

            subLST = productDef.GetSubProductDefByIdentify("THAN");
            if (subLST != null)
                if (!string.IsNullOrEmpty(subLST.ToString()))
                    _subProducts.Add(new SubProductTHANLST(subLST));
        }
    }
}
