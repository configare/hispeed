#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2013-11-1 8:53:33
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

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SnwFeatureMS
    /// 属性描述：记录微波积雪判识辅助信息
    /// 创建者：李喜佳   创建日期：2013-11-1 8:53:33
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
  public class SnwFeatureMS
    {
        public double si1;
        public double si2;
        public double ch23v;
        public double si22;
        public double si1si2;
        public double si1si22;

        private const string INFO_FORMAT =
          "第一层判识条件:{0}\n" +
          "第一层判识条件:{1}\n" +
          "第二层判识条件:{2}\n" +
          "第三层判识条件:{3}\n" +
          "厚雪判识:{4}\n"+
          "薄雪判识{5}\n";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                si1,
                si2,
                ch23v,
                si22,
                si1si2,
                si1si22 );
        }
    }
}
