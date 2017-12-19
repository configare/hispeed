using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    public interface ITask
    {
        string Name { get; set; }
        void Execute();
        void Reset();
        void AddAction(IAction action);
        bool VarIsEnough();
        void SetVarProvider(IVarProvider varProvider);
        void SetLog(ILog log);
        void SetTracker(IProgressTracker Tracker);
    }
}
