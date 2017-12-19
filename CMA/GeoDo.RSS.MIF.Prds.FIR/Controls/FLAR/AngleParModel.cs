using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 太阳耀斑角计算参数实体
    /// </summary>
   public  class AngleParModel
    {
       /// <summary>
       /// 太阳天顶角
       /// </summary>
       public string FileAsunZ { get; set; }
       /// <summary>
       /// 卫星天顶角
       /// </summary>
       public string FileAsatZ { get; set; }
       /// <summary>
       ///太阳方位角
       /// </summary>
       public string FileAsunA { get; set; }
       /// <summary>
       /// 太阳天顶角
       /// </summary>
       public string FileAsatA { get; set; }
      
    }
}
