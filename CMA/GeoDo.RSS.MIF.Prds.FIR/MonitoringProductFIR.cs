using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductFIR : MonitoringProduct
    {
        public MonitoringProductFIR()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "FIR");
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);
            _identify = productDef.Identify;
            //
            LoadSubProducts();
        }

        private void LoadSubProducts()
        {
            SubProductDef def = _productDef.GetSubProductDefByIdentify("DBLV");
            if (def != null)
                _subProducts.Add(new SubProductBinaryFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FIRG");
            if (def != null)
                _subProducts.Add(new SubProductBinaryFIRG(def));
            //
            def = _productDef.GetSubProductDefByIdentify("SMOK");
            if (def != null)
                _subProducts.Add(new SubProductBinarySMOK(def));
            //
            def = _productDef.GetSubProductDefByIdentify("0CLM");
            if (def != null)
                if (!string.IsNullOrEmpty(def.ToString()))
                    _subProducts.Add(new SubProductBinaryCLM(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FRIL");
            if (def != null)
                if (!string.IsNullOrEmpty(def.ToString()))
                    _subProducts.Add(new SubProductFireInfoList(def));
            // by chennan 20120813
            def = _productDef.GetSubProductDefByIdentify("0IMG");
            if (def != null)
                _subProducts.Add(new SubProductIMGFIR(def));

            def = _productDef.GetSubProductDefByIdentify("STAT");
            if (def != null)
                _subProducts.Add(new SubProductSTATFIR(def));

            def = _productDef.GetSubProductDefByIdentify("FREQ");
            if (def != null)
                _subProducts.Add(new SubProductFREQFIR(def));
            //
             def = _productDef.GetSubProductDefByIdentify("COMP");
            if (def != null)
                _subProducts.Add(new SubProductCOMPFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("GFRI");
            if (def != null)
                _subProducts.Add(new SubProductGFRIFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("GFRF");
            if (def != null)
                _subProducts.Add(new SubProductGFRFFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("GFCF");
            if (def != null)
                _subProducts.Add(new SubProductGFCFFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FIRF");
            if (def != null)
                _subProducts.Add(new SubProductFIRFFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FSTA");
            if (def != null)
                _subProducts.Add(new SubProductFSTAFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FLMX");
            if (def != null)
                _subProducts.Add(new SubProductFLMXFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FLMI");
            if (def != null)
                _subProducts.Add(new SubProductFLMIFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FLAV");
            if (def != null)
                _subProducts.Add(new SubProductFLAVFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FLAN");
            if (def != null)
                _subProducts.Add(new SubProductFLANFIR(def));
            //
            def = _productDef.GetSubProductDefByIdentify("FLCY");
            if (def != null)
                _subProducts.Add(new SubProductFLCYFIR(def));
            def = _productDef.GetSubProductDefByIdentify("FLAR");
            if (def != null)
                _subProducts.Add(new SubProductFLARFIR(def));

        }
    }
}
