using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 工具窗口
    /// </summary>
    public interface ISmartToolWindow:ISmartWindow
    {
        int Id { get; }
        void Init(ISmartSession session, params object[] arguments);
    }
}
