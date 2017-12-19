using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface IQueueTaskItem
    {
        string Name { get; }
        void Do(IProgressTracker tracker);
    }
}
