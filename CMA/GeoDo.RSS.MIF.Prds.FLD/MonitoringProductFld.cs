using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductFld : MonitoringProduct
    {
        public MonitoringProductFld()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "FLD");
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);

            _identify = productDef.Identify;
            _name = productDef.Name;

            SubProductDef subDef = productDef.GetSubProductDefByIdentify("DBLV");
            if (subDef != null)
            {
                SubProductBinaryFld subProduct = new SubProductBinaryFld(subDef);
                _subProducts.Add(subProduct);
            }

            subDef = productDef.GetSubProductDefByIdentify("0CLM");
            if (subDef != null)
            {
                SubProductBinaryCLM binaryClm = new SubProductBinaryCLM(subDef);
                _subProducts.Add(binaryClm);
            }
            subDef = productDef.GetSubProductDefByIdentify("0MIX");
            if (subDef != null)
            {
                SubProductRasterFldMix rasterFldMix = new SubProductRasterFldMix(subDef);
                _subProducts.Add(rasterFldMix);
            }
            subDef = productDef.GetSubProductDefByIdentify("FLOD");
            if (subDef != null)
            {
                SubProductRasterFldFlood rasterFldFlood = new SubProductRasterFldFlood(subDef);
                _subProducts.Add(rasterFldFlood);
            }
            subDef = productDef.GetSubProductDefByIdentify("FLDC");
            if (subDef != null)
            {
                SubProductRasterFldFloodCount rasterFldFldc = new SubProductRasterFldFloodCount(subDef);
                _subProducts.Add(rasterFldFldc);
            }
            subDef = productDef.GetSubProductDefByIdentify("BHFX");
            if (subDef != null)
            {
                SubProductRasterFldMix rasterFldBhfx = new SubProductRasterFldMix(subDef);
                _subProducts.Add(rasterFldBhfx);
            }
            subDef = productDef.GetSubProductDefByIdentify("STAT");
            if (subDef != null)
            {
                SubProductAnalysisFldStat rasterFldStat = new SubProductAnalysisFldStat(subDef);
                _subProducts.Add(rasterFldStat);
            }
            subDef = productDef.GetSubProductDefByIdentify("FWAS");
            if (subDef != null)
            {
                SubProductAnalysisFlodAreaStat subProduct = new SubProductAnalysisFlodAreaStat(subDef);
                _subProducts.Add(subProduct);
            }
            subDef = productDef.GetSubProductDefByIdentify("FLLS");
            if (subDef != null)
            {
                SubProductAnalysisFldFlls rasterFldFlls = new SubProductAnalysisFldFlls(subDef);
                _subProducts.Add(rasterFldFlls);
            }
            subDef = productDef.GetSubProductDefByIdentify("0IMG");
            if (subDef != null)
            {
                SubProductLayoutFldImg rasterFldImg = new SubProductLayoutFldImg(subDef);
                _subProducts.Add(rasterFldImg);
            }
            subDef = productDef.GetSubProductDefByIdentify("FREQ");
            if (subDef != null)
            {
                SubProductAnalysisFldFreq rasterFldMtfd = new SubProductAnalysisFldFreq(subDef);
                _subProducts.Add(rasterFldMtfd);
            }
            subDef = productDef.GetSubProductDefByIdentify("CYCI");
            if (subDef != null)
            {
                SubProductCYCIFLD rasterFldMtfd = new SubProductCYCIFLD(subDef);
                _subProducts.Add(rasterFldMtfd);
            }
            subDef = productDef.GetSubProductDefByIdentify("TFRE");
            if (subDef != null)
            {
                SubProductTFREFLD rasterFldTFRE = new SubProductTFREFLD(subDef);
                _subProducts.Add(rasterFldTFRE);
            }
            subDef = productDef.GetSubProductDefByIdentify("TFRQ");
            if (subDef != null)
            {
                SubProductTFRQFLD rasterFldTFRQ = new SubProductTFRQFLD(subDef);
                _subProducts.Add(rasterFldTFRQ);
            }
            subDef = productDef.GetSubProductDefByIdentify("TFRI");
            if (subDef != null)
            {
                SubProductTFRIFLD rasterFldTFRI = new SubProductTFRIFLD(subDef);
                _subProducts.Add(rasterFldTFRI);
            }
            subDef = productDef.GetSubProductDefByIdentify("TFQI");
            if (subDef != null)
            {
                SubProductTFQIFLD rasterFldTFQI = new SubProductTFQIFLD(subDef);
                _subProducts.Add(rasterFldTFQI);
            }
            subDef = productDef.GetSubProductDefByIdentify("EDGE");
            if (subDef != null)
            {
                SubProductEDGEFLD rasterFldEDGE = new SubProductEDGEFLD(subDef);
                _subProducts.Add(rasterFldEDGE);
            }

            subDef = productDef.GetSubProductDefByIdentify("TSTA");
            if (subDef != null)
            {
                SubProductAnalysisFldTStat rasterFldTSTA = new SubProductAnalysisFldTStat(subDef);
                _subProducts.Add(rasterFldTSTA);
            }

            subDef = productDef.GetSubProductDefByIdentify("FLDS");
            if (subDef != null)
            {
                SubProductRasterFldFloodLastdays binaryFLDS = new SubProductRasterFldFloodLastdays(subDef);
                _subProducts.Add(binaryFLDS);
            }
        }
    }
}
