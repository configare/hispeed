using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ProductDef
    {
        /// <summary>
        /// 产品名称 ：“沙尘”
        /// </summary>
        public string Name;
        /// <summary>
        /// 产品唯一标识 ： “DST”
        /// </summary>
        public string Identify;
        //图标文件
        public string Image;
        public string UIProvider;
        //产品分组
        public string Group;

        public ThemeDef Theme;

        public AOITemplate[] AOITemplates;

        /// <summary>
        /// 子产品集合
        /// </summary>
        public SubProductDef[] SubProducts;

        public SubProductDef GetSubProductDefByIdentify(string identify)
        {
            if (SubProducts == null || SubProducts.Length == 0 || identify == null)
                return null;
            foreach (SubProductDef prd in SubProducts)
                if (prd.Identify != null && prd.Identify.ToUpper() == identify.ToUpper())
                    return prd;
            return null;
        }
    }
}
