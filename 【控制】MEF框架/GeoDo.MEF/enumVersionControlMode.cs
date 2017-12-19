using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.MEF
{
    public enum enumVersionControlMode
    {
        /// <summary>
        /// 类型名称(类型+名字空间)必须完全匹配
        /// </summary>
        FullTypeMatch,
        /// <summary>
        /// 类型名称(不检查名字空间)必须完全匹配
        /// </summary>
        FullNameMatch,
        /// <summary>
        /// 不进行版本控制,可重复加载
        /// </summary>
        AllowRepeat
    }
}
