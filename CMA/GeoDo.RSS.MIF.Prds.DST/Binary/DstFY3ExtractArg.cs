#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/11/4 16:01:37
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

namespace GeoDo.RSS.MIF.Prds.DST
{
    /// <summary>
    /// 类名：DstFY3ExtractArg
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/11/4 16:01:37
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class DstFY3ExtractArg
    {
        public float[] TBB11;
        public float[] Ref650;
        public float[] TBB37;
        public float[] BTD11_12;
        public float[] BTD11_37;
        public float[] R47_64;
        public float[] NDVI;
        public float[] NDSI;
        public float[] NDDI;
        public float[] Ref470;
        public float[] IDSI;
        public float[] Ref1380;

        public DstFY3ExtractArg()
        {
            TBB11 = new float[4];
            Ref650 = new float[4];
            TBB37 = new float[4];
            BTD11_12= new float[4];
            BTD11_37= new float[4];
            R47_64 = new float[4];
            NDVI = new float[4];
            NDSI = new float[4];
            NDDI = new float[4];
            Ref470 = new float[4];
            IDSI = new float[4];
            Ref1380 = new float[4];
        }
    }
}
