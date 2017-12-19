using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 某个要素层内存缓存网格数限制方式
    /// </summary>
    public enum enumGridLimitType
    {
        None,//不做限制(隐含最大可使用内存限制)
        Count,
        MemorySize
    }
}
