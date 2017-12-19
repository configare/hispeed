using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductSnw : CmaMonitoringProduct
    {
        public MonitoringProductSnw()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            ProductDef pd = MonitoringThemeFactory.GetProductDef("CMA", "SNW");
            return pd;
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            subproducts.Add("DBLV", typeof(SubProductBinarySnw));
            subproducts.Add("COMP", typeof(SubProductCOMPSNW));
            subproducts.Add("0IMG", typeof(SubProductIMGSNW));
            subproducts.Add("STAT", typeof(SubProductSTATSNW));
            subproducts.Add("FREQ", typeof(SubProductFREQSNW));
            subproducts.Add("NIMG", typeof(SubProductNIMGSNW));
            subproducts.Add("0SDC", typeof(SubProductSNWDegree));
            subproducts.Add("MAXI", typeof(SubProductDayMaxSNW));
            subproducts.Add("0CLM", typeof(SubProductBinaryCLM));
            subproducts.Add("COMA", typeof(SubProductCompStatSNW));
            subproducts.Add("0SSD", typeof(SubProductRasterSNWSD));
            subproducts.Add("0PSI", typeof(SubProduct0PSISNW));
            subproducts.Add("FRDS", typeof(SubProductFRDSSNW));
            subproducts.Add("TSTA", typeof(SubProductFRDSSNW));
           
            return subproducts;
        }
    }
}
