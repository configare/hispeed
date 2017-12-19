#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：zhangyb     时间：2014-2-10 09:24:36
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：MonitoringProductMWS
    /// 属性描述：微波积雪监测分析专题产品
    /// 创建者：zhangyb     时间：2014-2-10 09:24:36
    /// 修改者：Lixj           修改日期：2014-2-11
    /// 修改描述：增加子产品
    /// 备注：
    /// </summary>
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductMWS : CmaMonitoringProduct
    {
         public MonitoringProductMWS()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            ProductDef pd = MonitoringThemeFactory.GetProductDef("CMA", "MWS");
            return pd;
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            subproducts.Add("DBLV", typeof(SubProductBinarySnw));
            subproducts.Add("0IMG", typeof(SubProductIMGSNW));
            subproducts.Add("0SSD", typeof(SubProductRasterSNWSD));
            subproducts.Add("ASTA", typeof(SubProductAreaSTATSNW));
            subproducts.Add("VSTA", typeof(SubProductSweVolSTAT));
            subproducts.Add("MWVI", typeof(SubProductRasterSNMWVI));
            subproducts.Add("SDWE", typeof(SubProductCustomIMG));
            subproducts.Add("ATSD", typeof(SubProductAutoSD));
            subproducts.Add("0PSI", typeof(SubProduct0PSISNW));
            subproducts.Add("JPAL", typeof(SubProductJuPingAnalysis));
            subproducts.Add("JPST", typeof(SubProductJPStat));
            subproducts.Add("HIST", typeof(SubProductHisDataIMG));
            //subproducts.Add("HSTA", typeof(SubProductHisDataSat));
            subproducts.Add("TSAN", typeof(SubProductTimeSeriesAnalysis));
            subproducts.Add("HSTA", typeof(SubProductTimeSeriesDataSat));
            return subproducts;
        }

    }
}
