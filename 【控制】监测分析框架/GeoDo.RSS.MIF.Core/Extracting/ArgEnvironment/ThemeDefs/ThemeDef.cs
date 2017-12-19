using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ThemeDef
    {
        /// <summary>
        /// 专题名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 专题唯一标识 ： “CMA” 气象专题
        /// </summary>
        public string Identify;
        public string ProductDir;
        /// <summary>
        /// 产品集合
        /// </summary>
        public ProductDef[] Products;

        /// <summary>
        /// 系统级AOI
        /// </summary>
        public AOIDef[] AOIDefs;
        public AOIDef DefaultAOIDef;

        public ProductDef GetProductDefByIdentify(string identify)
        {
            if (Products == null || Products.Length == 0 || identify == null)
                return null;
            foreach (ProductDef prd in Products)
                if (prd.Identify != null && prd.Identify.ToUpper() == identify.ToUpper())
                    return prd;
            return null;
        }
    }
}
