#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-10 18:43:17
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
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 接口名称：ISelectedPointLayer
    /// 创建者：罗战克
    /// 日期：2013-09-10 18:43:17
    /// 功能：
    /// 修改记录：
    /// </summary>
    interface ISelectMaskLayer
    {
        /// <summary>
        /// 当前标记的点
        /// </summary>
        Dictionary<int, CoordPoint> MaskPoints { get; }
        void Add(Dictionary<int, CoordPoint> points);
        void Remove(Dictionary<int, CoordPoint> points);
        void Clear();
    }
}
