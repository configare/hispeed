using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductHAE : MonitoringProduct
    {
        public MonitoringProductHAE()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "HAZ");
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            subproducts.Add("DBLV", typeof(SubProductBinaryHAZ));
            subproducts.Add("0CLM", typeof(SubProductBinaryCLM));
            subproducts.Add("HAZE", typeof(SubProductTOUHAZE));
            subproducts.Add("NCIM", typeof(SubProductNCIMHAZE));
            subproducts.Add("0IMG", typeof(SubProductIMGHAE));
            subproducts.Add("OAIM", typeof(SubProductOAIMHAZE));
            subproducts.Add("RFIM", typeof(SubProductRFIMHAZE));
            subproducts.Add("0LEV", typeof(SubProductLEVELHAZE));
            subproducts.Add("TFRE", typeof(SubProductTFREHAZE));
            subproducts.Add("LAOD", typeof(SubProductLAODHAZE));
            return subproducts;
        }
    }
}
