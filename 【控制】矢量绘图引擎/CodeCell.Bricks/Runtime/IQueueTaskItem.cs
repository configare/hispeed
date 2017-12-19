using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.Runtime
{
    public interface IQueueTaskItem
    {
        string Name { get; }
        void Do(IProgressTracker tracker);
    }
}
