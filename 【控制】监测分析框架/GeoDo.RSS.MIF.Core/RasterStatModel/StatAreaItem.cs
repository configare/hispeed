using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class StatAreaItem
    {
        public StatAreaItem()
        { }

        public StatAreaItem(double cover, double grandTotal)
        {
            Cover = cover;
            GrandTotal = grandTotal;
        }

        /// <summary>
        /// 最大覆盖
        /// </summary>
        public double Cover;
        /// <summary>
        /// 累计
        /// </summary>
        public double GrandTotal;
        //public int 
    }
}
