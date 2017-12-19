using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IPerformanceWatch
    {
        void BeginWatch(string identify);
        void EndWatch(string identify);
    }
}
