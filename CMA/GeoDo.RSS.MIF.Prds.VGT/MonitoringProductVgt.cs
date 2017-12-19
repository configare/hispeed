using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductVgt : MonitoringProduct
    {
        public MonitoringProductVgt()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "VGT");
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            //SubProductRasterVgtNdviSingleFile rasterVgtNdvi = new SubProductRasterVgtNdviSingleFile(ndviRef); //基于当前栅格文件的ndvi计算
            subproducts.Add("NDVI", typeof(SubProductRasterVgtNdvi));//基于文件夹的ndvi计算：指定文件夹路径
            subproducts.Add("0RVI", typeof(SubProductRasterVgtRvi));
            subproducts.Add("0DVI", typeof(SubProductRasterVgtDvi));
            subproducts.Add("0EVI", typeof(SubProductRasterVgtEvi));
            subproducts.Add("0VCI", typeof(SubProductRasterVgtVci));
            subproducts.Add("0MAX", typeof(SubProductRasterVgtMaxNdvi));
            subproducts.Add("0MIN", typeof(SubProductRasterVgtMinNdvi));
            subproducts.Add("0AVG", typeof(SubProductRasterVgtAvgNdvi));
            subproducts.Add("ANMI", typeof(SubProductRasterVgtAnomalies));
            subproducts.Add("CHAZ", typeof(SubProductRasterVgtDifference));
            subproducts.Add("CYCA", typeof(SubProductRasterVgtCYCA));
            subproducts.Add("0IMG", typeof(SubProductLayoutVgtImg));
            subproducts.Add("VTAT", typeof(SubProductAnalysisVgtNdvi));
            subproducts.Add("0CLM", typeof(SubProductBinaryCLM));
            subproducts.Add("AVGS", typeof(SubProductAvgStatic));
            subproducts.Add("0LAI", typeof(SubProductVgtMersiLAI));//基于hdf文件的叶面积指数LAI计算；
            return subproducts;
        }

        protected override void Init(ProductDef productDef)
        {
            base.Init(productDef);
            _identify = productDef.Identify;
            _name = productDef.Name;
        }
    }
}
