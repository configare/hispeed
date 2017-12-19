using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 光标信息显示窗口
    /// </summary>
    public interface ICursorInfoDisplayer
    {
        void RegisterProvider(ICursorInfoProvider infoProvider);
        void UnregisterProvider(ICursorInfoProvider infoProvider);
    }
}
