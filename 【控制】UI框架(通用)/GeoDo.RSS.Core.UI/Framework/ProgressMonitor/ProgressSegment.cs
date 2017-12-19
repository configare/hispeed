using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 进度条的分段定义
    /// </summary>
    public struct ProgressSegment
    {
        /// <summary>
        /// 时间刻度长度
        /// </summary>
        public int Length;
        /// <summary>
        /// 段提示
        /// </summary>
        public string Text;
    }
}
