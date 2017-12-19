using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 业务上下文视图(eg:专题制图、监测分析、脚本开发、图像浏览)
    /// </summary>
    public interface IOperationContextView:IDisposable
    {
        string Name { get; }
        void Register(ISmartWindow window);
        void Close();
    }
}
