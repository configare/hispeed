#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-13 11:19:04
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
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;


namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：MonitoringProductVAL
    /// 属性描述：产品评估专题
    /// 创建者：lxj   创建日期：2014-6-13 11:19:04
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductVAL:CmaMonitoringProduct
    {
        public MonitoringProductVAL()
            : base()
        {
        }

        protected override ProductDef GetProductDef()
        {
            ProductDef pd = MonitoringThemeFactory.GetProductDef("CMA", "VAL");
            return pd;
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            subproducts.Add("PROD", typeof(SubProductDataProcess));
            subproducts.Add("SITE", typeof(SubProductSiteVal));
            //subproducts.Add("SATE", typeof(SubProductSatellitePrdsVal));
            subproducts.Add("VSST", typeof(SubProductValSST));
            subproducts.Add("VCLD", typeof(SubProductValCLD));
            subproducts.Add("VASL", typeof(SubProductValASL));
            subproducts.Add("VLST", typeof(SubProductValLST));
            return subproducts;
        }
       
    }
}
