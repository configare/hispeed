using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductICE : MonitoringProduct
    {
        public MonitoringProductICE()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "ICE");
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            subproducts.Add("DBLV", typeof(SubProductBinaryICE));
            subproducts.Add("0IMG", typeof(SubProductIMGICE));
            subproducts.Add("STAT", typeof(SubProductSTATICE));
            subproducts.Add("FREQ", typeof(SubProductFREQICE));
            subproducts.Add("CYCI", typeof(SubProductCYCIICE));
            subproducts.Add("ISOT", typeof(SubProductISOTICE));
            subproducts.Add("ISOI", typeof(SubProductISOIICE));
            subproducts.Add("AEDG", typeof(SubProductAEDGICE));
            subproducts.Add("0CLM", typeof(SubProductBinaryCLM));
            subproducts.Add("EDGE", typeof(SubProductIEDGICE));
            subproducts.Add("EDGI", typeof(SubProductEDGIICE));
            subproducts.Add("DEGR", typeof(SubProductDEGRICE));
            subproducts.Add("0SIT", typeof(SubProduct0SITICE));
            subproducts.Add("SITI", typeof(SubProductSITIICE));
            subproducts.Add("SIAI", typeof(SubProductSIAIICE));
            subproducts.Add("PICI", typeof(SubProductPICIICE));
            subproducts.Add("COMP", typeof(SubProductCOMPICE));
            subproducts.Add("COMA", typeof(SubProductCompStatICE));
            subproducts.Add("TFRE", typeof(SubProductTFREICE));
            subproducts.Add("TFRQ", typeof(SubProductTFRQICE));
            subproducts.Add("TFRI", typeof(SubProductTFRIICE));
            subproducts.Add("TFQI", typeof(SubProductTFQIICE));
            return subproducts;
        }
    }
}
