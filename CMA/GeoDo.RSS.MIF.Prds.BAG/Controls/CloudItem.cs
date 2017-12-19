using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.BAG
{
   public  class CloudItem
    {
       //可见光反射率最小值
       public double MinVisiable { get; set; }
       /// <summary>
       /// 近红外最小值
       /// </summary>
       public double MinNearInFrared { get; set; }
       /// <summary>
       /// 近红外/短波红外 最大值
       /// </summary>
       public double MaxNSVI { get; set; }
       /// <summary>
       /// NDVI 最小值
       /// </summary>
       public double MinNDVI { get; set; }
    }
}
