using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductDRT : MonitoringProduct
    {
        public MonitoringProductDRT()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "DRT");
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);
            SubProductDef subDrt = productDef.GetSubProductDefByIdentify("0VTI");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductVTIDRTII(subDrt));
             subDrt = productDef.GetSubProductDefByIdentify("0VTIA");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductTCI(subDrt));
            //_subProducts.Add(new SubProductVTIDRT(subDrt));
            //NDVI数据背景库生产
            subDrt = productDef.GetSubProductDefByIdentify("NBGP");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new  SubProductNDVIBackFileDRT(subDrt));
            //亮温数据背景库生产
            subDrt = productDef.GetSubProductDefByIdentify("LBGP");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductLBBackFileDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("0SWI");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductSWIDRTII(subDrt));
            // _subProducts.Add(new SubProductSWIDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("0DNT");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductDNTDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("TVDI");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductTVDIDRTII(subDrt));
            //_subProducts.Add(new SubProductTVDIDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("VSWI");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductVSWIDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("0IMG");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductIMGDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("STAT");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductSTATDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("0PDI");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductPDIDRT(subDrt));

            subDrt = productDef.GetSubProductDefByIdentify("MPDI");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductMPDIDRT(subDrt));

            subDrt = _productDef.GetSubProductDefByIdentify("0CLM");
            if (subDrt != null)
                if (!string.IsNullOrEmpty(subDrt.ToString()))
                    _subProducts.Add(new SubProductBinaryCLM(subDrt));

        }
    }
}
