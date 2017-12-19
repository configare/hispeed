using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductFOG : MonitoringProduct
    {
        public MonitoringProductFOG()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "FOG");
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            subproducts.Add("DBLV", typeof(SubProductBinaryFOG));
            subproducts.Add("0CSR", typeof(SubProductCSRFOG));
            subproducts.Add("0CLM", typeof(SubProductBinaryCLM));
            subproducts.Add("OPTD", typeof(SubProductOPTDFOG));
            subproducts.Add("0LWP", typeof(SubProductLWPFOG));
            subproducts.Add("ERAD", typeof(SubProductERADFOG));
            subproducts.Add("0TOU", typeof(SubProductTOUFOG));
            subproducts.Add("0IMG", typeof(SubProductIMGFOG));
            subproducts.Add("STAT", typeof(SubProductSTATFOG));
            subproducts.Add("FREQ", typeof(SubProductFREQFOG));
            subproducts.Add("CYCI", typeof(SubProductCYCIFOG));
            subproducts.Add("NCIM", typeof(SubProductNCIMFOG));
            return subproducts;
        }
    }
}
