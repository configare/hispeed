using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface IUIProvider
    {
        object Content { get; }
        void Init(ISmartSession session, params object[] arguments);
        void UpdateStatus();
    }
}
