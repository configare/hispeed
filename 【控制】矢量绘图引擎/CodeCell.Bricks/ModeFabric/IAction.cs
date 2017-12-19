using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    public interface IAction:IDisposable
    {
        Guid Guid { get; }
        string Name { get; set; }
        bool Do(IContextMessage contextMessage);
        void Reset();
        void SetLog(ILog log);
        void SetTracker(IProgressTracker tracker);
    }
}
