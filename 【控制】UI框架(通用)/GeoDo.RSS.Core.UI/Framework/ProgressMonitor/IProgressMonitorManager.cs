using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 进度条管理对象
    /// </summary>
    public interface IProgressMonitorManager
    {
        /// <summary>
        /// 默认的进度条
        /// </summary>
        IProgressMonitor DefaultProgressMonitor { get; }
        /// <summary>
        /// 新建一个进度条
        /// 对于新建的进度条IProgressMonitor.Finish()操作将关闭进度条
        /// </summary>
        /// <returns></returns>
        IProgressMonitor NewProgressMonintor();
    }
}
